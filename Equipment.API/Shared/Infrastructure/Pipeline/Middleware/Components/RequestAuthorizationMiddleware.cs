using OsitoPolar.EquipmentService.Shared.Domain.Model;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Pipeline.Middleware.Attributes;
using OsitoPolar.EquipmentService.Shared.Infrastructure.Tokens.JWT.Services;

namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
/// Middleware to validate JWT tokens and set user context
/// This middleware validates tokens using the same secret as IAM Service
/// </summary>
public class RequestAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public RequestAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        Console.WriteLine($"[Equipment-Middleware] Processing: {context.Request.Method} {context.Request.Path}");

        // Skip authorization for swagger endpoints
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (path.Contains("/swagger"))
        {
            Console.WriteLine("[Equipment-Middleware] Skipping authorization for swagger endpoints");
            await _next(context);
            return;
        }

        // Check for AllowAnonymous attribute
        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            var allowAnonymous = endpoint.Metadata
                .Any(m => m.GetType() == typeof(AllowAnonymousAttribute) ||
                          m.GetType() == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));

            if (allowAnonymous)
            {
                Console.WriteLine("[Equipment-Middleware] AllowAnonymous found, but still attempting to extract user if token present");
            }
        }

        // Always try to extract user from token if present (even for AllowAnonymous endpoints)
        try
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                Console.WriteLine($"[Equipment-Middleware] Token found: {token.Substring(0, Math.Min(20, token.Length))}...");

                var userId = await tokenService.ValidateToken(token);
                if (userId != null)
                {
                    Console.WriteLine($"[Equipment-Middleware] Token valid for userId: {userId}");

                    // Create a lightweight User object with the extracted ID
                    // The full user profile will be fetched via ProfilesFacade when needed
                    var user = new User(userId.Value, "", "");
                    context.Items["User"] = user;
                }
                else
                {
                    Console.WriteLine("[Equipment-Middleware] Token validation failed");
                }
            }
            else
            {
                Console.WriteLine("[Equipment-Middleware] No valid authorization header found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Equipment-Middleware] Error in authorization: {ex.Message}");
        }

        await _next(context);
    }
}
