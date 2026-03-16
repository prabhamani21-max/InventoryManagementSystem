using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IPurityService
    {
        Task<Purity> GetPurityByIdAsync(int id);
        Task<IEnumerable<Purity>> GetAllPuritiesAsync();
        Task<Purity> CreatePurityAsync(Purity purity);
        Task<Purity> UpdatePurityAsync(Purity purity);
        Task<bool> DeletePurityAsync(int id);
    }
}