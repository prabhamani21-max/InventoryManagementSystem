using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Common.Models
{
    public class UserKyc
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string? PanCardNumber { get; set; }
        public string? AadhaarCardNumber { get; set; }
        public bool IsVerified { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
 
    }
}