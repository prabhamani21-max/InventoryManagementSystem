
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InventoryManagementSystem.Repository.Models
{
    [Table("inventory_transaction")]
    public class InventoryTransactionDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("jewellery_item_id")]
        public long JewelleryItemId { get; set; }
        [Column("warehouse_id")]
        public int WarehouseId { get; set; }
        [Required]
        [Column("transaction_type")]
        public string TransactionType { get; set; } // 'IN', 'OUT', 'ADJUST'
        [Required]
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("reference_id")]
        public int? ReferenceId { get; set; } // PurchaseOrderId or SaleOrderId
        [Column("reference_type")]
        public string? ReferenceType { get; set; } // 'PURCHASE', 'SALE'
        [Required]
        [Column("transaction_date")]
        public DateTime TransactionDate { get; set; }
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
        public virtual JewelleryItemDb JewelleryItem { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        public virtual WarehouseDb Warehouse { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}