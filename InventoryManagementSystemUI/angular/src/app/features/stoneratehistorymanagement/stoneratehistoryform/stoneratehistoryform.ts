// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { StoneRateHistoryService } from 'src/app/core/services/stone-rate-history.service';
import { StoneService } from 'src/app/core/services/stone.service';
import { StoneRateHistory, StoneRateHistoryCreate, StoneRateHistoryUpdate } from 'src/app/core/models/stone-rate-history.model';
import { Stone } from 'src/app/core/models/stone.model';
import { ToastrService } from 'ngx-toastr';
import { CUT_OPTIONS, COLOR_OPTIONS, CLARITY_OPTIONS, GRADE_OPTIONS } from 'src/app/core/models/stone-rate-history.model';

/**
 * Stone Rate History Form Component
 * Handles both create and edit operations for stone rate history
 */
@Component({
  selector: 'app-stoneratehistoryform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './stoneratehistoryform.html',
  styleUrl: './stoneratehistoryform.scss',
})
export class Stoneratehistoryform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private stoneRateHistoryService = inject(StoneRateHistoryService);
  private stoneService = inject(StoneService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  stoneRateForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  stoneRateId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  stones: Stone[] = [];

  // Dropdown options
  cutOptions = CUT_OPTIONS;
  colorOptions = COLOR_OPTIONS;
  clarityOptions = CLARITY_OPTIONS;
  gradeOptions = GRADE_OPTIONS;

  // Backend validation errors - populated from API response
  backendErrors: { [key: string]: string } = {};

  // Form validation messages - matching backend StoneRateDtoValidator
  validationMessages: { [key: string]: { [key: string]: string } } = {
    stoneId: {
      required: 'Stone is required',
    },
    carat: {
      required: 'Carat is required',
      min: 'Carat must be at least 0.01',
      max: 'Carat cannot exceed 10',
    },
    cut: {
      maxlength: 'Cut cannot exceed 50 characters',
    },
    color: {
      maxlength: 'Color cannot exceed 10 characters',
    },
    clarity: {
      maxlength: 'Clarity cannot exceed 10 characters',
    },
    grade: {
      maxlength: 'Grade cannot exceed 20 characters',
    },
    ratePerUnit: {
      required: 'Rate per unit is required',
      min: 'Rate must be greater than 0',
    },
    effectiveDate: {
      required: 'Effective date is required',
    },
  };

  ngOnInit(): void {
    this.loadStones();
    this.initializeForm();
    this.checkEditMode();
  }

  /**
   * Load all stones for dropdown
   */
  loadStones(): void {
    this.stoneService.getAllStones().subscribe({
      next: (stones) => {
        this.stones = stones.filter(s => s.statusId === 1); // Only active stones
      },
    });
  }

  /**
   * Initialize the reactive form with validations matching backend StoneRateDtoValidator
   * Backend validations:
   * - StoneId: Required
   * - Carat: Required, Range(0.01, 10)
   * - Cut: MaxLength(50)
   * - Color: MaxLength(10)
   * - Clarity: MaxLength(10)
   * - Grade: MaxLength(20)
   * - RatePerUnit: Required, Range(0.01, double.MaxValue)
   * - EffectiveDate: Required
   */
  initializeForm(): void {
    this.stoneRateForm = this.fb.group({
      stoneId: [null, [Validators.required]],
      carat: [0.01, [Validators.required, Validators.min(0.01), Validators.max(10)]],
      cut: ['', [Validators.maxLength(50)]],
      color: ['', [Validators.maxLength(10)]],
      clarity: ['', [Validators.maxLength(10)]],
      grade: ['', [Validators.maxLength(20)]],
      ratePerUnit: [0, [Validators.required, Validators.min(0.01)]],
      effectiveDate: [this.formatDateForInput(new Date()), [Validators.required]],
    });

    // Subscribe to value changes to clear backend errors when user starts typing
    this.stoneRateForm.valueChanges.subscribe(() => {
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
      const control = this.stoneRateForm.get(key);
      if (control && control.dirty) {
        delete this.backendErrors[key];
      }
    });
  }

  /**
   * Check if we're in edit mode and load stone rate data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.stoneRateId = +id;
      this.loadStoneRateData(this.stoneRateId);
    }
  }

  /**
   * Load stone rate data for editing
   * Note: Since there's no GET by ID endpoint, we'll need to get from the list
   */
  loadStoneRateData(id: number): void {
    this.isLoading = true;
    // We need to get all rates and find the one by ID
    this.stoneRateHistoryService.getAllCurrentRates().subscribe({
      next: (rates) => {
        const stoneRate = rates.find(r => r.id === id);
        if (stoneRate) {
          this.stoneRateForm.patchValue({
            stoneId: stoneRate.stoneId,
            carat: stoneRate.carat,
            cut: stoneRate.cut || '',
            color: stoneRate.color || '',
            clarity: stoneRate.clarity || '',
            grade: stoneRate.grade || '',
            ratePerUnit: stoneRate.ratePerUnit,
            effectiveDate: this.formatDateForInput(new Date(stoneRate.effectiveDate)),
          });
        } else {
          this.toastr.error('Stone rate not found');
          this.router.navigate(['jewelleryManagement/admin/stoneratehistory']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/stoneratehistory']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.stoneRateForm.controls;
  }

  /**
   * Check if a field has errors - shows errors immediately on value change
   */
  hasError(fieldName: string): boolean {
    const field = this.stoneRateForm.get(fieldName);
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
    const field = this.stoneRateForm.get(fieldName);
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
      const control = this.stoneRateForm.get(key);
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
    const control = this.stoneRateForm.get(fieldName);
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

    if (this.stoneRateForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.stoneRateForm.value;

    if (this.isEditMode && this.stoneRateId) {
      // Update existing stone rate
      const updateData: StoneRateHistoryUpdate = {
        id: this.stoneRateId,
        carat: formValue.carat,
        cut: formValue.cut || undefined,
        color: formValue.color || undefined,
        clarity: formValue.clarity || undefined,
        grade: formValue.grade || undefined,
        ratePerUnit: formValue.ratePerUnit,
        effectiveDate: formValue.effectiveDate,
      };

      this.stoneRateHistoryService.updateStoneRate(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/stoneratehistory']);
        },
        error: (error) => {
          this.isSubmitting = false;
          this.handleBackendError(error);
        },
      });
    } else {
      // Create new stone rate
      const createData: StoneRateHistoryCreate = {
        stoneId: formValue.stoneId,
        carat: formValue.carat,
        cut: formValue.cut || undefined,
        color: formValue.color || undefined,
        clarity: formValue.clarity || undefined,
        grade: formValue.grade || undefined,
        ratePerUnit: formValue.ratePerUnit,
        effectiveDate: formValue.effectiveDate,
      };

      this.stoneRateHistoryService.createStoneRate(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/stoneratehistory']);
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
          StoneId: 'stoneId',
          Carat: 'carat',
          Cut: 'cut',
          Color: 'color',
          Clarity: 'clarity',
          Grade: 'grade',
          RatePerUnit: 'ratePerUnit',
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
      this.toastr.error('Stone rate not found');
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
    Object.keys(this.stoneRateForm.controls).forEach((key) => {
      this.stoneRateForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/stoneratehistory']);
  }
}
