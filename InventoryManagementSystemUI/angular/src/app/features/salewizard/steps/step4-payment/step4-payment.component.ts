import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';
import { PaymentCreate, Payment, PaymentMethod, TransactionType, getPaymentMethodLabel } from 'src/app/core/models/payment.model';
import { formatCurrency } from 'src/app/core/models/sale-order-item.model';
import { HIGH_VALUE_TRANSACTION_THRESHOLD, isHighValueTransaction } from 'src/app/core/models/kyc.model';
import { ToastrService } from 'ngx-toastr';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

/**
 * Step4PaymentComponent
 * Handles payment collection for the sale wizard
 * Auto-fills orderId and customerId from wizard state
 * Shows amount to be paid and balance remaining
 * Auto-proceeds to invoice generation after payment
 * 
 * High-Value Transaction Rules:
 * - Transactions above ₹2,00,000 require KYC verification
 * - Cash payment is NOT allowed for high-value transactions
 */
@Component({
  selector: 'app-step4-payment',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './step4-payment.component.html',
  styleUrl: './step4-payment.component.scss',
})
export class Step4PaymentComponent implements OnInit, OnDestroy {
  private wizardService = inject(SaleWizardService);
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private modalService = inject(NgbModal);
  
  private destroy$ = new Subject<void>();
  
  // State
  wizardState: SaleWizardState = this.wizardService.getCurrentState();
  
  // Payment form
  paymentForm!: UntypedFormGroup;
  isSubmitting: boolean = false;
  
  // Payment method options
  paymentMethodOptions = [
    { value: PaymentMethod.CASH, label: 'Cash' },
    { value: PaymentMethod.CARD, label: 'Card' },
    { value: PaymentMethod.BANK_TRANSFER, label: 'Bank Transfer' },
    { value: PaymentMethod.UPI, label: 'UPI' },
    { value: PaymentMethod.CHEQUE, label: 'Cheque' },
  ];
  
  // Track payments made in this session
  sessionPayments: Payment[] = [];

  ngOnInit(): void {
    this.initializeForm();
    
    // Subscribe to wizard state
    this.wizardService.state$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.wizardState = state;
        this.autoFillFormFromWizard();
      });
    
    // Auto-fill form with wizard data
    this.autoFillFormFromWizard();
    
    // Ensure KYC status is loaded if customer is already selected
    this.ensureKycStatusLoaded();
    
    // Check if returning from KYC page - refresh KYC status
    this.checkAndRefreshKycStatus();
  }

  /**
   * Ensure KYC status is loaded if customer is already selected
   */
  private ensureKycStatusLoaded(): void {
    const customer = this.wizardState.selectedCustomer;
    if (customer && customer.id) {
      // Always refresh on step entry to avoid stale KYC state after returning from KYC form.
      this.wizardService.refreshKycStatus();
    }
  }

  /**
   * Check if user is returning from KYC page and refresh KYC status
   */
  private checkAndRefreshKycStatus(): void {
    // Check immediately if query param already exists (component already loaded)
    const params = this.route.snapshot.queryParamMap;
    if (params.get('kycCompleted') === 'true') {
      this.wizardService.refreshKycStatus();
      this.router.navigate([], {
        relativeTo: this.route,
        queryParams: { kycCompleted: null },
        queryParamsHandling: 'merge'
      });
      return;
    }
    
    // Also subscribe to future changes
    this.route.queryParams
      .pipe(takeUntil(this.destroy$))
      .subscribe((params) => {
        if (params['kycCompleted'] === 'true') {
          // Refresh the wizard state to get updated KYC status
          this.wizardService.refreshKycStatus();
          // Clear the query param
          this.router.navigate([], {
            relativeTo: this.route,
            queryParams: { kycCompleted: null },
            queryParamsHandling: 'merge'
          });
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize the payment form
   */
  initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.paymentForm = this.fb.group({
      orderId: [{ value: null, disabled: true }],
      orderType: [{ value: 'Sale', disabled: true }],
      customerId: [{ value: null, disabled: true }],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      paymentMethod: [PaymentMethod.CASH, [Validators.required]],
      paymentDate: [today, [Validators.required]],
      referenceNumber: [''],
    });
  }

  /**
   * Auto-fill form with data from wizard state
   */
  private autoFillFormFromWizard(): void {
    const state = this.wizardState;
    
    // Set orderId from sale order
    if (state.saleOrder?.id) {
      this.paymentForm.patchValue({
        orderId: state.saleOrder.id
      }, { emitEvent: false });
    }
    
    // Set customerId from selected customer
    if (state.selectedCustomer?.id) {
      this.paymentForm.patchValue({
        customerId: state.selectedCustomer.id
      }, { emitEvent: false });
    }
    
    // Set default amount to balance due
    if (state.balanceDue > 0 && !this.paymentForm.get('amount')?.value) {
      this.paymentForm.patchValue({
        amount: state.balanceDue
      }, { emitEvent: false });
    }
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.paymentForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.paymentForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get amount to be paid (order total)
   */
  getAmountToBePaid(): number {
    return this.wizardState.orderTotal || 0;
  }

  /**
   * Get total amount paid so far
   */
  getTotalPaid(): number {
    return this.wizardState.totalPaid || 0;
  }

  /**
   * Get balance remaining
   */
  getBalanceRemaining(): number {
    return this.wizardState.balanceDue || 0;
  }

  /**
   * Get balance after this payment
   */
  getBalanceAfterPayment(): number {
    const amountBeingPaid = this.paymentForm.get('amount')?.value || 0;
    const currentBalance = this.wizardState.balanceDue || 0;
    return Math.max(0, currentBalance - amountBeingPaid);
  }

  /**
   * Check if payment is complete (balance is zero or negative)
   */
  isPaymentComplete(): boolean {
    return this.wizardState.balanceDue <= 0;
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  // ==================== High-Value Transaction Methods ====================

  /**
   * Check if this is a high-value transaction
   */
  isHighValueTransaction(): boolean {
    return this.wizardState.isHighValueTransaction;
  }

  /**
   * Check if KYC is required for this transaction
   */
  isKycRequired(): boolean {
    return this.wizardState.kycValidationRequired;
  }

  /**
   * Check if KYC is required but not yet verified (needs attention)
   */
  isKycPending(): boolean {
    return this.isKycRequired() && !this.isKycVerified();
  }

  /**
   * Check if customer KYC is verified
   */
  isKycVerified(): boolean {
    return this.wizardState.customerKycStatus?.isVerified ?? false;
  }

  /**
   * Get available payment methods based on transaction value
   * Cash is disabled for high-value transactions with verified KYC
   */
  getAvailablePaymentMethods(): { value: PaymentMethod; label: string }[] {
    if (this.isHighValueTransaction() && this.isKycVerified()) {
      // Exclude cash for high-value transactions with verified KYC
      return this.paymentMethodOptions.filter(m => m.value !== PaymentMethod.CASH);
    }
    return this.paymentMethodOptions;
  }

  /**
   * Check if cash payment is disabled
   */
  isCashPaymentDisabled(): boolean {
    return this.isHighValueTransaction() && this.isKycVerified();
  }

  /**
   * Handle form submission - add payment
   */
  onSubmit(): void {
    if (this.paymentForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    // Validate amount doesn't exceed balance
    const amount = this.paymentForm.get('amount')?.value;
    if (amount > this.wizardState.balanceDue) {
      this.toastr.error('Payment amount cannot exceed balance due');
      return;
    }

    // Validate KYC for high-value transactions - only show error if KYC is required but NOT verified
    if (this.isKycRequired() && !this.isKycVerified()) {
      this.toastr.error('KYC verification is required for transactions above ₹2,00,000');
      return;
    }

    // Validate cash payment for high-value transactions
    const paymentMethod = this.paymentForm.get('paymentMethod')?.value;
    if (this.isHighValueTransaction() && paymentMethod === PaymentMethod.CASH && this.isKycVerified()) {
      this.toastr.error('Cash payment is not allowed for transactions above ₹2,00,000');
      return;
    }

    this.isSubmitting = true;
    const formValue = this.paymentForm.value;

    const paymentData: PaymentCreate = {
      orderId: this.wizardState.saleOrder?.id ?? null,
      orderType: TransactionType.SALE,
      customerId: this.wizardState.selectedCustomer?.id ?? null,
      salesPersonId: null,
      amount: formValue.amount,
      paymentMethod: formValue.paymentMethod,
      paymentDate: formValue.paymentDate,
      referenceNumber: formValue.referenceNumber || null,
      statusId: 1, // Active status
      orderTotal: this.wizardState.orderTotal, // Include order total for backend validation
    };

    this.wizardService.addPayment(paymentData).subscribe({
      next: (payment) => {
        this.isSubmitting = false;
        this.sessionPayments.push(payment);
        this.toastr.success('Payment recorded successfully');
        
        // Reset amount field for next payment
        this.paymentForm.patchValue({
          amount: this.wizardState.balanceDue || null,
          referenceNumber: ''
        });
        
        // Check if payment is complete, then auto-proceed to invoice
        if (this.wizardState.balanceDue <= 0) {
          this.proceedToInvoice();
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        // Handle validation errors from backend
        if (error.error?.requiresKyc) {
          this.toastr.error(error.error.message || 'KYC verification is required');
        } else if (error.error?.error === 'CASH_PAYMENT_NOT_ALLOWED') {
          this.toastr.error(error.error.message || 'Cash payment is not allowed for high-value transactions');
        } else {
          this.toastr.error('Failed to record payment');
        }
      },
    });
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.paymentForm.controls).forEach((key) => {
      this.paymentForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Proceed to invoice generation step
   */
  proceedToInvoice(): void {
    this.wizardService.nextStep();
    this.router.navigate(['../invoice'], { relativeTo: this.route });
  }

  /**
   * Skip payment and proceed to invoice (for partial payments)
   *

  /**
   * Get payment method label
   */
  getPaymentMethodLabel(method: PaymentMethod | string): string {
    return getPaymentMethodLabel(method);
  }

  /**
   * Get total of session payments
   */
  getSessionPaymentsTotal(): number {
    return this.sessionPayments.reduce((sum, p) => sum + p.amount, 0);
  }

  /**
   * Navigate to KYC page with return URL to redirect back after KYC completion
   */
  goToKycPage(): void {
    const customerId = this.wizardState.selectedCustomer?.id;
    if (customerId) {
      // Get current URL for redirecting back after KYC completion
      const returnUrl = this.router.createUrlTree(['../payment'], { relativeTo: this.route }).toString();
      this.router.navigate(['jewelleryManagement/admin/userkyc/add'], {
        queryParams: { 
          customerId: customerId,
          returnUrl: returnUrl
        }
      });
    }
  }
}
