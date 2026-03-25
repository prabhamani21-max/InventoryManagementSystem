using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        private readonly IInvoicePdfService _pdfService;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            IInvoicePdfService pdfService,
            ICurrentUser currentUser,
            ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _pdfService = pdfService;
            _currentUser = currentUser;
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
        /// Get invoices for the currently logged-in customer
        /// </summary>
        /// <returns>List of invoices for the current customer</returns>
        [HttpGet("my-invoices")]
        public async Task<IActionResult> GetMyInvoices()
        {
            try
            {
                var userId = _currentUser?.UserId;
                if (userId == null || userId <= 0)
                {
                    _logger.LogWarning("Unable to determine current user ID");
                    return Unauthorized(new { success = false, message = "Unable to determine user identity" });
                }

                _logger.LogInformation("Fetching invoices for customer ID: {CustomerId}", userId);
                var invoices = await _invoiceService.GetInvoicesByPartyIdAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve customer invoices");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving your invoices" });
            }
        }

        /// <summary>
        /// Get invoices for orders created by the currently logged-in sales person
        /// </summary>
        /// <returns>List of invoices for orders created by the current sales person</returns>
        [HttpGet("my-sales-invoices")]
        public async Task<IActionResult> GetMySalesInvoices()
        {
            try
            {
                var userId = _currentUser?.UserId;
                if (userId == null || userId <= 0)
                {
                    _logger.LogWarning("Unable to determine current user ID");
                    return Unauthorized(new { success = false, message = "Unable to determine user identity" });
                }

                _logger.LogInformation("Fetching invoices for sales person ID: {UserId}", userId);
                var invoices = await _invoiceService.GetInvoicesByCreatedByAsync(userId.Value);

                return Ok(new
                {
                    success = true,
                    data = invoices
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve sales person invoices");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving your invoices" });
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
                if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning(ex, "Invoice generation failed - resource not found");
                    return NotFound(new { success = false, message = ex.Message });
                }

                _logger.LogWarning(ex, "Invoice generation failed - invalid operation");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Invoice generation failed - database update error");
                return StatusCode(500, new { success = false, message = "A database error occurred while generating the invoice" });
            }
            catch (PostgresException ex)
            {
                _logger.LogError(ex, "Invoice generation failed - postgres error");
                return StatusCode(500, new { success = false, message = "A database error occurred while generating the invoice" });
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
        /// Download invoice as PDF by invoice ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>PDF file</returns>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadInvoicePdf(long id)
        {
            try
            {
                _logger.LogInformation("PDF download requested for invoice ID: {InvoiceId}", id);

                var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(id);
                var invoice = await _invoiceService.GetInvoiceByIdAsync(id);

                if (pdfBytes == null || pdfBytes.Length == 0)
                {
                    _logger.LogWarning("PDF generation returned no content for invoice ID: {InvoiceId}", id);
                    return NotFound(new { success = false, message = "Invoice not found or could not generate PDF" });
                }

                var fileName = $"Invoice_{invoice?.InvoiceNumber ?? id.ToString()}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate PDF for invoice ID {InvoiceId}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while generating the invoice PDF" });
            }
        }

        /// <summary>
        /// Download invoice as PDF by invoice number.
        /// Uses a query string so invoice numbers containing '/' still resolve correctly.
        /// </summary>
        /// <param name="invoiceNumber">Invoice number</param>
        /// <returns>PDF file</returns>
        [HttpGet("by-number/download")]
        public async Task<IActionResult> DownloadInvoicePdfByNumber([FromQuery] string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
            {
                _logger.LogWarning("PDF download requested without an invoice number");
                return BadRequest(new { success = false, message = "Invoice number is required" });
            }

            _logger.LogInformation("PDF download requested for invoice: {InvoiceNumber}", invoiceNumber);

            var pdfBytes = await _pdfService.GenerateInvoicePdfByNumberAsync(invoiceNumber);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                _logger.LogWarning("PDF generation returned no content for invoice {InvoiceNumber}", invoiceNumber);
                return NotFound(new { success = false, message = "Invoice not found or could not generate PDF" });
            }

            var fileName = $"Invoice_{invoiceNumber}.pdf";
            return File(pdfBytes, "application/pdf", fileName);
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
