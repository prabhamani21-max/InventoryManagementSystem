import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  SaleOrder,
  SaleOrderCreate,
  SaleOrderUpdate,
} from '../models/sale-order.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * SaleOrder Service
 * Handles all HTTP operations for SaleOrder management
 */
@Injectable({
  providedIn: 'root',
})
export class SaleOrderService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/SaleOrder`;

  /**
   * Get all sale orders
   * GET /api/SaleOrder
   */
  getAllSaleOrders(): Observable<SaleOrder[]> {
    return this.http.get<ApiResponse<SaleOrder[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load sale orders');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get sale order by ID
   * GET /api/SaleOrder/{id}
   */
  getSaleOrderById(id: number): Observable<SaleOrder | null> {
    return this.http.get<ApiResponse<SaleOrder>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Sale order not found');
        } else {
          this.toastr.error('Failed to load sale order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new sale order
   * POST /api/SaleOrder
   */
  createSaleOrder(order: SaleOrderCreate): Observable<SaleOrder> {
    return this.http.post<ApiResponse<SaleOrder>>(this.apiUrl, order).pipe(
      map((response) => {
        this.toastr.success('Sale order created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid sale order data');
        } else {
          this.toastr.error('Failed to create sale order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing sale order
   * PUT /api/SaleOrder/{id}
   */
  updateSaleOrder(order: SaleOrderUpdate): Observable<SaleOrder> {
    return this.http.put<ApiResponse<SaleOrder>>(`${this.apiUrl}/${order.id}`, order).pipe(
      map((response) => {
        this.toastr.success('Sale order updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid sale order data');
        } else if (error.status === 404) {
          this.toastr.error('Sale order not found');
        } else {
          this.toastr.error('Failed to update sale order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a sale order
   * DELETE /api/SaleOrder/{id}
   */
  deleteSaleOrder(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Sale order deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Sale order not found');
        } else {
          this.toastr.error('Failed to delete sale order');
        }
        return throwError(() => error);
      })
    );
  }
}
