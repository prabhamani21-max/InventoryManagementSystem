/**
 * Payment Model
 * Interfaces for Payment management based on backend DTOs
 */

/**
 * Payment Method enum matching backend PaymentMethod enum
 */
export enum PaymentMethod {
  CASH = 'Cash',
  CARD = 'Card',
  UPI = 'UPI',
  BANK_TRANSFER = 'BankTransfer',
  CHEQUE = 'Cheque',
}

/**
 * Transaction Type enum for order type
 */
export enum TransactionType {
  PURCHASE = 'PURCHASE',
  SALE = 'SALE',
}

/**
 * Main Payment interface matching PaymentDb from backend
 */
export interface Payment {
  id: number;
  orderId: number | null;
  orderType: TransactionType;
  customerId: number | null;
  salesPersonId: number | null;
  amount: number;
  paymentMethod: PaymentMethod | string;
  paymentDate: Date | string;
  referenceNumber: string | null;
  createdDate: Date | string;
  createdBy: number;
  updatedBy: number | null;
  updatedDate: Date | string | null;
  statusId: number;
}

/**
 * Interface for creating a new payment
 */
export interface PaymentCreate {
  orderId: number | null;
  orderType: TransactionType;
  customerId: number | null;
  salesPersonId: number | null;
  amount: number;
  paymentMethod: PaymentMethod | string;
  paymentDate: Date | string;
  referenceNumber?: string | null;
  statusId: number;
}

/**
 * Interface for updating an existing payment
 */
export interface PaymentUpdate {
  id: number;
  orderId: number | null;
  orderType: TransactionType;
  customerId: number | null;
  salesPersonId: number | null;
  amount: number;
  paymentMethod: PaymentMethod | string;
  paymentDate: Date | string;
  referenceNumber?: string | null;
  statusId: number;
}

/**
 * Helper function to get payment method label
 */
export function getPaymentMethodLabel(method: PaymentMethod | string): string {
  switch (method) {
    case PaymentMethod.CASH:
    case 'Cash':
      return 'Cash';
    case PaymentMethod.CARD:
    case 'Card':
      return 'Card';
    case PaymentMethod.UPI:
    case 'UPI':
      return 'UPI';
    case PaymentMethod.BANK_TRANSFER:
    case 'BankTransfer':
      return 'Bank Transfer';
    case PaymentMethod.CHEQUE:
    case 'Cheque':
      return 'Cheque';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get payment method CSS class
 */
export function getPaymentMethodClass(method: PaymentMethod | string): string {
  switch (method) {
    case PaymentMethod.CASH:
    case 'Cash':
      return 'badge bg-success';
    case PaymentMethod.CARD:
    case 'Card':
      return 'badge bg-primary';
    case PaymentMethod.UPI:
    case 'UPI':
      return 'badge bg-info';
    case PaymentMethod.BANK_TRANSFER:
    case 'BankTransfer':
      return 'badge bg-warning';
    case PaymentMethod.CHEQUE:
    case 'Cheque':
      return 'badge bg-secondary';
    default:
      return 'badge bg-light text-dark';
  }
}

/**
 * Helper function to get order type label
 */
export function getOrderTypeLabel(orderType: TransactionType): string {
  switch (orderType) {
    case TransactionType.PURCHASE:
      return 'Purchase';
    case TransactionType.SALE:
      return 'Sale';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get order type CSS class
 */
export function getOrderTypeClass(orderType: TransactionType): string {
  switch (orderType) {
    case TransactionType.PURCHASE:
      return 'badge bg-info';
    case TransactionType.SALE:
      return 'badge bg-success';
    default:
      return 'badge bg-light text-dark';
  }
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
      return 'Deleted';
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
  }).format(amount);
}

/**
 * Payment method options for dropdowns
 */
export const PAYMENT_METHOD_OPTIONS = [
  { id: PaymentMethod.CASH, name: 'Cash' },
  { id: PaymentMethod.CARD, name: 'Card' },
  { id: PaymentMethod.UPI, name: 'UPI' },
  { id: PaymentMethod.BANK_TRANSFER, name: 'Bank Transfer' },
  { id: PaymentMethod.CHEQUE, name: 'Cheque' },
];

/**
 * Order type options for dropdowns
 */
export const ORDER_TYPE_OPTIONS = [
  { id: TransactionType.PURCHASE, name: 'Purchase' },
  { id: TransactionType.SALE, name: 'Sale' },
];

/**
 * Status options for dropdowns
 */
export const STATUS_OPTIONS = [
  { id: 1, name: 'Active' },
  { id: 2, name: 'Inactive' },
];
