using InventoryManagementSytem.Common.Dtos;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// TCS (Tax Collected at Source) Service Interface
    /// Handles TCS calculation and reporting for B2C jewellery sales
    /// </summary>
    public interface ITcsService
    {
        /// <summary>
        /// Calculate TCS for a sale transaction
        /// Returns TCS details including applicability, rate, and amount
        /// </summary>
        /// <param name="request">TCS calculation request with customer and sale details</param>
        /// <returns>TCS calculation response</returns>
        Task<TcsCalculationResponseDto> CalculateTcsAsync(TcsCalculationRequestDto request);

        /// <summary>
        /// Get customer's cumulative sales for the financial year
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="financialYear">Financial year (e.g., "2024-25")</param>
        /// <returns>Total cumulative sales amount</returns>
        Task<decimal> GetCumulativeSalesAsync(long customerId, string financialYear);

        /// <summary>
        /// Check if customer has valid PAN for TCS rate determination
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>Tuple with hasValidPAN flag and PAN number</returns>
        Task<(bool hasValidPAN, string? panNumber)> CheckCustomerPANAsync(long customerId);

        /// <summary>
        /// Create TCS transaction record for an invoice
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <returns>Created TCS transaction DTO</returns>
        Task<TcsTransactionDto> CreateTcsTransactionAsync(long invoiceId);

        /// <summary>
        /// Generate Form 26Q report data for quarterly TCS return
        /// </summary>
        /// <param name="financialYear">Financial year</param>
        /// <param name="quarter">Quarter (1-4)</param>
        /// <returns>Form 26Q report DTO</returns>
        Task<Form26QReportDto> GenerateForm26QReportAsync(string financialYear, int quarter);

        /// <summary>
        /// Get TCS transactions for a customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="financialYear">Optional financial year filter</param>
        /// <returns>List of TCS transactions</returns>
        Task<List<TcsTransactionDto>> GetCustomerTcsTransactionsAsync(long customerId, string? financialYear = null);

        /// <summary>
        /// Get customer TCS summary for a financial year
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="financialYear">Financial year</param>
        /// <returns>Customer TCS summary</returns>
        Task<CustomerTcsSummaryDto> GetCustomerTcsSummaryAsync(long customerId, string financialYear);

        /// <summary>
        /// Get all TCS transactions for a financial year/quarter
        /// </summary>
        /// <param name="financialYear">Financial year</param>
        /// <param name="quarter">Optional quarter filter</param>
        /// <returns>List of TCS transactions</returns>
        Task<List<TcsTransactionDto>> GetTcsTransactionsAsync(string financialYear, int? quarter = null);

        /// <summary>
        /// Get TCS dashboard summary for a financial year
        /// </summary>
        /// <param name="financialYear">Financial year</param>
        /// <returns>TCS dashboard summary</returns>
        Task<TcsDashboardSummaryDto> GetDashboardSummaryAsync(string financialYear);

        /// <summary>
        /// Get TCS transaction by ID
        /// </summary>
        /// <param name="id">TCS transaction ID</param>
        /// <returns>TCS transaction DTO</returns>
        Task<TcsTransactionDto?> GetTcsTransactionByIdAsync(long id);

        /// <summary>
        /// Get TCS transaction by invoice ID
        /// </summary>
        /// <param name="invoiceId">Invoice ID</param>
        /// <returns>TCS transaction DTO</returns>
        Task<TcsTransactionDto?> GetTcsTransactionByInvoiceIdAsync(long invoiceId);

        /// <summary>
        /// Get current financial year based on current date
        /// </summary>
        /// <returns>Financial year string (e.g., "2024-25")</returns>
        string GetCurrentFinancialYear();

        /// <summary>
        /// Get financial year for a given date
        /// </summary>
        /// <param name="date">Date to determine financial year for</param>
        /// <returns>Financial year string</returns>
        string GetFinancialYear(DateTime date);
    }
}
