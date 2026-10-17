# Time2Pay

Time2Pay tracks hours worked, accrued earnings and actual payments for one or more jobs.
It stores only facts entered by people (work days, pay rates, payments) and reference data
(exchange rates); everything derived — hours per period, accrued earnings, balance owed — is calculated.

> Work in progress. The `employments` resource is implemented; work days, compensation rates
> and payments are next.

## Tech stack

- .NET 10, ASP.NET Core (controllers)
- EF Core 10 with PostgreSQL 18 (Npgsql), snake_case naming
- OpenTelemetry → .NET Aspire Dashboard
- Docker Compose for local infrastructure
- GitHub Actions for CI

## Architecture

N-tier, one project per layer. Dependencies point downwards only.

```
Time2Pay.API  →  Time2Pay.BLL  →  Time2Pay.DAL
                      ↓
                 Time2Pay.DTO
```

| Project | Responsibility |
|---|---|
| `Time2Pay.API` | Controllers, HTTP concerns, mapping service errors to ProblemDetails |
| `Time2Pay.BLL` | Services, business rules, Result pattern, entity ↔ DTO mapping |
| `Time2Pay.DAL` | `Time2PayDbContext`, entities, EF configurations, migrations, interceptors |
| `Time2Pay.DTO` | Request and response contracts |

Services return `Result` / `Result<T>` for expected failures (not found, conflict, validation);
the API turns them into ProblemDetails responses.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- EF Core CLI: `dotnet tool install -g dotnet-ef`

### 1. Start the infrastructure

```powershell
.\scripts\dev-infra.ps1 up
```

This starts PostgreSQL and the Aspire Dashboard. Other actions: `down`, `restart`, `logs`.

### 2. Run the API

```powershell
dotnet run --project src/Time2Pay.API --launch-profile https
```

In Development the API applies migrations and seeds test data on startup.

| Service | URL |
|---|---|
| API | http://localhost:5228, https://localhost:5229 |
| OpenAPI document | https://localhost:5229/openapi/v1.json |
| Aspire Dashboard | http://localhost:18888 |
| PostgreSQL | localhost:5432 (`postgres` / `postgres`, database `time2pay`) |

## API

| Method | Route | Success |
|---|---|---|
| `GET` | `/employments` | `200 OK` |
| `GET` | `/employments/{id}` | `200 OK` |
| `POST` | `/employments` | `201 Created` |
| `PUT` | `/employments/{id}` | `204 No Content` |
| `DELETE` | `/employments/{id}` | `204 No Content` |

Errors are returned as [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457).
Sample requests are in `src/Time2Pay.API/Time2Pay.API.http`.

## Database migrations

`scripts/ef.ps1` wraps `dotnet ef` and fills in the project, startup project, context
and output folder:

```powershell
.\scripts\ef.ps1 Time2Pay migrations add <MigrationName>
.\scripts\ef.ps1 Time2Pay database update
```

Any other `dotnet ef` arguments are passed through as is.

## Documentation

- [Database schema](docs/database.md), with the ER diagram source in `docs/database.eraser`

## CI

The `build` workflow restores, builds, tests and publishes the solution on every push.
