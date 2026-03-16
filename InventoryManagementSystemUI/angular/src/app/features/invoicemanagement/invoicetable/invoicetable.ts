// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoiceService } from 'src/app/core/services/invoice.service';
import {
  Invoice,
  getStatusLabel,
  getStatusClass,
  formatDate,
  formatCurrency,
  getPartyTypeLabel,
  getPartyTypeClass,
} from 'src/app/core/models/invoice.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Invoice Table Component
 * Displays invoices in a table format with operations
 */
@Component({
  selector: 'app-invoicetable',
  imports: [CommonModule, SharedModule],
  templateUrl: './invoicetable.html',
  styleUrl: './invoicetable.scss',
})
export class Invoicetable implements OnInit {
  // Injected services
  private invoiceService = inject(InvoiceService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  // Properties
  invoices: Invoice[] = [];
  isLoading: boolean = false;
  searchInvoiceNumber: string = '';
  downloadingInvoiceNumber: string | null = null;

  // Table columns
  displayedColumns: string[] = [
    'invoiceNumber',
    'invoiceDate',
    'partyName',
    'partyType',
    'grandTotal',
    'balanceDue',
    'status',
    'actions',
  ];

  ngOnInit(): void {
    this.loadAllInvoices();
  }

  /**
   * Load all invoices from the database
   */
  loadAllInvoices(): void {
    this.isLoading = true;
    this.invoiceService.getAllInvoices().subscribe({
      next: (invoices) => {
        this.invoices = invoices;
        this.isLoading = false;
      },
      error: () => {
        this.invoices = [];
        this.isLoading = false;
      },
    });
  }

  /**
   * Search invoice by invoice number
   * If search box is empty, reload all invoices
   */
  onSearchInvoice(): void {
    if (!this.searchInvoiceNumber || this.searchInvoiceNumber.trim() === '') {
      this.loadAllInvoices();
      return;
    }

    this.isLoading = true;
    this.invoiceService.getInvoiceByNumber(this.searchInvoiceNumber.trim()).subscribe({
      next: (invoice) => {
        if (invoice) {
          this.invoices = [invoice];
        } else {
          this.invoices = [];
          this.toastr.info('No invoice found with this number');
        }
        this.isLoading = false;
      },
      error: () => {
        this.invoices = [];
        this.isLoading = false;
      },
    });
  }

  /**
   * Clear search results and reload all invoices
   */
  onClearSearch(): void {
    this.searchInvoiceNumber = '';
    this.loadAllInvoices();
  }

  /**
   * Navigate to generate invoice form
   */
  onGenerateInvoice(): void {
    this.router.navigate(['jewelleryManagement/admin/invoice/generate']);
  }

  /**
   * View invoice details
   */
  onViewInvoice(invoiceNumber: string): void {
    this.router.navigate(['jewelleryManagement/admin/invoice/view', invoiceNumber]);
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
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Get party type label
   */
  getPartyTypeLabel(partyType: number): string {
    return getPartyTypeLabel(partyType);
  }

  /**
   * Get party type CSS class
   */
  getPartyTypeClass(partyType: number): string {
    return getPartyTypeClass(partyType);
  }
}