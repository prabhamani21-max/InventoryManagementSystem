using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PurchaseOrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            var purchaseOrderDb = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<PurchaseOrder>(purchaseOrderDb);
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            var purchaseOrdersDb = await _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<PurchaseOrder>>(purchaseOrdersDb);
        }

        public async Task<PurchaseOrder> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var entity = _mapper.Map<PurchaseOrderDb>(purchaseOrder);
            await _context.PurchaseOrders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<PurchaseOrder>(entity);
        }

        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            var entity = await _context.PurchaseOrders.FindAsync(purchaseOrder.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.SupplierId = purchaseOrder.SupplierId;
            entity.OrderNumber = purchaseOrder.OrderNumber;
            entity.OrderDate = purchaseOrder.OrderDate;
            entity.ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate;
            entity.TotalAmount = purchaseOrder.TotalAmount;
            entity.StatusId = purchaseOrder.StatusId;
            entity.UpdatedBy = purchaseOrder.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<PurchaseOrder>(entity);
        }

        public async Task<bool> DeletePurchaseOrderAsync(int id)
        {
            var entity = await _context.PurchaseOrders.FindAsync(id);
            if (entity == null) return false;
            _context.PurchaseOrders.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}