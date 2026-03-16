using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Common.Models
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public long? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
    }
}