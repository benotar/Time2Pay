# Time2Pay: database schema

Tracks hours worked, accrued earnings and actual payments. PostgreSQL, schema `time_2_pay`.

The ER diagram source is in [database.eraser](database.eraser) (paste it into an Eraser Entity Relationship diagram).

**Core principle:** the database stores only facts entered by people (when they worked, what the rate is, how much was
paid) and reference data (exchange rates). Everything derived — hours per period, accrued earnings, balance owed, cost
per hour — is **calculated**, not stored.

## users

An application user, implemented via ASP.NET Core Identity (`ApplicationUser : IdentityUser<Guid>`). Email, password and
other sign-in data are stored by Identity.

| Field                                     | Description                                |
|-------------------------------------------|--------------------------------------------|
| `id`                                      | User identifier (from Identity)            |
| `display_name`                            | Name shown in the app                      |
| `created_at_utc` / `last_modified_at_utc` | When the row was created and last modified |

## employments

A user's job. One user can have several jobs.

| Field                                     | Description                                  |
|-------------------------------------------|----------------------------------------------|
| `id`                                      | Job identifier                               |
| `user_id`                                 | Owner of the job → `users.id`                |
| `name`                                    | Display name to tell jobs apart              |
| `description`                             | Description. `NULL`                          |
| `start_date`                              | Employment start date                        |
| `end_date`                                | Employment end date. `NULL` = still employed |
| `created_at_utc` / `last_modified_at_utc` | Audit                                        |

## compensation_rates

A job's pay rate with change history. When the rate changes, the current row gets a `valid_to_date` and a new row is
created, so past periods are always calculated at the rate that applied at the time.

| Field                                     | Description                                             |
|-------------------------------------------|---------------------------------------------------------|
| `id`                                      | Identifier                                              |
| `employment_id`                           | Job this rate applies to → `employments.id`             |
| `rate_type`                               | `hourly` — per hour, `monthly` — fixed amount per month |
| `amount`                                  | Rate amount, `numeric(12,2)`                            |
| `currency`                                | Rate currency, `UAH` or `USD`                           |
| `valid_from_date`                         | Date the rate takes effect                              |
| `valid_to_date`                           | Last date the rate applied. `NULL` = still in effect    |
| `created_at_utc` / `last_modified_at_utc` | Audit                                                   |

**Constraints:** `valid_from_date`…`valid_to_date` ranges must not overlap for the same job.

## work_days

An actual work day: when the person started, when they finished, and how long the break was. One row per job per date.

| Field                                     | Description                                                                     |
|-------------------------------------------|---------------------------------------------------------------------------------|
| `id`                                      | Identifier                                                                      |
| `employment_id`                           | Job → `employments.id`                                                          |
| `date`                                    | Date picked by the person, in the app time zone (`Europe/Kyiv`)                 |
| `day_type`                                | `work`, `vacation`, `sick_leave`, `day_off`                                     |
| `started_at`                              | Actual start time. `NULL` if no work that day                                   |
| `ended_at`                                | Actual end time. `NULL` if no work that day                                     |
| `break_minutes`                           | Unpaid break in minutes. `0` if there is no break                               |
| `duration_minutes`                        | Paid minutes. Computed by the database: `ended_at − started_at − break_minutes` |
| `comment`                                 | Free-text note                                                                  |
| `created_at_utc` / `last_modified_at_utc` | Audit                                                                           |

**Constraints:**

- `employment_id` + `date` is unique;
- a `work` day has `started_at` and `ended_at`; other day types do not;
- `ended_at` is later than `started_at`; `break_minutes` ≥ 0.

## payments

An actual payment: every receipt of money is a separate row.

| Field                                     | Description                                                                      |
|-------------------------------------------|----------------------------------------------------------------------------------|
| `id`                                      | Identifier                                                                       |
| `employment_id`                           | Paying job → `employments.id`                                                    |
| `paid_date`                               | Date the money was received                                                      |
| `amount`                                  | Amount, `numeric(12,2)`                                                          |
| `currency`                                | Payment currency                                                                 |
| `payment_method`                          | `cash` or `card`                                                                 |
| `period_start_date` / `period_end_date`   | Work period the payment covers. Required. May not match a calendar week or month |
| `comment`                                 | Free-text note                                                                   |
| `created_at_utc` / `last_modified_at_utc` | Audit                                                                            |

**Constraints:** `period_start_date` and `period_end_date` are `NOT NULL`; `period_end_date` ≥ `period_start_date`.

## exchange_rates

Monthly average USD → UAH exchange rate from the National Bank of Ukraine (NBU). Stored locally so reports don't depend
on NBU availability.

| Field                                     | Description                                                                      |
|-------------------------------------------|----------------------------------------------------------------------------------|
| `month`                                   | Month, stored as its first day: `2026-09-01`. Primary key, so one rate per month |
| `rate`                                    | Exchange rate, `numeric(10,4)`                                                   |
| `created_at_utc` / `last_modified_at_utc` | Audit                                                                            |

## exchange_rate_caps

Maximum USD → UAH exchange rate set in the Ukrainian state budget. Changes at most once a year.

| Field                                     | Description                                         |
|-------------------------------------------|-----------------------------------------------------|
| `id`                                      | Identifier                                          |
| `rate`                                    | Cap rate, `numeric(10,4)`                           |
| `valid_from`                              | Date the cap takes effect                           |
| `valid_to`                                | Last date the cap applied. `NULL` = still in effect |
| `created_at_utc` / `last_modified_at_utc` | Audit                                               |

**Constraints:** `valid_from`…`valid_to` ranges must not overlap.

## calendar

Date dimension for reports and BI tools: one row per day. Generated once in a migration and never edited by hand.

| Field               | Description                                                                   |
|---------------------|-------------------------------------------------------------------------------|
| `date`              | Date, primary key                                                             |
| `year`              | Year                                                                          |
| `half_year`         | Half-year: 1 or 2                                                             |
| `quarter`           | Quarter: 1–4                                                                  |
| `month`             | Month: 1–12                                                                   |
| `iso_year`          | Year the ISO week belongs to                                                  |
| `iso_week`          | ISO week number                                                               |
| `iso_weekday`       | Day of week: 1 = Mon … 7 = Sun                                                |
| `is_weekend`        | Saturday or Sunday                                                            |
| `is_public_holiday` | Public holiday. Filled in separately, since it can't be derived from the date |

## Calculations

**Hours worked for a period:** sum of `work_days.duration_minutes` over the dates in the period.

**Accrued earnings:**

- hourly rate: hours worked × the rate in effect on the day of work;
- monthly rate in USD: rate × the lower of (`exchange_rates.rate` for that month, the `exchange_rate_caps.rate` in
  effect that month).

**Cash due / balance owed:**

```
total accrued since the start − total payments since the start
```

Positive: the employer owes money. Negative: an overpayment, which is deducted from the next payment.

**Cost per hour:** amount paid for the period ÷ hours worked in that period.
