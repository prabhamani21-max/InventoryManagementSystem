using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class Purity
    {
        public int Id { get; set; }
        public int MetalId { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}
