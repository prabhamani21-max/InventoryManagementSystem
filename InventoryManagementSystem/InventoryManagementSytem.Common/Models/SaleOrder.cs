using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class SaleOrder
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        /* ---------------- EXCHANGE AWARENESS (MINIMAL) ---------- */

        /// <summary>
        /// true = sold as part of exchange transaction
        /// </summary>
        [Required]
        public bool IsExchangeSale { get; set; }

        /// <summary>
        /// Foreign key to ExchangeOrder when this sale is part of an exchange transaction
        /// </summary>
        public long? ExchangeOrderId { get; set; }

        public DateTime? DeliveryDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}