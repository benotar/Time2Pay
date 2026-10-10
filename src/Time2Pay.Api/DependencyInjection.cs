using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Time2Pay.Api.Database;
using Time2Pay.Api.Entities;

namespace Time2Pay.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<Time2PayDbContext>((sp, options) =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            var environment = sp.GetRequiredService<IHostEnvironment>();

            options.UseNpgsql(configuration.GetConnectionString(Constants.DatabaseSection),
                    npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                        Constants.ApplicationSchema))
                .UseSnakeCaseNamingConvention();

            if (!environment.IsDevelopment())
            {
                return;
            }

            const string employmentName = "BenotaarInc";

            options.UseSeeding((dbContext, _) =>
                {
                    var employmentsDbSet = GetDbSet<Employment>(dbContext);
                    var existingEmployment = GetTestEmployment(employmentsDbSet, false, CancellationToken.None)
                        .GetAwaiter()
                        .GetResult();

                    if (existingEmployment is not null)
                    {
                        return;
                    }

                    employmentsDbSet.Add(CreateEmployment());
                    dbContext.SaveChanges();
                })
                .UseAsyncSeeding(async (dbContext, _, cancellationToken) =>
                {
                    var employmentsDbSet = GetDbSet<Employment>(dbContext);
                    var existingEmployment = await GetTestEmployment(employmentsDbSet, true, cancellationToken);

                    if (existingEmployment is not null)
                    {
                        return;
                    }

                    employmentsDbSet.Add(CreateEmployment());
                    await dbContext.SaveChangesAsync(cancellationToken);
                });

            return;

            static Employment CreateEmployment()
            {
                return new Employment
                {
                    Id = $"{Constants.EmploymentPrefix}{Guid.CreateVersion7()}",
                    Name = employmentName,
                    Description = "Test Employment for seeding",
                    StartDate = new DateOnly(2026, 10, 4),
                    EndDate = null,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    LastModifiedAtUtc = null
                };
            }

            static DbSet<TEntity> GetDbSet<TEntity>(DbContext dbContext) where TEntity : class
            {
                return dbContext.Set<TEntity>();
            }

            static Task<Employment?> GetTestEmployment(DbSet<Employment> employments, bool useAsync,
                CancellationToken cancellationToken)
            {
                Expression<Func<Employment, bool>> namePredicate = employment => employment.Name.Equals(employmentName);
                return useAsync
                    ? employments.FirstOrDefaultAsync(namePredicate, cancellationToken)
                    : Task.FromResult(employments.FirstOrDefault(namePredicate));
            }
        });

        return services;
    }
}
