using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// Invoice Item Repository Interface
    /// Handles data operations for invoice items only (ISP compliance)
    /// </summary>
    public interface IInvoiceItemRepository
    {
        /// <summary>
        /// Get a specific invoice item by ID
        /// </summary>
        Task<InvoiceItem?> GetInvoiceItemByIdAsync(long id);

        /// <summary>
        /// Get all invoice items for a specific invoice
        /// </summary>
        Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(long invoiceId);

        /// <summary>
        /// Get all invoice items
        /// </summary>
        Task<IEnumerable<InvoiceItem>> GetAllInvoiceItemsAsync();

        /// <summary>
        /// Add a single invoice item
        /// </summary>
        Task<InvoiceItem> AddInvoiceItemAsync(InvoiceItem item);

        /// <summary>
        /// Add multiple invoice items in a batch
        /// </summary>
        Task AddInvoiceItemsRangeAsync(IEnumerable<InvoiceItem> items);

        /// <summary>
        /// Delete all invoice items for a specific invoice
        /// </summary>
        Task<bool> DeleteInvoiceItemsByInvoiceIdAsync(long invoiceId);
    }
}