namespace OsitoPolar.EquipmentService.Domain.Model.Commands;

public record UpdateEquipmentPowerStateCommand(int EquipmentId, bool IsPoweredOn);