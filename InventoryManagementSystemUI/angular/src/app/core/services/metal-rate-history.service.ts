import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map, throwError, forkJoin, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  MetalRateHistory,
  MetalRateHistoryCreate,
  MetalRateHistoryUpdate,
  MetalRateResponse,
  MetalRateHistoryEntry,
  DateRangeQuery,
} from '../models/metal-rate-history.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';
import { Metal } from '../models/metal.model';

/**
 * Metal Rate History Service
 * Handles all HTTP operations for Metal Rate History management
 */
@Injectable({
  providedIn: 'root',
})
export class MetalRateHistoryService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/MetalRate`;

  /**
   * Get all current metal rates
   * GET /api/MetalRate/current
   */
  getAllCurrentRates(): Observable<MetalRateResponse[]> {
    return this.http
      .get<ApiResponse<MetalRateResponse[]>>(`${this.apiUrl}/current`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load metal rates');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current rate by purity ID
   * GET /api/MetalRate/current/purity/{purityId}
   */
  getCurrentRateByPurity(purityId: number): Observable<MetalRateResponse | null> {
    return this.http
      .get<ApiResponse<MetalRateResponse>>(`${this.apiUrl}/current/purity/${purityId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Metal rate not found for this purity');
          } else {
            this.toastr.error('Failed to load metal rate');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current rates by metal ID
   * GET /api/MetalRate/current/metal/{metalId}
   */
  getCurrentRatesByMetal(metalId: number): Observable<MetalRateResponse[]> {
    return this.http
      .get<ApiResponse<MetalRateResponse[]>>(`${this.apiUrl}/current/metal/${metalId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load metal rates');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get rate history for a specific purity
   * GET /api/MetalRate/history/purity/{purityId}
   */
  getRateHistoryByPurity(purityId: number): Observable<MetalRateHistoryEntry[]> {
    return this.http
      .get<ApiResponse<MetalRateHistoryEntry[]>>(`${this.apiUrl}/history/purity/${purityId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load rate history');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get rate history for a metal within a date range
   * GET /api/MetalRate/history/metal/{metalId}?startDate=...&endDate=...
   */
  getRateHistoryByMetal(metalId: number, dateRange?: DateRangeQuery): Observable<MetalRateHistoryEntry[]> {
    let params = new HttpParams();
    
    if (dateRange?.startDate) {
      params = params.set('startDate', this.formatDateForQuery(dateRange.startDate));
    }
    if (dateRange?.endDate) {
      params = params.set('endDate', this.formatDateForQuery(dateRange.endDate));
    }

    return this.http
      .get<ApiResponse<MetalRateHistoryEntry[]>>(`${this.apiUrl}/history/metal/${metalId}`, { params })
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load rate history');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get all rate history across all metals
   * Fetches history for each metal and combines the results
   */
  getAllRateHistory(metals: Metal[], dateRange?: DateRangeQuery): Observable<MetalRateHistoryEntry[]> {
    if (!metals || metals.length === 0) {
      return of([]);
    }

    const historyRequests = metals.map(metal => 
      this.getRateHistoryByMetal(metal.id, dateRange).pipe(
        catchError(() => of([])) // Return empty array on error for individual metal
      )
    );

    return forkJoin(historyRequests).pipe(
      map(results => results.flat())
    );
  }

  /**
   * Create a new metal rate entry
   * POST /api/MetalRate
   */
  createMetalRate(metalRate: MetalRateHistoryCreate): Observable<MetalRateHistory> {
    return this.http
      .post<ApiResponse<MetalRateHistory>>(this.apiUrl, metalRate)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Metal rate created successfully');
            return response.Data;
          }
          throw new Error(response.Message || 'Failed to create metal rate');
        }),
        catchError((error) => {
          this.handleValidationError(error);
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing metal rate entry
   * PUT /api/MetalRate
   */
  updateMetalRate(metalRate: MetalRateHistoryUpdate): Observable<MetalRateHistory> {
    return this.http
      .put<ApiResponse<MetalRateHistory>>(this.apiUrl, metalRate)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Metal rate updated successfully');
            return response.Data;
          }
          throw new Error(response.Message || 'Failed to update metal rate');
        }),
        catchError((error) => {
          this.handleValidationError(error);
          return throwError(() => error);
        })
      );
  }

  /**
   * Format date for query string
   */
  private formatDateForQuery(date: Date | string): string {
    if (typeof date === 'string') {
      return date;
    }
    const d = new Date(date);
    return d.toISOString().split('T')[0];
  }

  /**
   * Handle validation errors from backend
   * Displays specific field error messages
   */
  private handleValidationError(error: any): void {
    if (error.status === 400) {
      const errors = error.error?.errors;
      if (errors) {
        if (errors.PurityId) {
          this.toastr.error(errors.PurityId.join(', '));
        }
        if (errors.RatePerGram) {
          this.toastr.error(errors.RatePerGram.join(', '));
        }
        if (errors.EffectiveDate) {
          this.toastr.error(errors.EffectiveDate.join(', '));
        }
      } else if (error.error?.message) {
        this.toastr.error(error.error.message);
      } else {
        this.toastr.error('Invalid metal rate data');
      }
    } else if (error.status === 404) {
      this.toastr.error('Metal rate not found');
    } else {
      this.toastr.error('An error occurred. Please try again.');
    }
  }
}
