using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("invoice_payment")]
    public class InvoicePaymentDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("invoice_id")]
        public long InvoiceId { get; set; }

        [Required]
        [Column("payment_id")]
        public long PaymentId { get; set; }

        [Required]
        [Column("allocated_amount")]
        public decimal AllocatedAmount { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }

        // Navigation
        [ForeignKey(nameof(InvoiceId))]
        public virtual InvoiceDb Invoice { get; set; } = null!;

        [ForeignKey(nameof(PaymentId))]
        public virtual PaymentDb Payment { get; set; } = null!;
    }
}
