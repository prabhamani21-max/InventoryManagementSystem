using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("purchase_order")]
    public class PurchaseOrderDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("supplier_id")]
        public int SupplierId { get; set; }
        [Required]
        [Column("order_number")]
        public string OrderNumber { get; set; }
        [Required]
        [Column("order_date")]
        public DateTime OrderDate { get; set; }
        [Column("expected_delivery_date")]
        public DateTime? ExpectedDeliveryDate { get; set; }
        [Required]
        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
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
        [ForeignKey(nameof(SupplierId))]
        public virtual SupplierDb Supplier { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}