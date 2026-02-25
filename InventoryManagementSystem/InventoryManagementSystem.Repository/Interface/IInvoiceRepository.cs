using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// Invoice Repository Interface
    /// </summary>
    public interface IInvoiceRepository
    {
        // ==================== READ OPERATIONS ====================
        Task<Invoice?> GetInvoiceByIdAsync(long id);
        Task<Invoice?> GetInvoiceByInvoiceNumberAsync(string invoiceNumber);
        Task<Invoice?> GetInvoiceBySaleOrderIdAsync(long saleOrderId);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<InvoiceItem?> GetInvoiceItemByIdAsync(long id);
        Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(long invoiceId);
        Task<InvoicePayment?> GetInvoicePaymentByIdAsync(long id);
        Task<IEnumerable<InvoicePayment>> GetInvoicePaymentsByInvoiceIdAsync(long invoiceId);

        // ==================== WRITE OPERATIONS ====================
        Task<Invoice> GenerateInvoiceAsync(Invoice invoice);
        Task<Invoice> UpdateInvoiceAsync(Invoice invoice);
        Task<bool> DeleteInvoiceAsync(long id);
        Task<bool> CancelInvoiceAsync(string invoiceNumber);
    }
}
