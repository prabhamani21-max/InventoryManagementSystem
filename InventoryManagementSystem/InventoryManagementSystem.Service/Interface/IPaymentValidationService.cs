using System.Threading.Tasks;

namespace InventoryManagementSystem.Service.Interface
{
    /// <summary>
    /// Result of payment validation for high-value transactions
    /// </summary>
    public class PaymentValidationResult
    {
        /// <summary>
        /// Indicates whether the payment is valid and can proceed
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error code if validation fails
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Indicates if KYC verification is required for this transaction
        /// </summary>
        public bool RequiresKyc { get; set; }

        /// <summary>
        /// Indicates if cash payment is disabled for this transaction
        /// </summary>
        public bool CashPaymentDisabled { get; set; }

        /// <summary>
        /// Indicates if this is a high-value transaction
        /// </summary>
        public bool IsHighValueTransaction { get; set; }

        /// <summary>
        /// Creates a successful validation result
        /// </summary>
        public static PaymentValidationResult Success(bool isHighValueTransaction = false, bool cashPaymentDisabled = false)
        {
            return new PaymentValidationResult
            {
                IsValid = true,
                IsHighValueTransaction = isHighValueTransaction,
                CashPaymentDisabled = cashPaymentDisabled
            };
        }

        /// <summary>
        /// Creates a failed validation result
        /// </summary>
        public static PaymentValidationResult Failure(string errorCode, string errorMessage, bool requiresKyc = false)
        {
            return new PaymentValidationResult
            {
                IsValid = false,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage,
                RequiresKyc = requiresKyc
            };
        }
    }

    /// <summary>
    /// Service for validating payment rules, especially for high-value transactions
    /// </summary>
    public interface IPaymentValidationService
    {
        /// <summary>
        /// Validates a payment against business rules for high-value transactions
        /// </summary>
        /// <param name="customerId">The customer ID making the payment</param>
        /// <param name="amount">The payment amount</param>
        /// <param name="paymentMethod">The payment method (CASH, CARD, UPI, etc.)</param>
        /// <param name="orderTotal">The total order amount</param>
        /// <returns>Validation result indicating if payment can proceed</returns>
        Task<PaymentValidationResult> ValidatePaymentAsync(
            long customerId,
            decimal amount,
            string paymentMethod,
            decimal orderTotal);

        /// <summary>
        /// Checks if a transaction qualifies as high-value
        /// </summary>
        /// <param name="amount">The transaction amount</param>
        /// <returns>True if the amount exceeds the high-value threshold</returns>
        bool IsHighValueTransaction(decimal amount);

        /// <summary>
        /// Gets the KYC status for a customer
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <returns>Tuple indicating if KYC exists and is verified</returns>
        Task<(bool HasKyc, bool IsVerified)> GetCustomerKycStatusAsync(long customerId);
    }
}
