// angular import
import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { UserService } from 'src/app/core/services/user.service';
import { SaleWizardService } from 'src/app/core/services/sale-wizard.service';
import { RoleService } from 'src/app/core/services/role.service';
import {
  User,
  UserCreate,
  UserUpdate,
  getGenderLabel,
  getStatusLabel,
  getStatusClass,
  getRoleLabel,
  formatDate,
  formatDateForInput,
  Gender,
} from 'src/app/core/models/user.model';
import { ToastrService } from 'ngx-toastr';

/**
 * User Form Component
 * Handles both create and edit operations for users
 * Supports wizard mode for integration with sale wizard
 */
@Component({
  selector: 'app-userform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './userform.html',
  styleUrl: './userform.scss',
})
export class Userform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private wizardService = inject(SaleWizardService);
  private roleService = inject(RoleService);

  // Wizard mode inputs
  @Input() wizardMode: boolean = false;
  @Input() preselectedCustomerId: number | null = null;
  @Output() formSubmitted = new EventEmitter<User>();
  @Output() cancelClicked = new EventEmitter<void>();

  // Properties
  userForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  userId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  user: User | null = null;
  
  // Wizard mode state
  allCustomers: User[] = [];
  filteredCustomers: User[] = [];
  searchQuery: string = '';
  isSearching: boolean = false;
  showCustomerSearch: boolean = false;

  // Gender options
  genderOptions = [
    { id: Gender.Male, name: 'Male' },
    { id: Gender.Female, name: 'Female' },
    { id: Gender.Other, name: 'Other' },
  ];

  // Role options (fetched from API)
  roleOptions: { id: number; name: string }[] = [];

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Pending' },
  ];

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Name is required',
      minlength: 'Name must be at least 2 characters',
    },
    email: {
      required: 'Email is required',
      email: 'Please enter a valid email address',
    },
    password: {
      required: 'Password is required',
      minlength: 'Password must be at least 6 characters',
    },
    contactNumber: {
      required: 'Contact number is required',
      pattern: 'Please enter a valid contact number',
    },
    roleId: {
      required: 'Role is required',
    },
    statusId: {
      required: 'Status is required',
    },
    gender: {
      required: 'Gender is required',
    },
    dob: {
      required: 'Date of birth is required',
    },
  };

  ngOnInit(): void {
    this.loadRoles();
    this.initializeForm();
    
    // Check if wizardMode is set via route data
    this.route.data.subscribe((data) => {
      if (data['wizardMode']) {
        this.wizardMode = true;
      }
    });
    
    if (this.wizardMode) {
      // In wizard mode, load customers for selection
      this.loadCustomers();
      this.showCustomerSearch = true;
      // Make email and password optional in wizard mode
      this.userForm.get('email')?.clearValidators();
      this.userForm.get('email')?.setValidators([Validators.email]);
      this.userForm.get('email')?.updateValueAndValidity();
      this.userForm.get('password')?.clearValidators();
      this.userForm.get('password')?.updateValueAndValidity();
      this.userForm.get('dob')?.clearValidators();
      this.userForm.get('dob')?.updateValueAndValidity();
      // Clear validators for roleId and statusId as they are set programmatically in wizard mode
      this.userForm.get('roleId')?.clearValidators();
      this.userForm.get('roleId')?.updateValueAndValidity();
      this.userForm.get('statusId')?.clearValidators();
      this.userForm.get('statusId')?.updateValueAndValidity();
    } else {
      this.checkMode();
    }
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.userForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      contactNumber: ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      roleId: [4, [Validators.required]], // Default to Customer (roleId = 4)
      statusId: [1, [Validators.required]], // Default to Active
      gender: [Gender.Male, [Validators.required]],
      dob: ['', [Validators.required]],
      address: [''],
      profileImage: [''],
    });
  }

  /**
   * Load roles from the API
   */
  loadRoles(): void {
    this.roleService.getAllRoles().subscribe({
      next: (roles) => {
        // Filter only active roles and map to the format needed for dropdown
        this.roleOptions = roles
          .filter(role => role.statusId === 1)
          .map(role => ({ id: role.id, name: role.name }));
      },
      error: () => {
        // Error is already handled by the service
      },
    });
  }

  /**
   * Load customers for wizard mode selection
   */
  loadCustomers(): void {
    this.isSearching = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        // Filter customers (roleId = 4)
        this.allCustomers = users.filter(u => u.roleId === 4);
        this.filteredCustomers = [...this.allCustomers];
        this.isSearching = false;
      },
      error: () => {
        this.isSearching = false;
        this.toastr.error('Failed to load customers');
      },
    });
  }

  /**
   * Search customers by name or phone
   */
  searchCustomers(): void {
    if (!this.searchQuery || this.searchQuery.trim() === '') {
      this.filteredCustomers = [...this.allCustomers];
      return;
    }
    
    const query = this.searchQuery.toLowerCase().trim();
    this.filteredCustomers = this.allCustomers.filter(customer => 
      customer.name.toLowerCase().includes(query) ||
      customer.contactNumber.includes(query) ||
      (customer.email && customer.email.toLowerCase().includes(query))
    );
  }

  /**
   * Clear search
   */
  clearSearch(): void {
    this.searchQuery = '';
    this.filteredCustomers = [...this.allCustomers];
  }

  /**
   * Select an existing customer in wizard mode
   */
  selectCustomer(customer: User): void {
    this.wizardService.setCustomer(customer, false);
    this.toastr.success(`Customer selected: ${customer.name}`);
    this.formSubmitted.emit(customer);
    // Automatically move to Step 2 (Sale Order) after selecting an existing customer
    this.wizardService.nextStep();
    this.router.navigate(['../order'], { relativeTo: this.route });
  }

  /**
   * Check if we're in view/edit mode and load user data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.userId = +id;
      // Check if it's view mode based on URL
      const urlSegments = this.route.snapshot.url;
      this.isViewMode = urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = !this.isViewMode;
      this.loadUserData(this.userId);
      
      // Remove password required validation in edit mode
      if (this.isEditMode || this.isViewMode) {
        this.userForm.get('password')?.clearValidators();
        this.userForm.get('password')?.updateValueAndValidity();
      }
    }
  }

  /**
   * Load user data for viewing/editing
   */
  loadUserData(id: number): void {
    this.isLoading = true;
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
          this.userForm.patchValue({
            name: user.name,
            email: user.email,
            contactNumber: user.contactNumber,
            roleId: user.roleId,
            statusId: user.statusId,
            gender: user.gender,
            dob: formatDateForInput(user.dob),
            address: user.address || '',
            profileImage: user.profileImage || '',
          });

          // Disable form in view mode
          if (this.isViewMode) {
            this.userForm.disable();
          }
        } else {
          this.toastr.error('User not found');
          this.router.navigate(['jewelleryManagement/admin/user']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/user']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.userForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.userForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.userForm.get(fieldName);
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
    if (this.userForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.userForm.value;

    if (this.wizardMode) {
      // In wizard mode, create customer via wizard service
      const customerData: UserCreate = {
        name: formValue.name,
        email: formValue.email || '',
        password: 'Temp@123', // Temporary password - customer will set their own
        roleId: 4, // Customer role
        statusId: 1, // Active
        gender: formValue.gender,
        dob: formValue.dob || new Date(),
        address: formValue.address || null,
        contactNumber: formValue.contactNumber,
        profileImage: formValue.profileImage || null,
      };

      this.wizardService.createCustomer(customerData).subscribe({
        next: (response) => {
          this.isSubmitting = false;
          this.toastr.success('Customer created successfully');
          
          // Use the returned user from the API response
          if (response.user) {
            this.wizardService.setCustomer(response.user, true);
            this.formSubmitted.emit(response.user);
          } else {
            // Fallback: Create a temporary user object for the wizard state
            const newCustomer: User = {
              id: 0, // Will be updated when list reloads
              name: formValue.name,
              email: formValue.email || '',
              roleId: 4,
              statusId: 1,
              gender: formValue.gender,
              dob: formValue.dob || new Date(),
              address: formValue.address || null,
              contactNumber: formValue.contactNumber,
              profileImage: formValue.profileImage || null,
            };
            this.wizardService.setCustomer(newCustomer, true);
            this.formSubmitted.emit(newCustomer);
          }
          
          // Automatically move to Step 2 (Sale Order) after successful customer creation
          this.wizardService.nextStep();
          this.router.navigate(['../order'], { relativeTo: this.route });
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else if (this.isEditMode && this.userId) {
      // Update existing user
      const updateData: UserUpdate = {
        id: this.userId,
        name: formValue.name,
        email: formValue.email,
        roleId: formValue.roleId,
        statusId: formValue.statusId,
        gender: formValue.gender,
        dob: formValue.dob,
        address: formValue.address || null,
        contactNumber: formValue.contactNumber,
        profileImage: formValue.profileImage || null,
      };

      this.userService.updateUser(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/user']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new user
      const createData: UserCreate = {
        name: formValue.name,
        email: formValue.email,
        password: formValue.password,
        roleId: formValue.roleId,
        statusId: formValue.statusId,
        gender: formValue.gender,
        dob: formValue.dob,
        address: formValue.address || null,
        contactNumber: formValue.contactNumber,
        profileImage: formValue.profileImage || null,
      };

      this.userService.registerUser(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/user']);
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
    Object.keys(this.userForm.controls).forEach((key) => {
      this.userForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    if (this.wizardMode) {
      this.cancelClicked.emit();
    } else {
      this.router.navigate(['jewelleryManagement/admin/user']);
    }
  }

  /**
   * Get gender label
   */
  getGenderLabel(gender: number): string {
    return getGenderLabel(gender);
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
   * Get role label
   */
  getRoleLabel(roleId: number): string {
    return getRoleLabel(roleId);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }
}