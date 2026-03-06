using AutoMapper;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    /// <summary>
    /// Invoice Repository Implementation
    /// Simplified to handle Invoice entity only (ISP compliance)
    /// InvoiceItem operations are handled by IInvoiceItemRepository
    /// InvoicePayment operations are handled by IInvoicePaymentRepository
    /// </summary>
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceRepository> _logger;

        public InvoiceRepository(
            AppDbContext context,
            IMapper mapper,
            ILogger<InvoiceRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // ==================== READ OPERATIONS ====================

        public async Task<Invoice?> GetInvoiceByIdAsync(long id)
        {
            var invoiceDb = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<Invoice?> GetInvoiceByInvoiceNumberAsync(string invoiceNumber)
        {
            var invoiceDb = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);

            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice with number {InvoiceNumber} not found", invoiceNumber);
                return null;
            }

            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<Invoice?> GetInvoiceBySaleOrderIdAsync(long saleOrderId)
        {
            var invoiceDb = await _context.Invoices
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.SaleOrderId == saleOrderId);

            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice for Sale Order {SaleOrderId} not found", saleOrderId);
                return null;
            }

            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            var invoicesDb = await _context.Invoices
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Invoice>>(invoicesDb);
        }

        // ==================== WRITE OPERATIONS ====================

        public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
        {
            _logger.LogInformation("Adding invoice for Sale Order {SaleOrderId}", invoice.SaleOrderId);

            var invoiceDb = _mapper.Map<InvoiceDb>(invoice);
            await _context.Invoices.AddAsync(invoiceDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {InvoiceNumber} added successfully with ID {Id}", invoiceDb.InvoiceNumber, invoiceDb.Id);
            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            _logger.LogInformation("Updating invoice ID {Id}", invoice.Id);

            var invoiceDb = await _context.Invoices.FindAsync(invoice.Id);
            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found for update", invoice.Id);
                return null;
            }

            // Manual mapping to preserve CreatedBy and CreatedDate
            invoiceDb.InvoiceNumber = invoice.InvoiceNumber;
            invoiceDb.InvoiceDate = invoice.InvoiceDate;
            invoiceDb.InvoiceType = Enum.Parse<InventoryManagementSytem.Common.Enums.TransactionType>(invoice.InvoiceType);
            invoiceDb.CompanyName = invoice.CompanyName;
            invoiceDb.CompanyAddress = invoice.CompanyAddress;
            invoiceDb.CompanyPhone = invoice.CompanyPhone;
            invoiceDb.CompanyEmail = invoice.CompanyEmail;
            invoiceDb.CompanyGSTIN = invoice.CompanyGSTIN;
            invoiceDb.CompanyPAN = invoice.CompanyPAN;
            invoiceDb.CompanyHallmarkLicense = invoice.CompanyHallmarkLicense;
            invoiceDb.PartyId = invoice.PartyId;
            invoiceDb.PartyType = invoice.PartyType;
            invoiceDb.PartyName = invoice.PartyName;
            invoiceDb.PartyAddress = invoice.PartyAddress;
            invoiceDb.PartyPhone = invoice.PartyPhone;
            invoiceDb.PartyEmail = invoice.PartyEmail;
            invoiceDb.PartyGSTIN = invoice.PartyGSTIN;
            invoiceDb.PartyPANNUmber = invoice.PartyPANNUmber;
            invoiceDb.SaleOrderId = invoice.SaleOrderId;
            invoiceDb.PurchaseOrderId = invoice.PurchaseOrderId;
            invoiceDb.SubTotal = invoice.SubTotal;
            invoiceDb.DiscountAmount = invoice.DiscountAmount;
            invoiceDb.TaxableAmount = invoice.TaxableAmount;
            invoiceDb.CGSTAmount = invoice.CGSTAmount;
            invoiceDb.SGSTAmount = invoice.SGSTAmount;
            invoiceDb.IGSTAmount = invoice.IGSTAmount;
            invoiceDb.TotalGSTAmount = invoice.TotalGSTAmount;
            invoiceDb.RoundOff = invoice.RoundOff;
            invoiceDb.GrandTotal = invoice.GrandTotal;
            invoiceDb.GrandTotalInWords = invoice.GrandTotalInWords;
            invoiceDb.TotalPaid = invoice.TotalPaid;
            invoiceDb.BalanceDue = invoice.BalanceDue;
            invoiceDb.TotalGoldWeight = invoice.TotalGoldWeight;
            invoiceDb.TotalStoneWeight = invoice.TotalStoneWeight;
            invoiceDb.TotalPieces = invoice.TotalPieces;
            invoiceDb.TermsAndConditions = invoice.TermsAndConditions;
            invoiceDb.ReturnPolicy = invoice.ReturnPolicy;
            invoiceDb.Notes = invoice.Notes;
            invoiceDb.Declaration = invoice.Declaration;
            invoiceDb.StatusId = invoice.StatusId;
            invoiceDb.IRN = invoice.IRN;
            invoiceDb.IRNGeneratedDate = invoice.IRNGeneratedDate;
            invoiceDb.QRCode = invoice.QRCode;
            invoiceDb.EInvoiceStatus = invoice.EInvoiceStatus;
            invoiceDb.EInvoiceCancelledDate = invoice.EInvoiceCancelledDate;
            invoiceDb.EInvoiceCancelReason = invoice.EInvoiceCancelReason;
            invoiceDb.AcknowledgementNumber = invoice.AcknowledgementNumber;
            invoiceDb.AcknowledgementDate = invoice.AcknowledgementDate;
            invoiceDb.UpdatedBy = invoice.UpdatedBy;
            invoiceDb.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {InvoiceNumber} updated successfully", invoiceDb.InvoiceNumber);
            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<bool> DeleteInvoiceAsync(long id)
        {
            var invoiceDb = await _context.Invoices.FindAsync(id);
            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found for deletion", id);
                return false;
            }

            _context.Invoices.Remove(invoiceDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {InvoiceNumber} deleted successfully", invoiceDb.InvoiceNumber);
            return true;
        }

        public async Task<bool> CancelInvoiceAsync(string invoiceNumber)
        {
            _logger.LogInformation("Invoice {InvoiceNumber} cancellation requested", invoiceNumber);

            var invoiceDb = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);

            if (invoiceDb == null)
            {
                _logger.LogWarning("Invoice {InvoiceNumber} not found for cancellation", invoiceNumber);
                return false;
            }

            invoiceDb.StatusId = 4; // Cancelled status
            invoiceDb.UpdatedDate = DateTime.UtcNow;
            invoiceDb.UpdatedBy = 1;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {InvoiceNumber} cancelled successfully", invoiceNumber);
            return true;
        }
    }
}
