import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  InvoiceItem,
  InvoiceItemCreate,
  InvoiceItemUpdate,
} from '../models/invoice-item.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Internal response type for InvoiceItem controller responses
 * The controller returns { success: true, data: T } which gets wrapped by middleware
 */
interface InvoiceItemControllerResponse<T> {
  success: boolean;
  data: T;
}

/**
 * InvoiceItem Service
 * Handles all HTTP operations for InvoiceItem management
 * Uses the separate InvoiceItemController endpoint
 */
@Injectable({
  providedIn: 'root',
})
export class InvoiceItemService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/InvoiceItem`;

  /**
   * Get all invoice items
   * GET /api/InvoiceItem
   */
  getAllInvoiceItems(): Observable<InvoiceItem[]> {
    return this.http.get<ApiResponse<InvoiceItemControllerResponse<InvoiceItem[]>>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data?.data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load invoice items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoice item by ID
   * GET /api/InvoiceItem/{id}
   */
  getInvoiceItemById(id: number): Observable<InvoiceItem | null> {
    return this.http.get<ApiResponse<InvoiceItemControllerResponse<InvoiceItem>>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data?.data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Invoice item not found');
        } else {
          this.toastr.error('Failed to load invoice item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoice items by invoice ID
   * GET /api/InvoiceItem/invoice/{invoiceId}
   */
  getInvoiceItemsByInvoiceId(invoiceId: number): Observable<InvoiceItem[]> {
    return this.http.get<ApiResponse<InvoiceItemControllerResponse<InvoiceItem[]>>>(`${this.apiUrl}/invoice/${invoiceId}`).pipe(
      map((response) => {
        return response.Data?.data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load invoice items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new invoice item
   * POST /api/InvoiceItem
   */
  createInvoiceItem(item: InvoiceItemCreate): Observable<InvoiceItem> {
    return this.http.post<ApiResponse<InvoiceItemControllerResponse<InvoiceItem>>>(this.apiUrl, item).pipe(
      map((response) => {
        this.toastr.success('Invoice item created successfully');
        return response.Data.data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid invoice item data');
        } else {
          this.toastr.error('Failed to create invoice item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing invoice item
   * PUT /api/InvoiceItem/{id}
   */
  updateInvoiceItem(item: InvoiceItemUpdate): Observable<InvoiceItem> {
    return this.http.put<ApiResponse<InvoiceItemControllerResponse<InvoiceItem>>>(`${this.apiUrl}/${item.id}`, item).pipe(
      map((response) => {
        this.toastr.success('Invoice item updated successfully');
        return response.Data.data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid invoice item data');
        } else if (error.status === 404) {
          this.toastr.error('Invoice item not found');
        } else {
          this.toastr.error('Failed to update invoice item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete an invoice item
   * DELETE /api/InvoiceItem/{id}
   */
  deleteInvoiceItem(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<InvoiceItemControllerResponse<void>>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Invoice item deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Invoice item not found');
        } else {
          this.toastr.error('Failed to delete invoice item');
        }
        return throwError(() => error);
      })
    );
  }
}
