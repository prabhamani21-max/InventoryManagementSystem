using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class JewelleryItemRepository : IJewelleryItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public JewelleryItemRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<JewelleryItem> GetJewelleryItemByIdAsync(long id)
        {
            var jewelleryItemDb = await _context.JewelleryItems
                .Include(j => j.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
            return _mapper.Map<JewelleryItem>(jewelleryItemDb);
        }

        public async Task<IEnumerable<JewelleryItem>> GetAllJewelleryItemsAsync()
        {
            var jewelleryItemsDb = await _context.JewelleryItems
                .Include(j => j.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<JewelleryItem>>(jewelleryItemsDb);
        }

        public async Task<JewelleryItem> CreateJewelleryItemAsync(JewelleryItem jewelleryItem)
        {
            var entity = _mapper.Map<JewelleryItemDb>(jewelleryItem);
            await _context.JewelleryItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<JewelleryItem>(entity);
        }

        public async Task<JewelleryItem> UpdateJewelleryItemAsync(JewelleryItem jewelleryItem)
        {
            var entity = await _context.JewelleryItems.FindAsync(jewelleryItem.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.ItemCode = jewelleryItem.ItemCode;
            entity.Name = jewelleryItem.Name;
            entity.Description = jewelleryItem.Description;
            entity.CategoryId = jewelleryItem.CategoryId;
            entity.HasStone = jewelleryItem.HasStone;
            entity.StoneId = jewelleryItem.StoneId;
            entity.MetalId = jewelleryItem.MetalId;
            entity.PurityId = jewelleryItem.PurityId;
            entity.GrossWeight = jewelleryItem.GrossWeight;
            entity.NetMetalWeight = jewelleryItem.NetMetalWeight;
            entity.MakingChargeType = jewelleryItem.MakingChargeType;
            entity.MakingChargeValue = jewelleryItem.MakingChargeValue;
            entity.WastagePercentage = jewelleryItem.WastagePercentage;
            entity.IsHallmarked = jewelleryItem.IsHallmarked;
            entity.HUID = jewelleryItem.HUID;
            entity.BISCertificationNumber = jewelleryItem.BISCertificationNumber;
            entity.HallmarkCenterName = jewelleryItem.HallmarkCenterName;
            entity.HallmarkDate = jewelleryItem.HallmarkDate;
            entity.StatusId = jewelleryItem.StatusId;
            entity.UpdatedBy = jewelleryItem.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<JewelleryItem>(entity);
        }

        public async Task<bool> DeleteJewelleryItemAsync(long id)
        {
            var entity = await _context.JewelleryItems.FindAsync(id);
            if (entity == null) return false;
            _context.JewelleryItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // New methods for Db model access with navigation properties
        public async Task<JewelleryItemDb?> GetJewelleryItemDbByIdAsync(long id)
        {
            return await _context.JewelleryItems
                .Include(j => j.Status)
                .Include(j => j.Metal)
                .Include(j => j.Purity)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<IEnumerable<JewelleryItemDb>> GetAllJewelleryItemsDbAsync()
        {
            return await _context.JewelleryItems
                .Include(j => j.Status)
                .Include(j => j.Metal)
                .Include(j => j.Purity)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<JewelleryItemDb> UpdateJewelleryItemDbAsync(JewelleryItemDb jewelleryItem)
        {
            _context.JewelleryItems.Update(jewelleryItem);
            await _context.SaveChangesAsync();
            return jewelleryItem;
        }
    }
}
