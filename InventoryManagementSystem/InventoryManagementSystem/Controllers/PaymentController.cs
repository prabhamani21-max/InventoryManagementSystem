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
        private readonly ILogger<PaymentController> _logger;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public PaymentController(IPaymentService paymentService, IMapper mapper, ILogger<PaymentController> logger, ICurrentUser currentUser)
        {
            _paymentService = paymentService;
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
    }
}