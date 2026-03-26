// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { InvoiceService } from 'src/app/core/services/invoice.service';
import { ExchangeService } from 'src/app/core/services/exchange.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { DecodedToken } from 'src/app/core/models/decoded-token.model';
import { SaleOrder } from 'src/app/core/models/sale-order.model';
import { Invoice } from 'src/app/core/models/invoice.model';
import { ExchangeOrder } from 'src/app/core/models/exchange.model';

/**
 * Sales Overview Component
 * Dashboard overview showing sales summary and quick actions
 */
@Component({
  selector: 'app-sales-overview',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-overview.component.html',
  styleUrl: './sales-overview.component.scss'
})
export class SalesOverviewComponent implements OnInit {
  // Injected services
  private saleOrderService = inject(SaleOrderService);
  private invoiceService = inject(InvoiceService);
  private exchangeService = inject(ExchangeService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  currentUser: DecodedToken | null = null;
  isLoading = true;
  
  // Summary data
  totalOrders = 0;
  pendingOrders = 0;
  completedOrders = 0;
  totalInvoices = 0;
  totalExchanges = 0;
  
  // Recent orders
  recentOrders: SaleOrder[] = [];
  recentInvoices: Invoice[] = [];

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.isLoading = true;
    
    // Load orders summary
    this.saleOrderService.getMySalesOrders().subscribe({
      next: (orders: SaleOrder[]) => {
        this.totalOrders = orders.length;
        this.pendingOrders = orders.filter((o: SaleOrder) => o.statusId === 3).length;
        this.completedOrders = orders.filter((o: SaleOrder) => o.statusId === 4).length;
        this.recentOrders = orders.slice(0, 5);
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading orders:', error);
        this.isLoading = false;
      }
    });

    // Load invoices
    this.invoiceService.getMySalesInvoices().subscribe({
      next: (invoices: Invoice[]) => {
        this.totalInvoices = invoices.length;
        this.recentInvoices = invoices.slice(0, 5);
      },
      error: (error: Error) => {
        console.error('Error loading invoices:', error);
      }
    });

    // Load exchanges count
    this.exchangeService.getMyExchanges().subscribe({
      next: (exchanges: ExchangeOrder[]) => {
        this.totalExchanges = exchanges.length;
      },
      error: (error: Error) => {
        console.error('Error loading exchanges:', error);
      }
    });
  }

  // Navigation methods
  goToOrders(): void {
    this.router.navigate(['/jewelleryManagement/admin/sales-dashboard/orders']);
  }

  goToInvoices(): void {
    this.router.navigate(['/jewelleryManagement/admin/sales-dashboard/invoices']);
  }

  goToCustomers(): void {
    this.router.navigate(['/jewelleryManagement/admin/sales-dashboard/customers']);
  }

  goToExchanges(): void {
    this.router.navigate(['/jewelleryManagement/admin/sales-dashboard/exchanges']);
  }

  goToNewSale(): void {
    this.router.navigate(['/jewelleryManagement/admin/sale-wizard']);
  }

  goToNewExchange(): void {
    this.router.navigate(['/jewelleryManagement/admin/exchange/add']);
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
}
