using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IJewelleryItemRepository
    {
        Task<JewelleryItem> GetJewelleryItemByIdAsync(long id);
        Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsAsync();
        Task<JewelleryItem> CreateJewelleryItemAsync(JewelleryItem jewelleryItem);
        Task<JewelleryItem> UpdateJewelleryItemAsync(JewelleryItem jewelleryItem);
        Task<bool> DeleteJewelleryItemAsync(long id);

        // New methods for Db model access with navigation properties
        Task<JewelleryItemDb?> GetJewelleryItemDbByIdAsync(long id);
        Task<IEnumerable<JewelleryItemDb>> GetAllJewelleryItemsDbAsync();
        Task<JewelleryItemDb> UpdateJewelleryItemDbAsync(JewelleryItemDb jewelleryItem);
    }
}
