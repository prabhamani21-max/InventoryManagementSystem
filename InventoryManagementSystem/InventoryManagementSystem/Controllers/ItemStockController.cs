using AutoMapper;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemStockController : ControllerBase
    {
        private readonly IItemStockService _itemStockService;
        private readonly ILogger<ItemStockController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ItemStockController(IItemStockService itemStockService, IMapper mapper, ILogger<ItemStockController> logger, ICurrentUser currentUser)
        {
            _itemStockService = itemStockService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItemStocks()
        {
            _logger.LogInformation("Fetching all item stocks");
            var itemStocks = await _itemStockService.GetAllItemStocksAsync();
            var itemStockDtos = _mapper.Map<IEnumerable<ItemStockDto>>(itemStocks);
            return Ok(itemStockDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemStockById(int id)
        {
            _logger.LogInformation("Fetching item stock by ID: {Id}", id);
            var itemStock = await _itemStockService.GetItemStockByIdAsync(id);
            if (itemStock == null)
            {
                _logger.LogWarning("Item stock not found for ID: {Id}", id);
                return NotFound("Item stock not found");
            }
            var itemStockDto = _mapper.Map<ItemStockDto>(itemStock);
            return Ok(itemStockDto);
        }

        [HttpGet("by-item/{jewelleryItemId}")]
        public async Task<IActionResult> GetItemStockByJewelleryItemId(long jewelleryItemId, [FromQuery] int? warehouseId = null)
        {
            _logger.LogInformation("Fetching item stock by JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
            var itemStock = await _itemStockService.GetItemStockByJewelleryItemIdAsync(jewelleryItemId, warehouseId);
            if (itemStock == null)
            {
                _logger.LogWarning("Item stock not found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                return NotFound("Item stock not found");
            }
            var itemStockDto = _mapper.Map<ItemStockDto>(itemStock);
            return Ok(itemStockDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateItemStock([FromBody] ItemStockDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating item stock for JewelleryItemId: {JewelleryItemId}, WarehouseId: {WarehouseId}", dto.JewelleryItemId, dto.WarehouseId);

            var itemStock = _mapper.Map<ItemStock>(dto);
            itemStock.CreatedDate = DateTime.UtcNow;
            itemStock.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var createdItemStock = await _itemStockService.CreateItemStockAsync(itemStock);
            var createdDto = _mapper.Map<ItemStockDto>(createdItemStock);

            _logger.LogInformation("Item stock created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetItemStockById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemStock(int id, [FromBody] ItemStockDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating item stock ID: {Id}", id);

            var existingItemStock = await _itemStockService.GetItemStockByIdAsync(id);
            if (existingItemStock == null)
            {
                _logger.LogWarning("Item stock not found for update ID: {Id}", id);
                return NotFound("Item stock not found");
            }

            var itemStock = _mapper.Map<ItemStock>(dto);
            itemStock.Id = id;
            itemStock.UpdatedDate = DateTime.UtcNow;
            itemStock.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedItemStock = await _itemStockService.UpdateItemStockAsync(itemStock);
            var updatedDto = _mapper.Map<ItemStockDto>(updatedItemStock);

            _logger.LogInformation("Item stock updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemStock(int id)
        {
            _logger.LogInformation("Deleting item stock ID: {Id}", id);

            var result = await _itemStockService.DeleteItemStockAsync(id);
            if (!result)
            {
                _logger.LogWarning("Item stock not found for deletion ID: {Id}", id);
                return NotFound("Item stock not found");
            }

            _logger.LogInformation("Item stock deleted successfully ID: {Id}", id);
            return NoContent();
        }

        // ==================== STOCK VALIDATION ENDPOINTS ====================

        /// <summary>
        /// Checks if sufficient stock is available for a specific item
        /// </summary>
        [HttpGet("check-availability/{jewelleryItemId}")]
        public async Task<IActionResult> CheckStockAvailability(long jewelleryItemId, [FromQuery] int quantity, [FromQuery] int? warehouseId = null)
        {
            _logger.LogInformation("Checking stock availability for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}", jewelleryItemId, quantity);
            
            var isAvailable = await _itemStockService.CheckStockAvailabilityAsync(jewelleryItemId, quantity, warehouseId);
            
            return Ok(new { 
                JewelleryItemId = jewelleryItemId, 
                RequestedQuantity = quantity, 
                IsAvailable = isAvailable 
            });
        }

        /// <summary>
        /// Validates stock for multiple items (useful for order validation before submission)
        /// </summary>
        [HttpPost("validate-order-stock")]
        public async Task<IActionResult> ValidateOrderStock([FromBody] List<StockValidationRequestDto> items)
        {
            _logger.LogInformation("Validating stock for {Count} items", items.Count);

            var validationRequests = items.Select(i => new StockValidationRequest
            {
                JewelleryItemId = i.JewelleryItemId,
                RequestedQuantity = i.RequestedQuantity,
                WarehouseId = i.WarehouseId
            });

            var result = await _itemStockService.ValidateStockForOrderAsync(validationRequests);
            
            return Ok(result);
        }
    }

    public class StockValidationRequestDto
    {
        public long JewelleryItemId { get; set; }
        public int RequestedQuantity { get; set; }
        public int? WarehouseId { get; set; }
    }
}