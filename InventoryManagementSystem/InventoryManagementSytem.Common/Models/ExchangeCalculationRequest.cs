using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    /// <summary>
    /// Request model for exchange value calculation
    /// </summary>
    public class ExchangeCalculationRequest
    {
        public long CustomerId { get; set; }
        public int ExchangeType { get; set; } // 1 = Exchange, 2 = Buyback
        public List<ExchangeItemInput> Items { get; set; } = new List<ExchangeItemInput>();
        public decimal? NewPurchaseAmount { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Input model for each item in exchange calculation
    /// </summary>
    public class ExchangeItemInput
    {
        public int MetalId { get; set; }
        public int PurityId { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal MakingChargeDeductionPercent { get; set; }
        public decimal WastageDeductionPercent { get; set; }
        public string? ItemDescription { get; set; }
    }
}
