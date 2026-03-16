using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class PurchaseOrderItemRepository : IPurchaseOrderItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseOrderItemRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PurchaseOrderItem> GetPurchaseOrderItemByIdAsync(int id)
        {
            var purchaseOrderItemDb = await _context.PurchaseOrderItems
                .Include(p => p.PurchaseOrder)
                .Include(p => p.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<PurchaseOrderItem>(purchaseOrderItemDb);
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetAllPurchaseOrderItemsAsync()
        {
            var purchaseOrderItemsDb = await _context.PurchaseOrderItems
                .Include(p => p.PurchaseOrder)
                .Include(p => p.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<PurchaseOrderItem>>(purchaseOrderItemsDb);
        }

        public async Task<PurchaseOrderItem> CreatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem)
        {
            var entity = _mapper.Map<PurchaseOrderItemDb>(purchaseOrderItem);
            await _context.PurchaseOrderItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<PurchaseOrderItem>(entity);
        }

        public async Task<PurchaseOrderItem> UpdatePurchaseOrderItemAsync(PurchaseOrderItem purchaseOrderItem)
        {
            var entity = await _context.PurchaseOrderItems.FindAsync(purchaseOrderItem.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.PurchaseOrderId = purchaseOrderItem.PurchaseOrderId;
            entity.JewelleryItemId = purchaseOrderItem.JewelleryItemId;
            entity.Quantity = purchaseOrderItem.Quantity;
            entity.UnitPrice = purchaseOrderItem.UnitPrice;
            entity.TotalPrice = purchaseOrderItem.TotalPrice;
            entity.StatusId = purchaseOrderItem.StatusId;
            entity.UpdatedBy = purchaseOrderItem.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<PurchaseOrderItem>(entity);
        }

        public async Task<bool> DeletePurchaseOrderItemAsync(int id)
        {
            var entity = await _context.PurchaseOrderItems.FindAsync(id);
            if (entity == null) return false;
            _context.PurchaseOrderItems.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}