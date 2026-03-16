import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  InvoicePayment,
  InvoicePaymentCreate,
  InvoicePaymentUpdate,
  Payment,
  PaymentCreate,
  PaymentUpdate,
  InvoicePaymentDto,
} from '../models/invoice-payment.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * InvoicePayment Service
 * Handles all HTTP operations for InvoicePayment and Payment management
 */
@Injectable({
  providedIn: 'root',
})
export class InvoicePaymentService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly paymentApiUrl = `${environment.apiUrl}/Payment`;
  private readonly invoiceApiUrl = `${environment.apiUrl}/Invoice`;

  // ==================== Payment Operations ====================

  /**
   * Get all payments
   * GET /api/Payment
   */
  getAllPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(this.paymentApiUrl).pipe(
      map((response) => {
        return response || [];
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
    return this.http.get<Payment>(`${this.paymentApiUrl}/${id}`).pipe(
      map((response) => {
        return response || null;
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
   * Create a new payment
   * POST /api/Payment
   */
  createPayment(payment: PaymentCreate): Observable<Payment> {
    return this.http.post<Payment>(this.paymentApiUrl, payment).pipe(
      map((response) => {
        this.toastr.success('Payment created successfully');
        return response;
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
    return this.http.put<Payment>(`${this.paymentApiUrl}/${payment.id}`, payment).pipe(
      map((response) => {
        this.toastr.success('Payment updated successfully');
        return response;
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
    return this.http.delete<void>(`${this.paymentApiUrl}/${id}`, { observe: 'response' }).pipe(
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

  // ==================== Invoice Operations ====================

  /**
   * Get invoice by invoice number
   * GET /api/Invoice/{invoiceNumber}
   */
  getInvoiceByNumber(invoiceNumber: string): Observable<any> {
    return this.http.get<any>(`${this.invoiceApiUrl}/${invoiceNumber}`).pipe(
      map((response) => {
        return response?.data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Invoice not found');
        } else {
          this.toastr.error('Failed to load invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoice by sale order ID
   * GET /api/Invoice/saleorder/{saleOrderId}
   */
  getInvoiceBySaleOrderId(saleOrderId: number): Observable<any> {
    return this.http.get<any>(`${this.invoiceApiUrl}/saleorder/${saleOrderId}`).pipe(
      map((response) => {
        return response?.data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Invoice not found for this sale order');
        } else {
          this.toastr.error('Failed to load invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Generate invoice from sale order
   * POST /api/Invoice/generate
   */
  generateInvoice(request: { saleOrderId: number; includePaymentDetails?: boolean; includeTermsAndConditions?: boolean; notes?: string }): Observable<any> {
    return this.http.post<any>(`${this.invoiceApiUrl}/generate`, request).pipe(
      map((response) => {
        this.toastr.success('Invoice generated successfully');
        return response?.data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid invoice request');
        } else if (error.status === 404) {
          this.toastr.error('Sale order not found');
        } else {
          this.toastr.error('Failed to generate invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Cancel an invoice
   * POST /api/Invoice/{invoiceNumber}/cancel
   */
  cancelInvoice(invoiceNumber: string): Observable<boolean> {
    return this.http.post<any>(`${this.invoiceApiUrl}/${invoiceNumber}/cancel`, {}).pipe(
      map((response) => {
        if (response?.success) {
          this.toastr.success('Invoice cancelled successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        this.toastr.error('Failed to cancel invoice');
        return throwError(() => error);
      })
    );
  }

  /**
   * Regenerate an invoice
   * POST /api/Invoice/{invoiceNumber}/regenerate
   */
  regenerateInvoice(invoiceNumber: string, notes?: string): Observable<any> {
    let params: Record<string, string> = {};
    if (notes) {
      params = { notes };
    }
    return this.http.post<any>(`${this.invoiceApiUrl}/${invoiceNumber}/regenerate`, {}, { params }).pipe(
      map((response) => {
        this.toastr.success('Invoice regenerated successfully');
        return response?.data;
      }),
      catchError((error) => {
        this.toastr.error('Failed to regenerate invoice');
        return throwError(() => error);
      })
    );
  }

  /**
   * Convert number to words
   * GET /api/Invoice/number-to-words
   */
  numberToWords(number: number): Observable<string> {
    return this.http.get<any>(`${this.invoiceApiUrl}/number-to-words`, { params: { number } }).pipe(
      map((response) => {
        return response?.data?.words || '';
      }),
      catchError((error) => {
        return throwError(() => error);
      })
    );
  }

  // ==================== Invoice Payment Operations ====================

  /**
   * Get payments for an invoice by invoice number
   * This extracts payments from the invoice response
   */
  getInvoicePayments(invoiceNumber: string): Observable<InvoicePaymentDto[]> {
    return this.getInvoiceByNumber(invoiceNumber).pipe(
      map((invoice) => {
        return invoice?.payments || [];
      })
    );
  }

  /**
   * Get payments for an invoice by sale order ID
   * This extracts payments from the invoice response
   */
  getInvoicePaymentsBySaleOrderId(saleOrderId: number): Observable<InvoicePaymentDto[]> {
    return this.getInvoiceBySaleOrderId(saleOrderId).pipe(
      map((invoice) => {
        return invoice?.payments || [];
      })
    );
  }
}
