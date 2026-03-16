using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IPurchaseOrderItemRepository
    {
        Task<PurchaseOrderItem> GetPurchaseOrderItemByIdAsync(int id);
        Task<IEnumerable<PurchaseOrderItem>> GetAllPurchaseOrderItemsAsync();
        Task<PurchaseOrderItem> CreatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem);
        Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem);
        Task<bool> DeletePurchaseOrderItemAsync(int id);
    }
}