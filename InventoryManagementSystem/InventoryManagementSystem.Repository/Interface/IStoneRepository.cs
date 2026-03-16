using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IStoneRepository
    {
        Task<Stone> GetStoneByIdAsync(int id);
        Task<IEnumerable<Stone>> GetAllStonesAsync();
        Task<Stone> CreateStoneAsync(Stone stone);
        Task<Stone> UpdateStoneAsync(Stone stone);
        Task<bool> DeleteStoneAsync(int id);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<IEnumerable<Stone>> SearchStonesByNameAsync(string name);
    }
}