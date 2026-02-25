using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class ExchangeItem
    {
        public int Id { get; set; }
        public long ExchangeOrderId { get; set; }
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
        public string? ItemDescription { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }

        // Navigation property
        public virtual ExchangeOrder? ExchangeOrder { get; set; }
    }
}
