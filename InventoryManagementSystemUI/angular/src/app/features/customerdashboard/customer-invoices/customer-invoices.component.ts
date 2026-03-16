// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';

// service import
import { InvoiceService } from 'src/app/core/services/invoice.service';
import { AuthenticationService } from 'src/app/core/services/auth.service';

// model import
import { Invoice, formatCurrency } from 'src/app/core/models/invoice.model';
import { DecodedToken } from 'src/app/core/models/decoded-token.model';

/**
 * Customer Invoices Component
 * Displays the invoice history for the logged-in customer with PDF download
 */
@Component({
  selector: 'app-customer-invoices',
  imports: [SharedModule, CommonModule],
  templateUrl: './customer-invoices.component.html',
  styleUrl: './customer-invoices.component.scss'
})
export class CustomerInvoicesComponent implements OnInit {
  // Injected services
  private invoiceService = inject(InvoiceService);
  private authService = inject(AuthenticationService);
  private router = inject(Router);

  // Properties
  invoices: Invoice[] = [];
  isLoading = true;
  downloadingInvoiceNumber: string | null = null;
  currentUser: DecodedToken | null = null;

  ngOnInit(): void {
    this.currentUser = this.authService.getUserInformation();
    if (this.currentUser && this.currentUser.userId) {
      this.loadMyInvoices();
    } else {
      this.isLoading = false;
    }
  }

  /**
   * Load invoices for the current customer
   */
  loadMyInvoices(): void {
    this.isLoading = true;
    this.invoiceService.getMyInvoices().subscribe({
      next: (invoices) => {
        this.invoices = invoices;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading invoices:', error);
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
  formatAmount(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Check if invoice is paid
   */
  isPaid(invoice: Invoice): boolean {
    return invoice.balanceDue <= 0;
  }

  /**
   * Get payment status label
   */
  getPaymentStatusLabel(invoice: Invoice): string {
    if (invoice.balanceDue <= 0) {
      return 'Paid';
    } else if (invoice.totalPaid > 0) {
      return 'Partially Paid';
    }
    return 'Unpaid';
  }

  /**
   * Get payment status CSS class
   */
  getPaymentStatusClass(invoice: Invoice): string {
    if (invoice.balanceDue <= 0) {
      return 'badge bg-success';
    } else if (invoice.totalPaid > 0) {
      return 'badge bg-warning';
    }
    return 'badge bg-danger';
  }

  /**
   * Download invoice as PDF
   */
  downloadInvoicePdf(invoice: Invoice): void {
    if (!invoice.invoiceNumber) return;

    this.downloadingInvoiceNumber = invoice.invoiceNumber;
    
    this.invoiceService.downloadInvoicePdfByNumber(invoice.invoiceNumber).subscribe({
      next: (blob: Blob) => {
        // Create download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Invoice_${invoice.invoiceNumber}.pdf`;
        
        // Trigger download
        document.body.appendChild(link);
        link.click();
        
        // Cleanup
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
        
        this.downloadingInvoiceNumber = null;
      },
      error: (error) => {
        console.error('Error downloading invoice:', error);
        this.downloadingInvoiceNumber = null;
      }
    });
  }



  /**
   * Check if currently downloading this invoice
   */
  isDownloading(invoiceNumber: string): boolean {
    return this.downloadingInvoiceNumber === invoiceNumber;
  }
}
