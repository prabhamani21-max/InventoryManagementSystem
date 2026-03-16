using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Constants;
using InventoryManagementSytem.Common.Enums;
using InventoryManagementSytem.Common.Exceptions;

namespace InventoryManagementSystem.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserKycRepository _userKycRepository;
        private readonly IInvoiceSettlementService _invoiceSettlementService;

        public PaymentService(
            AppDbContext dbContext,
            IPaymentRepository paymentRepository,
            IUserKycRepository userKycRepository,
            IInvoiceSettlementService invoiceSettlementService)
        {
            _dbContext = dbContext;
            _paymentRepository = paymentRepository;
            _userKycRepository = userKycRepository;
            _invoiceSettlementService = invoiceSettlementService;
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            return await _paymentRepository.GetPaymentByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllPaymentsAsync();
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await ValidateHighValueTransactionAsync(payment);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var createdPayment = await _paymentRepository.CreatePaymentAsync(payment);
                await ReconcileSaleInvoiceAsync(createdPayment);
                await transaction.CommitAsync();
                return createdPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            await ValidateHighValueTransactionAsync(payment);

            var existingPayment = await _paymentRepository.GetPaymentByIdAsync(payment.Id);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var updatedPayment = await _paymentRepository.UpdatePaymentAsync(payment);
                if (updatedPayment == null)
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                await ReconcileSaleInvoiceAsync(existingPayment);

                if (!AreSameSaleOrder(existingPayment, updatedPayment))
                {
                    await ReconcileSaleInvoiceAsync(updatedPayment);
                }

                await transaction.CommitAsync();
                return updatedPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            var existingPayment = await _paymentRepository.GetPaymentByIdAsync(id);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var deleted = await _paymentRepository.DeletePaymentAsync(id);
                if (!deleted)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                await ReconcileSaleInvoiceAsync(existingPayment);
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Validates business rules for high-value transactions (above 2 lakhs)
        /// - KYC must be verified
        /// - Cash payment is not allowed
        /// </summary>
        /// <param name="payment">The payment to validate</param>
        /// <exception cref="BusinessException">Thrown when a business rule is violated</exception>
        private async Task ValidateHighValueTransactionAsync(Payment payment)
        {
            // Only validate for high-value transactions
            if (payment.Amount <= BusinessRules.HIGH_VALUE_TRANSACTION_THRESHOLD)
            {
                return;
            }

            // Skip validation if no customer is associated (e.g., for purchase orders)
            if (!payment.CustomerId.HasValue || payment.CustomerId <= 0)
            {
                return;
            }

            // Rule 1: KYC must be verified for high-value transactions
            var userKyc = await _userKycRepository.GetUserKycByUserIdAsync(payment.CustomerId.Value);
            if (userKyc == null || !userKyc.IsVerified)
            {
                throw new BusinessException(
                    BusinessRules.KYC_REQUIRED_CODE, 
                    BusinessRules.KYC_REQUIRED_MESSAGE);
            }

            // Rule 2: Cash payment is not allowed for high-value transactions
            if (string.Equals(payment.PaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(payment.PaymentMethod, "CASH", StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException(
                    BusinessRules.CASH_PAYMENT_NOT_ALLOWED_CODE, 
                    BusinessRules.CASH_PAYMENT_NOT_ALLOWED_MESSAGE);
            }
        }

        private async Task ReconcileSaleInvoiceAsync(Payment? payment)
        {
            var saleOrderId = GetSaleOrderId(payment);
            if (!saleOrderId.HasValue)
            {
                return;
            }

            await _invoiceSettlementService.RefreshSaleInvoicePaymentsAsync(saleOrderId.Value);
        }

        private static long? GetSaleOrderId(Payment? payment)
        {
            if (payment?.OrderId == null || string.IsNullOrWhiteSpace(payment.OrderType))
            {
                return null;
            }

            if (!Enum.TryParse<TransactionType>(payment.OrderType, true, out var orderType))
            {
                return null;
            }

            return orderType == TransactionType.SALE ? payment.OrderId : null;
        }

        private static bool AreSameSaleOrder(Payment? left, Payment? right)
        {
            return GetSaleOrderId(left) == GetSaleOrderId(right);
        }
    }
}
