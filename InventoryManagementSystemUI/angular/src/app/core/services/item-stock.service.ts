import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ItemStock,
  ItemStockCreate,
  ItemStockUpdate,
  StockValidationRequest,
  StockValidationResult,
  StockAvailabilityResponse,
} from '../models/item-stock.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * ItemStock Service
 * Handles all HTTP operations for ItemStock management
 */
@Injectable({
  providedIn: 'root',
})
export class ItemStockService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/ItemStock`;

  /**
   * Get all item stocks
   * GET /api/ItemStock
   */
  getAllItemStocks(): Observable<ItemStock[]> {
    return this.http.get<ApiResponse<ItemStock[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load item stocks');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get item stock by ID
   * GET /api/ItemStock/{id}
   */
  getItemStockById(id: number): Observable<ItemStock | null> {
    return this.http.get<ApiResponse<ItemStock>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Item stock not found');
        } else {
          this.toastr.error('Failed to load item stock');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get item stock by Jewellery Item ID
   * GET /api/ItemStock/by-item/{jewelleryItemId}
   */
  getItemStockByJewelleryItemId(jewelleryItemId: number, warehouseId?: number): Observable<ItemStock | null> {
    let url = `${this.apiUrl}/by-item/${jewelleryItemId}`;
    if (warehouseId) {
      url += `?warehouseId=${warehouseId}`;
    }
    return this.http.get<ApiResponse<ItemStock>>(url).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status !== 404) {
          this.toastr.error('Failed to load item stock');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new item stock
   * POST /api/ItemStock
   */
  createItemStock(item: ItemStockCreate): Observable<ItemStock> {
    return this.http.post<ApiResponse<ItemStock>>(this.apiUrl, item).pipe(
      map((response) => {
        this.toastr.success('Item stock created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid item stock data');
        } else {
          this.toastr.error('Failed to create item stock');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing item stock
   * PUT /api/ItemStock/{id}
   */
  updateItemStock(item: ItemStockUpdate): Observable<ItemStock> {
    return this.http.put<ApiResponse<ItemStock>>(`${this.apiUrl}/${item.id}`, item).pipe(
      map((response) => {
        this.toastr.success('Item stock updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid item stock data');
        } else if (error.status === 404) {
          this.toastr.error('Item stock not found');
        } else {
          this.toastr.error('Failed to update item stock');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete an item stock
   * DELETE /api/ItemStock/{id}
   */
  deleteItemStock(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Item stock deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Item stock not found');
        } else {
          this.toastr.error('Failed to delete item stock');
        }
        return throwError(() => error);
      })
    );
  }

  // ==================== STOCK VALIDATION METHODS ====================

  /**
   * Check stock availability for a single item
   * GET /api/ItemStock/check-availability/{jewelleryItemId}?quantity={quantity}&warehouseId={warehouseId}
   */
  checkStockAvailability(jewelleryItemId: number, quantity: number, warehouseId?: number): Observable<StockAvailabilityResponse> {
    let url = `${this.apiUrl}/check-availability/${jewelleryItemId}?quantity=${quantity}`;
    if (warehouseId) {
      url += `&warehouseId=${warehouseId}`;
    }
    return this.http.get<ApiResponse<StockAvailabilityResponse>>(url).pipe(
      map((response) => {
        return response.Data;
      }),
      catchError((error) => {
        this.toastr.error('Failed to check stock availability');
        return throwError(() => error);
      })
    );
  }

  /**
   * Validate stock for multiple items (for order validation)
   * POST /api/ItemStock/validate-order-stock
   */
  validateOrderStock(items: StockValidationRequest[]): Observable<StockValidationResult> {
    return this.http.post<ApiResponse<StockValidationResult>>(`${this.apiUrl}/validate-order-stock`, items).pipe(
      map((response) => {
        return response.Data;
      }),
      catchError((error) => {
        this.toastr.error('Failed to validate order stock');
        return throwError(() => error);
      })
    );
  }
}