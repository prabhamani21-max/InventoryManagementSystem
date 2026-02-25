using InventoryManagementSytem.Common.Dtos;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Invoice Service Interface
    /// Provides functionality to generate jewellery-specific invoices from sale orders
    /// </summary>
    public interface IInvoiceService
    {
        /// <summary>
        /// Generate a complete invoice from a sale order
        /// </summary>
        /// <param name="request">Invoice generation request</param>
        /// <returns>Complete invoice with jewellery-specific details</returns>
        Task<InvoiceResponseDto> GenerateInvoiceAsync(InvoiceRequestDto request);

        /// <summary>
        /// Get invoice by invoice number
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to search</param>
        /// <returns>Invoice if found, null otherwise</returns>
        Task<InvoiceResponseDto?> GetInvoiceByNumberAsync(string invoiceNumber);

        /// <summary>
        /// Get invoice by sale order ID
        /// </summary>
        /// <param name="saleOrderId">Sale order ID</param>
        /// <returns>Invoice if found, null otherwise</returns>
        Task<InvoiceResponseDto?> GetInvoiceBySaleOrderIdAsync(long saleOrderId);

        /// <summary>
        /// Generate bulk invoices for multiple sale orders
        /// </summary>
        /// <param name="request">Bulk invoice generation request</param>
        /// <returns>Bulk invoice generation result</returns>
        Task<BulkInvoiceResponseDto> GenerateBulkInvoicesAsync(BulkInvoiceRequestDto request);

        /// <summary>
        /// Regenerate invoice with updated details
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to regenerate</param>
        /// <param name="notes">Optional notes to add</param>
        /// <returns>Updated invoice</returns>
        Task<InvoiceResponseDto?> RegenerateInvoiceAsync(string invoiceNumber, string? notes = null);

        /// <summary>
        /// Cancel an invoice
        /// </summary>
        /// <param name="invoiceNumber">Invoice number to cancel</param>
        /// <returns>True if cancelled successfully</returns>
        Task<bool> CancelInvoiceAsync(string invoiceNumber);

        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>List of all invoices</returns>
        Task<List<InvoiceResponseDto>> GetAllInvoicesAsync();

        /// <summary>
        /// Convert number to words (for grand total in words)
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>Number in words</returns>
        string NumberToWords(decimal number);
    }
}
