using Microsoft.EntityFrameworkCore;

namespace OsitoPolar.EquipmentService.Domain.Model.ValueObjects;

/// <summary>
/// Represents geographical coordinates for equipment location.
/// </summary>
/// <param name="Latitude">The latitude coordinate</param>
/// <param name="Longitude">The longitude coordinate</param>

[Owned]
public record Coordinates(decimal Latitude, decimal Longitude);