using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class ExchangeOrder
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long CustomerId { get; set; }
        public int ExchangeType { get; set; } // 1 = Exchange, 2 = Buyback
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalPureWeight { get; set; }
        public decimal TotalMarketValue { get; set; }
        public decimal TotalDeductionAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime ExchangeDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
        public long? LinkedSaleOrderId { get; set; }
        public string? LinkedSaleOrderNumber { get; set; }
        public long? LinkedInvoiceId { get; set; }
        public string? LinkedInvoiceNumber { get; set; }
        public decimal? LinkedSaleGrandTotal { get; set; }
        public decimal? RemainingCustomerPayment { get; set; }

        // Navigation property
        public virtual ICollection<ExchangeItem> Items { get; set; }
    }
}
