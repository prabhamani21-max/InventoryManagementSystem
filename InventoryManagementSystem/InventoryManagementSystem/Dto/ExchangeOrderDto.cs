namespace InventoryManagementSystem.DTO
{
    // Exchange Type enum
    public enum ExchangeType
    {
        EXCHANGE = 1,  // Exchange for new purchase
        BUYBACK = 2    // Direct cash buyback
    }

    // Request DTO for calculating exchange value
    public class ExchangeCalculateRequestDto
    {
        public long CustomerId { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public List<ExchangeItemInputDto> Items { get; set; }
        public decimal? NewPurchaseAmount { get; set; } // For EXCHANGE type
        public string? Notes { get; set; }
    }

    // Input DTO for each item (with variable deductions)
    public class ExchangeItemInputDto
    {
        public int MetalId { get; set; }
        public int PurityId { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal MakingChargeDeductionPercent { get; set; } // Variable at POS
        public decimal WastageDeductionPercent { get; set; } // Variable at POS
        public string? ItemDescription { get; set; }
    }

    // Response DTO with calculated values
    public class ExchangeCalculateResponseDto
    {
        public long CustomerId { get; set; }
        public string ExchangeType { get; set; }
        public int ItemCount { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalPureWeight { get; set; }
        public decimal TotalMarketValue { get; set; }
        public decimal TotalDeductionAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal? NewPurchaseAmount { get; set; }
        public decimal? BalanceRefund { get; set; }
        public decimal? CashPayment { get; set; }
        public List<ExchangeItemResponseDto> ItemDetails { get; set; }
    }

    // Response DTO for each item
    public class ExchangeItemResponseDto
    {
        public int MetalId { get; set; }
        public string? MetalName { get; set; }
        public int PurityId { get; set; }
        public string? PurityName { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal PurityPercentage { get; set; }
        public decimal PureWeight { get; set; }
        public decimal CurrentRatePerGram { get; set; }
        public decimal MarketValue { get; set; }
        public decimal MakingChargeDeductionPercent { get; set; }
        public decimal WastageDeductionPercent { get; set; }
        public decimal TotalDeductionPercent { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal CreditAmount { get; set; }
    }

    // Full Exchange Order DTO
    public class ExchangeOrderDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string ExchangeType { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalNetWeight { get; set; }
        public decimal TotalPureWeight { get; set; }
        public decimal TotalMarketValue { get; set; }
        public decimal TotalDeductionAmount { get; set; }
        public decimal TotalCreditAmount { get; set; }
        public decimal? NewPurchaseAmount { get; set; }
        public decimal? BalanceRefund { get; set; }
        public decimal? CashPayment { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? Notes { get; set; }
        public DateTime ExchangeDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<ExchangeItemDto> Items { get; set; }
    }

    // Exchange Item DTO
    public class ExchangeItemDto
    {
        public int Id { get; set; }
        public long ExchangeOrderId { get; set; }
        public int MetalId { get; set; }
        public string? MetalName { get; set; }
        public int PurityId { get; set; }
        public string? PurityName { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal PurityPercentage { get; set; }
        public decimal PureWeight { get; set; }
        public decimal CurrentRatePerGram { get; set; }
        public decimal MarketValue { get; set; }
        public decimal MakingChargeDeductionPercent { get; set; }
        public decimal WastageDeductionPercent { get; set; }
        public decimal TotalDeductionPercent { get; set; }
        public decimal DeductionAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public string? ItemDescription { get; set; }
        public int StatusId { get; set; }
    }

    // Create Exchange Order Request
    public class ExchangeOrderCreateDto
    {
        public long CustomerId { get; set; }
        public ExchangeType ExchangeType { get; set; }
        public List<ExchangeItemInputDto> Items { get; set; }
        public decimal? NewPurchaseAmount { get; set; }
        public string? Notes { get; set; }
    }

    // Complete Exchange Request
    public class ExchangeCompleteDto
    {
        public long ExchangeOrderId { get; set; }
        public string? Notes { get; set; }
    }
}
