using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IMetalRepository
    {
        Task<Metal> GetMetalByIdAsync(int id);
        Task<IEnumerable<Metal>> GetAllMetalsAsync();
        Task<Metal> CreateMetalAsync(Metal metal);
        Task<Metal> UpdateMetalAsync(Metal metal);
        Task<bool> DeleteMetalAsync(int id);

        /// <summary>
        /// Gets metals by a list of IDs
        /// </summary>
        /// <param name="ids">Collection of metal IDs</param>
        /// <returns>Dictionary of metals keyed by ID</returns>
        Task<Dictionary<int, MetalDb>> GetMetalsByIdsAsync(IEnumerable<int> ids);
    }
}