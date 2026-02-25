import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Supplier,
  SupplierCreate,
  SupplierUpdate,
} from '../models/supplier.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Supplier Service
 * Handles all HTTP operations for Supplier management
 */
@Injectable({
  providedIn: 'root',
})
export class SupplierService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Supplier`;

  /**
   * Get all suppliers
   * GET /api/Supplier
   */
  getAllSuppliers(): Observable<Supplier[]> {
    return this.http
      .get<ApiResponse<Supplier[]>>(this.apiUrl)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load suppliers');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get supplier by ID
   * GET /api/Supplier/{id}
   */
  getSupplierById(id: number): Observable<Supplier | null> {
    return this.http
      .get<ApiResponse<Supplier>>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Supplier not found');
          } else {
            this.toastr.error('Failed to load supplier');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Create a new supplier
   * POST /api/Supplier
   */
  createSupplier(supplier: SupplierCreate): Observable<Supplier> {
    return this.http
      .post<ApiResponse<Supplier>>(this.apiUrl, supplier)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Supplier created successfully');
            return response.Data;
          }
          throw new Error('Failed to create supplier');
        }),
        catchError((error) => {
          // Don't show toast here - let component handle field-specific errors
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing supplier
   * PUT /api/Supplier/{id}
   */
  updateSupplier(supplier: SupplierUpdate): Observable<Supplier> {
    return this.http
      .put<ApiResponse<Supplier>>(`${this.apiUrl}/${supplier.id}`, supplier)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Supplier updated successfully');
            return response.Data;
          }
          throw new Error('Failed to update supplier');
        }),
        catchError((error) => {
          // Don't show toast here - let component handle field-specific errors
          return throwError(() => error);
        })
      );
  }

  /**
   * Delete a supplier
   * DELETE /api/Supplier/{id}
   */
  deleteSupplier(id: number): Observable<boolean> {
    return this.http
      .delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map((response) => {
          if (response.status === 200 || response.status === 204) {
            this.toastr.success('Supplier deleted successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.error('Supplier not found');
          } else {
            this.toastr.error('Failed to delete supplier');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Handle validation errors from backend
   * Displays specific field error messages
   */
  private handleValidationError(error: any): void {
    if (error.status === 409) {
      // Handle unique constraint violations for GST or TAN number
      const errors = error.error?.errors;
      if (errors?.GSTNumber) {
        this.toastr.error('GST number already exists');
      } else if (errors?.TANNumber) {
        this.toastr.error('TAN number already exists');
      } else {
        this.toastr.error('A supplier with this GST or TAN number already exists');
      }
    } else if (error.status === 400) {
      // Handle validation errors from FluentValidation
      const errors = error.error?.errors;
      if (errors) {
        // Display each validation error
        if (errors.Name) {
          this.toastr.error(errors.Name.join(', '));
        }
        if (errors.ContactPerson) {
          this.toastr.error(errors.ContactPerson.join(', '));
        }
        if (errors.Email) {
          this.toastr.error(errors.Email.join(', '));
        }
        if (errors.Phone) {
          this.toastr.error(errors.Phone.join(', '));
        }
        if (errors.Address) {
          this.toastr.error(errors.Address.join(', '));
        }
        if (errors.GSTNumber) {
          this.toastr.error(errors.GSTNumber.join(', '));
        }
        if (errors.TANNumber) {
          this.toastr.error(errors.TANNumber.join(', '));
        }
        if (errors.StatusId) {
          this.toastr.error(errors.StatusId.join(', '));
        }
      } else if (error.error?.message) {
        this.toastr.error(error.error.message);
      } else {
        this.toastr.error('Invalid supplier data');
      }
    } else if (error.status === 404) {
      this.toastr.error('Supplier not found');
    } else {
      this.toastr.error('An error occurred. Please try again.');
    }
  }
}
