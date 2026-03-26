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
        /// Get invoice by invoice ID.
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <returns>Invoice if found, null otherwise</returns>
        Task<InvoiceResponseDto?> GetInvoiceByIdAsync(long invoiceId);

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
        /// <summary>
        /// Get all invoices
        /// </summary>
        /// <returns>List of all invoices</returns>
        Task<List<InvoiceResponseDto>> GetAllInvoicesAsync();

        /// <summary>
        /// Get all invoices for a specific customer (party)
        /// </summary>
        /// <param name="partyId">The customer's user ID (party ID)</param>
        /// <returns>List of invoices for the customer</returns>
        Task<List<InvoiceResponseDto>> GetInvoicesByPartyIdAsync(long partyId);

        /// <summary>
        /// Get all invoices for orders created by a specific sales person
        /// </summary>
        /// <param name="createdBy">The sales person's user ID</param>
        /// <returns>List of invoices for orders created by the sales person</returns>
        Task<List<InvoiceResponseDto>> GetInvoicesByCreatedByAsync(long createdBy);

        /// <summary>
        /// Convert number to words (for grand total in words)
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>Number in words</returns>
        string NumberToWords(decimal number);
    }
}
