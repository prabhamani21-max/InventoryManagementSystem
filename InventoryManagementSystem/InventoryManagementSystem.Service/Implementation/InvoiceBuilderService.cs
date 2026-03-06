using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class InvoiceBuilderService : IInvoiceBuilderService
    {
        private readonly IInvoiceTaxService _taxService;

        public InvoiceBuilderService(IInvoiceTaxService taxService)
        {
            _taxService = taxService;
        }

        /// <summary>
        /// Build invoice items from sale order items
        /// Snapshots metal, stone, and pricing data at billing time
        /// 
        /// IMPORTANT: All values are snapshotted at billing time to ensure:
        /// - Stone price changes don't break old invoices
        /// - Returns can be processed with original values
        /// - Metal stock valuation remains accurate
        /// 
        /// GST Structure:
        /// - Metal GST (3%): Applied on MetalAmount + WastageAmount + StoneAmount
        /// - Making Charges GST (5%): Applied on TotalMakingCharges
        /// </summary>
        public List<InvoiceItem> BuildInvoiceItems(
            List<SaleOrderItemDb> saleOrderItems,
            Dictionary<int, JewelleryItemDb> jewelleryItems,
            List<ItemStoneDb> itemStones,
            Dictionary<int, MetalDb> metals,
            Dictionary<int, PurityDb> purities)
        {
            var items = new List<InvoiceItem>();

            foreach (var item in saleOrderItems)
            {
                if (!jewelleryItems.TryGetValue((int)item.JewelleryItemId, out var jewelleryItem))
                {
                    continue;
                }

                // Get stone data for this jewellery item
                var stoneData = itemStones.FirstOrDefault(is_ => is_.JewelleryItemId == item.JewelleryItemId);

                // Calculate Metal GST breakdown (3% on metal value)
                // GST is derived from SaleOrderItem, NOT recomputed independently
                var (cgstAmount, sgstAmount, igstAmount, gstAmount) = _taxService.CalculateItemGST(item.GstAmount);

                // Calculate Making Charges GST breakdown (5% on making charges)
                var (mcCgst, mcSgst, mcIgst, mcGst) = _taxService.CalculateItemGST(item.MakingChargesGstAmount);

                var invoiceItem = new InvoiceItem
                {
                    ReferenceItemId = item.Id,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,

                    // Metal Details - snapshot from SaleOrderItem
                    MetalId = item.MetalId,
                    PurityId = item.PurityId,
                    NetMetalWeight = item.NetMetalWeight,
                    MetalAmount = item.MetalAmount, // FIXED: Use item.MetalAmount from SaleOrderItem, NOT item.TotalAmount

                    // Stone Details - SNAPSHOT at billing time (never depend on JewelleryItem later)
                    // This ensures stone price changes don't affect existing invoices
                    StoneId = item.HasStone ? jewelleryItem.StoneId : null,
                    StoneWeight = item.HasStone && stoneData != null ? stoneData.Weight : null,
                    StoneRate = null, // Would need StoneRateHistory lookup at billing time
                    StoneAmount = item.StoneAmount ?? 0, // Snapshot from SaleOrderItem

                    // Making Charges - from SaleOrderItem
                    MakingCharges = item.TotalMakingCharges,
                    WastageAmount = item.WastageAmount,

                    // Pricing - from SaleOrderItem
                    ItemSubtotal = item.ItemSubtotal,
                    Discount = item.DiscountAmount,
                    TaxableAmount = item.TaxableAmount,

                    // Metal GST Breakdown (3% on metal value) - derived from SaleOrderItem (NOT recomputed)
                    CGSTAmount = cgstAmount,
                    SGSTAmount = sgstAmount,
                    IGSTAmount = igstAmount,
                    GSTAmount = gstAmount,

                    // Making Charges GST Breakdown (5% on making charges) - derived from SaleOrderItem
                    MakingChargesCGSTAmount = mcCgst,
                    MakingChargesSGSTAmount = mcSgst,
                    MakingChargesIGSTAmount = mcIgst,
                    MakingChargesGSTAmount = mcGst,

                    // Total GST = Metal GST + Making Charges GST
                    TotalGSTAmount = item.TotalGstAmount,
                    TotalAmount = item.TotalAmount,
                };

                items.Add(invoiceItem);
            }

            return items;
        }
    }
}
