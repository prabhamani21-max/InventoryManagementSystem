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
    public class ExchangeController : ControllerBase
    {
        private readonly IExchangeService _exchangeService;
        private readonly ILogger<ExchangeController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ExchangeController(
            IExchangeService exchangeService,
            ILogger<ExchangeController> logger,
            IMapper mapper,
            ICurrentUser currentUser)
        {
            _exchangeService = exchangeService;
            _logger = logger;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Calculate exchange value (preview without creating order)
        /// </summary>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateExchangeValue([FromBody] ExchangeCalculateRequestDto request)
        {
            _logger.LogInformation("Calculating exchange value for customer {CustomerId}", request.CustomerId);

            // Map DTO to Model
            var calculationRequest = new ExchangeCalculationRequest
            {
                CustomerId = request.CustomerId,
                ExchangeType = (int)request.ExchangeType,
                Items = _mapper.Map<List<ExchangeItemInput>>(request.Items),
                Notes = request.Notes
            };

            var result = await _exchangeService.CalculateExchangeValueAsync(calculationRequest);

            // Map Model to DTO for response
            var response = _mapper.Map<ExchangeCalculateResponseDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Create new exchange/buyback order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExchangeOrder([FromBody] ExchangeOrderCreateDto request)
        {
            _logger.LogInformation("Creating exchange order for customer {CustomerId}", request.CustomerId);

            // Map DTO to Model
            var exchangeOrder = _mapper.Map<ExchangeOrder>(request);
            exchangeOrder.CreatedDate = DateTime.UtcNow;
            exchangeOrder.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            exchangeOrder.StatusId = (int)StatusEnum.Active;

            var result = await _exchangeService.CreateExchangeOrderAsync(exchangeOrder);

            // Map Model to DTO for response
            var response = _mapper.Map<ExchangeOrderDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Link a sale order to an exchange order for Phase 1 settlement.
        /// </summary>
        [HttpPost("{orderId}/link-sale")]
        public async Task<IActionResult> LinkSaleOrder(long orderId, [FromBody] ExchangeLinkSaleDto request)
        {
            _logger.LogInformation("Linking sale order {SaleOrderId} to exchange order {OrderId}", request.SaleOrderId, orderId);

            var result = await _exchangeService.LinkSaleOrderAsync(orderId, request.SaleOrderId);
            var response = _mapper.Map<ExchangeOrderDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Get exchange order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExchangeOrder(long id)
        {
            var result = await _exchangeService.GetExchangeOrderByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { Message = $"Exchange order {id} not found" });
            }

            // Map Model to DTO for response
            var response = _mapper.Map<ExchangeOrderDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Get exchange order by order number
        /// </summary>
        [HttpGet("orderNumber/{orderNumber}")]
        public async Task<IActionResult> GetExchangeOrderByOrderNumber(string orderNumber)
        {
            var result = await _exchangeService.GetExchangeOrderByOrderNumberAsync(orderNumber);
            if (result == null)
            {
                return NotFound(new { Message = $"Exchange order {orderNumber} not found" });
            }

            // Map Model to DTO for response
            var response = _mapper.Map<ExchangeOrderDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Get all exchange orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllExchangeOrders()
        {
            var result = await _exchangeService.GetAllExchangeOrdersAsync();

            // Map Model to DTO for response
            var response = _mapper.Map<IEnumerable<ExchangeOrderDto>>(result);
            return Ok(response);
        }

        /// <summary>
        /// Get exchange orders by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetExchangeOrdersByCustomer(long customerId)
        {
            var result = await _exchangeService.GetExchangeOrdersByCustomerIdAsync(customerId);

            // Map Model to DTO for response
            var response = _mapper.Map<IEnumerable<ExchangeOrderDto>>(result);
            return Ok(response);
        }

        /// <summary>
        /// Get exchange orders created by the currently logged-in sales person
        /// </summary>
        /// <returns>List of exchange orders created by the current sales person</returns>
        [HttpGet("my-exchanges")]
        public async Task<IActionResult> GetMyExchanges()
        {
            try
            {
                var userId = _currentUser?.UserId;
                if (userId == null || userId <= 0)
                {
                    _logger.LogWarning("Unable to determine current user ID");
                    return Unauthorized(new { success = false, message = "Unable to determine user identity" });
                }

                _logger.LogInformation("Fetching exchange orders created by sales person ID: {UserId}", userId);
                var result = await _exchangeService.GetExchangeOrdersByCreatedByAsync(userId.Value);
                
                // Map Model to DTO for response
                var response = _mapper.Map<IEnumerable<ExchangeOrderDto>>(result);
                return Ok(new { success = true, data = response });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchange orders for sales person");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Complete exchange order after linked sale and invoice validation.
        /// </summary>
        [HttpPost("{orderId}/complete")]
        public async Task<IActionResult> CompleteExchangeOrder(long orderId, [FromBody] ExchangeCompleteDto request)
        {
            _logger.LogInformation("Completing exchange order {OrderId}", orderId);

            var result = await _exchangeService.CompleteExchangeOrderAsync(orderId, request.Notes);

            // Map Model to DTO for response
            var response = _mapper.Map<ExchangeOrderDto>(result);
            return Ok(response);
        }

        /// <summary>
        /// Cancel exchange order
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelExchangeOrder(long orderId, [FromBody] CancelRequestDto request)
        {
            _logger.LogInformation("Cancelling exchange order {OrderId}", orderId);

            var result = await _exchangeService.CancelExchangeOrderAsync(orderId, request.Reason);
            if (!result)
            {
                return NotFound(new { Message = $"Exchange order {orderId} not found" });
            }

            return Ok(new { Message = "Exchange order cancelled successfully" });
        }
    }

    public class CancelRequestDto
    {
        public string? Reason { get; set; }
    }
}
