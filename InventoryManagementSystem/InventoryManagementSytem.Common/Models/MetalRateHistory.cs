
namespace InventoryManagementSystem.Common.Models
{
    public class MetalRateHistory
    {
        public int Id { get; set; }
        public int PurityId { get; set; }
        public decimal RatePerGram { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}
