using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MetalController : ControllerBase
    {
        private readonly IMetalService _metalService;
        private readonly ILogger<MetalController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public MetalController(IMetalService metalService, IMapper mapper, ILogger<MetalController> logger, ICurrentUser currentUser)
        {
            _metalService = metalService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMetals()
        {
            _logger.LogInformation("Fetching all metals");
            var metals = await _metalService.GetAllMetalsAsync();
            var metalDtos = _mapper.Map<IEnumerable<MetalDto>>(metals);
            return Ok(metalDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMetalById(int id)
        {
            _logger.LogInformation("Fetching metal by ID: {Id}", id);
            var metal = await _metalService.GetMetalByIdAsync(id);
            if (metal == null)
            {
                _logger.LogWarning("Metal not found for ID: {Id}", id);
                return NotFound("Metal not found");
            }
            var metalDto = _mapper.Map<MetalDto>(metal);
            return Ok(metalDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMetal([FromBody] MetalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating metal: {Name}", dto.Name);

            var metal = _mapper.Map<Metal>(dto);
            metal.CreatedDate = DateTime.UtcNow;
            metal.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var createdMetal = await _metalService.CreateMetalAsync(metal);
            var createdDto = _mapper.Map<MetalDto>(createdMetal);

            _logger.LogInformation("Metal created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetMetalById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMetal(int id, [FromBody] MetalDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating metal ID: {Id}", id);

            var existingMetal = await _metalService.GetMetalByIdAsync(id);
            if (existingMetal == null)
            {
                _logger.LogWarning("Metal not found for update ID: {Id}", id);
                return NotFound("Metal not found");
            }

            var metal = _mapper.Map<Metal>(dto);
            metal.Id = id;
            metal.UpdatedDate = DateTime.UtcNow;
            metal.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedMetal = await _metalService.UpdateMetalAsync(metal);
            var updatedDto = _mapper.Map<MetalDto>(updatedMetal);

            _logger.LogInformation("Metal updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMetal(int id)
        {
            _logger.LogInformation("Deleting metal ID: {Id}", id);

            var result = await _metalService.DeleteMetalAsync(id);
            if (!result)
            {
                _logger.LogWarning("Metal not found for deletion ID: {Id}", id);
                return NotFound("Metal not found");
            }

            _logger.LogInformation("Metal deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}