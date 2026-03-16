
namespace InventoryManagementSystem.Common.Models
{
    public class InventoryTransaction
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int WarehouseId { get; set; }
        public string TransactionType { get; set; } // 'IN', 'OUT', 'ADJUST'
        public int Quantity { get; set; }
        public long? ReferenceId { get; set; } // PurchaseOrderId or SaleOrderId
        public string? ReferenceType { get; set; } // 'PURCHASE', 'SALE'
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
       

    }
}