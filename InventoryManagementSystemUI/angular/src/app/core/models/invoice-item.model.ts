/**
 * InvoiceItem Model
 * Interfaces for InvoiceItem management based on backend DTOs
 */

/**
 * Main InvoiceItem interface matching InvoiceItemDto from backend
 */
export interface InvoiceItem {
  id: number;

  /* ---------------- PARENT ---------------- */
  invoiceId: number;
  invoiceNumber?: string; // From navigation property Invoice.InvoiceNumber
  referenceItemId?: number; // SaleOrderItemId / PurchaseOrderItemId

  /* ---------------- ITEM SNAPSHOT ---------------- */
  itemName: string;
  quantity: number;

  /* ---------------- METAL SNAPSHOT ---------------- */
  metalId: number;
  purityId: number;
  metalType?: string;
  purity?: string;
  grossWeight?: number;
  netMetalWeight?: number;
  metalAmount?: number;

  /* ---------------- STONE SNAPSHOT ---------------- */
  stoneId?: number;
  stoneWeight?: number;
  stoneRate?: number;
  stoneAmount?: number;
  stoneType?: string;
  stonePieces?: number;
  stoneDetails?: string;

  /* ---------------- MAKING CHARGES ---------------- */
  makingCharges?: number;
  wastageAmount?: number;
  makingChargesPerGram?: number;
  totalMakingCharges?: number;

  /* ---------------- PRICING ---------------- */
  itemSubtotal: number;
  unitPrice?: number;
  discount: number;
  taxableAmount: number;
  gstPercentage?: number;
  gstAmount: number;

  /* ---------------- GST BREAKDOWN ---------------- */
  cgstAmount: number;
  sgstAmount: number;
  igstAmount: number;
  totalAmount: number;

  /* ---------------- HALLMARK ---------------- */
  isHallmarked: boolean;
  hallmarkDetails?: string;
}

/**
 * Interface for creating a new invoice item
 */
export interface InvoiceItemCreate {
  invoiceId: number;
  referenceItemId?: number;
  itemName: string;
  quantity: number;
  metalId: number;
  purityId: number;
  netMetalWeight?: number;
  metalAmount?: number;
  stoneId?: number;
  stoneWeight?: number;
  stoneRate?: number;
  stoneAmount?: number;
  makingCharges?: number;
  wastageAmount?: number;
  itemSubtotal: number;
  discount: number;
  taxableAmount: number;
  cgstAmount: number;
  sgstAmount: number;
  igstAmount: number;
  gstAmount: number;
  totalAmount: number;
  isHallmarked: boolean;
}

/**
 * Interface for updating an existing invoice item
 */
export interface InvoiceItemUpdate {
  id: number;
  invoiceId: number;
  referenceItemId?: number;
  itemName: string;
  quantity: number;
  metalId: number;
  purityId: number;
  netMetalWeight?: number;
  metalAmount?: number;
  stoneId?: number;
  stoneWeight?: number;
  stoneRate?: number;
  stoneAmount?: number;
  makingCharges?: number;
  wastageAmount?: number;
  itemSubtotal: number;
  discount: number;
  taxableAmount: number;
  cgstAmount: number;
  sgstAmount: number;
  igstAmount: number;
  gstAmount: number;
  totalAmount: number;
  isHallmarked: boolean;
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
 * Helper function to format currency
 */
export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}

/**
 * Helper function to format weight
 */
export function formatWeight(weight: number): string {
  return `${weight.toFixed(3)} g`;
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
 * Helper function to get hallmark label
 */
export function getHallmarkLabel(isHallmarked: boolean): string {
  return isHallmarked ? 'Yes' : 'No';
}

/**
 * Helper function to get hallmark CSS class
 */
export function getHallmarkClass(isHallmarked: boolean): string {
  return isHallmarked ? 'badge bg-success' : 'badge bg-light text-dark';
}

/**
 * Helper function to get stone label
 */
export function getStoneLabel(hasStone: boolean): string {
  return hasStone ? 'Yes' : 'No';
}

/**
 * Helper function to get stone CSS class
 */
export function getStoneClass(hasStone: boolean): string {
  return hasStone ? 'badge bg-info' : 'badge bg-light text-dark';
}
