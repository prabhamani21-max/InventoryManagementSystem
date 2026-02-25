using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IItemStockRepository
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

    public class StockValidationRequest
    {
        public long JewelleryItemId { get; set; }
        public int RequestedQuantity { get; set; }
        public int? WarehouseId { get; set; }
    }

    public class StockValidationResult
    {
        public bool IsValid { get; set; }
        public List<StockValidationError> Errors { get; set; } = new List<StockValidationError>();
    }

    public class StockValidationError
    {
        public long JewelleryItemId { get; set; }
        public string ItemName { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string Message { get; set; }
    }
}