
namespace InventoryManagementSystem.Common.Models
{
    public class ItemStone
    {
        public int Id { get; set; }
        public long JewelleryItemId { get; set; }
        public int StoneId { get; set; }
        public decimal Quantity { get; set; } // Number of stones
        public decimal? Weight { get; set; } // Total weight in carats or grams
 
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
        
    }
}
