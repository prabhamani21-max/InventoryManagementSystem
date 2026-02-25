using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IInventoryTransactionService
    {
        Task<IEnumerable<InventoryTransaction>> GetAllAsync();
        Task<InventoryTransaction> GetByIdAsync(int id);
        Task<InventoryTransaction> CreateAsync(InventoryTransaction transaction);
        Task<InventoryTransaction> UpdateAsync(InventoryTransaction transaction);
        Task<bool> DeleteAsync(int id);
    }
}