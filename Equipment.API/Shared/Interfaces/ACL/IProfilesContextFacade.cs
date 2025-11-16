namespace OsitoPolar.EquipmentService.Shared.Interfaces.ACL;

/// <summary>
/// Facade for communicating with Profiles Service
/// Provides methods to validate and retrieve Owner and Provider profiles
/// </summary>
public interface IProfilesContextFacade
{
    /// <summary>
    /// Check if a user has an Owner profile
    /// </summary>
    /// <param name="userId">User ID from IAM Service</param>
    /// <returns>True if user is an owner, false otherwise</returns>
    Task<bool> IsUserAnOwner(int userId);

    /// <summary>
    /// Fetch the Owner profile ID for a given user
    /// </summary>
    /// <param name="userId">User ID from IAM Service</param>
    /// <returns>Owner ID if found, 0 otherwise</returns>
    Task<int> FetchOwnerIdByUserId(int userId);

    /// <summary>
    /// Check if a user has a Provider (RenterProvider) profile
    /// </summary>
    /// <param name="userId">User ID from IAM Service</param>
    /// <returns>True if user is a provider, false otherwise</returns>
    Task<bool> IsUserAProvider(int userId);

    /// <summary>
    /// Fetch the Provider profile ID for a given user
    /// </summary>
    /// <param name="userId">User ID from IAM Service</param>
    /// <returns>Provider ID if found, 0 otherwise</returns>
    Task<int> FetchProviderIdByUserId(int userId);

    /// <summary>
    /// Get Owner profile details by ID
    /// </summary>
    /// <param name="ownerId">Owner profile ID</param>
    /// <returns>Owner details or null if not found</returns>
    Task<OwnerDto?> GetOwnerByIdAsync(int ownerId);

    /// <summary>
    /// Get Provider profile details by ID
    /// </summary>
    /// <param name="providerId">Provider profile ID</param>
    /// <returns>Provider details or null if not found</returns>
    Task<ProviderDto?> GetProviderByIdAsync(int providerId);

    /// <summary>
    /// Check if an Owner exists
    /// </summary>
    /// <param name="ownerId">Owner profile ID</param>
    /// <returns>True if owner exists, false otherwise</returns>
    Task<bool> OwnerExists(int ownerId);

    /// <summary>
    /// Check if a Provider exists
    /// </summary>
    /// <param name="providerId">Provider profile ID</param>
    /// <returns>True if provider exists, false otherwise</returns>
    Task<bool> ProviderExists(int providerId);

    /// <summary>
    /// Fetch the company name for a Provider
    /// </summary>
    /// <param name="providerId">Provider profile ID</param>
    /// <returns>Company name if found, empty string otherwise</returns>
    Task<string> FetchProviderCompanyName(int providerId);
}

/// <summary>
/// DTO for Owner profile data from Profiles Service
/// </summary>
public record OwnerDto(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    int MaxUnits
);

/// <summary>
/// DTO for Provider profile data from Profiles Service
/// </summary>
public record ProviderDto(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string CompanyName,
    int MaxClients
);
