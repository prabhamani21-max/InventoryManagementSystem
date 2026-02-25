using InventoryManagementSystem.Common.Models;

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
    }
}