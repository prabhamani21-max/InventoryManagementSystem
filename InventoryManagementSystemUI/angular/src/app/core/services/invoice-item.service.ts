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
 * InvoiceItem Service
 * Handles all HTTP operations for InvoiceItem management
 */
@Injectable({
  providedIn: 'root',
})
export class InvoiceItemService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Invoice`;

  /**
   * Get all invoice items
   * GET /api/Invoice/items
   */
  getAllInvoiceItems(): Observable<InvoiceItem[]> {
    return this.http.get<ApiResponse<InvoiceItem[]>>(`${this.apiUrl}/items`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load invoice items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get invoice item by ID
   * GET /api/Invoice/items/{id}
   */
  getInvoiceItemById(id: number): Observable<InvoiceItem | null> {
    return this.http.get<ApiResponse<InvoiceItem>>(`${this.apiUrl}/items/${id}`).pipe(
      map((response) => {
        return response.Data || null;
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
   * GET /api/Invoice/{invoiceId}/items
   */
  getInvoiceItemsByInvoiceId(invoiceId: number): Observable<InvoiceItem[]> {
    return this.http.get<ApiResponse<InvoiceItem[]>>(`${this.apiUrl}/${invoiceId}/items`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load invoice items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new invoice item
   * POST /api/Invoice/items
   */
  createInvoiceItem(item: InvoiceItemCreate): Observable<InvoiceItem> {
    return this.http.post<ApiResponse<InvoiceItem>>(`${this.apiUrl}/items`, item).pipe(
      map((response) => {
        this.toastr.success('Invoice item created successfully');
        return response.Data;
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
   * PUT /api/Invoice/items/{id}
   */
  updateInvoiceItem(item: InvoiceItemUpdate): Observable<InvoiceItem> {
    return this.http.put<ApiResponse<InvoiceItem>>(`${this.apiUrl}/items/${item.id}`, item).pipe(
      map((response) => {
        this.toastr.success('Invoice item updated successfully');
        return response.Data;
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
   * DELETE /api/Invoice/items/{id}
   */
  deleteInvoiceItem(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/items/${id}`, { observe: 'response' }).pipe(
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
