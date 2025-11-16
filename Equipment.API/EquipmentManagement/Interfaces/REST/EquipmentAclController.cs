using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using OsitoPolar.EquipmentService.Interfaces.ACL;

namespace OsitoPolar.EquipmentService.Interfaces.REST;

/// <summary>
/// ACL Controller for Equipment Service
/// Provides endpoints for inter-service communication (Analytics, WorkOrders, etc.)
/// </summary>
[AllowAnonymous] // Inter-service communication - no auth required
[ApiController]
[Route("api/v1/equipment/acl")]
[SwaggerTag("ACL Endpoints - Inter-service Communication")]
public class EquipmentAclController : ControllerBase
{
    private readonly IEquipmentContextFacade _facade;
    private readonly ILogger<EquipmentAclController> _logger;

    public EquipmentAclController(
        IEquipmentContextFacade facade,
        ILogger<EquipmentAclController> logger)
    {
        _facade = facade;
        _logger = logger;
    }

    /// <summary>
    /// Get all equipment IDs for a specific owner
    /// </summary>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>List of equipment IDs</returns>
    [HttpGet("by-owner/{ownerId:int}/ids")]
    [SwaggerOperation(
        Summary = "Get equipment IDs by owner",
        Description = "Returns all equipment IDs owned by the specified owner (called by Analytics Service)")]
    [SwaggerResponse(200, "Equipment IDs retrieved successfully")]
    public async Task<IActionResult> GetEquipmentIdsByOwner(int ownerId)
    {
        try
        {
            _logger.LogInformation("ACL: Fetching equipment IDs for owner {OwnerId}", ownerId);
            var ids = await _facade.FetchEquipmentIdsByOwnerId(ownerId);
            return Ok(new { Ids = ids.ToList() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching equipment IDs for owner {OwnerId}", ownerId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get optimal temperature range for equipment
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Temperature range (min, max)</returns>
    [HttpGet("{equipmentId:int}/temperature-range")]
    [SwaggerOperation(
        Summary = "Get equipment temperature range",
        Description = "Returns the optimal temperature range for the specified equipment (called by Analytics Service)")]
    [SwaggerResponse(200, "Temperature range retrieved successfully")]
    [SwaggerResponse(404, "Equipment not found")]
    public async Task<IActionResult> GetTemperatureRange(int equipmentId)
    {
        try
        {
            _logger.LogInformation("ACL: Fetching temperature range for equipment {EquipmentId}", equipmentId);
            var range = await _facade.GetEquipmentOptimalTemperatureRange(equipmentId);

            if (range == null)
            {
                _logger.LogWarning("Equipment {EquipmentId} not found", equipmentId);
                return NotFound(new { message = "Equipment not found" });
            }

            return Ok(new { MinTemp = range.Value.minTemp, MaxTemp = range.Value.maxTemp });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching temperature range for equipment {EquipmentId}", equipmentId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get maintenance data for equipment
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Maintenance data (installation date, temperature range)</returns>
    [HttpGet("{equipmentId:int}/maintenance-data")]
    [SwaggerOperation(
        Summary = "Get equipment maintenance data",
        Description = "Returns maintenance data including installation date and temperature range (called by Analytics Service)")]
    [SwaggerResponse(200, "Maintenance data retrieved successfully")]
    [SwaggerResponse(404, "Equipment not found")]
    public async Task<IActionResult> GetMaintenanceData(int equipmentId)
    {
        try
        {
            _logger.LogInformation("ACL: Fetching maintenance data for equipment {EquipmentId}", equipmentId);
            var data = await _facade.GetEquipmentMaintenanceData(equipmentId);

            if (data == null)
            {
                _logger.LogWarning("Equipment {EquipmentId} not found", equipmentId);
                return NotFound(new { message = "Equipment not found" });
            }

            return Ok(new
            {
                InstallationDate = data.Value.installationDate,
                MinTemp = data.Value.minTemp,
                MaxTemp = data.Value.maxTemp
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching maintenance data for equipment {EquipmentId}", equipmentId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if equipment exists
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <returns>Boolean indicating if equipment exists</returns>
    [HttpGet("../exists/{equipmentId:int}")]
    [SwaggerOperation(
        Summary = "Check if equipment exists",
        Description = "Returns true if equipment exists, false otherwise (called by Analytics Service)")]
    [SwaggerResponse(200, "Check completed successfully")]
    public async Task<IActionResult> EquipmentExists(int equipmentId)
    {
        try
        {
            _logger.LogInformation("ACL: Checking if equipment {EquipmentId} exists", equipmentId);
            var exists = await _facade.EquipmentExists(equipmentId);
            return Ok(new { Exists = exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if equipment {EquipmentId} exists", equipmentId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if equipment is owned by a specific owner
    /// </summary>
    /// <param name="equipmentId">Equipment ID</param>
    /// <param name="ownerId">Owner ID</param>
    /// <returns>Boolean indicating if equipment is owned by the owner</returns>
    [HttpGet("../is-owned-by/{equipmentId:int}/{ownerId:int}")]
    [SwaggerOperation(
        Summary = "Check equipment ownership",
        Description = "Returns true if equipment is owned by the specified owner (called by Analytics Service)")]
    [SwaggerResponse(200, "Check completed successfully")]
    public async Task<IActionResult> IsOwnedBy(int equipmentId, int ownerId)
    {
        try
        {
            _logger.LogInformation("ACL: Checking if equipment {EquipmentId} is owned by owner {OwnerId}", equipmentId, ownerId);
            var isOwned = await _facade.IsEquipmentOwnedBy(equipmentId, ownerId);
            return Ok(new { IsOwned = isOwned });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking ownership of equipment {EquipmentId} for owner {OwnerId}", equipmentId, ownerId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
