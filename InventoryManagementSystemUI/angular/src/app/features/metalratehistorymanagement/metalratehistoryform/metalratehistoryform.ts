// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { MetalRateHistoryService } from 'src/app/core/services/metal-rate-history.service';
import { PurityService } from 'src/app/core/services/purity.service';
import { MetalService } from 'src/app/core/services/metal.service';
import { MetalRateHistory, MetalRateHistoryCreate, MetalRateHistoryUpdate, MetalRateResponse } from 'src/app/core/models/metal-rate-history.model';
import { Purity } from 'src/app/core/models/purity.model';
import { Metal } from 'src/app/core/models/metal.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Metal Rate History Form Component
 * Handles both create and edit operations for metal rate history
 */
@Component({
  selector: 'app-metalratehistoryform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './metalratehistoryform.html',
  styleUrl: './metalratehistoryform.scss',
})
export class Metalratehistoryform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private metalRateHistoryService = inject(MetalRateHistoryService);
  private purityService = inject(PurityService);
  private metalService = inject(MetalService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  metalRateForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  purityId: number | null = null;
  existingRateId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  purities: Purity[] = [];
  metals: Metal[] = [];
  filteredPurities: Purity[] = [];
  selectedMetalId: number | null = null;

  // Backend validation errors - populated from API response
  backendErrors: { [key: string]: string } = {};

  // Form validation messages - matching backend MetalRateDto validation
  validationMessages: { [key: string]: { [key: string]: string } } = {
    purityId: {
      required: 'Purity is required',
    },
    ratePerGram: {
      required: 'Rate per gram is required',
      min: 'Rate must be greater than 0',
    },
    effectiveDate: {
      required: 'Effective date is required',
    },
  };

  ngOnInit(): void {
    this.loadMetals();
    this.loadPurities();
    this.initializeForm();
    this.checkEditMode();
  }

  /**
   * Load all metals for dropdown
   */
  loadMetals(): void {
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals.filter(m => m.statusId === 1); // Only active metals
      },
    });
  }

  /**
   * Load all purities for dropdown
   */
  loadPurities(): void {
    this.purityService.getAllPurities().subscribe({
      next: (purities) => {
        this.purities = purities.filter(p => p.statusId === 1); // Only active purities
        this.filteredPurities = this.purities;
      },
    });
  }

  /**
   * Filter purities based on selected metal
   */
  onMetalChange(metalId: number | string): void {
    // Convert string to number since select element returns string value
    const numericMetalId = typeof metalId === 'string' ? parseInt(metalId, 10) : metalId;
    this.selectedMetalId = numericMetalId || null;
    if (numericMetalId) {
      this.filteredPurities = this.purities.filter(p => p.metalId === numericMetalId);
    } else {
      this.filteredPurities = this.purities;
    }
    // Reset purity selection when metal changes
    this.metalRateForm.patchValue({ purityId: null });
  }

  /**
   * Initialize the reactive form with validations matching backend MetalRateDto
   * Backend validations:
   * - PurityId: Required
   * - RatePerGram: Required, Range(0.01, double.MaxValue)
   * - EffectiveDate: Required
   */
  initializeForm(): void {
    this.metalRateForm = this.fb.group({
      metalId: [null, []],
      purityId: [null, [Validators.required]],
      ratePerGram: [0, [Validators.required, Validators.min(0.01)]],
      effectiveDate: [this.formatDateForInput(new Date()), [Validators.required]],
    });

    // Subscribe to value changes to clear backend errors when user starts typing
    this.metalRateForm.valueChanges.subscribe(() => {
      this.clearBackendErrorsOnChange();
    });
  }

  /**
   * Format date for date input (YYYY-MM-DD)
   */
  private formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  /**
   * Clear backend errors when user modifies the form
   */
  private clearBackendErrorsOnChange(): void {
    Object.keys(this.backendErrors).forEach((key) => {
      const control = this.metalRateForm.get(key);
      if (control && control.dirty) {
        delete this.backendErrors[key];
      }
    });
  }

  /**
   * Check if we're in edit mode and load metal rate data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.purityId = +id;
      this.loadMetalRateData(this.purityId);
    }
  }

  /**
   * Load metal rate data for editing
   */
  loadMetalRateData(purityId: number): void {
    this.isLoading = true;
    this.metalRateHistoryService.getCurrentRateByPurity(purityId).subscribe({
      next: (metalRate) => {
        if (metalRate) {
          this.selectedMetalId = metalRate.metalId;
          this.filteredPurities = this.purities.filter(p => p.metalId === metalRate.metalId);
          
          this.metalRateForm.patchValue({
            metalId: metalRate.metalId,
            purityId: metalRate.purityId,
            ratePerGram: metalRate.currentRatePerGram,
            effectiveDate: this.formatDateForInput(new Date(metalRate.effectiveDate)),
          });
        } else {
          this.toastr.error('Metal rate not found');
          this.router.navigate(['jewelleryManagement/admin/metalratehistory']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/metalratehistory']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.metalRateForm.controls;
  }

  /**
   * Check if a field has errors - shows errors immediately on value change
   */
  hasError(fieldName: string): boolean {
    const field = this.metalRateForm.get(fieldName);
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
    const field = this.metalRateForm.get(fieldName);
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
      const control = this.metalRateForm.get(key);
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
    const control = this.metalRateForm.get(fieldName);
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

    if (this.metalRateForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.metalRateForm.value;

    if (this.isEditMode && this.purityId) {
      // For update, we need the existing rate ID
      // First get the current rate to find the ID
      this.metalRateHistoryService.getRateHistoryByPurity(this.purityId).subscribe({
        next: (history) => {
          if (history && history.length > 0) {
            // Get the latest rate entry
            const latestRate = history[0];
            const updateData: MetalRateHistoryUpdate = {
              id: latestRate.id,
              ratePerGram: formValue.ratePerGram,
              effectiveDate: formValue.effectiveDate,
            };

            this.metalRateHistoryService.updateMetalRate(updateData).subscribe({
              next: () => {
                this.isSubmitting = false;
                this.router.navigate(['jewelleryManagement/admin/metalratehistory']);
              },
              error: (error) => {
                this.isSubmitting = false;
                this.handleBackendError(error);
              },
            });
          } else {
            this.isSubmitting = false;
            this.toastr.error('No rate history found to update');
          }
        },
        error: () => {
          this.isSubmitting = false;
          this.toastr.error('Failed to load rate history');
        },
      });
    } else {
      // Create new metal rate
      const createData: MetalRateHistoryCreate = {
        purityId: formValue.purityId,
        ratePerGram: formValue.ratePerGram,
        effectiveDate: formValue.effectiveDate,
      };

      this.metalRateHistoryService.createMetalRate(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/metalratehistory']);
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
          PurityId: 'purityId',
          RatePerGram: 'ratePerGram',
          EffectiveDate: 'effectiveDate',
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
    } else if (error.status === 404) {
      this.toastr.error('Metal rate not found');
      return;
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
    Object.keys(this.metalRateForm.controls).forEach((key) => {
      this.metalRateForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/metalratehistory']);
  }
}