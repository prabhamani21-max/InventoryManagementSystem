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
    public class SaleOrderController : ControllerBase
    {
        private readonly ISaleOrderService _saleOrderService;
        private readonly ILogger<SaleOrderController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public SaleOrderController(ISaleOrderService saleOrderService, IMapper mapper, ILogger<SaleOrderController> logger, ICurrentUser currentUser)
        {
            _saleOrderService = saleOrderService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSaleOrders()
        {
            _logger.LogInformation("Fetching all sale orders");
            var saleOrders = await _saleOrderService.GetAllSaleOrdersAsync();
            var saleOrderDtos = _mapper.Map<IEnumerable<SaleOrderDto>>(saleOrders);
            return Ok(saleOrderDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleOrderById(int id)
        {
            _logger.LogInformation("Fetching sale order by ID: {Id}", id);
            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (saleOrder == null)
            {
                _logger.LogWarning("Sale order not found for ID: {Id}", id);
                return NotFound("Sale order not found");
            }
            var saleOrderDto = _mapper.Map<SaleOrderDto>(saleOrder);
            return Ok(saleOrderDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaleOrder([FromBody] SaleOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Auto-generate OrderNumber if not provided
            var orderNumber = string.IsNullOrWhiteSpace(dto.OrderNumber)
                ? $"SO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}"
                : dto.OrderNumber;

            _logger.LogInformation("Creating sale order: {OrderNumber}", orderNumber);

            var saleOrder = _mapper.Map<SaleOrder>(dto);
            saleOrder.OrderNumber = orderNumber;
            saleOrder.CreatedDate = DateTime.UtcNow;
            saleOrder.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            saleOrder.StatusId = (int)StatusEnum.Active;

            var createdSaleOrder = await _saleOrderService.CreateSaleOrderAsync(saleOrder);
            var createdDto = _mapper.Map<SaleOrderDto>(createdSaleOrder);

            _logger.LogInformation("Sale order created successfully ID: {Id}, OrderNumber: {OrderNumber}", createdDto.Id, createdDto.OrderNumber);
            return CreatedAtAction(nameof(GetSaleOrderById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleOrder(int id, [FromBody] SaleOrderDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating sale order ID: {Id}", id);

            var existingSaleOrder = await _saleOrderService.GetSaleOrderByIdAsync(id);
            if (existingSaleOrder == null)
            {
                _logger.LogWarning("Sale order not found for update ID: {Id}", id);
                return NotFound("Sale order not found");
            }

            var saleOrder = _mapper.Map<SaleOrder>(dto);
            saleOrder.Id = id;
            saleOrder.UpdatedDate = DateTime.UtcNow;
            saleOrder.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedSaleOrder = await _saleOrderService.UpdateSaleOrderAsync(saleOrder);
            var updatedDto = _mapper.Map<SaleOrderDto>(updatedSaleOrder);

            _logger.LogInformation("Sale order updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleOrder(int id)
        {
            _logger.LogInformation("Deleting sale order ID: {Id}", id);

            var result = await _saleOrderService.DeleteSaleOrderAsync(id);
            if (!result)
            {
                _logger.LogWarning("Sale order not found for deletion ID: {Id}", id);
                return NotFound("Sale order not found");
            }

            _logger.LogInformation("Sale order deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}