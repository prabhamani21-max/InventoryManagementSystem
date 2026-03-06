using AutoMapper;
using InventoryManagementSytem.Common.Dtos;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Invoice Service Implementation
    /// Delegates invoice generation to IInvoiceGeneratorService (SRP compliance)
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceGeneratorService _generatorService;
        private readonly IInvoiceRepository _invoiceRepo;
        private readonly IInvoiceItemRepository _invoiceItemRepo;
        private readonly IInvoicePaymentRepository _invoicePaymentRepo;
        private readonly INumberToWordsConverter _numberConverter;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceGeneratorService generatorService,
            IInvoiceRepository invoiceRepo,
            IInvoiceItemRepository invoiceItemRepo,
            IInvoicePaymentRepository invoicePaymentRepo,
            INumberToWordsConverter numberConverter,
            ILogger<InvoiceService> logger,
            IMapper mapper)
        {
            _generatorService = generatorService;
            _invoiceRepo = invoiceRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _invoicePaymentRepo = invoicePaymentRepo;
            _numberConverter = numberConverter;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<InvoiceResponseDto> GenerateInvoiceAsync(InvoiceRequestDto request)
        {
            _logger.LogInformation("Generating invoice for Sale Order {SaleOrderId}", request.SaleOrderId);

            // Validate SaleOrderId
            if (request.SaleOrderId <= 0)
            {
                throw new InvalidOperationException("SaleOrderId is required and must be greater than 0");
            }

            // Delegate to generator service
            var invoice = await _generatorService.GenerateInvoiceFromSaleOrderAsync(
                request.SaleOrderId,
                request.IncludeTermsAndConditions,
                request.Notes);

            _logger.LogInformation("Invoice {InvoiceNumber} generated successfully", invoice.InvoiceNumber);
            return MapToResponseDto(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            var invoice = await _invoiceRepo.GetInvoiceByInvoiceNumberAsync(invoiceNumber);
            if (invoice == null) return null;
            
            // Fetch related items and payments
            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();
            
            return MapToResponseDto(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceBySaleOrderIdAsync(long saleOrderId)
        {
            var invoice = await _invoiceRepo.GetInvoiceBySaleOrderIdAsync(saleOrderId);
            if (invoice == null) return null;
            
            // Fetch related items and payments
            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();
            
            return MapToResponseDto(invoice);
        }

        public async Task<BulkInvoiceResponseDto> GenerateBulkInvoicesAsync(BulkInvoiceRequestDto request)
        {
            var response = new BulkInvoiceResponseDto();

            foreach (var saleOrderId in request.SaleOrderIds)
            {
                try
                {
                    var invoiceRequest = new InvoiceRequestDto
                    {
                        SaleOrderId = saleOrderId,
                        IncludePaymentDetails = request.IncludePaymentDetails,
                        IncludeTermsAndConditions = request.IncludeTermsAndConditions
                    };

                    var invoice = await GenerateInvoiceAsync(invoiceRequest);
                    response.Invoices.Add(invoice);
                    response.TotalGenerated++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to generate invoice for Sale Order {SaleOrderId}", saleOrderId);
                    response.TotalFailed++;
                    response.Errors.Add($"Sale Order {saleOrderId}: {ex.Message}");
                }
            }

            return response;
        }

        public async Task<InvoiceResponseDto?> RegenerateInvoiceAsync(string invoiceNumber, string? notes = null)
        {
            var invoice = await _invoiceRepo.GetInvoiceByInvoiceNumberAsync(invoiceNumber);
            if (invoice == null) return null;

            invoice.Notes = notes ?? invoice.Notes;
            var updatedInvoice = await _invoiceRepo.UpdateInvoiceAsync(invoice);
            return MapToResponseDto(updatedInvoice);
        }

        public async Task<bool> CancelInvoiceAsync(string invoiceNumber)
        {
            _logger.LogInformation("Invoice {InvoiceNumber} cancellation requested", invoiceNumber);
            return await _generatorService.CancelInvoiceAsync(invoiceNumber);
        }

        public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
        {
            _logger.LogInformation("Fetching all invoices");
            var invoices = await _invoiceRepo.GetAllInvoicesAsync();
            return invoices.Select(invoice => MapToResponseDto(invoice)).ToList();
        }

        /// <summary>
        /// Convert number to words - delegates to INumberToWordsConverter
        /// </summary>
        public string NumberToWords(decimal number)
        {
            return _numberConverter.Convert(number);
        }

        private InvoiceResponseDto MapToResponseDto(Invoice invoice)
        {
            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceType = invoice.InvoiceType,

                // Company Details
                CompanyName = invoice.CompanyName,
                CompanyAddress = invoice.CompanyAddress,
                CompanyPhone = invoice.CompanyPhone,
                CompanyEmail = invoice.CompanyEmail,
                CompanyGSTIN = invoice.CompanyGSTIN,
                CompanyPAN = invoice.CompanyPAN,

                // Party Details
                PartyId = invoice.PartyId,
                PartyName = invoice.PartyName,
                PartyAddress = invoice.PartyAddress,
                PartyEmail = invoice.PartyEmail,

                // Order Reference
                SaleOrderId = invoice.SaleOrderId,

                // Pricing
                SubTotal = invoice.SubTotal,
                DiscountAmount = invoice.DiscountAmount,
                TaxableAmount = invoice.TaxableAmount,
                CGSTAmount = invoice.CGSTAmount,
                SGSTAmount = invoice.SGSTAmount,
                IGSTAmount = invoice.IGSTAmount,
                TotalGSTAmount = invoice.TotalGSTAmount,
                GrandTotal = invoice.GrandTotal,
                GrandTotalInWords = invoice.GrandTotalInWords,
                RoundOff = invoice.RoundOff,

                // Payment Details
                TotalPaid = invoice.TotalPaid,
                BalanceDue = invoice.BalanceDue,

                // Jewellery-specific
                TotalGoldWeight = invoice.TotalGoldWeight,
                TotalStoneWeight = invoice.TotalStoneWeight,
                TotalPieces = invoice.TotalPieces,

                // Footer
                TermsAndConditions = invoice.TermsAndConditions,
                ReturnPolicy = invoice.ReturnPolicy,
                Notes = invoice.Notes,
                Declaration = invoice.Declaration,

                // Audit
                CreatedDate = invoice.CreatedDate,
                StatusId = invoice.StatusId
            };
        }
    }
}
