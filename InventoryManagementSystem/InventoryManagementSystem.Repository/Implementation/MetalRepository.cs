using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class MetalRepository : IMetalRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MetalRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Metal> GetMetalByIdAsync(int id)
        {
            var metalDb = await _context.Metals
                .Include(m => m.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            return _mapper.Map<Metal>(metalDb);
        }

        public async Task<IEnumerable<Metal>> GetAllMetalsAsync()
        {
            var metalsDb = await _context.Metals
                .Include(m => m.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Metal>>(metalsDb);
        }

        public async Task<Metal> CreateMetalAsync(Metal metal)
        {
            var entity = _mapper.Map<MetalDb>(metal);
            await _context.Metals.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Metal>(entity);
        }

        public async Task<Metal> UpdateMetalAsync(Metal metal)
        {
            var entity = await _context.Metals.FindAsync(metal.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.Name = metal.Name;
            entity.StatusId = metal.StatusId;
            entity.UpdatedBy = metal.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Metal>(entity);
        }

        public async Task<bool> DeleteMetalAsync(int id)
        {
            var entity = await _context.Metals.FindAsync(id);
            if (entity == null) return false;
            _context.Metals.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}