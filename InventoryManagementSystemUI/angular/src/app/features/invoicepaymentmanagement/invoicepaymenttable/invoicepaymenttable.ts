// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoicePaymentService } from 'src/app/core/services/invoice-payment.service';
import {
  Payment,
  getStatusLabel,
  getStatusClass,
  getPaymentMethodLabel,
  getPaymentMethodClass,
  formatDate,
  formatCurrency,
} from 'src/app/core/models/invoice-payment.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * InvoicePayment Table Component
 * Displays payments in a table format with CRUD operations
 */
@Component({
  selector: 'app-invoicepaymenttable',
  imports: [CommonModule, SharedModule],
  templateUrl: './invoicepaymenttable.html',
  styleUrl: './invoicepaymenttable.scss',
})
export class Invoicepaymenttable implements OnInit {
  // Injected services
  private invoicePaymentService = inject(InvoicePaymentService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  payments: Payment[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'orderId', 'orderType', 'amount', 'paymentMethod', 'paymentDate', 'referenceNumber', 'status', 'actions'];

  ngOnInit(): void {
    this.loadPayments();
  }

  /**
   * Load all payments from the API
   */
  loadPayments(): void {
    this.isLoading = true;
    this.invoicePaymentService.getAllPayments().subscribe({
      next: (payments) => {
        this.payments = payments;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add payment form
   */
  onAddPayment(): void {
    this.router.navigate(['jewelleryManagement/admin/invoicepayment/add']);
  }

  /**
   * Navigate to edit payment form
   */
  onEditPayment(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/invoicepayment/edit', id]);
  }

  /**
   * Delete payment with confirmation dialog
   */
  onDeletePayment(id: number): void {
    this.confirmationService
      .confirm('Delete Payment', 'Are you sure you want to delete this payment? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.invoicePaymentService.deletePayment(id).subscribe({
            next: () => {
              this.loadPayments();
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
   * Get payment method label
   */
  getPaymentMethodLabel(method: string): string {
    return getPaymentMethodLabel(method);
  }

  /**
   * Get payment method CSS class
   */
  getPaymentMethodClass(method: string): string {
    return getPaymentMethodClass(method);
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
   * Get order type label
   */
  getOrderTypeLabel(orderType: string): string {
    return orderType === 'SALE' ? 'Sale' : 'Purchase';
  }

  /**
   * Get order type CSS class
   */
  getOrderTypeClass(orderType: string): string {
    return orderType === 'SALE' ? 'badge bg-success' : 'badge bg-info';
  }
}
