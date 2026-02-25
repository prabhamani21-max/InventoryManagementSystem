import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Category,
  CategoryCreate,
  CategoryUpdate,
} from '../models/category.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Inner response interface for Category API responses
 * The CategoryController wraps responses in { success, data, message }
 */
interface CategoryInnerResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
}

/**
 * Category Service
 * Handles all HTTP operations for Category management
 */
@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Category`;

  /**
   * Get all categories (hierarchical tree)
   * GET /api/Category
   */
  getAllCategories(): Observable<Category[]> {
    return this.http
      .get<ApiResponse<CategoryInnerResponse<Category[]>>>(this.apiUrl)
      .pipe(
        map((response) => {
          // The response structure is: { Status, Message, Data: { success, data: [...] }, HttpStatus }
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            return response.Data.data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load categories');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get category by ID
   * GET /api/Category/{id}
   */
  getCategoryById(id: number): Observable<Category | null> {
    return this.http
      .get<ApiResponse<CategoryInnerResponse<Category>>>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            return response.Data.data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Category not found');
          } else {
            this.toastr.error('Failed to load category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get category by name
   * GET /api/Category/name/{name}
   */
  getCategoryByName(name: string): Observable<Category | null> {
    return this.http
      .get<ApiResponse<CategoryInnerResponse<Category>>>(`${this.apiUrl}/name/${encodeURIComponent(name)}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            return response.Data.data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Category not found');
          } else {
            this.toastr.error('Failed to load category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get all active categories
   * GET /api/Category/active
   */
  getActiveCategories(): Observable<Category[]> {
    return this.http
      .get<ApiResponse<CategoryInnerResponse<Category[]>>>(`${this.apiUrl}/active`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            return response.Data.data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load active categories');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get subcategories by parent ID
   * GET /api/Category/parent/{parentId}
   */
  getSubCategories(parentId: number): Observable<Category[]> {
    return this.http
      .get<ApiResponse<CategoryInnerResponse<Category[]>>>(`${this.apiUrl}/parent/${parentId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            return response.Data.data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load subcategories');
          return throwError(() => error);
        })
      );
  }

  /**
   * Create a new category
   * POST /api/Category
   */
  createCategory(category: CategoryCreate): Observable<Category> {
    return this.http
      .post<ApiResponse<CategoryInnerResponse<Category>>>(this.apiUrl, category)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            this.toastr.success('Category created successfully');
            return response.Data.data;
          }
          throw new Error(response.Message || 'Failed to create category');
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid category data');
          } else {
            this.toastr.error('Failed to create category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing category
   * PUT /api/Category
   */
  updateCategory(category: CategoryUpdate): Observable<Category> {
    return this.http
      .put<ApiResponse<CategoryInnerResponse<Category>>>(this.apiUrl, category)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success && response.Data.data) {
            this.toastr.success('Category updated successfully');
            return response.Data.data;
          }
          throw new Error(response.Message || 'Failed to update category');
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid category data');
          } else if (error.status === 404) {
            this.toastr.error('Category not found');
          } else {
            this.toastr.error('Failed to update category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Delete a category
   * DELETE /api/Category/{id}
   */
  deleteCategory(id: number): Observable<boolean> {
    return this.http
      .delete<ApiResponse<CategoryInnerResponse<void>>>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success) {
            this.toastr.success('Category deleted successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Cannot delete category');
          } else if (error.status === 404) {
            this.toastr.error('Category not found');
          } else {
            this.toastr.error('Failed to delete category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Activate a category
   * POST /api/Category/{id}/activate
   */
  activateCategory(id: number): Observable<boolean> {
    return this.http
      .post<ApiResponse<CategoryInnerResponse<void>>>(`${this.apiUrl}/${id}/activate`, {})
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success) {
            this.toastr.success('Category activated successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Cannot activate category');
          } else {
            this.toastr.error('Failed to activate category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Deactivate a category
   * POST /api/Category/{id}/deactivate
   */
  deactivateCategory(id: number): Observable<boolean> {
    return this.http
      .post<ApiResponse<CategoryInnerResponse<void>>>(`${this.apiUrl}/${id}/deactivate`, {})
      .pipe(
        map((response) => {
          if (response.Status && response.Data && response.Data.success) {
            this.toastr.success('Category deactivated successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Cannot deactivate category');
          } else {
            this.toastr.error('Failed to deactivate category');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Flatten hierarchical categories for display in dropdowns
   * @param categories Hierarchical category list
   * @param level Current depth level for indentation
   * @returns Flattened array with indentation prefix
   */
  flattenCategories(categories: Category[], level: number = 0): Array<Category & { indent: string }> {
    const result: Array<Category & { indent: string }> = [];
    
    for (const category of categories) {
      result.push({
        ...category,
        indent: '  '.repeat(level) + (level > 0 ? '  ' : ''),
      });
      
      if (category.subCategories && category.subCategories.length > 0) {
        result.push(...this.flattenCategories(category.subCategories, level + 1));
      }
    }
    
    return result;
  }
}