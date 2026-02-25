using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IItemStoneService
    {
        Task<ItemStone> GetItemStoneByIdAsync(int id);
        Task<IEnumerable<ItemStone>> GetAllItemStonesAsync();
        Task<ItemStone> CreateItemStoneAsync(ItemStone itemStone);
        Task<ItemStone> UpdateItemStoneAsync(ItemStone itemStone);
        Task<bool> DeleteItemStoneAsync(int id);
    }
}