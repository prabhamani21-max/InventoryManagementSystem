using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IUserRepository
    {
        public Task<UserDb> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(long id);
        public Task<UserDb> GetUserByContactNumberAsync(string contactNumber);
        public Task<User> RegisterUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ContactNumberExistsAsync(string contactNo);
        Task<List<User>> GetAllUsersAsync();
    }
}