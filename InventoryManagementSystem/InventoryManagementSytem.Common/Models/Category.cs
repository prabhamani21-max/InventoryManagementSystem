using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Common.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? ParentId { get; set; }

        public int  StatusId { get; set; } 

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public long CreatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public long? UpdatedBy { get; set; }

        // Navigation property for subcategories
        public ICollection<Category>? SubCategories { get; set; }
    }
}
