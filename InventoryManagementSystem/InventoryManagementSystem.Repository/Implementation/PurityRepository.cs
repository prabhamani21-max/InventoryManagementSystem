using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class PurityRepository : IPurityRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurityRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Purity> GetPurityByIdAsync(int id)
        {
            var purityDb = await _context.Purities
                .Include(p => p.Metal)
                .Include(p => p.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<Purity>(purityDb);
        }

        public async Task<IEnumerable<Purity>> GetAllPuritiesAsync()
        {
            var puritiesDb = await _context.Purities
                .Include(p => p.Metal)
                .Include(p => p.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Purity>>(puritiesDb);
        }

        public async Task<Purity> CreatePurityAsync(Purity purity)
        {
            var entity = _mapper.Map<PurityDb>(purity);
            await _context.Purities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Purity>(entity);
        }

        public async Task<Purity> UpdatePurityAsync(Purity purity)
        {
            var entity = await _context.Purities.FindAsync(purity.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.MetalId = purity.MetalId;
            entity.Name = purity.Name;
            entity.Percentage = purity.Percentage;
            entity.StatusId = purity.StatusId;
            entity.UpdatedBy = purity.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Purity>(entity);
        }

        public async Task<bool> DeletePurityAsync(int id)
        {
            var entity = await _context.Purities.FindAsync(id);
            if (entity == null) return false;
            _context.Purities.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}