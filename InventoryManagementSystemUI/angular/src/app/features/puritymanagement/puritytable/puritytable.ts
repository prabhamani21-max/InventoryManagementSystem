// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { PurityService } from 'src/app/core/services/purity.service';
import { Purity } from 'src/app/core/models/purity.model';
import { MetalService } from 'src/app/core/services/metal.service';
import { Metal } from 'src/app/core/models/metal.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Purity Table Component
 * Displays purities in a table format with CRUD operations
 */
@Component({
  selector: 'app-puritytable',
  imports: [CommonModule, SharedModule],
  templateUrl: './puritytable.html',
  styleUrl: './puritytable.scss',
})
export class Puritytable implements OnInit {
  // Injected services
  private purityService = inject(PurityService);
  private metalService = inject(MetalService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  purities: Purity[] = [];
  metals: Metal[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['name', 'metal', 'percentage', 'status', 'actions'];

  ngOnInit(): void {
    this.loadMetals();
    this.loadPurities();
  }

  /**
   * Load all metals for dropdown display
   */
  loadMetals(): void {
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals;
      },
    });
  }

  /**
   * Load all purities from the API
   */
  loadPurities(): void {
    this.isLoading = true;
    this.purityService.getAllPurities().subscribe({
      next: (purities) => {
        this.purities = purities;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add purity form
   */
  onAddPurity(): void {
    this.router.navigate(['jewelleryManagement/admin/purity/add']);
  }

  /**
   * Navigate to edit purity form
   */
  onEditPurity(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/purity/edit', id]);
  }

  /**
   * Delete purity with confirmation dialog
   */
  onDeletePurity(id: number): void {
    this.confirmationService
      .confirm('Delete Purity', 'Are you sure you want to delete this purity? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.purityService.deletePurity(id).subscribe({
            next: () => {
              this.loadPurities();
            },
          });
        }
      });
  }

  /**
   * Get metal name by ID
   */
  getMetalName(metalId: number): string {
    const metal = this.metals.find((m) => m.id === metalId);
    return metal ? metal.name : '-';
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
