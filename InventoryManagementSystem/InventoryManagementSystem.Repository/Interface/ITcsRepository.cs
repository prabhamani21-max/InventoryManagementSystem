using InventoryManagementSystem.Repository.Models;

namespace InventoryManagementSystem.Repository.Interface
{
    /// <summary>
    /// TCS (Tax Collected at Source) Repository Interface
    /// Handles data access for TCS transactions
    /// </summary>
    public interface ITcsRepository
    {
        // ==================== READ OPERATIONS ====================

        /// <summary>
        /// Get TCS transaction by ID
        /// </summary>
        Task<TcsTransactionDb?> GetByIdAsync(long id);

        /// <summary>
        /// Get TCS transaction by invoice ID
        /// </summary>
        Task<TcsTransactionDb?> GetByInvoiceIdAsync(long invoiceId);

        /// <summary>
        /// Get all TCS transactions for a customer in a financial year
        /// </summary>
        Task<IEnumerable<TcsTransactionDb>> GetByCustomerIdAsync(long customerId, string? financialYear = null);

        /// <summary>
        /// Get all TCS transactions for a financial year and optional quarter
        /// </summary>
        Task<IEnumerable<TcsTransactionDb>> GetByFinancialYearAsync(string financialYear, int? quarter = null);

        /// <summary>
        /// Get cumulative sales for a customer in a financial year
        /// </summary>
        Task<decimal> GetCumulativeSalesAsync(long customerId, string financialYear);

        /// <summary>
        /// Get all TCS transactions
        /// </summary>
        Task<IEnumerable<TcsTransactionDb>> GetAllAsync();

        /// <summary>
        /// Check if TCS transaction exists for an invoice
        /// </summary>
        Task<bool> ExistsForInvoiceAsync(long invoiceId);

        // ==================== WRITE OPERATIONS ====================

        /// <summary>
        /// Create a new TCS transaction
        /// </summary>
        Task<TcsTransactionDb> CreateAsync(TcsTransactionDb transaction);

        /// <summary>
        /// Update an existing TCS transaction
        /// </summary>
        Task<TcsTransactionDb> UpdateAsync(TcsTransactionDb transaction);

        /// <summary>
        /// Delete a TCS transaction
        /// </summary>
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Get TCS summary for dashboard
        /// </summary>
        Task<(decimal TotalTcs, int TransactionCount, int CustomerCount, decimal TotalSales)> GetSummaryAsync(string financialYear);

        /// <summary>
        /// Get quarterly breakdown for a financial year
        /// </summary>
        Task<IEnumerable<(int Quarter, decimal TcsAmount, int TransactionCount, decimal SaleAmount)>> GetQuarterlySummaryAsync(string financialYear);
    }
}
