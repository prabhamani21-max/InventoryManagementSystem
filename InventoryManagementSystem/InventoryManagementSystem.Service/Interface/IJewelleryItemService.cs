using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IJewelleryItemService
    {
        Task<JewelleryItem> GetJewelleryItemByIdAsync(long id);
        Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsAsync();
        Task<JewelleryItem> CreateJewelleryItemAsync(JewelleryItem jewelleryItem);
        Task<JewelleryItem> UpdateJewelleryItemAsync(JewelleryItem jewelleryItem);
        Task<bool> DeleteJewelleryItemAsync(long id);

        /// <summary>
        /// Calculate metal amount based on current rate for a jewellery item
        /// </summary>
        Task<decimal> CalculateMetalAmountAsync(long jewelleryItemId);

        /// <summary>
        /// Get current metal rate for a jewellery item
        /// </summary>
        Task<decimal> GetCurrentMetalRateAsync(long jewelleryItemId);


        /// <summary>
        /// Get jewellery item with calculated metal amount
        /// </summary>
        Task<JewelleryItem> GetJewelleryItemWithMetalAmountAsync(long jewelleryItemId);

        /// <summary>
        /// Get all jewellery items with calculated metal amounts
        /// </summary>
        Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsWithMetalAmountsAsync();
    }
}
