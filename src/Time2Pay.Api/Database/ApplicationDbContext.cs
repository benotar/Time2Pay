using Microsoft.EntityFrameworkCore;

namespace Time2Pay.Api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DatabaseConstants.ApplicationSchema);
    }
}
