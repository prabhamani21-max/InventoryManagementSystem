/**
 * ItemStock Model
 * Interfaces for ItemStock management based on backend DTOs
 */

/**
 * Main ItemStock interface matching ItemStockDto from backend
 */
export interface ItemStock {
  id: number;
  jewelleryItemId: number;
  warehouseId: number;
  quantity: number;
  reservedQuantity: number;
  statusId: number;
}

/**
 * Interface for creating a new item stock
 */
export interface ItemStockCreate {
  jewelleryItemId: number;
  warehouseId: number;
  quantity: number;
  reservedQuantity: number;
  statusId: number;
}

/**
 * Interface for updating an existing item stock
 */
export interface ItemStockUpdate {
  id: number;
  jewelleryItemId: number;
  warehouseId: number;
  quantity: number;
  reservedQuantity: number;
  statusId: number;
}

/**
 * Interface for stock validation request
 */
export interface StockValidationRequest {
  jewelleryItemId: number;
  requestedQuantity: number;
  warehouseId?: number;
}

/**
 * Interface for stock validation error
 */
export interface StockValidationError {
  jewelleryItemId: number;
  itemName: string;
  requestedQuantity: number;
  availableQuantity: number;
  message: string;
}

/**
 * Interface for stock validation result
 */
export interface StockValidationResult {
  isValid: boolean;
  errors: StockValidationError[];
}

/**
 * Interface for stock availability check response
 */
export interface StockAvailabilityResponse {
  jewelleryItemId: number;
  requestedQuantity: number;
  isAvailable: boolean;
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
      return 'OutOfStock';
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
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Helper function to calculate available quantity
 */
export function getAvailableQuantity(itemStock: ItemStock): number {
  return itemStock.quantity - itemStock.reservedQuantity;
}