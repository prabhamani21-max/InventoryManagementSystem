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
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ILogger<PurchaseOrderController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PurchaseOrderController(IPurchaseOrderService purchaseOrderService, IMapper mapper, ILogger<PurchaseOrderController> logger, ICurrentUser currentUser)
        {
            _purchaseOrderService = purchaseOrderService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchaseOrders()
        {
            _logger.LogInformation("Fetching all purchase orders");
            var purchaseOrders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
            var purchaseOrderDtos = _mapper.Map<IEnumerable<PurchaseOrderDto>>(purchaseOrders);
            return Ok(purchaseOrderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderById(int id)
        {
            _logger.LogInformation("Fetching purchase order by ID: {Id}", id);
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (purchaseOrder == null)
            {
                _logger.LogWarning("Purchase order not found for ID: {Id}", id);
                return NotFound("Purchase order not found");
            }
            var purchaseOrderDto = _mapper.Map<PurchaseOrderDto>(purchaseOrder);
            return Ok(purchaseOrderDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] PurchaseOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating purchase order: {OrderNumber}", dto.OrderNumber);

            var purchaseOrder = _mapper.Map<PurchaseOrder>(dto);
            purchaseOrder.CreatedDate = DateTime.UtcNow;
            purchaseOrder.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            purchaseOrder.StatusId = (int)StatusEnum.Active;

            var createdPurchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrder);
            var createdDto = _mapper.Map<PurchaseOrderDto>(createdPurchaseOrder);

            _logger.LogInformation("Purchase order created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetPurchaseOrderById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] PurchaseOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating purchase order ID: {Id}", id);

            var existingPurchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (existingPurchaseOrder == null)
            {
                _logger.LogWarning("Purchase order not found for update ID: {Id}", id);
                return NotFound("Purchase order not found");
            }

            var purchaseOrder = _mapper.Map<PurchaseOrder>(dto);
            purchaseOrder.Id = id;
            purchaseOrder.UpdatedDate = DateTime.UtcNow;
            purchaseOrder.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedPurchaseOrder = await _purchaseOrderService.UpdatePurchaseOrderAsync(purchaseOrder);
            var updatedDto = _mapper.Map<PurchaseOrderDto>(updatedPurchaseOrder);

            _logger.LogInformation("Purchase order updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            _logger.LogInformation("Deleting purchase order ID: {Id}", id);

            var result = await _purchaseOrderService.DeletePurchaseOrderAsync(id);
            if (!result)
            {
                _logger.LogWarning("Purchase order not found for deletion ID: {Id}", id);
                return NotFound("Purchase order not found");
            }

            _logger.LogInformation("Purchase order deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}