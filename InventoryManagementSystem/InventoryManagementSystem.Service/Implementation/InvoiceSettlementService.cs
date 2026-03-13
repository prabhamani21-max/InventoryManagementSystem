using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Enums;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Rebuilds invoice payment allocations from sale-order payments.
    /// </summary>
    public class InvoiceSettlementService : IInvoiceSettlementService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoicePaymentRepository _invoicePaymentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<InvoiceSettlementService> _logger;

        public InvoiceSettlementService(
            IInvoiceRepository invoiceRepository,
            IInvoicePaymentRepository invoicePaymentRepository,
            IPaymentRepository paymentRepository,
            ICurrentUser currentUser,
            ILogger<InvoiceSettlementService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _invoicePaymentRepository = invoicePaymentRepository;
            _paymentRepository = paymentRepository;
            _currentUser = currentUser;
            _logger = logger;
        }

        public Task<Invoice?> GetActiveSaleInvoiceAsync(long saleOrderId)
        {
            return _invoiceRepository.GetActiveInvoiceBySaleOrderIdAsync(saleOrderId);
        }

        public async Task<Invoice?> RefreshSaleInvoicePaymentsAsync(long saleOrderId)
        {
            var invoice = await _invoiceRepository.GetActiveInvoiceBySaleOrderIdAsync(saleOrderId);
            if (invoice == null)
            {
                return null;
            }

            await RebuildPaymentAllocationsAsync(invoice);
            return invoice;
        }

        private async Task RebuildPaymentAllocationsAsync(Invoice invoice)
        {
            if (!invoice.SaleOrderId.HasValue)
            {
                return;
            }

            var payments = await _paymentRepository.GetPaymentsByOrderIdAndTypeAsync(
                invoice.SaleOrderId.Value,
                TransactionType.SALE);

            var orderedPayments = payments
                .OrderBy(payment => payment.PaymentDate)
                .ThenBy(payment => payment.Id)
                .ToList();

            await _invoicePaymentRepository.DeleteInvoicePaymentsByInvoiceIdAsync(invoice.Id);

            var invoicePayments = new List<InvoicePayment>();
            var remaining = Math.Max(0, invoice.NetAmountPayable);
            decimal totalPaid = 0;
            var auditUserId = (_currentUser?.UserId > 0) ? _currentUser.UserId : invoice.CreatedBy;
            var now = DateTime.UtcNow;

            foreach (var payment in orderedPayments)
            {
                if (remaining <= 0)
                {
                    break;
                }

                var allocatedAmount = Math.Min(payment.Amount, remaining);
                if (allocatedAmount <= 0)
                {
                    continue;
                }

                invoicePayments.Add(new InvoicePayment
                {
                    InvoiceId = invoice.Id,
                    PaymentId = payment.Id,
                    PaymentDate = payment.PaymentDate,
                    AllocatedAmount = allocatedAmount,
                    CreatedDate = now,
                    CreatedBy = auditUserId
                });

                remaining -= allocatedAmount;
                totalPaid += allocatedAmount;
            }

            invoice.TotalPaid = totalPaid;
            invoice.BalanceDue = Math.Max(0, invoice.NetAmountPayable - totalPaid);
            invoice.UpdatedDate = now;
            invoice.UpdatedBy = auditUserId;

            await _invoiceRepository.UpdateInvoiceAsync(invoice);

            if (invoicePayments.Count > 0)
            {
                await _invoicePaymentRepository.AddInvoicePaymentsRangeAsync(invoicePayments);
            }

            invoice.InvoicePayments = invoicePayments;

            _logger.LogInformation(
                "Reconciled invoice {InvoiceNumber} for SaleOrderId {SaleOrderId}: paid {TotalPaid}, balance {BalanceDue}",
                invoice.InvoiceNumber,
                invoice.SaleOrderId,
                invoice.TotalPaid,
                invoice.BalanceDue);
        }
    }
}
