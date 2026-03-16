// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { JewelleryItemService } from 'src/app/core/services/jewellery-item.service';
import { JewelleryItem, MakingChargeType, getMakingChargeTypeLabel } from 'src/app/core/models/jewellery-item.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * JewelleryItem Table Component
 * Displays jewellery items in a table format with CRUD operations
 */
@Component({
  selector: 'app-jewelleryitemtable',
  imports: [CommonModule, SharedModule],
  templateUrl: './jewelleryitemtable.html',
  styleUrl: './jewelleryitemtable.scss',
})
export class Jewelleryitemtable implements OnInit {
  // Injected services
  private jewelleryItemService = inject(JewelleryItemService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  jewelleryItems: JewelleryItem[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['itemCode', 'name', 'category', 'metal', 'purity', 'grossWeight', 'netMetalWeight', 'makingChargeType', 'hasStone', 'isHallmarked', 'status', 'actions'];

  ngOnInit(): void {
    this.loadJewelleryItems();
  }

  /**
   * Load all jewellery items from the API
   */
  loadJewelleryItems(): void {
    this.isLoading = true;
    this.jewelleryItemService.getAllJewelleryItems().subscribe({
      next: (items) => {
        this.jewelleryItems = items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add jewellery item form
   */
  onAddJewelleryItem(): void {
    this.router.navigate(['jewelleryManagement/admin/jewellery/add']);
  }

  /**
   * Navigate to edit jewellery item form
   */
  onEditJewelleryItem(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/jewellery/edit', id]);
  }

  /**
   * Delete jewellery item with confirmation dialog
   */
  onDeleteJewelleryItem(id: number): void {
    this.confirmationService
      .confirm('Delete Jewellery Item', 'Are you sure you want to delete this jewellery item? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.jewelleryItemService.deleteJewelleryItem(id).subscribe({
            next: () => {
              this.loadJewelleryItems();
            },
          });
        }
      });
  }

  /**
   * Get making charge type label
   */
  getMakingChargeTypeLabel(type: MakingChargeType): string {
    return getMakingChargeTypeLabel(type);
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

  /**
   * Get making charge type CSS class
   */
  getMakingChargeTypeClass(type: MakingChargeType): string {
    switch (type) {
      case MakingChargeType.PER_GRAM:
        return 'badge bg-info';
      case MakingChargeType.PERCENTAGE:
        return 'badge bg-primary';
      case MakingChargeType.FIXED:
        return 'badge bg-warning';
      default:
        return 'badge bg-secondary';
    }
  }
}
