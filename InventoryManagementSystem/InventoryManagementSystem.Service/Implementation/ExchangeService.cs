using InventoryManagementSystem.Common.Enum;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Dtos;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRepository _exchangeRepository;
        private readonly ISaleOrderService _saleOrderService;
        private readonly IInvoiceService _invoiceService;
        private readonly IExchangeValuationService _exchangeValuationService;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<ExchangeService> _logger;

        public ExchangeService(
            IExchangeRepository exchangeRepository,
            ISaleOrderService saleOrderService,
            IInvoiceService invoiceService,
            IExchangeValuationService exchangeValuationService,
            ICurrentUser currentUser,
            ILogger<ExchangeService> logger)
        {
            _exchangeRepository = exchangeRepository;
            _saleOrderService = saleOrderService;
            _invoiceService = invoiceService;
            _exchangeValuationService = exchangeValuationService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ExchangeCalculationResult> CalculateExchangeValueAsync(ExchangeCalculationRequest request)
        {
            _logger.LogInformation("Calculating exchange value for customer {CustomerId}", request.CustomerId);
            EnsureExchangeOnly(request.ExchangeType);
            return await _exchangeValuationService.CalculateAsync(request);
        }

        public async Task<ExchangeOrder> CreateExchangeOrderAsync(ExchangeOrder request)
        {
            _logger.LogInformation("Creating exchange order for customer {CustomerId}", request.CustomerId);
            EnsureExchangeOnly(request.ExchangeType);

            if (request.Items == null || request.Items.Count == 0)
            {
                throw new ArgumentException("At least one exchange item is required.");
            }

            var valuation = await _exchangeValuationService.CalculateAsync(BuildCalculationRequest(request));
            ApplyValuationSnapshot(request, valuation);

            // Generate order number
            var orderNumber = $"EXC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            request.OrderNumber = orderNumber;

            // Set audit fields
            var currentUserId = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            var now = DateTime.UtcNow;
            request.ExchangeDate = now;
            if (request.CreatedDate == default)
            {
                request.CreatedDate = now;
            }

            if (request.CreatedBy <= 0)
            {
                request.CreatedBy = currentUserId;
            }
            
            // Set default status to PENDING
            if (request.StatusId == 0)
            {
                request.StatusId = (int)StatusEnum.Active;
            }

            // Set audit fields for items
            foreach (var item in request.Items)
            {
                item.CreatedDate = now;
                item.CreatedBy = currentUserId;
            }

            var createdOrder = await _exchangeRepository.CreateExchangeOrderAsync(request);
            _logger.LogInformation("Exchange order {OrderNumber} created successfully", orderNumber);

            return await EnrichExchangeOrderAsync(createdOrder);
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByIdAsync(long id)
        {
            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(id);
            return order == null ? null : await EnrichExchangeOrderAsync(order);
        }

        public async Task<ExchangeOrder?> GetExchangeOrderByOrderNumberAsync(string orderNumber)
        {
            var order = await _exchangeRepository.GetExchangeOrderByOrderNumberAsync(orderNumber);
            return order == null ? null : await EnrichExchangeOrderAsync(order);
        }

        public async Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCustomerIdAsync(long customerId)
        {
            var orders = await _exchangeRepository.GetExchangeOrdersByCustomerIdAsync(customerId);
            return await EnrichExchangeOrdersAsync(orders);
        }

        /// <summary>
        /// Get all exchange orders created by a specific sales person
        /// </summary>
        /// <param name="createdBy">The sales person's user ID</param>
        /// <returns>List of exchange orders created by the sales person</returns>
        public async Task<IEnumerable<ExchangeOrder>> GetExchangeOrdersByCreatedByAsync(long createdBy)
        {
            _logger.LogInformation("Fetching exchange orders created by sales person ID {CreatedBy}", createdBy);
            var orders = await _exchangeRepository.GetExchangeOrdersByCreatedByAsync(createdBy);
            return await EnrichExchangeOrdersAsync(orders);
        }

        public async Task<IEnumerable<ExchangeOrder>> GetAllExchangeOrdersAsync()
        {
            var orders = await _exchangeRepository.GetAllExchangeOrdersAsync();
            return await EnrichExchangeOrdersAsync(orders);
        }

        public async Task<ExchangeOrder> LinkSaleOrderAsync(long exchangeOrderId, long saleOrderId)
        {
            _logger.LogInformation(
                "Linking sale order {SaleOrderId} to exchange order {ExchangeOrderId}",
                saleOrderId,
                exchangeOrderId);

            var exchangeOrder = await _exchangeRepository.GetExchangeOrderByIdAsync(exchangeOrderId);
            if (exchangeOrder == null)
            {
                throw new ArgumentException($"Exchange order {exchangeOrderId} not found");
            }

            EnsureExchangeOnly(exchangeOrder.ExchangeType);
            EnsureExchangeOrderIsOpen(exchangeOrder);

            var saleOrder = await _saleOrderService.GetSaleOrderByIdAsync((int)saleOrderId);
            if (saleOrder == null)
            {
                throw new ArgumentException($"Sale order {saleOrderId} not found");
            }

            if (saleOrder.CustomerId != exchangeOrder.CustomerId)
            {
                throw new InvalidOperationException("Linked sale order must belong to the same customer as the exchange order.");
            }

            if (saleOrder.ExchangeOrderId.HasValue && saleOrder.ExchangeOrderId.Value != exchangeOrderId)
            {
                throw new InvalidOperationException(
                    $"Sale order {saleOrderId} is already linked to exchange order {saleOrder.ExchangeOrderId.Value}.");
            }

            var existingLinkedSale = await _saleOrderService.GetSaleOrderByExchangeOrderIdAsync(exchangeOrderId);
            if (existingLinkedSale != null && existingLinkedSale.Id != saleOrder.Id)
            {
                throw new InvalidOperationException(
                    $"Exchange order {exchangeOrderId} is already linked to sale order {existingLinkedSale.Id}.");
            }

            saleOrder.IsExchangeSale = true;
            saleOrder.ExchangeOrderId = exchangeOrderId;
            saleOrder.UpdatedDate = DateTime.UtcNow;
            saleOrder.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;

            await _saleOrderService.UpdateSaleOrderAsync(saleOrder);

            return await EnrichExchangeOrderAsync(exchangeOrder);
        }

        public async Task<ExchangeOrder> CompleteExchangeOrderAsync(long orderId, string? notes)
        {
            _logger.LogInformation("Completing exchange order {OrderId}", orderId);

            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Exchange order {orderId} not found");
            }

            EnsureExchangeOnly(order.ExchangeType);
            EnsureExchangeOrderIsOpen(order);

            var linkedSaleOrder = await _saleOrderService.GetSaleOrderByExchangeOrderIdAsync(orderId);
            if (linkedSaleOrder == null)
            {
                throw new InvalidOperationException("Exchange order cannot be completed until it is linked to a sale order.");
            }

            if (linkedSaleOrder.CustomerId != order.CustomerId)
            {
                throw new InvalidOperationException("Linked sale order customer does not match the exchange order customer.");
            }

            var invoice = await _invoiceService.GetInvoiceBySaleOrderIdAsync(linkedSaleOrder.Id);
            if (invoice == null)
            {
                throw new InvalidOperationException("Generate an invoice for the linked sale order before completing the exchange.");
            }

            if (invoice.GrandTotal < order.TotalCreditAmount)
            {
                throw new InvalidOperationException("Exchange credit cannot exceed the linked sale total in Phase 1.");
            }

            var remainingCustomerPayment = CalculateRemainingCustomerPayment(order.TotalCreditAmount, invoice);
            if (remainingCustomerPayment > 0)
            {
                throw new InvalidOperationException(
                    $"Customer payment is still pending for the linked sale. Remaining amount: {remainingCustomerPayment:0.##}");
            }

            // Update order status
            order.StatusId = (int)StatusEnum.Inactive;
            order.UpdatedDate = DateTime.UtcNow;
            order.UpdatedBy = (_currentUser?.UserId > 0) ? _currentUser.UserId : (long)SystemUser.SuperAdmin;
            order.Notes = notes ?? order.Notes;

            var updatedOrder = await _exchangeRepository.UpdateExchangeOrderAsync(order);
            _logger.LogInformation("Exchange order {OrderId} completed successfully", orderId);

            return await EnrichExchangeOrderAsync(updatedOrder);
        }

        public async Task<bool> CancelExchangeOrderAsync(long orderId, string? reason)
        {
            _logger.LogInformation("Cancelling exchange order {OrderId}", orderId);

            var order = await _exchangeRepository.GetExchangeOrderByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }

            if (order.StatusId == (int)StatusEnum.Inactive || order.StatusId == (int)StatusEnum.Deleted)
            {
                throw new InvalidOperationException("Exchange order cannot be cancelled in its current status.");
            }

            var result = await _exchangeRepository.CancelExchangeOrderAsync(orderId);
            if (result)
            {
                _logger.LogInformation("Exchange order {OrderId} cancelled successfully", orderId);
            }

            return result;
        }

        private static ExchangeCalculationRequest BuildCalculationRequest(ExchangeOrder request)
        {
            return new ExchangeCalculationRequest
            {
                CustomerId = request.CustomerId,
                ExchangeType = request.ExchangeType,
                Notes = request.Notes,
                Items = request.Items.Select(item => new ExchangeItemInput
                {
                    MetalId = item.MetalId,
                    PurityId = item.PurityId,
                    GrossWeight = item.GrossWeight,
                    NetWeight = item.NetWeight,
                    MakingChargeDeductionPercent = item.MakingChargeDeductionPercent,
                    WastageDeductionPercent = item.WastageDeductionPercent,
                    ItemDescription = item.ItemDescription
                }).ToList()
            };
        }

        private static void EnsureExchangeOnly(int exchangeType)
        {
            if (exchangeType != 1)
            {
                throw new InvalidOperationException("Phase 1 supports only exchange-against-sale transactions.");
            }
        }

        private static void EnsureExchangeOrderIsOpen(ExchangeOrder order)
        {
            if (order.StatusId != (int)StatusEnum.Active)
            {
                throw new InvalidOperationException("Exchange order is not open for this operation.");
            }
        }

        private static decimal CalculateRemainingCustomerPayment(decimal exchangeCredit, InvoiceResponseDto invoice)
        {
            var remaining = invoice.GrandTotal - exchangeCredit - invoice.TotalPaid;
            return remaining > 0 ? remaining : 0;
        }

        private static void ApplyValuationSnapshot(ExchangeOrder request, ExchangeCalculationResult valuation)
        {
            if (valuation.ItemDetails.Count != request.Items.Count)
            {
                throw new InvalidOperationException("Exchange valuation could not be completed for all items.");
            }

            request.TotalGrossWeight = valuation.TotalGrossWeight;
            request.TotalNetWeight = valuation.TotalNetWeight;
            request.TotalPureWeight = valuation.TotalPureWeight;
            request.TotalMarketValue = valuation.TotalMarketValue;
            request.TotalDeductionAmount = valuation.TotalDeductionAmount;
            request.TotalCreditAmount = valuation.TotalCreditAmount;

            for (var index = 0; index < request.Items.Count; index++)
            {
                var source = request.Items.ElementAt(index);
                var calculated = valuation.ItemDetails.ElementAt(index);

                source.PurityPercentage = calculated.PurityPercentage;
                source.PureWeight = calculated.PureWeight;
                source.CurrentRatePerGram = calculated.CurrentRatePerGram;
                source.MarketValue = calculated.MarketValue;
                source.DeductionAmount = calculated.DeductionAmount;
                source.CreditAmount = calculated.CreditAmount;
            }
        }

        private async Task<IEnumerable<ExchangeOrder>> EnrichExchangeOrdersAsync(IEnumerable<ExchangeOrder> orders)
        {
            var enrichedOrders = new List<ExchangeOrder>();
            foreach (var order in orders)
            {
                enrichedOrders.Add(await EnrichExchangeOrderAsync(order));
            }

            return enrichedOrders;
        }

        private async Task<ExchangeOrder> EnrichExchangeOrderAsync(ExchangeOrder order)
        {
            var linkedSaleOrder = await _saleOrderService.GetSaleOrderByExchangeOrderIdAsync(order.Id);
            if (linkedSaleOrder == null)
            {
                return order;
            }

            order.LinkedSaleOrderId = linkedSaleOrder.Id;
            order.LinkedSaleOrderNumber = linkedSaleOrder.OrderNumber;

            var invoice = await _invoiceService.GetInvoiceBySaleOrderIdAsync(linkedSaleOrder.Id);
            if (invoice == null)
            {
                return order;
            }

            order.LinkedInvoiceId = invoice.Id;
            order.LinkedInvoiceNumber = invoice.InvoiceNumber;
            order.LinkedSaleGrandTotal = invoice.GrandTotal;
            order.RemainingCustomerPayment = CalculateRemainingCustomerPayment(order.TotalCreditAmount, invoice);

            return order;
        }
    }
}
