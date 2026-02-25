/**
 * Warehouse Model
 * Interfaces for Warehouse management based on backend DTOs
 */

/**
 * Main Warehouse interface matching WarehouseDto from backend
 */
export interface Warehouse {
  id: number;
  name: string;
  address?: string;
  managerId?: number;
  managerName?: string;
  statusId: number;
}

/**
 * Interface for creating a new warehouse
 */
export interface WarehouseCreate {
  name: string;
  address?: string;
  managerId?: number;
  statusId: number;
}

/**
 * Interface for updating an existing warehouse
 */
export interface WarehouseUpdate {
  id: number;
  name: string;
  address?: string;
  managerId?: number;
  statusId: number;
}

/**
 * Status enum for warehouse status
 */
export enum WarehouseStatus {
  Active = 1,
  Inactive = 2,
  Pending = 3,
}

/**
 * Get status label based on status ID
 */
export function getStatusLabel(statusId: number): string {
  switch (statusId) {
    case WarehouseStatus.Active:
      return 'Active';
    case WarehouseStatus.Inactive:
      return 'Inactive';
    case WarehouseStatus.Pending:
      return 'Pending';
    default:
      return 'Unknown';
  }
}

/**
 * Get status CSS class based on status ID
 */
export function getStatusClass(statusId: number): string {
  switch (statusId) {
    case WarehouseStatus.Active:
      return 'badge bg-success';
    case WarehouseStatus.Inactive:
      return 'badge bg-danger';
    case WarehouseStatus.Pending:
      return 'badge bg-warning';
    default:
      return 'badge bg-secondary';
  }
}