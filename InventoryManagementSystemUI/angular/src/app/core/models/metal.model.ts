/**
 * Metal Model
 * Interfaces for Metal management based on backend DTOs
 */

/**
 * Main Metal interface matching MetalDto from backend
 */
export interface Metal {
  id: number;
  name: string;
  statusId: number;
}

/**
 * Interface for creating a new metal
 */
export interface MetalCreate {
  name: string;
  statusId: number;
}

/**
 * Interface for updating an existing metal
 */
export interface MetalUpdate {
  id: number;
  name: string;
  statusId: number;
}
