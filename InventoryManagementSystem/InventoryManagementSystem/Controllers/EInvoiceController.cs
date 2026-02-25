using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    /// <summary>
    /// E-Invoice API Controller for GST Compliance
    /// Provides endpoints for IRN generation, QR code generation, and e-invoice management
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EInvoiceController : ControllerBase
    {
        private readonly IEInvoiceService _eInvoiceService;
        private readonly ILogger<EInvoiceController> _logger;

        public EInvoiceController(IEInvoiceService eInvoiceService, ILogger<EInvoiceController> logger)
        {
            _eInvoiceService = eInvoiceService;
            _logger = logger;
        }

        /// <summary>
        /// Generate IRN (Invoice Reference Number) for an invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID to generate IRN for</param>
        /// <returns>E-Invoice response with IRN and QR code</returns>
        [HttpPost("generate-irn/{invoiceId}")]
        public async Task<IActionResult> GenerateIRN(long invoiceId)
        {
            try
            {
                _logger.LogInformation("IRN generation requested for Invoice {InvoiceId}", invoiceId);

                var result = await _eInvoiceService.GenerateIRNAsync(invoiceId);

                if (result.Status == "Error")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage ?? "Failed to generate IRN",
                        errorCode = result.ErrorCode
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "IRN generated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IRN generation failed for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while generating IRN" });
            }
        }

        /// <summary>
        /// Cancel e-invoice on NIC portal
        /// </summary>
        /// <param name="invoiceId">Invoice ID to cancel</param>
        /// <param name="request">Cancellation request with reason</param>
        /// <returns>Cancellation result</returns>
        [HttpPost("cancel/{invoiceId}")]
        public async Task<IActionResult> CancelEInvoice(long invoiceId, [FromBody] EInvoiceCancelRequest request)
        {
            try
            {
                _logger.LogInformation("E-Invoice cancellation requested for Invoice {InvoiceId}", invoiceId);

                if (string.IsNullOrWhiteSpace(request.CancelReason))
                {
                    return BadRequest(new { success = false, message = "Cancel reason is required" });
                }

                var result = await _eInvoiceService.CancelEInvoiceAsync(invoiceId, request.CancelReason);

                if (!result)
                {
                    return BadRequest(new { success = false, message = "Failed to cancel e-invoice" });
                }

                return Ok(new
                {
                    success = true,
                    message = "E-Invoice cancelled successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "E-Invoice cancellation failed for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling e-invoice" });
            }
        }

        /// <summary>
        /// Get IRN details from NIC portal
        /// </summary>
        /// <param name="irn">Invoice Reference Number</param>
        /// <returns>E-Invoice details</returns>
        [HttpGet("irn/{irn}")]
        public async Task<IActionResult> GetIRNDetails(string irn)
        {
            try
            {
                _logger.LogInformation("IRN details requested for {IRN}", irn);

                var result = await _eInvoiceService.GetIRNDetailsAsync(irn);

                if (result == null)
                {
                    return NotFound(new { success = false, message = "IRN not found" });
                }

                if (result.Status == "Error")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage ?? "Failed to get IRN details"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get IRN details for {IRN}", irn);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving IRN details" });
            }
        }

        /// <summary>
        /// Generate QR code for an invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID to generate QR code for</param>
        /// <returns>Base64 encoded QR code</returns>
        [HttpGet("qrcode/{invoiceId}")]
        public async Task<IActionResult> GenerateQRCode(long invoiceId)
        {
            try
            {
                _logger.LogInformation("QR code generation requested for Invoice {InvoiceId}", invoiceId);

                var qrCode = await _eInvoiceService.GenerateQRCodeAsync(invoiceId);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        invoiceId = invoiceId,
                        qrCode = qrCode,
                        qrCodeUrl = $"data:image/png;base64,{qrCode}"
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invoice not found for QR code generation");
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QR code generation failed for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while generating QR code" });
            }
        }

        /// <summary>
        /// Check if invoice is eligible for e-invoicing
        /// </summary>
        /// <param name="invoiceId">Invoice ID to check</param>
        /// <returns>Eligibility status</returns>
        [HttpGet("eligibility/{invoiceId}")]
        public async Task<IActionResult> CheckEligibility(long invoiceId)
        {
            try
            {
                var isEligible = await _eInvoiceService.IsEligibleForEInvoicingAsync(invoiceId);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        invoiceId = invoiceId,
                        isEligible = isEligible,
                        message = isEligible
                            ? "Invoice is eligible for e-invoicing"
                            : "Invoice is not eligible for e-invoicing. B2B invoices with GSTIN and value above threshold are required."
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Eligibility check failed for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while checking eligibility" });
            }
        }

        /// <summary>
        /// Sync invoice with NIC portal
        /// </summary>
        /// <param name="invoiceId">Invoice ID to sync</param>
        /// <returns>E-Invoice response</returns>
        [HttpPost("sync/{invoiceId}")]
        public async Task<IActionResult> SyncWithNICPortal(long invoiceId)
        {
            try
            {
                _logger.LogInformation("NIC portal sync requested for Invoice {InvoiceId}", invoiceId);

                var result = await _eInvoiceService.SyncWithNICPortalAsync(invoiceId);

                if (result.Status == "Error")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.ErrorMessage ?? "Failed to sync with NIC portal"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Synced with NIC portal successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NIC portal sync failed for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while syncing with NIC portal" });
            }
        }
    }

    /// <summary>
    /// Request model for e-invoice cancellation
    /// </summary>
    public class EInvoiceCancelRequest
    {
        public string CancelReason { get; set; } = string.Empty;
    }
}
