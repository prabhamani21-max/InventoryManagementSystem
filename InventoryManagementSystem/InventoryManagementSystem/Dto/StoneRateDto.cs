using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.DTO
{
    public class StoneRateDto
    {
        public int Id { get; set; }
        public int StoneId { get; set; }
        public string? StoneName { get; set; }
        public string? StoneUnit { get; set; }
        public decimal Carat { get; set; }
        public string? Cut { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Grade { get; set; }
        public decimal RatePerUnit { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusId { get; set; }
    }

    public class StoneRateCreateDto
    {
        /// <example>1</example>
        [Required]
        public int StoneId { get; set; }

        /// <example>1.00</example>
        [Range(0.01, 10, ErrorMessage = "Carat must be between 0.01 and 10")]
        public decimal Carat { get; set; }

        /// <example>Excellent</example>
        [MaxLength(50)]
        public string? Cut { get; set; } // Excellent, Very Good, Good, Fair

        /// <example>E</example>
        [MaxLength(10)]
        public string? Color { get; set; } // D, E, F, G, H, I, J

        /// <example>VVS2</example>
        [MaxLength(10)]
        public string? Clarity { get; set; } // IF, VVS1, VVS2, VS1, VS2, SI1, SI2

        /// <example>Lab-Grown</example>
        [MaxLength(20)]
        public string? Grade { get; set; } // AAA, AA, A (for colored stones)

        /// <example>32000</example>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        public decimal RatePerUnit { get; set; }

        /// <example>2026-02-08T00:00:00Z</example>
        [Required]
        public DateTime EffectiveDate { get; set; }
    }

    public class StoneRateUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Range(0.01, 10, ErrorMessage = "Carat must be between 0.01 and 10")]
        public decimal Carat { get; set; }

        [MaxLength(50)]
        public string? Cut { get; set; }

        [MaxLength(10)]
        public string? Color { get; set; }

        [MaxLength(10)]
        public string? Clarity { get; set; }

        [MaxLength(20)]
        public string? Grade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
        public decimal RatePerUnit { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }

    public class StoneRateResponseDto
    {
        public int StoneId { get; set; }
        public string StoneName { get; set; } = string.Empty;
        public string StoneUnit { get; set; } = string.Empty; // Carat, Gram, etc.
        
        // 4Cs or Grade
        public decimal Carat { get; set; }
        public string? Cut { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Grade { get; set; }
        
        public decimal CurrentRatePerUnit { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class Diamond4CsRateDto
    {
        public decimal Carat { get; set; }
        public string Cut { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Clarity { get; set; } = string.Empty;
        public decimal RatePerCarat { get; set; }
    }

    public class StoneRateSearchDto
    {
        public int? StoneId { get; set; }
        public decimal? Carat { get; set; }
        public string? Cut { get; set; }
        public string? Color { get; set; }
        public string? Clarity { get; set; }
        public string? Grade { get; set; }
    }
}
