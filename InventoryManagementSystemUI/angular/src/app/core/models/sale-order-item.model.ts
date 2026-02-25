/**
 * SaleOrderItem Model
 * Interfaces for SaleOrderItem management based on backend DTOs
 */

/**
 * Making Charge Type enum matching backend MakingChargeType
 */
export enum MakingChargeType {
  PER_GRAM = 1,
  PERCENTAGE = 2,
  FIXED = 3,
}

/**
 * Main SaleOrderItem interface matching SaleOrderItem from backend
 */
export interface SaleOrderItem {
  id: number;

  /* ---------------- PARENT ---------------- */
  saleOrderId: number;
  saleOrderNumber?: string; // From navigation property SaleOrder.OrderNumber
  jewelleryItemId: number;

  /* ---------------- ITEM SNAPSHOT ---------------- */
  itemCode?: string;
  itemName: string;
  description?: string;
  quantity: number;

  /* ---------------- METAL SNAPSHOT ---------------- */
  metalId: number;
  purityId: number;
  grossWeight: number;
  netMetalWeight: number;
  metalRatePerGram: number;
  metalAmount: number;

  /* ---------------- MAKING CHARGES ---------------- */
  makingChargeType: MakingChargeType;
  makingChargeValue: number;
  totalMakingCharges: number;
  wastagePercentage: number;
  wastageWeight: number;
  wastageAmount: number;

  /* ---------------- STONE SUMMARY ---------------- */
  hasStone: boolean;
  stoneAmount?: number;

  /* ---------------- PRICE & TAX ---------------- */
  itemSubtotal: number;
  discountAmount: number;
  taxableAmount: number;
  gstPercentage: number;
  gstAmount: number;
  totalAmount: number;

  /* ---------------- HALLMARK ---------------- */
  isHallmarked: boolean;
  huid?: string; // 6-digit alphanumeric Hallmark Unique ID
  bisCertificationNumber?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;

  /* ---------------- AUDIT ---------------- */
  createdDate: Date | string;
  createdBy: number;
  updatedBy?: number;
  updatedDate?: Date | string;
  statusId: number;
}

/**
 * Interface for creating a new sale order item
 */
export interface SaleOrderItemCreate {
  saleOrderId: number;
  jewelleryItemId: number;
  quantity: number;
  discountAmount?: number;
  gstPercentage: number;
  stoneAmount?: number;
}

/**
 * Interface for creating a sale order item with automatic calculation
 * This matches the backend CreateSaleOrderItemWithCalculationAsync method
 */
export interface SaleOrderItemWithCalculation {
  saleOrderId: number;
  jewelleryItemId: number;
  discountAmount?: number;
  gstPercentage: number;
  stoneAmount?: number;
  quantity: number;
}

/**
 * Interface for updating an existing sale order item
 */
export interface SaleOrderItemUpdate {
  id: number;
  saleOrderId: number;
  jewelleryItemId: number;
  quantity: number;
  discountAmount: number;
  gstPercentage: number;
  stoneAmount?: number;
  statusId: number;
}

/**
 * Helper function to get making charge type label
 */
export function getMakingChargeTypeLabel(type: MakingChargeType): string {
  switch (type) {
    case MakingChargeType.PER_GRAM:
      return 'Per Gram';
    case MakingChargeType.PERCENTAGE:
      return 'Percentage';
    case MakingChargeType.FIXED:
      return 'Fixed';
    default:
      return 'Unknown';
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
