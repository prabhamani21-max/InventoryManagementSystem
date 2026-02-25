// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { CategoryService } from 'src/app/core/services/category.service';
import { Category } from 'src/app/core/models/category.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Category Table Component
 * Displays categories in a table format with CRUD operations
 */
@Component({
  selector: 'app-categorytable',
  imports: [CommonModule, SharedModule],
  templateUrl: './categorytable.html',
  styleUrl: './categorytable.scss',
})
export class Categorytable implements OnInit {
  // Injected services
  private categoryService = inject(CategoryService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  categories: Category[] = [];
  flattenedCategories: Array<Category & { indent: string }> = [];
  parentCategories: Category[] = []; // Only categories with parentId === null
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['name', 'description', 'parent', 'status', 'actions'];

  ngOnInit(): void {
    this.loadCategories();
  }

  /**
   * Load all categories from the API
   */
  loadCategories(): void {
    this.isLoading = true;
    this.categoryService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.flattenedCategories = this.categoryService.flattenCategories(categories);
        // Filter only parent categories (where parentId is null)
        this.parentCategories = categories.filter((cat) => cat.parentId === null);
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add category form
   */
  onAddCategory(): void {
    this.router.navigate(['jewelleryManagement/admin/category/add']);
  }

  /**
   * Navigate to edit category form
   */
  onEditCategory(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/category/edit', id]);
  }

  /**
   * Delete category with confirmation dialog
   */
  onDeleteCategory(id: number): void {
    this.confirmationService
      .confirm('Delete Category', 'Are you sure you want to delete this category? This action cannot be undone. If the category has subcategories, they will also be affected.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.categoryService.deleteCategory(id).subscribe({
            next: () => {
              this.loadCategories();
            },
          });
        }
      });
  }

  /**
   * Activate a category
   */
  onActivateCategory(id: number): void {
    this.categoryService.activateCategory(id).subscribe({
      next: () => {
        this.loadCategories();
      },
    });
  }

  /**
   * Deactivate a category
   */
  onDeactivateCategory(id: number): void {
    this.categoryService.deactivateCategory(id).subscribe({
      next: () => {
        this.loadCategories();
      },
    });
  }

  /**
   * Get parent category name by ID
   * Only displays parent categories (where parentId is null)
   */
  getParentName(parentId: number | null): string {
    if (!parentId) return '-';
    // Find the parent in parentCategories (only top-level categories)
    const parent = this.parentCategories.find((c) => c.id === parentId);
    return parent ? parent.name : '-';
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    // Assuming 1 = Active, 2 = Inactive based on common patterns
    switch (statusId) {
      case 1:
        return 'Active';
      case 2:
        return 'Inactive';
      default:
        return 'Unknown';
    }
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'badge bg-success';
      case 2:
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }
}
