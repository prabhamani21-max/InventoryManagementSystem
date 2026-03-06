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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentValidationService _paymentValidationService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PaymentController(
            IPaymentService paymentService,
            IPaymentValidationService paymentValidationService,
            IMapper mapper,
            ILogger<PaymentController> logger,
            ICurrentUser currentUser)
        {
            _paymentService = paymentService;
            _paymentValidationService = paymentValidationService;
            _mapper = mapper;
            _logger = logger;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPayments()
        {
            _logger.LogInformation("Fetching all payments");
            var payments = await _paymentService.GetAllPaymentsAsync();
            var paymentDtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
            return Ok(paymentDtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            _logger.LogInformation("Fetching payment by ID: {Id}", id);
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for ID: {Id}", id);
                return NotFound("Payment not found");
            }
            var paymentDto = _mapper.Map<PaymentDto>(payment);
            return Ok(paymentDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentDto dto)
        {
           // if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Creating payment for OrderId: {OrderId}", dto.OrderId);

            // Validate high-value transaction rules
            var customerId = dto.CustomerId ?? 0;
            var orderTotal = dto.OrderTotal ?? dto.Amount;
            
            if (customerId > 0)
            {
                var validation = await _paymentValidationService.ValidatePaymentAsync(
                    customerId,
                    dto.Amount,
                    dto.PaymentMethod,
                    orderTotal
                );

                if (!validation.IsValid)
                {
                    _logger.LogWarning(
                        "Payment validation failed for CustomerId: {CustomerId}, ErrorCode: {ErrorCode}",
                        customerId,
                        validation.ErrorCode
                    );
                    
                    return BadRequest(new
                    {
                        error = validation.ErrorCode,
                        message = validation.ErrorMessage,
                        requiresKyc = validation.RequiresKyc,
                        isHighValueTransaction = validation.IsHighValueTransaction,
                        cashPaymentDisabled = validation.CashPaymentDisabled
                    });
                }
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.CreatedDate = DateTime.UtcNow;
            payment.CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            payment.StatusId = (int)StatusEnum.Active;

            var createdPayment = await _paymentService.CreatePaymentAsync(payment);
            var createdDto = _mapper.Map<PaymentDto>(createdPayment);

            _logger.LogInformation("Payment created successfully ID: {Id}", createdDto.Id);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] PaymentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("Updating payment ID: {Id}", id);

            var existingPayment = await _paymentService.GetPaymentByIdAsync(id);
            if (existingPayment == null)
            {
                _logger.LogWarning("Payment not found for update ID: {Id}", id);
                return NotFound("Payment not found");
            }

            var payment = _mapper.Map<Payment>(dto);
            payment.Id = id;
            payment.UpdatedDate = DateTime.UtcNow;
            payment.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            var updatedPayment = await _paymentService.UpdatePaymentAsync(payment);
            var updatedDto = _mapper.Map<PaymentDto>(updatedPayment);

            _logger.LogInformation("Payment updated successfully ID: {Id}", id);
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            _logger.LogInformation("Deleting payment ID: {Id}", id);

            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
            {
                _logger.LogWarning("Payment not found for deletion ID: {Id}", id);
                return NotFound("Payment not found");
            }

            _logger.LogInformation("Payment deleted successfully ID: {Id}", id);
            return NoContent();
        }

        /// <summary>
        /// Validates if a payment can be made for a high-value transaction
        /// </summary>
        /// <param name="request">Validation request with customer ID and order total</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate")]
        public async Task<IActionResult> ValidatePayment([FromBody] PaymentValidationRequest request)
        {
            _logger.LogInformation(
                "Validating payment for CustomerId: {CustomerId}, OrderTotal: {OrderTotal}",
                request.CustomerId,
                request.OrderTotal
            );

            var isHighValue = _paymentValidationService.IsHighValueTransaction(request.OrderTotal);
            
            if (!isHighValue)
            {
                return Ok(new
                {
                    isValid = true,
                    isHighValueTransaction = false,
                    cashPaymentDisabled = false,
                    requiresKyc = false
                });
            }

            var (hasKyc, isVerified) = await _paymentValidationService.GetCustomerKycStatusAsync(request.CustomerId);

            return Ok(new
            {
                isValid = isVerified,
                isHighValueTransaction = true,
                cashPaymentDisabled = isVerified,
                requiresKyc = !isVerified,
                hasKyc = hasKyc,
                isKycVerified = isVerified
            });
        }
    }

    /// <summary>
    /// Request model for payment validation
    /// </summary>
    public class PaymentValidationRequest
    {
        public long CustomerId { get; set; }
        public decimal OrderTotal { get; set; }
    }
}