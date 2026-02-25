// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { PurityService } from 'src/app/core/services/purity.service';
import { MetalService } from 'src/app/core/services/metal.service';
import { Purity, PurityCreate, PurityUpdate } from 'src/app/core/models/purity.model';
import { Metal } from 'src/app/core/models/metal.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Purity Form Component
 * Handles both create and edit operations for purities
 */
@Component({
  selector: 'app-purityform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './purityform.html',
  styleUrl: './purityform.scss',
})
export class Purityform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private purityService = inject(PurityService);
  private metalService = inject(MetalService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  purityForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  purityId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  metals: Metal[] = [];

  // Form validation messages
  validationMessages = {
    metalId: {
      required: 'Metal is required',
    },
    name: {
      required: 'Purity name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 50 characters',
    },
    percentage: {
      required: 'Percentage is required',
      min: 'Percentage must be at least 0',
      max: 'Percentage cannot exceed 100',
    },
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.loadMetals();
    this.checkEditMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.purityForm = this.fb.group({
      metalId: [null, [Validators.required]],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      percentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Load metals for dropdown
   */
  loadMetals(): void {
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals;
      },
    });
  }

  /**
   * Check if we're in edit mode and load purity data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.purityId = +id;
      this.loadPurityData(this.purityId);
    }
  }

  /**
   * Load purity data for editing
   */
  loadPurityData(id: number): void {
    this.isLoading = true;
    this.purityService.getPurityById(id).subscribe({
      next: (purity) => {
        if (purity) {
          this.purityForm.patchValue({
            metalId: purity.metalId,
            name: purity.name,
            percentage: purity.percentage,
            statusId: purity.statusId,
          });
        } else {
          this.toastr.error('Purity not found');
          this.router.navigate(['jewelleryManagement/admin/purity']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/purity']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.purityForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.purityForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.purityForm.get(fieldName);
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
    if (this.purityForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.purityForm.value;

    if (this.isEditMode && this.purityId) {
      // Update existing purity
      const updateData: PurityUpdate = {
        id: this.purityId,
        metalId: formValue.metalId,
        name: formValue.name,
        percentage: formValue.percentage,
        statusId: formValue.statusId,
      };

      this.purityService.updatePurity(this.purityId, updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/purity']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new purity
      const createData: PurityCreate = {
        metalId: formValue.metalId,
        name: formValue.name,
        percentage: formValue.percentage,
        statusId: formValue.statusId,
      };

      this.purityService.createPurity(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/purity']);
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
    Object.keys(this.purityForm.controls).forEach((key) => {
      this.purityForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/purity']);
  }
}
