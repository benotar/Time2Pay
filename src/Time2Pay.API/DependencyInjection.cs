using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Time2Pay.API.Keycloak;
using Time2Pay.API.Middleware;
using Time2Pay.API.Options;
using Time2Pay.DAL;

namespace Time2Pay.API;

public static class DependencyInjection
{
    public static void AddApi(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true;
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        builder.Services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<KeycloakSecuritySchemeTransformer>();
        });

        builder.Services.AddOptions<KeycloakOptions>()
            .Bind(builder.Configuration.GetSection(KeycloakOptions.SectionName));

        var keycloakOptions = builder.Configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>()!;

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MetadataAddress = keycloakOptions.MetadataAddress;
                options.Audience = keycloakOptions.Audience;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = keycloakOptions.Issuer
                };

                // Required for HTTP in development (Keycloak uses HTTP by default in dev mode)
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            });

        builder.Services.AddAuthorization();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddNpgsql())
            .WithMetrics(metrics => metrics
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation())
            .UseOtlpExporter();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
        });
    }

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
