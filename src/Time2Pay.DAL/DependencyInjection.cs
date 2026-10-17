using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Time2Pay.DAL.Entities;
using Time2Pay.DAL.Interceptors;
using Time2Pay.DAL.Providers;
using Time2Pay.DAL.Providers.Interfaces;
using Time2Pay.DAL.Utils;

namespace Time2Pay.DAL;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDatabase(bool isDevelopment)
        {
            services.AddDbContext<Time2PayDbContext>((sp, options) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

                var dbContextOptionsBuilder = options.UseNpgsql(configuration.GetConnectionString(DalConstants.DatabaseSection),
                        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                            DalConstants.ApplicationSchema))
                    .UseSnakeCaseNamingConvention()
                    .EnableDetailedErrors();

                if (!isDevelopment)
                {
                    return;
                }

                dbContextOptionsBuilder.EnableSensitiveDataLogging();

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

                        var employment = CreateEmployment();

                        employmentsDbSet.Add(employment);
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

                static Employment CreateEmployment() => new Employment
                {
                    Id = IdGenerator.PrefixId<Employment>(),
                    Name = employmentName,
                    Description = "Test Employment for seeding",
                    StartDate = new DateOnly(2026, 10, 4),
                    EndDate = null,
                    CreatedAtUtc = DateTimeOffset.UtcNow,
                    LastModifiedAtUtc = null
                };

                static DbSet<TEntity> GetDbSet<TEntity>(DbContext dbContext) where TEntity : class => dbContext.Set<TEntity>();

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

        public IServiceCollection AddDallServices()
        {
            services.AddSingleton<IDateTimeOffsetProvider, DateTimeOffsetProvider>();

            services.AddScoped<ISaveChangesInterceptor, UpdateAuditEntityInterceptor>();

            return services;
        }
    }
}
