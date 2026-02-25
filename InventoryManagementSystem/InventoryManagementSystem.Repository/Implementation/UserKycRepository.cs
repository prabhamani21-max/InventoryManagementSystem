using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class UserKycRepository : IUserKycRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserKycRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserKyc> GetUserKycByIdAsync(long id)
        {
            var userKycDb = await _context.UserKycs
                .Include(u => u.User)
                .Include(u => u.Status)
                .Include(u => u.CreatedUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
            return _mapper.Map<UserKyc>(userKycDb);
        }

        public async Task<UserKyc> GetUserKycByUserIdAsync(long userId)
        {
            var userKycDb = await _context.UserKycs
                .Include(u => u.User)
                .Include(u => u.Status)
                .Include(u => u.CreatedUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
            return _mapper.Map<UserKyc>(userKycDb);
        }

        public async Task<UserKyc> CreateUserKycAsync(UserKyc userKyc)
        {
            var entity = _mapper.Map<UserKycDb>(userKyc);
            await _context.UserKycs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<UserKyc>(entity);
        }

        public async Task<UserKyc> UpdateUserKycAsync(UserKyc userKyc)
        {
            var entity = await _context.UserKycs
                .FirstOrDefaultAsync(x => x.Id == userKyc.Id);

            if (entity == null)
                throw new Exception("User KYC not found");

            // Manually map updateable fields - preserve CreatedBy and CreatedDate
            entity.UserId = userKyc.UserId;
            entity.PanCardNumber = userKyc.PanCardNumber;
            entity.AadhaarCardNumber = userKyc.AadhaarCardNumber;
            entity.IsVerified = userKyc.IsVerified;
            entity.StatusId = userKyc.StatusId;
            entity.UpdatedBy = userKyc.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<UserKyc>(entity);
        }


        public async Task<bool> DeleteUserKycAsync(long id)
        {
            var entity = await _context.UserKycs.FindAsync(id);
            if (entity == null) return false;
            _context.UserKycs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserKyc>> GetAllUserKycsAsync()
        {
            var userKycsDb = await _context.UserKycs
                .Include(u => u.User)
                .Include(u => u.Status)
                .Include(u => u.CreatedUser)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<UserKyc>>(userKycsDb);
        }
        public async Task<bool> UserKycExistsAsync(long userId, long? excludeId = null)
        {
            return await _context.UserKycs
                .AnyAsync(x => x.UserId == userId && (excludeId == null || x.Id != excludeId));
        }
        public async Task<bool> PanCardNumberExistsAsync(string panCardNumber, long? excludeId = null)
        {
            return await _context.UserKycs.AnyAsync(u => u.PanCardNumber == panCardNumber && (excludeId == null || u.Id != excludeId));
        }

        public async Task<bool> AadhaarCardNumberExistsAsync(string aadhaarCardNumber, long? excludeId = null)
        {
            return await _context.UserKycs.AnyAsync(u => u.AadhaarCardNumber == aadhaarCardNumber && (excludeId == null || u.Id != excludeId));
        }
    }
}