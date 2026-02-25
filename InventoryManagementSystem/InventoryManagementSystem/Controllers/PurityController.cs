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
    public class PurityController : ControllerBase
    {
        private readonly IPurityService _purityService;
        private readonly ILogger<PurityController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PurityController(IPurityService purityService, IMapper mapper, ILogger<PurityController> logger, ICurrentUser currentUser)
        {
            _purityService = purityService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurities()
        {
            _logger.LogInformation("Fetching all purities");
            var purities = await _purityService.GetAllPuritiesAsync();
            var purityDtos = _mapper.Map<IEnumerable<PurityDto>>(purities);
            return Ok(purityDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurityById(int id)
        {
            _logger.LogInformation("Fetching purity by ID: {Id}", id);
            var purity = await _purityService.GetPurityByIdAsync(id);
            if (purity == null)
            {
                _logger.LogWarning("Purity not found for ID: {Id}", id);
                return NotFound("Purity not found");
            }
            var purityDto = _mapper.Map<PurityDto>(purity);
            return Ok(purityDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurity([FromBody] PurityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating purity: {Name}", dto.Name);

            var purity = _mapper.Map<Purity>(dto);
            purity.CreatedDate = DateTime.UtcNow;
            purity.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var createdPurity = await _purityService.CreatePurityAsync(purity);
            var createdDto = _mapper.Map<PurityDto>(createdPurity);

            _logger.LogInformation("Purity created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetPurityById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurity(int id, [FromBody] PurityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating purity ID: {Id}", id);

            var existingPurity = await _purityService.GetPurityByIdAsync(id);
            if (existingPurity == null)
            {
                _logger.LogWarning("Purity not found for update ID: {Id}", id);
                return NotFound("Purity not found");
            }

            var purity = _mapper.Map<Purity>(dto);
            purity.Id = id;
            purity.UpdatedDate = DateTime.UtcNow;
            purity.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedPurity = await _purityService.UpdatePurityAsync(purity);
            var updatedDto = _mapper.Map<PurityDto>(updatedPurity);

            _logger.LogInformation("Purity updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurity(int id)
        {
            _logger.LogInformation("Deleting purity ID: {Id}", id);

            var result = await _purityService.DeletePurityAsync(id);
            if (!result)
            {
                _logger.LogWarning("Purity not found for deletion ID: {Id}", id);
                return NotFound("Purity not found");
            }

            _logger.LogInformation("Purity deleted successfully ID: {Id}", id);
            return Ok(true);
        }
    }
}