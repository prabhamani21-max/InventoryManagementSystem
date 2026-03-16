import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GeneralStatus } from './general-status.model';
import { ApiResponse } from '../../core/models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * GeneralStatus Service
 * Handles all HTTP operations for GeneralStatus management
 */
@Injectable({
  providedIn: 'root',
})
export class GeneralStatusService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Status`;

  /**
   * Get all general statuses
   * GET /api/Status/userStatuses/GetAllUserStatuses
   */
  getAllGeneralStatuses(): Observable<GeneralStatus[]> {
    return this.http.get<ApiResponse<GeneralStatus[]>>(`${this.apiUrl}/userStatuses/GetAllUserStatuses`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load general statuses');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get general status by ID
   * GET /api/Status/userStatuses/GetById/{id}
   */
  getGeneralStatusById(id: number): Observable<GeneralStatus | null> {
    return this.http.get<ApiResponse<GeneralStatus>>(`${this.apiUrl}/userStatuses/GetById/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('General status not found');
        } else {
          this.toastr.error('Failed to load general status');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new general status
   * POST /api/Status/userStatuses/AddEdit
   */
  createGeneralStatus(status: GeneralStatus): Observable<GeneralStatus> {
    // For create, ensure Id is 0 if not set
    const createStatus = { ...status, id: status.id ?? 0 };
    return this.http.post<ApiResponse<GeneralStatus>>(`${this.apiUrl}/userStatuses/AddEdit`, createStatus).pipe(
      map((response) => {
        this.toastr.success('General status created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid general status data');
        } else {
          this.toastr.error('Failed to create general status');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing general status
   * POST /api/Status/userStatuses/AddEdit
   */
  updateGeneralStatus(status: GeneralStatus): Observable<GeneralStatus> {
    return this.http.post<ApiResponse<GeneralStatus>>(`${this.apiUrl}/userStatuses/AddEdit`, status).pipe(
      map((response) => {
        this.toastr.success('General status updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid general status data');
        } else {
          this.toastr.error('Failed to update general status');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a general status
   * DELETE /api/Status/userStatuses/Delete/{id}
   */
  deleteGeneralStatus(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/userStatuses/Delete/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('General status deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('General status not found');
        } else {
          this.toastr.error('Failed to delete general status');
        }
        return throwError(() => error);
      })
    );
  }
}