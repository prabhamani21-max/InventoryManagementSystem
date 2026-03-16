import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  StoneRateHistory,
  StoneRateHistoryCreate,
  StoneRateHistoryUpdate,
  StoneRateSearch,
} from '../models/stone-rate-history.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Stone Rate History Service
 * Handles all HTTP operations for Stone Rate History management
 */
@Injectable({
  providedIn: 'root',
})
export class StoneRateHistoryService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/StoneRate`;

  /**
   * Get all current stone rates
   * GET /api/StoneRate/all
   */
  getAllCurrentRates(): Observable<StoneRateHistory[]> {
    return this.http
      .get<ApiResponse<StoneRateHistory[]>>(`${this.apiUrl}/all`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load stone rates');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current rate by stone ID
   * GET /api/StoneRate/stone/{stoneId}
   */
  getCurrentRateByStoneId(stoneId: number): Observable<StoneRateHistory | null> {
    return this.http
      .get<ApiResponse<StoneRateHistory>>(`${this.apiUrl}/stone/${stoneId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('Stone rate not found');
          } else {
            this.toastr.error('Failed to load stone rate');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get current stone rate by search criteria
   * GET /api/StoneRate/current
   */
  getCurrentRateBySearch(search: StoneRateSearch): Observable<StoneRateHistory | null> {
    return this.http
      .post<ApiResponse<StoneRateHistory>>(`${this.apiUrl}/current`, search)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('No stone rate found for the given criteria');
          } else {
            this.toastr.error('Failed to search stone rate');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get diamond rate by 4Cs
   * GET /api/StoneRate/diamond
   */
  getDiamondRateBy4Cs(
    carat: number,
    cut: string,
    color: string,
    clarity: string
  ): Observable<StoneRateHistory | null> {
    const params = new HttpParams()
      .set('carat', carat.toString())
      .set('cut', cut)
      .set('color', color)
      .set('clarity', clarity);

    return this.http
      .get<ApiResponse<StoneRateHistory>>(`${this.apiUrl}/diamond`, { params })
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return null;
        }),
        catchError((error) => {
          if (error.status === 404) {
            this.toastr.warning('No diamond rate found for the given 4Cs');
          } else {
            this.toastr.error('Failed to load diamond rate');
          }
          return throwError(() => error);
        })
      );
  }

  /**
   * Get rate history for a stone
   * GET /api/StoneRate/history/{stoneId}
   */
  getRateHistoryByStoneId(stoneId: number): Observable<StoneRateHistory[]> {
    return this.http
      .get<ApiResponse<StoneRateHistory[]>>(`${this.apiUrl}/history/${stoneId}`)
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
   * Get diamond rate card (all 4Cs combinations)
   * GET /api/StoneRate/diamond-rate-card
   */
  getDiamondRateCard(): Observable<StoneRateHistory[]> {
    return this.http
      .get<ApiResponse<StoneRateHistory[]>>(`${this.apiUrl}/diamond-rate-card`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            return response.Data;
          }
          return [];
        }),
        catchError((error) => {
          this.toastr.error('Failed to load diamond rate card');
          return throwError(() => error);
        })
      );
  }

  /**
   * Get latest rate per unit
   * GET /api/StoneRate/latest-rate/{stoneId}
   */
  getLatestRatePerUnit(
    stoneId: number,
    carat?: number,
    cut?: string,
    color?: string,
    clarity?: string,
    grade?: string
  ): Observable<number> {
    let params = new HttpParams();
    if (carat) params = params.set('carat', carat.toString());
    if (cut) params = params.set('cut', cut);
    if (color) params = params.set('color', color);
    if (clarity) params = params.set('clarity', clarity);
    if (grade) params = params.set('grade', grade);

    return this.http
      .get<ApiResponse<number>>(`${this.apiUrl}/latest-rate/${stoneId}`, { params })
      .pipe(
        map((response) => {
          if (response.Status && response.Data !== null) {
            return response.Data;
          }
          return 0;
        }),
        catchError((error) => {
          this.toastr.error('Failed to load latest rate');
          return throwError(() => error);
        })
      );
  }

  /**
   * Create a new stone rate entry
   * POST /api/StoneRate
   */
  createStoneRate(stoneRate: StoneRateHistoryCreate): Observable<StoneRateHistory> {
    return this.http
      .post<ApiResponse<StoneRateHistory>>(this.apiUrl, stoneRate)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Stone rate created successfully');
            return response.Data;
          }
          throw new Error('Failed to create stone rate');
        }),
        catchError((error) => {
          return throwError(() => error);
        })
      );
  }

  /**
   * Update an existing stone rate entry
   * PUT /api/StoneRate
   */
  updateStoneRate(stoneRate: StoneRateHistoryUpdate): Observable<StoneRateHistory> {
    return this.http
      .put<ApiResponse<StoneRateHistory>>(this.apiUrl, stoneRate)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            this.toastr.success('Stone rate updated successfully');
            return response.Data;
          }
          throw new Error('Failed to update stone rate');
        }),
        catchError((error) => {
          return throwError(() => error);
        })
      );
  }

  /**
   * Handle validation errors from backend
   * Displays specific field error messages
   */
  private handleValidationError(error: any): void {
    if (error.status === 400) {
      const errors = error.error?.errors;
      if (errors) {
        if (errors.StoneId) {
          this.toastr.error(errors.StoneId.join(', '));
        }
        if (errors.Carat) {
          this.toastr.error(errors.Carat.join(', '));
        }
        if (errors.Cut) {
          this.toastr.error(errors.Cut.join(', '));
        }
        if (errors.Color) {
          this.toastr.error(errors.Color.join(', '));
        }
        if (errors.Clarity) {
          this.toastr.error(errors.Clarity.join(', '));
        }
        if (errors.Grade) {
          this.toastr.error(errors.Grade.join(', '));
        }
        if (errors.RatePerUnit) {
          this.toastr.error(errors.RatePerUnit.join(', '));
        }
        if (errors.EffectiveDate) {
          this.toastr.error(errors.EffectiveDate.join(', '));
        }
      } else if (error.error?.message) {
        this.toastr.error(error.error.message);
      } else {
        this.toastr.error('Invalid stone rate data');
      }
    } else if (error.status === 404) {
      this.toastr.error('Stone rate not found');
    } else {
      this.toastr.error('An error occurred. Please try again.');
    }
  }
}
