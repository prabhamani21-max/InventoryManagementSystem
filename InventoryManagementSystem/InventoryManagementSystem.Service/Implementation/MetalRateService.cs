using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class MetalRateService : IMetalRateService
    {
        private readonly IMetalRateRepository _metalRateRepository;

        public MetalRateService(IMetalRateRepository metalRateRepository)
        {
            _metalRateRepository = metalRateRepository;
        }

        public async Task<MetalRateHistory?> GetCurrentRateByPurityIdAsync(int purityId)
        {
            return await _metalRateRepository.GetCurrentRateByPurityIdAsync(purityId);
        }

        public async Task<MetalRateHistory?> GetRateByIdAsync(int id)
        {
            return await _metalRateRepository.GetRateByIdAsync(id);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetCurrentRatesByMetalIdAsync(int metalId)
        {
            return await _metalRateRepository.GetCurrentRatesByMetalIdAsync(metalId);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetAllCurrentRatesAsync()
        {
            return await _metalRateRepository.GetAllCurrentRatesAsync();
        }

        public async Task<MetalRateHistory> AddMetalRateAsync(MetalRateHistory metalRate)
        {
            return await _metalRateRepository.AddMetalRateAsync(metalRate);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetRateHistoryByPurityIdAsync(int purityId)
        {
            return await _metalRateRepository.GetRateHistoryByPurityIdAsync(purityId);
        }

        public async Task<IEnumerable<MetalRateHistory>> GetRateHistoryByMetalIdAsync(int metalId, DateTime startDate, DateTime endDate)
        {
            return await _metalRateRepository.GetRateHistoryByMetalIdAsync(metalId, startDate, endDate);
        }

        public async Task<MetalRateHistory> UpdateMetalRateAsync(MetalRateHistory metalRate)
        {
            return await _metalRateRepository.UpdateMetalRateAsync(metalRate);
        }

        public async Task<decimal> GetLatestRatePerGramAsync(int purityId)
        {
            return await _metalRateRepository.GetLatestRatePerGramAsync(purityId);
        }
    }
}
