using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IPurityRepository
    {
        Task<Purity> GetPurityByIdAsync(int id);
        Task<IEnumerable<Purity>> GetAllPuritiesAsync();
        Task<Purity> CreatePurityAsync(Purity purity);
        Task<Purity> UpdatePurityAsync(Purity purity);
        Task<bool> DeletePurityAsync(int id);

        /// <summary>
        /// Gets purities by a list of IDs
        /// </summary>
        /// <param name="ids">Collection of purity IDs</param>
        /// <returns>Dictionary of purities keyed by ID</returns>
        Task<Dictionary<int, PurityDb>> GetPuritiesByIdsAsync(IEnumerable<int> ids);
    }
}