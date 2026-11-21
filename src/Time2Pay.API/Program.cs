using System.Security.Claims;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Time2Pay.API;
using Time2Pay.BLL;
using Time2Pay.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDall(builder.Environment.IsDevelopment())
    .AddBll();

builder.AddApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");

    app.MapScalarApiReference("/docs", options =>
    {
        options
            .WithTitle("Time2Pay API")

            // Appearance
            .WithTheme(ScalarTheme.DeepSpace)
            .ForceDarkMode()
            .HideDarkModeToggle()

            // Sidebar
            .ExpandAllTags()
            .SortTagsAlphabetically()
            .SortOperationsByMethod()

            // Operations & schemas
            .ShowOperationId()
            .PreserveSchemaPropertyOrder()

            // Code samples
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

        // Authentication
        var keycloakClientId = builder.Configuration["Keycloak:ClientId"]!;

        options.AddAuthorizationCodeFlow(nameof(SecuritySchemeType.OAuth2), flow =>
        {
            flow.WithClientId(keycloakClientId)
                .WithSelectedScopes("openid", "profile")
                .WithPkce(Pkce.Sha256);
        });
    });

    await app.ApplyMigrationsAsync();
}

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapGet("users/me", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                UserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = user.FindFirstValue(ClaimTypes.Email),
                Name = user.FindFirstValue("preferred_username"),
                Claims = user.Claims.Select(c => new { c.Type, c.Value })
            });
        })
        .RequireAuthorization();
}

await app.RunAsync();
