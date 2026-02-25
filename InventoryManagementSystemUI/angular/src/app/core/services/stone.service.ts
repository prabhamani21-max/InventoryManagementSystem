import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Stone,
  StoneCreate,
  StoneUpdate,
} from '../models/stone.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Stone Service
 * Handles all HTTP operations for Stone management
 */
@Injectable({
  providedIn: 'root',
})
export class StoneService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Stone`;

  /**
   * Get all stones
   * GET /api/Stone
   */
  getAllStones(): Observable<Stone[]> {
    return this.http
      .get<ApiResponse<Stone[]>>(this.apiUrl)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load stones');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get stone by ID
   * GET /api/Stone/{id}
   */
  getStoneById(id: number): Observable<Stone | null> {
    return this.http
      .get<ApiResponse<Stone>>(`${this.apiUrl}/${id}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Stone not found');
          } else {
            this.toastr.error('Failed to load stone');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Search stones by name
   * GET /api/Stone/search?name={name}
   */
  searchStonesByName(name: string): Observable<Stone[]> {
    return this.http
      .get<ApiResponse<Stone[]>>(`${this.apiUrl}/search`, { params: { name } })
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to search stones');
          return throwError(() => error);
        })
      );
  }

  /**
   * Create a new stone
   * POST /api/Stone
   */
  createStone(stone: StoneCreate): Observable<Stone> {
    return this.http
      .post<ApiResponse<Stone>>(this.apiUrl, stone)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Stone created successfully');
            return response.Data;
          }
          throw new Error('Failed to create stone');
        }),
        catchError((error) => {
          if (error.status === 409) {
            this.toastr.error('A stone with this name already exists');
          } else if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid stone data');
          } else {
            this.toastr.error('Failed to create stone');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing stone
   * PUT /api/Stone/{id}
   */
  updateStone(stone: StoneUpdate): Observable<Stone> {
    return this.http
      .put<ApiResponse<Stone>>(`${this.apiUrl}/${stone.id}`, stone)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Stone updated successfully');
            return response.Data;
          }
          throw new Error('Failed to update stone');
        }),
        catchError((error) => {
          if (error.status === 409) {
            this.toastr.error('A stone with this name already exists');
          } else if (error.status === 400) {
            this.toastr.error(error.error?.message || 'Invalid stone data');
          } else if (error.status === 404) {
            this.toastr.error('Stone not found');
          } else {
            this.toastr.error('Failed to update stone');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Delete a stone
   * DELETE /api/Stone/{id}
   */
  deleteStone(id: number): Observable<boolean> {
    return this.http
      .delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' })
      .pipe(
        map((response) => {
          if (response.status === 200 || response.status === 204) {
            this.toastr.success('Stone deleted successfully');
            return true;
          }
          return false;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.error('Stone not found');
          } else {
            this.toastr.error('Failed to delete stone');
          }
          return throwError(() => error);
        })
      );
  }
}
