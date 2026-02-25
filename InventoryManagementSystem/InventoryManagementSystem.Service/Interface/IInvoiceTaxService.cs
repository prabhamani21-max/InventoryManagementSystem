using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IInvoiceTaxService
    {
        /// <summary>
        /// Calculate GST breakdown for an invoice item from SaleOrderItem
        /// Returns CGST, SGST, IGST amounts (split 50/50 for intra-state)
        /// </summary>
        (decimal CGSTAmount, decimal SGSTAmount, decimal IGSTAmount, decimal GSTAmount) CalculateItemGST(decimal gstAmount);

        /// <summary>
        /// Calculate totals from invoice items (sum-based, not recomputed)
        /// </summary>
        (decimal SubTotal, decimal DiscountAmount, decimal TaxableAmount, decimal CGSTAmount, 
         decimal SGSTAmount, decimal IGSTAmount, decimal TotalGSTAmount, decimal GrandTotal) CalculateInvoiceTotals(
            IEnumerable<InvoiceItem> items);
    }
}
