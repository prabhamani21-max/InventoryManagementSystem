// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { ExchangeService } from 'src/app/core/services/exchange.service';
import {
  ExchangeOrder,
  ExchangeOrderCreate,
  ExchangeCalculateRequest,
  ExchangeCalculateResponse,
  ExchangeItemInput,
  ExchangeType,
  getExchangeTypeLabel,
  getStatusLabel,
  getStatusClass,
  formatDate,
  formatCurrency,
  formatWeight,
} from 'src/app/core/models/exchange.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Exchange Form Component
 * Handles both create and view operations for exchange orders
 */
@Component({
  selector: 'app-exchangeform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './exchangeform.html',
  styleUrl: './exchangeform.scss',
})
export class Exchangeform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private exchangeService = inject(ExchangeService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  exchangeForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  exchangeOrderId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  isCalculating: boolean = false;
  calculatedResponse: ExchangeCalculateResponse | null = null;
  exchangeOrder: ExchangeOrder | null = null;

  // Exchange type options
  exchangeTypeOptions = [
    { id: ExchangeType.EXCHANGE, name: 'Exchange (For New Purchase)' },
    { id: ExchangeType.BUYBACK, name: 'Buyback (Cash Payment)' },
  ];

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
    exchangeType: {
      required: 'Exchange type is required',
    },
    newPurchaseAmount: {
      min: 'Purchase amount must be greater than 0',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.checkMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.exchangeForm = this.fb.group({
      customerId: [null, [Validators.required]],
      exchangeType: [ExchangeType.EXCHANGE, [Validators.required]],
      newPurchaseAmount: [null, [Validators.min(0)]],
      notes: [''],
      items: this.fb.array([this.createItemFormGroup()]),
    });
  }

  /**
   * Create a form group for an exchange item
   */
  createItemFormGroup(): UntypedFormGroup {
    return this.fb.group({
      metalId: [null, [Validators.required]],
      purityId: [null, [Validators.required]],
      grossWeight: [0, [Validators.required, Validators.min(0)]],
      netWeight: [0, [Validators.required, Validators.min(0)]],
      makingChargeDeductionPercent: [0, [Validators.min(0), Validators.max(100)]],
      wastageDeductionPercent: [0, [Validators.min(0), Validators.max(100)]],
      itemDescription: [''],
    });
  }

  /**
   * Get the items form array
   */
  get itemsArray(): FormArray {
    return this.exchangeForm.get('items') as FormArray;
  }

  /**
   * Add a new item to the form
   */
  addItem(): void {
    this.itemsArray.push(this.createItemFormGroup());
  }

  /**
   * Remove an item from the form
   */
  removeItem(index: number): void {
    if (this.itemsArray.length > 1) {
      this.itemsArray.removeAt(index);
    } else {
      this.toastr.warning('At least one item is required');
    }
  }

  /**
   * Check if we're in view mode and load exchange order data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.exchangeOrderId = +id;
      // Check if it's view mode based on URL
      const urlSegments = this.route.snapshot.url;
      this.isViewMode = urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = !this.isViewMode;
      this.loadExchangeOrderData(this.exchangeOrderId);
    }
  }

  /**
   * Load exchange order data for viewing/editing
   */
  loadExchangeOrderData(id: number): void {
    this.isLoading = true;
    this.exchangeService.getExchangeOrderById(id).subscribe({
      next: (order) => {
        if (order) {
          this.exchangeOrder = order;
          // Clear existing items and populate with order items
          while (this.itemsArray.length) {
            this.itemsArray.removeAt(0);
          }
          
          // Populate form with order data
          this.exchangeForm.patchValue({
            customerId: order.customerId,
            exchangeType: order.exchangeType === 'EXCHANGE' ? ExchangeType.EXCHANGE : ExchangeType.BUYBACK,
            newPurchaseAmount: order.newPurchaseAmount,
            notes: order.notes,
          });

          // Add items
          if (order.items && order.items.length > 0) {
            order.items.forEach((item) => {
              this.itemsArray.push(
                this.fb.group({
                  metalId: [item.metalId, [Validators.required]],
                  purityId: [item.purityId, [Validators.required]],
                  grossWeight: [item.grossWeight, [Validators.required, Validators.min(0)]],
                  netWeight: [item.netWeight, [Validators.required, Validators.min(0)]],
                  makingChargeDeductionPercent: [item.makingChargeDeductionPercent, [Validators.min(0), Validators.max(100)]],
                  wastageDeductionPercent: [item.wastageDeductionPercent, [Validators.min(0), Validators.max(100)]],
                  itemDescription: [item.itemDescription || ''],
                })
              );
            });
          } else {
            this.itemsArray.push(this.createItemFormGroup());
          }

          // Disable form in view mode
          if (this.isViewMode) {
            this.exchangeForm.disable();
          }
        } else {
          this.toastr.error('Exchange order not found');
          this.router.navigate(['jewelleryManagement/admin/exchange']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/exchange']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.exchangeForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.exchangeForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Check if an item field has errors
   */
  hasItemError(index: number, fieldName: string): boolean {
    const item = this.itemsArray.at(index);
    const field = item.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.exchangeForm.get(fieldName);
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
   * Calculate exchange value
   */
  onCalculate(): void {
    if (this.exchangeForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isCalculating = true;
    const formValue = this.exchangeForm.value;

    const items: ExchangeItemInput[] = formValue.items.map((item: any) => ({
      metalId: item.metalId,
      purityId: item.purityId,
      grossWeight: item.grossWeight,
      netWeight: item.netWeight,
      makingChargeDeductionPercent: item.makingChargeDeductionPercent || 0,
      wastageDeductionPercent: item.wastageDeductionPercent || 0,
      itemDescription: item.itemDescription || null,
    }));

    const request: ExchangeCalculateRequest = {
      customerId: formValue.customerId,
      exchangeType: +formValue.exchangeType,
      items: items,
      newPurchaseAmount: formValue.newPurchaseAmount || null,
      notes: formValue.notes || null,
    };

    this.exchangeService.calculateExchangeValue(request).subscribe({
      next: (response) => {
        this.calculatedResponse = response;
        this.isCalculating = false;
        this.toastr.success('Exchange value calculated successfully');
      },
      error: () => {
        this.isCalculating = false;
      },
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.exchangeForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.exchangeForm.value;

    const items: ExchangeItemInput[] = formValue.items.map((item: any) => ({
      metalId: item.metalId,
      purityId: item.purityId,
      grossWeight: item.grossWeight,
      netWeight: item.netWeight,
      makingChargeDeductionPercent: item.makingChargeDeductionPercent || 0,
      wastageDeductionPercent: item.wastageDeductionPercent || 0,
      itemDescription: item.itemDescription || null,
    }));

    const createData: ExchangeOrderCreate = {
      customerId: formValue.customerId,
      exchangeType: +formValue.exchangeType,
      items: items,
      newPurchaseAmount: formValue.newPurchaseAmount || null,
      notes: formValue.notes || null,
    };

    this.exchangeService.createExchangeOrder(createData).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['jewelleryManagement/admin/exchange']);
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
    Object.keys(this.exchangeForm.controls).forEach((key) => {
      if (key === 'items') {
        this.itemsArray.controls.forEach((item) => {
          Object.keys((item as UntypedFormGroup).controls).forEach((itemKey) => {
            item.get(itemKey)?.markAsTouched();
          });
        });
      } else {
        this.exchangeForm.get(key)?.markAsTouched();
      }
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/exchange']);
  }

  /**
   * Get exchange type label
   */
  getExchangeTypeLabel(type: ExchangeType): string {
    return getExchangeTypeLabel(type);
  }

  /**
   * Get status label
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number | null | undefined): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number | null | undefined): string {
    return formatWeight(weight);
  }
}