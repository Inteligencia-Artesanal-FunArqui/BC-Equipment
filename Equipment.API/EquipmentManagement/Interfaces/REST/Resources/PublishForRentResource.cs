using System.ComponentModel.DataAnnotations;

namespace OsitoPolar.EquipmentService.Interfaces.REST.Resources;

/// <summary>
/// Resource for publishing equipment for rent
/// </summary>
public record PublishForRentResource
{
    [Required]
    public DateTimeOffset StartDate { get; init; }

    [Required]
    public DateTimeOffset EndDate { get; init; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Monthly fee must be positive")]
    public decimal MonthlyFee { get; init; }
}
