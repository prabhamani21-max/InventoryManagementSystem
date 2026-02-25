using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Implementation
{
    /// <summary>
    /// TCS (Tax Collected at Source) Repository Implementation
    /// Handles data access for TCS transactions
    /// </summary>
    public class TcsRepository : ITcsRepository
    {
        private readonly AppDbContext _context;

        public TcsRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==================== READ OPERATIONS ====================

        public async Task<TcsTransactionDb?> GetByIdAsync(long id)
        {
            return await _context.TcsTransactions
                .Include(t => t.Invoice)
                .Include(t => t.Customer)
                .Include(t => t.CreatedUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TcsTransactionDb?> GetByInvoiceIdAsync(long invoiceId)
        {
            return await _context.TcsTransactions
                .Include(t => t.Invoice)
                .Include(t => t.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<TcsTransactionDb>> GetByCustomerIdAsync(long customerId, string? financialYear = null)
        {
            var query = _context.TcsTransactions
                .Include(t => t.Invoice)
                .Include(t => t.Customer)
                .Where(t => t.CustomerId == customerId);

            if (!string.IsNullOrEmpty(financialYear))
            {
                query = query.Where(t => t.FinancialYear == financialYear);
            }

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<TcsTransactionDb>> GetByFinancialYearAsync(string financialYear, int? quarter = null)
        {
            var query = _context.TcsTransactions
                .Include(t => t.Invoice)
                .Include(t => t.Customer)
                .Where(t => t.FinancialYear == financialYear);

            if (quarter.HasValue)
            {
                query = query.Where(t => t.Quarter == quarter.Value);
            }

            return await query
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<decimal> GetCumulativeSalesAsync(long customerId, string financialYear)
        {
            var transactions = await _context.TcsTransactions
                .Where(t => t.CustomerId == customerId && t.FinancialYear == financialYear)
                .ToListAsync();

            // Sum all sale amounts for the customer in the financial year
            return transactions.Sum(t => t.SaleAmount);
        }

        public async Task<IEnumerable<TcsTransactionDb>> GetAllAsync()
        {
            return await _context.TcsTransactions
                .Include(t => t.Invoice)
                .Include(t => t.Customer)
                .OrderByDescending(t => t.TransactionDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsForInvoiceAsync(long invoiceId)
        {
            return await _context.TcsTransactions
                .AnyAsync(t => t.InvoiceId == invoiceId);
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task<TcsTransactionDb> CreateAsync(TcsTransactionDb transaction)
        {
            await _context.TcsTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<TcsTransactionDb> UpdateAsync(TcsTransactionDb transaction)
        {
            var existing = await _context.TcsTransactions
                .FirstOrDefaultAsync(t => t.Id == transaction.Id);

            if (existing == null)
                throw new InvalidOperationException($"TCS Transaction with ID {transaction.Id} not found");

            // Update fields
            existing.TcsRate = transaction.TcsRate;
            existing.TcsAmount = transaction.TcsAmount;
            existing.TcsType = transaction.TcsType;
            existing.IsExempted = transaction.IsExempted;
            existing.ExemptionReason = transaction.ExemptionReason;
            existing.UpdatedBy = transaction.UpdatedBy;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.TcsTransactions.FindAsync(id);
            if (entity == null) return false;

            _context.TcsTransactions.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(decimal TotalTcs, int TransactionCount, int CustomerCount, decimal TotalSales)> GetSummaryAsync(string financialYear)
        {
            var transactions = await _context.TcsTransactions
                .Where(t => t.FinancialYear == financialYear && !t.IsExempted)
                .ToListAsync();

            var totalTcs = transactions.Sum(t => t.TcsAmount);
            var transactionCount = transactions.Count;
            var customerCount = transactions.Select(t => t.CustomerId).Distinct().Count();
            var totalSales = transactions.Sum(t => t.SaleAmount);

            return (totalTcs, transactionCount, customerCount, totalSales);
        }

        public async Task<IEnumerable<(int Quarter, decimal TcsAmount, int TransactionCount, decimal SaleAmount)>> GetQuarterlySummaryAsync(string financialYear)
        {
            var transactions = await _context.TcsTransactions
                .Where(t => t.FinancialYear == financialYear && !t.IsExempted)
                .ToListAsync();

            var quarterlyData = transactions
                .GroupBy(t => t.Quarter)
                .Select(g => (
                    Quarter: g.Key,
                    TcsAmount: g.Sum(t => t.TcsAmount),
                    TransactionCount: g.Count(),
                    SaleAmount: g.Sum(t => t.SaleAmount)
                ))
                .OrderBy(q => q.Quarter)
                .ToList();

            return quarterlyData;
        }
    }
}
