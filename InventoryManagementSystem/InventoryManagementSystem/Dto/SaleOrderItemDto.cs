using InventoryManagementSytem.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.DTO
{
    public class SaleOrderItemDto
    {
        public long Id { get; set; }

        /* ---------------- PARENT ---------------- */

        public long SaleOrderId { get; set; }

        public string? SaleOrderNumber { get; set; }

        public long JewelleryItemId { get; set; }

        /* ---------------- ITEM SNAPSHOT ---------------- */

        public string? ItemCode { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int Quantity { get; set; } = 1;

        /* ---------------- METAL SNAPSHOT ---------------- */

        public int MetalId { get; set; }

        public int PurityId { get; set; }

        public decimal GrossWeight { get; set; }

        public decimal NetMetalWeight { get; set; }

        public decimal MetalRatePerGram { get; set; }

        public decimal MetalAmount { get; set; }

        /* ---------------- MAKING CHARGES ---------------- */

        public MakingChargeType MakingChargeType { get; set; }

        public decimal MakingChargeValue { get; set; }

        public decimal TotalMakingCharges { get; set; }

        public decimal WastagePercentage { get; set; }

        public decimal WastageWeight { get; set; }

        public decimal WastageAmount { get; set; }

        /* ---------------- STONE SUMMARY ---------------- */

        public bool HasStone { get; set; }

        public decimal? StoneAmount { get; set; }

        /* ---------------- PRICE & TAX ---------------- */

        public decimal ItemSubtotal { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TaxableAmount { get; set; }

        public decimal GstPercentage { get; set; }

        public decimal GstAmount { get; set; }

        public decimal TotalAmount { get; set; }

        /* ---------------- HALLMARK ---------------- */

        public bool IsHallmarked { get; set; }

        public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID

        public string? BISCertificationNumber { get; set; }

        public string? HallmarkCenterName { get; set; }

        public DateTime? HallmarkDate { get; set; }

        /* ---------------- AUDIT ---------------- */

        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public int StatusId { get; set; }
    }
    public class CreateSaleOrderItemDto
    {
        /* ---------------- REQUIRED INPUTS ---------------- */

        [Required]
        public long SaleOrderId { get; set; }

        [Required]
        public long JewelleryItemId { get; set; }

        /* ---------------- OPTIONAL OVERRIDES ---------------- */

        /// <summary>
        /// Optional discount amount override. If null, defaults to 0.
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// GST percentage. Depends on product type.
        /// </summary>
        [Range(0, 100)]
        public decimal GstPercentage { get; set; }

        /// <summary>
        /// Stone amount override. If null, will be calculated from ItemStones.
        /// </summary>
        public decimal? StoneAmount { get; set; }

        /* ---------------- QUANTITY ---------------- */

        /// <summary>
        /// Quantity of items (default: 1)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;

        /* ---------------- EXCHANGE AWARENESS (MINIMAL) ---------- */

        /// <summary>
        /// true = sold as part of exchange transaction
        /// </summary>
        public bool IsExchangeSale { get; set; }
    }
}
