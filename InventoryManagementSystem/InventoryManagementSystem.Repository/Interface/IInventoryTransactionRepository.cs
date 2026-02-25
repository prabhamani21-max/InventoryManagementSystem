using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IInventoryTransactionRepository
    {
        Task<IEnumerable<InventoryTransaction>> GetAllAsync();
        Task<InventoryTransaction> GetByIdAsync(int id);
        Task<InventoryTransaction> AddAsync(InventoryTransaction transaction);
        Task<InventoryTransaction> UpdateAsync(InventoryTransaction transaction);
        Task<bool> DeleteAsync(int id);
    }
}