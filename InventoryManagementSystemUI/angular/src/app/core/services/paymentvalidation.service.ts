import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from 'src/environments/environment';
import { KycStatus, PaymentValidationRequest, PaymentValidationResponse } from '../models/kyc.model';
import { ApiResponse } from '../models/api-response.model';


/**
 * KycService
 * Service for handling KYC-related operations
 */
@Injectable({
  providedIn: 'root',
})
export class PaymentValidationService {
  private apiUrl = `${environment.apiUrl}/UserKyc`;
  private paymentApiUrl = `${environment.apiUrl}/Payment`;

  constructor(private http: HttpClient) {}

  /**
   * Get KYC status for a customer
   * @param customerId The customer ID
   * @returns Observable of KycStatus
   */
  getCustomerKycStatus(customerId: number): Observable<KycStatus> {
    return this.http.get<ApiResponse<KycStatus>>(`${this.apiUrl}/customer/${customerId}/status`).pipe(
      map((response) => response.Data)
    );
  }

  /**
   * Validate payment for high-value transaction rules
   * @param request Payment validation request
   * @returns Observable of PaymentValidationResponse
   */
  validatePayment(request: PaymentValidationRequest): Observable<PaymentValidationResponse> {
    return this.http.post<PaymentValidationResponse>(`${this.paymentApiUrl}/validate`, request);
  }
}
