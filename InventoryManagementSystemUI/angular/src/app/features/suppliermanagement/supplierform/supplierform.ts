// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SupplierService } from 'src/app/core/services/supplier.service';
import { Supplier, SupplierCreate, SupplierUpdate } from 'src/app/core/models/supplier.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Supplier Form Component
 * Handles both create and edit operations for suppliers
 */
@Component({
  selector: 'app-supplierform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './supplierform.html',
  styleUrl: './supplierform.scss',
})
export class Supplierform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private supplierService = inject(SupplierService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  supplierForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  supplierId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Backend validation errors - populated from API response
  backendErrors: { [key: string]: string } = {};

  // Form validation messages - matching backend SupplierDtoValidator
  validationMessages: { [key: string]: { [key: string]: string } } = {
    name: {
      required: 'Supplier name is required',
      minlength: 'Name must be at least 3 characters',
      maxlength: 'Name cannot exceed 100 characters',
    },
    contactPerson: {
      maxlength: 'Contact person name cannot exceed 100 characters',
    },
    email: {
      email: 'Invalid email format',
    },
    phone: {
      pattern: 'Phone number must be 10-15 digits',
      minlength: 'Phone number must be at least 10 digits',
      maxlength: 'Phone number cannot exceed 15 digits',
    },
    address: {
      maxlength: 'Address cannot exceed 255 characters',
    },
    tanNumber: {
      required: 'TAN number is required',
      maxlength: 'TAN number cannot exceed 15 characters',
    },
    gstNumber: {
      required: 'GST number is required',
      maxlength: 'GST number cannot exceed 15 characters',
    },
    statusId: {
      required: 'Status is required',
      min: 'Please select a valid status',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.checkEditMode();
  }

  /**
   * Initialize the reactive form with validations matching backend SupplierDtoValidator
   * Backend validations:
   * - Name: Required, MinLength(3), MaxLength(100)
   * - ContactPerson: MaxLength(100) when provided
   * - Email: Valid email format when provided
   * - Phone: Must match ^\d{10,15}$ (10-15 digits) when provided
   * - Address: MaxLength(255) when provided
   * - GSTNumber: MaxLength(15) when provided
   * - TANNumber: MaxLength(15) when provided
   * - StatusId: GreaterThan(0)
   */
  initializeForm(): void {
    this.supplierForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
      contactPerson: ['', [Validators.maxLength(100)]],
      email: ['', [Validators.email]],
      phone: ['', [Validators.pattern(/^\d{10,15}$/)]],
      address: ['', [Validators.maxLength(255)]],
      tanNumber: ['', [Validators.required, Validators.maxLength(15)]],
      gstNumber: ['', [Validators.required, Validators.maxLength(15)]],
      statusId: [1, [Validators.required, Validators.min(1)]],
    });

    // Subscribe to value changes to clear backend errors when user starts typing
    this.supplierForm.valueChanges.subscribe(() => {
      this.clearBackendErrorsOnChange();
    });
  }

  /**
   * Clear backend errors when user modifies the form
   */
  private clearBackendErrorsOnChange(): void {
    Object.keys(this.backendErrors).forEach((key) => {
      const control = this.supplierForm.get(key);
      if (control && control.dirty) {
        delete this.backendErrors[key];
      }
    });
  }

  /**
   * Check if we're in edit mode and load supplier data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.supplierId = +id;
      this.loadSupplierData(this.supplierId);
    }
  }

  /**
   * Load supplier data for editing
   */
  loadSupplierData(id: number): void {
    this.isLoading = true;
    this.supplierService.getSupplierById(id).subscribe({
      next: (supplier) => {
        if (supplier) {
          this.supplierForm.patchValue({
            name: supplier.name,
            contactPerson: supplier.contactPerson || '',
            email: supplier.email || '',
            phone: supplier.phone || '',
            address: supplier.address || '',
            tanNumber: supplier.tanNumber,
            gstNumber: supplier.gstNumber,
            statusId: supplier.statusId,
          });
        } else {
          this.toastr.error('Supplier not found');
          this.router.navigate(['jewelleryManagement/admin/supplier']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/supplier']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.supplierForm.controls;
  }

  /**
   * Check if a field has errors - shows errors immediately on value change
   */
  hasError(fieldName: string): boolean {
    const field = this.supplierForm.get(fieldName);
    // Show frontend validation errors when field has been interacted with
    const hasFrontendError = !!(field && field.invalid && (field.dirty || field.touched));
    // Show backend errors immediately
    const hasBackendError = !!this.backendErrors[fieldName];
    return hasFrontendError || hasBackendError;
  }

  /**
   * Get error message for a field - combines frontend and backend errors
   */
  getErrorMessage(fieldName: string): string {
    // Check for backend error first
    if (this.backendErrors[fieldName]) {
      return this.backendErrors[fieldName];
    }

    // Then check for frontend validation errors
    const field = this.supplierForm.get(fieldName);
    if (field && field.errors) {
      const errorKey = Object.keys(field.errors)[0];
      const messages = this.validationMessages[fieldName];
      if (messages && typeof messages === 'object') {
        return (messages as Record<string, string>)[errorKey] || 'Invalid value';
      }
    }
    return '';
  }

  /**
   * Set backend validation errors on form fields
   */
  setBackendErrors(errors: { [key: string]: string }): void {
    this.backendErrors = { ...errors };
    // Mark fields as touched to show errors
    Object.keys(errors).forEach((key) => {
      const control = this.supplierForm.get(key);
      if (control) {
        control.markAsTouched();
      }
    });
  }

  /**
   * Clear all backend errors
   */
  clearBackendErrors(): void {
    this.backendErrors = {};
  }

  /**
   * Handle field change - clear backend error for this field and show frontend validation
   */
  onFieldChange(fieldName: string): void {
    // Clear backend error when user starts typing
    if (this.backendErrors[fieldName]) {
      delete this.backendErrors[fieldName];
    }
    // Mark field as touched to show frontend validation errors
    const control = this.supplierForm.get(fieldName);
    if (control) {
      control.markAsTouched();
    }
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    // Clear previous backend errors
    this.clearBackendErrors();

    if (this.supplierForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.supplierForm.value;

    if (this.isEditMode && this.supplierId) {
      // Update existing supplier
      const updateData: SupplierUpdate = {
        id: this.supplierId,
        name: formValue.name,
        contactPerson: formValue.contactPerson || undefined,
        email: formValue.email || undefined,
        phone: formValue.phone || undefined,
        address: formValue.address || undefined,
        tanNumber: formValue.tanNumber,
        gstNumber: formValue.gstNumber,
        statusId: formValue.statusId,
      };

      this.supplierService.updateSupplier(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/supplier']);
        },
        error: (error) => {
          this.isSubmitting = false;
          this.handleBackendError(error);
        },
      });
    } else {
      // Create new supplier
      const createData: SupplierCreate = {
        name: formValue.name,
        contactPerson: formValue.contactPerson || undefined,
        email: formValue.email || undefined,
        phone: formValue.phone || undefined,
        address: formValue.address || undefined,
        tanNumber: formValue.tanNumber,
        gstNumber: formValue.gstNumber,
        statusId: formValue.statusId,
      };

      this.supplierService.createSupplier(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/supplier']);
        },
        error: (error) => {
          this.isSubmitting = false;
          this.handleBackendError(error);
        },
      });
    }
  }

  /**
   * Handle backend validation errors and display them on specific fields
   */
  private handleBackendError(error: any): void {
    const fieldErrors: { [key: string]: string } = {};

    if (error.status === 400) {
      // Handle FluentValidation errors from backend
      const errors = error.error?.errors;
      if (errors) {
        // Map backend field names to frontend field names
        const fieldMapping: { [key: string]: string } = {
          Name: 'name',
          ContactPerson: 'contactPerson',
          Email: 'email',
          Phone: 'phone',
          Address: 'address',
          GSTNumber: 'gstNumber',
          TANNumber: 'tanNumber',
          StatusId: 'statusId',
        };

        Object.keys(errors).forEach((key) => {
          const frontendField = fieldMapping[key] || key.toLowerCase();
          if (Array.isArray(errors[key])) {
            fieldErrors[frontendField] = errors[key].join(', ');
          } else {
            fieldErrors[frontendField] = errors[key];
          }
        });
      }
    } else if (error.status === 409) {
      // Handle unique constraint violations
      const errors = error.error?.errors;
      if (errors?.GSTNumber) {
        fieldErrors['gstNumber'] = 'GST number already exists';
      }
      if (errors?.TANNumber) {
        fieldErrors['tanNumber'] = 'TAN number already exists';
      }
      if (!errors) {
        // Generic conflict message
        this.toastr.error('A supplier with this GST or TAN number already exists');
        return;
      }
    }

    if (Object.keys(fieldErrors).length > 0) {
      this.setBackendErrors(fieldErrors);
    } else {
      // Generic error message if no field-specific errors
      this.toastr.error('An error occurred. Please try again.');
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.supplierForm.controls).forEach((key) => {
      this.supplierForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/supplier']);
  }
}