// angular import
import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoicePaymentService } from 'src/app/core/services/invoice-payment.service';
import { SaleWizardService } from 'src/app/core/services/sale-wizard.service';
import {
  Payment,
  PaymentCreate,
  PaymentUpdate,
  PaymentMethod,
  TransactionType,
} from 'src/app/core/models/invoice-payment.model';
import {
  PaymentCreate as WizardPaymentCreate,
  Payment as WizardPayment,
} from 'src/app/core/models/payment.model';
import { ToastrService } from 'ngx-toastr';

/**
 * InvoicePayment Form Component
 * Handles both create and edit operations for payments
 * Supports wizard mode for integration with sale wizard
 */
@Component({
  selector: 'app-invoicepaymentform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './invoicepaymentform.html',
  styleUrl: './invoicepaymentform.scss',
})
export class Invoicepaymentform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private invoicePaymentService = inject(InvoicePaymentService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private wizardService = inject(SaleWizardService);

  // Wizard mode inputs
  @Input() wizardMode: boolean = false;
  @Input() preselectedOrderId: number | null = null;
  @Input() preselectedCustomerId: number | null = null;
  @Input() maxAmount: number | null = null;
  @Output() formSubmitted = new EventEmitter<Payment>();
  @Output() cancelClicked = new EventEmitter<void>();

  // Properties
  paymentForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  paymentId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Payment method options
  paymentMethodOptions = [
    { value: PaymentMethod.Cash, label: 'Cash' },
    { value: PaymentMethod.Card, label: 'Card' },
    { value: PaymentMethod.BankTransfer, label: 'Bank Transfer' },
    { value: PaymentMethod.UPI, label: 'UPI' },
    { value: PaymentMethod.Cheque, label: 'Cheque' },
    { value: PaymentMethod.Other, label: 'Other' },
  ];

  // Transaction type options
  transactionTypeOptions = [
    { value: TransactionType.Sale, label: 'Sale' },
    { value: TransactionType.Purchase, label: 'Purchase' },
  ];

  // Form validation messages
  validationMessages = {
    orderType: {
      required: 'Order type is required',
    },
    amount: {
      required: 'Amount is required',
      min: 'Amount must be greater than 0',
    },
    paymentMethod: {
      required: 'Payment method is required',
    },
    paymentDate: {
      required: 'Payment date is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    
    // Check if wizardMode is set via route data
    this.route.data.subscribe((data) => {
      if (data['wizardMode']) {
        this.wizardMode = true;
      }
    });
    
    if (this.wizardMode) {
      // In wizard mode, pre-fill order and customer IDs if provided
      if (this.preselectedOrderId) {
        this.paymentForm.patchValue({ orderId: this.preselectedOrderId });
      }
      if (this.preselectedCustomerId) {
        this.paymentForm.patchValue({ customerId: this.preselectedCustomerId });
      }
      // Set default amount to max amount (balance due) if provided
      if (this.maxAmount) {
        this.paymentForm.patchValue({ amount: this.maxAmount });
      }
    } else {
      this.checkEditMode();
    }
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.paymentForm = this.fb.group({
      orderId: [null],
      orderType: [TransactionType.Sale, [Validators.required]],
      customerId: [null],
      salesPersonId: [null],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      paymentMethod: [PaymentMethod.Cash, [Validators.required]],
      paymentDate: [today, [Validators.required]],
      referenceNumber: [''],
    });
  }

  /**
   * Check if we're in edit mode and load payment data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.paymentId = +id;
      this.loadPaymentData(this.paymentId);
    }
  }

  /**
   * Load payment data for editing
   */
  loadPaymentData(id: number): void {
    this.isLoading = true;
    this.invoicePaymentService.getPaymentById(id).subscribe({
      next: (payment) => {
        if (payment) {
          // Format date for the form
          const paymentDate = payment.paymentDate ? new Date(payment.paymentDate).toISOString().split('T')[0] : '';
          
          this.paymentForm.patchValue({
            orderId: payment.orderId,
            orderType: payment.orderType,
            customerId: payment.customerId,
            salesPersonId: payment.salesPersonId,
            amount: payment.amount,
            paymentMethod: payment.paymentMethod,
            paymentDate: paymentDate,
            referenceNumber: payment.referenceNumber,
          });
        } else {
          this.toastr.error('Payment not found');
          this.router.navigate(['jewelleryManagement/admin/invoicepayment']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/invoicepayment']);
      },
    });
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
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.paymentForm.get(fieldName);
    if (field && field.errors) {
      const errorKey = Object.keys(field.errors)[0];
      const messages = this.validationMessages[fieldName as keyof typeof this.validationMessages];
      if (messages && typeof messages === 'object') {
        return (messages as Record<string, string>)[errorKey] || 'Invalid value';
      }
    }
    return '';
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.paymentForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    // In wizard mode, validate amount doesn't exceed max
    if (this.wizardMode && this.maxAmount !== null) {
      const amount = this.paymentForm.get('amount')?.value;
      if (amount > this.maxAmount) {
        this.toastr.error('Payment amount cannot exceed balance due');
        return;
      }
    }

    this.isSubmitting = true;
    const formValue = this.paymentForm.value;

    if (this.wizardMode) {
      // In wizard mode, add payment via wizard service
      const paymentData: WizardPaymentCreate = {
        orderId: formValue.orderId || undefined,
        orderType: formValue.orderType,
        customerId: formValue.customerId || undefined,
        salesPersonId: formValue.salesPersonId || undefined,
        amount: formValue.amount,
        paymentMethod: formValue.paymentMethod,
        paymentDate: formValue.paymentDate,
        referenceNumber: formValue.referenceNumber || undefined,
        statusId: 1, // Active status
      };

      this.wizardService.addPayment(paymentData).subscribe({
        next: (payment) => {
          this.isSubmitting = false;
          this.toastr.success('Payment recorded successfully');
          // Emit the payment - cast to any to handle type differences between models
          this.formSubmitted.emit(payment as any);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else if (this.isEditMode && this.paymentId) {
      // Update existing payment
      const updateData: PaymentUpdate = {
        id: this.paymentId,
        orderId: formValue.orderId || undefined,
        orderType: formValue.orderType,
        customerId: formValue.customerId || undefined,
        salesPersonId: formValue.salesPersonId || undefined,
        amount: formValue.amount,
        paymentMethod: formValue.paymentMethod,
        paymentDate: formValue.paymentDate,
        referenceNumber: formValue.referenceNumber || undefined,
      };

      this.invoicePaymentService.updatePayment(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/invoicepayment']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new payment
      const createData: PaymentCreate = {
        orderId: formValue.orderId || undefined,
        orderType: formValue.orderType,
        customerId: formValue.customerId || undefined,
        salesPersonId: formValue.salesPersonId || undefined,
        amount: formValue.amount,
        paymentMethod: formValue.paymentMethod,
        paymentDate: formValue.paymentDate,
        referenceNumber: formValue.referenceNumber || undefined,
      };

      this.invoicePaymentService.createPayment(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/invoicepayment']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    }
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
   * Cancel and navigate back to list
   */
  onCancel(): void {
    if (this.wizardMode) {
      this.cancelClicked.emit();
    } else {
      this.router.navigate(['jewelleryManagement/admin/invoicepayment']);
    }
  }
}
