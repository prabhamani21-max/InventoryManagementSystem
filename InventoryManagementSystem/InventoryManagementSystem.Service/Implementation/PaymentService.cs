using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Repository.Interface;
using InventoryManagementSystem.Service.Interface;
using InventoryManagementSytem.Common.Constants;
using InventoryManagementSytem.Common.Exceptions;

namespace InventoryManagementSystem.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUserKycRepository _userKycRepository;

        public PaymentService(IPaymentRepository paymentRepository, IUserKycRepository userKycRepository)
        {
            _paymentRepository = paymentRepository;
            _userKycRepository = userKycRepository;
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
            // Validate high-value transaction business rules
            await ValidateHighValueTransactionAsync(payment);
            
            return await _paymentRepository.CreatePaymentAsync(payment);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            // Validate high-value transaction business rules
            await ValidateHighValueTransactionAsync(payment);
            
            return await _paymentRepository.UpdatePaymentAsync(payment);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            return await _paymentRepository.DeletePaymentAsync(id);
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
    }
}