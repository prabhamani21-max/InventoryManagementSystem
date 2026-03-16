using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class StoneService : IStoneService
    {
        private readonly IStoneRepository _stoneRepository;

        public StoneService(IStoneRepository stoneRepository)
        {
            _stoneRepository = stoneRepository;
        }

        public async Task<Stone> GetStoneByIdAsync(int id)
        {
            return await _stoneRepository.GetStoneByIdAsync(id);
        }

        public async Task<IEnumerable<Stone>> GetAllStonesAsync()
        {
            return await _stoneRepository.GetAllStonesAsync();
        }

        public async Task<Stone> CreateStoneAsync(Stone stone)
        {
            return await _stoneRepository.CreateStoneAsync(stone);
        }

        public async Task<Stone> UpdateStoneAsync(Stone stone)
        {
            return await _stoneRepository.UpdateStoneAsync(stone);
        }

        public async Task<bool> DeleteStoneAsync(int id)
        {
            return await _stoneRepository.DeleteStoneAsync(id);
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            return await _stoneRepository.NameExistsAsync(name, excludeId);
        }

        public async Task<IEnumerable<Stone>> SearchStonesByNameAsync(string name)
        {
            return await _stoneRepository.SearchStonesByNameAsync(name);
        }
    }
}