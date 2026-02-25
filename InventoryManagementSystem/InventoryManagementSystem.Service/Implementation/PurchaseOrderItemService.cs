using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class PurchaseOrderItemService : IPurchaseOrderItemService
    {
        private readonly IPurchaseOrderItemRepository _purchaseOrderItemRepository;

        public PurchaseOrderItemService(IPurchaseOrderItemRepository purchaseOrderItemRepository)
        {
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
        }

        public async Task<PurchaseOrderItem> GetPurchaseOrderItemByIdAsync(int id)
        {
            return await _purchaseOrderItemRepository.GetPurchaseOrderItemByIdAsync(id);
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetAllPurchaseOrderItemsAsync()
        {
            return await _purchaseOrderItemRepository.GetAllPurchaseOrderItemsAsync();
        }

        public async Task<PurchaseOrderItem> CreatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem)
        {
            return await _purchaseOrderItemRepository.CreatePurchaseOrderItemAsync(purchaseOrderItem);
        }

        public async Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem)
        {
            return await _purchaseOrderItemRepository.UpdatePurchaseOrderItemAsync(purchaseOrderItem);
        }

        public async Task<bool> DeletePurchaseOrderItemAsync(int id)
        {
            return await _purchaseOrderItemRepository.DeletePurchaseOrderItemAsync(id);
        }
    }
}