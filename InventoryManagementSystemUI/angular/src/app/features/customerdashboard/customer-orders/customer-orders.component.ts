// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderItemService } from 'src/app/core/services/sale-order-item.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { SaleOrder } from 'src/app/core/models/sale-order.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';
import { SaleOrderItem, formatCurrency, formatWeight } from 'src/app/core/models/sale-order-item.model';

/**
 * Customer Orders Component
 * Displays the order history for the logged-in customer
 */
@Component({
  selector: 'app-customer-orders',
  imports: [SharedModule, CommonModule],
  templateUrl: './customer-orders.component.html',
  styleUrl: './customer-orders.component.scss'
})
export class CustomerOrdersComponent implements OnInit {
  // Injected services
  private saleOrderService = inject(SaleOrderService);
  private saleOrderItemService = inject(SaleOrderItemService);
  private authService = inject(AuthenticationService);

  // Properties
  orders: SaleOrder[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  expandedOrderId: number | null = null;
  orderItemsByOrderId: Record<number, SaleOrderItem[]> = {};
  orderItemsLoadingState: Record<number, boolean> = {};

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    if (this.currentUser && this.currentUser.userId) {
      this.loadMyOrders();
    } else {
      this.isLoading = false;
    }
  }

  /**
   * Load orders for the current customer
   */
  loadMyOrders(): void {
    this.isLoading = true;
    this.saleOrderService.getMyOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading orders:', error);
        this.isLoading = false;
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
      case 3:
        return 'Pending';
      case 4:
        return 'Completed';
      case 5:
        return 'Cancelled';
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
      case 3:
        return 'badge bg-warning';
      case 4:
        return 'badge bg-primary';
      case 5:
        return 'badge bg-secondary';
      default:
        return 'badge bg-secondary';
    }
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  /**
   * Format currency for display
   */
  formatAmount(amount: number | null | undefined): string {
    return formatCurrency(amount ?? 0);
  }

  /**
   * Format weight for display
   */
  formatItemWeight(weight: number | null | undefined): string {
    return formatWeight(weight ?? 0);
  }

  /**
   * Toggle order details in the table
   */
  toggleOrderDetails(order: SaleOrder): void {
    if (this.expandedOrderId === order.id) {
      this.expandedOrderId = null;
      return;
    }

    this.expandedOrderId = order.id;

    if (Object.prototype.hasOwnProperty.call(this.orderItemsByOrderId, order.id)) {
      return;
    }

    this.loadOrderItems(order.id);
  }

  /**
   * Check whether an order is expanded
   */
  isOrderExpanded(orderId: number): boolean {
    return this.expandedOrderId === orderId;
  }

  /**
   * Get all loaded items for the given order
   */
  getOrderItems(orderId: number): SaleOrderItem[] {
    return this.orderItemsByOrderId[orderId] ?? [];
  }

  /**
   * Check whether item details are loading
   */
  isOrderItemsLoading(orderId: number): boolean {
    return this.orderItemsLoadingState[orderId] ?? false;
  }

  /**
   * Load purchased items for a sale order
   */
  private loadOrderItems(orderId: number): void {
    this.orderItemsLoadingState[orderId] = true;

    this.saleOrderItemService.getSaleOrderItemsBySaleOrderId(orderId).subscribe({
      next: (items) => {
        this.orderItemsByOrderId[orderId] = items;
        this.orderItemsLoadingState[orderId] = false;
      },
      error: (error) => {
        console.error(`Error loading items for order ${orderId}:`, error);
        delete this.orderItemsByOrderId[orderId];
        this.orderItemsLoadingState[orderId] = false;
      }
    });
  }
}
