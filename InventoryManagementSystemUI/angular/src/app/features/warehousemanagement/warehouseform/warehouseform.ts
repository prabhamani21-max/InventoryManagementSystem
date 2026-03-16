// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { WarehouseService } from 'src/app/core/services/warehouse.service';
import {
  Warehouse,
  WarehouseCreate,
  WarehouseUpdate,
  getStatusLabel,
  getStatusClass,
  WarehouseStatus,
} from 'src/app/core/models/warehouse.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Warehouse Form Component
 * Handles both create and edit operations for warehouses
 */
@Component({
  selector: 'app-warehouseform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './warehouseform.html',
  styleUrl: './warehouseform.scss',
})
export class Warehouseform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private warehouseService = inject(WarehouseService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  warehouseForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  warehouseId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  warehouse: Warehouse | null = null;

  // Status options
  statusOptions = [
    { id: WarehouseStatus.Active, name: 'Active' },
    { id: WarehouseStatus.Inactive, name: 'Inactive' },
    { id: WarehouseStatus.Pending, name: 'Pending' },
  ];

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Warehouse name is required',
      minlength: 'Warehouse name must be at least 2 characters',
    },
    statusId: {
      required: 'Status is required',
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
    this.warehouseForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      address: [''],
      managerId: [null],
      statusId: [WarehouseStatus.Active, [Validators.required]], // Default to Active
    });
  }

  /**
   * Check if we're in view/edit mode and load warehouse data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.warehouseId = +id;
      // Check if it's view mode based on URL
      const urlSegments = this.route.snapshot.url;
      this.isViewMode = urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = !this.isViewMode;
      this.loadWarehouseData(this.warehouseId);
    }
  }

  /**
   * Load warehouse data from API
   */
  loadWarehouseData(id: number): void {
    this.isLoading = true;
    this.warehouseService.getWarehouseById(id).subscribe({
      next: (warehouse) => {
        if (warehouse) {
          this.warehouse = warehouse;
          this.warehouseForm.patchValue({
            name: warehouse.name,
            address: warehouse.address,
            managerId: warehouse.managerId,
            statusId: warehouse.statusId,
          });

          // Disable form in view mode
          if (this.isViewMode) {
            this.warehouseForm.disable();
          }
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error('Failed to load warehouse data');
        this.onCancel();
      },
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.warehouseForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.warehouseForm.value;

    if (this.isEditMode && this.warehouseId) {
      // Update existing warehouse
      const updateData: WarehouseUpdate = {
        id: this.warehouseId,
        name: formValue.name,
        address: formValue.address || undefined,
        managerId: formValue.managerId || undefined,
        statusId: formValue.statusId,
      };

      this.warehouseService.updateWarehouse(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.onCancel();
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new warehouse
      const createData: WarehouseCreate = {
        name: formValue.name,
        address: formValue.address || undefined,
        managerId: formValue.managerId || undefined,
        statusId: formValue.statusId,
      };

      this.warehouseService.createWarehouse(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.onCancel();
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
  markFormGroupTouched(): void {
    Object.keys(this.warehouseForm.controls).forEach((key) => {
      this.warehouseForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string, errorType?: string): boolean {
    const field = this.warehouseForm.get(fieldName);
    if (errorType) {
      return field?.hasError(errorType) && (field?.touched || field?.dirty) || false;
    }
    return field?.invalid && (field?.touched || field?.dirty) || false;
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.warehouseForm.get(fieldName);
    if (field?.errors) {
      const errorKey = Object.keys(field.errors)[0];
      const messages: Record<string, Record<string, string>> = this.validationMessages;
      const fieldMessages = messages[fieldName];
      if (fieldMessages && errorKey in fieldMessages) {
        return fieldMessages[errorKey] || 'Invalid value';
      }
    }
    return '';
  }

  /**
   * Navigate back to the list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/warehouse']);
  }

  /**
   * Navigate to edit mode from view mode
   */
  onEdit(): void {
    if (this.warehouseId) {
      this.router.navigate(['jewelleryManagement/admin/warehouse/edit', this.warehouseId]);
    }
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }
}
