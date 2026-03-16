import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Warehouse, WarehouseCreate, WarehouseUpdate } from '../models/warehouse.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Warehouse Service
 * Handles all HTTP operations for Warehouse management
 */
@Injectable({
  providedIn: 'root',
})
export class WarehouseService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Warehouse`;

  /**
   * Get all warehouses
   * GET /api/Warehouse
   */
  getAllWarehouses(): Observable<Warehouse[]> {
    return this.http.get<ApiResponse<Warehouse[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load warehouses');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get warehouse by ID
   * GET /api/Warehouse/{id}
   */
  getWarehouseById(id: number): Observable<Warehouse | null> {
    return this.http.get<ApiResponse<Warehouse>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Warehouse not found');
        } else {
          this.toastr.error('Failed to load warehouse');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new warehouse
   * POST /api/Warehouse
   */
  createWarehouse(warehouse: WarehouseCreate): Observable<Warehouse> {
    return this.http.post<ApiResponse<Warehouse>>(this.apiUrl, warehouse).pipe(
      map((response) => {
        this.toastr.success('Warehouse created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid warehouse data');
        } else {
          this.toastr.error('Failed to create warehouse');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing warehouse
   * PUT /api/Warehouse/{id}
   */
  updateWarehouse(warehouse: WarehouseUpdate): Observable<Warehouse> {
    return this.http.put<ApiResponse<Warehouse>>(`${this.apiUrl}/${warehouse.id}`, warehouse).pipe(
      map((response) => {
        this.toastr.success('Warehouse updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid warehouse data');
        } else if (error.status === 404) {
          this.toastr.error('Warehouse not found');
        } else {
          this.toastr.error('Failed to update warehouse');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a warehouse
   * DELETE /api/Warehouse/{id}
   */
  deleteWarehouse(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Warehouse deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Warehouse not found');
        } else {
          this.toastr.error('Failed to delete warehouse');
        }
        return throwError(() => error);
      })
    );
  }
}