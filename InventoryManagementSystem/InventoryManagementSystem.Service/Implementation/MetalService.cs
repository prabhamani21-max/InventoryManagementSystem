using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class MetalService : IMetalService
    {
        private readonly IMetalRepository _metalRepository;

        public MetalService(IMetalRepository metalRepository)
        {
            _metalRepository = metalRepository;
        }

        public async Task<Metal> GetMetalByIdAsync(int id)
        {
            return await _metalRepository.GetMetalByIdAsync(id);
        }

        public async Task<IEnumerable<Metal>> GetAllMetalsAsync()
        {
            return await _metalRepository.GetAllMetalsAsync();
        }

        public async Task<Metal> CreateMetalAsync(Metal metal)
        {
            return await _metalRepository.CreateMetalAsync(metal);
        }

        public async Task<Metal> UpdateMetalAsync(Metal metal)
        {
            return await _metalRepository.UpdateMetalAsync(metal);
        }

        public async Task<bool> DeleteMetalAsync(int id)
        {
            return await _metalRepository.DeleteMetalAsync(id);
        }
    }
}