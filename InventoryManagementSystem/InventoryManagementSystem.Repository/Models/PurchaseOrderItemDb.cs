using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("PurchaseOrderItemDB")]
    public class PurchaseOrderItemDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("purchase_order_id")]
        public long PurchaseOrderId { get; set; }
        [Column("jewellery_item_id")]
        public long JewelleryItemId { get; set; }
        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }
        [Required]
        [Column("unit_price")]
        public decimal UnitPrice { get; set; }
        [Required]
        [Column("total_price")]
        public decimal TotalPrice { get; set; }
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
        [ForeignKey(nameof(PurchaseOrderId))]
        public virtual PurchaseOrderDb PurchaseOrder { get; set; }
        [ForeignKey(nameof(JewelleryItemId))]
        public virtual JewelleryItemDb JewelleryItem { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}