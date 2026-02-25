using OpenTelemetry.Trace;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("category")]
    public class CategoryDb
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; } = string.Empty; // Ring, Necklace, Earrings, etc.

        [Column("description")]
        public string? Description { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; } // For subcategories

        [Column("status_id")]
        public int StatusId { get; set; } 

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public long CreatedBy { get; set; }

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [Column("updated_by")]
        public long? UpdatedBy { get; set; }

        // Navigation property for subcategories
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        [ForeignKey(nameof(ParentId))]
        public virtual CategoryDb? ParentCategory { get; set; }
        public virtual ICollection<CategoryDb>? SubCategories { get; set; }
    }
}
