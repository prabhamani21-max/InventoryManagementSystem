using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class InvoiceNumberService : IInvoiceNumberService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<InvoiceNumberService> _logger;

        public InvoiceNumberService(AppDbContext context, ILogger<InvoiceNumberService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateNextInvoiceNumberAsync()
        {
            // Get financial year (April-March)
            var now = DateTime.UtcNow;
            int year = now.Year;
            int month = now.Month;
            
            // Financial year starts in April
            int startYear = month >= 4 ? year : year - 1;
            int endYear = startYear + 1;
            var fy = $"{startYear.ToString().Substring(2, 2)}-{endYear.ToString().Substring(2, 2)}";
            
            // Get the next sequence number from database
            var existingCount = await _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV/{fy}/"))
                .CountAsync();
            
            var sequence = existingCount + 1;
            var invoiceNumber = $"INV/{fy}/{sequence:D6}";
            
            _logger.LogInformation("Generated invoice number: {InvoiceNumber}", invoiceNumber);
            
            return invoiceNumber;
        }
    }
}
