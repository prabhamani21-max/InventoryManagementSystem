using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class SaleOrderRepository : ISaleOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SaleOrderRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SaleOrder> GetSaleOrderByIdAsync(int id)
        {
            var saleOrderDb = await _context.SaleOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.ExchangeOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
            return _mapper.Map<SaleOrder>(saleOrderDb);
        }

        public async Task<IEnumerable<SaleOrder>> GetAllSaleOrdersAsync()
        {
            var saleOrdersDb = await _context.SaleOrders
                .Include(s => s.Customer)
                .Include(s => s.Status)
                .Include(s => s.ExchangeOrder)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<SaleOrder>>(saleOrdersDb);
        }

        public async Task<SaleOrder> CreateSaleOrderAsync(SaleOrder saleOrder)
        {
            var entity = _mapper.Map<SaleOrderDb>(saleOrder);
            await _context.SaleOrders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<SaleOrder>(entity);
        }

        public async Task<SaleOrder> UpdateSaleOrderAsync(SaleOrder saleOrder)
        {
            var entity = await _context.SaleOrders.FindAsync(saleOrder.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.CustomerId = saleOrder.CustomerId;
            entity.OrderNumber = saleOrder.OrderNumber;
            entity.OrderDate = saleOrder.OrderDate;
            entity.IsExchangeSale = saleOrder.IsExchangeSale;
            entity.ExchangeOrderId = saleOrder.ExchangeOrderId;
            entity.DeliveryDate = saleOrder.DeliveryDate;
            entity.StatusId = saleOrder.StatusId;
            entity.UpdatedBy = saleOrder.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<SaleOrder>(entity);
        }

        public async Task<bool> DeleteSaleOrderAsync(int id)
        {
            var entity = await _context.SaleOrders.FindAsync(id);
            if (entity == null) return false;
            _context.SaleOrders.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}