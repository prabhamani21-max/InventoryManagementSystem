// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderItemService } from 'src/app/core/services/sale-order-item.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { SaleOrder } from 'src/app/core/models/sale-order.model';
import { SaleOrderItem, formatCurrency, formatWeight } from 'src/app/core/models/sale-order-item.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

/**
 * Sales Orders Component
 * Displays orders created by the logged-in sales person
 */
@Component({
  selector: 'app-sales-orders',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-orders.component.html',
  styleUrl: './sales-orders.component.scss'
})
export class SalesOrdersComponent implements OnInit {
  // Injected services
  private saleOrderService = inject(SaleOrderService);
  private saleOrderItemService = inject(SaleOrderItemService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  orders: SaleOrder[] = [];
  filteredOrders: SaleOrder[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  expandedOrderId: number | null = null;
  orderItemsByOrderId: Record<number, SaleOrderItem[]> = {};
  orderItemsLoadingState: Record<number, boolean> = {};
  
  // Filter properties
  searchTerm = '';
  statusFilter = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.loadMyOrders();
  }

  loadMyOrders(): void {
    this.isLoading = true;
    this.saleOrderService.getMySalesOrders().subscribe({
      next: (orders: SaleOrder[]) => {
        this.orders = orders;
        this.filteredOrders = orders;
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading orders:', error);
        this.isLoading = false;
      }
    });
  }

  // Filter methods
  applyFilters(): void {
    let result = [...this.orders];
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(order => 
        order.orderNumber?.toLowerCase().includes(term)
      );
    }
    
    if (this.statusFilter) {
      result = result.filter(order => 
        order.statusId.toString() === this.statusFilter
      );
    }
    
    this.filteredOrders = result;
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.applyFilters();
  }

  onStatusFilterChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.statusFilter = select.value;
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.statusFilter = '';
    this.filteredOrders = [...this.orders];
  }

  // Order details expansion
  toggleOrderDetails(order: SaleOrder): void {
    if (this.expandedOrderId === order.id) {
      this.expandedOrderId = null;
      return;
    }

    this.expandedOrderId = order.id;

    if (!this.orderItemsByOrderId[order.id]) {
      this.loadOrderItems(order.id);
    }
  }

  isOrderExpanded(orderId: number): boolean {
    return this.expandedOrderId === orderId;
  }

  getOrderItems(orderId: number): SaleOrderItem[] {
    return this.orderItemsByOrderId[orderId] || [];
  }

  isOrderItemsLoading(orderId: number): boolean {
    return this.orderItemsLoadingState[orderId] || false;
  }

  private loadOrderItems(orderId: number): void {
    this.orderItemsLoadingState[orderId] = true;

    this.saleOrderItemService.getSaleOrderItemsBySaleOrderId(orderId).subscribe({
      next: (items: SaleOrderItem[]) => {
        this.orderItemsByOrderId[orderId] = items;
        this.orderItemsLoadingState[orderId] = false;
      },
      error: (error: Error) => {
        console.error(`Error loading items for order ${orderId}:`, error);
        delete this.orderItemsByOrderId[orderId];
        this.orderItemsLoadingState[orderId] = false;
      }
    });
  }

  // Navigation methods
  goToSaleWizard(): void {
    this.router.navigate(['/jewelleryManagement/admin/sale-wizard']);
  }

  viewOrderDetails(orderId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/saleorder/edit', orderId]);
  }

  // Helper methods
  getStatusLabel(statusId: number): string {
    switch (statusId) {
      case 1: return 'Active';
      case 2: return 'Inactive';
      case 3: return 'Pending';
      case 4: return 'Completed';
      case 5: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  getStatusClass(statusId: number): string {
    switch (statusId) {
      case 1: return 'badge bg-success';
      case 2: return 'badge bg-danger';
      case 3: return 'badge bg-warning';
      case 4: return 'badge bg-primary';
      case 5: return 'badge bg-secondary';
      default: return 'badge bg-secondary';
    }
  }

  formatDate(date: Date | string | null): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  formatAmount(amount: number | null | undefined): string {
    return formatCurrency(amount ?? 0);
  }

  formatItemWeight(weight: number | null | undefined): string {
    return formatWeight(weight ?? 0);
  }
}
