using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.DTO
{
    public class MetalRateDto
    {
        public int Id { get; set; }
        public int PurityId { get; set; }
        public string? PurityName { get; set; }
        public int MetalId { get; set; }
        public string? MetalName { get; set; }
        public decimal RatePerGram { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusId { get; set; }
    }

    public class MetalRateCreateDto
    {
        [Required]
        public int PurityId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        public decimal RatePerGram { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }

    public class MetalRateUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        public decimal RatePerGram { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }

    public class MetalRateResponseDto
    {
        public int PurityId { get; set; }
        public string PurityName { get; set; } = string.Empty;
        public int MetalId { get; set; }
        public string MetalName { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public decimal CurrentRatePerGram { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class MetalRateHistoryDto
    {
        public int Id { get; set; }
        public int PurityId { get; set; }
        public string PurityName { get; set; } = string.Empty;
        public decimal RatePerGram { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
