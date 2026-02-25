using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class ItemStoneRepository : IItemStoneRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ItemStoneRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ItemStone> GetItemStoneByIdAsync(int id)
        {
            var itemStoneDb = await _context.ItemStones
                .Include(s => s.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<ItemStone>(itemStoneDb);
        }

        public async Task<IEnumerable<ItemStone>> GetAllItemStonesAsync()
        {
            var itemStonesDb = await _context.ItemStones
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<ItemStone>>(itemStonesDb);
        }

        public async Task<IEnumerable<ItemStone>> GetStonesByItemIdAsync(long jewelleryItemId)
        {
            var itemStonesDb = await _context.ItemStones
                .Where(s => s.JewelleryItemId == jewelleryItemId)
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<ItemStone>>(itemStonesDb);
        }

        public async Task<ItemStone> CreateItemStoneAsync(ItemStone itemStone)
        {
            var entity = _mapper.Map<ItemStoneDb>(itemStone);
            await _context.ItemStones.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ItemStone>(entity);
        }

        public async Task<ItemStone> UpdateItemStoneAsync(ItemStone itemStone)
        {
            var entity = await _context.ItemStones.FindAsync(itemStone.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.JewelleryItemId = itemStone.JewelleryItemId;
            entity.StoneId = itemStone.StoneId;
            entity.Quantity = itemStone.Quantity;
            entity.Weight = itemStone.Weight;
            entity.StatusId = itemStone.StatusId;
            entity.UpdatedBy = itemStone.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<ItemStone>(entity);
        }

        public async Task<bool> DeleteItemStoneAsync(int id)
        {
            var entity = await _context.ItemStones.FindAsync(id);
            if (entity == null) return false;
            _context.ItemStones.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}