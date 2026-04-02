import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { jwtDecode } from 'jwt-decode';
import { ToastrService } from 'ngx-toastr';
import {
  ClaimTypes,
  CustomJwtPayload,
} from '../../common/claim-types';
import { DecodedToken } from '../models/decoded-token.model';
import { Observable, of, throwError } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { Role } from '../models/role.model';
import { UserCreate } from '../models/user.model';
import { RoleEnum } from '../enums/role.enum';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private cookieService = inject(CookieService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private tokenExpirationTimer: any;

  public readonly authSessionKey = 'JewelleryManagement_AUTH_TOKEN';// JWT Token
public readonly decodedTokenKey = 'userDetails';  // decoded details from jwt
  constructor(private http: HttpClient) {}

  login(identifier: string, password: string) {
    return this.http
      .post<any>(`${environment.apiUrl}/User/login`, { identifier, password })
      .pipe(
        tap((response) => {
          if (response.Status && response.Data.token) {
            this.initializeAuthenticatedSession(response.Data.token);
          }
        }),
        map((response) => response.Data),
      );
  }

  logout(): void {
    this.clearTokenExpirationTimer(); // Clear timer on logout
    // 1. Remove the auth token from cookies
    this.removeToken();

    // 2. Clear all user-related data from storage
    this.clearAuthData();

    // 3. Navigate to login page
    this.router.navigate(['/jewelleryManagement/auth/sign-in']);

    // 4. Show logout notification
    this.showLogoutNotification();
  }

  private clearAuthData(): void {
    // Clear localStorage data
    localStorage.removeItem(this.decodedTokenKey);
  }

  private showLogoutNotification(): void {
    this.toastr.success('You have been logged out successfully');
  }


  saveToken(token: string): void {
    const expirationDate = new Date();
    expirationDate.setHours(expirationDate.getHours() + 1);

    this.cookieService.set(
      this.authSessionKey,
      token,
      expirationDate,
      '/',
      '',
      true,
      'Strict',
    );
  }

  initializeSession(): void {
    const token = this.token;
    if (!token) {
      return;
    }

    const decoded = this.persistDecodedToken(token);
    if (!decoded) {
      this.clearTokenExpirationTimer();
      this.removeToken();
      this.clearAuthData();
      return;
    }

    this.startTokenExpirationTimer(decoded.exp);
  }

  get token(): string | null {
    return this.cookieService.get(this.authSessionKey) || null;
  }

  removeToken(): void {
    this.cookieService.delete(this.authSessionKey, '/');
  }

  isAuthenticated(): boolean {
    return !!this.token;
  }

  getDecodedToken(): DecodedToken | null {
    const token = this.token;
    if (!token) return null;

    return this.persistDecodedToken(token);
  }
getUserInformation(): DecodedToken | null {
  const data = localStorage.getItem(this.decodedTokenKey);
  return data ? JSON.parse(data) as DecodedToken : null;
}
  getTokenExpiration(): Date | null {
    const decoded = this.getUserInformation();
    if (!decoded || !decoded.exp) return null;
    return new Date(decoded.exp * 1000);
  }

  isTokenExpired(): boolean {
    const expiration = this.getTokenExpiration();
    if (!expiration) return true;
    return expiration < new Date();
  }

  private initializeAuthenticatedSession(token: string): void {
    this.saveToken(token);
    const decoded = this.persistDecodedToken(token);
    if (!decoded) {
      return;
    }

    this.startTokenExpirationTimer(decoded.exp);
  }

  private persistDecodedToken(token: string): DecodedToken | null {
    try {
      const decoded = jwtDecode<CustomJwtPayload>(token);
      const mapped: DecodedToken = {
        userId: decoded[ClaimTypes.NAME_IDENTIFIER],
        name: decoded[ClaimTypes.NAME],
        roleId: decoded.RoleId,
        exp: decoded[ClaimTypes.EXPIRATION],
      };

      localStorage.setItem(this.decodedTokenKey, JSON.stringify(mapped));
      return mapped;
    } catch (e) {
      console.error('Error decoding token:', e);
      return null;
    }
  }

  private startTokenExpirationTimer(expirationEpochSeconds?: number): void {
    const expiration = expirationEpochSeconds
      ? new Date(expirationEpochSeconds * 1000)
      : this.getTokenExpiration();
    if (!expiration) return;

    this.clearTokenExpirationTimer();

    const now = new Date();
    const expiresIn = expiration.getTime() - now.getTime();

    if (expiresIn > 0) {
      this.tokenExpirationTimer = setTimeout(() => {
        this.toastr.warning('Your session has expired. Please log in again.');
        this.logout();
      }, expiresIn);
    } else {
      this.logout(); // Token already expired
    }
  }

  private clearTokenExpirationTimer(): void {
    if (this.tokenExpirationTimer) {
      clearTimeout(this.tokenExpirationTimer);
      this.tokenExpirationTimer = null;
    }
  }
  register(userData: UserCreate): Observable<any> {
    return this.http
      .post<ApiResponse<any>>(`${environment.apiUrl}/User/register`, userData)
      .pipe(
        catchError((error) => {
          return throwError(() => error);
        }),
      );
  }

  fetchRoles(): Observable<ApiResponse<Role[]>> {
    return this.http
      .get<ApiResponse<Role[]>>(`${environment.apiUrl}/Roles/GetAllRoles`)
      .pipe(
        tap((response) => {
          console.log('Roles API Response:', response); // Add logging
        }),
        catchError((error) => {
          console.error('Roles API Error:', error); // Add error logging
          return throwError(() => error);
        }),
      );
  }
  checkEmailExists(
    email: string,
    currentUserId: number | null = null,
  ): Observable<ApiResponse<{ message: string }>> {
    const params: any = { email };
    if (currentUserId !== null) {
      params.currentUserId = currentUserId;
    }
    return this.http
      .get<
        ApiResponse<{ message: string }>
      >(`${environment.apiUrl}/User/checkEmail`, { params: { email } })
      .pipe(
        catchError((error) => {
          // Handle 409 Conflict as a valid response
          if (error.status === 409) {
            return of({
              Status: false,
              Message: error.error?.Message || 'Email already exists',
              Data: {
                message: error.error?.Data?.message || 'Email already exists',
              },
              HttpStatus: 'Conflict',
            });
          }

          console.error('Email check error:', error);
          return of({
            Status: false,
            Message: 'Error checking email',
            Data: { message: 'Error checking email' },
            HttpStatus: '500',
          });
        }),
      );
  }

  checkContactExists(
    contactNo: string,
    currentUserId: number | null = null,
  ): Observable<ApiResponse<{ message: string }>> {
    const params: any = { contactNo };
    if (currentUserId !== null) {
      params.currentUserId = currentUserId;
    }
    return this.http
      .get<
        ApiResponse<{ message: string }>
      >(`${environment.apiUrl}/User/checkContact`, { params: { contactNo } })
      .pipe(
        catchError((error) => {
          // Handle 409 Conflict as a valid response
          if (error.status === 409) {
            return of({
              Status: false,
              Message: error.error?.Message || 'Contact already exists',
              Data: {
                message: error.error?.Data?.message || 'Contact already exists',
              },
              HttpStatus: 'Conflict',
            });
          }

          console.error('Contact check error:', error);
          return of({
            Status: false,
            Message: 'Error checking contact',
            Data: { message: 'Error checking contact' },
            HttpStatus: '500',
          });
        }),
      );
  }
  private readonly roleIdToEnumMap: Record<string, RoleEnum> = {
    '1': RoleEnum.SuperAdmin,
    '2': RoleEnum.Manager,
    '3': RoleEnum.Sales,
    '4': RoleEnum.Customer
  };

  getHomeRoute(roleId?: string | number | null): string[] {
    const currentRoleId = roleId ?? this.getUserInformation()?.roleId ?? this.getDecodedToken()?.roleId;
    const role = this.normalizeRoleId(currentRoleId);

    switch (role) {
      case RoleEnum.SuperAdmin:
      case RoleEnum.Manager:
        return ['/jewelleryManagement/admin/analytics'];
      case RoleEnum.Sales:
        return ['/jewelleryManagement/admin/sales-dashboard'];
      case RoleEnum.Customer:
        return ['/jewelleryManagement/admin/customer'];
      default:
        return ['/jewelleryManagement/auth/sign-in'];
    }
  }

  private normalizeRoleId(roleId: string | number | null | undefined): RoleEnum | null {
    if (roleId === null || roleId === undefined) {
      return null;
    }

    return this.roleIdToEnumMap[roleId.toString()] ?? null;
  }

  getCurrentUserRole(): RoleEnum | null {
    const decoded = this.getUserInformation() ?? this.getDecodedToken();
    return this.normalizeRoleId(decoded?.roleId);
  }
}
