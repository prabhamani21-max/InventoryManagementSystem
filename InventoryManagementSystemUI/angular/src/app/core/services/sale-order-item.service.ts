import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  SaleOrderItem,
  SaleOrderItemCreate,
  SaleOrderItemUpdate,
  SaleOrderItemWithCalculation,
} from '../models/sale-order-item.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * SaleOrderItem Service
 * Handles all HTTP operations for SaleOrderItem management
 */
@Injectable({
  providedIn: 'root',
})
export class SaleOrderItemService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/SaleOrderItem`;

  /**
   * Get all sale order items
   * GET /api/SaleOrderItem
   */
  getAllSaleOrderItems(): Observable<SaleOrderItem[]> {
    return this.http.get<ApiResponse<SaleOrderItem[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load sale order items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get sale order item by ID
   * GET /api/SaleOrderItem/{id}
   */
  getSaleOrderItemById(id: number): Observable<SaleOrderItem | null> {
    return this.http.get<ApiResponse<SaleOrderItem>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Sale order item not found');
        } else {
          this.toastr.error('Failed to load sale order item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get sale order items by sale order ID
   * GET /api/SaleOrderItem/by-sale-order/{saleOrderId}
   */
  getSaleOrderItemsBySaleOrderId(saleOrderId: number): Observable<SaleOrderItem[]> {
    return this.http.get<ApiResponse<SaleOrderItem[]>>(`${this.apiUrl}/by-sale-order/${saleOrderId}`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load sale order items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new sale order item
   * POST /api/SaleOrderItem
   */
  createSaleOrderItem(item: SaleOrderItemCreate): Observable<SaleOrderItem> {
    return this.http.post<ApiResponse<SaleOrderItem>>(this.apiUrl, item).pipe(
      map((response) => {
        this.toastr.success('Sale order item created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid sale order item data');
        } else {
          this.toastr.error('Failed to create sale order item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a sale order item with automatic price calculation
   * POST /api/SaleOrderItem/calculate
   * This calls the backend's CreateSaleOrderItemWithCalculationAsync method
   * Note: Backend validates stock availability before creating the item
   */
  createSaleOrderItemWithCalculation(item: SaleOrderItemWithCalculation): Observable<SaleOrderItem> {
    return this.http.post<ApiResponse<SaleOrderItem>>(`${this.apiUrl}/calculate`, item).pipe(
      map((response) => {
        this.toastr.success('Sale order item created with automatic calculation');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          // Check if it's a stock validation error
          const errorMessage = error.error?.message || error.error || 'Invalid sale order item data';
          this.toastr.error(errorMessage);
        } else {
          this.toastr.error('Failed to create sale order item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing sale order item
   * PUT /api/SaleOrderItem/{id}
   */
  updateSaleOrderItem(item: SaleOrderItemUpdate): Observable<SaleOrderItem> {
    return this.http.put<ApiResponse<SaleOrderItem>>(`${this.apiUrl}/${item.id}`, item).pipe(
      map((response) => {
        this.toastr.success('Sale order item updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid sale order item data');
        } else if (error.status === 404) {
          this.toastr.error('Sale order item not found');
        } else {
          this.toastr.error('Failed to update sale order item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a sale order item
   * DELETE /api/SaleOrderItem/{id}
   */
  deleteSaleOrderItem(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Sale order item deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Sale order item not found');
        } else {
          this.toastr.error('Failed to delete sale order item');
        }
        return throwError(() => error);
      })
    );
  }
}
