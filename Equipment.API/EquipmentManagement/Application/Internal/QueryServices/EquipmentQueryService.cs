using OsitoPolar.EquipmentService.Domain.Model.Aggregates;
using OsitoPolar.EquipmentService.Domain.Model.Queries;
using OsitoPolar.EquipmentService.Domain.Repositories;
using OsitoPolar.EquipmentService.Domain.Services;

namespace OsitoPolar.EquipmentService.Application.Internal.QueryServices;

/// <summary>
/// Concrete implementation of IEquipmentQueryService.
/// </summary>
public class EquipmentQueryService(IEquipmentRepository equipmentRepository) : IEquipmentQueryService
{
    public async Task<Equipment?> Handle(GetEquipmentByIdQuery query)
    {
        return await equipmentRepository.FindByIdAsync(query.EquipmentId);
    }

    public async Task<IEnumerable<Equipment>> Handle(GetAllEquipmentsQuery query)
    {
        return await equipmentRepository.ListAsync();
    }

    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentsByOwnerIdQuery query)
    {
        return await equipmentRepository.FindByOwnerIdAsync(query.OwnerId);
    }

    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentsByTypeQuery query)
    {
        return await equipmentRepository.FindByTypeAsync(query.EquipmentType);
    }

    public async Task<IEnumerable<Equipment>> Handle(GetEquipmentsByStatusQuery query)
    {
        return await equipmentRepository.FindByStatusAsync(query.Status);
    }

    // ========== RENTAL EQUIPMENT QUERY HANDLERS ==========

    public async Task<IEnumerable<Equipment>> Handle(GetAvailableRentalEquipmentQuery query)
    {
        var equipments = await equipmentRepository.FindAvailableForRentAsync();

        // Apply filters
        if (!string.IsNullOrEmpty(query.Type))
        {
            equipments = equipments.Where(e => e.Type.ToString().Equals(query.Type, StringComparison.OrdinalIgnoreCase));
        }

        if (query.MaxMonthlyFee.HasValue)
        {
            equipments = equipments.Where(e => e.RentalInfo != null && e.RentalInfo.MonthlyFee <= query.MaxMonthlyFee.Value);
        }

        return equipments;
    }

    public async Task<Equipment?> Handle(GetRentalEquipmentByIdQuery query)
    {
        var equipment = await equipmentRepository.FindByIdAsync(query.EquipmentId);

        // Verify it's published for rent (has rental info with monthly fee)
        if (equipment?.RentalInfo == null || equipment.RentalInfo.MonthlyFee <= 0)
            return null;

        return equipment;
    }
}