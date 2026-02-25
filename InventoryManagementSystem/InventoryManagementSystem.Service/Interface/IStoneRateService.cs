using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IStoneRateService
    {
        /// <summary>
        /// Get current rate for a stone based on 4Cs or grade
        /// </summary>
        Task<StoneRateHistory?> GetCurrentRateBySearchAsync(
            int? stoneId = null,
            decimal? carat = null,
            string? cut = null,
            string? color = null,
            string? clarity = null,
            string? grade = null);

        /// <summary>
        /// Get current rate for a stone by ID
        /// </summary>
        Task<StoneRateHistory?> GetCurrentRateByStoneIdAsync(int stoneId);

        /// <summary>
        /// Get current diamond rate by 4Cs
        /// </summary>
        Task<StoneRateHistory?> GetDiamondRateBy4CsAsync(decimal carat, string cut, string color, string clarity);

        /// <summary>
        /// Get stone rate by ID
        /// </summary>
        Task<StoneRateHistory?> GetRateByIdAsync(int id);

        /// <summary>
        /// Add a new stone rate entry
        /// </summary>
        Task<StoneRateHistory> AddStoneRateAsync(StoneRateHistory entity);

        /// <summary>
        /// Update stone rate
        /// </summary>
        Task<StoneRateHistory> UpdateStoneRateAsync(StoneRateHistory entity);

        /// <summary>
        /// Get rate history for a stone
        /// </summary>
        Task<IEnumerable<StoneRateHistory>> GetRateHistoryByStoneIdAsync(int stoneId);

        /// <summary>
        /// Get all current stone rates
        /// </summary>
        Task<IEnumerable<StoneRateHistory>> GetAllCurrentRatesAsync();

        /// <summary>
        /// Get the latest rate per unit (used by Price Calculation Engine)
        /// </summary>
        Task<decimal> GetLatestRatePerUnitAsync(int stoneId, decimal? carat = null, string? cut = null, string? color = null, string? clarity = null, string? grade = null);

        /// <summary>
        /// Get diamond rate card (all 4Cs combinations)
        /// </summary>
        Task<IEnumerable<StoneRateHistory>> GetDiamondRateCardAsync();
    }
}
