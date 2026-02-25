
namespace InventoryManagementSystem.Common.Models
{
    public class ItemStock
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }

    }
}