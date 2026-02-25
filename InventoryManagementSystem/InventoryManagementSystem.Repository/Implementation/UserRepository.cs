using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        // Assuming you have a DbContext, replace with your actual context
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == id);
            return _mapper.Map<User>(user);
        }
        public async Task<UserDb> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserDb> GetUserByContactNumberAsync(string contactNumber)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ContactNumber == contactNumber);
        }

        public async Task<User> RegisterUserAsync(User user)
        {
            var entity = _mapper.Map<UserDb>(user);
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ContactNumberExistsAsync(string contactNo)
        {
            return await _context.Users.AnyAsync(u => u.ContactNumber == contactNo);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Status)
                .ToListAsync();
            return _mapper.Map<List<User>>(users);
        }
    }
}