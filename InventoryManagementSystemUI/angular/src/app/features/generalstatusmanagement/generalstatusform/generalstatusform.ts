// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { GeneralStatusService } from '../general-status.service';
import { GeneralStatus } from '../general-status.model';
import { ToastrService } from 'ngx-toastr';

/**
 * General Status Form Component
 * Handles both create and edit operations for general statuses
 */
@Component({
  selector: 'app-generalstatusform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './generalstatusform.html',
  styleUrl: './generalstatusform.scss',
})
export class GeneralStatusform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private generalStatusService = inject(GeneralStatusService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  generalStatusForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  generalStatusId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Form validation messages
  validationMessages = {
    name: {
      required: 'General status name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 50 characters',
    },
    isActive: {
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
    this.generalStatusForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      isActive: [true, [Validators.required]],
    });
  }

  /**
   * Check if we're in edit mode and load general status data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.generalStatusId = +id;
      this.loadGeneralStatusData(this.generalStatusId);
    }
  }

  /**
   * Load general status data for editing
   */
  loadGeneralStatusData(id: number): void {
    this.isLoading = true;
    this.generalStatusService.getGeneralStatusById(id).subscribe({
      next: (status) => {
        if (status) {
          this.generalStatusForm.patchValue({
            name: status.name,
            isActive: status.isActive,
          });
        } else {
          this.toastr.error('General status not found');
          this.router.navigate(['jewelleryManagement/admin/generalstatus']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/generalstatus']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.generalStatusForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.generalStatusForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.generalStatusForm.get(fieldName);
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
    if (this.generalStatusForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.generalStatusForm.value;

    if (this.isEditMode && this.generalStatusId) {
      // Update existing general status
      const updateData: GeneralStatus = {
        id: this.generalStatusId,
        name: formValue.name,
        isActive: formValue.isActive,
      };

      this.generalStatusService.updateGeneralStatus(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.toastr.success('General status updated successfully');
          this.router.navigate(['jewelleryManagement/admin/generalstatus']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new general status
      const createData: GeneralStatus = {
        id: 0,
        name: formValue.name,
        isActive: formValue.isActive,
      };

      this.generalStatusService.createGeneralStatus(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.toastr.success('General status created successfully');
          this.router.navigate(['jewelleryManagement/admin/generalstatus']);
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
    Object.keys(this.generalStatusForm.controls).forEach((key) => {
      this.generalStatusForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/generalstatus']);
  }
}