using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.Common.Models
{
    public class JewelleryItem
    {
        public long Id { get; set; }
        public string? ItemCode { get; set; } // Barcode/SKU
        public string Name { get; set; } /// LAdies Ring Model A
        public string Description { get; set; }
        public int CategoryId { get; set; }  //Ring,Chain etc
        public bool HasStone { get; set; }
        public int? StoneId { get; set; }
        public int MetalId { get; set; }
        public int PurityId { get; set; }
        public decimal GrossWeight { get; set; } 
        public decimal NetMetalWeight { get; set; }
        public MakingChargeType MakingChargeType { get; set; }
        public decimal MakingChargeValue { get; set; }
        public decimal WastagePercentage { get; set; }
        public bool IsHallmarked { get; set; }
        
        // Hallmark Details
        public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID
        public string? BISCertificationNumber { get; set; }
        public string? HallmarkCenterName { get; set; }
        public DateTime? HallmarkDate { get; set; }
        
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public int StatusId { get; set; }
     
    }
}
