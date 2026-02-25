
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleById(int id);
        Task<Role> AddEditRole(Role role);
        Task<Role> DeleteRole(int id);
        Task<bool> IsRoleInUse(int id);
        Task<Role> GetByName(string name);
    }
}