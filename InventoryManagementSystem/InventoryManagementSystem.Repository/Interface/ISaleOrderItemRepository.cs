using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface ISaleOrderItemRepository
    {
        Task<SaleOrderItem> GetSaleOrderItemByIdAsync(int id);
        Task<IEnumerable<SaleOrderItem>> GetAllSaleOrderItemsAsync();
        Task<SaleOrderItem> CreateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<SaleOrderItem> UpdateSaleOrderItemAsync(SaleOrderItem saleOrderItem);
        Task<bool> DeleteSaleOrderItemAsync(int id);
    }
}