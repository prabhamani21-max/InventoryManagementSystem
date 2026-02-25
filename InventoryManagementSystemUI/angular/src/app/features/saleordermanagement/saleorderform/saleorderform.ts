// angular import
import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleWizardService } from 'src/app/core/services/sale-wizard.service';
import { SaleOrder, SaleOrderCreate, SaleOrderUpdate } from 'src/app/core/models/sale-order.model';
import { ToastrService } from 'ngx-toastr';

/**
 * SaleOrder Form Component
 * Handles both create and edit operations for sale orders
 * Supports wizard mode for integration with sale wizard
 */
@Component({
  selector: 'app-saleorderform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './saleorderform.html',
  styleUrl: './saleorderform.scss',
})
export class Saleorderform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private saleOrderService = inject(SaleOrderService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private wizardService = inject(SaleWizardService);

  // Wizard mode inputs
  @Input() wizardMode: boolean = false;
  @Input() preselectedCustomerId: number | null = null;
  @Output() formSubmitted = new EventEmitter<SaleOrder>();
  @Output() cancelClicked = new EventEmitter<void>();

  // Properties
  saleOrderForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  saleOrderId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Pending' },
    { id: 4, name: 'Completed' },
    { id: 5, name: 'Cancelled' },
  ];

  // Form validation messages
  validationMessages = {
    customerId: {
      required: 'Customer is required',
    },
    orderDate: {
      required: 'Order date is required',
    },
    statusId: {
      required: 'Status is required',
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
      // In wizard mode, automatically fetch the selected customer's userId from wizard service
      const wizardState = this.wizardService.getCurrentState();
      if (wizardState.selectedCustomer && wizardState.selectedCustomer.id) {
        this.saleOrderForm.patchValue({ customerId: wizardState.selectedCustomer.id });
      } else if (this.preselectedCustomerId) {
        // Fallback to preselectedCustomerId if provided
        this.saleOrderForm.patchValue({ customerId: this.preselectedCustomerId });
      }
      // Watch for exchange sale toggle
      this.saleOrderForm.get('isExchangeSale')?.valueChanges.subscribe((isExchange) => {
        const exchangeOrderIdControl = this.saleOrderForm.get('exchangeOrderId');
        if (isExchange) {
          exchangeOrderIdControl?.setValidators([Validators.required]);
        } else {
          exchangeOrderIdControl?.clearValidators();
          exchangeOrderIdControl?.setValue(null);
        }
        exchangeOrderIdControl?.updateValueAndValidity();
      });
    } else {
      this.checkEditMode();
    }
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];
    
    this.saleOrderForm = this.fb.group({
      customerId: [null, [Validators.required]],
      orderDate: [today, [Validators.required]],
      deliveryDate: [null],
      isExchangeSale: [false],
      exchangeOrderId: [null],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Check if we're in edit mode and load sale order data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.saleOrderId = +id;
      this.loadSaleOrderData(this.saleOrderId);
    }
  }

  /**
   * Load sale order data for editing
   */
  loadSaleOrderData(id: number): void {
    this.isLoading = true;
    this.saleOrderService.getSaleOrderById(id).subscribe({
      next: (order) => {
        if (order) {
          // Format dates for the form
          const orderDate = order.orderDate ? new Date(order.orderDate).toISOString().split('T')[0] : '';
          const deliveryDate = order.deliveryDate ? new Date(order.deliveryDate).toISOString().split('T')[0] : null;
          
          this.saleOrderForm.patchValue({
            customerId: order.customerId,
            orderDate: orderDate,
            deliveryDate: deliveryDate,
            isExchangeSale: order.isExchangeSale,
            exchangeOrderId: order.exchangeOrderId,
            statusId: order.statusId,
          });
        } else {
          this.toastr.error('Sale order not found');
          this.router.navigate(['jewelleryManagement/admin/saleorder']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/saleorder']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.saleOrderForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.saleOrderForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.saleOrderForm.get(fieldName);
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
    if (this.saleOrderForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.saleOrderForm.value;

    if (this.wizardMode) {
      // In wizard mode, create sale order via wizard service
      const orderData: SaleOrderCreate = {
        customerId: formValue.customerId,
        orderDate: formValue.orderDate,
        deliveryDate: formValue.deliveryDate || null,
        isExchangeSale: formValue.isExchangeSale || false,
        exchangeOrderId: formValue.exchangeOrderId || null,
        statusId: formValue.statusId,
      };

      this.wizardService.createSaleOrder(orderData).subscribe({
        next: (order) => {
          this.isSubmitting = false;
          this.toastr.success('Sale order created successfully');
          this.formSubmitted.emit(order);
          // Automatically move to Step 3 (Items) after successful sale order creation
          this.wizardService.nextStep();
          this.router.navigate(['../items'], { relativeTo: this.route });
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else if (this.isEditMode && this.saleOrderId) {
      // Update existing sale order
      const updateData: SaleOrderUpdate = {
        id: this.saleOrderId,
        customerId: formValue.customerId,
        orderDate: formValue.orderDate,
        deliveryDate: formValue.deliveryDate || null,
        isExchangeSale: formValue.isExchangeSale,
        exchangeOrderId: formValue.exchangeOrderId || null,
        statusId: formValue.statusId,
      };

      this.saleOrderService.updateSaleOrder(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/saleorder']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new sale order
      const createData: SaleOrderCreate = {
        customerId: formValue.customerId,
        orderDate: formValue.orderDate,
        deliveryDate: formValue.deliveryDate || null,
        isExchangeSale: formValue.isExchangeSale,
        exchangeOrderId: formValue.exchangeOrderId || null,
        statusId: formValue.statusId,
      };

      this.saleOrderService.createSaleOrder(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/saleorder']);
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
    Object.keys(this.saleOrderForm.controls).forEach((key) => {
      this.saleOrderForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    if (this.wizardMode) {
      this.cancelClicked.emit();
    } else {
      this.router.navigate(['jewelleryManagement/admin/saleorder']);
    }
  }
}
