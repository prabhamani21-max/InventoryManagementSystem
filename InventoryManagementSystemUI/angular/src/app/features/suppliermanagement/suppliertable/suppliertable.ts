// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SupplierService } from 'src/app/core/services/supplier.service';
import { Supplier } from 'src/app/core/models/supplier.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Supplier Table Component
 * Displays suppliers in a table format with CRUD operations
 */
@Component({
  selector: 'app-suppliertable',
  imports: [CommonModule, SharedModule],
  templateUrl: './suppliertable.html',
  styleUrl: './suppliertable.scss',
})
export class Suppliertable implements OnInit {
  // Injected services
  private supplierService = inject(SupplierService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  suppliers: Supplier[] = [];
  filteredSuppliers: Supplier[] = [];
  isLoading: boolean = false;
  searchTerm: string = '';

  // Table columns
  displayedColumns: string[] = ['name', 'contactPerson', 'email', 'phone', 'gstNumber', 'tanNumber', 'status', 'actions'];

  ngOnInit(): void {
    this.loadSuppliers();
  }

  /**
   * Load all suppliers from the API
   */
  loadSuppliers(): void {
    this.isLoading = true;
    this.supplierService.getAllSuppliers().subscribe({
      next: (suppliers) => {
        this.suppliers = suppliers;
        this.filteredSuppliers = suppliers;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Search suppliers by name or contact person
   */
  onSearch(): void {
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      this.filteredSuppliers = this.suppliers.filter(
        (supplier) =>
          supplier.name.toLowerCase().includes(term) ||
          (supplier.contactPerson && supplier.contactPerson.toLowerCase().includes(term)) ||
          (supplier.gstNumber && supplier.gstNumber.toLowerCase().includes(term)) ||
          (supplier.tanNumber && supplier.tanNumber.toLowerCase().includes(term))
      );
    } else {
      this.filteredSuppliers = this.suppliers;
    }
  }

  /**
   * Clear search and show all suppliers
   */
  onClearSearch(): void {
    this.searchTerm = '';
    this.filteredSuppliers = this.suppliers;
  }

  /**
   * Navigate to add supplier form
   */
  onAddSupplier(): void {
    this.router.navigate(['jewelleryManagement/admin/supplier/add']);
  }

  /**
   * Navigate to edit supplier form
   */
  onEditSupplier(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/supplier/edit', id]);
  }

  /**
   * Delete supplier with confirmation dialog
   */
  onDeleteSupplier(id: number): void {
    this.confirmationService
      .confirm('Delete Supplier', 'Are you sure you want to delete this supplier? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.supplierService.deleteSupplier(id).subscribe({
            next: () => {
              this.loadSuppliers();
            },
          });
        }
      });
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
