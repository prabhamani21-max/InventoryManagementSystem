// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { MetalService } from 'src/app/core/services/metal.service';
import { Metal, MetalCreate, MetalUpdate } from 'src/app/core/models/metal.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Metal Form Component
 * Handles both create and edit operations for metals
 */
@Component({
  selector: 'app-metalform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './metalform.html',
  styleUrl: './metalform.scss',
})
export class Metalform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private metalService = inject(MetalService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  metalForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  metalId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Metal name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 50 characters',
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
    this.metalForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Check if we're in edit mode and load metal data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.metalId = +id;
      this.loadMetalData(this.metalId);
    }
  }

  /**
   * Load metal data for editing
   */
  loadMetalData(id: number): void {
    this.isLoading = true;
    this.metalService.getMetalById(id).subscribe({
      next: (metal) => {
        if (metal) {
          this.metalForm.patchValue({
            name: metal.name,
            statusId: metal.statusId,
          });
        } else {
          this.toastr.error('Metal not found');
          this.router.navigate(['jewelleryManagement/admin/metal']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/metal']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.metalForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.metalForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.metalForm.get(fieldName);
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
    if (this.metalForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.metalForm.value;

    if (this.isEditMode && this.metalId) {
      // Update existing metal
      const updateData: MetalUpdate = {
        id: this.metalId,
        name: formValue.name,
        statusId: formValue.statusId,
      };

      this.metalService.updateMetal(this.metalId, updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/metal']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new metal
      const createData: MetalCreate = {
        name: formValue.name,
        statusId: formValue.statusId,
      };

      this.metalService.createMetal(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/metal']);
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
    Object.keys(this.metalForm.controls).forEach((key) => {
      this.metalForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/metal']);
  }
}
