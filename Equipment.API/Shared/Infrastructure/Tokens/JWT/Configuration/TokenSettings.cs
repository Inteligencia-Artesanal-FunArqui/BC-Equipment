namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Tokens.JWT.Configuration;

/// <summary>
/// Token settings for JWT validation
/// Must use the same secret as IAM Service
/// </summary>
public class TokenSettings
{
    public string Secret { get; set; } = string.Empty;
}
