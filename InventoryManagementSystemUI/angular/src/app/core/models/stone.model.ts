/**
 * Stone Model
 * Interfaces for Stone management based on backend DTOs
 */

/**
 * Main Stone interface matching StoneDto from backend
 */
export interface Stone {
  id: number;
  name: string;
  unit?: string;
  statusId: number;
}

/**
 * Interface for creating a new stone
 */
export interface StoneCreate {
  name: string;
  unit?: string;
  statusId: number;
}

/**
 * Interface for updating an existing stone
 */
export interface StoneUpdate {
  id: number;
  name: string;
  unit?: string;
  statusId: number;
}
