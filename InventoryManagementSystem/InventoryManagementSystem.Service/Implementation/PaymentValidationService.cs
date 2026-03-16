using System;
using System.Threading.Tasks;
using InventoryManagementSytem.Common.Constants;
using InventoryManagementSytem.Common.Enums;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service.Implementation
{
    /// <summary>
    /// Service for validating payment rules, especially for high-value transactions
    /// </summary>
    public class PaymentValidationService : IPaymentValidationService
    {
        private readonly IUserKycService _userKycService;

        public PaymentValidationService(IUserKycService userKycService)
        {
            _userKycService = userKycService;
        }

        /// <summary>
        /// Validates a payment against business rules for high-value transactions
        /// </summary>
        /// <param name="customerId">The customer ID making the payment</param>
        /// <param name="amount">The payment amount</param>
        /// <param name="paymentMethod">The payment method (CASH, CARD, UPI, etc.)</param>
        /// <param name="orderTotal">The total order amount</param>
        /// <returns>Validation result indicating if payment can proceed</returns>
        public async Task<PaymentValidationResult> ValidatePaymentAsync(
            long customerId,
            decimal amount,
            string paymentMethod,
            decimal orderTotal)
        {
            // Check if this is a high-value transaction
            bool isHighValue = IsHighValueTransaction(orderTotal);

            // If not high-value, all payment methods are allowed
            if (!isHighValue)
            {
                return PaymentValidationResult.Success();
            }

            // For high-value transactions, check KYC status
            var (hasKyc, isVerified) = await GetCustomerKycStatusAsync(customerId);

            // If KYC is not completed or not verified
            if (!hasKyc || !isVerified)
            {
                return PaymentValidationResult.Failure(
                    BusinessRules.KYC_REQUIRED_CODE,
                    BusinessRules.KYC_REQUIRED_MESSAGE,
                    requiresKyc: true
                );
            }

            // Check if payment method is CASH for high-value transaction
            if (IsCashPayment(paymentMethod))
            {
                return PaymentValidationResult.Failure(
                    BusinessRules.CASH_PAYMENT_NOT_ALLOWED_CODE,
                    BusinessRules.CASH_PAYMENT_NOT_ALLOWED_MESSAGE,
                    requiresKyc: false
                );
            }

            // Valid high-value transaction with non-cash payment
            return PaymentValidationResult.Success(
                isHighValueTransaction: true,
                cashPaymentDisabled: true
            );
        }

        /// <summary>
        /// Checks if a transaction qualifies as high-value
        /// </summary>
        /// <param name="amount">The transaction amount</param>
        /// <returns>True if the amount exceeds the high-value threshold</returns>
        public bool IsHighValueTransaction(decimal amount)
        {
            return amount > BusinessRules.HIGH_VALUE_TRANSACTION_THRESHOLD;
        }

        /// <summary>
        /// Gets the KYC status for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>Tuple indicating if KYC exists and is verified</returns>
        public async Task<(bool HasKyc, bool IsVerified)> GetCustomerKycStatusAsync(long customerId)
        {
            var userKyc = await _userKycService.GetUserKycByUserIdAsync(customerId);
            
            if (userKyc == null)
            {
                return (HasKyc: false, IsVerified: false);
            }

            return (HasKyc: true, IsVerified: userKyc.IsVerified);
        }

        /// <summary>
        /// Checks if the payment method is cash
        /// </summary>
        /// <param name="paymentMethod">The payment method string</param>
        /// <returns>True if the payment method is cash</returns>
        private bool IsCashPayment(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
            {
                return false;
            }

            // Compare with enum value
            if (Enum.TryParse<PaymentMethod>(paymentMethod, true, out var method))
            {
                return method == PaymentMethod.CASH;
            }

            // Also compare with string representation
            return string.Equals(paymentMethod, "CASH", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(paymentMethod, "1", StringComparison.OrdinalIgnoreCase);
        }
    }
}
