using InventoryManagementSytem.Common.Enums;

namespace InventoryManagementSystem.DTO
{
    public class JewelleryItemDto
    {
        public long Id { get; set; }
        public string? ItemCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool HasStone { get; set; }
        public int? StoneId { get; set; }
        public MakingChargeType MakingChargeType { get; set; }

        public decimal MakingChargeValue  { get; set; }
        public decimal WastagePercentage { get; set; }
        public bool IsHallmarked { get; set; }
        public int StatusId { get; set; }

        // Hallmark Details
        public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID
        public string? BISCertificationNumber { get; set; }
        public string? HallmarkCenterName { get; set; }
        public DateTime? HallmarkDate { get; set; }

        // Metal Details - moved from Metal
        public int MetalId { get; set; }
        public string? MetalName { get; set; }
        public int PurityId { get; set; }
        public string? PurityName { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetMetalWeight { get; set; }

    }

    // public class JewelleryItemCreateDto
    // {
    //     public string? ItemCode { get; set; }
    //     public string Name { get; set; }
    //     public string Description { get; set; }
    //     public int CategoryId { get; set; }
    //     public bool HasStone { get; set; }
    //     public int StoneId { get; set; }
    //     public decimal MakingCharges { get; set; }
    //     public decimal Wastage { get; set; }
    //     public bool IsHallmarked { get; set; }

    //     // Metal Details
    //     public int MetalId { get; set; }
    //     public int PurityId { get; set; }
    //     public decimal GrossWeight { get; set; }
    //     public decimal NetWeight { get; set; }
    // }

    // public class JewelleryItemUpdateDto
    // {
    //     public long Id { get; set; }
    //     public string? ItemCode { get; set; }
    //     public string Name { get; set; }
    //     public string Description { get; set; }
    //     public int CategoryId { get; set; }
    //     public bool HasStone { get; set; }
    //     public int StoneId { get; set; }
    //     public decimal MakingCharges { get; set; }
    //     public decimal Wastage { get; set; }
    //     public bool IsHallmarked { get; set; }

    //     // Metal Details
    //     public int MetalId { get; set; }
    //     public int PurityId { get; set; }
    //     public decimal GrossWeight { get; set; }
    //     public decimal NetWeight { get; set; }
    // }

    // public class LockRateDto
    // {
    //     public long JewelleryItemId { get; set; }
    //     public decimal RatePerGram { get; set; }
    // }

    // public class JewelleryItemWithCalculationDto : JewelleryItemDto
    // {
    //     public decimal MetalValue { get; set; }
    //     public decimal WastageValue { get; set; }
    //     public decimal StoneValue { get; set; }
    //     public decimal TotalMakingCharges { get; set; }
    //     public decimal SubTotal { get; set; }
    // }
}
