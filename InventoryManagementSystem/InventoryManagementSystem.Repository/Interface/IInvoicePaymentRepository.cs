using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// Invoice Payment Repository Interface
    /// Handles data operations for invoice payments only (ISP compliance)
    /// </summary>
    public interface IInvoicePaymentRepository
    {
        /// <summary>
        /// Get a specific invoice payment by ID
        /// </summary>
        Task<InvoicePayment?> GetInvoicePaymentByIdAsync(long id);

        /// <summary>
        /// Get all invoice payments for a specific invoice
        /// </summary>
        Task<IEnumerable<InvoicePayment>> GetInvoicePaymentsByInvoiceIdAsync(long invoiceId);

        /// <summary>
        /// Add a single invoice payment
        /// </summary>
        Task<InvoicePayment> AddInvoicePaymentAsync(InvoicePayment payment);

        /// <summary>
        /// Add multiple invoice payments in a batch
        /// </summary>
        Task AddInvoicePaymentsRangeAsync(IEnumerable<InvoicePayment> payments);

        /// <summary>
        /// Delete all invoice payments for a specific invoice
        /// </summary>
        Task<bool> DeleteInvoicePaymentsByInvoiceIdAsync(long invoiceId);
    }
}