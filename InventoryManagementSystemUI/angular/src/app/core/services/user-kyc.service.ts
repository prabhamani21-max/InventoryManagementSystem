import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserKyc, UserKycCreate, UserKycUpdate } from '../models/user-kyc.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * UserKyc Service
 * Handles all HTTP operations for UserKYC management
 */
@Injectable({
  providedIn: 'root',
})
export class UserKycService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/UserKyc`;

  /**
   * Get all user KYCs
   * GET /api/UserKyc
   */
  getAllUserKycs(): Observable<UserKyc[]> {
    return this.http.get<ApiResponse<UserKyc[]>>(`${this.apiUrl}`).pipe(
      map((response) => {
        return response?.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load user KYCs');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get user KYC by ID
   * GET /api/UserKyc/{id}
   */
  getUserKycById(id: number): Observable<UserKyc | null> {
    return this.http.get<ApiResponse<UserKyc>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response?.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('User KYC not found');
        } else {
          this.toastr.error('Failed to load user KYC');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get user KYC by User ID
   * GET /api/UserKyc/user/{userId}
   */
  getUserKycByUserId(userId: number): Observable<UserKyc | null> {
    return this.http.get<ApiResponse<UserKyc>>(`${this.apiUrl}/user/${userId}`).pipe(
      map((response) => {
        return response?.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('User KYC not found for this user');
        } else {
          this.toastr.error('Failed to load user KYC');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get customer KYC status
   * GET /api/UserKyc/customer/{customerId}/status
   */
  getCustomerKycStatus(customerId: number): Observable<{
    hasKyc: boolean;
    isVerified: boolean;
    panCardNumber: string | null;
    aadhaarCardNumber: string | null;
  }> {
    return this.http.get<{
      hasKyc: boolean;
      isVerified: boolean;
      panCardNumber: string | null;
      aadhaarCardNumber: string | null;
    }>(`${this.apiUrl}/customer/${customerId}/status`).pipe(
      map((response) => {
        return response;
      }),
      catchError((error) => {
        this.toastr.error('Failed to load customer KYC status');
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new user KYC
   * POST /api/UserKyc
   */
  createUserKyc(userKyc: UserKycCreate): Observable<UserKyc> {
    return this.http.post<ApiResponse<UserKyc>>(`${this.apiUrl}`, userKyc).pipe(
      map((response) => {
        this.toastr.success('User KYC created successfully');
        return response?.Data!;
      }),
      catchError((error) => {
        if (error.status === 409) {
          const errorMessage = error.error?.message || 'KYC already exists for this user';
          this.toastr.error(errorMessage);
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid KYC data');
        } else {
          this.toastr.error('Failed to create user KYC');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing user KYC
   * PUT /api/UserKyc/{id}
   */
  updateUserKyc(userKyc: UserKycUpdate): Observable<UserKyc> {
    return this.http.put<ApiResponse<UserKyc>>(`${this.apiUrl}/${userKyc.id}`, userKyc).pipe(
      map((response) => {
        this.toastr.success('User KYC updated successfully');
        return response?.Data!;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('User KYC not found');
        } else if (error.status === 409) {
          const errorMessage = error.error?.message || 'KYC already exists for this user';
          this.toastr.error(errorMessage);
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid KYC data');
        } else {
          this.toastr.error('Failed to update user KYC');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a user KYC
   * DELETE /api/UserKyc/{id}
   */
  deleteUserKyc(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`).pipe(
      map(() => {
        this.toastr.success('User KYC deleted successfully');
        return true;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('User KYC not found');
        } else {
          this.toastr.error('Failed to delete user KYC');
        }
        return throwError(() => error);
      })
    );
  }
}
