using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    /// <summary>
    /// TCS (Tax Collected at Source) API Controller
    /// Provides endpoints for TCS calculation, reporting, and Form 26Q generation
    /// Applicable for B2C jewellery sales exceeding ₹10 lakh threshold
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TcsController : ControllerBase
    {
        private readonly ITcsService _tcsService;
        private readonly ILogger<TcsController> _logger;

        public TcsController(ITcsService tcsService, ILogger<TcsController> logger)
        {
            _tcsService = tcsService;
            _logger = logger;
        }

        /// <summary>
        /// Calculate TCS for a sale transaction
        /// </summary>
        /// <param name="request">TCS calculation request</param>
        /// <returns>TCS calculation details</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateTcs([FromBody] TcsCalculationRequestDto request)
        {
            try
            {
                _logger.LogInformation(
                    "TCS calculation requested for Customer {CustomerId}, Amount: {Amount}",
                    request.CustomerId, request.SaleAmount);

                var result = await _tcsService.CalculateTcsAsync(request);

                return Ok(new
                {
                    success = true,
                    message = result.IsTcsApplicable
                        ? $"TCS applicable at {result.TcsRate * 100}%"
                        : "TCS not applicable",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TCS calculation failed for Customer {CustomerId}", request.CustomerId);
                return StatusCode(500, new { success = false, message = "TCS calculation failed" });
            }
        }

        /// <summary>
        /// Get customer TCS summary for a financial year
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="financialYear">Financial year (optional, defaults to current)</param>
        /// <returns>Customer TCS summary</returns>
        [HttpGet("customer/{customerId}/summary")]
        public async Task<IActionResult> GetCustomerSummary(
            long customerId,
            [FromQuery] string? financialYear)
        {
            try
            {
                var fy = financialYear ?? _tcsService.GetCurrentFinancialYear();
                var result = await _tcsService.GetCustomerTcsSummaryAsync(customerId, fy);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS summary for Customer {CustomerId}", customerId);
                return StatusCode(500, new { success = false, message = "Failed to retrieve TCS summary" });
            }
        }

        /// <summary>
        /// Get TCS transactions for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="financialYear">Financial year filter (optional)</param>
        /// <returns>List of TCS transactions</returns>
        [HttpGet("customer/{customerId}/transactions")]
        public async Task<IActionResult> GetCustomerTransactions(
            long customerId,
            [FromQuery] string? financialYear)
        {
            try
            {
                var result = await _tcsService.GetCustomerTcsTransactionsAsync(customerId, financialYear);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS transactions for Customer {CustomerId}", customerId);
                return StatusCode(500, new { success = false, message = "Failed to retrieve TCS transactions" });
            }
        }

        /// <summary>
        /// Generate Form 26Q report for quarterly TCS return
        /// </summary>
        /// <param name="financialYear">Financial year (e.g., "2024-25")</param>
        /// <param name="quarter">Quarter (1-4)</param>
        /// <returns>Form 26Q report data</returns>
        [HttpGet("report/form26q")]
        public async Task<IActionResult> GenerateForm26Q(
            [FromQuery] string financialYear,
            [FromQuery] int quarter)
        {
            try
            {
                if (quarter < 1 || quarter > 4)
                {
                    return BadRequest(new { success = false, message = "Quarter must be between 1 and 4" });
                }

                _logger.LogInformation(
                    "Form 26Q report requested for FY {FinancialYear} Q{Quarter}",
                    financialYear, quarter);

                var result = await _tcsService.GenerateForm26QReportAsync(financialYear, quarter);

                return Ok(new
                {
                    success = true,
                    message = $"Form 26Q generated for FY {financialYear} Q{quarter}",
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Form 26Q generation failed for FY {FinancialYear} Q{Quarter}", financialYear, quarter);
                return StatusCode(500, new { success = false, message = "Form 26Q generation failed" });
            }
        }

        /// <summary>
        /// Get all TCS transactions for a financial year/quarter
        /// </summary>
        /// <param name="financialYear">Financial year</param>
        /// <param name="quarter">Quarter filter (optional)</param>
        /// <returns>List of TCS transactions</returns>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(
            [FromQuery] string financialYear,
            [FromQuery] int? quarter)
        {
            try
            {
                var fy = financialYear ?? _tcsService.GetCurrentFinancialYear();
                var result = await _tcsService.GetTcsTransactionsAsync(fy, quarter);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS transactions");
                return StatusCode(500, new { success = false, message = "Failed to retrieve TCS transactions" });
            }
        }

        /// <summary>
        /// Get TCS dashboard summary for a financial year
        /// </summary>
        /// <param name="financialYear">Financial year (optional, defaults to current)</param>
        /// <returns>TCS dashboard summary</returns>
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary([FromQuery] string? financialYear)
        {
            try
            {
                var fy = financialYear ?? _tcsService.GetCurrentFinancialYear();
                var result = await _tcsService.GetDashboardSummaryAsync(fy);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS dashboard summary");
                return StatusCode(500, new { success = false, message = "Failed to retrieve dashboard summary" });
            }
        }

        /// <summary>
        /// Get TCS transaction by ID
        /// </summary>
        /// <param name="id">TCS transaction ID</param>
        /// <returns>TCS transaction details</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTcsTransaction(long id)
        {
            try
            {
                var result = await _tcsService.GetTcsTransactionByIdAsync(id);

                if (result == null)
                {
                    return NotFound(new { success = false, message = "TCS transaction not found" });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS transaction {Id}", id);
                return StatusCode(500, new { success = false, message = "Failed to retrieve TCS transaction" });
            }
        }

        /// <summary>
        /// Get TCS transaction by invoice ID
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <returns>TCS transaction details</returns>
        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetTcsTransactionByInvoice(long invoiceId)
        {
            try
            {
                var result = await _tcsService.GetTcsTransactionByInvoiceIdAsync(invoiceId);

                if (result == null)
                {
                    return NotFound(new { success = false, message = "TCS transaction not found for this invoice" });
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get TCS transaction for Invoice {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "Failed to retrieve TCS transaction" });
            }
        }

        /// <summary>
        /// Get current financial year
        /// </summary>
        /// <returns>Current financial year string</returns>
        [HttpGet("current-financial-year")]
        public IActionResult GetCurrentFinancialYear()
        {
            var fy = _tcsService.GetCurrentFinancialYear();
            return Ok(new
            {
                success = true,
                data = new
                {
                    financialYear = fy,
                    description = $"FY {fy}"
                }
            });
        }

        /// <summary>
        /// Check customer PAN status for TCS
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>PAN status details</returns>
        [HttpGet("customer/{customerId}/pan-status")]
        public async Task<IActionResult> CheckCustomerPANStatus(long customerId)
        {
            try
            {
                var (hasValidPAN, panNumber) = await _tcsService.CheckCustomerPANAsync(customerId);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        customerId,
                        hasValidPAN,
                        panNumber,
                        tcsRate = hasValidPAN ? 0.1m : 1.0m,
                        message = hasValidPAN
                            ? "Valid PAN available. TCS rate: 0.1%"
                            : panNumber != null
                                ? "PAN available but not verified. TCS rate: 1%"
                                : "No PAN available. TCS rate: 1%"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check PAN status for Customer {CustomerId}", customerId);
                return StatusCode(500, new { success = false, message = "Failed to check PAN status" });
            }
        }
    }
}
