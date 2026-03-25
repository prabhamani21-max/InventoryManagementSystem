// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { ExchangeService } from 'src/app/core/services/exchange.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { ExchangeOrder } from 'src/app/core/models/exchange.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

/**
 * Sales Exchanges Component
 * Displays exchange orders created by the logged-in sales person
 */
@Component({
  selector: 'app-sales-exchanges',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-exchanges.component.html',
  styleUrl: './sales-exchanges.component.scss'
})
export class SalesExchangesComponent implements OnInit {
  // Injected services
  private exchangeService = inject(ExchangeService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  exchanges: ExchangeOrder[] = [];
  filteredExchanges: ExchangeOrder[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  
  // Filter properties
  searchTerm = '';
  statusFilter = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.loadMyExchanges();
  }

  loadMyExchanges(): void {
    this.isLoading = true;
    this.exchangeService.getMyExchanges().subscribe({
      next: (exchanges: ExchangeOrder[]) => {
        this.exchanges = exchanges;
        this.filteredExchanges = exchanges;
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading exchanges:', error);
        this.isLoading = false;
      }
    });
  }

  // Filter methods
  applyFilters(): void {
    let result = [...this.exchanges];
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(exchange => 
        exchange.orderNumber?.toLowerCase().includes(term)
      );
    }
    
    if (this.statusFilter) {
      result = result.filter(exchange => 
        exchange.statusId.toString() === this.statusFilter
      );
    }
    
    this.filteredExchanges = result;
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
    this.filteredExchanges = [...this.exchanges];
  }

  // Navigation methods
  goToNewExchange(): void {
    this.router.navigate(['/jewelleryManagement/admin/exchange/add']);
  }

  viewExchangeDetails(exchangeId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/exchange/view', exchangeId]);
  }

  editExchange(exchangeId: number): void {
    this.router.navigate(['/jewelleryManagement/admin/exchange/edit', exchangeId]);
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
    if (amount === null || amount === undefined) return '₹0.00';
    return '₹' + amount.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }
}
