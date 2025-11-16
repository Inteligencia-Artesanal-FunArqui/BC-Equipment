using System.Net.Http.Json;
using OsitoPolar.EquipmentService.Shared.Interfaces.ACL;

namespace OsitoPolar.EquipmentService.Application.ACL.Services;

/// <summary>
/// HTTP Facade for communicating with Profiles Service
/// Replaces direct database access with HTTP calls to the Profiles microservice
/// </summary>
public class ProfilesHttpFacade : IProfilesContextFacade
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProfilesHttpFacade> _logger;

    public ProfilesHttpFacade(HttpClient httpClient, ILogger<ProfilesHttpFacade> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> IsUserAnOwner(int userId)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} is an owner via Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("User {UserId} is not an owner", userId);
                return false;
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("User {UserId} is an owner", userId);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to check if user {UserId} is an owner", userId);
            return false;
        }
    }

    public async Task<int> FetchOwnerIdByUserId(int userId)
    {
        try
        {
            _logger.LogInformation("Fetching Owner ID for user {UserId} from Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Owner profile not found for user {UserId}", userId);
                return 0;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OwnerResponse>();

            if (result == null)
            {
                _logger.LogWarning("Received null response for user {UserId}", userId);
                return 0;
            }

            _logger.LogInformation("Found Owner ID {OwnerId} for user {UserId}", result.Id, userId);
            return result.Id;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch Owner ID for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<bool> IsUserAProvider(int userId)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} is a provider via Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("User {UserId} is not a provider", userId);
                return false;
            }

            response.EnsureSuccessStatusCode();
            _logger.LogInformation("User {UserId} is a provider", userId);
            return true;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to check if user {UserId} is a provider", userId);
            return false;
        }
    }

    public async Task<int> FetchProviderIdByUserId(int userId)
    {
        try
        {
            _logger.LogInformation("Fetching Provider ID for user {UserId} from Profiles Service", userId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/by-user/{userId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Provider profile not found for user {UserId}", userId);
                return 0;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ProviderResponse>();

            if (result == null)
            {
                _logger.LogWarning("Received null response for user {UserId}", userId);
                return 0;
            }

            _logger.LogInformation("Found Provider ID {ProviderId} for user {UserId}", result.Id, userId);
            return result.Id;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch Provider ID for user {UserId}", userId);
            return 0;
        }
    }

    public async Task<OwnerDto?> GetOwnerByIdAsync(int ownerId)
    {
        try
        {
            _logger.LogInformation("Fetching Owner {OwnerId} from Profiles Service", ownerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/{ownerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Owner {OwnerId} not found", ownerId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OwnerResponse>();

            if (result == null)
                return null;

            return new OwnerDto(
                result.Id,
                result.UserId,
                result.FullName,
                result.Email,
                result.PhoneNumber,
                result.MaxUnits
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch Owner {OwnerId}", ownerId);
            return null;
        }
    }

    public async Task<ProviderDto?> GetProviderByIdAsync(int providerId)
    {
        try
        {
            _logger.LogInformation("Fetching Provider {ProviderId} from Profiles Service", providerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/{providerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Provider {ProviderId} not found", providerId);
                return null;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ProviderResponse>();

            if (result == null)
                return null;

            return new ProviderDto(
                result.Id,
                result.UserId,
                result.FullName,
                result.Email,
                result.PhoneNumber,
                result.CompanyName,
                result.MaxClients
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch Provider {ProviderId}", providerId);
            return null;
        }
    }

    public async Task<bool> OwnerExists(int ownerId)
    {
        try
        {
            _logger.LogInformation("Checking if Owner {OwnerId} exists via Profiles Service", ownerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/owners/{ownerId}");
            var exists = response.IsSuccessStatusCode;

            _logger.LogInformation("Owner {OwnerId} exists: {Exists}", ownerId, exists);
            return exists;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to check if Owner {OwnerId} exists", ownerId);
            return false;
        }
    }

    public async Task<bool> ProviderExists(int providerId)
    {
        try
        {
            _logger.LogInformation("Checking if Provider {ProviderId} exists via Profiles Service", providerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/{providerId}");
            var exists = response.IsSuccessStatusCode;

            _logger.LogInformation("Provider {ProviderId} exists: {Exists}", providerId, exists);
            return exists;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to check if Provider {ProviderId} exists", providerId);
            return false;
        }
    }

    public async Task<string> FetchProviderCompanyName(int providerId)
    {
        try
        {
            _logger.LogInformation("Fetching company name for Provider {ProviderId} from Profiles Service", providerId);

            var response = await _httpClient.GetAsync($"/api/v1/profiles/providers/{providerId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Provider {ProviderId} not found", providerId);
                return string.Empty;
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ProviderResponse>();

            if (result == null)
            {
                _logger.LogWarning("Received null response for Provider {ProviderId}", providerId);
                return string.Empty;
            }

            _logger.LogInformation("Found company name '{CompanyName}' for Provider {ProviderId}", result.CompanyName, providerId);
            return result.CompanyName;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch company name for Provider {ProviderId}", providerId);
            return string.Empty;
        }
    }
}

// Response DTOs for deserializing HTTP responses from Profiles Service
internal record OwnerResponse(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    int MaxUnits
);

internal record ProviderResponse(
    int Id,
    int UserId,
    string FullName,
    string Email,
    string PhoneNumber,
    string CompanyName,
    int MaxClients
);
