using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IPurchaseOrderItemService
    {
        Task<PurchaseOrderItem> GetPurchaseOrderItemByIdAsync(int id);
        Task<IEnumerable<PurchaseOrderItem>> GetAllPurchaseOrderItemsAsync();
        Task<PurchaseOrderItem> CreatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem);
        Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem);
        Task<bool> DeletePurchaseOrderItemAsync(int id);
    }
}