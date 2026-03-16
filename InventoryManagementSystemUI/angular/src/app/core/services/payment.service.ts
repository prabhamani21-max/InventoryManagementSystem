import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Payment,
  PaymentCreate,
  PaymentUpdate,
} from '../models/payment.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Payment Service
 * Handles all HTTP operations for Payment management
 */
@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Payment`;

  /**
   * Get all payments
   * GET /api/Payment
   */
  getAllPayments(): Observable<Payment[]> {
    return this.http.get<ApiResponse<Payment[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load payments');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get payment by ID
   * GET /api/Payment/{id}
   */
  getPaymentById(id: number): Observable<Payment | null> {
    return this.http.get<ApiResponse<Payment>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Payment not found');
        } else {
          this.toastr.error('Failed to load payment');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get payments by order ID
   * GET /api/Payment/order/{orderId}
   */
  getPaymentsByOrderId(orderId: number): Observable<Payment[]> {
    return this.http.get<ApiResponse<Payment[]>>(`${this.apiUrl}/order/${orderId}`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load payments for order');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new payment
   * POST /api/Payment
   */
  createPayment(payment: PaymentCreate): Observable<Payment> {
    return this.http.post<ApiResponse<Payment>>(this.apiUrl, payment).pipe(
      map((response) => {
        this.toastr.success('Payment created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid payment data');
        } else {
          this.toastr.error('Failed to create payment');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing payment
   * PUT /api/Payment/{id}
   */
  updatePayment(payment: PaymentUpdate): Observable<Payment> {
    return this.http.put<ApiResponse<Payment>>(`${this.apiUrl}/${payment.id}`, payment).pipe(
      map((response) => {
        this.toastr.success('Payment updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid payment data');
        } else if (error.status === 404) {
          this.toastr.error('Payment not found');
        } else {
          this.toastr.error('Failed to update payment');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a payment
   * DELETE /api/Payment/{id}
   */
  deletePayment(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Payment deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Payment not found');
        } else {
          this.toastr.error('Failed to delete payment');
        }
        return throwError(() => error);
      })
    );
  }
}
