using AutoMapper;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSytem.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Repository.Implementation
{
    /// <summary>
    /// Invoice Repository - handles database operations for invoices
    /// Uses specialized services for calculations (InvoiceNumberService, InvoiceTaxService, InvoiceBuilderService)
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
                .Include(i => i.InvoiceItems)
                .Include(i => i.InvoicePayments)
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
                .Include(i => i.InvoiceItems)
                .Include(i => i.InvoicePayments)
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
                .Include(i => i.InvoiceItems)
                .Include(i => i.InvoicePayments)
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
                .Include(i => i.InvoiceItems)
                .Include(i => i.InvoicePayments)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<Invoice>>(invoicesDb);
        }

        public async Task<InvoiceItem?> GetInvoiceItemByIdAsync(long id)
        {
            var itemDb = await _context.InvoiceItems.FindAsync(id);
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

        public async Task<InvoicePayment?> GetInvoicePaymentByIdAsync(long id)
        {
            var paymentDb = await _context.InvoicePayments.FindAsync(id);
            return _mapper.Map<InvoicePayment>(paymentDb);
        }

        public async Task<IEnumerable<InvoicePayment>> GetInvoicePaymentsByInvoiceIdAsync(long invoiceId)
        {
            var paymentsDb = await _context.InvoicePayments
                .Where(i => i.InvoiceId == invoiceId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<InvoicePayment>>(paymentsDb);
        }

        // ==================== WRITE OPERATIONS ====================

        /// <summary>
        /// Generates and saves a complete invoice from a sale order
        /// All business logic (GST, numbering, item building) is handled by specialized services
        /// </summary>
        public async Task<Invoice> GenerateInvoiceAsync(Invoice invoice)
        {
            _logger.LogInformation("Generating invoice for Sale Order {SaleOrderId}", invoice.SaleOrderId);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var saleOrderId = invoice.SaleOrderId ?? throw new InvalidOperationException("SaleOrderId is required");

                // Get sale order
                var saleOrderDb = await _context.SaleOrders
                    .FirstOrDefaultAsync(so => so.Id == saleOrderId)
                    ?? throw new InvalidOperationException($"Sale Order with ID {saleOrderId} not found");

                // Get customer
                var customer = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == saleOrderDb.CustomerId);

                // Get sale order items with jewellery details
                var saleOrderItems = await _context.SaleOrderItems
                    .Include(soi => soi.JewelleryItem)
                    .Where(soi => soi.SaleOrderId == saleOrderId)
                    .ToListAsync();

                if (!saleOrderItems.Any())
                {
                    throw new InvalidOperationException("No items found in sale order");
                }

                // Get jewellery item details
                var jewelleryItemIds = saleOrderItems.Select(soi => soi.JewelleryItemId).Distinct().ToList();
                var jewelleryItems = await _context.JewelleryItems
                    .Where(ji => jewelleryItemIds.Contains(ji.Id))
                    .ToDictionaryAsync(ji => (int)ji.Id, ji => ji);

                // Get item stones
                var itemStones = await _context.ItemStones
                    .Where(is_ => jewelleryItemIds.Contains((long)is_.JewelleryItemId))
                    .ToListAsync();

                // Get metal and purity details
                var metalIds = jewelleryItems.Values.Select(ji => ji.MetalId).Distinct().ToList();
                var purityIds = jewelleryItems.Values.Select(ji => ji.PurityId).Distinct().ToList();
                var metals = await _context.Metals.Where(m => metalIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id, m => m);
                var purities = await _context.Purities.Where(p => purityIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p);

                // Get payments
                var payments = await _context.Payments
                    .Where(p => p.OrderId == saleOrderId && p.OrderType == TransactionType.SALE)
                    .ToListAsync();

                // Build invoice
                invoice.InvoiceNumber = await GenerateNextInvoiceNumberAsync();
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceType = TransactionType.SALE.ToString();

                // Company Details
                invoice.CompanyName = "Jewellery Store";
                invoice.CompanyAddress = "123 Jewellery Market, City - 123456";
                invoice.CompanyPhone = "+91 9876543210";
                invoice.CompanyEmail = "info@jewellerystore.com";
                invoice.CompanyGSTIN = "07AAACJ1234A1Z5";
                invoice.CompanyPAN = "AAACJ1234A";
                invoice.CompanyHallmarkLicense = "HM/2023/123456";

                // Party (Customer) Details
                invoice.PartyId = saleOrderDb.CustomerId;
                invoice.PartyType = PartyType.CUSTOMER;
                invoice.PartyName = customer?.Name ?? "Walk-in Customer";
                invoice.PartyAddress = customer?.Address;
                invoice.PartyEmail = customer?.Email;

                // Order Reference
                invoice.SaleOrderId = saleOrderDb.Id;

                // Build invoice items (using specialized builder logic)
                var invoiceItems = BuildInvoiceItems(saleOrderItems, jewelleryItems, itemStones, metals, purities);

                // Calculate sums from items
                invoice.InvoiceItems = invoiceItems;
                invoice.SubTotal = invoiceItems.Sum(i => i.ItemSubtotal);
                invoice.DiscountAmount = invoiceItems.Sum(i => i.Discount);
                invoice.TaxableAmount = invoiceItems.Sum(i => i.TaxableAmount);
                invoice.CGSTAmount = invoiceItems.Sum(i => i.CGSTAmount);
                invoice.SGSTAmount = invoiceItems.Sum(i => i.SGSTAmount);
                invoice.IGSTAmount = invoiceItems.Sum(i => i.IGSTAmount);
                invoice.TotalGSTAmount = invoiceItems.Sum(i => i.GSTAmount);

                invoice.RoundOff = 0;
                invoice.GrandTotal = invoice.TaxableAmount + invoice.TotalGSTAmount;
                invoice.RoundOff = Math.Round(invoice.GrandTotal) - invoice.GrandTotal;
                invoice.GrandTotal = Math.Round(invoice.GrandTotal);
                invoice.GrandTotalInWords = NumberToWords(invoice.GrandTotal);

                // Jewellery-specific totals
                invoice.TotalGoldWeight = invoiceItems.Sum(i => i.NetMetalWeight ?? 0);
                invoice.TotalStoneWeight = invoiceItems.Where(i => i.StoneWeight.HasValue).Sum(i => i.StoneWeight ?? 0);
                invoice.TotalPieces = invoiceItems.Count;
                invoice.IsHallmarked = invoiceItems.Any(i => i.IsHallmarked);
                invoice.BISHallmarkNumber = invoice.IsHallmarked ? "HM/2023/123456" : null;

                // Payment allocation
                decimal totalPaid = 0;
                var remaining = invoice.GrandTotal;
                invoice.InvoicePayments = new List<InvoicePayment>();
                foreach (var payment in payments)
                {
                    if (remaining <= 0) break;
                    var alloc = Math.Min(payment.Amount, remaining);
                    invoice.InvoicePayments.Add(new InvoicePayment
                    {
                        PaymentId = payment.Id,
                        AllocatedAmount = alloc,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1
                    });
                    remaining -= alloc;
                    totalPaid += alloc;
                }
                invoice.TotalPaid = totalPaid;
                invoice.BalanceDue = invoice.GrandTotal - totalPaid;

                // Footer
                invoice.TermsAndConditions = invoice.IncludeTermsAndConditions 
                    ? @"1. Goods once sold will not be taken back.
2. Exchange within 15 days with original bill.
3. Hallmark certified jewellery as per BIS standards.
4. 100% return policy on diamond jewellery." 
                    : null;
                invoice.ReturnPolicy = invoice.IncludeTermsAndConditions 
                    ? @"• 15-day return policy on all items
• 100% money back guarantee
• Exchange available on full amount
• Making charges are non-refundable" 
                    : null;
                invoice.Notes = invoice.Notes;
                invoice.Declaration = invoice.IncludeTermsAndConditions 
                    ? @"I hereby declare that the jewellery items mentioned above are hallmarked as per BIS standards and the stone details are as per the specification provided by the customer." 
                    : null;

                // Audit fields
                invoice.CreatedDate = DateTime.UtcNow;
                invoice.CreatedBy = 1;
                invoice.UpdatedDate = null;
                invoice.UpdatedBy = null;
                invoice.StatusId = 1;

                // Save invoice
                var invoiceDb = _mapper.Map<InvoiceDb>(invoice);
                await _context.Invoices.AddAsync(invoiceDb);
                await _context.SaveChangesAsync();

                // Save invoice items
                foreach (var item in invoiceItems)
                {
                    item.InvoiceId = invoiceDb.Id;
                    var itemDb = _mapper.Map<InvoiceItemDb>(item);
                    await _context.InvoiceItems.AddAsync(itemDb);
                }

                // Save invoice payments
                foreach (var payment in invoice.InvoicePayments)
                {
                    payment.InvoiceId = invoiceDb.Id;
                    var paymentDb = _mapper.Map<InvoicePaymentDb>(payment);
                    await _context.InvoicePayments.AddAsync(paymentDb);
                }

                await _context.SaveChangesAsync();
                
                // Deduct stock for each item in the invoice
                foreach (var saleOrderItem in saleOrderItems)
                {
                    var deducted = await DeductStockForInvoiceAsync(saleOrderItem.JewelleryItemId, saleOrderItem.Quantity);
                    if (!deducted)
                    {
                        _logger.LogWarning("Failed to deduct stock for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}", 
                            saleOrderItem.JewelleryItemId, saleOrderItem.Quantity);
                    }
                }
                
                await transaction.CommitAsync();

                _logger.LogInformation("Invoice {InvoiceNumber} generated successfully", invoice.InvoiceNumber);
                return _mapper.Map<Invoice>(invoiceDb);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error generating invoice for Sale Order {SaleOrderId}", invoice.SaleOrderId);
                throw;
            }
        }

        /// <summary>
        /// Deducts stock when an invoice is generated
        /// </summary>
        private async Task<bool> DeductStockForInvoiceAsync(long jewelleryItemId, int quantity)
        {
            var stock = await _context.ItemStocks
                .FirstOrDefaultAsync(s => s.JewelleryItemId == jewelleryItemId);

            if (stock == null)
            {
                _logger.LogWarning("No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                return false;
            }

            if (stock.Quantity < quantity)
            {
                _logger.LogWarning("Insufficient stock for JewelleryItemId: {JewelleryItemId}. Available: {Available}, Requested: {Requested}",
                    jewelleryItemId, stock.Quantity, quantity);
                return false;
            }

            stock.Quantity -= quantity;
            // Also reduce reserved quantity (the stock was reserved during order creation)
            stock.ReservedQuantity = Math.Max(0, stock.ReservedQuantity - quantity);
            stock.UpdatedDate = DateTime.UtcNow;

            _logger.LogInformation("Deducted {Quantity} units from stock for JewelleryItemId: {JewelleryItemId}. Remaining: {Remaining}",
                quantity, jewelleryItemId, stock.Quantity);

            return true;
        }

        public async Task<Invoice> UpdateInvoiceAsync(Invoice invoice)
        {
            _logger.LogInformation("Updating invoice ID {Id}", invoice.Id);

            var invoiceDb = await _context.Invoices.FindAsync(invoice.Id);
            if (invoiceDb == null) return null;

            // Manual mapping to preserve CreatedBy and CreatedDate
            invoiceDb.InvoiceNumber = invoice.InvoiceNumber;
            invoiceDb.InvoiceDate = invoice.InvoiceDate;
            invoiceDb.InvoiceType = Enum.Parse<TransactionType>(invoice.InvoiceType);
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

            return _mapper.Map<Invoice>(invoiceDb);
        }

        public async Task<bool> DeleteInvoiceAsync(long id)
        {
            var invoiceDb = await _context.Invoices.FindAsync(id);
            if (invoiceDb == null) return false;

            _context.Invoices.Remove(invoiceDb);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelInvoiceAsync(string invoiceNumber)
        {
            _logger.LogInformation("Invoice {InvoiceNumber} cancellation requested", invoiceNumber);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoiceDb = await _context.Invoices
                    .Include(i => i.InvoiceItems)
                    .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);

                if (invoiceDb == null)
                {
                    _logger.LogWarning("Invoice {InvoiceNumber} not found for cancellation", invoiceNumber);
                    return false;
                }

                // Get sale order items to restore stock
                if (invoiceDb.SaleOrderId.HasValue)
                {
                    var saleOrderItems = await _context.SaleOrderItems
                        .Where(soi => soi.SaleOrderId == invoiceDb.SaleOrderId.Value)
                        .ToListAsync();

                    // Restore stock for each item
                    foreach (var item in saleOrderItems)
                    {
                        var restored = await RestoreStockForInvoiceCancellationAsync(item.JewelleryItemId, item.Quantity);
                        if (!restored)
                        {
                            _logger.LogWarning("Failed to restore stock for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}",
                                item.JewelleryItemId, item.Quantity);
                        }
                    }
                }

                invoiceDb.StatusId = 0;
                invoiceDb.UpdatedDate = DateTime.UtcNow;
                invoiceDb.UpdatedBy = 1;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Invoice {InvoiceNumber} cancelled successfully and stock restored", invoiceNumber);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error cancelling invoice {InvoiceNumber}", invoiceNumber);
                throw;
            }
        }

        /// <summary>
        /// Restores stock when an invoice is cancelled
        /// </summary>
        private async Task<bool> RestoreStockForInvoiceCancellationAsync(long jewelleryItemId, int quantity)
        {
            var stock = await _context.ItemStocks
                .FirstOrDefaultAsync(s => s.JewelleryItemId == jewelleryItemId);

            if (stock == null)
            {
                _logger.LogWarning("No stock record found for JewelleryItemId: {JewelleryItemId}", jewelleryItemId);
                return false;
            }

            stock.Quantity += quantity;
            stock.UpdatedDate = DateTime.UtcNow;

            _logger.LogInformation("Restored {Quantity} units to stock for JewelleryItemId: {JewelleryItemId}. New Total: {NewTotal}",
                quantity, jewelleryItemId, stock.Quantity);

            return true;
        }

        // ==================== HELPER METHODS ====================

        private List<InvoiceItem> BuildInvoiceItems(
            List<SaleOrderItemDb> saleOrderItems,
            Dictionary<int, JewelleryItemDb> jewelleryItems,
            List<ItemStoneDb> itemStones,
            Dictionary<int, MetalDb> metals,
            Dictionary<int, PurityDb> purities)
        {
            var items = new List<InvoiceItem>();

            foreach (var item in saleOrderItems)
            {
                if (!jewelleryItems.TryGetValue((int)item.JewelleryItemId, out var jewelleryItem))
                {
                    continue;
                }

                var stoneData = itemStones.FirstOrDefault(is_ => is_.JewelleryItemId == item.JewelleryItemId);

                // GST breakdown from SaleOrderItem (50% CGST + 50% SGST)
                var cgstAmount = item.GstAmount / 2;
                var sgstAmount = item.GstAmount / 2;
                var igstAmount = 0m;

                var invoiceItem = new InvoiceItem
                {
                    ReferenceItemId = item.Id,
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,

                    // Metal Details - snapshot from SaleOrderItem
                    MetalId = item.MetalId,
                    PurityId = item.PurityId,
                    NetMetalWeight = item.NetMetalWeight,
                    MetalAmount = item.MetalAmount, // FIXED: Use item.MetalAmount, NOT item.TotalAmount

                    // Stone Details - SNAPSHOT at billing time
                    StoneId = item.HasStone ? jewelleryItem.StoneId : null,
                    StoneWeight = item.HasStone && stoneData != null ? stoneData.Weight : null,
                    StoneAmount = item.StoneAmount ?? 0,

                    // Making Charges - from SaleOrderItem
                    MakingCharges = item.TotalMakingCharges,
                    WastageAmount = item.WastageAmount,

                    // Pricing - from SaleOrderItem
                    ItemSubtotal = item.ItemSubtotal,
                    Discount = item.DiscountAmount,
                    TaxableAmount = item.TaxableAmount,

                    // GST Breakdown - derived from SaleOrderItem
                    CGSTAmount = cgstAmount,
                    SGSTAmount = sgstAmount,
                    IGSTAmount = igstAmount,
                    GSTAmount = item.GstAmount,
                    TotalAmount = item.TotalAmount,

                    // Hallmark snapshot
                    IsHallmarked = item.IsHallmarked
                };

                items.Add(invoiceItem);
            }

            return items;
        }

        private async Task<string> GenerateNextInvoiceNumberAsync()
        {
            var now = DateTime.UtcNow;
            int month = now.Month;
            int year = now.Year;
            int startYear = month >= 4 ? year : year - 1;
            int endYear = startYear + 1;
            var fy = $"{startYear.ToString().Substring(2, 2)}-{endYear.ToString().Substring(2, 2)}";
            
            var existingCount = await _context.Invoices
                .Where(i => i.InvoiceNumber.StartsWith($"INV/{fy}/"))
                .CountAsync();
            
            return $"INV/{fy}/{(existingCount + 1):D6}";
        }

        public string NumberToWords(decimal number)
        {
            var units = new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            var teens = new[] { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tens = new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            var rupees = (long)number;
            var paise = (int)((number - rupees) * 100);

            if (rupees == 0)
                return "Zero Rupees";

            var words = "";

            var crores = rupees / 10000000;
            rupees %= 10000000;
            if (crores > 0)
            {
                words += NumberToWordsUnderCrore((long)crores) + " Crore ";
            }

            var lakhs = rupees / 100000;
            rupees %= 100000;
            if (lakhs > 0)
            {
                words += NumberToWordsUnderCrore((long)lakhs) + " Lakh ";
            }

            var thousands = rupees / 1000;
            rupees %= 1000;
            if (thousands > 0)
            {
                words += NumberToWordsUnderCrore((long)thousands) + " Thousand ";
            }

            var hundreds = rupees / 100;
            rupees %= 100;
            if (hundreds > 0)
            {
                words += units[hundreds] + " Hundred ";
            }

            if (rupees >= 20)
            {
                words += tens[rupees / 10] + " " + units[rupees % 10];
            }
            else if (rupees >= 10)
            {
                words += teens[rupees - 10];
            }
            else if (rupees > 0)
            {
                words += units[rupees];
            }

            words += "Rupees";

            if (paise > 0)
            {
                if (paise >= 20)
                {
                    words += " and " + tens[paise / 10] + " " + units[paise % 10] + " Paise";
                }
                else if (paise >= 10)
                {
                    words += " and " + teens[paise - 10] + " Paise";
                }
                else
                {
                    words += " and " + units[paise] + " Paise";
                }
            }

            return words;
        }

        private string NumberToWordsUnderCrore(long number)
        {
            var words = "";
            var units = new[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
            var tens = new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            var thousands = number / 1000;
            number %= 1000;
            if (thousands > 0)
            {
                words += units[thousands] + " Thousand ";
            }

            var hundreds = number / 100;
            number %= 100;
            if (hundreds > 0)
            {
                words += units[hundreds] + " Hundred ";
            }

            if (number >= 20)
            {
                words += tens[number / 10] + " " + units[number % 10];
            }
            else if (number >= 10)
            {
                var teensLocal = new[] { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                words += teensLocal[number - 10];
            }
            else if (number > 0)
            {
                words += units[number];
            }

            return words;
        }
    }
}
