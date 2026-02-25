/**
 * InvoicePayment Model
 * Interfaces for InvoicePayment management based on backend DTOs
 */

/**
 * Payment Method enum matching backend PaymentMethod enum
 */
export enum PaymentMethod {
  Cash = 'Cash',
  Card = 'Card',
  BankTransfer = 'BankTransfer',
  UPI = 'UPI',
  Cheque = 'Cheque',
  Other = 'Other'
}

/**
 * Transaction Type enum matching backend TransactionType enum
 */
export enum TransactionType {
  Purchase = 'PURCHASE',
  Sale = 'SALE'
}

/**
 * Main InvoicePayment interface matching InvoicePayment model from backend
 */
export interface InvoicePayment {
  id: number;
  invoiceId: number;
  paymentId: number;
  allocatedAmount: number;
  paymentDate: Date | string;
  createdDate: Date | string;
  createdBy: number;
}

/**
 * Interface for creating a new invoice payment
 */
export interface InvoicePaymentCreate {
  invoiceId: number;
  paymentId: number;
  allocatedAmount: number;
  paymentDate: Date | string;
}

/**
 * Interface for updating an existing invoice payment
 */
export interface InvoicePaymentUpdate {
  id: number;
  invoiceId: number;
  paymentId: number;
  allocatedAmount: number;
  paymentDate: Date | string;
}

/**
 * Payment interface matching Payment model from backend
 */
export interface Payment {
  id: number;
  orderId?: number;
  orderType: TransactionType;
  customerId?: number;
  salesPersonId?: number;
  amount: number;
  paymentMethod: PaymentMethod;
  paymentDate: Date | string;
  referenceNumber?: string;
  createdDate: Date | string;
  createdBy: number;
  updatedBy?: number;
  updatedDate?: Date | string;
  statusId: number;
}

/**
 * Interface for creating a new payment
 */
export interface PaymentCreate {
  orderId?: number;
  orderType: TransactionType;
  customerId?: number;
  salesPersonId?: number;
  amount: number;
  paymentMethod: PaymentMethod;
  paymentDate: Date | string;
  referenceNumber?: string;
}

/**
 * Interface for updating an existing payment
 */
export interface PaymentUpdate {
  id: number;
  orderId?: number;
  orderType: TransactionType;
  customerId?: number;
  salesPersonId?: number;
  amount: number;
  paymentMethod: PaymentMethod;
  paymentDate: Date | string;
  referenceNumber?: string;
}

/**
 * Invoice Payment DTO from InvoiceDto
 */
export interface InvoicePaymentDto {
  paymentId: number;
  paymentDate: Date | string;
  paymentMethod: string;
  referenceNumber?: string;
  amount: number;
}

/**
 * Helper function to get status label based on status ID
 */
export function getStatusLabel(statusId: number): string {
  switch (statusId) {
    case 1:
      return 'Active';
    case 2:
      return 'Inactive';
    case 3:
      return 'Pending';
    case 4:
      return 'Completed';
    case 5:
      return 'Cancelled';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get status CSS class based on status ID
 */
export function getStatusClass(statusId: number): string {
  switch (statusId) {
    case 1:
      return 'badge bg-success';
    case 2:
      return 'badge bg-danger';
    case 3:
      return 'badge bg-warning';
    case 4:
      return 'badge bg-primary';
    case 5:
      return 'badge bg-secondary';
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Helper function to get payment method label
 */
export function getPaymentMethodLabel(method: PaymentMethod | string): string {
  switch (method) {
    case PaymentMethod.Cash:
    case 'Cash':
      return 'Cash';
    case PaymentMethod.Card:
    case 'Card':
      return 'Card';
    case PaymentMethod.BankTransfer:
    case 'BankTransfer':
      return 'Bank Transfer';
    case PaymentMethod.UPI:
    case 'UPI':
      return 'UPI';
    case PaymentMethod.Cheque:
    case 'Cheque':
      return 'Cheque';
    case PaymentMethod.Other:
    case 'Other':
      return 'Other';
    default:
      return method;
  }
}

/**
 * Helper function to get payment method CSS class
 */
export function getPaymentMethodClass(method: PaymentMethod | string): string {
  switch (method) {
    case PaymentMethod.Cash:
    case 'Cash':
      return 'badge bg-success';
    case PaymentMethod.Card:
    case 'Card':
      return 'badge bg-info';
    case PaymentMethod.BankTransfer:
    case 'BankTransfer':
      return 'badge bg-primary';
    case PaymentMethod.UPI:
    case 'UPI':
      return 'badge bg-warning';
    case PaymentMethod.Cheque:
    case 'Cheque':
      return 'badge bg-secondary';
    default:
      return 'badge bg-light text-dark';
  }
}

/**
 * Helper function to format date for display
 */
export function formatDate(date: Date | string | null): string {
  if (!date) return '-';
  const d = new Date(date);
  return d.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

/**
 * Helper function to format currency for display
 */
export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}
