import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  JewelleryItem,
  JewelleryItemCreate,
  JewelleryItemUpdate,
} from '../models/jewellery-item.model';
import { ApiResponse } from '../models/api-response.model';
import { ToastrService } from 'ngx-toastr';

/**
 * JewelleryItem Service
 * Handles all HTTP operations for JewelleryItem management
 */
@Injectable({
  providedIn: 'root',
})
export class JewelleryItemService {
  private http = inject(HttpClient);
  private toastr = inject(ToastrService);
  private readonly apiUrl = `${environment.apiUrl}/JewelleryItem`;

  /**
   * Get all jewellery items
   * GET /api/JewelleryItem
   */
  getAllJewelleryItems(): Observable<JewelleryItem[]> {
    return this.http.get<ApiResponse<JewelleryItem[]>>(this.apiUrl).pipe(
      map((response) => {
        return response.Data || [];
      }),
      catchError((error) => {
        this.toastr.error('Failed to load jewellery items');
        return throwError(() => error);
      })
    );
  }

  /**
   * Get jewellery item by ID
   * GET /api/JewelleryItem/{id}
   */
  getJewelleryItemById(id: number): Observable<JewelleryItem | null> {
    return this.http.get<ApiResponse<JewelleryItem>>(`${this.apiUrl}/${id}`).pipe(
      map((response) => {
        return response.Data || null;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.warning('Jewellery item not found');
        } else {
          this.toastr.error('Failed to load jewellery item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Create a new jewellery item
   * POST /api/JewelleryItem
   */
  createJewelleryItem(item: JewelleryItemCreate): Observable<JewelleryItem> {
    return this.http.post<ApiResponse<JewelleryItem>>(this.apiUrl, item).pipe(
      map((response) => {
        this.toastr.success('Jewellery item created successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid jewellery item data');
        } else {
          this.toastr.error('Failed to create jewellery item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Update an existing jewellery item
   * PUT /api/JewelleryItem/{id}
   */
  updateJewelleryItem(item: JewelleryItemUpdate): Observable<JewelleryItem> {
    return this.http.put<ApiResponse<JewelleryItem>>(`${this.apiUrl}/${item.id}`, item).pipe(
      map((response) => {
        this.toastr.success('Jewellery item updated successfully');
        return response.Data;
      }),
      catchError((error) => {
        if (error.status === 400) {
          this.toastr.error(error.error?.message || 'Invalid jewellery item data');
        } else if (error.status === 404) {
          this.toastr.error('Jewellery item not found');
        } else {
          this.toastr.error('Failed to update jewellery item');
        }
        return throwError(() => error);
      })
    );
  }

  /**
   * Delete a jewellery item
   * DELETE /api/JewelleryItem/{id}
   */
  deleteJewelleryItem(id: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map((response) => {
        if (response.status === 200 || response.status === 204) {
          this.toastr.success('Jewellery item deleted successfully');
          return true;
        }
        return false;
      }),
      catchError((error) => {
        if (error.status === 404) {
          this.toastr.error('Jewellery item not found');
        } else {
          this.toastr.error('Failed to delete jewellery item');
        }
        return throwError(() => error);
      })
    );
  }
}
