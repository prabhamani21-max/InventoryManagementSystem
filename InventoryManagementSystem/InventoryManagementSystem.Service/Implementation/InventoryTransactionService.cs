using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IInventoryTransactionRepository _repository;

        public InventoryTransactionService(IInventoryTransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<InventoryTransaction> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<InventoryTransaction> CreateAsync(InventoryTransaction transaction)
        {
            return await _repository.AddAsync(transaction);
        }

        public async Task<InventoryTransaction> UpdateAsync(InventoryTransaction transaction)
        {
            return await _repository.UpdateAsync(transaction);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}