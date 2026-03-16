using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Invoice Item Service Implementation
    /// Business logic layer for invoice item operations (Three-tier architecture compliance)
    /// </summary>
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IInvoiceItemRepository _invoiceItemRepository;
        private readonly ILogger<InvoiceItemService> _logger;

        public InvoiceItemService(
            IInvoiceItemRepository invoiceItemRepository,
            ILogger<InvoiceItemService> logger)
        {
            _invoiceItemRepository = invoiceItemRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<InvoiceItem?> GetInvoiceItemByIdAsync(long id)
        {
            _logger.LogInformation("Fetching invoice item with ID: {Id}", id);
            var item = await _invoiceItemRepository.GetInvoiceItemByIdAsync(id);
            
            if (item == null)
            {
                _logger.LogWarning("Invoice item with ID {Id} not found", id);
            }
            
            return item;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(long invoiceId)
        {
            _logger.LogInformation("Fetching invoice items for invoice ID: {InvoiceId}", invoiceId);
            var items = await _invoiceItemRepository.GetInvoiceItemsByInvoiceIdAsync(invoiceId);
            
            _logger.LogInformation("Retrieved {Count} invoice items for invoice ID: {InvoiceId}", items.Count(), invoiceId);
            return items;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InvoiceItem>> GetAllInvoiceItemsAsync()
        {
            _logger.LogInformation("Fetching all invoice items");
            var items = await _invoiceItemRepository.GetAllInvoiceItemsAsync();
            
            _logger.LogInformation("Retrieved {Count} invoice items", items.Count());
            return items;
        }

        /// <inheritdoc/>
        public async Task<InvoiceItem> AddInvoiceItemAsync(InvoiceItem item)
        {
            _logger.LogInformation("Adding invoice item for invoice ID: {InvoiceId}", item.InvoiceId);
            var addedItem = await _invoiceItemRepository.AddInvoiceItemAsync(item);
            
            _logger.LogInformation("Successfully added invoice item with ID: {Id}", addedItem.Id);
            return addedItem;
        }

        /// <inheritdoc/>
        public async Task AddInvoiceItemsRangeAsync(IEnumerable<InvoiceItem> items)
        {
            var itemsList = items.ToList();
            _logger.LogInformation("Adding {Count} invoice items", itemsList.Count);
            
            await _invoiceItemRepository.AddInvoiceItemsRangeAsync(itemsList);
            
            _logger.LogInformation("Successfully added {Count} invoice items", itemsList.Count);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteInvoiceItemsByInvoiceIdAsync(long invoiceId)
        {
            _logger.LogInformation("Deleting invoice items for invoice ID: {InvoiceId}", invoiceId);
            var result = await _invoiceItemRepository.DeleteInvoiceItemsByInvoiceIdAsync(invoiceId);
            
            if (result)
            {
                _logger.LogInformation("Successfully deleted invoice items for invoice ID: {InvoiceId}", invoiceId);
            }
            else
            {
                _logger.LogWarning("No invoice items found to delete for invoice ID: {InvoiceId}", invoiceId);
            }
            
            return result;
        }
    }
}
