using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class SaleOrderItemRepository : ISaleOrderItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SaleOrderItemRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SaleOrderItem> GetSaleOrderItemByIdAsync(int id)
        {
            var saleOrderItemDb = await _context.SaleOrderItems
                .Include(s => s.SaleOrder)
                .Include(s => s.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<SaleOrderItem>(saleOrderItemDb);
        }

        public async Task<IEnumerable<SaleOrderItem>> GetAllSaleOrderItemsAsync()
        {
            var saleOrderItemsDb = await _context.SaleOrderItems
                .Include(s => s.SaleOrder)
                .Include(s => s.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleOrderItem>>(saleOrderItemsDb);
        }

        public async Task<SaleOrderItem> CreateSaleOrderItemAsync(SaleOrderItem saleOrderItem)
        {
            var entity = _mapper.Map<SaleOrderItemDb>(saleOrderItem);
            await _context.SaleOrderItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<SaleOrderItem>(entity);
        }

        public async Task<SaleOrderItem> UpdateSaleOrderItemAsync(SaleOrderItem saleOrderItem)
        {
            var entity = await _context.SaleOrderItems.FindAsync(saleOrderItem.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.SaleOrderId = saleOrderItem.SaleOrderId;
            entity.JewelleryItemId = saleOrderItem.JewelleryItemId;
            entity.ItemCode = saleOrderItem.ItemCode;
            entity.ItemName = saleOrderItem.ItemName;
            entity.Description = saleOrderItem.Description;
            entity.Quantity = saleOrderItem.Quantity;
            entity.MetalId = saleOrderItem.MetalId;
            entity.PurityId = saleOrderItem.PurityId;
            entity.GrossWeight = saleOrderItem.GrossWeight;
            entity.NetMetalWeight = saleOrderItem.NetMetalWeight;
            entity.MetalRatePerGram = saleOrderItem.MetalRatePerGram;
            entity.MetalAmount = saleOrderItem.MetalAmount;
            entity.MakingChargeType = saleOrderItem.MakingChargeType;
            entity.MakingChargeValue = saleOrderItem.MakingChargeValue;
            entity.TotalMakingCharges = saleOrderItem.TotalMakingCharges;
            entity.WastagePercentage = saleOrderItem.WastagePercentage;
            entity.WastageWeight = saleOrderItem.WastageWeight;
            entity.WastageAmount = saleOrderItem.WastageAmount;
            entity.HasStone = saleOrderItem.HasStone;
            entity.StoneAmount = saleOrderItem.StoneAmount;
            entity.ItemSubtotal = saleOrderItem.ItemSubtotal;
            entity.DiscountAmount = saleOrderItem.DiscountAmount;
            entity.TaxableAmount = saleOrderItem.TaxableAmount;
            entity.GstPercentage = saleOrderItem.GstPercentage;
            entity.GstAmount = saleOrderItem.GstAmount;
            entity.TotalAmount = saleOrderItem.TotalAmount;
            entity.IsHallmarked = saleOrderItem.IsHallmarked;
            entity.HUID = saleOrderItem.HUID;
            entity.BISCertificationNumber = saleOrderItem.BISCertificationNumber;
            entity.HallmarkCenterName = saleOrderItem.HallmarkCenterName;
            entity.HallmarkDate = saleOrderItem.HallmarkDate;
            entity.StatusId = saleOrderItem.StatusId;
            entity.UpdatedBy = saleOrderItem.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<SaleOrderItem>(entity);
        }

        public async Task<bool> DeleteSaleOrderItemAsync(int id)
        {
            var entity = await _context.SaleOrderItems.FindAsync(id);
            if (entity == null) return false;
            _context.SaleOrderItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}