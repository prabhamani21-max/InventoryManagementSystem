import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Metal, MetalCreate, MetalUpdate } from '../models/metal.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Metal Service
 * Handles all HTTP operations for Metal management
 */
@Injectable({
  providedIn: 'root',
})
export class MetalService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Metal`;

  /**
   * Get all metals
   * GET /api/Metal
   */
  getAllMetals(): Observable<Metal[]> {
    return this.http
      .get<ApiResponse<Metal[]>>(this.apiUrl)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load metals');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get metal by ID
   * GET /api/Metal/{id}
   */
  getMetalById(id: number): Observable<Metal | null> {
    return this.http
      .get<ApiResponse<Metal>>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Metal not found');
          } else {
            this.toastr.error('Failed to load metal');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Create a new metal
   * POST /api/Metal
   */
  createMetal(metal: MetalCreate): Observable<Metal> {
    return this.http
      .post<ApiResponse<Metal>>(this.apiUrl, metal)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Metal created successfully');
            return response.Data;
          }
          throw new Error(response.Message || 'Failed to create metal');
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid metal data');
          } else {
            this.toastr.error('Failed to create metal');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing metal
   * PUT /api/Metal/{id}
   */
  updateMetal(id: number, metal: MetalUpdate): Observable<Metal> {
    return this.http
      .put<ApiResponse<Metal>>(`${this.apiUrl}/${id}`, metal)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Metal updated successfully');
            return response.Data;
          }
          throw new Error(response.Message || 'Failed to update metal');
        }),
        catchError((error) => {
          if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid metal data');
          } else if (error.status === 404) {
            this.toastr.error('Metal not found');
          } else {
            this.toastr.error('Failed to update metal');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Delete a metal
   * DELETE /api/Metal/{id}
   */
  deleteMetal(id: number): Observable<boolean> {
    return this.http
      .delete<void>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map((response) => {
          if (response.status === 204) {
            this.toastr.success('Metal deleted successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.error('Metal not found');
          } else {
            this.toastr.error('Failed to delete metal');
          }
          return throwError(() => error);
        })
      );
  }
}