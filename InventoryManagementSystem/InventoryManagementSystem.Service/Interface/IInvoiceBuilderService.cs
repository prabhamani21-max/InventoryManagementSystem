using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IInvoiceBuilderService
    {
        /// <summary>
        /// Build invoice items from sale order items
        /// Snapshots metal, stone, and pricing data at billing time
        /// </summary>
        List<InvoiceItem> BuildInvoiceItems(
            List<SaleOrderItemDb> saleOrderItems,
            Dictionary<int, JewelleryItemDb> jewelleryItems,
            List<ItemStoneDb> itemStones,
            Dictionary<int, MetalDb> metals,
            Dictionary<int, PurityDb> purities);
    }
}
