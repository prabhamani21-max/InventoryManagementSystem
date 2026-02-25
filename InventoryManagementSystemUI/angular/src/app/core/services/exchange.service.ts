import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ExchangeOrder,
  ExchangeOrderCreate,
  ExchangeCalculateRequest,
  ExchangeCalculateResponse,
  ExchangeCompleteRequest,
  ExchangeCancelRequest,
} from '../models/exchange.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Exchange Service
 * Handles all HTTP operations for Exchange management
 */
@Injectable({
  providedIn: 'root',
})
export class ExchangeService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Exchange`;

  /**
   * Calculate exchange value (preview without creating order)
   * POST /api/Exchange/calculate
   */
  calculateExchangeValue(request: ExchangeCalculateRequest): Observable<ExchangeCalculateResponse> {
    return this.http.post<ExchangeCalculateResponse>(`${this.apiUrl}/calculate`, request).pipe(
      map((response) => {
        return response;
      }),
      catchError((error) => {
        this.toastr.error(error.error?.message || 'Failed to calculate exchange value');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get all exchange orders
   * GET /api/Exchange
   */
  getAllExchangeOrders(): Observable<ExchangeOrder[]> {
    return this.http.get<ExchangeOrder[]>(this.apiUrl).pipe(
      map((response) => {
        return response || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load exchange orders');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get exchange order by ID
   * GET /api/Exchange/{id}
   */
  getExchangeOrderById(id: number): Observable<ExchangeOrder | null> {
    return this.http.get<ExchangeOrder>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Exchange order not found');
        } else {
          this.toastr.error('Failed to load exchange order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get exchange order by order number
   * GET /api/Exchange/orderNumber/{orderNumber}
   */
  getExchangeOrderByOrderNumber(orderNumber: string): Observable<ExchangeOrder | null> {
    return this.http.get<ExchangeOrder>(`${this.apiUrl}/orderNumber/${orderNumber}`).pipe(
      map((response) => {
        return response || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Exchange order not found');
        } else {
          this.toastr.error('Failed to load exchange order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get exchange orders by customer ID
   * GET /api/Exchange/customer/{customerId}
   */
  getExchangeOrdersByCustomer(customerId: number): Observable<ExchangeOrder[]> {
    return this.http.get<ExchangeOrder[]>(`${this.apiUrl}/customer/${customerId}`).pipe(
      map((response) => {
        return response || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load exchange orders for customer');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new exchange order
   * POST /api/Exchange
   */
  createExchangeOrder(order: ExchangeOrderCreate): Observable<ExchangeOrder> {
    return this.http.post<ExchangeOrder>(this.apiUrl, order).pipe(
      map((response) => {
        this.toastr.success('Exchange order created successfully');
        return response;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid exchange order data');
        } else {
          this.toastr.error('Failed to create exchange order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Complete exchange order (add to inventory, apply credit)
   * POST /api/Exchange/{orderId}/complete
   */
  completeExchangeOrder(orderId: number, request: ExchangeCompleteRequest): Observable<ExchangeOrder> {
    return this.http.post<ExchangeOrder>(`${this.apiUrl}/${orderId}/complete`, request).pipe(
      map((response) => {
        this.toastr.success('Exchange order completed successfully');
        return response;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Exchange order not found');
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Cannot complete exchange order');
        } else {
          this.toastr.error('Failed to complete exchange order');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Cancel exchange order
   * POST /api/Exchange/{orderId}/cancel
   */
  cancelExchangeOrder(orderId: number, request: ExchangeCancelRequest): Observable<boolean> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${orderId}/cancel`, request).pipe(
      map(() => {
        this.toastr.success('Exchange order cancelled successfully');
        return true;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Exchange order not found');
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Cannot cancel exchange order');
        } else {
          this.toastr.error('Failed to cancel exchange order');
        }
        return throwError(() => error);
      })
    );
  }
}