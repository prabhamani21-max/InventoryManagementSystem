using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class ExchangeValuationService : IExchangeValuationService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ILogger<ExchangeValuationService> _logger;

        public ExchangeValuationService(
            IExchangeRepository exchangeRepository,
            ILogger<ExchangeValuationService> logger)
        {
            _exchangeRepository = exchangeRepository;
            _logger = logger;
        }

        public async Task<ExchangeCalculationResult> CalculateAsync(ExchangeCalculationRequest request)
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
                var (ratePerGram, purityPercentage) = await _exchangeRepository.GetCurrentRateWithPurityAsync(item.PurityId);

                if (ratePerGram <= 0)
                {
                    throw new InvalidOperationException($"No active metal rate found for purity {item.PurityId}.");
                }

                var purity = purityPercentage / 100m;
                var pureWeight = item.NetWeight * purity;
                // Rate per gram is already tied to the selected purity, so valuation uses net weight directly.
                var marketValue = item.NetWeight * ratePerGram;

                var totalDeductionPercent = item.MakingChargeDeductionPercent + item.WastageDeductionPercent;
                var deductionAmount = marketValue * (totalDeductionPercent / 100m);
                var creditAmount = marketValue - deductionAmount;

                response.ItemDetails.Add(new ExchangeItemCalculation
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
                    DeductionAmount = deductionAmount,
                    CreditAmount = creditAmount
                });

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

            return response;
        }
    }
}
