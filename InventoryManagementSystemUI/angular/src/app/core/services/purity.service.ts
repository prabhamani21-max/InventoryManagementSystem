import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Purity, PurityCreate, PurityUpdate } from '../models/purity.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Purity Service
 * Handles all HTTP operations for Purity management
 */
@Injectable({
  providedIn: 'root',
})
export class PurityService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Purity`;

  /**
   * Get all purities
   * GET /api/Purity
   */
  getAllPurities(): Observable<Purity[]> {
    return this.http.get<ApiResponse<Purity[]>>(this.apiUrl).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          return response.Data;
        }
        return [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load purities');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get purity by ID
   * GET /api/Purity/{id}
   */
  getPurityById(id: number): Observable<Purity | null> {
    return this.http.get<ApiResponse<Purity>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          return response.Data;
        }
        return null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Purity not found');
        } else {
          this.toastr.error('Failed to load purity');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new purity
   * POST /api/Purity
   */
  createPurity(purity: PurityCreate): Observable<Purity> {
    return this.http.post<ApiResponse<Purity>>(this.apiUrl, purity).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          this.toastr.success('Purity created successfully');
          return response.Data;
        }
        throw new Error(response.Message || 'Failed to create purity');
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid purity data');
        } else {
          this.toastr.error('Failed to create purity');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing purity
   * PUT /api/Purity/{id}
   */
  updatePurity(id: number, purity: PurityUpdate): Observable<Purity> {
    return this.http.put<ApiResponse<Purity>>(`${this.apiUrl}/${id}`, purity).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          this.toastr.success('Purity updated successfully');
          return response.Data;
        }
        throw new Error(response.Message || 'Failed to update purity');
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid purity data');
        } else if (error.status === 404) {
          this.toastr.error('Purity not found');
        } else {
          this.toastr.error('Failed to update purity');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a purity
   * DELETE /api/Purity/{id}
   */
  deletePurity(id: number): Observable<boolean> {
    return this.http
      .delete<void>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map((response) => {
          if (response.status === 200 || response.status === 204) {
            this.toastr.success('Purity deleted successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.error('Purity not found');
          } else {
            this.toastr.error('Failed to delete purity');
          }
          return throwError(() => error);
        })
      );
  }
}
