using System;

namespace InventoryManagementSytem.Common.Constants
{
    /// <summary>
    /// Business rule constants for the inventory management system
    /// </summary>
    public static class BusinessRules
    {
        /// <summary>
        /// High-value transaction threshold: ₹2,00,000
        /// Transactions above this amount require KYC verification and non-cash payment methods
        /// </summary>
        public const decimal HIGH_VALUE_TRANSACTION_THRESHOLD = 200000m;

        /// <summary>
        /// Error code for cash payment not allowed on high-value transactions
        /// </summary>
        public const string CASH_PAYMENT_NOT_ALLOWED_CODE = "CASH_PAYMENT_NOT_ALLOWED";

        /// <summary>
        /// Message displayed when cash payment is attempted for high-value transactions
        /// </summary>
        public const string CASH_PAYMENT_NOT_ALLOWED_MESSAGE =
            "Cash payment is not allowed for transactions above ₹2,00,000. Please use Card, UPI, Bank Transfer, or Cheque.";

        /// <summary>
        /// Error code for KYC required on high-value transactions
        /// </summary>
        public const string KYC_REQUIRED_CODE = "KYC_REQUIRED";

        /// <summary>
        /// Message displayed when KYC verification is required
        /// </summary>
        public const string KYC_REQUIRED_MESSAGE =
            "KYC verification is mandatory for transactions above ₹2,00,000. Please complete your KYC to proceed.";

        /// <summary>
        /// Error code for KYC not verified
        /// </summary>
        public const string KYC_NOT_VERIFIED_CODE = "KYC_NOT_VERIFIED";

        /// <summary>
        /// Message displayed when KYC is not verified
        /// </summary>
        public const string KYC_NOT_VERIFIED_MESSAGE =
            "Your KYC is not yet verified. Please wait for verification or contact support.";
    }
}
