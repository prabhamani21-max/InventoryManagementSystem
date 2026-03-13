using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// Invoice Repository Interface
    /// Simplified to handle Invoice entity only (ISP compliance)
    /// Use IInvoiceItemRepository for invoice items
    /// Use IInvoicePaymentRepository for invoice payments
    /// </summary>
    public interface IInvoiceRepository
    {
        // ==================== READ OPERATIONS ====================
        
        /// <summary>
        /// Get invoice by ID
        /// </summary>
        Task<Invoice?> GetInvoiceByIdAsync(long id);
        
        /// <summary>
        /// Get invoice by invoice number
        /// </summary>
        Task<Invoice?> GetInvoiceByInvoiceNumberAsync(string invoiceNumber);
        
        /// <summary>
        /// Get invoice by sale order ID
        /// </summary>
        Task<Invoice?> GetInvoiceBySaleOrderIdAsync(long saleOrderId);

        /// <summary>
        /// Get the active invoice by sale order ID
        /// </summary>
        Task<Invoice?> GetActiveInvoiceBySaleOrderIdAsync(long saleOrderId);
        
        /// <summary>
        /// Get all invoices
        /// </summary>
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();

        /// <summary>
        /// Get all invoices for a specific customer (party)
        /// </summary>
        /// <param name="partyId">The customer's user ID (party ID)</param>
        /// <returns>List of invoices for the customer</returns>
        Task<IEnumerable<Invoice>> GetInvoicesByPartyIdAsync(long partyId);

        // ==================== WRITE OPERATIONS ====================
        
        /// <summary>
        /// Add a new invoice
        /// </summary>
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        
        /// <summary>
        /// Update an existing invoice
        /// </summary>
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        
        /// <summary>
        /// Delete an invoice by ID
        /// </summary>
        Task<bool> DeleteInvoiceAsync(long id);
        
        /// <summary>
        /// Cancel an invoice by invoice number
        /// </summary>
        Task<bool> CancelInvoiceAsync(string invoiceNumber);
    }
}
