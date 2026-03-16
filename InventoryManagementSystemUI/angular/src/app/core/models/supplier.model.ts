/**
 * Supplier Model
 * Interfaces for Supplier management based on backend DTOs
 */

/**
 * Main Supplier interface matching SupplierDto from backend
 */
export interface Supplier {
  id: number;
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  address?: string;
  tanNumber: string;
  gstNumber: string;
  statusId: number;
}

/**
 * Interface for creating a new supplier
 */
export interface SupplierCreate {
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  address?: string;
  tanNumber: string;
  gstNumber: string;
  statusId: number;
}

/**
 * Interface for updating an existing supplier
 */
export interface SupplierUpdate {
  id: number;
  name: string;
  contactPerson?: string;
  email?: string;
  phone?: string;
  address?: string;
  tanNumber: string;
  gstNumber: string;
  statusId: number;
}
