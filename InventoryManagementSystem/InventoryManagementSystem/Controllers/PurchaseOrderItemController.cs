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
    public class PurchaseOrderItemController : ControllerBase
    {
        private readonly IPurchaseOrderItemService _purchaseOrderItemService;
        private readonly ILogger<PurchaseOrderItemController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PurchaseOrderItemController(IPurchaseOrderItemService purchaseOrderItemService, IMapper mapper, ILogger<PurchaseOrderItemController> logger, ICurrentUser currentUser)
        {
            _purchaseOrderItemService = purchaseOrderItemService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrderItems()
        {
            _logger.LogInformation("Fetching all purchase order items");
            var purchaseOrderItems = await _purchaseOrderItemService.GetAllPurchaseOrderItemsAsync();
            var purchaseOrderItemDtos = _mapper.Map<IEnumerable<PurchaseOrderItemDto>>(purchaseOrderItems);
            return Ok(purchaseOrderItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderItemById(int id)
        {
            _logger.LogInformation("Fetching purchase order item by ID: {Id}", id);
            var purchaseOrderItem = await _purchaseOrderItemService.GetPurchaseOrderItemByIdAsync(id);
            if (purchaseOrderItem == null)
            {
                _logger.LogWarning("Purchase order item not found for ID: {Id}", id);
                return NotFound("Purchase order item not found");
            }
            var purchaseOrderItemDto = _mapper.Map<PurchaseOrderItemDto>(purchaseOrderItem);
            return Ok(purchaseOrderItemDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrderItem([FromBody] PurchaseOrderItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating purchase order item");

            var purchaseOrderItem = _mapper.Map<PurchaseOrderItem>(dto);
            purchaseOrderItem.CreatedDate = DateTime.UtcNow;
            purchaseOrderItem.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            purchaseOrderItem.StatusId = (int)StatusEnum.Active;

            var createdPurchaseOrderItem = await _purchaseOrderItemService.CreatePurchaseOrderItemAsync(purchaseOrderItem);
            var createdDto = _mapper.Map<PurchaseOrderItemDto>(createdPurchaseOrderItem);

            _logger.LogInformation("Purchase order item created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetPurchaseOrderItemById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrderItem(int id, [FromBody] PurchaseOrderItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating purchase order item ID: {Id}", id);

            var existingPurchaseOrderItem = await _purchaseOrderItemService.GetPurchaseOrderItemByIdAsync(id);
            if (existingPurchaseOrderItem == null)
            {
                _logger.LogWarning("Purchase order item not found for update ID: {Id}", id);
                return NotFound("Purchase order item not found");
            }

            var purchaseOrderItem = _mapper.Map<PurchaseOrderItem>(dto);
            purchaseOrderItem.Id = id;
            purchaseOrderItem.UpdatedDate = DateTime.UtcNow;
            purchaseOrderItem.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedPurchaseOrderItem = await _purchaseOrderItemService.UpdatePurchaseOrderItemAsync(purchaseOrderItem);
            var updatedDto = _mapper.Map<PurchaseOrderItemDto>(updatedPurchaseOrderItem);

            _logger.LogInformation("Purchase order item updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderItem(int id)
        {
            _logger.LogInformation("Deleting purchase order item ID: {Id}", id);

            var result = await _purchaseOrderItemService.DeletePurchaseOrderItemAsync(id);
            if (!result)
            {
                _logger.LogWarning("Purchase order item not found for deletion ID: {Id}", id);
                return NotFound("Purchase order item not found");
            }

            _logger.LogInformation("Purchase order item deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}