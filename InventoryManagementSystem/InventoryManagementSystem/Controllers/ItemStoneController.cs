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
    public class ItemStoneController : ControllerBase
    {
        private readonly IItemStoneService _itemStoneService;
        private readonly ILogger<ItemStoneController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ItemStoneController(IItemStoneService itemStoneService, IMapper mapper, ILogger<ItemStoneController> logger, ICurrentUser currentUser)
        {
            _itemStoneService = itemStoneService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemStones()
        {
            _logger.LogInformation("Fetching all item stones");
            var itemStones = await _itemStoneService.GetAllItemStonesAsync();
            var itemStoneDtos = _mapper.Map<IEnumerable<ItemStoneDto>>(itemStones);
            return Ok(itemStoneDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemStoneById(int id)
        {
            _logger.LogInformation("Fetching item stone by ID: {Id}", id);
            var itemStone = await _itemStoneService.GetItemStoneByIdAsync(id);
            if (itemStone == null)
            {
                _logger.LogWarning("Item stone not found for ID: {Id}", id);
                return NotFound("Item stone not found");
            }
            var itemStoneDto = _mapper.Map<ItemStoneDto>(itemStone);
            return Ok(itemStoneDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemStone([FromBody] ItemStoneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating item stone for ItemId: {ItemId}, StoneId: {StoneId}", dto.JewelleryItemId, dto.StoneId);

            var itemStone = _mapper.Map<ItemStone>(dto);
            itemStone.CreatedDate = DateTime.UtcNow;
            itemStone.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            itemStone.StatusId = (int)StatusEnum.Active;

            var createdItemStone = await _itemStoneService.CreateItemStoneAsync(itemStone);
            var createdDto = _mapper.Map<ItemStoneDto>(createdItemStone);

            _logger.LogInformation("Item stone created successfully ID: {Id}", createdDto.Id);
            return Ok( createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemStone(int id, [FromBody] ItemStoneDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating item stone ID: {Id}", id);

            var existingItemStone = await _itemStoneService.GetItemStoneByIdAsync(id);
            if (existingItemStone == null)
            {
                _logger.LogWarning("Item stone not found for update ID: {Id}", id);
                return NotFound("Item stone not found");
            }

            var itemStone = _mapper.Map<ItemStone>(dto);
            itemStone.Id = id;
            itemStone.UpdatedDate = DateTime.UtcNow;
            itemStone.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedItemStone = await _itemStoneService.UpdateItemStoneAsync(itemStone);
            var updatedDto = _mapper.Map<ItemStoneDto>(updatedItemStone);

            _logger.LogInformation("Item stone updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemStone(int id)
        {
            _logger.LogInformation("Deleting item stone ID: {Id}", id);

            var result = await _itemStoneService.DeleteItemStoneAsync(id);
            if (!result)
            {
                _logger.LogWarning("Item stone not found for deletion ID: {Id}", id);
                return NotFound("Item stone not found");
            }

            _logger.LogInformation("Item stone deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}