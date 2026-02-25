using AutoMapper;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Common.Models;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ExchangeRepository> _logger;

        public ExchangeRepository(AppDbContext context, IMapper mapper, ILogger<ExchangeRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
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
            entity.NewPurchaseAmount = exchangeOrder.NewPurchaseAmount;
            entity.BalanceRefund = exchangeOrder.BalanceRefund;
            entity.CashPayment = exchangeOrder.CashPayment;
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
            
            // Get cancelled status id (typically 5 = cancelled)
            var cancelledStatus = await _context.Status
                .FirstOrDefaultAsync(s => s.Name.ToUpper() == "CANCELLED");
            
            if (cancelledStatus != null)
            {
                entity.StatusId = cancelledStatus.Id;
            }
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ExchangeCalculationResult> CalculateExchangeValueAsync(ExchangeCalculationRequest request)
        {
            _logger.LogInformation("Calculating exchange value for customer {CustomerId}", request.CustomerId);

            var response = new ExchangeCalculationResult
            {
                CustomerId = request.CustomerId,
                ExchangeType = request.ExchangeType == 1 ? "EXCHANGE" : "BUYBACK",
                ItemDetails = new List<ExchangeItemCalculation>()
            };

            decimal totalGrossWeight = 0;
            decimal totalNetWeight = 0;
            decimal totalPureWeight = 0;
            decimal totalMarketValue = 0;
            decimal totalDeductionAmount = 0;

            foreach (var item in request.Items)
            {
                // Get current metal rate for the purity
                var (ratePerGram, purityPercentage) = await GetCurrentRateWithPurityAsync(item.PurityId);

                if (ratePerGram <= 0)
                {
                    _logger.LogWarning("No metal rate found for purity {PurityId}", item.PurityId);
                    continue;
                }

                // Calculate pure weight based on purity percentage
                var purity = purityPercentage / 100m;
                var pureWeight = item.NetWeight * purity;
                var marketValue = pureWeight * ratePerGram;

                // Calculate deductions
                var totalDeductionPercent = item.MakingChargeDeductionPercent + item.WastageDeductionPercent;
                var deductionAmount = marketValue * (totalDeductionPercent / 100m);
                var creditAmount = marketValue - deductionAmount;

                var itemDetail = new ExchangeItemCalculation
                {
                    MetalId = item.MetalId,
                    PurityId = item.PurityId,
                    GrossWeight = item.GrossWeight,
                    NetWeight = item.NetWeight,
                    PurityPercentage = purityPercentage,
                    PureWeight = pureWeight,
                    CurrentRatePerGram = ratePerGram,
                    MarketValue = marketValue,
                    MakingChargeDeductionPercent = item.MakingChargeDeductionPercent,
                    WastageDeductionPercent = item.WastageDeductionPercent,
                    TotalDeductionPercent = totalDeductionPercent,
                    DeductionAmount = deductionAmount,
                    CreditAmount = creditAmount
                };

                response.ItemDetails.Add(itemDetail);

                totalGrossWeight += item.GrossWeight;
                totalNetWeight += item.NetWeight;
                totalPureWeight += pureWeight;
                totalMarketValue += marketValue;
                totalDeductionAmount += deductionAmount;
            }

            response.TotalGrossWeight = totalGrossWeight;
            response.TotalNetWeight = totalNetWeight;
            response.TotalPureWeight = totalPureWeight;
            response.TotalMarketValue = totalMarketValue;
            response.TotalDeductionAmount = totalDeductionAmount;
            response.TotalCreditAmount = totalMarketValue - totalDeductionAmount;
            response.ItemCount = request.Items.Count;
            response.NewPurchaseAmount = request.NewPurchaseAmount;

            // Calculate balance refund or cash payment
            if (request.NewPurchaseAmount.HasValue)
            {
                if (response.TotalCreditAmount > request.NewPurchaseAmount.Value)
                {
                    response.BalanceRefund = response.TotalCreditAmount - request.NewPurchaseAmount.Value;
                }
                else
                {
                    response.CashPayment = request.NewPurchaseAmount.Value - response.TotalCreditAmount;
                }
            }

            return response;
        }

        public async Task<int> GetStatusIdByNameAsync(string statusName)
        {
            // This would typically come from a status repository
            // For now, returning hardcoded values based on common patterns
            return statusName.ToUpper() switch
            {
                "PENDING" => 1,
                "COMPLETED" => 2,
                "CANCELLED" => 5,
                "PROCESSING" => 3,
                _ => 1
            };
        }

        private async Task<(decimal ratePerGram, decimal purityPercentage)> GetCurrentRateWithPurityAsync(int purityId)
        {
            // Get the purity percentage from the Purity table
            var purity = await _context.Purities
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == purityId);

            if (purity == null)
            {
                _logger.LogWarning("Purity {PurityId} not found", purityId);
                return (0, 0);
            }

            // Get the current metal rate
            var latestRate = await _context.MetalRateHistories
                .Where(g => g.PurityId == purityId)
                .OrderByDescending(g => g.EffectiveDate)
                .FirstOrDefaultAsync();

            if (latestRate == null)
            {
                _logger.LogWarning("No metal rate found for purity {PurityId}", purityId);
                return (0, purity.Percentage);
            }

            return (latestRate.RatePerGram, purity.Percentage);
        }
    }
}
