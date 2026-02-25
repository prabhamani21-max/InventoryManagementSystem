using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IMetalRepository
    {
        Task<Metal> GetMetalByIdAsync(int id);
        Task<IEnumerable<Metal>> GetAllMetalsAsync();
        Task<Metal> CreateMetalAsync(Metal metal);
        Task<Metal> UpdateMetalAsync(Metal metal);
        Task<bool> DeleteMetalAsync(int id);
    }
}