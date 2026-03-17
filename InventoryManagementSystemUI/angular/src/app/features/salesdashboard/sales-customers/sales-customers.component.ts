// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { UserService } from 'src/app/core/services/user.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { User } from 'src/app/core/models/user.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

/**
 * Sales Customers Component
 * Displays customers served by the logged-in sales person
 */
@Component({
  selector: 'app-sales-customers',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-customers.component.html',
  styleUrl: './sales-customers.component.scss'
})
export class SalesCustomersComponent implements OnInit {
  // Injected services
  private userService = inject(UserService);
  private saleOrderService = inject(SaleOrderService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  customers: User[] = [];
  filteredCustomers: User[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  
  // Filter properties
  searchTerm = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.loadMyCustomers();
  }

  loadMyCustomers(): void {
    this.isLoading = true;
    this.userService.getMyCustomers().subscribe({
      next: (customers: User[]) => {
        this.customers = customers;
        this.filteredCustomers = customers;
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading customers:', error);
        this.isLoading = false;
      }
    });
  }

  // Filter methods
  applyFilters(): void {
    let result = [...this.customers];
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(customer => 
        customer.name?.toLowerCase().includes(term) ||
        customer.email?.toLowerCase().includes(term) ||
        customer.contactNumber?.toLowerCase().includes(term)
      );
    }
    
    this.filteredCustomers = result;
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.filteredCustomers = [...this.customers];
  }

  // Navigation methods
  goToNewSale(): void {
    this.router.navigate(['/jewelleryManagement/admin/sale-wizard']);
  }

  createOrderForCustomer(customerId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/sale-wizard'], { 
      queryParams: { customerId } 
    });
  }

  viewCustomerDetails(customerId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/user/view', customerId]);
  }

  // Helper methods
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
