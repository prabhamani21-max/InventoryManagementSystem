import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  TcsCalculationRequest,
  TcsCalculationResponse,
  TcsTransaction,
  Form26QReport,
  CustomerTcsSummary,
  TcsDashboardSummary,
  getCurrentFinancialYear
} from '../models/tcs.model';

/**
 * TCS (Tax Collected at Source) Service
 * Handles API calls for TCS calculation and reporting
 */
@Injectable({
  providedIn: 'root'
})
export class TcsService {
  private readonly apiUrl = `${environment.apiUrl}/tcs`;

  constructor(private http: HttpClient) {}

  /**
   * Calculate TCS for a sale transaction
   */
  calculateTcs(request: TcsCalculationRequest): Observable<TcsCalculationResponse> {
    return this.http.post<{ success: boolean; data: TcsCalculationResponse }>(
      `${this.apiUrl}/calculate`, request
    ).pipe(map(response => response.data));
  }

  /**
   * Get customer TCS summary for a financial year
   */
  getCustomerSummary(customerId: number, financialYear?: string): Observable<CustomerTcsSummary> {
    const fy = financialYear || getCurrentFinancialYear();
    const params = new HttpParams().set('financialYear', fy);

    return this.http.get<{ success: boolean; data: CustomerTcsSummary }>(
      `${this.apiUrl}/customer/${customerId}/summary`, { params }
    ).pipe(map(response => response.data));
  }

  /**
   * Get TCS transactions for a customer
   */
  getCustomerTransactions(customerId: number, financialYear?: string): Observable<TcsTransaction[]> {
    let params = new HttpParams();
    if (financialYear) {
      params = params.set('financialYear', financialYear);
    }

    return this.http.get<{ success: boolean; data: TcsTransaction[] }>(
      `${this.apiUrl}/customer/${customerId}/transactions`, { params }
    ).pipe(map(response => response.data));
  }

  /**
   * Generate Form 26Q report for quarterly TCS return
   */
  generateForm26Q(financialYear: string, quarter: number): Observable<Form26QReport> {
    const params = new HttpParams()
      .set('financialYear', financialYear)
      .set('quarter', quarter.toString());

    return this.http.get<{ success: boolean; data: Form26QReport }>(
      `${this.apiUrl}/report/form26q`, { params }
    ).pipe(map(response => response.data));
  }

  /**
   * Get all TCS transactions for a financial year/quarter
   */
  getTransactions(financialYear?: string, quarter?: number): Observable<TcsTransaction[]> {
    const fy = financialYear || getCurrentFinancialYear();
    let params = new HttpParams().set('financialYear', fy);
    if (quarter) {
      params = params.set('quarter', quarter.toString());
    }

    return this.http.get<{ success: boolean; data: TcsTransaction[] }>(
      `${this.apiUrl}/transactions`, { params }
    ).pipe(map(response => response.data));
  }

  /**
   * Get TCS dashboard summary for a financial year
   */
  getDashboardSummary(financialYear?: string): Observable<TcsDashboardSummary> {
    const fy = financialYear || getCurrentFinancialYear();
    const params = new HttpParams().set('financialYear', fy);

    return this.http.get<{ success: boolean; data: TcsDashboardSummary }>(
      `${this.apiUrl}/dashboard`, { params }
    ).pipe(map(response => response.data));
  }

  /**
   * Get TCS transaction by ID
   */
  getTcsTransactionById(id: number): Observable<TcsTransaction> {
    return this.http.get<{ success: boolean; data: TcsTransaction }>(
      `${this.apiUrl}/${id}`
    ).pipe(map(response => response.data));
  }

  /**
   * Get TCS transaction by invoice ID
   */
  getTcsTransactionByInvoiceId(invoiceId: number): Observable<TcsTransaction> {
    return this.http.get<{ success: boolean; data: TcsTransaction }>(
      `${this.apiUrl}/invoice/${invoiceId}`
    ).pipe(map(response => response.data));
  }

  /**
   * Get current financial year
   */
  getCurrentFinancialYear(): Observable<{ financialYear: string; description: string }> {
    return this.http.get<{ success: boolean; data: { financialYear: string; description: string } }>(
      `${this.apiUrl}/current-financial-year`
    ).pipe(map(response => response.data));
  }

  /**
   * Check customer PAN status for TCS
   */
  checkCustomerPANStatus(customerId: number): Observable<{
    customerId: number;
    hasValidPAN: boolean;
    panNumber: string | null;
    tcsRate: number;
    message: string;
  }> {
    return this.http.get<{ success: boolean; data: {
      customerId: number;
      hasValidPAN: boolean;
      panNumber: string | null;
      tcsRate: number;
      message: string;
    } }>(
      `${this.apiUrl}/customer/${customerId}/pan-status`
    ).pipe(map(response => response.data));
  }
}
