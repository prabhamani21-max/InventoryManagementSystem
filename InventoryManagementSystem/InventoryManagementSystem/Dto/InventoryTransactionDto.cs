namespace InventoryManagementSystem.DTO
{
    public class InventoryTransactionDto
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int WarehouseId { get; set; }
        public string TransactionType { get; set; } // 'IN', 'OUT', 'ADJUST'
        public int Quantity { get; set; }
        public int? ReferenceId { get; set; } // PurchaseOrderId or SaleOrderId
        public string? ReferenceType { get; set; } // 'PURCHASE', 'SALE'
        public DateTime TransactionDate { get; set; }
        public int StatusId { get; set; }
    }
}