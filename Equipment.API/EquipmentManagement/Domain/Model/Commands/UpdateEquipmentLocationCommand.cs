namespace OsitoPolar.EquipmentService.Domain.Model.Commands;

public record UpdateEquipmentLocationCommand(
    int EquipmentId,
    string LocationName,
    string LocationAddress,
    decimal Latitude,
    decimal Longitude
);