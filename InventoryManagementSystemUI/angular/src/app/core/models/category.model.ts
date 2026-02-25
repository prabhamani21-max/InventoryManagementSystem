/**
 * Category Model
 * Interfaces for Category management based on backend DTOs
 */

/**
 * Main Category interface matching CategoryDto from backend
 */
export interface Category {
  id: number;
  name: string;
  description?: string;
  parentId: number | null;  // null means it's a parent category
  statusId: number;
  createdDate: Date;
  createdBy: number;
  updatedDate?: Date;
  updatedBy?: number;
  subCategories?: Category[];
}

/**
 * Interface for creating a new category (CategoryCreateDto)
 */
export interface CategoryCreate {
  name: string;
  description?: string;
  parentId: number | null;  // null means it's a parent category
  statusId: number;
}

/**
 * Interface for updating an existing category (CategoryUpdateDto)
 */
export interface CategoryUpdate {
  id: number;
  name: string;
  description?: string;
  parentId: number | null;  // null means it's a parent category
  statusId: number;
}

/**
 * Interface for category response from API (CategoryResponseDto)
 */
export interface CategoryResponse {
  id: number;
  name: string;
  description?: string;
  parentId: number | null;
  statusId: number;
  createdDate: Date;
  subCategories?: CategoryResponse[];
}

/**
 * Interface for API response wrapper
 */
