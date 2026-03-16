import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Role, RoleCreate, RoleUpdate } from '../models/role.model';
import { ToastrService } from 'ngx-toastr';
import { ApiResponse } from '../models/api-response.model';

/**
 * Role Service
 * Handles all HTTP operations for Role management
 */
@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/Role`;

  /**
   * Get all roles
   * GET /api/Role/GetAllRoles
   */
  getAllRoles(): Observable<Role[]> {
    return this.http.get<ApiResponse<Role[]>>(`${this.apiUrl}/GetAllRoles`).pipe(
      map((response) => {
        return response?.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load roles');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get role by ID
   * GET /api/Role/GetByID/{id}
   */
  getRoleById(id: number): Observable<Role | null> {
    return this.http.get<ApiResponse<Role>>(`${this.apiUrl}/GetByID/${id}`).pipe(
      map((response) => {
        return response?.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Role not found');
        } else {
          this.toastr.error('Failed to load role');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Add or Edit a role
   * POST /api/Role/AddEdit
   */
  addEditRole(role: RoleCreate | RoleUpdate): Observable<Role> {
    return this.http.post<ApiResponse<Role>>(`${this.apiUrl}/AddEdit`, role).pipe(
      map((response) => {
        if (role.id) {
          this.toastr.success('Role updated successfully');
        } else {
          this.toastr.success('Role created successfully');
        }
        return response?.Data!;
      }),
      catchError((error) => {
        if (error.status === 409) {
          this.toastr.error('Role name already exists');
        } else if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid role data');
        } else {
          this.toastr.error('Failed to save role');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a role
   * DELETE /api/Role/Delete/{id}
   */
  deleteRole(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/Delete/${id}`).pipe(
      map(() => {
        this.toastr.success('Role deleted successfully');
        return true;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Role not found');
        } else {
          this.toastr.error('Failed to delete role');
        }
        return throwError(() => error);
      })
    );
  }
}
