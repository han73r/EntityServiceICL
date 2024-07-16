using Microsoft.AspNetCore.Mvc;
using EntityService.Application.Interfaces;
using EntityService.Application.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace EntityService.Web.Contollers;
[ApiController]
[Route("api/[controller]")]
public class EntityController : ControllerBase {
    private readonly IEntityService _entityService;
    private readonly ILogger<EntityController> _logger;
    public EntityController(IEntityService entityService, ILogger<EntityController> logger) {
        _entityService = entityService;
        _logger = logger;
    }
    [HttpPost("insert")]
    public async Task<IActionResult> InsertEntity([FromBody] EntityDto entityDto) {
        if (entityDto == null) {
            _logger.LogWarning("InsertEntity called with null entityDto");
            return BadRequest("Invalid entity data.");
        }
        if (!ModelState.IsValid) {
            _logger.LogWarning("InsertEntity called with invalid model state: {ModelState}", ModelState);
            return BadRequest(ModelState);
        }
        try {
            await _entityService.SaveEntityAsync(entityDto);
            return Ok("Entity saved successfully.");
        } catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while saving entity.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    [HttpGet("get/{id}")]
    public async Task<IActionResult> GetEntityById(Guid id) {
        if (id == Guid.Empty) {
            _logger.LogWarning("GetEntityById called with empty GUID");
            return BadRequest("Invalid entity ID.");
        }
        try {
            var entityDto = await _entityService.GetEntityByIdAsync(id);
            if (entityDto == null) {
                _logger.LogWarning("Entity with ID {Id} not found", id);
                return NotFound("Entity not found.");
            }
            return Ok(entityDto);
        } catch (Exception ex) {
            _logger.LogError(ex, "Error occurred while retrieving entity.");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}