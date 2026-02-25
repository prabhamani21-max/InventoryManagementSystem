// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { ExchangeService } from 'src/app/core/services/exchange.service';
import {
  ExchangeOrder,
  getStatusLabel,
  getStatusClass,
  getExchangeTypeLabel,
  getExchangeTypeClass,
  formatDate,
  formatCurrency,
  formatWeight,
} from 'src/app/core/models/exchange.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Exchange Table Component
 * Displays exchange orders in a table format with CRUD operations
 */
@Component({
  selector: 'app-exchangetable',
  imports: [CommonModule, SharedModule],
  templateUrl: './exchangetable.html',
  styleUrl: './exchangetable.scss',
})
export class Exchangetable implements OnInit {
  // Injected services
  private exchangeService = inject(ExchangeService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  exchangeOrders: ExchangeOrder[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'orderNumber', 'customerName', 'exchangeType', 'exchangeDate', 'totalCreditAmount', 'status', 'actions'];

  ngOnInit(): void {
    this.loadExchangeOrders();
  }

  /**
   * Load all exchange orders from the API
   */
  loadExchangeOrders(): void {
    this.isLoading = true;
    this.exchangeService.getAllExchangeOrders().subscribe({
      next: (orders) => {
        this.exchangeOrders = orders;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add exchange order form
   */
  onAddExchangeOrder(): void {
    this.router.navigate(['jewelleryManagement/admin/exchange/add']);
  }

  /**
   * Navigate to view exchange order details
   */
  onViewExchangeOrder(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/exchange/view', id]);
  }

  /**
   * Navigate to edit exchange order form
   */
  onEditExchangeOrder(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/exchange/edit', id]);
  }

  /**
   * Complete exchange order with confirmation
   */
  onCompleteExchangeOrder(id: number): void {
    this.confirmationService
      .confirm(
        'Complete Exchange Order',
        'Are you sure you want to complete this exchange order? This will add items to inventory and apply credit to the customer.',
        'Complete',
        'Cancel'
      )
      .then((confirmed) => {
        if (confirmed) {
          this.exchangeService.completeExchangeOrder(id, { exchangeOrderId: id }).subscribe({
            next: () => {
              this.loadExchangeOrders();
            },
          });
        }
      });
  }

  /**
   * Cancel exchange order with confirmation
   */
  onCancelExchangeOrder(id: number): void {
    this.confirmationService
      .confirm('Cancel Exchange Order', 'Are you sure you want to cancel this exchange order? This action cannot be undone.', 'Cancel Order', 'Keep Order')
      .then((confirmed) => {
        if (confirmed) {
          this.exchangeService.cancelExchangeOrder(id, {}).subscribe({
            next: () => {
              this.loadExchangeOrders();
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
   * Get exchange type label
   */
  getExchangeTypeLabel(exchangeType: string): string {
    return getExchangeTypeLabel(exchangeType);
  }

  /**
   * Get exchange type CSS class
   */
  getExchangeTypeClass(exchangeType: string): string {
    return getExchangeTypeClass(exchangeType);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number | null | undefined): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number | null | undefined): string {
    return formatWeight(weight);
  }

  /**
   * Check if order can be completed (Pending status)
   */
  canComplete(statusId: number): boolean {
    return statusId === 3; // Pending status
  }

  /**
   * Check if order can be cancelled (not already cancelled or completed)
   */
  canCancel(statusId: number): boolean {
    return statusId !== 4 && statusId !== 5; // Not Completed and not Cancelled
  }
}