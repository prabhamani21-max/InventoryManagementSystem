// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleOrderItemService } from 'src/app/core/services/sale-order-item.service';
import {
  SaleOrderItem,
  getStatusLabel,
  getStatusClass,
  formatCurrency,
  formatWeight,
  formatDate,
  getMakingChargeTypeLabel,
} from 'src/app/core/models/sale-order-item.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * SaleOrderItem Table Component
 * Displays sale order items in a table format with CRUD operations
 */
@Component({
  selector: 'app-saleorderitemtable',
  imports: [CommonModule, SharedModule],
  templateUrl: './saleorderitemtable.html',
  styleUrl: './saleorderitemtable.scss',
})
export class Saleorderitemtable implements OnInit {
  // Injected services
  private saleOrderItemService = inject(SaleOrderItemService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  saleOrderItems: SaleOrderItem[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = [
    'id',
    'itemName',
    'saleOrderId',
    'quantity',
    'metalAmount',
    'totalAmount',
    'status',
    'actions',
  ];

  ngOnInit(): void {
    this.loadSaleOrderItems();
  }

  /**
   * Load all sale order items from the API
   */
  loadSaleOrderItems(): void {
    this.isLoading = true;
    this.saleOrderItemService.getAllSaleOrderItems().subscribe({
      next: (items) => {
        this.saleOrderItems = items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add sale order item form
   */
  onAddSaleOrderItem(): void {
    this.router.navigate(['jewelleryManagement/admin/saleorderitem/add']);
  }

  /**
   * Navigate to edit sale order item form
   */
  onEditSaleOrderItem(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/saleorderitem/edit', id]);
  }

  /**
   * Delete sale order item with confirmation dialog
   */
  onDeleteSaleOrderItem(id: number): void {
    this.confirmationService
      .confirm(
        'Delete Sale Order Item',
        'Are you sure you want to delete this sale order item? This action cannot be undone.',
        'Delete',
        'Cancel'
      )
      .then((confirmed) => {
        if (confirmed) {
          this.saleOrderItemService.deleteSaleOrderItem(id).subscribe({
            next: () => {
              this.loadSaleOrderItems();
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

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number): string {
    return formatWeight(weight);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Get making charge type label
   */
  getMakingChargeTypeLabel(type: number): string {
    return getMakingChargeTypeLabel(type);
  }

  /**
   * Get stone label
   */
  getStoneLabel(hasStone: boolean): string {
    return hasStone ? 'Yes' : 'No';
  }

  /**
   * Get stone CSS class
   */
  getStoneClass(hasStone: boolean): string {
    return hasStone ? 'badge bg-info' : 'badge bg-light text-dark';
  }

  /**
   * Get hallmark label
   */
  getHallmarkLabel(isHallmarked: boolean): string {
    return isHallmarked ? 'Yes' : 'No';
  }

  /**
   * Get hallmark CSS class
   */
  getHallmarkClass(isHallmarked: boolean): string {
    return isHallmarked ? 'badge bg-success' : 'badge bg-light text-dark';
  }
}
