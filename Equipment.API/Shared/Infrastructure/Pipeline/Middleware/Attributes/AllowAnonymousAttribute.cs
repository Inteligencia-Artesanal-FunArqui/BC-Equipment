namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
/// Attribute to mark endpoints that should bypass JWT authentication
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}
