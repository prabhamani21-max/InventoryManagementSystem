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
  EInvoiceResponse,
  EInvoiceCancelRequest,
  EInvoiceEligibilityResponse,
  QRCodeResponse,
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
  private readonly eInvoiceApiUrl = `${environment.apiUrl}/EInvoice`;

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
            return (response.Data as any).data as Invoice[];
          }
          return response.Data as Invoice[];
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
          return (response.Data as any).data as Invoice;
        }
        // Fallback for direct Data access
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error(error.error?.message || 'Sale order not found');
        } else {
          this.toastr.error('Failed to generate invoice');
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
          return (response.Data as any).data as Invoice;
        }
        return response.Data || null;
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
  getInvoiceBySaleOrderId(saleOrderId: number): Observable<Invoice | null> {
    return this.http.get<ApiResponse<Invoice>>(`${this.apiUrl}/saleorder/${saleOrderId}`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: Invoice }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return (response.Data as any).data as Invoice;
        }
        return response.Data || null;
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
        return result;
      }),
      catchError((error) => {
        this.toastr.error('Failed to generate bulk invoices');
        return throwError(() => error);
      })
    );
  }

  /**
   * Regenerate invoice with updated details
   * POST /api/Invoice/regenerate?invoiceNumber={invoiceNumber}
   */
  regenerateInvoice(invoiceNumber: string, notes?: string): Observable<Invoice | null> {
    const params: any = { invoiceNumber };
    if (notes) {
      params.notes = notes;
    }
    return this.http.post<ApiResponse<Invoice>>(`${this.apiUrl}/regenerate`, null, { params }).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: Invoice }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          this.toastr.success('Invoice regenerated successfully');
          return (response.Data as any).data as Invoice;
        }
        this.toastr.success('Invoice regenerated successfully');
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Invoice not found');
        } else {
          this.toastr.error('Failed to regenerate invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Cancel an invoice
   * POST /api/Invoice/cancel?invoiceNumber={invoiceNumber}
   */
  cancelInvoice(invoiceNumber: string): Observable<boolean> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/cancel`, null, {
      params: { invoiceNumber }
    }).pipe(
      map(() => {
        this.toastr.success('Invoice cancelled successfully');
        return true;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Failed to cancel invoice');
        } else {
          this.toastr.error('Failed to cancel invoice');
        }
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

  // ==================== E-INVOICE METHODS (GST COMPLIANCE) ====================

  /**
   * Generate IRN (Invoice Reference Number) for an invoice
   * POST /api/EInvoice/generate-irn/{invoiceId}
   */
  generateIRN(invoiceId: number): Observable<EInvoiceResponse> {
    return this.http.post<ApiResponse<EInvoiceResponse>>(`${this.eInvoiceApiUrl}/generate-irn/${invoiceId}`, null).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: EInvoiceResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          this.toastr.success('IRN generated successfully');
          return (response.Data as any).data as EInvoiceResponse;
        }
        this.toastr.success('IRN generated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Failed to generate IRN');
        } else {
          this.toastr.error('Failed to generate IRN');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Cancel e-invoice on NIC portal
   * POST /api/EInvoice/cancel/{invoiceId}
   */
  cancelEInvoice(invoiceId: number, cancelReason: string): Observable<boolean> {
    const request: EInvoiceCancelRequest = { cancelReason };
    return this.http.post<ApiResponse<void>>(`${this.eInvoiceApiUrl}/cancel/${invoiceId}`, request).pipe(
      map(() => {
        this.toastr.success('E-Invoice cancelled successfully');
        return true;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Failed to cancel e-invoice');
        } else {
          this.toastr.error('Failed to cancel e-invoice');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get IRN details from NIC portal
   * GET /api/EInvoice/irn/{irn}
   */
  getIRNDetails(irn: string): Observable<EInvoiceResponse | null> {
    return this.http.get<ApiResponse<EInvoiceResponse>>(`${this.eInvoiceApiUrl}/irn/${irn}`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: EInvoiceResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return (response.Data as any).data as EInvoiceResponse;
        }
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('IRN not found');
        } else {
          this.toastr.error('Failed to get IRN details');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Generate QR code for an invoice
   * GET /api/EInvoice/qrcode/{invoiceId}
   */
  generateQRCode(invoiceId: number): Observable<QRCodeResponse> {
    return this.http.get<ApiResponse<QRCodeResponse>>(`${this.eInvoiceApiUrl}/qrcode/${invoiceId}`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: QRCodeResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return (response.Data as any).data as QRCodeResponse;
        }
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Invoice not found');
        } else {
          this.toastr.error('Failed to generate QR code');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Check if invoice is eligible for e-invoicing
   * GET /api/EInvoice/eligibility/{invoiceId}
   */
  checkEInvoiceEligibility(invoiceId: number): Observable<EInvoiceEligibilityResponse> {
    return this.http.get<ApiResponse<EInvoiceEligibilityResponse>>(`${this.eInvoiceApiUrl}/eligibility/${invoiceId}`).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: EInvoiceEligibilityResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          return (response.Data as any).data as EInvoiceEligibilityResponse;
        }
        return response.Data;
      }),
      catchError((error) => {
        this.toastr.error('Failed to check e-invoice eligibility');
        return throwError(() => error);
      })
    );
  }

  /**
   * Sync invoice with NIC portal
   * POST /api/EInvoice/sync/{invoiceId}
   */
  syncWithNICPortal(invoiceId: number): Observable<EInvoiceResponse> {
    return this.http.post<ApiResponse<EInvoiceResponse>>(`${this.eInvoiceApiUrl}/sync/${invoiceId}`, null).pipe(
      map((response) => {
        // The response structure is: { Status, Message, Data: { success, data: EInvoiceResponse }, HttpStatus }
        if (response.Status && response.Data && (response.Data as any).data) {
          this.toastr.success('Synced with NIC portal successfully');
          return (response.Data as any).data as EInvoiceResponse;
        }
        this.toastr.success('Synced with NIC portal successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Failed to sync with NIC portal');
        } else {
          this.toastr.error('Failed to sync with NIC portal');
        }
        return throwError(() => error);
      })
    );
  }
}