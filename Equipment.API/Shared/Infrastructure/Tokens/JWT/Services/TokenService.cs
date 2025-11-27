using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Tokens.JWT.Configuration;

namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Tokens.JWT.Services;

/// <summary>
/// Token service for JWT validation
/// Uses the same secret as IAM Service to validate tokens
/// </summary>
public class TokenService : ITokenService
{
    private readonly TokenSettings _tokenSettings;

    public TokenService(IOptions<TokenSettings> tokenSettings)
    {
        _tokenSettings = tokenSettings.Value;
    }

    /// <summary>
    /// Validate a JWT token and extract the user ID
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>The user ID if valid, null otherwise</returns>
    public async Task<int?> ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);

        try
        {
            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            });

            var jwtToken = (JsonWebToken)tokenValidationResult.SecurityToken;
            var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == ClaimTypes.Sid).Value);
            return userId;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[TokenService] Token validation failed: {e.Message}");
            return null;
        }
    }
}
