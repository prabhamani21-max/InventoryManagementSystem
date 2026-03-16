using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("item_stone")]
    public class ItemStoneDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("item_id")]
        public long JewelleryItemId { get; set; }  //// JewelleryItemId
        [Column("stone_id")]
        public int StoneId { get; set; }
        [Required]
        [Column("quantity")]
        public decimal Quantity { get; set; } // Number of stones
        [Required]
        [Column("weight")]
        public decimal? Weight { get; set; } // Total weight in carats or grams
   
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
        [Column("status_id")]
        public int StatusId { get; set; }

        [ForeignKey(nameof(JewelleryItemId))]
        public virtual JewelleryItemDb Jewellery { get; set; }
        [ForeignKey(nameof(StoneId))]

        public virtual StoneDb Stone { get; set; }

        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}
