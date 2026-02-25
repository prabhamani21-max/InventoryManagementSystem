/**
 * Purity Model
 * Interfaces for Purity management based on backend DTOs
 */

/**
 * Main Purity interface matching PurityDto from backend
 */
export interface Purity {
  id: number;
  metalId: number;
  name: string;
  percentage: number;
  statusId: number;
}

/**
 * Interface for creating a new purity
 */
export interface PurityCreate {
  metalId: number;
  name: string;
  percentage: number;
  statusId: number;
}

/**
 * Interface for updating an existing purity
 */
export interface PurityUpdate {
  id: number;
  metalId: number;
  name: string;
  percentage: number;
  statusId: number;
}

/**
 * Interface for purity with related data (from API responses with includes)
 */
export interface PurityWithRelations extends Purity {
  metalName?: string;
  statusName?: string;
}
