using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("metal_rate_history")]
    public class MetalRateHistoryDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("purity_id")]
        public int PurityId { get; set; }
        [Required]
        [Column("rate_per_gram")]
        public decimal RatePerGram { get; set; }
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
        [ForeignKey(nameof(PurityId))]
        public virtual PurityDb Purity { get; set; }
    }
}
