namespace Time2Pay.DAL.Entities;

public sealed class User : AuditEntity, IPrefix
{
    public string Email { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// I'll use this to store the IdentityId from the Keycloak
    /// </summary>
    public string ExternalId { get; set; }

    public static string Prefix => "user_";
}
