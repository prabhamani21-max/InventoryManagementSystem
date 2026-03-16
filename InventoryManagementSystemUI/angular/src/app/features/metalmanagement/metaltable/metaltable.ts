// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { MetalService } from 'src/app/core/services/metal.service';
import { Metal } from 'src/app/core/models/metal.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Metal Table Component
 * Displays metals in a table format with CRUD operations
 */
@Component({
  selector: 'app-metaltable',
  imports: [CommonModule, SharedModule],
  templateUrl: './metaltable.html',
  styleUrl: './metaltable.scss',
})
export class Metaltable implements OnInit {
  // Injected services
  private metalService = inject(MetalService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  metals: Metal[] = [];
  isLoading: boolean = false;

  ngOnInit(): void {
    this.loadMetals();
  }

  /**
   * Load all metals from the API
   */
  loadMetals(): void {
    this.isLoading = true;
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add metal form
   */
  onAddMetal(): void {
    this.router.navigate(['jewelleryManagement/admin/metal/add']);
  }

  /**
   * Navigate to edit metal form
   */
  onEditMetal(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/metal/edit', id]);
  }

  /**
   * Delete metal with confirmation dialog
   */
  onDeleteMetal(id: number): void {
    this.confirmationService
      .confirm('Delete Metal', 'Are you sure you want to delete this metal? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.metalService.deleteMetal(id).subscribe({
            next: () => {
              this.loadMetals();
            },
          });
        }
      });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
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
