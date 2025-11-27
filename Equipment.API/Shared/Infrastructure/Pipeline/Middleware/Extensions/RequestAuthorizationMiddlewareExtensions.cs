using OsitoPolar.EquipmentService.Shared.Infrastructure.Pipeline.Middleware.Components;

namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
/// Extension methods to register the RequestAuthorizationMiddleware
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}
