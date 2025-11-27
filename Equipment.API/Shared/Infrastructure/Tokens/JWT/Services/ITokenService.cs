namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Tokens.JWT.Services;

/// <summary>
/// Token service interface for JWT validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Validate a JWT token and extract the user ID
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>The user ID if valid, null otherwise</returns>
    Task<int?> ValidateToken(string token);
}
