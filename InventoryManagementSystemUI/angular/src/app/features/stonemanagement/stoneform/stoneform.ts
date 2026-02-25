// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { StoneService } from 'src/app/core/services/stone.service';
import { Stone, StoneCreate, StoneUpdate } from 'src/app/core/models/stone.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Stone Form Component
 * Handles both create and edit operations for stones
 */
@Component({
  selector: 'app-stoneform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './stoneform.html',
  styleUrl: './stoneform.scss',
})
export class Stoneform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private stoneService = inject(StoneService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  stoneForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  stoneId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Stone name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 100 characters',
    },
    unit: {
      maxlength: 'Unit cannot exceed 50 characters',
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
    this.stoneForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      unit: ['', [Validators.maxLength(50)]],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Check if we're in edit mode and load stone data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.stoneId = +id;
      this.loadStoneData(this.stoneId);
    }
  }

  /**
   * Load stone data for editing
   */
  loadStoneData(id: number): void {
    this.isLoading = true;
    this.stoneService.getStoneById(id).subscribe({
      next: (stone) => {
        if (stone) {
          this.stoneForm.patchValue({
            name: stone.name,
            unit: stone.unit || '',
            statusId: stone.statusId,
          });
        } else {
          this.toastr.error('Stone not found');
          this.router.navigate(['jewelleryManagement/admin/stone']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/stone']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.stoneForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.stoneForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.stoneForm.get(fieldName);
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
    if (this.stoneForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.stoneForm.value;

    if (this.isEditMode && this.stoneId) {
      // Update existing stone
      const updateData: StoneUpdate = {
        id: this.stoneId,
        name: formValue.name,
        unit: formValue.unit || undefined,
        statusId: formValue.statusId,
      };

      this.stoneService.updateStone(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/stone']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new stone
      const createData: StoneCreate = {
        name: formValue.name,
        unit: formValue.unit || undefined,
        statusId: formValue.statusId,
      };

      this.stoneService.createStone(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/stone']);
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
    Object.keys(this.stoneForm.controls).forEach((key) => {
      this.stoneForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/stone']);
  }
}
