using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface ISaleOrderItemService
    {
        Task<SaleOrderItem> GetSaleOrderItemByIdAsync(int id);
        Task<IEnumerable<SaleOrderItem>> GetAllSaleOrderItemsAsync();
        Task<SaleOrderItem> CreateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<SaleOrderItem> UpdateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<bool> DeleteSaleOrderItemAsync(int id);

        /// <summary>
        /// Creates a sale order item with automatic price calculation.
        /// Fetches metal rate, calculates metal amount, making charges, wastage, and tax.
        /// </summary>
        Task<SaleOrderItem> CreateSaleOrderItemWithCalculationAsync(
            long saleOrderId,
            long jewelleryItemId,
            decimal? discountAmount,
            decimal gstPercentage,
            decimal? stoneAmount,
            int quantity);
    }
}