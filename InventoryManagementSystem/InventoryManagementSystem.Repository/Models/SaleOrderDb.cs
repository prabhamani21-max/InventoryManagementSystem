using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("sale_order")]
    public class SaleOrderDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("customer_id")]
        public long CustomerId { get; set; }
        /* ---------------- EXCHANGE AWARENESS (MINIMAL) ---------- */

        /// <summary>
        /// true = sold as part of exchange transaction
        /// </summary>
        [Required]
        [Column("is_exchange_sale")]
        public bool IsExchangeSale { get; set; }

        /// <summary>
        /// Foreign key to ExchangeOrderDb when this sale is part of an exchange transaction
        /// </summary>
        [Column("exchange_order_id")]
        public long? ExchangeOrderId { get; set; }

        [ForeignKey(nameof(ExchangeOrderId))]
        public virtual ExchangeOrderDb? ExchangeOrder { get; set; }

        /* ---------------- AUDIT ---------------- */
        [Required]
        [Column("order_number")]
        public string OrderNumber { get; set; }
        [Required]
        [Column("order_date")]
        public DateTime OrderDate { get; set; }
        [Column("delivery_date")]
        public DateTime? DeliveryDate { get; set; }
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
        [ForeignKey(nameof(CustomerId))]
        public virtual UserDb Customer { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}