using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSytem.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("payment")]
    public class PaymentDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("order_id")]
        public int? OrderId { get; set; } // PurchaseOrderId or SaleOrderId
        [Required]
        [Column("order_type")]
        public TransactionType OrderType { get; set; } // 'PURCHASE' or 'SALE'
        [Column("customer_id")]
        public long? CustomerId { get; set; } // Customer who paid
        [Column("sales_person_id")]
        public long? SalesPersonId { get; set; } // Salesperson who processed the sale
        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }
        [Required]
        [Column("payment_method")]
        public PaymentMethod PaymentMethod { get; set; } // Cash, Card, Bank Transfer, etc.
        [Required]
        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }
        [Column("reference_number")]
        public string? ReferenceNumber { get; set; }
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
        [ForeignKey("CustomerId")]
        public virtual UserDb? Customer { get; set; }
        [ForeignKey(nameof(SalesPersonId))]
        public virtual UserDb? SalesPerson { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        public virtual ICollection<InvoicePaymentDb> InvoicePayments { get; set; }
           = new List<InvoicePaymentDb>();
    }
}