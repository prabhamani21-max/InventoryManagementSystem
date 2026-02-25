/**
 * Exchange Model
 * Interfaces for Exchange management based on backend DTOs
 */

/**
 * Exchange Type enum matching backend
 */
export enum ExchangeType {
  EXCHANGE = 1,  // Exchange for new purchase
  BUYBACK = 2    // Direct cash buyback
}

/**
 * Helper function to get exchange type label
 */
export function getExchangeTypeLabel(exchangeType: string | ExchangeType): string {
  const type = typeof exchangeType === 'string' ? exchangeType.toUpperCase() : exchangeType;
  switch (type) {
    case 'EXCHANGE':
    case ExchangeType.EXCHANGE:
      return 'Exchange';
    case 'BUYBACK':
    case ExchangeType.BUYBACK:
      return 'Buyback';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get exchange type CSS class
 */
export function getExchangeTypeClass(exchangeType: string | ExchangeType): string {
  const type = typeof exchangeType === 'string' ? exchangeType.toUpperCase() : exchangeType;
  switch (type) {
    case 'EXCHANGE':
    case ExchangeType.EXCHANGE:
      return 'badge bg-primary';
    case 'BUYBACK':
    case ExchangeType.BUYBACK:
      return 'badge bg-info';
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Exchange Item interface matching ExchangeItemDto from backend
 */
export interface ExchangeItem {
  id: number;
  exchangeOrderId: number;
  metalId: number;
  metalName: string | null;
  purityId: number;
  purityName: string | null;
  grossWeight: number;
  netWeight: number;
  purityPercentage: number;
  pureWeight: number;
  currentRatePerGram: number;
  marketValue: number;
  makingChargeDeductionPercent: number;
  wastageDeductionPercent: number;
  totalDeductionPercent: number;
  deductionAmount: number;
  creditAmount: number;
  itemDescription: string | null;
  statusId: number;
}

/**
 * Main Exchange Order interface matching ExchangeOrderDto from backend
 */
export interface ExchangeOrder {
  id: number;
  orderNumber: string;
  customerId: number;
  customerName: string | null;
  exchangeType: string;
  totalGrossWeight: number;
  totalNetWeight: number;
  totalPureWeight: number;
  totalMarketValue: number;
  totalDeductionAmount: number;
  totalCreditAmount: number;
  newPurchaseAmount: number | null;
  balanceRefund: number | null;
  cashPayment: number | null;
  statusId: number;
  statusName: string | null;
  notes: string | null;
  exchangeDate: Date | string;
  createdDate: Date | string;
  items: ExchangeItem[];
}

/**
 * Interface for exchange item input (for create/calculate)
 */
export interface ExchangeItemInput {
  metalId: number;
  purityId: number;
  grossWeight: number;
  netWeight: number;
  makingChargeDeductionPercent: number;
  wastageDeductionPercent: number;
  itemDescription?: string | null;
}

/**
 * Interface for calculating exchange value request
 */
export interface ExchangeCalculateRequest {
  customerId: number;
  exchangeType: ExchangeType;
  items: ExchangeItemInput[];
  newPurchaseAmount?: number | null;
  notes?: string | null;
}

/**
 * Interface for exchange item response (calculated)
 */
export interface ExchangeItemResponse {
  metalId: number;
  metalName: string | null;
  purityId: number;
  purityName: string | null;
  grossWeight: number;
  netWeight: number;
  purityPercentage: number;
  pureWeight: number;
  currentRatePerGram: number;
  marketValue: number;
  makingChargeDeductionPercent: number;
  wastageDeductionPercent: number;
  totalDeductionPercent: number;
  deductionAmount: number;
  creditAmount: number;
}

/**
 * Interface for exchange calculation response
 */
export interface ExchangeCalculateResponse {
  customerId: number;
  exchangeType: string;
  itemCount: number;
  totalGrossWeight: number;
  totalNetWeight: number;
  totalPureWeight: number;
  totalMarketValue: number;
  totalDeductionAmount: number;
  totalCreditAmount: number;
  newPurchaseAmount: number | null;
  balanceRefund: number | null;
  cashPayment: number | null;
  itemDetails: ExchangeItemResponse[];
}

/**
 * Interface for creating a new exchange order
 */
export interface ExchangeOrderCreate {
  customerId: number;
  exchangeType: ExchangeType;
  items: ExchangeItemInput[];
  newPurchaseAmount?: number | null;
  notes?: string | null;
}

/**
 * Interface for completing an exchange order
 */
export interface ExchangeCompleteRequest {
  exchangeOrderId: number;
  notes?: string | null;
}

/**
 * Interface for cancel request
 */
export interface ExchangeCancelRequest {
  reason?: string | null;
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
export function formatCurrency(amount: number | null | undefined): string {
  if (amount === null || amount === undefined) return '-';
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}

/**
 * Helper function to format weight for display
 */
export function formatWeight(weight: number | null | undefined): string {
  if (weight === null || weight === undefined) return '-';
  return `${weight.toFixed(3)}g`;
}