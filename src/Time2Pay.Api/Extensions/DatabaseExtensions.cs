using Microsoft.EntityFrameworkCore;
using Time2Pay.Api.Database;

namespace Time2Pay.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while applying database migrations");
            throw;
        }
    }
}
