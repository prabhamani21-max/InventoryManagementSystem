using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    /// <summary>
    /// Invoice Item API Controller
    /// Provides endpoints for managing invoice items
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceItemController : ControllerBase
    {
        private readonly IInvoiceItemService _invoiceItemService;
        private readonly ILogger<InvoiceItemController> _logger;

        public InvoiceItemController(
            IInvoiceItemService invoiceItemService,
            ILogger<InvoiceItemController> logger)
        {
            _invoiceItemService = invoiceItemService;
            _logger = logger;
        }

        /// <summary>
        /// Get all invoice items
        /// </summary>
        /// <returns>List of all invoice items</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllInvoiceItems()
        {
            try
            {
                _logger.LogInformation("Fetching all invoice items");
                var invoiceItems = await _invoiceItemService.GetAllInvoiceItemsAsync();

                return Ok(new
                {
                    success = true,
                    data = invoiceItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve all invoice items");
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving invoice items" });
            }
        }

        /// <summary>
        /// Get a specific invoice item by ID
        /// </summary>
        /// <param name="id">Invoice item ID</param>
        /// <returns>Invoice item if found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceItemById(long id)
        {
            try
            {
                _logger.LogInformation("Fetching invoice item with ID: {Id}", id);
                var invoiceItem = await _invoiceItemService.GetInvoiceItemByIdAsync(id);

                if (invoiceItem == null)
                {
                    return NotFound(new { success = false, message = "Invoice item not found" });
                }

                return Ok(new
                {
                    success = true,
                    data = invoiceItem
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve invoice item with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving the invoice item" });
            }
        }

        /// <summary>
        /// Get all invoice items for a specific invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <returns>List of invoice items for the specified invoice</returns>
        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceItemsByInvoiceId(long invoiceId)
        {
            try
            {
                _logger.LogInformation("Fetching invoice items for invoice ID: {InvoiceId}", invoiceId);
                var invoiceItems = await _invoiceItemService.GetInvoiceItemsByInvoiceIdAsync(invoiceId);

                return Ok(new
                {
                    success = true,
                    data = invoiceItems
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve invoice items for invoice ID: {InvoiceId}", invoiceId);
                return StatusCode(500, new { success = false, message = "An error occurred while retrieving invoice items" });
            }
        }
    }
}
