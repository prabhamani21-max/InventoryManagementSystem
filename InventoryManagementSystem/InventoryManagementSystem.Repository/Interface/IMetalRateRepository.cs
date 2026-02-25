using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IMetalRateRepository
    {
        Task<MetalRateHistory?> GetCurrentRateByPurityIdAsync(int purityId);
        Task<MetalRateHistory?> GetRateByIdAsync(int id);
        Task<IEnumerable<MetalRateHistory>> GetCurrentRatesByMetalIdAsync(int metalId);
        Task<IEnumerable<MetalRateHistory>> GetAllCurrentRatesAsync();
        Task<MetalRateHistory> AddMetalRateAsync(MetalRateHistory metalRate);
        Task<IEnumerable<MetalRateHistory>> GetRateHistoryByPurityIdAsync(int purityId);
        Task<IEnumerable<MetalRateHistory>> GetRateHistoryByMetalIdAsync(int metalId, DateTime startDate, DateTime endDate);
        Task<MetalRateHistory> UpdateMetalRateAsync(MetalRateHistory metalRate);
        Task<decimal> GetLatestRatePerGramAsync(int purityId);
    }
}
