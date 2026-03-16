using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class PurityService : IPurityService
    {
        private readonly IPurityRepository _purityRepository;

        public PurityService(IPurityRepository purityRepository)
        {
            _purityRepository = purityRepository;
        }

        public async Task<Purity> GetPurityByIdAsync(int id)
        {
            return await _purityRepository.GetPurityByIdAsync(id);
        }

        public async Task<IEnumerable<Purity>> GetAllPuritiesAsync()
        {
            return await _purityRepository.GetAllPuritiesAsync();
        }

        public async Task<Purity> CreatePurityAsync(Purity purity)
        {
            return await _purityRepository.CreatePurityAsync(purity);
        }

        public async Task<Purity> UpdatePurityAsync(Purity purity)
        {
            return await _purityRepository.UpdatePurityAsync(purity);
        }

        public async Task<bool> DeletePurityAsync(int id)
        {
            return await _purityRepository.DeletePurityAsync(id);
        }
    }
}