using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class Stone
    {
        public int Id { get; set; }
        public string Name { get; set; } // Diamond, Emerald, Pearl, Stone etc
        public string? Unit { get; set; } // Carat, Gram, Etc
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }

    }
}
