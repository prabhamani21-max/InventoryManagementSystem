
namespace InventoryManagementSystem.Common.Models
{
    public class StoneRateHistory
    {
        public int Id { get; set; }
        public int StoneId { get; set; }
        public Stone? Stone { get; set; } // Navigation property for Stone
        public decimal Carat { get; set; } // For diamonds - carat weight
        public string? Cut { get; set; } // For diamonds - Excellent, Very Good, Good, Fair
        public string? Color { get; set; } // For diamonds - D, E, F, G, H, etc.
        public string? Clarity { get; set; } // For diamonds - IF, VVS1, VVS2, VS1, VS2, etc.
        public string? Grade { get; set; } // For colored stones - AAA, AA, A, etc.
        public decimal RatePerUnit { get; set; } // Rate per carat/gram
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}
