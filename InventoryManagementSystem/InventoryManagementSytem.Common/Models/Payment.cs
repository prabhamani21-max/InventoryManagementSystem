using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public long OrderId { get; set; } // PurchaseOrderId or SaleOrderId
        public string OrderType { get; set; } // 'PURCHASE' or 'SALE'
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } // Cash, Card, Bank Transfer, etc.
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }

    }
}