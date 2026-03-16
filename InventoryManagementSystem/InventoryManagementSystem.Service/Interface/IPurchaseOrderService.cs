using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IPurchaseOrderService
    {
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id);
        Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeletePurchaseOrderAsync(int id);
    }
}