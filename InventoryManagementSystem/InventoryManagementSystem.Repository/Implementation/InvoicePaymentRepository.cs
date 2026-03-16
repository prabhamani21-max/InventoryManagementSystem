using AutoMapper;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    /// <summary>
    /// Invoice Payment Repository Implementation
    /// Handles data operations for invoice payments only (ISP compliance)
    /// </summary>
    public class InvoicePaymentRepository : IInvoicePaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoicePaymentRepository> _logger;

        public InvoicePaymentRepository(
            AppDbContext context,
            IMapper mapper,
            ILogger<InvoicePaymentRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InvoicePayment?> GetInvoicePaymentByIdAsync(long id)
        {
            var paymentDb = await _context.InvoicePayments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paymentDb == null)
            {
                _logger.LogWarning("Invoice payment with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<InvoicePayment>(paymentDb);
        }

        public async Task<IEnumerable<InvoicePayment>> GetInvoicePaymentsByInvoiceIdAsync(long invoiceId)
        {
            var paymentsDb = await _context.InvoicePayments
                .Where(p => p.InvoiceId == invoiceId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<InvoicePayment>>(paymentsDb);
        }

        public async Task<InvoicePayment> AddInvoicePaymentAsync(InvoicePayment payment)
        {
            var paymentDb = _mapper.Map<InvoicePaymentDb>(payment);
            await _context.InvoicePayments.AddAsync(paymentDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added invoice payment ID {Id} for invoice {InvoiceId}", paymentDb.Id, payment.InvoiceId);
            return _mapper.Map<InvoicePayment>(paymentDb);
        }

        public async Task AddInvoicePaymentsRangeAsync(IEnumerable<InvoicePayment> payments)
        {
            var paymentsDb = _mapper.Map<IEnumerable<InvoicePaymentDb>>(payments);
            await _context.InvoicePayments.AddRangeAsync(paymentsDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added {Count} invoice payments", paymentsDb.Count());
        }

        public async Task<bool> DeleteInvoicePaymentsByInvoiceIdAsync(long invoiceId)
        {
            var payments = await _context.InvoicePayments
                .Where(p => p.InvoiceId == invoiceId)
                .ToListAsync();

            if (!payments.Any())
            {
                _logger.LogWarning("No invoice payments found for invoice ID {InvoiceId}", invoiceId);
                return false;
            }

            _context.InvoicePayments.RemoveRange(payments);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted {Count} invoice payments for invoice ID {InvoiceId}", payments.Count, invoiceId);
            return true;
        }
    }
}