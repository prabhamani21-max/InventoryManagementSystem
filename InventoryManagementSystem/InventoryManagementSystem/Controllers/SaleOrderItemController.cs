using AutoMapper;
using FluentValidation;
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
    public class SaleOrderItemController : ControllerBase
    {
        private readonly ISaleOrderItemService _saleOrderItemService;
        private readonly ILogger<SaleOrderItemController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
      //  private readonly IValidator<CreateSaleOrderItemDto> _createValidator;

        public SaleOrderItemController(
            ISaleOrderItemService saleOrderItemService,
            IMapper mapper,
            ILogger<SaleOrderItemController> logger,
            ICurrentUser currentUser
)        {
            _saleOrderItemService = saleOrderItemService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        //    _createValidator = createValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSaleOrderItems()
        {
            _logger.LogInformation("Fetching all sale order items");
            var saleOrderItems = await _saleOrderItemService.GetAllSaleOrderItemsAsync();
            var saleOrderItemDtos = _mapper.Map<IEnumerable<SaleOrderItemDto>>(saleOrderItems);
            return Ok(saleOrderItemDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSaleOrderItemById(int id)
        {
            _logger.LogInformation("Fetching sale order item by ID: {Id}", id);
            var saleOrderItem = await _saleOrderItemService.GetSaleOrderItemByIdAsync(id);
            if (saleOrderItem == null)
            {
                _logger.LogWarning("Sale order item not found for ID: {Id}", id);
                return NotFound("Sale order item not found");
            }
            var saleOrderItemDto = _mapper.Map<SaleOrderItemDto>(saleOrderItem);
            return Ok(saleOrderItemDto);
        }

        /// <summary>
        /// Creates a sale order item with AUTOMATIC PRICE CALCULATION.
        /// Metal rate, making charges, wastage, and tax are calculated server-side.
        /// </summary>
        [HttpPost("calculate")]
        public async Task<IActionResult> CreateSaleOrderItemWithCalculation([FromBody] CreateSaleOrderItemDto dto)
        {
            // Validate the input DTO
            //var validationResult = await _createValidator.ValidateAsync(dto);
            //if (!validationResult.IsValid)
            //{
            //    _logger.LogWarning("Validation failed for CreateSaleOrderItemDto: {Errors}", validationResult.Errors);
            //    return BadRequest(validationResult.Errors);
            //}

            _logger.LogInformation("Creating sale order item with automatic price calculation for JewelleryItemId: {JewelleryItemId}", dto.JewelleryItemId);

            try
            {
                var createdSaleOrderItem = await _saleOrderItemService.CreateSaleOrderItemWithCalculationAsync(
                    saleOrderId: dto.SaleOrderId,
                    jewelleryItemId: dto.JewelleryItemId,
                    discountAmount: dto.DiscountAmount,
                    gstPercentage: dto.GstPercentage,
                    stoneAmount: dto.StoneAmount,
                    quantity: dto.Quantity);
                var createdDto = _mapper.Map<SaleOrderItemDto>(createdSaleOrderItem);

                _logger.LogInformation("Sale order item created successfully with automatic calculation. ID: {Id}, TotalAmount: {TotalAmount}", 
                    createdDto.Id, createdDto.TotalAmount);
                return CreatedAtAction(nameof(GetSaleOrderItemById), new { id = createdDto.Id }, createdDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create sale order item: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSaleOrderItem([FromBody] SaleOrderItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating sale order item");

            var saleOrderItem = _mapper.Map<SaleOrderItem>(dto);
            saleOrderItem.CreatedDate = DateTime.UtcNow;
            saleOrderItem.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            saleOrderItem.StatusId = (int)StatusEnum.Active;

            var createdSaleOrderItem = await _saleOrderItemService.CreateSaleOrderItemAsync(saleOrderItem);
            var createdDto = _mapper.Map<SaleOrderItemDto>(createdSaleOrderItem);

            _logger.LogInformation("Sale order item created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetSaleOrderItemById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSaleOrderItem(int id, [FromBody] SaleOrderItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating sale order item ID: {Id}", id);

            var existingSaleOrderItem = await _saleOrderItemService.GetSaleOrderItemByIdAsync(id);
            if (existingSaleOrderItem == null)
            {
                _logger.LogWarning("Sale order item not found for update ID: {Id}", id);
                return NotFound("Sale order item not found");
            }

            var saleOrderItem = _mapper.Map<SaleOrderItem>(dto);
            saleOrderItem.Id = id;
            saleOrderItem.UpdatedDate = DateTime.UtcNow;
            saleOrderItem.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedSaleOrderItem = await _saleOrderItemService.UpdateSaleOrderItemAsync(saleOrderItem);
            var updatedDto = _mapper.Map<SaleOrderItemDto>(updatedSaleOrderItem);

            _logger.LogInformation("Sale order item updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSaleOrderItem(int id)
        {
            _logger.LogInformation("Deleting sale order item ID: {Id}", id);

            var result = await _saleOrderItemService.DeleteSaleOrderItemAsync(id);
            if (!result)
            {
                _logger.LogWarning("Sale order item not found for deletion ID: {Id}", id);
                return NotFound("Sale order item not found");
            }

            _logger.LogInformation("Sale order item deleted successfully ID: {Id}", id);
            return NoContent();
        }
    }
}