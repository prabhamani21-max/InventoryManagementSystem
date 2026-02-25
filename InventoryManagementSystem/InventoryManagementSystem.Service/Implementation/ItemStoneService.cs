using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class ItemStoneService : IItemStoneService
    {
        private readonly IItemStoneRepository _itemStoneRepository;

        public ItemStoneService(IItemStoneRepository itemStoneRepository)
        {
            _itemStoneRepository = itemStoneRepository;
        }

        public async Task<ItemStone> GetItemStoneByIdAsync(int id)
        {
            return await _itemStoneRepository.GetItemStoneByIdAsync(id);
        }

        public async Task<IEnumerable<ItemStone>> GetAllItemStonesAsync()
        {
            return await _itemStoneRepository.GetAllItemStonesAsync();
        }

        public async Task<ItemStone> CreateItemStoneAsync(ItemStone itemStone)
        {
            return await _itemStoneRepository.CreateItemStoneAsync(itemStone);
        }

        public async Task<ItemStone> UpdateItemStoneAsync(ItemStone itemStone)
        {
            return await _itemStoneRepository.UpdateItemStoneAsync(itemStone);
        }

        public async Task<bool> DeleteItemStoneAsync(int id)
        {
            return await _itemStoneRepository.DeleteItemStoneAsync(id);
        }
    }
}