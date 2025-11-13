using OsitoPolar.EquipmentService.Domain.Model.Aggregates;
using OsitoPolar.EquipmentService.Shared.Domain.Repositories;

namespace OsitoPolar.EquipmentService.Domain.Repositories;

/// <summary>
/// Defines the contract for data access operations for Equipment aggregate.
/// </summary>
public interface IEquipmentRepository : IBaseRepository<Equipment>
{
    Task<IEnumerable<Equipment>> FindByOwnerIdAsync(int ownerId);
    Task<IEnumerable<Equipment>> FindByTypeAsync(string equipmentType);
    Task<IEnumerable<Equipment>> FindByStatusAsync(string status);
    Task<bool> ExistsBySerialNumberAsync(string serialNumber);
    Task<bool> ExistsByCodeAsync(string code);

    // Rental Equipment queries
    /// <summary>
    /// Find equipment available for rent (has RentalInfo and is active)
    /// </summary>
    Task<IEnumerable<Equipment>> FindAvailableForRentAsync();

    /// <summary>
    /// Find available rental equipment by type
    /// </summary>
    Task<IEnumerable<Equipment>> FindAvailableForRentByTypeAsync(string type);

    /// <summary>
    /// Find rental equipment by provider
    /// </summary>
    Task<IEnumerable<Equipment>> FindRentalByProviderIdAsync(int providerId);
}