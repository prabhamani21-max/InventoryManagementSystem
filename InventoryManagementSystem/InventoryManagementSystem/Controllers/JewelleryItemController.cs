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
    public class JewelleryItemController : ControllerBase
    {
        private readonly IJewelleryItemService _jewelleryItemService;
        private readonly ILogger<JewelleryItemController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public JewelleryItemController(IJewelleryItemService jewelleryItemService, IMapper mapper, ILogger<JewelleryItemController> logger, ICurrentUser currentUser)
        {
            _jewelleryItemService = jewelleryItemService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJewelleryItems()
        {
            _logger.LogInformation("Fetching all jewellery items");
            var jewelleryItems = await _jewelleryItemService.GetAllJewelleryItemsAsync();
            var jewelleryItemDtos = _mapper.Map<IEnumerable<JewelleryItemDto>>(jewelleryItems);
            return Ok(jewelleryItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetJewelleryItemById(long id)
        {
            _logger.LogInformation("Fetching jewellery item by ID: {Id}", id);
            var jewelleryItem = await _jewelleryItemService.GetJewelleryItemByIdAsync(id);
            if (jewelleryItem == null)
            {
                _logger.LogWarning("Jewellery item not found for ID: {Id}", id);
                return NotFound("Jewellery item not found");
            }
            var jewelleryItemDto = _mapper.Map<JewelleryItemDto>(jewelleryItem);
            return Ok(jewelleryItemDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJewelleryItem([FromBody] JewelleryItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating jewellery item: {Name}", dto.Name);

            var jewelleryItem = _mapper.Map<JewelleryItem>(dto);
            jewelleryItem.CreatedDate = DateTime.UtcNow;
            jewelleryItem.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            jewelleryItem.StatusId = (int)StatusEnum.Active;

            var createdJewelleryItem = await _jewelleryItemService.CreateJewelleryItemAsync(jewelleryItem);
            var createdDto = _mapper.Map<JewelleryItemDto>(createdJewelleryItem);

            _logger.LogInformation("Jewellery item created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetJewelleryItemById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJewelleryItem(long id, [FromBody] JewelleryItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating jewellery item ID: {Id}", id);

            var existingJewelleryItem = await _jewelleryItemService.GetJewelleryItemByIdAsync(id);
            if (existingJewelleryItem == null)
            {
                _logger.LogWarning("Jewellery item not found for update ID: {Id}", id);
                return NotFound("Jewellery item not found");
            }

            var jewelleryItem = _mapper.Map<JewelleryItem>(dto);
            jewelleryItem.Id = id;
            jewelleryItem.UpdatedDate = DateTime.UtcNow;
            jewelleryItem.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedJewelleryItem = await _jewelleryItemService.UpdateJewelleryItemAsync(jewelleryItem);
            var updatedDto = _mapper.Map<JewelleryItemDto>(updatedJewelleryItem);

            _logger.LogInformation("Jewellery item updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJewelleryItem(long id)
        {
            _logger.LogInformation("Deleting jewellery item ID: {Id}", id);

            var result = await _jewelleryItemService.DeleteJewelleryItemAsync(id);
            if (!result)
            {
                _logger.LogWarning("Jewellery item not found for deletion ID: {Id}", id);
                return NotFound("Jewellery item not found");
            }

            _logger.LogInformation("Jewellery item deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}