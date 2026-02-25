
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.DTO;

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

        public ExchangeController(
            IExchangeService exchangeService,
            ILogger<ExchangeController> logger,
            IMapper mapper)
        {
            _exchangeService = exchangeService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Calculate exchange value (preview without creating order)
        /// </summary>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateExchangeValue([FromBody] ExchangeCalculateRequestDto request)
        {
            try
            {
                _logger.LogInformation("Calculating exchange value for customer {CustomerId}", request.CustomerId);
                
                // Map DTO to Model
                var calculationRequest = new ExchangeCalculationRequest
                {
                    CustomerId = request.CustomerId,
                    ExchangeType = (int)request.ExchangeType,
                    Items = _mapper.Map<List<ExchangeItemInput>>(request.Items),
                    NewPurchaseAmount = request.NewPurchaseAmount,
                    Notes = request.Notes
                };
                
                var result = await _exchangeService.CalculateExchangeValueAsync(calculationRequest);
                
                // Map Model to DTO for response
                var response = _mapper.Map<ExchangeCalculateResponseDto>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating exchange value");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Create new exchange/buyback order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateExchangeOrder([FromBody] ExchangeOrderCreateDto request)
        {
            try
            {
                _logger.LogInformation("Creating exchange order for customer {CustomerId}", request.CustomerId);
                
                // Map DTO to Model
                var exchangeOrder = _mapper.Map<ExchangeOrder>(request);
                
                var result = await _exchangeService.CreateExchangeOrderAsync(exchangeOrder);
                
                // Map Model to DTO for response
                var response = _mapper.Map<ExchangeOrderDto>(result);
                return CreatedAtAction(nameof(GetExchangeOrder), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating exchange order");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Get exchange order by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetExchangeOrder(long id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchange order {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Get exchange order by order number
        /// </summary>
        [HttpGet("orderNumber/{orderNumber}")]
        public async Task<IActionResult> GetExchangeOrderByOrderNumber(string orderNumber)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchange order {OrderNumber}", orderNumber);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Get all exchange orders
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllExchangeOrders()
        {
            try
            {
                var result = await _exchangeService.GetAllExchangeOrdersAsync();
                
                // Map Model to DTO for response
                var response = _mapper.Map<IEnumerable<ExchangeOrderDto>>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all exchange orders");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Get exchange orders by customer ID
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetExchangeOrdersByCustomer(long customerId)
        {
            try
            {
                var result = await _exchangeService.GetExchangeOrdersByCustomerIdAsync(customerId);
                
                // Map Model to DTO for response
                var response = _mapper.Map<IEnumerable<ExchangeOrderDto>>(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchange orders for customer {CustomerId}", customerId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Complete exchange order (add to inventory, apply credit)
        /// </summary>
        [HttpPost("{orderId}/complete")]
        public async Task<IActionResult> CompleteExchangeOrder(long orderId, [FromBody] ExchangeCompleteDto request)
        {
            try
            {
                _logger.LogInformation("Completing exchange order {OrderId}", orderId);
                
                var result = await _exchangeService.CompleteExchangeOrderAsync(orderId, request.Notes);
                
                // Map Model to DTO for response
                var response = _mapper.Map<ExchangeOrderDto>(result);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing exchange order {OrderId}", orderId);
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel exchange order
        /// </summary>
        [HttpPost("{orderId}/cancel")]
        public async Task<IActionResult> CancelExchangeOrder(long orderId, [FromBody] CancelRequestDto request)
        {
            try
            {
                _logger.LogInformation("Cancelling exchange order {OrderId}", orderId);
                
                var result = await _exchangeService.CancelExchangeOrderAsync(orderId, request.Reason);
                if (!result)
                {
                    return NotFound(new { Message = $"Exchange order {orderId} not found" });
                }
                return Ok(new { Message = "Exchange order cancelled successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling exchange order {OrderId}", orderId);
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

    public class CancelRequestDto
    {
        public string? Reason { get; set; }
    }
}
