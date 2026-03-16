using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class StoneRepository : IStoneRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StoneRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Stone> GetStoneByIdAsync(int id)
        {
            var stoneDb = await _context.Stones
                .Include(s => s.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<Stone>(stoneDb);
        }

        public async Task<IEnumerable<Stone>> GetAllStonesAsync()
        {
            var stonesDb = await _context.Stones
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Stone>>(stonesDb);
        }

        public async Task<Stone> CreateStoneAsync(Stone stone)
        {
            var entity = _mapper.Map<StoneDb>(stone);
            await _context.Stones.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<Stone>(entity);
        }

        public async Task<Stone> UpdateStoneAsync(Stone stone)
        {
            var entity = await _context.Stones.FindAsync(stone.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.Name = stone.Name;
            entity.Unit = stone.Unit;
            entity.StatusId = stone.StatusId;
            entity.UpdatedBy = stone.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<Stone>(entity);
        }

        public async Task<bool> DeleteStoneAsync(int id)
        {
            var entity = await _context.Stones.FindAsync(id);
            if (entity == null) return false;
            _context.Stones.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.Stones.Where(s => s.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Stone>> SearchStonesByNameAsync(string name)
        {
            var stonesDb = await _context.Stones
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<Stone>>(stonesDb);
        }
    }
}