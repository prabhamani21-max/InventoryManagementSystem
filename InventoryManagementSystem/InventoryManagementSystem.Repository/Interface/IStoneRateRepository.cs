using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IStoneRateRepository
    {
        Task<StoneRateHistory?> GetCurrentRateBySearchAsync(
            int? stoneId = null,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null);

        Task<StoneRateHistory?> GetCurrentRateByStoneIdAsync(int stoneId);

        Task<StoneRateHistory?> GetDiamondRateBy4CsAsync(decimal carat, string cut, string color, string clarity);

        Task<StoneRateHistory?> GetRateByIdAsync(int id);

        Task<StoneRateHistory> AddAsync(StoneRateHistory entity);

        Task<StoneRateHistory> UpdateAsync(StoneRateHistory entity);

        Task<IEnumerable<StoneRateHistory>> GetRateHistoryByStoneIdAsync(int stoneId);

        Task<IEnumerable<StoneRateHistory>> GetAllCurrentRatesAsync();

        Task<decimal> GetLatestRatePerUnitAsync(
            int stoneId,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null);

        Task<IEnumerable<StoneRateHistory>> GetDiamondRateCardAsync();
    }
}
