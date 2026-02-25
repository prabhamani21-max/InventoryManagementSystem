namespace InventoryManagementSystem.DTO
{
    public class ItemStockDto
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int StatusId { get; set; }
    }
}