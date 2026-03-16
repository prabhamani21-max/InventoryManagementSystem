using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Service.Interface
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllRoles();
        Task<Role> GetRoleById(int id);
        Task<Role> AddEditRole(Role role);
        Task<Role> DeleteRole(int id);
        Task<Role> GetByName(string roleName);
    }
}