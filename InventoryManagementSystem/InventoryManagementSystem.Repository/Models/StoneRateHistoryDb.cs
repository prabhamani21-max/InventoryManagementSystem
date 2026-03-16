using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("stone_rate_history")]
    public class StoneRateHistoryDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("stone_id")]
        public int StoneId { get; set; }
        
        // Diamond 4Cs
        [Column("carat")]
        public decimal? Carat { get; set; } // Weight in carats
        
        [Column("cut")]
        public string? Cut { get; set; } // Excellent, Very Good, Good, Fair
        
        [Column("color")]
        public string? Color { get; set; } // D, E, F, G, H, I, J (for diamonds)
        
        [Column("clarity")]
        public string? Clarity { get; set; } // IF, VVS1, VVS2, VS1, VS2, SI1, SI2
        
        // For colored stones
        [Column("grade")]
        public string? Grade { get; set; } // AAA, AA, A, etc.
        
        [Required]
        [Column("rate_per_unit")]
        public decimal RatePerUnit { get; set; } // Rate per carat/gram
        
        [Required]
        [Column("effective_date")]
        public DateTime EffectiveDate { get; set; }
        
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        
        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }
        
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
        
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        
        [Column("status_id")]
        public int StatusId { get; set; }

        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        [ForeignKey(nameof(StoneId))]
        public virtual StoneDb Stone { get; set; }
    }
}
