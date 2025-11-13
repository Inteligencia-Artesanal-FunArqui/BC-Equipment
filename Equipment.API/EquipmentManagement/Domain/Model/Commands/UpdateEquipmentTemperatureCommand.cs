namespace OsitoPolar.EquipmentService.Domain.Model.Commands;

public record UpdateEquipmentTemperatureCommand(int EquipmentId, decimal NewTemperature);