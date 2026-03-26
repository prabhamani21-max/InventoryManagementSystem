using AutoMapper;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ExchangeRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByIdAsync(long id)
        {
            var exchangeOrderDb = await _context.ExchangeOrders
                .Include(eo => eo.ExchangeItems)
                .Include(eo => eo.Customer)
                .Include(eo => eo.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(eo => eo.Id == id);
            return _mapper.Map<ExchangeOrder>(exchangeOrderDb);
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByOrderNumberAsync(string orderNumber)
        {
            var exchangeOrderDb = await _context.ExchangeOrders
                .Include(eo => eo.ExchangeItems)
                .Include(eo => eo.Customer)
                .Include(eo => eo.Status)
                .AsNoTracking()
                .FirstOrDefaultAsync(eo => eo.OrderNumber == orderNumber);
            return _mapper.Map<ExchangeOrder>(exchangeOrderDb);
        }

        public async Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCustomerIdAsync(long customerId)
        {
            var exchangeOrdersDb = await _context.ExchangeOrders
                .Include(eo => eo.ExchangeItems)
                .Include(eo => eo.Customer)
                .Include(eo => eo.Status)
                .AsNoTracking()
                .Where(eo => eo.CustomerId == customerId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExchangeOrder>>(exchangeOrdersDb);
        }

        public async Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync()
        {
            var exchangeOrdersDb = await _context.ExchangeOrders
                .Include(eo => eo.ExchangeItems)
                .Include(eo => eo.Customer)
                .Include(eo => eo.Status)
                .AsNoTracking()
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExchangeOrder>>(exchangeOrdersDb);
        }

        public async Task<ExchangeOrder> CreateExchangeOrderAsync(ExchangeOrder exchangeOrder)
        {
            var entity = _mapper.Map<ExchangeOrderDb>(exchangeOrder);
            await _context.ExchangeOrders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ExchangeOrder>(entity);
        }

        public async Task<ExchangeOrder> UpdateExchangeOrderAsync(ExchangeOrder exchangeOrder)
        {
            var entity = await _context.ExchangeOrders.FindAsync(exchangeOrder.Id);
            if (entity == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            entity.OrderNumber = exchangeOrder.OrderNumber;
            entity.CustomerId = exchangeOrder.CustomerId;
            entity.ExchangeType = exchangeOrder.ExchangeType == 1 ? "EXCHANGE" : "BUYBACK";
            entity.TotalGrossWeight = exchangeOrder.TotalGrossWeight;
            entity.TotalNetWeight = exchangeOrder.TotalNetWeight;
            entity.TotalPureWeight = exchangeOrder.TotalPureWeight;
            entity.TotalMarketValue = exchangeOrder.TotalMarketValue;
            entity.TotalDeductionAmount = exchangeOrder.TotalDeductionAmount;
            entity.TotalCreditAmount = exchangeOrder.TotalCreditAmount;
            entity.StatusId = exchangeOrder.StatusId;
            entity.Notes = exchangeOrder.Notes;
            entity.ExchangeDate = exchangeOrder.ExchangeDate;
            entity.UpdatedBy = exchangeOrder.UpdatedBy;
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<ExchangeOrder>(entity);
        }

        public async Task<bool> CancelExchangeOrderAsync(long orderId)
        {
            var entity = await _context.ExchangeOrders.FindAsync(orderId);
            if (entity == null) return false;

            entity.StatusId = (int)StatusEnum.Deleted;
            
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Get all exchange orders created by a specific sales person
        /// </summary>
        /// <param name="createdBy">The sales person's user ID</param>
        /// <returns>List of exchange orders created by the sales person</returns>
        public async Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCreatedByAsync(long createdBy)
        {
            var exchangeOrdersDb = await _context.ExchangeOrders
                .Include(eo => eo.ExchangeItems)
                .Include(eo => eo.Customer)
                .Include(eo => eo.Status)
                .AsNoTracking()
                .Where(eo => eo.CreatedBy == createdBy)
                .OrderByDescending(eo => eo.ExchangeDate)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ExchangeOrder>>(exchangeOrdersDb);
        }

        public async Task<(decimal RatePerGram, decimal PurityPercentage)> GetCurrentRateWithPurityAsync(int purityId)
        {
            var purity = await _context.Purities
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == purityId);

            if (purity == null)
            {
                return (0, 0);
            }

            var latestRate = await _context.MetalRateHistories
                .Where(g => g.PurityId == purityId)
                .OrderByDescending(g => g.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                return (0, purity.Percentage);
            }

            return (latestRate.RatePerGram, purity.Percentage);
        }
    }
}
