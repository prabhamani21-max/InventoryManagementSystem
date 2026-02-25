using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class ItemStockService : IItemStockService
    {
        private readonly IItemStockRepository _itemStockRepository;

        public ItemStockService(IItemStockRepository itemStockRepository)
        {
            _itemStockRepository = itemStockRepository;
        }

        public async Task<ItemStock> GetItemStockByIdAsync(int id)
        {
            return await _itemStockRepository.GetItemStockByIdAsync(id);
        }

        public async Task<ItemStock> GetItemStockByJewelleryItemIdAsync(long jewelleryItemId, int? warehouseId = null)
        {
            return await _itemStockRepository.GetItemStockByJewelleryItemIdAsync(jewelleryItemId, warehouseId);
        }

        public async Task<IEnumerable<ItemStock>> GetAllItemStocksAsync()
        {
            return await _itemStockRepository.GetAllItemStocksAsync();
        }

        public async Task<ItemStock> CreateItemStockAsync(ItemStock itemStock)
        {
            return await _itemStockRepository.CreateItemStockAsync(itemStock);
        }

        public async Task<ItemStock> UpdateItemStockAsync(ItemStock itemStock)
        {
            return await _itemStockRepository.UpdateItemStockAsync(itemStock);
        }

        public async Task<bool> DeleteItemStockAsync(int id)
        {
            return await _itemStockRepository.DeleteItemStockAsync(id);
        }

        // Stock validation and management methods
        public async Task<bool> CheckStockAvailabilityAsync(long jewelleryItemId, int requestedQuantity, int? warehouseId = null)
        {
            return await _itemStockRepository.CheckStockAvailabilityAsync(jewelleryItemId, requestedQuantity, warehouseId);
        }

        public async Task<StockValidationResult> ValidateStockForOrderAsync(IEnumerable<StockValidationRequest> items)
        {
            return await _itemStockRepository.ValidateStockForOrderAsync(items);
        }

        public async Task<bool> ReserveStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            return await _itemStockRepository.ReserveStockAsync(jewelleryItemId, quantity, warehouseId);
        }

        public async Task<bool> ReleaseReservedStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            return await _itemStockRepository.ReleaseReservedStockAsync(jewelleryItemId, quantity, warehouseId);
        }

        public async Task<bool> DeductStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            return await _itemStockRepository.DeductStockAsync(jewelleryItemId, quantity, warehouseId);
        }

        public async Task<bool> RestoreStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null)
        {
            return await _itemStockRepository.RestoreStockAsync(jewelleryItemId, quantity, warehouseId);
        }
    }
}