using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("invoice_item")]
    public class InvoiceItemDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("invoice_id")]
        public long InvoiceId { get; set; }

        [Column("reference_item_id")]
        public long? ReferenceItemId { get; set; } // SaleOrderItemId / PurchaseOrderItemId

        [Required]
        [Column("item_name")]
        public string ItemName { get; set; } = string.Empty;

        [Column("quantity")]
        public int Quantity { get; set; }

        /* ---------- METAL SNAPSHOT ---------- */

        [Column("metal_id")]
        public int MetalId { get; set; }

        [Column("purity_id")]
        public int PurityId { get; set; }

        [Column("net_metal_weight")]
        public decimal? NetMetalWeight { get; set; }

        [Column("metal_amount")]
        public decimal? MetalAmount { get; set; }

        /* ---------- STONE SNAPSHOT (SNAPSHOT at billing time - never depend on JewelleryItem later) ---------- */
        [Column("stone_id")]
        public int? StoneId { get; set; }
        
        [Column("stone_weight")]
        public decimal? StoneWeight { get; set; }
        
        [Column("stone_rate")]
        public decimal? StoneRate { get; set; }

        [Column("stone_amount")]
        public decimal? StoneAmount { get; set; }

        /* ---------- CHARGES ---------- */

        [Column("making_charges")]
        public decimal? MakingCharges { get; set; }

        [Column("wastage_amount")]
        public decimal? WastageAmount { get; set; }

        /* ---------- PRICING ---------- */

        [Column("item_subtotal")]
        public decimal ItemSubtotal { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("taxable_amount")]
        public decimal TaxableAmount { get; set; }

        /* ---------- GST BREAKDOWN (derived from SaleOrderItem - NOT recomputed independently) ---------- */

        [Column("cgst_amount")]
        public decimal CGSTAmount { get; set; }

        [Column("sgst_amount")]
        public decimal SGSTAmount { get; set; }

        [Column("igst_amount")]
        public decimal IGSTAmount { get; set; }

        [Column("gst_amount")]
        public decimal GSTAmount { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        /* ---------- HALLMARK SNAPSHOT ---------- */

        [Column("is_hallmarked")]
        public bool IsHallmarked { get; set; }

        [Column("huid")]
        [StringLength(6)]
        public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID

        [Column("bis_certification_number")]
        [StringLength(50)]
        public string? BISCertificationNumber { get; set; }

        [Column("hallmark_center_name")]
        [StringLength(100)]
        public string? HallmarkCenterName { get; set; }

        [Column("hallmark_date")]
        public DateTime? HallmarkDate { get; set; }

        [ForeignKey(nameof(InvoiceId))]
        public virtual InvoiceDb Invoice { get; set; }
        
        [ForeignKey(nameof(PurityId))]
        public virtual PurityDb Purity { get; set; }
        
        [ForeignKey(nameof(MetalId))]
        public virtual MetalDb Metal { get; set; } = null!;
    }
}
