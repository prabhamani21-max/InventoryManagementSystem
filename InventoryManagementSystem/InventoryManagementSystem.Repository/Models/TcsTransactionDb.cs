using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    /// <summary>
    /// TCS (Tax Collected at Source) Transaction Database Model
    /// Stores TCS collection details for sales exceeding ₹10 lakh threshold
    /// Applicable for B2C customers only
    /// </summary>
    [Table("tcs_transactions")]
    public class TcsTransactionDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("invoice_id")]
        public long InvoiceId { get; set; }

        [Column("customer_id")]
        public long CustomerId { get; set; }

        /// <summary>
        /// Financial year in format "2024-25"
        /// Indian FY: April 1 to March 31
        /// </summary>
        [Column("financial_year")]
        [MaxLength(10)]
        public string FinancialYear { get; set; } = string.Empty;

        /// <summary>
        /// Customer PAN number (if available)
        /// With PAN: TCS rate 0.1%
        /// Without PAN: TCS rate 1%
        /// </summary>
        [Column("pan_number")]
        [MaxLength(10)]
        public string? PanNumber { get; set; }

        /// <summary>
        /// Sale amount for this transaction
        /// </summary>
        [Column("sale_amount")]
        public decimal SaleAmount { get; set; }

        /// <summary>
        /// Cumulative sales for the customer in this financial year
        /// including this transaction
        /// </summary>
        [Column("cumulative_sale_amount")]
        public decimal CumulativeSaleAmount { get; set; }

        /// <summary>
        /// TCS rate applied (0.001 for 0.1% or 0.01 for 1%)
        /// </summary>
        [Column("tcs_rate")]
        public decimal TcsRate { get; set; }

        /// <summary>
        /// TCS amount collected
        /// </summary>
        [Column("tcs_amount")]
        public decimal TcsAmount { get; set; }

        /// <summary>
        /// Type of TCS: "WithPAN", "WithoutPAN", "BelowThreshold", "Exempted"
        /// </summary>
        [Column("tcs_type")]
        [MaxLength(20)]
        public string TcsType { get; set; } = string.Empty;

        /// <summary>
        /// Whether TCS is exempted for this transaction
        /// </summary>
        [Column("is_exempted")]
        public bool IsExempted { get; set; }

        /// <summary>
        /// Reason for exemption if applicable
        /// </summary>
        [Column("exemption_reason")]
        [MaxLength(255)]
        public string? ExemptionReason { get; set; }

        /// <summary>
        /// Date of the transaction
        /// </summary>
        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// Quarter of the financial year (1-4)
        /// Q1: Apr-Jun, Q2: Jul-Sep, Q3: Oct-Dec, Q4: Jan-Mar
        /// </summary>
        [Column("quarter")]
        public int Quarter { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        [ForeignKey(nameof(InvoiceId))]
        public virtual InvoiceDb Invoice { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        public virtual UserDb Customer { get; set; } = null!;

        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedUser { get; set; } = null!;

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}
