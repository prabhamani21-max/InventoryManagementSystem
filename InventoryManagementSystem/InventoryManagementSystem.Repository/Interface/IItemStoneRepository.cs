using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IItemStoneRepository
    {
        Task<ItemStone> GetItemStoneByIdAsync(int id);
        Task<IEnumerable<ItemStone>> GetAllItemStonesAsync();
        Task<IEnumerable<ItemStone>> GetStonesByItemIdAsync(long itemId);
        Task<ItemStone> CreateItemStoneAsync(ItemStone itemStone);
        Task<ItemStone> UpdateItemStoneAsync(ItemStone itemStone);
        Task<bool> DeleteItemStoneAsync(int id);

        /// <summary>
        /// Gets item stones by a list of jewellery item IDs
        /// </summary>
        /// <param name="jewelleryItemIds">Collection of jewellery item IDs</param>
        /// <returns>List of item stone DB models</returns>
        Task<List<ItemStoneDb>> GetItemStonesByJewelleryItemIdsAsync(IEnumerable<long> jewelleryItemIds);
    }
}