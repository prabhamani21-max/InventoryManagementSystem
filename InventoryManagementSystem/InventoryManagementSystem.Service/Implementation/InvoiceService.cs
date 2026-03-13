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
        private readonly IPaymentRepository _paymentRepo;
        private readonly ISaleOrderRepository _saleOrderRepo;
        private readonly IExchangeRepository _exchangeRepo;
        private readonly INumberToWordsConverter _numberConverter;
        private readonly IInvoiceSettlementService _invoiceSettlementService;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IMapper _mapper;

        public InvoiceService(
            IInvoiceGeneratorService generatorService,
            IInvoiceRepository invoiceRepo,
            IInvoiceItemRepository invoiceItemRepo,
            IInvoicePaymentRepository invoicePaymentRepo,
            IPaymentRepository paymentRepo,
            ISaleOrderRepository saleOrderRepo,
            IExchangeRepository exchangeRepo,
            INumberToWordsConverter numberConverter,
            IInvoiceSettlementService invoiceSettlementService,
            ILogger<InvoiceService> logger,
            IMapper mapper)
        {
            _generatorService = generatorService;
            _invoiceRepo = invoiceRepo;
            _invoiceItemRepo = invoiceItemRepo;
            _invoicePaymentRepo = invoicePaymentRepo;
            _paymentRepo = paymentRepo;
            _saleOrderRepo = saleOrderRepo;
            _exchangeRepo = exchangeRepo;
            _numberConverter = numberConverter;
            _invoiceSettlementService = invoiceSettlementService;
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

            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();

            _logger.LogInformation("Invoice {InvoiceNumber} generated successfully", invoice.InvoiceNumber);
            return await MapToResponseDtoAsync(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByNumberAsync(string invoiceNumber)
        {
            var invoice = await _invoiceRepo.GetInvoiceByInvoiceNumberAsync(invoiceNumber);
            if (invoice == null) return null;

            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();

            return await MapToResponseDtoAsync(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(long invoiceId)
        {
            var invoice = await _invoiceRepo.GetInvoiceByIdAsync(invoiceId);
            if (invoice == null) return null;

            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();

            return await MapToResponseDtoAsync(invoice);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceBySaleOrderIdAsync(long saleOrderId)
        {
            var invoice = await _invoiceSettlementService.RefreshSaleInvoicePaymentsAsync(saleOrderId)
                ?? await _invoiceRepo.GetInvoiceBySaleOrderIdAsync(saleOrderId);
            if (invoice == null) return null;

            invoice.InvoiceItems = (await _invoiceItemRepo.GetInvoiceItemsByInvoiceIdAsync(invoice.Id)).ToList();
            invoice.InvoicePayments = (await _invoicePaymentRepo.GetInvoicePaymentsByInvoiceIdAsync(invoice.Id)).ToList();

            return await MapToResponseDtoAsync(invoice);
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

        public async Task<List<InvoiceResponseDto>> GetAllInvoicesAsync()
        {
            _logger.LogInformation("Fetching all invoices");
            var invoices = await _invoiceRepo.GetAllInvoicesAsync();
            var response = new List<InvoiceResponseDto>();
            foreach (var invoice in invoices)
            {
                response.Add(await MapToResponseDtoAsync(invoice));
            }

            return response;
        }

        /// <summary>
        /// Get all invoices for a specific customer (party)
        /// </summary>
        /// <param name="partyId">The customer's user ID (party ID)</param>
        /// <returns>List of invoices for the customer</returns>
        public async Task<List<InvoiceResponseDto>> GetInvoicesByPartyIdAsync(long partyId)
        {
            _logger.LogInformation("Fetching invoices for party ID {PartyId}", partyId);
            var invoices = await _invoiceRepo.GetInvoicesByPartyIdAsync(partyId);
            var response = new List<InvoiceResponseDto>();
            foreach (var invoice in invoices)
            {
                response.Add(await MapToResponseDtoAsync(invoice));
            }

            _logger.LogInformation("Found {Count} invoices for party ID {PartyId}", response.Count, partyId);
            return response;
        }

        /// <summary>
        /// Convert number to words - delegates to INumberToWordsConverter
        /// </summary>
        public string NumberToWords(decimal number)
        {
            return _numberConverter.Convert(number);
        }

        private async Task<InvoiceResponseDto> MapToResponseDtoAsync(Invoice invoice)
        {
            var exchangeSummary = await BuildExchangeInvoiceSummaryAsync(invoice);
            var saleOrder = invoice.SaleOrderId.HasValue
                ? await _saleOrderRepo.GetSaleOrderByIdAsync((int)invoice.SaleOrderId.Value)
                : null;
            var itemDtos = BuildInvoiceItemDtos(invoice.InvoiceItems);
            var paymentDtos = await BuildInvoicePaymentDtosAsync(invoice.InvoicePayments);

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
                CompanyHallmarkLicense = invoice.CompanyHallmarkLicense,

                // Party Details
                PartyId = invoice.PartyId,
                PartyType = invoice.PartyType.ToString(),
                PartyName = invoice.PartyName,
                PartyAddress = invoice.PartyAddress,
                PartyPhone = invoice.PartyPhone,
                PartyEmail = invoice.PartyEmail,
                PartyGSTIN = invoice.PartyGSTIN,
                PartyPANNUmber = invoice.PartyPANNUmber,

                // Order Reference
                SaleOrderId = invoice.SaleOrderId,
                PurchaseOrderId = invoice.PurchaseOrderId,
                OrderNumber = saleOrder?.OrderNumber ?? string.Empty,
                OrderDate = saleOrder?.OrderDate ?? invoice.InvoiceDate,
                DeliveryDate = saleOrder?.DeliveryDate,
                Items = itemDtos,
                InvoiceItems = itemDtos,

                // Pricing
                SubTotal = invoice.SubTotal,
                DiscountAmount = invoice.DiscountAmount,
                TaxableAmount = invoice.TaxableAmount,
                CGSTAmount = invoice.CGSTAmount,
                SGSTAmount = invoice.SGSTAmount,
                IGSTAmount = invoice.IGSTAmount,
                MakingChargesCGSTAmount = invoice.MakingChargesCGSTAmount,
                MakingChargesSGSTAmount = invoice.MakingChargesSGSTAmount,
                MakingChargesIGSTAmount = invoice.MakingChargesIGSTAmount,
                MakingChargesGSTAmount = invoice.MakingChargesGSTAmount,
                TotalGSTAmount = invoice.TotalGSTAmount,
                GrandTotal = invoice.GrandTotal,
                GrandTotalInWords = invoice.GrandTotalInWords,
                RoundOff = invoice.RoundOff,
                ExchangeCreditApplied = exchangeSummary.ExchangeCreditApplied,
                NetAmountPayable = exchangeSummary.NetAmountPayable,

                // Payment Details
                TotalPaid = invoice.TotalPaid,
                BalanceDue = exchangeSummary.BalanceDue,
                Payments = paymentDtos,
                InvoicePayments = paymentDtos,

                // Jewellery-specific
                TotalGoldWeight = invoice.TotalGoldWeight,
                TotalStoneWeight = invoice.TotalStoneWeight,
                TotalPieces = invoice.TotalPieces,
                IsHallmarked = invoice.IsHallmarked,
                BISHallmarkNumber = invoice.BISHallmarkNumber,

                // Footer
                IncludeTermsAndConditions = invoice.IncludeTermsAndConditions,
                TermsAndConditions = invoice.TermsAndConditions,
                ReturnPolicy = invoice.ReturnPolicy,
                Notes = invoice.Notes,
                Declaration = invoice.Declaration,

                // Audit
                CreatedDate = invoice.CreatedDate,
                CreatedBy = invoice.CreatedBy,
                UpdatedBy = invoice.UpdatedBy,
                UpdatedDate = invoice.UpdatedDate,
                StatusId = invoice.StatusId,

                // Generated By
                GeneratedAt = invoice.CreatedDate
            };
        }

        private static List<InvoiceItemDto> BuildInvoiceItemDtos(IEnumerable<InvoiceItem>? invoiceItems)
        {
            return invoiceItems?.Select(item => new InvoiceItemDto
            {
                Id = item.Id,
                ReferenceItemId = item.ReferenceItemId,
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                MetalId = item.MetalId,
                PurityId = item.PurityId,
                MetalType = item.MetalType,
                Purity = item.Purity,
                NetMetalWeight = item.NetMetalWeight,
                MetalAmount = item.MetalAmount,
                StoneId = item.StoneId,
                StoneWeight = item.StoneWeight,
                StoneAmount = item.StoneAmount,
                MakingCharges = item.MakingCharges,
                WastageAmount = item.WastageAmount,
                TotalMakingCharges = item.TotalMakingCharges ?? item.MakingCharges,
                ItemSubtotal = item.ItemSubtotal,
                Discount = item.Discount,
                TaxableAmount = item.TaxableAmount,
                GSTAmount = item.GSTAmount,
                TotalGSTAmount = item.TotalGSTAmount,
                TotalAmount = item.TotalAmount,
                IsHallmarked = item.IsHallmarked,
                HallmarkDetails = item.HallmarkDetails,
                HUID = item.HUID
            }).ToList() ?? new List<InvoiceItemDto>();
        }

        private async Task<List<InvoicePaymentDto>> BuildInvoicePaymentDtosAsync(IEnumerable<InvoicePayment>? invoicePayments)
        {
            if (invoicePayments == null)
            {
                return new List<InvoicePaymentDto>();
            }

            var paymentDtos = new List<InvoicePaymentDto>();
            foreach (var invoicePayment in invoicePayments)
            {
                var payment = await _paymentRepo.GetPaymentByIdAsync((int)invoicePayment.PaymentId);
                paymentDtos.Add(new InvoicePaymentDto
                {
                    PaymentId = invoicePayment.PaymentId,
                    PaymentDate = payment?.PaymentDate ?? invoicePayment.PaymentDate,
                    PaymentMethod = payment?.PaymentMethod ?? string.Empty,
                    ReferenceNumber = payment?.ReferenceNumber,
                    Amount = invoicePayment.AllocatedAmount
                });
            }

            return paymentDtos;
        }

        private async Task<(decimal? ExchangeCreditApplied, decimal NetAmountPayable, decimal BalanceDue)> BuildExchangeInvoiceSummaryAsync(
            Invoice invoice)
        {
            if (invoice.ExchangeCreditApplied > 0 || invoice.NetAmountPayable > 0)
            {
                return (
                    invoice.ExchangeCreditApplied,
                    invoice.NetAmountPayable,
                    Math.Max(0, invoice.NetAmountPayable - invoice.TotalPaid));
            }

            if (!invoice.SaleOrderId.HasValue)
            {
                return (null, invoice.GrandTotal, invoice.BalanceDue);
            }

            var saleOrder = await _saleOrderRepo.GetSaleOrderByIdAsync((int)invoice.SaleOrderId.Value);
            if (saleOrder == null || !saleOrder.IsExchangeSale || !saleOrder.ExchangeOrderId.HasValue)
            {
                return (null, invoice.GrandTotal, invoice.BalanceDue);
            }

            var exchangeOrder = await _exchangeRepo.GetExchangeOrderByIdAsync(saleOrder.ExchangeOrderId.Value);
            if (exchangeOrder == null)
            {
                return (null, invoice.GrandTotal, invoice.BalanceDue);
            }

            var exchangeCreditApplied = Math.Min(exchangeOrder.TotalCreditAmount, invoice.GrandTotal);
            var netAmountPayable = Math.Max(0, invoice.GrandTotal - exchangeCreditApplied);
            var balanceDue = Math.Max(0, netAmountPayable - invoice.TotalPaid);

            return (exchangeCreditApplied, netAmountPayable, balanceDue);
        }
    }
}
