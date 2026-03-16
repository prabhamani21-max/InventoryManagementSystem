using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IItemStockService
    {
        Task<ItemStock> GetItemStockByIdAsync(int id);
        Task<ItemStock> GetItemStockByJewelleryItemIdAsync(long jewelleryItemId, int? warehouseId = null);
        Task<IEnumerable<ItemStock>> GetAllItemStocksAsync();
        Task<ItemStock> CreateItemStockAsync(ItemStock itemStock);
        Task<ItemStock> UpdateItemStockAsync(ItemStock itemStock);
        Task<bool> DeleteItemStockAsync(int id);
        
        // Stock validation and management methods
        Task<bool> CheckStockAvailabilityAsync(long jewelleryItemId, int requestedQuantity, int? warehouseId = null);
        Task<StockValidationResult> ValidateStockForOrderAsync(IEnumerable<StockValidationRequest> items);
        Task<bool> ReserveStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null);
        Task<bool> ReleaseReservedStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null);
        Task<bool> DeductStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null);
        Task<bool> RestoreStockAsync(long jewelleryItemId, int quantity, int? warehouseId = null);
    }
}