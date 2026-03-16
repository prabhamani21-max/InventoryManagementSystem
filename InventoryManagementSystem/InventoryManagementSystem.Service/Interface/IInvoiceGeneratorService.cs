using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Invoice Generator Service Interface
    /// Orchestrates the complete invoice generation process from a sale order
    /// This service coordinates all data fetching, calculations, and persistence
    /// (SRP compliance - separates orchestration from data access)
    /// </summary>
    public interface IInvoiceGeneratorService
    {
        /// <summary>
        /// Generates a complete invoice from a sale order
        /// Orchestrates all data fetching, calculations, and persistence
        /// </summary>
        /// <param name="saleOrderId">The sale order ID to generate invoice from</param>
        /// <param name="includeTermsAndConditions">Whether to include terms and conditions</param>
        /// <param name="notes">Optional notes for the invoice</param>
        /// <returns>The generated invoice</returns>
        Task<Invoice> GenerateInvoiceFromSaleOrderAsync(
            long saleOrderId,
            bool includeTermsAndConditions,
            string? notes);

        /// <summary>
        /// Cancels an invoice and restores stock
        /// </summary>
        /// <param name="invoiceNumber">The invoice number to cancel</param>
        /// <returns>True if cancelled successfully</returns>
        Task<bool> CancelInvoiceAsync(string invoiceNumber);
    }
}