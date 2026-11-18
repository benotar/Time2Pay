using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Time2Pay.API.Keycloak;

internal sealed class KeycloakSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private readonly IConfiguration _configuration;

    public KeycloakSecuritySchemeTransformer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Time 2Pay API",
            Version = "v1"
        };

        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(MakeUrl("auth")),
                    TokenUrl = new Uri(MakeUrl("token")),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "OpenID Connect scope" },
                        { "profile", "User profile" }
                    }
                }
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Security ??= [];

        document.Components.SecuritySchemes[nameof(SecuritySchemeType.OAuth2)] = scheme;
        document.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(nameof(SecuritySchemeType.OAuth2), document),
                []
            }
        });

        return Task.CompletedTask;
    }

    private string MakeUrl(string part)
    {
        var authority = _configuration["Keycloak:Authority"]!;
        return $"{authority}/protocol/openid-connect/{part}";
    }
}
