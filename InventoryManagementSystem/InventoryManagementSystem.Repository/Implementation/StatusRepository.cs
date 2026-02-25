using AutoMapper;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class StatusRepository : IStatusRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StatusRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<GenericStatus>> GetAllUserStatuses()
        {
            return await _context.Status
                .OrderBy(us => us.Id)
                .Select(us => new GenericStatus
                {
                    Id = us.Id,
                    Name = us.Name,
                    IsActive = us.IsActive,
                })
                .ToListAsync();
        }

        public async Task<GenericStatus> GetUserStatusById(int id)
        {
            var userStatus = await _context.Status
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.Id == id);

            if (userStatus == null)
                return null;

            return new GenericStatus
            {
                Id = userStatus.Id,
                Name = userStatus.Name,
                IsActive = userStatus.IsActive,
            };
        }

        public async Task<GenericStatus> AddEditUserStatus(GenericStatus status)
        {
            var existingStatus = await GetByName(status.Name);
            if (status.Id == 0)
            {

                var entity = _mapper.Map<GenericStatusDb>(status);
                await _context.Status.AddAsync(entity);
                await _context.SaveChangesAsync();
                return status;

            }
            else
            {
                var dbUserStatus = await _context.Status.FindAsync(status.Id);
                if (dbUserStatus == null)
                    return null;

                dbUserStatus.Name = status.Name;
                dbUserStatus.IsActive = status.IsActive;
                dbUserStatus.UpdatedBy = status.UpdatedBy;
                dbUserStatus.UpdatedDate = DateTime.UtcNow;

                _context.Status.Update(dbUserStatus);
                await _context.SaveChangesAsync();

                return new GenericStatus
                {
                    Id = dbUserStatus.Id,
                    Name = dbUserStatus.Name,
                    IsActive = dbUserStatus.IsActive,
                    UpdatedBy = dbUserStatus.UpdatedBy,
                    UpdatedDate = dbUserStatus.UpdatedDate
                };
            }
        }

        public async Task<GenericStatus> DeleteUserStatus(int id)
        {
            var userStatus = await _context.Status.FindAsync(id);
            if (userStatus == null)
                return null;

            _context.Status.Remove(userStatus);
            await _context.SaveChangesAsync();

            return new GenericStatus
            {
                Id = userStatus.Id,
                Name = userStatus.Name,
            };
        }

        public async Task<bool> IsUserStatusInUse(int id)
        {
            return await _context.Users.AnyAsync(u => u.StatusId == id);
        }

        public async Task<GenericStatus> GetByName(string name)
        {
            var userStatus = await _context.Status
                .AsNoTracking()
                .FirstOrDefaultAsync(us => us.Name == name);

            if (userStatus == null)
                return null;

            return new GenericStatus
            {
                Id = userStatus.Id,
                Name = userStatus.Name,
                IsActive = userStatus.IsActive,
            };
        }
    }
}