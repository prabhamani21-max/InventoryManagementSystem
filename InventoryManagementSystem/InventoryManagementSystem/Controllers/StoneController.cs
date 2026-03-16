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
    public class StoneController : ControllerBase
    {
        private readonly IStoneService _stoneService;
        private readonly ILogger<StoneController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public StoneController(IStoneService stoneService, IMapper mapper, ILogger<StoneController> logger, ICurrentUser currentUser)
        {
            _stoneService = stoneService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStones()
        {
            _logger.LogInformation("Fetching all stones");
            var stones = await _stoneService.GetAllStonesAsync();
            var stoneDtos = _mapper.Map<IEnumerable<StoneDto>>(stones);
            return Ok(stoneDtos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStones([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name parameter is required for search");
            }

            _logger.LogInformation("Searching stones by name: {Name}", name);
            var stones = await _stoneService.SearchStonesByNameAsync(name);
            var stoneDtos = _mapper.Map<IEnumerable<StoneDto>>(stones);
            return Ok(stoneDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStoneById(int id)
        {
            _logger.LogInformation("Fetching stone by ID: {Id}", id);
            var stone = await _stoneService.GetStoneByIdAsync(id);
            if (stone == null)
            {
                _logger.LogWarning("Stone not found for ID: {Id}", id);
                return NotFound("Stone not found");
            }
            var stoneDto = _mapper.Map<StoneDto>(stone);
            return Ok(stoneDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStone([FromBody] StoneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating stone: {Name}", dto.Name);

            // Check for name uniqueness
            if (await _stoneService.NameExistsAsync(dto.Name))
            {
                ModelState.AddModelError("Name", "A stone with this name already exists");
                return Conflict(ModelState);
            }

            var stone = _mapper.Map<Stone>(dto);
            stone.CreatedDate = DateTime.UtcNow;
            stone.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            stone.StatusId = (int)StatusEnum.Active;

            var createdStone = await _stoneService.CreateStoneAsync(stone);
            var createdDto = _mapper.Map<StoneDto>(createdStone);

            _logger.LogInformation("Stone created successfully ID: {Id}", createdDto.Id);
            return Ok(createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStone(int id, [FromBody] StoneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating stone ID: {Id}", id);

            var existingStone = await _stoneService.GetStoneByIdAsync(id);
            if (existingStone == null)
            {
                _logger.LogWarning("Stone not found for update ID: {Id}", id);
                return NotFound("Stone not found");
            }

            // Check for name uniqueness, excluding current stone
            if (await _stoneService.NameExistsAsync(dto.Name, id))
            {
                ModelState.AddModelError("Name", "A stone with this name already exists");
                return Conflict(ModelState);
            }

            var stone = _mapper.Map<Stone>(dto);
            stone.Id = id;
            stone.UpdatedDate = DateTime.UtcNow;
            stone.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedStone = await _stoneService.UpdateStoneAsync(stone);
            var updatedDto = _mapper.Map<StoneDto>(updatedStone);

            _logger.LogInformation("Stone updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStone(int id)
        {
            _logger.LogInformation("Deleting stone ID: {Id}", id);

            var result = await _stoneService.DeleteStoneAsync(id);
            if (!result)
            {
                _logger.LogWarning("Stone not found for deletion ID: {Id}", id);
                return NotFound("Stone not found");
            }

            _logger.LogInformation("Stone deleted successfully ID: {Id}", id);
            return Ok(true);
        }
    }
}