using InventoryManagementSytem.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("sale_order_item")]
    public class SaleOrderItemDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /* ---------------- PARENT ---------------- */

        [Required]
        [Column("sale_order_id")]
        public long SaleOrderId { get; set; }

        [Required]
        [Column("jewellery_item_id")]
        public long JewelleryItemId { get; set; }

        /* ---------------- ITEM SNAPSHOT ---------------- */

        [Column("item_code")]
        public string? ItemCode { get; set; }

        [Required]
        [Column("item_name")]
        public string ItemName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        /* ---------------- METAL SNAPSHOT ---------------- */

        [Required]
        [Column("metal_id")]
        public int MetalId { get; set; }

        [Required]
        [Column("purity_id")]
        public int PurityId { get; set; }

        [Required]
        [Column("gross_weight")]
        public decimal GrossWeight { get; set; }

        [Required]
        [Column("net_metal_weight")]
        public decimal NetMetalWeight { get; set; }

        [Required]
        [Column("metal_rate_per_gram")]
        public decimal MetalRatePerGram { get; set; }

        [Required]
        [Column("metal_amount")]
        public decimal MetalAmount { get; set; }

        /* ---------------- MAKING CHARGES ---------------- */

        [Required]
        [Column("making_charge_type")]
        public MakingChargeType MakingChargeType { get; set; }

        [Required]
        [Column("making_charge_value")]
        public decimal MakingChargeValue { get; set; }

        [Required]
        [Column("total_making_charges")]
        public decimal TotalMakingCharges { get; set; }

        [Required]
        [Column("wastage_percentage")]
        public decimal WastagePercentage { get; set; }

        [Required]
        [Column("wastage_weight")]
        public decimal WastageWeight { get; set; }

        [Required]
        [Column("wastage_amount")]
        public decimal WastageAmount { get; set; }

        /* ---------------- STONE SUMMARY ---------------- */

        [Required]
        [Column("has_stone")]
        public bool HasStone { get; set; }

        [Column("stone_amount")]
        public decimal? StoneAmount { get; set; }

        /* ---------------- PRICE & TAX ---------------- */

        [Required]
        [Column("item_subtotal")]
        public decimal ItemSubtotal { get; set; }

        [Required]
        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [Required]
        [Column("taxable_amount")]
        public decimal TaxableAmount { get; set; }

        [Required]
        [Column("gst_percentage")]
        public decimal GstPercentage { get; set; }

        [Required]
        [Column("gst_amount")]
        public decimal GstAmount { get; set; }

        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        /* ---------------- HALLMARK ---------------- */

        [Required]
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

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }
        [ForeignKey(nameof(SaleOrderId))]
        public virtual SaleOrderDb SaleOrder { get; set; }
        [ForeignKey(nameof(JewelleryItemId))]
        public virtual JewelleryItemDb JewelleryItem { get; set; }
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