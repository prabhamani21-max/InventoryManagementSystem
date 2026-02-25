using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("exchange_order")]
    public class ExchangeOrderDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("order_number")]
        public string OrderNumber { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }

        [Column("exchange_type")]
        public string ExchangeType { get; set; } // "EXCHANGE" or "BUYBACK"

        [Column("total_gross_weight")]
        public decimal TotalGrossWeight { get; set; }

        [Column("total_net_weight")]
        public decimal TotalNetWeight { get; set; }

        [Column("total_pure_weight")]
        public decimal TotalPureWeight { get; set; }

        [Column("total_market_value")]
        public decimal TotalMarketValue { get; set; }

        [Column("total_deduction_amount")]
        public decimal TotalDeductionAmount { get; set; }

        [Column("total_credit_amount")]
        public decimal TotalCreditAmount { get; set; }

        [Column("new_purchase_amount")]
        public decimal? NewPurchaseAmount { get; set; }

        [Column("balance_refund")]
        public decimal? BalanceRefund { get; set; }

        [Column("cash_payment")]
        public decimal? CashPayment { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("exchange_date")]
        public DateTime ExchangeDate { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public virtual UserDb Customer { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        public virtual ICollection<ExchangeItemDb> ExchangeItems { get; set; }
    }
}
