using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    /// <summary>
    /// Invoice API Controller
    /// Provides endpoints for generating and managing jewellery invoices
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(IInvoiceService invoiceService, ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>List of all invoices</returns>
        [HttpGet("GetAllInvoices")]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                _logger.LogInformation("Fetching all invoices");
                var invoices = await _invoiceService.GetAllInvoicesAsync();

                return Ok(new
                {
                    success = true,
                    data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all invoices");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving invoices" });
            }
        }

        /// <summary>
        /// Generate invoice from a sale order
        /// </summary>
        /// <param name="request">Invoice generation request</param>
        /// <returns>Complete invoice with jewellery-specific details</returns>
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateInvoice([FromBody] InvoiceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Invoice generation requested for Sale Order {SaleOrderId}", request.SaleOrderId);

                var invoice = await _invoiceService.GenerateInvoiceAsync(request);

                return Ok(new
                {
                    success = true,
                    message = "Invoice generated successfully",
                    data = invoice
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invoice generation failed - not found");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invoice generation failed");
                return StatusCode(500, new { success = false, message = "An error occurred while generating the invoice" });
            }
        }

        /// <summary>
        /// Get invoice by invoice number
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to search</param>
        /// <returns>Invoice if found</returns>
        [HttpGet("by-number")]
        public async Task<IActionResult> GetInvoiceByNumber([FromQuery] string invoiceNumber)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);

                if (invoice == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new
                {
                    success = true,
                    data = invoice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve invoice {InvoiceNumber}", invoiceNumber);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the invoice" });
            }
        }

        /// <summary>
        /// Get invoice by sale order ID
        /// </summary>
        /// <param name="saleOrderId">Sale order ID</param>
        /// <returns>Invoice if found</returns>
        [HttpGet("saleorder/{saleOrderId}")]
        public async Task<IActionResult> GetInvoiceBySaleOrderId(long saleOrderId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceBySaleOrderIdAsync(saleOrderId);

                if (invoice == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found for this sale order" });
                }

                return Ok(new
                {
                    success = true,
                    data = invoice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve invoice for Sale Order {SaleOrderId}", saleOrderId);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the invoice" });
            }
        }

        /// <summary>
        /// Generate bulk invoices for multiple sale orders
        /// </summary>
        /// <param name="request">Bulk invoice generation request</param>
        /// <returns>Bulk invoice generation result</returns>
        [HttpPost("generate-bulk")]
        public async Task<IActionResult> GenerateBulkInvoices([FromBody] BulkInvoiceRequestDto request)
        {
            try
            {
                _logger.LogInformation("Bulk invoice generation requested for {Count} sale orders", request.SaleOrderIds.Count);

                var result = await _invoiceService.GenerateBulkInvoicesAsync(request);

                return Ok(new
                {
                    success = true,
                    message = $"Generated {result.TotalGenerated} invoices, {result.TotalFailed} failed",
                    data = new
                    {
                        totalGenerated = result.TotalGenerated,
                        totalFailed = result.TotalFailed,
                        errors = result.Errors,
                        invoices = result.Invoices
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk invoice generation failed");
                return StatusCode(500, new { success = false, message = "An error occurred while generating bulk invoices" });
            }
        }

        /// <summary>
        /// Regenerate invoice with updated details
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to regenerate</param>
        /// <param name="notes">Optional notes to add</param>
        /// <returns>Updated invoice</returns>
        [HttpPost("regenerate")]
        public async Task<IActionResult> RegenerateInvoice([FromQuery] string invoiceNumber, [FromQuery] string? notes)
        {
            try
            {
                var invoice = await _invoiceService.RegenerateInvoiceAsync(invoiceNumber, notes);

                if (invoice == null)
                {
                    return NotFound(new { success = false, message = "Invoice not found" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Invoice regenerated successfully",
                    data = invoice
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invoice regeneration failed for {InvoiceNumber}", invoiceNumber);
                return StatusCode(500, new { success = false, message = "An error occurred while regenerating the invoice" });
            }
        }

        /// <summary>
        /// Cancel an invoice
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to cancel</param>
        /// <returns>Cancellation result</returns>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelInvoice([FromQuery] string invoiceNumber)
        {
            try
            {
                var result = await _invoiceService.CancelInvoiceAsync(invoiceNumber);

                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to cancel invoice" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Invoice cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invoice cancellation failed for {InvoiceNumber}", invoiceNumber);
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling the invoice" });
            }
        }

        /// <summary>
        /// Convert number to words (for grand total)
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>Number in words</returns>
        [HttpGet("number-to-words")]
        public IActionResult NumberToWords([FromQuery] decimal number)
        {
            try
            {
                var words = _invoiceService.NumberToWords(number);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        number = number,
                        words = words
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Number to words conversion failed for {Number}", number);
                return StatusCode(500, new { success = false, message = "An error occurred while converting number to words" });
            }
        }
    }
}
