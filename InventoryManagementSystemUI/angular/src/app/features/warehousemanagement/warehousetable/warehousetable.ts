// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { WarehouseService } from 'src/app/core/services/warehouse.service';
import {
  Warehouse,
  getStatusLabel,
  getStatusClass,
} from 'src/app/core/models/warehouse.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Warehouse Table Component
 * Displays warehouses in a table format with CRUD operations
 */
@Component({
  selector: 'app-warehousetable',
  imports: [CommonModule, SharedModule],
  templateUrl: './warehousetable.html',
  styleUrl: './warehousetable.scss',
})
export class Warehousetable implements OnInit {
  // Injected services
  private warehouseService = inject(WarehouseService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  warehouses: Warehouse[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'name', 'address', 'managerName', 'status', 'actions'];

  ngOnInit(): void {
    this.loadWarehouses();
  }

  /**
   * Load all warehouses from the API
   */
  loadWarehouses(): void {
    this.isLoading = true;
    this.warehouseService.getAllWarehouses().subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add warehouse form
   */
  onAddWarehouse(): void {
    this.router.navigate(['jewelleryManagement/admin/warehouse/add']);
  }

  /**
   * Navigate to view warehouse details
   */
  onViewWarehouse(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/warehouse/view', id]);
  }

  /**
   * Navigate to edit warehouse form
   */
  onEditWarehouse(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/warehouse/edit', id]);
  }

  /**
   * Delete warehouse with confirmation dialog
   */
  onDeleteWarehouse(id: number): void {
    this.confirmationService
      .confirm('Delete Warehouse', 'Are you sure you want to delete this warehouse? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.warehouseService.deleteWarehouse(id).subscribe({
            next: () => {
              this.loadWarehouses();
            },
          });
        }
      });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }
}
