// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoiceItemService } from 'src/app/core/services/invoice-item.service';
import {
  InvoiceItem,
  InvoiceItemCreate,
  InvoiceItemUpdate,
  formatCurrency,
  formatWeight,
  getHallmarkLabel,
} from 'src/app/core/models/invoice-item.model';
import { ToastrService } from 'ngx-toastr';

/**
 * InvoiceItem Form Component
 * Handles both create and edit operations for invoice items
 */
@Component({
  selector: 'app-invoiceitemform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './invoiceitemform.html',
  styleUrl: './invoiceitemform.scss',
})
export class Invoiceitemform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private invoiceItemService = inject(InvoiceItemService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  invoiceItemForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  invoiceItemId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Existing item data for display
  existingItem: InvoiceItem | null = null;

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
    invoiceId: {
      required: 'Invoice is required',
    },
    itemName: {
      required: 'Item name is required',
    },
    quantity: {
      required: 'Quantity is required',
      min: 'Quantity must be at least 1',
    },
    metalId: {
      required: 'Metal is required',
    },
    purityId: {
      required: 'Purity is required',
    },
    itemSubtotal: {
      required: 'Item subtotal is required',
      min: 'Item subtotal cannot be negative',
    },
    taxableAmount: {
      required: 'Taxable amount is required',
      min: 'Taxable amount cannot be negative',
    },
    gstAmount: {
      required: 'GST amount is required',
      min: 'GST amount cannot be negative',
    },
    totalAmount: {
      required: 'Total amount is required',
      min: 'Total amount cannot be negative',
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
    this.invoiceItemForm = this.fb.group({
      invoiceId: [null, [Validators.required]],
      referenceItemId: [null],
      itemName: ['', [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      metalId: [null, [Validators.required]],
      purityId: [null, [Validators.required]],
      netMetalWeight: [null, [Validators.min(0)]],
      metalAmount: [null, [Validators.min(0)]],
      stoneId: [null],
      stoneWeight: [null, [Validators.min(0)]],
      stoneRate: [null, [Validators.min(0)]],
      stoneAmount: [null, [Validators.min(0)]],
      makingCharges: [null, [Validators.min(0)]],
      wastageAmount: [null, [Validators.min(0)]],
      itemSubtotal: [0, [Validators.required, Validators.min(0)]],
      discount: [0, [Validators.min(0)]],
      taxableAmount: [0, [Validators.required, Validators.min(0)]],
      cgstAmount: [0, [Validators.min(0)]],
      sgstAmount: [0, [Validators.min(0)]],
      igstAmount: [0, [Validators.min(0)]],
      gstAmount: [0, [Validators.required, Validators.min(0)]],
      totalAmount: [0, [Validators.required, Validators.min(0)]],
      isHallmarked: [false],
    });
  }

  /**
   * Check if we're in edit mode and load invoice item data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.invoiceItemId = +id;
      this.loadInvoiceItemData(this.invoiceItemId);
    }
  }

  /**
   * Load invoice item data for editing
   */
  loadInvoiceItemData(id: number): void {
    this.isLoading = true;
    this.invoiceItemService.getInvoiceItemById(id).subscribe({
      next: (item) => {
        if (item) {
          this.existingItem = item;
          this.invoiceItemForm.patchValue({
            invoiceId: item.invoiceId,
            referenceItemId: item.referenceItemId,
            itemName: item.itemName,
            quantity: item.quantity,
            metalId: item.metalId,
            purityId: item.purityId,
            netMetalWeight: item.netMetalWeight,
            metalAmount: item.metalAmount,
            stoneId: item.stoneId,
            stoneWeight: item.stoneWeight,
            stoneRate: item.stoneRate,
            stoneAmount: item.stoneAmount,
            makingCharges: item.makingCharges,
            wastageAmount: item.wastageAmount,
            itemSubtotal: item.itemSubtotal,
            discount: item.discount,
            taxableAmount: item.taxableAmount,
            cgstAmount: item.cgstAmount,
            sgstAmount: item.sgstAmount,
            igstAmount: item.igstAmount,
            gstAmount: item.gstAmount,
            totalAmount: item.totalAmount,
            isHallmarked: item.isHallmarked,
          });
        } else {
          this.toastr.error('Invoice item not found');
          this.router.navigate(['jewelleryManagement/admin/invoiceitem']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/invoiceitem']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.invoiceItemForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.invoiceItemForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.invoiceItemForm.get(fieldName);
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
    if (this.invoiceItemForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.invoiceItemForm.value;

    if (this.isEditMode && this.invoiceItemId) {
      // Update existing invoice item
      const updateData: InvoiceItemUpdate = {
        id: this.invoiceItemId,
        invoiceId: formValue.invoiceId,
        referenceItemId: formValue.referenceItemId,
        itemName: formValue.itemName,
        quantity: formValue.quantity,
        metalId: formValue.metalId,
        purityId: formValue.purityId,
        netMetalWeight: formValue.netMetalWeight,
        metalAmount: formValue.metalAmount,
        stoneId: formValue.stoneId,
        stoneWeight: formValue.stoneWeight,
        stoneRate: formValue.stoneRate,
        stoneAmount: formValue.stoneAmount,
        makingCharges: formValue.makingCharges,
        wastageAmount: formValue.wastageAmount,
        itemSubtotal: formValue.itemSubtotal,
        discount: formValue.discount,
        taxableAmount: formValue.taxableAmount,
        cgstAmount: formValue.cgstAmount,
        sgstAmount: formValue.sgstAmount,
        igstAmount: formValue.igstAmount,
        gstAmount: formValue.gstAmount,
        totalAmount: formValue.totalAmount,
        isHallmarked: formValue.isHallmarked,
      };

      this.invoiceItemService.updateInvoiceItem(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/invoiceitem']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new invoice item
      const createData: InvoiceItemCreate = {
        invoiceId: formValue.invoiceId,
        referenceItemId: formValue.referenceItemId,
        itemName: formValue.itemName,
        quantity: formValue.quantity,
        metalId: formValue.metalId,
        purityId: formValue.purityId,
        netMetalWeight: formValue.netMetalWeight,
        metalAmount: formValue.metalAmount,
        stoneId: formValue.stoneId,
        stoneWeight: formValue.stoneWeight,
        stoneRate: formValue.stoneRate,
        stoneAmount: formValue.stoneAmount,
        makingCharges: formValue.makingCharges,
        wastageAmount: formValue.wastageAmount,
        itemSubtotal: formValue.itemSubtotal,
        discount: formValue.discount,
        taxableAmount: formValue.taxableAmount,
        cgstAmount: formValue.cgstAmount,
        sgstAmount: formValue.sgstAmount,
        igstAmount: formValue.igstAmount,
        gstAmount: formValue.gstAmount,
        totalAmount: formValue.totalAmount,
        isHallmarked: formValue.isHallmarked,
      };

      this.invoiceItemService.createInvoiceItem(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/invoiceitem']);
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
    Object.keys(this.invoiceItemForm.controls).forEach((key) => {
      this.invoiceItemForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/invoiceitem']);
  }

  /**
   * Get hallmark label
   */
  getHallmarkLabel(isHallmarked: boolean): string {
    return getHallmarkLabel(isHallmarked);
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number): string {
    return formatWeight(weight);
  }
}