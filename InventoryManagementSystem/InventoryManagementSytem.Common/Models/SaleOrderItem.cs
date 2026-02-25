using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.Common.Models
{
    public class SaleOrderItem
    {
        public long Id { get; set; }

        /* ---------------- PARENT ---------------- */

        public long SaleOrderId { get; set; }

        public string? SaleOrderNumber { get; set; }

        public long JewelleryItemId { get; set; }

        /* ---------------- ITEM SNAPSHOT ---------------- */

        public string? ItemCode { get; set; }

        [Required]
        public string ItemName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        /* ---------------- METAL SNAPSHOT ---------------- */

        [Required]
        public int MetalId { get; set; }

        [Required]
        public int PurityId { get; set; }

        [Required]
        public decimal GrossWeight { get; set; }

        [Required]
        public decimal NetMetalWeight { get; set; }

        [Required]
        public decimal MetalRatePerGram { get; set; }

        [Required]
        public decimal MetalAmount { get; set; }

        /* ---------------- MAKING CHARGES ---------------- */

        [Required]
        public MakingChargeType MakingChargeType { get; set; }

        [Required]
        public decimal MakingChargeValue { get; set; }

        [Required]
        public decimal TotalMakingCharges { get; set; }

        [Required]
        public decimal WastagePercentage { get; set; }

        [Required]
        public decimal WastageWeight { get; set; }

        [Required]
        public decimal WastageAmount { get; set; }

        /* ---------------- STONE SUMMARY ---------------- */

        [Required]
        public bool HasStone { get; set; }

        public decimal? StoneAmount { get; set; }

        /* ---------------- PRICE & TAX ---------------- */

        [Required]
        public decimal ItemSubtotal { get; set; }

        [Required]
        public decimal DiscountAmount { get; set; }

        [Required]
        public decimal TaxableAmount { get; set; }

        [Required]
        public decimal GstPercentage { get; set; }

        [Required]
        public decimal GstAmount { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        /* ---------------- HALLMARK ---------------- */

        [Required]
        public bool IsHallmarked { get; set; }

        public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID

        public string? BISCertificationNumber { get; set; }

        public string? HallmarkCenterName { get; set; }

        public DateTime? HallmarkDate { get; set; }

        /* ---------------- AUDIT ---------------- */

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        [Required]
        public int StatusId { get; set; }
    }
}
