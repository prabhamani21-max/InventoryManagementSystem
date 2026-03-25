// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { InvoiceService } from 'src/app/core/services/invoice.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { Invoice } from 'src/app/core/models/invoice.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

/**
 * Sales Invoices Component
 * Displays invoices generated from orders created by the logged-in sales person
 */
@Component({
  selector: 'app-sales-invoices',
  imports: [SharedModule, CommonModule, RouterModule],
  templateUrl: './sales-invoices.component.html',
  styleUrl: './sales-invoices.component.scss'
})
export class SalesInvoicesComponent implements OnInit {
  // Injected services
  private invoiceService = inject(InvoiceService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  invoices: Invoice[] = [];
  filteredInvoices: Invoice[] = [];
  isLoading = true;
  currentUser: DecodedToken | null = null;
  
  // Filter properties
  searchTerm = '';
  dateFilter = '';

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    this.loadMyInvoices();
  }

  loadMyInvoices(): void {
    this.isLoading = true;
    this.invoiceService.getMySalesInvoices().subscribe({
      next: (invoices: Invoice[]) => {
        this.invoices = invoices;
        this.filteredInvoices = invoices;
        this.isLoading = false;
      },
      error: (error: Error) => {
        console.error('Error loading invoices:', error);
        this.isLoading = false;
      }
    });
  }

  // Filter methods
  applyFilters(): void {
    let result = [...this.invoices];
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(invoice => 
        invoice.invoiceNumber?.toLowerCase().includes(term) ||
        invoice.partyName?.toLowerCase().includes(term)
      );
    }
    
    if (this.dateFilter) {
      const today = new Date();
      const filterDate = new Date();
      
      switch (this.dateFilter) {
        case 'today':
          filterDate.setHours(0, 0, 0, 0);
          result = result.filter(invoice => {
            const invoiceDate = new Date(invoice.invoiceDate);
            return invoiceDate >= filterDate && invoiceDate <= today;
          });
          break;
        case 'week':
          filterDate.setDate(today.getDate() - 7);
          result = result.filter(invoice => {
            const invoiceDate = new Date(invoice.invoiceDate);
            return invoiceDate >= filterDate;
          });
          break;
        case 'month':
          filterDate.setMonth(today.getMonth() - 1);
          result = result.filter(invoice => {
            const invoiceDate = new Date(invoice.invoiceDate);
            return invoiceDate >= filterDate;
          });
          break;
      }
    }
    
    this.filteredInvoices = result;
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.applyFilters();
  }

  onDateFilterChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    this.dateFilter = select.value;
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.dateFilter = '';
    this.filteredInvoices = [...this.invoices];
  }

  // Navigation methods
  viewInvoice(invoiceNumber: string): void {
    this.router.navigate(['/jewelleryManagement/admin/invoice/view', invoiceNumber]);
  }

  downloadInvoice(invoice: Invoice): void {
    this.invoiceService.downloadInvoicePdfByNumber(invoice.invoiceNumber).subscribe({
      next: (blob: Blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Invoice_${invoice.invoiceNumber}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (error: Error) => {
        console.error('Error downloading invoice:', error);
      }
    });
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

  formatAmount(amount: number | null | undefined): string {
    if (amount === null || amount === undefined) return '₹0.00';
    return '₹' + amount.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }
}
