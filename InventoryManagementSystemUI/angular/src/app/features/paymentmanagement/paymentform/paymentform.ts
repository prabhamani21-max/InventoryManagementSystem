// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { PaymentService } from 'src/app/core/services/payment.service';
import {
  Payment,
  PaymentCreate,
  PaymentUpdate,
  PaymentMethod,
  TransactionType,
  PAYMENT_METHOD_OPTIONS,
  ORDER_TYPE_OPTIONS,
  STATUS_OPTIONS,
} from 'src/app/core/models/payment.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Payment Form Component
 * Handles both create and edit operations for payments
 */
@Component({
  selector: 'app-paymentform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './paymentform.html',
  styleUrl: './paymentform.scss',
})
export class Paymentform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private paymentService = inject(PaymentService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  paymentForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  paymentId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Dropdown options
  paymentMethodOptions = PAYMENT_METHOD_OPTIONS;
  orderTypeOptions = ORDER_TYPE_OPTIONS;
  statusOptions = STATUS_OPTIONS;

  // Form validation messages
  validationMessages = {
    orderId: {
      required: 'Order ID is required',
    },
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
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.checkEditMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.paymentForm = this.fb.group({
      orderId: [null],
      orderType: [TransactionType.SALE, [Validators.required]],
      customerId: [null],
      salesPersonId: [null],
      amount: [null, [Validators.required, Validators.min(0.01)]],
      paymentMethod: [PaymentMethod.CASH, [Validators.required]],
      paymentDate: [today, [Validators.required]],
      referenceNumber: [null],
      statusId: [1, [Validators.required]],
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
    this.paymentService.getPaymentById(id).subscribe({
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
            statusId: payment.statusId,
          });
        } else {
          this.toastr.error('Payment not found');
          this.router.navigate(['jewelleryManagement/admin/payment']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/payment']);
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

    this.isSubmitting = true;
    const formValue = this.paymentForm.value;

    if (this.isEditMode && this.paymentId) {
      // Update existing payment
      const updateData: PaymentUpdate = {
        id: this.paymentId,
        orderId: formValue.orderId || null,
        orderType: formValue.orderType,
        customerId: formValue.customerId || null,
        salesPersonId: formValue.salesPersonId || null,
        amount: formValue.amount,
        paymentMethod: formValue.paymentMethod,
        paymentDate: formValue.paymentDate,
        referenceNumber: formValue.referenceNumber || null,
        statusId: formValue.statusId,
      };

      this.paymentService.updatePayment(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/payment']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new payment
      const createData: PaymentCreate = {
        orderId: formValue.orderId || null,
        orderType: formValue.orderType,
        customerId: formValue.customerId || null,
        salesPersonId: formValue.salesPersonId || null,
        amount: formValue.amount,
        paymentMethod: formValue.paymentMethod,
        paymentDate: formValue.paymentDate,
        referenceNumber: formValue.referenceNumber || null,
        statusId: formValue.statusId,
      };

      this.paymentService.createPayment(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/payment']);
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
    this.router.navigate(['jewelleryManagement/admin/payment']);
  }
}
