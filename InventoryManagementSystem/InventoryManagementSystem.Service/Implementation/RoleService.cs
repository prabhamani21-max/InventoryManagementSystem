
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _roleRepository.GetAllRoles();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await _roleRepository.GetRoleById(id);
        }

        public async Task<Role> AddEditRole(Role role)
        {
            return await _roleRepository.AddEditRole(role);
        }

        public async Task<Role> DeleteRole(int id)
        {
            return await _roleRepository.DeleteRole(id);
        }
        public async Task<Role> GetByName(string roleName)
        {
            return await _roleRepository.GetByName(roleName);
        }
    }
}