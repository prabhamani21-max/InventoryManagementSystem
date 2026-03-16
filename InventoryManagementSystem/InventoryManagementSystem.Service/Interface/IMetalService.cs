using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IMetalService
    {
        Task<Metal> GetMetalByIdAsync(int id);
        Task<IEnumerable<Metal>> GetAllMetalsAsync();
        Task<Metal> CreateMetalAsync(Metal metal);
        Task<Metal> UpdateMetalAsync(Metal metal);
        Task<bool> DeleteMetalAsync(int id);
    }
}