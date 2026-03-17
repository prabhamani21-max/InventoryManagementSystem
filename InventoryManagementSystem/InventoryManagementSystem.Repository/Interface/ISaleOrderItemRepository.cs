using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface ISaleOrderItemRepository
    {
        Task<SaleOrderItem> GetSaleOrderItemByIdAsync(long id);
        Task<IEnumerable<SaleOrderItem>> GetAllSaleOrderItemsAsync();
        Task<SaleOrderItem> CreateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<SaleOrderItem> UpdateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<bool> DeleteSaleOrderItemAsync(long id);
        
        /// <summary>
        /// Gets all sale order items for a specific sale order
        /// </summary>
        /// <param name="saleOrderId">The sale order ID</param>
        /// <returns>Collection of sale order items for the specified sale order</returns>
        Task<IEnumerable<SaleOrderItem>> GetSaleOrderItemsBySaleOrderIdAsync(long saleOrderId);

        /// <summary>
        /// Gets sale order items with jewellery item details for a specific sale order
        /// </summary>
        /// <param name="saleOrderId">The sale order ID</param>
        /// <returns>Collection of sale order item DB models with jewellery item included</returns>
        Task<List<SaleOrderItemDb>> GetSaleOrderItemsWithJewelleryBySaleOrderIdAsync(long saleOrderId);
    }
}