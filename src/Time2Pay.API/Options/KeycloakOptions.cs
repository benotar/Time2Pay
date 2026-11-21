namespace Time2Pay.API.Options;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    public string Authority { get; set; }
    public string ClientId { get; set; }
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string MetadataAddress { get; set; }
}
