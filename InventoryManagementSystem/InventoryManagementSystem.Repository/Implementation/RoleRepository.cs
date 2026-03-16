using AutoMapper;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;


namespace InventoryManagementSystem.Repository.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _context.Roles
                .Where(r => r.StatusId == r.StatusId)
                .OrderBy(r => r.Id)
                .Select(r => new Role
                {
                    Id = r.Id,
                    Name = r.Name,
                    StatusId = r.StatusId,
                })
                .ToListAsync();
        }

        public async Task<Role> GetRoleById(int id)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && r.StatusId == r.StatusId);

            return role == null ? null : new Role
            {
                Id = role.Id,
                Name = role.Name,
                StatusId = role.StatusId,
            };
        }

        public async Task<Role> AddEditRole(Role role)
        {
            var existingRole = await GetByName(role.Name);
           

            if (role.Id == 0) // Add new
            {
                var newRole = _mapper.Map<RoleDb>(role);
                await _context.Roles.AddAsync(newRole);
                await _context.SaveChangesAsync();
                return role;
            }
            else // Update
            {
                var dbRole = await _context.Roles.FindAsync(role.Id);

                dbRole.Name = role.Name;
                dbRole.StatusId = role.StatusId;
                dbRole.UpdatedBy = role.UpdatedBy;
                dbRole.UpdatedDate = DateTime.UtcNow;

                _context.Roles.Update(dbRole);
                await _context.SaveChangesAsync();

                return new Role
                {
                    Id = dbRole.Id,
                    Name = dbRole.Name,
                    StatusId = dbRole.StatusId,
                    UpdatedBy = dbRole.UpdatedBy,
                    UpdatedDate = dbRole.UpdatedDate
                };
            }
        }

        public async Task<Role> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return null;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return new Role { Id = role.Id, Name = role.Name };
        }

        public async Task<bool> IsRoleInUse(int id)
        {
            return await _context.Users.AnyAsync(u => u.RoleId == id && u.StatusId == u.StatusId);
        }

        public async Task<Role> GetByName(string name)
        {
            var role = await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == name);

            return role == null ? null : new Role
            {
                Id = role.Id,
                Name = role.Name,
                StatusId = role.StatusId,
            };
        }
    }
}