
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user);
        Task<UserDb> GetUserByEmailAsync(string email);
        Task<UserDb> GetUserByContactNumberAsync(string contactNumber);
        Task<User> GetUserByIdAsync(long id);
        Task<List<User>> GetAllUsersAsync();
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ContactNumberExistsAsync(string contactNo);


    }
}