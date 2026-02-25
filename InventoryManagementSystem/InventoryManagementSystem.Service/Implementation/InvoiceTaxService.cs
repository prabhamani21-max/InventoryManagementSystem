using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class InvoiceTaxService : IInvoiceTaxService
    {
        /// <summary>
        /// Calculate GST breakdown for an invoice item from SaleOrderItem
        /// CGST + SGST = 50% each for intra-state (standard GST split)
        /// </summary>
        public (decimal CGSTAmount, decimal SGSTAmount, decimal IGSTAmount, decimal GSTAmount) CalculateItemGST(decimal gstAmount)
        {
            // Standard GST split: 50% CGST + 50% SGST for intra-state transactions
            var cgst = gstAmount / 2;
            var sgst = gstAmount / 2;
            var igst = 0m; // IGST only for inter-state

            return (cgst, sgst, igst, gstAmount);
        }

        /// <summary>
        /// Calculate totals from invoice items (sum-based, NOT recomputed independently)
        /// This ensures invoice totals match item totals exactly
        /// </summary>
        public (decimal SubTotal, decimal DiscountAmount, decimal TaxableAmount, decimal CGSTAmount, 
                decimal SGSTAmount, decimal IGSTAmount, decimal TotalGSTAmount, decimal GrandTotal) CalculateInvoiceTotals(
            IEnumerable<InvoiceItem> items)
        {
            var itemList = items.ToList();
            
            var subTotal = itemList.Sum(i => i.ItemSubtotal);
            var discountAmount = itemList.Sum(i => i.Discount);
            var taxableAmount = itemList.Sum(i => i.TaxableAmount);
            var cgstAmount = itemList.Sum(i => i.CGSTAmount);
            var sgstAmount = itemList.Sum(i => i.SGSTAmount);
            var igstAmount = itemList.Sum(i => i.IGSTAmount);
            var totalGstAmount = itemList.Sum(i => i.GSTAmount);
            var grandTotal = taxableAmount + totalGstAmount;

            return (subTotal, discountAmount, taxableAmount, cgstAmount, sgstAmount, igstAmount, totalGstAmount, grandTotal);
        }
    }
}
