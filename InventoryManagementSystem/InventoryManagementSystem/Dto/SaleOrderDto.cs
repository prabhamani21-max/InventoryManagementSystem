namespace InventoryManagementSystem.DTO
{
    public class SaleOrderDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public string? OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        /* ---------------- EXCHANGE AWARENESS (MINIMAL) ---------- */

        /// <summary>
        /// true = sold as part of exchange transaction
        /// </summary>
        public bool IsExchangeSale { get; set; }

        /// <summary>
        /// Foreign key to ExchangeOrder when this sale is part of an exchange transaction
        /// </summary>
        public long? ExchangeOrderId { get; set; }

        /// <summary>
        /// Exchange order number when this sale is linked to an exchange
        /// </summary>
        public string? ExchangeOrderNumber { get; set; }

        public int StatusId { get; set; }
    }
}