using Microsoft.EntityFrameworkCore;
using Time2Pay.DAL;

namespace Time2Pay.API;

public static class DependencyInjection
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        await using var dbContext = scope.ServiceProvider.GetRequiredService<Time2PayDbContext>();

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
