using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("exchange_item")]
    public class ExchangeItemDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("exchange_order_id")]
        public long ExchangeOrderId { get; set; }

        [Column("metal_id")]
        public int MetalId { get; set; }

        [Column("purity_id")]
        public int PurityId { get; set; }

        [Column("gross_weight")]
        public decimal GrossWeight { get; set; }

        [Column("net_weight")]
        public decimal NetWeight { get; set; }

        [Column("purity_percentage")]
        public decimal PurityPercentage { get; set; }

        [Column("pure_weight")]
        public decimal PureWeight { get; set; }

        [Column("current_rate_per_gram")]
        public decimal CurrentRatePerGram { get; set; }

        [Column("market_value")]
        public decimal MarketValue { get; set; }

        [Column("making_charge_deduction_percent")]
        public decimal MakingChargeDeductionPercent { get; set; }

        [Column("wastage_deduction_percent")]
        public decimal WastageDeductionPercent { get; set; }

        [Column("total_deduction_percent")]
        public decimal TotalDeductionPercent { get; set; }

        [Column("deduction_amount")]
        public decimal DeductionAmount { get; set; }

        [Column("credit_amount")]
        public decimal CreditAmount { get; set; }

        [Column("item_description")]
        public string? ItemDescription { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column ("updated_by")]
        public long? UpdatedBy { get; set; }


        // Navigation properties
        [ForeignKey(nameof(ExchangeOrderId))]
        public virtual ExchangeOrderDb ExchangeOrder { get; set; }
        [ForeignKey(nameof(MetalId))]
        public virtual MetalDb Metal { get; set; }
        [ForeignKey(nameof(PurityId))]
        public virtual PurityDb Purity { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}
