using Microsoft.EntityFrameworkCore;
using Time2Pay.Api.Entities;

namespace Time2Pay.Api.Database;

public sealed class Time2PayDbContext(DbContextOptions<Time2PayDbContext> options) : DbContext(options)
{
    public DbSet<Employment> Employments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.ApplicationSchema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Time2PayDbContext).Assembly);
    }
}
