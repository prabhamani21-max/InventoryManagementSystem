// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { PaymentService } from 'src/app/core/services/payment.service';
import {
  Payment,
  PaymentMethod,
  getPaymentMethodLabel,
  getPaymentMethodClass,
  getOrderTypeLabel,
  getOrderTypeClass,
  getStatusLabel,
  getStatusClass,
  formatDate,
  formatCurrency,
} from 'src/app/core/models/payment.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Payment Table Component
 * Displays payments in a table format with CRUD operations
 */
@Component({
  selector: 'app-paymenttable',
  imports: [CommonModule, SharedModule],
  templateUrl: './paymenttable.html',
  styleUrl: './paymenttable.scss',
})
export class Paymenttable implements OnInit {
  // Injected services
  private paymentService = inject(PaymentService);
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
    this.paymentService.getAllPayments().subscribe({
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
    this.router.navigate(['jewelleryManagement/admin/payment/add']);
  }

  /**
   * Navigate to edit payment form
   */
  onEditPayment(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/payment/edit', id]);
  }

  /**
   * Delete payment with confirmation dialog
   */
  onDeletePayment(id: number): void {
    this.confirmationService
      .confirm('Delete Payment', 'Are you sure you want to delete this payment? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.paymentService.deletePayment(id).subscribe({
            next: () => {
              this.loadPayments();
            },
          });
        }
      });
  }

  /**
   * Get payment method label
   */
  getPaymentMethodLabel(method: PaymentMethod | string): string {
    return getPaymentMethodLabel(method);
  }

  /**
   * Get payment method CSS class
   */
  getPaymentMethodClass(method: PaymentMethod | string): string {
    return getPaymentMethodClass(method);
  }

  /**
   * Get order type label
   */
  getOrderTypeLabel(orderType: string): string {
    return getOrderTypeLabel(orderType as any);
  }

  /**
   * Get order type CSS class
   */
  getOrderTypeClass(orderType: string): string {
    return getOrderTypeClass(orderType as any);
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
}
