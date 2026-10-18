using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Time2Pay.API;
using Time2Pay.BLL;
using Time2Pay.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});

builder.Services.AddOpenApi();

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

    await app.ApplyMigrationsAsync();
}

app.MapControllers();

await app.RunAsync();
