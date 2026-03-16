using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            return await _purchaseOrderRepository.GetPurchaseOrderByIdAsync(id);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _purchaseOrderRepository.GetAllPurchaseOrdersAsync();
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            return await _purchaseOrderRepository.CreatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            return await _purchaseOrderRepository.UpdatePurchaseOrderAsync(purchaseOrder);
        }

        public async Task<bool> DeletePurchaseOrderAsync(int id)
        {
            return await _purchaseOrderRepository.DeletePurchaseOrderAsync(id);
        }
    }
}