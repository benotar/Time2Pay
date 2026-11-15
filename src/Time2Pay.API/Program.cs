using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Time2Pay.API;
using Time2Pay.API.Middleware;
using Time2Pay.BLL;
using Time2Pay.DAL;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddOpenApi();

// TODO Move to respective method and set up keycloak options
var keycloakAuthority = builder.Configuration["Keycloak:Authority"]!;
var keycloakClientId = builder.Configuration["Keycloak:ClientId"]!;

// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo { Title = "Time 2Pay API", Version = "v1" });
//
//     // Define the OAuth 2.0 security scheme
//     options.AddSecurityDefinition(nameof(SecuritySchemeType.OAuth2),
//         new OpenApiSecurityScheme
//         {
//             Type = SecuritySchemeType.OAuth2,
//             Flows = new OpenApiOAuthFlows
//             {
//                 AuthorizationCode = new OpenApiOAuthFlow
//                 {
//                     AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
//                     TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
//                     Scopes = new Dictionary<string, string>
//                     {
//                         { "openid", "OpenID Connect scope" }, { "profile", "User profile" }
//                     }
//                 }
//             }
//         });
//
//     // Apply security to all operations
//     options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
//     {
//         { new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2), doc), [] }
//     });
// });

builder.Services
    .AddDallServices()
    .AddDatabase(builder.Environment.IsDevelopment())
    .AddBll();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // app.UseSwagger();
    // app.UseSwaggerUI(options =>
    // {
    //     options.OAuthClientId(keycloakClientId); // Default Client ID
    //     options.OAuthUsePkce();
    // });
    //
    await app.ApplyMigrationsAsync();
}

app.UseExceptionHandler();

app.MapControllers();

await app.RunAsync();
