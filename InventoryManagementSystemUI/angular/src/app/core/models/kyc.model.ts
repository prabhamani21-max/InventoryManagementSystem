/**
 * KYC Status interface for customer KYC verification
 */
export interface KycStatus {
  /** Whether the customer has submitted KYC */
  hasKyc: boolean;
  /** Whether the KYC is verified */
  isVerified: boolean;
  /** PAN card number (if available) */
  panCardNumber?: string;
  /** Masked Aadhaar card number (if available) */
  aadhaarCardNumber?: string;
}

/**
 * Payment validation request interface
 */
export interface PaymentValidationRequest {
  /** Customer ID */
  customerId: number;
  /** Total order amount */
  orderTotal: number;
}

/**
 * Payment validation response interface
 */
export interface PaymentValidationResponse {
  /** Whether the payment is valid */
  isValid: boolean;
  /** Whether this is a high-value transaction */
  isHighValueTransaction: boolean;
  /** Whether cash payment is disabled */
  cashPaymentDisabled: boolean;
  /** Whether KYC is required */
  requiresKyc?: boolean;
  /** Whether customer has KYC */
  hasKyc?: boolean;
  /** Whether KYC is verified */
  isKycVerified?: boolean;
}

/**
 * High-value transaction threshold constant (₹2,00,000)
 */
export const HIGH_VALUE_TRANSACTION_THRESHOLD = 200000;

/**
 * Error codes for payment validation
 */
export enum PaymentValidationErrorCode {
  CASH_PAYMENT_NOT_ALLOWED = 'CASH_PAYMENT_NOT_ALLOWED',
  KYC_REQUIRED = 'KYC_REQUIRED',
  KYC_NOT_VERIFIED = 'KYC_NOT_VERIFIED',
}

/**
 * Check if order total qualifies as high-value transaction
 */
export function isHighValueTransaction(amount: number): boolean {
  return amount > HIGH_VALUE_TRANSACTION_THRESHOLD;
}
