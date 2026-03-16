// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { RoleService } from 'src/app/core/services/role.service';
import { Role, getStatusLabel, getStatusClass } from 'src/app/core/models/role.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Role Form Component
 * Handles both create and edit operations for roles
 */
@Component({
  selector: 'app-roleform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './roleform.html',
  styleUrl: './roleform.scss',
})
export class Roleform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private roleService = inject(RoleService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  roleForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  roleId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  role: Role | null = null;

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Pending' },
  ];

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Role name is required',
      minlength: 'Role name must be at least 2 characters',
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
    this.roleForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      statusId: [1, [Validators.required]], // Default to Active
    });
  }

  /**
   * Check if we're in view/edit mode and load role data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.roleId = +id;
      // Check if it's view mode based on URL
      const urlSegments = this.route.snapshot.url;
      this.isViewMode = urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = !this.isViewMode;
      this.loadRoleData(this.roleId);
    }
  }

  /**
   * Load role data for viewing/editing
   */
  loadRoleData(id: number): void {
    this.isLoading = true;
    this.roleService.getRoleById(id).subscribe({
      next: (role) => {
        if (role) {
          this.role = role;
          this.roleForm.patchValue({
            name: role.name,
            statusId: role.statusId,
          });

          // Disable form in view mode
          if (this.isViewMode) {
            this.roleForm.disable();
          }
        } else {
          this.toastr.error('Role not found');
          this.router.navigate(['jewelleryManagement/admin/role']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/role']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.roleForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.roleForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.roleForm.get(fieldName);
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
    if (this.roleForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.roleForm.value;

    if (this.isEditMode && this.roleId) {
      // Update existing role
      const updateData = {
        id: this.roleId,
        name: formValue.name,
        statusId: formValue.statusId,
      };

      this.roleService.addEditRole(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/role']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new role
      const createData = {
        id: 0,
        name: formValue.name,
        statusId: formValue.statusId,
      };

      this.roleService.addEditRole(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/role']);
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
    Object.keys(this.roleForm.controls).forEach((key) => {
      this.roleForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/role']);
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
}
