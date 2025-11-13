using OsitoPolar.EquipmentService.Domain.Model.Aggregates;
using OsitoPolar.EquipmentService.Domain.Model.Commands;

namespace OsitoPolar.EquipmentService.Domain.Services;

/// <summary>
/// Defines the contract for command-based operations on Equipment.
/// </summary>
public interface IEquipmentCommandService
{
    Task<Equipment?> Handle(CreateEquipmentCommand command);
    Task<Equipment?> Handle(UpdateEquipmentTemperatureCommand command);
    Task<Equipment?> Handle(UpdateEquipmentPowerStateCommand command);
    Task<Equipment?> Handle(UpdateEquipmentLocationCommand command);
    Task<bool> Handle(DeleteEquipmentCommand command);

    // Rental Equipment commands
    Task<Equipment?> Handle(PublishEquipmentForRentCommand command);
    Task<Equipment?> Handle(UnpublishEquipmentFromRentCommand command);
}