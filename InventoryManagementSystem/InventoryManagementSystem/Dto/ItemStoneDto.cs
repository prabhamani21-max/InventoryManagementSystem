namespace InventoryManagementSystem.DTO
{
    public class ItemStoneDto
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int StoneId { get; set; }
        public decimal Quantity { get; set; }
        public decimal? Weight { get; set; }
        public int StatusId { get; set; }
    }
}