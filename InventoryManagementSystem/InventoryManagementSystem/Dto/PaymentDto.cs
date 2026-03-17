using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.DTO
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OrderType { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public long? SalesPersonId { get; set; }
        public string? SalesPersonName { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public int StatusId { get; set; }
        
        /// <summary>
        /// The total order amount for validation purposes.
        /// Used for high-value transaction validation.
        /// </summary>
        public decimal? OrderTotal { get; set; }
    }
}
