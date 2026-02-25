using InventoryManagementSytem.Common.Enums;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("jewellery_item")]
    public class JewelleryItemDb
    {
        
            [Key]
            [Column("id")]
            public long Id { get; set; }

            [Column("item_code")]
            public string? ItemCode { get; set; }   // Barcode / SKU

            [Required]
            [Column("name")]
            public string Name { get; set; }

            [Required]
            [Column("description")]
            public string Description { get; set; }

            // Category
            [Required]
            [Column("category_id")]
            public int CategoryId { get; set; }

            // Stone info
            [Column("has_stone")]
            public bool HasStone { get; set; }

            [Column("stone_id")]
            public int? StoneId { get; set; }   // NULL when HasStone = false

            // Metal info (SINGLE metal per item)
            [Required]
            [Column("metal_id")]
            public int MetalId { get; set; }

            [Required]
            [Column("purity_id")]
            public int PurityId { get; set; }

            // Weights
            [Required]
            [Column("gross_weight")]
            public decimal GrossWeight { get; set; }     // Metal + stone

            [Required]
            [Column("net_metal_weight")]
            public decimal NetMetalWeight { get; set; }  // ONLY metal

            // Making charges (RULE, not amount)
            [Required]
            [Column("making_charge_type")]
            public MakingChargeType MakingChargeType { get; set; }

            [Required]
            [Column("making_charge_value")]
            public decimal MakingChargeValue { get; set; }

            // Wastage (percentage)
            [Required]
            [Column("wastage_percentage")]
            public decimal WastagePercentage { get; set; }

            // Compliance
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

            // Audit
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

            // Status
            [Required]
            [Column("status_id")]
            public int StatusId { get; set; }

            // Navigation properties
            [ForeignKey(nameof(StatusId))]
            public virtual GenericStatusDb Status { get; set; }

            [ForeignKey(nameof(CreatedBy))]
            public virtual UserDb CreatedByUser { get; set; }

            [ForeignKey(nameof(UpdatedBy))]
            public virtual UserDb? UpdatedByUser { get; set; }

            [ForeignKey(nameof(CategoryId))]
            public virtual CategoryDb Category { get; set; }

            [ForeignKey(nameof(StoneId))]
            public virtual StoneDb? Stone { get; set; }

            [ForeignKey(nameof(MetalId))]
            public virtual MetalDb Metal { get; set; }

            [ForeignKey(nameof(PurityId))]
            public virtual PurityDb Purity { get; set; }
        }

    }
