// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { UserKycService } from 'src/app/core/services/user-kyc.service';
import { UserService } from 'src/app/core/services/user.service';
import { UserKyc, UserKycCreate, UserKycUpdate, getStatusLabel, getStatusClass } from 'src/app/core/models/user-kyc.model';
import { User } from 'src/app/core/models/user.model';
import { ToastrService } from 'ngx-toastr';

/**
 * UserKyc Form Component
 * Handles both create and edit operations for user KYCs
 */
@Component({
  selector: 'app-userkycform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './userkycform.html',
  styleUrl: './userkycform.scss',
})
export class UserKycform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private userKycService = inject(UserKycService);
  private userService = inject(UserService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  userKycForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  userKycId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  userKyc: UserKyc | null = null;
  
  // Users list for dropdown
  users: User[] = [];

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Pending' },
  ];

  // Form validation messages
  validationMessages = {
    userId: {
      required: 'User is required',
    },
    panCardNumber: {
      pattern: 'Invalid PAN card number format',
    },
    aadhaarCardNumber: {
      required: 'Aadhaar card number is required',
      pattern: 'Invalid Aadhaar card number format (should be 12 digits)',
    },
    isVerified: {
      required: 'Verification status is required',
    },
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.prefillUserFromQueryParams();
    this.loadUsers();
    this.checkMode();
  }

  /**
   * Prefill user from query params when opened from sale wizard KYC flow
   */
  private prefillUserFromQueryParams(): void {
    const customerIdParam = this.route.snapshot.queryParamMap.get('customerId');
    if (!customerIdParam) {
      return;
    }

    const customerId = Number(customerIdParam);
    if (!Number.isFinite(customerId) || customerId <= 0) {
      return;
    }

    this.userKycForm.patchValue({
      userId: customerId,
    });
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.userKycForm = this.fb.group({
      userId: ['', [Validators.required]],
      panCardNumber: ['', [Validators.pattern(/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/i)]],
      aadhaarCardNumber: ['', [Validators.required, Validators.pattern(/^\d{12}$/)]],
      isVerified: [false, [Validators.required]],
      statusId: [1, [Validators.required]], // Default to Active
    });
  }

  /**
   * Load users for dropdown
   */
  loadUsers(): void {
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: () => {
        this.toastr.error('Failed to load users');
      },
    });
  }

  /**
   * Check if we're in view/edit mode and load user KYC data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userKycId = +id;
      // Check if it's view mode based on URL
      const urlSegments = this.route.snapshot.url;
      this.isViewMode = urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = !this.isViewMode;
      this.loadUserKycData(this.userKycId);
    }
  }

  /**
   * Load user KYC data for viewing/editing
   */
  loadUserKycData(id: number): void {
    this.isLoading = true;
    this.userKycService.getUserKycById(id).subscribe({
      next: (userKyc) => {
        if (userKyc) {
          this.userKyc = userKyc;
          this.userKycForm.patchValue({
            userId: userKyc.userId,
            panCardNumber: userKyc.panCardNumber,
            aadhaarCardNumber: userKyc.aadhaarCardNumber,
            isVerified: userKyc.isVerified,
            statusId: userKyc.statusId,
          });

          // Disable form in view mode
          if (this.isViewMode) {
            this.userKycForm.disable();
          }
        } else {
          this.toastr.error('User KYC not found');
          this.router.navigate(['jewelleryManagement/admin/userkyc']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/userkyc']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.userKycForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.userKycForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.userKycForm.get(fieldName);
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
    if (this.userKycForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.userKycForm.value;
    
    // Get return URL from query params
    const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');

    if (this.isEditMode && this.userKycId) {
      // Update existing user KYC
      const updateData: UserKycUpdate = {
        id: this.userKycId,
        userId: formValue.userId,
        panCardNumber: formValue.panCardNumber || null,
        aadhaarCardNumber: formValue.aadhaarCardNumber,
        isVerified: formValue.isVerified,
        statusId: formValue.statusId,
      };

      this.userKycService.updateUserKyc(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.handleRedirectAfterSave(returnUrl);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new user KYC
      const createData: UserKycCreate = {
        userId: formValue.userId,
        panCardNumber: formValue.panCardNumber || null,
        aadhaarCardNumber: formValue.aadhaarCardNumber,
        isVerified: formValue.isVerified,
        statusId: formValue.statusId,
      };

      this.userKycService.createUserKyc(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.handleRedirectAfterSave(returnUrl);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    }
  }

  /**
   * Handle redirect after successful KYC save
   */
  private handleRedirectAfterSave(returnUrl: string | null): void {
    if (returnUrl) {
      // Redirect to return URL with kycCompleted query param.
      const returnUrlTree = this.router.parseUrl(returnUrl);
      returnUrlTree.queryParams = {
        ...returnUrlTree.queryParams,
        kycCompleted: 'true',
      };
      this.router.navigateByUrl(returnUrlTree);
    } else {
      this.router.navigate(['jewelleryManagement/admin/userkyc']);
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.userKycForm.controls).forEach((key) => {
      this.userKycForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/userkyc']);
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

  /**
   * Get user name by ID
   */
  getUserName(userId: number): string {
    const user = this.users.find(u => u.id === userId);
    return user ? user.name : `User #${userId}`;
  }
}
