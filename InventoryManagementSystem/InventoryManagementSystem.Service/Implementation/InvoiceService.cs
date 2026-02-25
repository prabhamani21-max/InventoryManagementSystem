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
    /// Orchestrates invoice generation using the repository
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ILogger<InvoiceService> logger,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
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

            // Create minimal invoice object - repository handles all business logic
            var invoice = new Invoice
            {
                SaleOrderId = request.SaleOrderId,
                IncludeTermsAndConditions = request.IncludeTermsAndConditions,
                Notes = request.Notes
            };

            // Generate invoice using repository (transaction and all business logic handled inside)
            var createdInvoice = await _invoiceRepository.GenerateInvoiceAsync(invoice);

            _logger.LogInformation("Invoice {InvoiceNumber} generated successfully", createdInvoice.InvoiceNumber);
            return MapToResponseDto(createdInvoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            var invoice = await _invoiceRepository.GetInvoiceByInvoiceNumberAsync(invoiceNumber);
            return invoice == null ? null : MapToResponseDto(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceBySaleOrderIdAsync(long saleOrderId)
        {
            var invoice = await _invoiceRepository.GetInvoiceBySaleOrderIdAsync(saleOrderId);
            return invoice == null ? null : MapToResponseDto(invoice);
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
            var invoice = await _invoiceRepository.GetInvoiceByInvoiceNumberAsync(invoiceNumber);
            if (invoice == null) return null;

            invoice.Notes = notes ?? invoice.Notes;
            var updatedInvoice = await _invoiceRepository.UpdateInvoiceAsync(invoice);
            return MapToResponseDto(updatedInvoice);
        }

        public async Task<bool> CancelInvoiceAsync(string invoiceNumber)
        {
            _logger.LogInformation("Invoice {InvoiceNumber} cancellation requested", invoiceNumber);
            return await _invoiceRepository.CancelInvoiceAsync(invoiceNumber);
        }

        public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
        {
            _logger.LogInformation("Fetching all invoices");
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return invoices.Select(invoice => MapToResponseDto(invoice)).ToList();
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

            // Crores
            var crores = rupees / 10000000;
            rupees %= 10000000;
            if (crores > 0)
            {
                words += NumberToWordsUnderCrore((long)crores) + " Crore ";
            }

            // Lakhs
            var lakhs = rupees / 100000;
            rupees %= 100000;
            if (lakhs > 0)
            {
                words += NumberToWordsUnderCrore((long)lakhs) + " Lakh ";
            }

            // Thousands
            var thousands = rupees / 1000;
            rupees %= 1000;
            if (thousands > 0)
            {
                words += NumberToWordsUnderCrore((long)thousands) + " Thousand ";
            }

            // Hundreds
            var hundreds = rupees / 100;
            rupees %= 100;
            if (hundreds > 0)
            {
                words += units[hundreds] + " Hundred ";
            }

            // Tens and units
            if (rupees >= 20)
            {
                words += tens[rupees / 10] + " " + units[rupees % 10];
            }
            else if (rupees > 0)
            {
                words += teens[rupees];
            }

            words += "Rupees";

            if (paise > 0)
            {
                words += " and " + teens[paise] + " Paise";
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
            else if (number > 0)
            {
                var teensLocal = new[] { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                words += teensLocal[number];
            }

            return words;
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
