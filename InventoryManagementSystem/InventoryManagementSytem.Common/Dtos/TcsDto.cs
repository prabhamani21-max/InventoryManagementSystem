using System;
using System.Collections.Generic;

namespace InventoryManagementSytem.Common.Dtos
{
    /// <summary>
    /// Request DTO for TCS calculation
    /// </summary>
    public class TcsCalculationRequestDto
    {
        /// <summary>
        /// Customer ID (B2C customer)
        /// </summary>
        public long CustomerId { get; set; }

        /// <summary>
        /// Sale amount for the transaction
        /// </summary>
        public decimal SaleAmount { get; set; }

        /// <summary>
        /// Date of the transaction
        /// </summary>
        public DateTime TransactionDate { get; set; }
    }

    /// <summary>
    /// Response DTO for TCS calculation
    /// </summary>
    public class TcsCalculationResponseDto
    {
        /// <summary>
        /// Whether TCS is applicable for this transaction
        /// </summary>
        public bool IsTcsApplicable { get; set; }

        /// <summary>
        /// TCS rate (0.001 for 0.1% or 0.01 for 1%)
        /// </summary>
        public decimal TcsRate { get; set; }

        /// <summary>
        /// TCS amount to be collected
        /// </summary>
        public decimal TcsAmount { get; set; }

        /// <summary>
        /// Type: "WithPAN", "WithoutPAN", "BelowThreshold", "Exempted"
        /// </summary>
        public string TcsType { get; set; } = string.Empty;

        /// <summary>
        /// Cumulative sales for customer in this financial year (before this transaction)
        /// </summary>
        public decimal CumulativeSaleAmount { get; set; }

        /// <summary>
        /// Threshold limit (₹10,00,000)
        /// </summary>
        public decimal ThresholdLimit { get; set; } = 1000000m;

        /// <summary>
        /// Whether customer has valid PAN
        /// </summary>
        public bool HasValidPAN { get; set; }

        /// <summary>
        /// Customer PAN number if available
        /// </summary>
        public string? PanNumber { get; set; }

        /// <summary>
        /// Whether this transaction is exempted from TCS
        /// </summary>
        public bool IsExempted { get; set; }

        /// <summary>
        /// Reason for exemption if applicable
        /// </summary>
        public string? ExemptionReason { get; set; }

        /// <summary>
        /// Financial year for this transaction
        /// </summary>
        public string FinancialYear { get; set; } = string.Empty;
    }

    /// <summary>
    /// TCS Transaction DTO for display and reporting
    /// </summary>
    public class TcsTransactionDto
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public string FinancialYear { get; set; } = string.Empty;
        public string? PanNumber { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal CumulativeSaleAmount { get; set; }
        public decimal TcsRate { get; set; }
        public decimal TcsAmount { get; set; }
        public string TcsType { get; set; } = string.Empty;
        public bool IsExempted { get; set; }
        public string? ExemptionReason { get; set; }
        public DateTime TransactionDate { get; set; }
        public int Quarter { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Form 26Q Report DTO - Quarterly TCS return format
    /// </summary>
    public class Form26QReportDto
    {
        public string FinancialYear { get; set; } = string.Empty;
        public int Quarter { get; set; }
        public string QuarterDescription { get; set; } = string.Empty;
        public List<Form26QEntryDto> Entries { get; set; } = new();
        public decimal TotalTcsCollected { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public DateTime GeneratedDate { get; set; }
    }

    /// <summary>
    /// Individual entry in Form 26Q
    /// </summary>
    public class Form26QEntryDto
    {
        public int SerialNumber { get; set; }
        public string CollecteePAN { get; set; } = string.Empty;
        public string CollecteeName { get; set; } = string.Empty;
        public string? CollecteeAddress { get; set; }
        public string? CollecteePhone { get; set; }
        public DateTime TransactionDate { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal AmountReceived { get; set; }
        public decimal TcsRate { get; set; }
        public decimal TcsAmount { get; set; }
        public string NatureOfGoods { get; set; } = "Jewellery";
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// Customer TCS Summary for a financial year
    /// </summary>
    public class CustomerTcsSummaryDto
    {
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerPhone { get; set; }
        public string? CustomerAddress { get; set; }
        public string? PanNumber { get; set; }
        public bool HasValidPAN { get; set; }
        public string FinancialYear { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public decimal TotalTcsCollected { get; set; }
        public int TransactionCount { get; set; }
        public decimal RemainingThreshold { get; set; }
        public decimal ThresholdLimit { get; set; } = 1000000m;
        public List<TcsTransactionDto> RecentTransactions { get; set; } = new();
    }

    /// <summary>
    /// TCS Report Filter DTO
    /// </summary>
    public class TcsReportFilterDto
    {
        public string? FinancialYear { get; set; }
        public int? Quarter { get; set; }
        public long? CustomerId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? TcsType { get; set; }
    }

    /// <summary>
    /// TCS Dashboard Summary DTO
    /// </summary>
    public class TcsDashboardSummaryDto
    {
        public string FinancialYear { get; set; } = string.Empty;
        public decimal TotalTcsCollected { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalSaleAmount { get; set; }
        public List<TcsQuarterlySummaryDto> QuarterlySummaries { get; set; } = new();
    }

    /// <summary>
    /// Quarterly TCS Summary
    /// </summary>
    public class TcsQuarterlySummaryDto
    {
        public int Quarter { get; set; }
        public string QuarterDescription { get; set; } = string.Empty;
        public decimal TcsCollected { get; set; }
        public int TransactionCount { get; set; }
        public decimal SaleAmount { get; set; }
    }
}
