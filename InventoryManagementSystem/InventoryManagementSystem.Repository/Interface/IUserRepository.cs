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

        /// <summary>
        /// Get all customers served by a specific sales person
        /// </summary>
        /// <param name="salesPersonId">The sales person's user ID</param>
        /// <returns>List of customers served by the sales person</returns>
        Task<IEnumerable<User>> GetCustomersBySalesPersonIdAsync(long salesPersonId);
    }
}