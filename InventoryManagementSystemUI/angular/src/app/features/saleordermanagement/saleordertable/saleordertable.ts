// angular import
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderNotificationService } from 'src/app/core/services/sale-order-notification.service';
import { SaleOrder, getStatusLabel, getStatusClass, formatDate } from 'src/app/core/models/sale-order.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * SaleOrder Table Component
 * Displays sale orders in a table format with CRUD operations
 * Includes real-time updates via SignalR
 */
@Component({
  selector: 'app-saleordertable',
  imports: [CommonModule, SharedModule],
  templateUrl: './saleordertable.html',
  styleUrl: './saleordertable.scss',
})
export class Saleordertable implements OnInit, OnDestroy {
  // Injected services
  private saleOrderService = inject(SaleOrderService);
  private saleOrderNotificationService = inject(SaleOrderNotificationService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  saleOrders: SaleOrder[] = [];
  isLoading: boolean = false;
  private destroy$ = new Subject<void>();

  // Table columns
  displayedColumns: string[] = ['id', 'orderNumber', 'customerId', 'orderDate', 'deliveryDate', 'isExchangeSale', 'status', 'actions'];

  ngOnInit(): void {
    this.loadSaleOrders();
    this.initializeSignalR();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize SignalR connection and subscribe to notifications
   */
  private initializeSignalR(): void {
    // Start SignalR connection
    this.saleOrderNotificationService.startConnection()
      .then(() => {
        // Join the sales team group to receive all sale order notifications
        return this.saleOrderNotificationService.joinSalesTeamGroup();
      })
      .then(() => {
        console.log('Connected to SaleOrderHub and joined SalesTeam group');
      })
      .catch((error) => {
        console.error('Failed to connect to SaleOrderHub:', error);
      });

    // Subscribe to sale order created notifications
    this.saleOrderNotificationService.saleOrderCreated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.success(`New sale order created: ${notification.OrderNumber}`, 'Sale Order Created');
        this.loadSaleOrders(); // Refresh the list
      });

    // Subscribe to sale order status changed notifications
    this.saleOrderNotificationService.saleOrderStatusChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.info(`Sale order status changed: ${notification.Status}`, 'Status Updated');
        this.loadSaleOrders(); // Refresh the list
      });

    // Subscribe to sale order deleted notifications
    this.saleOrderNotificationService.saleOrderDeleted$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.warning(`Sale order deleted: ID ${notification.SaleOrderId}`, 'Sale Order Deleted');
        // Remove the deleted order from the local list
        this.saleOrders = this.saleOrders.filter(order => order.id !== notification.SaleOrderId);
      });

    // Subscribe to delivery date updated notifications
    this.saleOrderNotificationService.deliveryDateUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.info(`Delivery date updated for order ID: ${notification.SaleOrderId}`, 'Delivery Date Updated');
        this.loadSaleOrders(); // Refresh the list
      });

    // Subscribe to connection state changes
    this.saleOrderNotificationService.connectionStateChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe((isConnected) => {
        if (isConnected) {
          console.log('SaleOrderHub connection established');
        } else {
          console.log('SaleOrderHub connection lost');
        }
      });
  }

  /**
   * Load all sale orders from the API
   */
  loadSaleOrders(): void {
    this.isLoading = true;
    this.saleOrderService.getAllSaleOrders().subscribe({
      next: (orders) => {
        this.saleOrders = orders;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add sale order form
   */
  onAddSaleOrder(): void {
    this.router.navigate(['jewelleryManagement/admin/saleorder/add']);
  }

  /**
   * Navigate to edit sale order form
   */
  onEditSaleOrder(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/saleorder/edit', id]);
  }

  /**
   * Delete sale order with confirmation dialog
   */
  onDeleteSaleOrder(id: number): void {
    this.confirmationService
      .confirm('Delete Sale Order', 'Are you sure you want to delete this sale order? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.saleOrderService.deleteSaleOrder(id).subscribe({
            next: () => {
              this.loadSaleOrders();
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
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Get exchange sale label
   */
  getExchangeSaleLabel(isExchangeSale: boolean): string {
    return isExchangeSale ? 'Yes' : 'No';
  }

  /**
   * Get exchange sale CSS class
   */
  getExchangeSaleClass(isExchangeSale: boolean): string {
    return isExchangeSale ? 'badge bg-info' : 'badge bg-light text-dark';
  }
}
