using AutoMapper;
using InventoryManagementSytem.Common.Enums;
using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Repository.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Invoice Generator Service Implementation
    /// Orchestrates the complete invoice generation process from a sale order
    /// (SRP compliance - separates orchestration from data access)
    /// </summary>
    public class InvoiceGeneratorService : IInvoiceGeneratorService
    {
        private readonly AppDbContext _context;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IInvoiceItemRepository _invoiceItemRepo;
        private readonly IInvoicePaymentRepository _invoicePaymentRepo;
        private readonly ISaleOrderRepository _saleOrderRepo;
        private readonly ISaleOrderItemRepository _saleOrderItemRepo;
        private readonly IJewelleryItemRepository _jewelleryItemRepo;
        private readonly IItemStoneRepository _itemStoneRepo;
        private readonly IMetalRepository _metalRepo;
        private readonly IPurityRepository _purityRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IExchangeRepository _exchangeRepo;
        private readonly IItemStockService _itemStockService;
        private readonly IUserRepository _userRepo;
        
        // Existing services to use
        private readonly IInvoiceBuilderService _invoiceBuilder;
        private readonly IInvoiceTaxService _taxService;
        private readonly IInvoiceNumberService _numberService;
        
        // New services
        private readonly ICompanyDetailsProvider _companyProvider;
        private readonly INumberToWordsConverter _numberConverter;
        private readonly ICurrentUser _currentUser;
        private readonly IInvoiceSettlementService _invoiceSettlementService;
        
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceGeneratorService> _logger;

        /// <summary>
        /// Status ID for cancelled invoices
        /// </summary>
        private const int CancelledStatusId = 4;

        public InvoiceGeneratorService(
            AppDbContext context,
            IInvoiceRepository invoiceRepo,
            IInvoiceItemRepository invoiceItemRepo,
            IInvoicePaymentRepository invoicePaymentRepo,
            ISaleOrderRepository saleOrderRepo,
            ISaleOrderItemRepository saleOrderItemRepo,
            IJewelleryItemRepository jewelleryItemRepo,
            IItemStoneRepository itemStoneRepo,
            IMetalRepository metalRepo,
            IPurityRepository purityRepo,
            IPaymentRepository paymentRepo,
            IExchangeRepository exchangeRepo,
            IItemStockService itemStockService,
            IUserRepository userRepo,
            IInvoiceBuilderService invoiceBuilder,
            IInvoiceTaxService taxService,
            IInvoiceNumberService numberService,
            ICompanyDetailsProvider companyProvider,
            INumberToWordsConverter numberConverter,
            ICurrentUser currentUser,
            IInvoiceSettlementService invoiceSettlementService,
            IMapper mapper,
            ILogger<InvoiceGeneratorService> logger)
        {
            _context = context;
            _invoiceRepo = invoiceRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _invoicePaymentRepo = invoicePaymentRepo;
            _saleOrderRepo = saleOrderRepo;
            _saleOrderItemRepo = saleOrderItemRepo;
            _jewelleryItemRepo = jewelleryItemRepo;
            _itemStoneRepo = itemStoneRepo;
            _metalRepo = metalRepo;
            _purityRepo = purityRepo;
            _paymentRepo = paymentRepo;
            _exchangeRepo = exchangeRepo;
            _itemStockService = itemStockService;
            _userRepo = userRepo;
            _invoiceBuilder = invoiceBuilder;
            _taxService = taxService;
            _numberService = numberService;
            _companyProvider = companyProvider;
            _numberConverter = numberConverter;
            _currentUser = currentUser;
            _invoiceSettlementService = invoiceSettlementService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Invoice> GenerateInvoiceFromSaleOrderAsync(
            long saleOrderId,
            bool includeTermsAndConditions,
            string? notes)
        {
            _logger.LogInformation("Generating invoice for Sale Order {SaleOrderId}", saleOrderId);

            var existingInvoice = await _invoiceSettlementService.RefreshSaleInvoicePaymentsAsync(saleOrderId);
            if (existingInvoice != null)
            {
                _logger.LogInformation(
                    "Reusing existing invoice {InvoiceNumber} for Sale Order {SaleOrderId}",
                    existingInvoice.InvoiceNumber,
                    saleOrderId);
                return existingInvoice;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Fetch sale order using existing repository
                var saleOrder = await _saleOrderRepo.GetSaleOrderByIdAsync((int)saleOrderId);
                if (saleOrder == null)
                {
                    throw new InvalidOperationException($"Sale Order with ID {saleOrderId} not found");
                }

                // 2. Fetch customer using existing repository
                var customer = await _userRepo.GetUserByIdAsync(saleOrder.CustomerId);

                // 3. Fetch sale order items with jewellery details using repository
                var saleOrderItems = await _saleOrderItemRepo.GetSaleOrderItemsWithJewelleryBySaleOrderIdAsync(saleOrderId);

                if (!saleOrderItems.Any())
                {
                    throw new InvalidOperationException("No items found in sale order");
                }

                // 4. Get jewellery item details using repository
                var jewelleryItemIds = saleOrderItems.Select(soi => soi.JewelleryItemId).Distinct().ToList();
                var jewelleryItems = await _jewelleryItemRepo.GetJewelleryItemsByIdsAsync(jewelleryItemIds);

                // 5. Get item stones using repository
                var itemStones = await _itemStoneRepo.GetItemStonesByJewelleryItemIdsAsync(jewelleryItemIds);

                // 6. Get metal and purity details using repositories
                var metalIds = jewelleryItems.Values.Select(ji => ji.MetalId).Distinct().ToList();
                var purityIds = jewelleryItems.Values.Select(ji => ji.PurityId).Distinct().ToList();
                var metals = await _metalRepo.GetMetalsByIdsAsync(metalIds);
                var purities = await _purityRepo.GetPuritiesByIdsAsync(purityIds);

                // 7. Get payments using repository
                var payments = await _paymentRepo.GetPaymentsByOrderIdAndTypeAsync(saleOrderId, TransactionType.SALE);

                // 8. Generate invoice number using EXISTING service
                var invoiceNumber = await _numberService.GenerateNextInvoiceNumberAsync();

                // 9. Build invoice items using EXISTING service
                var invoiceItems = _invoiceBuilder.BuildInvoiceItems(
                    saleOrderItems, jewelleryItems, itemStones, metals, purities);

                // 10. Calculate totals using EXISTING service
                var totals = _taxService.CalculateInvoiceTotals(invoiceItems);
                var exchangeSummary = await BuildExchangeInvoiceSummaryAsync(saleOrder, totals.GrandTotal);

                // 11. Get company details from NEW configuration provider
                var companyDetails = _companyProvider.GetCompanyDetails();

                // 12. Build invoice entity
                var invoice = new Invoice
                {
                    InvoiceNumber = invoiceNumber,
                    InvoiceDate = DateTime.UtcNow,
                    InvoiceType = TransactionType.SALE.ToString(),
                    SaleOrderId = saleOrderId,

                    // Company Details (from configuration)
                    CompanyName = companyDetails.Name,
                    CompanyAddress = companyDetails.Address,
                    CompanyPhone = companyDetails.Phone,
                    CompanyEmail = companyDetails.Email,
                    CompanyGSTIN = companyDetails.GSTIN,
                    CompanyPAN = companyDetails.PAN,
                    CompanyHallmarkLicense = companyDetails.HallmarkLicense,

                    // Party (Customer) Details
                    PartyId = saleOrder.CustomerId,
                    PartyType = PartyType.CUSTOMER,
                    PartyName = customer?.Name ?? "Walk-in Customer",
                    PartyAddress = customer?.Address,
                    PartyEmail = customer?.Email,

                    // Pricing
                    SubTotal = totals.SubTotal,
                    DiscountAmount = totals.DiscountAmount,
                    TaxableAmount = totals.TaxableAmount,

                    // Metal GST (3% on metal value)
                    CGSTAmount = totals.CGSTAmount,
                    SGSTAmount = totals.SGSTAmount,
                    IGSTAmount = totals.IGSTAmount,

                    // Making Charges GST (5% on making charges)
                    MakingChargesCGSTAmount = totals.MakingChargesCGSTAmount,
                    MakingChargesSGSTAmount = totals.MakingChargesSGSTAmount,
                    MakingChargesIGSTAmount = totals.MakingChargesIGSTAmount,
                    MakingChargesGSTAmount = totals.MakingChargesGSTAmount,

                    TotalGSTAmount = totals.TotalGSTAmount,
                    GrandTotal = totals.GrandTotal,
                    RoundOff = Math.Round(totals.GrandTotal) - totals.GrandTotal,
                    GrandTotalInWords = _numberConverter.Convert(Math.Round(totals.GrandTotal)),
                    ExchangeCreditApplied = exchangeSummary.ExchangeCreditApplied,
                    NetAmountPayable = exchangeSummary.NetAmountPayable,

                    // Jewellery-specific totals
                    TotalGoldWeight = invoiceItems.Sum(i => i.NetMetalWeight ?? 0),
                    TotalStoneWeight = invoiceItems.Where(i => i.StoneWeight.HasValue).Sum(i => i.StoneWeight ?? 0),
                    TotalPieces = invoiceItems.Count,
                    IsHallmarked = invoiceItems.Any(i => i.IsHallmarked),
                    BISHallmarkNumber = invoiceItems.Any(i => i.IsHallmarked) ? companyDetails.HallmarkLicense : null,

                    // Footer (from configuration)
                    TermsAndConditions = includeTermsAndConditions ? _companyProvider.GetTermsAndConditions() : null,
                    ReturnPolicy = includeTermsAndConditions ? _companyProvider.GetReturnPolicy() : null,
                    Declaration = includeTermsAndConditions ? _companyProvider.GetDeclaration() : null,
                    Notes = notes,

                    // Audit fields
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin,
                    StatusId = 1
                };

                // 13. Calculate payment allocation
                decimal totalPaid = 0;
                var remaining = invoice.NetAmountPayable;
                var invoicePayments = new List<InvoicePayment>();
                foreach (var payment in payments)
                {
                    if (remaining <= 0) break;
                    var alloc = Math.Min(payment.Amount, remaining);
                    invoicePayments.Add(new InvoicePayment
                    {
                        PaymentId = payment.Id,
                        AllocatedAmount = alloc,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin
                    });
                    remaining -= alloc;
                    totalPaid += alloc;
                }
                invoice.TotalPaid = totalPaid;
                invoice.BalanceDue = Math.Max(0, invoice.NetAmountPayable - totalPaid);

                // 14. Persist using simplified repositories
                var savedInvoice = await _invoiceRepo.AddInvoiceAsync(invoice);

                // 15. Save invoice items
                foreach (var item in invoiceItems)
                {
                    item.InvoiceId = savedInvoice.Id;
                }
                await _invoiceItemRepo.AddInvoiceItemsRangeAsync(invoiceItems);

                // 16. Save invoice payments
                foreach (var payment in invoicePayments)
                {
                    payment.InvoiceId = savedInvoice.Id;
                }
                await _invoicePaymentRepo.AddInvoicePaymentsRangeAsync(invoicePayments);

                // 17. Deduct stock using EXISTING service
                foreach (var saleOrderItem in saleOrderItems)
                {
                    var deducted = await _itemStockService.DeductStockAsync(saleOrderItem.JewelleryItemId, saleOrderItem.Quantity);
                    if (!deducted)
                    {
                        _logger.LogWarning("Failed to deduct stock for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}",
                            saleOrderItem.JewelleryItemId, saleOrderItem.Quantity);
                    }
                }

                await transaction.CommitAsync();

                _logger.LogInformation("Invoice {InvoiceNumber} generated successfully", invoice.InvoiceNumber);
                
                // Return invoice with items and payments populated
                savedInvoice.InvoiceItems = invoiceItems;
                savedInvoice.InvoicePayments = invoicePayments;
                return savedInvoice;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error generating invoice for Sale Order {SaleOrderId}", saleOrderId);
                throw;
            }
        }

        public async Task<bool> CancelInvoiceAsync(string invoiceNumber)
        {
            _logger.LogInformation("Invoice {InvoiceNumber} cancellation requested", invoiceNumber);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = await _invoiceRepo.GetInvoiceByInvoiceNumberAsync(invoiceNumber);

                if (invoice == null)
                {
                    _logger.LogWarning("Invoice {InvoiceNumber} not found for cancellation", invoiceNumber);
                    return false;
                }

                // Get sale order items to restore stock using repository
                if (invoice.SaleOrderId.HasValue)
                {
                    var saleOrderItems = await _saleOrderItemRepo.GetSaleOrderItemsWithJewelleryBySaleOrderIdAsync(invoice.SaleOrderId.Value);

                    // Restore stock for each item using EXISTING service
                    foreach (var item in saleOrderItems)
                    {
                        var restored = await _itemStockService.RestoreStockAsync(item.JewelleryItemId, item.Quantity);
                        if (!restored)
                        {
                            _logger.LogWarning("Failed to restore stock for JewelleryItemId: {JewelleryItemId}, Quantity: {Quantity}",
                                item.JewelleryItemId, item.Quantity);
                        }
                    }
                }

                // Update invoice status using repository
                invoice.StatusId = CancelledStatusId;
                invoice.UpdatedDate = DateTime.UtcNow;
                invoice.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
                await _invoiceRepo.UpdateInvoiceAsync(invoice);

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

        private async Task<(decimal? ExchangeCreditApplied, decimal NetAmountPayable)> BuildExchangeInvoiceSummaryAsync(
            SaleOrder saleOrder,
            decimal grandTotal)
        {
            if (!saleOrder.IsExchangeSale || !saleOrder.ExchangeOrderId.HasValue)
            {
                return (null, grandTotal);
            }

            var exchangeOrder = await _exchangeRepo.GetExchangeOrderByIdAsync(saleOrder.ExchangeOrderId.Value);
            if (exchangeOrder == null)
            {
                throw new InvalidOperationException(
                    $"Exchange Order with ID {saleOrder.ExchangeOrderId.Value} not found for Sale Order {saleOrder.Id}");
            }

            var exchangeCreditApplied = Math.Min(exchangeOrder.TotalCreditAmount, grandTotal);
            var netAmountPayable = Math.Max(0, grandTotal - exchangeCreditApplied);

            return (exchangeCreditApplied, netAmountPayable);
        }
    }
}
