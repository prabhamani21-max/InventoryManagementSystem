/**
 * JewelleryItem Model
 * Interfaces for JewelleryItem management based on backend DTOs
 */

/**
 * Making Charge Type enum matching backend enum
 */
export enum MakingChargeType {
  PER_GRAM = 1,
  PERCENTAGE = 2,
  FIXED = 3
}

/**
 * Main JewelleryItem interface matching JewelleryItemDto from backend
 */
export interface JewelleryItem {
  id: number;
  itemCode?: string;
  name: string;
  description: string;
  categoryId: number;
  hasStone: boolean;
  stoneId?: number;
  makingChargeType: MakingChargeType;
  makingChargeValue: number;
  wastagePercentage: number;
  isHallmarked: boolean;
  // Hallmark Details
  huid?: string; // 6-digit alphanumeric Hallmark Unique ID
  bisCertificationNumber?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;
  statusId: number;
  metalId: number;
  metalName?: string;
  purityId: number;
  purityName?: string;
  grossWeight: number;
  netMetalWeight: number;
}

/**
 * Interface for creating a new jewellery item
 */
export interface JewelleryItemCreate {
  itemCode?: string;
  name: string;
  description: string;
  categoryId: number;
  hasStone: boolean;
  stoneId?: number;
  makingChargeType: MakingChargeType;
  makingChargeValue: number;
  wastagePercentage: number;
  isHallmarked: boolean;
  // Hallmark Details
  huid?: string; // 6-digit alphanumeric Hallmark Unique ID
  bisCertificationNumber?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;
  statusId: number;
  metalId: number;
  purityId: number;
  grossWeight: number;
  netMetalWeight: number;
}

/**
 * Interface for updating an existing jewellery item
 */
export interface JewelleryItemUpdate {
  id: number;
  itemCode?: string;
  name: string;
  description: string;
  categoryId: number;
  hasStone: boolean;
  stoneId?: number;
  makingChargeType: MakingChargeType;
  makingChargeValue: number;
  wastagePercentage: number;
  isHallmarked: boolean;
  // Hallmark Details
  huid?: string; // 6-digit alphanumeric Hallmark Unique ID
  bisCertificationNumber?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;
  statusId: number;
  metalId: number;
  purityId: number;
  grossWeight: number;
  netMetalWeight: number;
}

/**
 * Extended interface for JewelleryItem with calculated price and stock info
 * Used in sale wizard and order item management
 */
export interface JewelleryItemWithDetails extends JewelleryItem {
  sellingPrice: number;
  stockQuantity: number;
}

/**
 * Helper function to get MakingChargeType label
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
