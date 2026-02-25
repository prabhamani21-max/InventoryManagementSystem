// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { CategoryService } from 'src/app/core/services/category.service';
import { Category, CategoryCreate, CategoryUpdate } from 'src/app/core/models/category.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Category Form Component
 * Handles both create and edit operations for categories
 */
@Component({
  selector: 'app-categoryform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './categoryform.html',
  styleUrl: './categoryform.scss',
})
export class Categoryform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  categoryForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  categoryId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  parentCategories: Category[] = []; // Only parent categories (parentId === null)

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Category name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 100 characters',
    },
    description: {
      maxlength: 'Description cannot exceed 500 characters',
    },
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.loadParentCategories();
    this.checkEditMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      description: ['', [Validators.maxLength(500)]],
      parentId: [null],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Load parent categories for dropdown (only categories with parentId === null)
   */
  loadParentCategories(): void {
    this.categoryService.getAllCategories().subscribe({
      next: (categories) => {
        // Filter only parent categories (where parentId is null)
        this.parentCategories = categories.filter((cat) => cat.parentId === null);
      },
    });
  }

  /**
   * Check if we're in edit mode and load category data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.categoryId = +id;
      this.loadCategoryData(this.categoryId);
    }
  }

  /**
   * Load category data for editing
   */
  loadCategoryData(id: number): void {
    this.isLoading = true;
    this.categoryService.getCategoryById(id).subscribe({
      next: (category) => {
        if (category) {
          this.categoryForm.patchValue({
            name: category.name,
            description: category.description || '',
            parentId: category.parentId,  // Can be null for parent categories
            statusId: category.statusId,
          });
        } else {
          this.toastr.error('Category not found');
          this.router.navigate(['jewelleryManagement/admin/category']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/category']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.categoryForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.categoryForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.categoryForm.get(fieldName);
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
   * Filter out current category from parent dropdown
   * Returns only parent categories (where parentId is null)
   */
  getFilteredParentCategories(): Category[] {
    if (!this.isEditMode || !this.categoryId) {
      return this.parentCategories;
    }
    
    // Filter out current category from parent options to prevent circular reference
    return this.parentCategories.filter((cat) => cat.id !== this.categoryId);
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.categoryForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.categoryForm.value;

    // Handle parentId: if null or empty, it means "None" selected (parent category)
    const parentIdValue = formValue.parentId ? formValue.parentId : null;

    if (this.isEditMode && this.categoryId) {
      // Update existing category
      const updateData: CategoryUpdate = {
        id: this.categoryId,
        name: formValue.name,
        description: formValue.description || undefined,
        parentId: parentIdValue,
        statusId: formValue.statusId,
      };

      this.categoryService.updateCategory(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/category']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new category
      const createData: CategoryCreate = {
        name: formValue.name,
        description: formValue.description || undefined,
        parentId: parentIdValue,
        statusId: formValue.statusId,
      };

      this.categoryService.createCategory(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/category']);
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
    Object.keys(this.categoryForm.controls).forEach((key) => {
      this.categoryForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/category']);
  }
}
