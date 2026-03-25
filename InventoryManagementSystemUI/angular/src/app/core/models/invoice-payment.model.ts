/**
 * InvoicePayment Model
 * Interfaces for InvoicePayment management based on backend DTOs
 */

/**
 * Payment Method enum matching backend PaymentMethod enum
 */
export enum PaymentMethod {
  Cash = 1,
  Card = 2,
  UPI = 3,
  BankTransfer = 4,
  Cheque = 5
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
  customerName?: string;
  salesPersonId?: number;
  salesPersonName?: string;
  amount: number;
  paymentMethod: PaymentMethod | string | number;
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
  paymentMethod: PaymentMethod | string | number;
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
  paymentMethod: PaymentMethod | string | number;
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
export function getPaymentMethodLabel(method: PaymentMethod | string | number): string {
  const methodNum = typeof method === 'string' ? parseInt(method, 10) : method;

  switch (methodNum) {
    case PaymentMethod.Cash:
    case 1:
      return 'Cash';
    case PaymentMethod.Card:
    case 2:
      return 'Card';
    case PaymentMethod.UPI:
    case 3:
      return 'UPI';
    case PaymentMethod.BankTransfer:
    case 4:
      return 'Bank Transfer';
    case PaymentMethod.Cheque:
    case 5:
      return 'Cheque';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get payment method CSS class
 */
export function getPaymentMethodClass(method: PaymentMethod | string | number): string {
  const methodNum = typeof method === 'string' ? parseInt(method, 10) : method;

  switch (methodNum) {
    case PaymentMethod.Cash:
    case 1:
      return 'badge bg-success';
    case PaymentMethod.Card:
    case 2:
      return 'badge bg-info';
    case PaymentMethod.UPI:
    case 3:
      return 'badge bg-warning';
    case PaymentMethod.BankTransfer:
    case 4:
      return 'badge bg-primary';
    case PaymentMethod.Cheque:
    case 5:
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
