namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Invoice PDF Service Interface
    /// Provides functionality to generate PDF documents for invoices
    /// </summary>
    public interface IInvoicePdfService
    {
        /// <summary>
        /// Generate a PDF document for an invoice by ID
        /// </summary>
        /// <param name="invoiceId">The invoice ID</param>
        /// <returns>PDF document as byte array</returns>
        Task<byte[]> GenerateInvoicePdfAsync(long invoiceId);

        /// <summary>
        /// Generate a PDF document for an invoice by invoice number
        /// </summary>
        /// <param name="invoiceNumber">The invoice number</param>
        /// <returns>PDF document as byte array</returns>
        Task<byte[]> GenerateInvoicePdfByNumberAsync(string invoiceNumber);
    }
}
