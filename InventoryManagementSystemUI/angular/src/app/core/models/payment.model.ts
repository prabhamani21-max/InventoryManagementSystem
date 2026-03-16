/**
 * Payment Model
 * Interfaces for Payment management based on backend DTOs
 */

/**
 * Payment Method enum matching backend PaymentMethod enum
 * Backend uses int values: CASH=1, CARD=2, UPI=3, BANK_TRANSFER=4, CHEQUE=5
 */
export enum PaymentMethod {
  CASH = 1,
  CARD = 2,
  UPI = 3,
  BANK_TRANSFER = 4,
  CHEQUE = 5,
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
  customerName: string | null;
  salesPersonId: number | null;
  salesPersonName: string | null;
  amount: number;
  paymentMethod: PaymentMethod | string | number;
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
  paymentMethod: PaymentMethod | string | number;
  paymentDate: Date | string;
  referenceNumber?: string | null;
  statusId: number;
  /** The total order amount for validation purposes (used for high-value transaction validation) */
  orderTotal?: number;
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
  paymentMethod: PaymentMethod | string | number;
  paymentDate: Date | string;
  referenceNumber?: string | null;
  statusId: number;
}

/**
 * Helper function to get payment method label
 * Handles both numeric enum values (1-5) and string values
 */
export function getPaymentMethodLabel(method: PaymentMethod | string | number): string {
  const methodNum = typeof method === 'string' ? parseInt(method, 10) : method;
  
  switch (methodNum) {
    case PaymentMethod.CASH:
    case 1:
      return 'Cash';
    case PaymentMethod.CARD:
    case 2:
      return 'Card';
    case PaymentMethod.UPI:
    case 3:
      return 'UPI';
    case PaymentMethod.BANK_TRANSFER:
    case 4:
      return 'Bank Transfer';
    case PaymentMethod.CHEQUE:
    case 5:
      return 'Cheque';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get payment method CSS class
 * Handles both numeric enum values (1-5) and string values
 */
export function getPaymentMethodClass(method: PaymentMethod | string | number): string {
  const methodNum = typeof method === 'string' ? parseInt(method, 10) : method;
  
  switch (methodNum) {
    case PaymentMethod.CASH:
    case 1:
      return 'badge bg-success';
    case PaymentMethod.CARD:
    case 2:
      return 'badge bg-primary';
    case PaymentMethod.UPI:
    case 3:
      return 'badge bg-info';
    case PaymentMethod.BANK_TRANSFER:
    case 4:
      return 'badge bg-warning';
    case PaymentMethod.CHEQUE:
    case 5:
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
