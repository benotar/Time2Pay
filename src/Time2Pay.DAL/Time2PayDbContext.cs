using Microsoft.EntityFrameworkCore;
using Time2Pay.DAL.Entities;

namespace Time2Pay.DAL;

public sealed class Time2PayDbContext(DbContextOptions<Time2PayDbContext> options) : DbContext(options)
{
    public DbSet<Employment> Employments { get; set; }

    public DbSet<WorkDay> WorkDays { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DalConstants.ApplicationSchema);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Time2PayDbContext).Assembly);
    }
}
