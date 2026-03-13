import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Invoice,
  InvoiceRequest,
  BulkInvoiceRequest,
  BulkInvoiceResult,
  NumberToWordsResponse,
} from '../models/invoice.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Invoice Service
 * Handles all HTTP operations for Invoice management
 */
@Injectable({
  providedIn: 'root',
})
export class InvoiceService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Invoice`;

  private normalizeInvoice(invoice: Invoice | null | undefined): Invoice | null {
    if (!invoice) {
      return null;
    }

    const invoiceItems = invoice.invoiceItems ?? invoice.items ?? [];
    const invoicePayments = invoice.invoicePayments ?? invoice.payments ?? [];

    return {
      ...invoice,
      items: invoiceItems,
      payments: invoicePayments,
      invoiceItems,
      invoicePayments,
    };
  }

  /**
   * Get all invoices
   * GET /api/Invoice/GetAllInvoices
   */
  getAllInvoices(): Observable<Invoice[]> {
    return this.http.get<ApiResponse<Invoice[]>>(`${this.apiUrl}/GetAllInvoices`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: Invoice[] }, HttpStatus }
        if (response.Status && response.Data) {
          if ((response.Data as any).data) {
            return ((response.Data as any).data as Invoice[]).map((invoice) =>
              this.normalizeInvoice(invoice) as Invoice
            );
          }
          return (response.Data as Invoice[]).map((invoice) =>
            this.normalizeInvoice(invoice) as Invoice
          );
        }
        return [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load invoices');
        return throwError(() => error);
      })
    );
  }

  /**
   * Generate invoice from a sale order
   * POST /api/Invoice/generate
   */
  generateInvoice(request: InvoiceRequest): Observable<Invoice> {
    return this.http.post<ApiResponse<Invoice>>(`${this.apiUrl}/generate`, request).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, message, data: Invoice }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          this.toastr.success('Invoice generated successfully');
          return this.normalizeInvoice((response.Data as any).data as Invoice) as Invoice;
        }
        // Fallback for direct Data access
        if (response.Status && response.Data) {
          this.toastr.success('Invoice generated successfully');
          return this.normalizeInvoice(response.Data) as Invoice;
        }

        const message =
          (response.Data as any)?.message ||
          response.Message ||
          'Failed to generate invoice';
        throw new Error(message);
      }),
      catchError((error) => {
        const backendMessage =
          error?.error?.Data?.message ||
          error?.error?.Data?.Message ||
          error?.error?.message ||
          error?.message;

        if (error.status === 404) {
          this.toastr.error(backendMessage || 'Sale order not found');
        } else {
          this.toastr.error(backendMessage || 'Failed to generate invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoice by invoice number
   * GET /api/Invoice/by-number?invoiceNumber={invoiceNumber}
   */
  getInvoiceByNumber(invoiceNumber: string): Observable<Invoice | null> {
    return this.http.get<ApiResponse<Invoice>>(`${this.apiUrl}/by-number`, {
      params: { invoiceNumber }
    }).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: Invoice }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return this.normalizeInvoice((response.Data as any).data as Invoice);
        }
        return this.normalizeInvoice(response.Data || null);
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
  getInvoiceBySaleOrderId(saleOrderId: number, suppressNotFoundToast: boolean = false): Observable<Invoice | null> {
    return this.http.get<ApiResponse<Invoice>>(`${this.apiUrl}/saleorder/${saleOrderId}`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: Invoice }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return this.normalizeInvoice((response.Data as any).data as Invoice);
        }
        return this.normalizeInvoice(response.Data || null);
      }),
      catchError((error) => {
        if (error.status === 404) {
          if (!suppressNotFoundToast) {
            this.toastr.warning('Invoice not found for this sale order');
          }
        } else {
          this.toastr.error('Failed to load invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Generate bulk invoices for multiple sale orders
   * POST /api/Invoice/generate-bulk
   */
  generateBulkInvoices(request: BulkInvoiceRequest): Observable<BulkInvoiceResult> {
    return this.http.post<ApiResponse<BulkInvoiceResult>>(`${this.apiUrl}/generate-bulk`, request).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: BulkInvoiceResult }, HttpStatus }
        let result: BulkInvoiceResult;
        if (response.Status && response.Data && (response.Data as any).data) {
          result = (response.Data as any).data as BulkInvoiceResult;
        } else {
          result = response.Data;
        }
        if (result.totalGenerated > 0) {
          this.toastr.success(`Generated ${result.totalGenerated} invoices successfully`);
        }
        if (result.totalFailed > 0) {
          this.toastr.warning(`${result.totalFailed} invoices failed to generate`);
        }
        return {
          ...result,
          invoices: (result.invoices || []).map((invoice) => this.normalizeInvoice(invoice) as Invoice),
        };
      }),
      catchError((error) => {
        this.toastr.error('Failed to generate bulk invoices');
        return throwError(() => error);
      })
    );
  }

  /**
   * Convert number to words (for grand total)
   * GET /api/Invoice/number-to-words
   */
  numberToWords(number: number): Observable<NumberToWordsResponse> {
    return this.http.get<ApiResponse<NumberToWordsResponse>>(`${this.apiUrl}/number-to-words`, {
      params: { number: number.toString() }
    }).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: NumberToWordsResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return (response.Data as any).data as NumberToWordsResponse;
        }
        return response.Data;
      }),
      catchError((error) => {
        this.toastr.error('Failed to convert number to words');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoices for the currently logged-in customer
   * GET /api/Invoice/my-invoices
   */
  getMyInvoices(): Observable<Invoice[]> {
    return this.http.get<ApiResponse<Invoice[]>>(`${this.apiUrl}/my-invoices`).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          if ((response.Data as any).data) {
            return ((response.Data as any).data as Invoice[]).map((invoice) =>
              this.normalizeInvoice(invoice) as Invoice
            );
          }
          return (response.Data as Invoice[]).map((invoice) =>
            this.normalizeInvoice(invoice) as Invoice
          );
        }
        return [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load your invoices');
        return throwError(() => error);
      })
    );
  }

  /**
   * Download invoice as PDF by invoice ID
   * GET /api/Invoice/{id}/download
   */
  downloadInvoicePdf(invoiceId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${invoiceId}/download`, {
      responseType: 'blob'
    }).pipe(
      catchError((error) => {
        this.toastr.error('Failed to download invoice');
        return throwError(() => error);
      })
    );
  }

  /**
   * Download invoice as PDF by invoice number
   * GET /api/Invoice/by-number/download?invoiceNumber={invoiceNumber}
   */
  downloadInvoicePdfByNumber(invoiceNumber: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/by-number/download`, {
      params: { invoiceNumber },
      responseType: 'blob'
    }).pipe(
      catchError((error) => {
        this.toastr.error('Failed to download invoice');
        return throwError(() => error);
      })
    );
  }
}
