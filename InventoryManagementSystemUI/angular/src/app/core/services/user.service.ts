import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, map, of, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  User,
  UserCreate,
  UserUpdate,

} from '../models/user.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from '../services/auth.service';

/**
 * User Service
 * Handles all HTTP operations for User management
 */
@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
    private authService = inject(AuthenticationService);
    
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private readonly apiUrl = `${environment.apiUrl}/User`;
  currentUser: any;

getCurrentUser(): Observable<
    (User & { profileImageUrl: string }) | null
  > {
    const decodedToken = this.authService.getUserInformation();
    if (!decodedToken?.userId) {
      return of(null);
    }

    return this.http
      .get<
        ApiResponse<User>
      >(`${environment.apiUrl}/User/GetUserById/${decodedToken.userId}`)
      .pipe(
        map((response) => {
          if (response.Status && response.Data) {
            const user = response.Data;

            // Construct the derived profileImageUrl (not part of backend model)
            const profileImageUrl = 
            'assets/images/users/dummy-avatar.jpg';

            const userWithImageUrl = {
              ...user,
              profileImageUrl,
            };

            this.currentUserSubject.next(userWithImageUrl);
            return userWithImageUrl;
          }
          return null;
        }),
        catchError((error) => {
          console.error('Error fetching user:', error);
          return of(null);
        }),
      );
  }
  /**
   * Register new user
   * POST /api/User/register
   */
  registerUser(user: UserCreate): Observable<{ message: string; user?: User }> {
    return this.http.post<{ message: string; user?: User }>(`${this.apiUrl}/register`, user).pipe(
      map((response) => {
        this.toastr.success('User registered successfully');
        return response;
      }),
      catchError((error) => {
        if (error.status === 409) {
          // Conflict - email or contact number already exists
          if (error.error?.errors?.Email) {
            this.toastr.error('Email already registered');
          } else if (error.error?.errors?.ContactNo) {
            this.toastr.error('Contact number already registered');
          } else {
            this.toastr.error('User already exists');
          }
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid user data');
        } else {
          this.toastr.error('Failed to register user');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Get all users
   * GET /api/User
   * Note: This endpoint may not exist in the backend. Added for completeness.
   */
  getAllUsers(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(`${this.apiUrl}/GetAllUsers`).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load users');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get user by ID
   * GET /api/User/{id}
   */
  getUserById(id: number): Observable<User | null> {
    return this.http.get<ApiResponse<User>>(`${this.apiUrl}/GetUserById/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('User not found');
        } else {
          this.toastr.error('Failed to load user');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing user
   * PUT /api/User/{id}
   * Note: This endpoint may not exist in the backend. Added for completeness.
   */
  updateUser(user: UserUpdate): Observable<User> {
    return this.http.put<ApiResponse<User>>(`${this.apiUrl}/${user.id}`, user).pipe(
      map((response) => {
        this.toastr.success('User updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid user data');
        } else if (error.status === 404) {
          this.toastr.error('User not found');
        } else {
          this.toastr.error('Failed to update user');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a user
   * DELETE /api/User/{id}
   * Note: This endpoint may not exist in the backend. Added for completeness.
   */
  deleteUser(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('User deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('User not found');
        } else {
          this.toastr.error('Failed to delete user');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Check if email exists
   * GET /api/User/check-email?email={email}
   */
  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-email`, { params: { email } }).pipe(
      map((response) => response.exists),
      catchError(() => {
        return throwError(() => new Error('Failed to check email'));
      })
    );
  }

  /**
   * Check if contact number exists
   * GET /api/User/check-contact?contactNumber={contactNumber}
   */
  checkContactNumberExists(contactNumber: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-contact`, { params: { contactNumber } }).pipe(
      map((response) => response.exists),
      catchError(() => {
        return throwError(() => new Error('Failed to check contact number'));
      })
    );
  }

  /**
   * Get all customers (users with roleId = 4)
   * GET /api/User/GetAllUsers - then filter by roleId
   */
  getCustomers(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(`${this.apiUrl}/GetAllUsers`).pipe(
      map((response) => {
        // Filter users with roleId = 4 (Customer)
        const customers = (response.Data || []).filter(user => user.roleId === 4);
        return customers;
      }),
      catchError((error) => {
        this.toastr.error('Failed to load customers');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get customers served by the currently logged-in sales person
   * GET /api/User/my-customers
   */
  getMyCustomers(): Observable<User[]> {
    return this.http.get<ApiResponse<User[]>>(`${this.apiUrl}/my-customers`).pipe(
      map((response) => {
        if (response.Status && response.Data) {
          if ((response.Data as any).data) {
            return (response.Data as any).data as User[];
          }
          return response.Data as User[];
        }
        return [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load your customers');
        return throwError(() => error);
      })
    );
  }
}