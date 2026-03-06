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
    /// Invoice Item Repository Implementation
    /// Handles data operations for invoice items only (ISP compliance)
    /// </summary>
    public class InvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceItemRepository> _logger;

        public InvoiceItemRepository(
            AppDbContext context,
            IMapper mapper,
            ILogger<InvoiceItemRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<InvoiceItem?> GetInvoiceItemByIdAsync(long id)
        {
            var itemDb = await _context.InvoiceItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (itemDb == null)
            {
                _logger.LogWarning("Invoice item with ID {Id} not found", id);
                return null;
            }

            return _mapper.Map<InvoiceItem>(itemDb);
        }

        public async Task<IEnumerable<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(long invoiceId)
        {
            var itemsDb = await _context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<InvoiceItem>>(itemsDb);
        }

        public async Task<InvoiceItem> AddInvoiceItemAsync(InvoiceItem item)
        {
            var itemDb = _mapper.Map<InvoiceItemDb>(item);
            await _context.InvoiceItems.AddAsync(itemDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added invoice item ID {Id} for invoice {InvoiceId}", itemDb.Id, item.InvoiceId);
            return _mapper.Map<InvoiceItem>(itemDb);
        }

        public async Task AddInvoiceItemsRangeAsync(IEnumerable<InvoiceItem> items)
        {
            var itemsDb = _mapper.Map<IEnumerable<InvoiceItemDb>>(items);
            await _context.InvoiceItems.AddRangeAsync(itemsDb);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added {Count} invoice items", itemsDb.Count());
        }

        public async Task<bool> DeleteInvoiceItemsByInvoiceIdAsync(long invoiceId)
        {
            var items = await _context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToListAsync();

            if (!items.Any())
            {
                _logger.LogWarning("No invoice items found for invoice ID {InvoiceId}", invoiceId);
                return false;
            }

            _context.InvoiceItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted {Count} invoice items for invoice ID {InvoiceId}", items.Count, invoiceId);
            return true;
        }
    }
}