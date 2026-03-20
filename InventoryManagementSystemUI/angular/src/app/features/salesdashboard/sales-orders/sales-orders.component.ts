// angular import
import { Component, OnDestroy, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { SaleOrderItemService } from 'src/app/core/services/sale-order-item.service';
import { SaleOrderNotificationService } from 'src/app/core/services/sale-order-notification.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';
import { UserService } from 'src/app/core/services/user.service';

// model import
import { RoleEnum } from 'src/app/core/enums/role.enum';
import { SaleOrder } from 'src/app/core/models/sale-order.model';
import { SaleOrderItem, formatCurrency, formatWeight } from 'src/app/core/models/sale-order-item.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';
import { User } from 'src/app/core/models/user.model';

/**
 * Sales Orders Component
 * Displays sales orders for the current role
 */
@Component({
  selector: 'app-sales-orders',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-orders.component.html',
  styleUrl: './sales-orders.component.scss'
})
export class SalesOrdersComponent implements OnInit, OnDestroy {
  // Injected services
  private saleOrderService = inject(SaleOrderService);
  private saleOrderItemService = inject(SaleOrderItemService);
  private saleOrderNotificationService = inject(SaleOrderNotificationService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);
  private userService = inject(UserService);
  private destroy$ = new Subject<void>();

  // Properties
  orders: SaleOrder[] = [];
  filteredOrders: SaleOrder[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  currentRole: RoleEnum | null = null;
  expandedOrderId: number | null = null;
  orderItemsByOrderId: Record<number, SaleOrderItem[]> = {};
  orderItemsLoadingState: Record<number, boolean> = {};
  salesPersonNameById: Record<number, string> = {};
  
  // Filter properties
  searchTerm = '';
  statusFilter = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.currentRole = this.authService.getCurrentUserRole();
    this.loadSalesPeople();
    this.loadOrders();
    this.initializeSignalR();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  get isManagerOrAdmin(): boolean {
    return this.currentRole === RoleEnum.SuperAdmin || this.currentRole === RoleEnum.Manager;
  }

  get headerTitle(): string {
    return this.isManagerOrAdmin ? 'All Orders' : 'My Orders';
  }

  get createButtonLabel(): string {
    return this.isManagerOrAdmin ? 'Add Sale Order' : 'New Sale Order';
  }

  get emptyStateActionLabel(): string {
    return this.isManagerOrAdmin ? 'Add Sale Order' : 'Create Your First Order';
  }

  get emptyStateMessage(): string {
    return this.isManagerOrAdmin
      ? 'No sale orders found.'
      : "You haven't created any orders yet.";
  }

  get loadingMessage(): string {
    return this.isManagerOrAdmin ? 'Loading sale orders...' : 'Loading your orders...';
  }

  loadOrders(): void {
    this.isLoading = true;
    const request$ = this.isManagerOrAdmin
      ? this.saleOrderService.getAllSaleOrders()
      : this.saleOrderService.getMySalesOrders();

    request$.subscribe({
      next: (orders: SaleOrder[]) => {
        this.orders = orders;
        this.applyFilters();
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

  // onSearchChange(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   this.searchTerm = input.value;
  //   this.applyFilters();
  // }

  // onStatusFilterChange(event: Event): void {
  //   const select = event.target as HTMLSelectElement;
  //   this.statusFilter = select.value;
  //   this.applyFilters();
  // }

  // clearFilters(): void {
  //   this.searchTerm = '';
  //   this.statusFilter = '';
  //   this.filteredOrders = [...this.orders];
  // }

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

  // SignalR notifications
  private initializeSignalR(): void {
    this.saleOrderNotificationService.startConnection()
      .then(() => this.saleOrderNotificationService.joinSalesTeamGroup())
      .then(() => {
        console.log('Connected to SaleOrderHub and joined SalesTeam group');
      })
      .catch((error) => {
        console.error('Failed to connect to SaleOrderHub:', error);
      });

    this.saleOrderNotificationService.saleOrderCreated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.success(`New sale order created: ${notification.OrderNumber}`, 'Sale Order Created');
        this.loadOrders();
      });

    this.saleOrderNotificationService.saleOrderStatusChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.info(`Sale order status changed: ${notification.Status}`, 'Status Updated');
        this.loadOrders();
      });

    this.saleOrderNotificationService.saleOrderDeleted$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.warning(`Sale order deleted: ID ${notification.SaleOrderId}`, 'Sale Order Deleted');
        this.removeOrderFromLists(notification.SaleOrderId);
      });

    this.saleOrderNotificationService.deliveryDateUpdated$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.info(`Delivery date updated for order ID: ${notification.SaleOrderId}`, 'Delivery Date Updated');
        this.loadOrders();
      });

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

  private removeOrderFromLists(orderId: number): void {
    this.orders = this.orders.filter(order => order.id !== orderId);
    this.filteredOrders = this.filteredOrders.filter(order => order.id !== orderId);
    delete this.orderItemsByOrderId[orderId];
    delete this.orderItemsLoadingState[orderId];
    if (this.expandedOrderId === orderId) {
      this.expandedOrderId = null;
    }
  }

  // Navigation methods
  goToCreateOrder(): void {
    if (this.isManagerOrAdmin) {
      this.router.navigate(['/jewelleryManagement/admin/saleorder/add']);
      return;
    }
    this.router.navigate(['/jewelleryManagement/admin/sale-wizard']);
  }

  viewOrderDetails(orderId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/saleorder/edit', orderId]);
  }

  onDeleteSaleOrder(orderId: number): void {
    if (!this.isManagerOrAdmin) {
      return;
    }

    this.confirmationService
      .confirm('Delete Sale Order', 'Are you sure you want to delete this sale order? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (!confirmed) {
          return;
        }

        this.saleOrderService.deleteSaleOrder(orderId).subscribe({
          next: () => {
            this.removeOrderFromLists(orderId);
          },
        });
      });
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

  getSalesPersonDisplay(order: SaleOrder): string {
    if (order.salesPersonName) {
      return order.salesPersonName;
    }

    const salesPersonId = order.salesPersonId;
    if (salesPersonId == null) {
      return '-';
    }

    const mappedName = this.salesPersonNameById[salesPersonId];
    if (mappedName) {
      return mappedName;
    }

    if (this.currentUser && Number(this.currentUser.userId) === salesPersonId && this.currentUser.name) {
      return this.currentUser.name;
    }

    return '-';
  }

  private loadSalesPeople(): void {
    this.userService.getAllUsers().subscribe({
      next: (users: User[]) => {
        const salesPeople = (users ?? []).filter((user) => user.roleId === RoleEnum.Sales);
        this.salesPersonNameById = salesPeople.reduce<Record<number, string>>((acc, user) => {
          if (user.id != null && user.name) {
            acc[user.id] = user.name;
          }
          return acc;
        }, {});
      },
      error: () => {
        this.salesPersonNameById = {};
      },
    });
  }
}
