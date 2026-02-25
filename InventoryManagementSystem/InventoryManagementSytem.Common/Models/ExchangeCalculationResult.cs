using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    /// <summary>
    /// Result model for exchange value calculation
    /// </summary>
    public class ExchangeCalculationResult
    {
        public long CustomerId { get; set; }
        public string ExchangeType { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalPureWeight { get; set; }
        public decimal TotalMarketValue { get; set; }
        public decimal TotalDeductionAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal? NewPurchaseAmount { get; set; }
        public decimal? BalanceRefund { get; set; }
        public decimal? CashPayment { get; set; }
        public List<ExchangeItemCalculation> ItemDetails { get; set; } = new List<ExchangeItemCalculation>();
    }

    /// <summary>
    /// Calculation result for each exchange item
    /// </summary>
    public class ExchangeItemCalculation
    {
        public int MetalId { get; set; }
        public int PurityId { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal PurityPercentage { get; set; }
        public decimal PureWeight { get; set; }
        public decimal CurrentRatePerGram { get; set; }
        public decimal MarketValue { get; set; }
        public decimal MakingChargeDeductionPercent { get; set; }
        public decimal WastageDeductionPercent { get; set; }
        public decimal TotalDeductionPercent { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }
}
