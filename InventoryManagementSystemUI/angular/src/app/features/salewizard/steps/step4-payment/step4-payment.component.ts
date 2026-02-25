import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';
import { PaymentCreate, Payment, PaymentMethod, TransactionType, getPaymentMethodLabel } from 'src/app/core/models/payment.model';
import { formatCurrency } from 'src/app/core/models/sale-order-item.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Step4PaymentComponent
 * Handles payment collection for the sale wizard
 * Auto-fills orderId and customerId from wizard state
 * Shows amount to be paid and balance remaining
 * Auto-proceeds to invoice generation after payment
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
      error: () => {
        this.isSubmitting = false;
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
   */
  skipPayment(): void {
    if (this.wizardState.totalPaid < this.wizardState.orderTotal * 0.5) {
      this.toastr.warning('At least 50% payment is required before proceeding');
      return;
    }
    this.proceedToInvoice();
  }

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
}
