// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoiceItemService } from 'src/app/core/services/invoice-item.service';
import {
  InvoiceItem,
  formatCurrency,
  formatWeight,
  getHallmarkLabel,
  getHallmarkClass,
} from 'src/app/core/models/invoice-item.model';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * InvoiceItem Table Component
 * Displays invoice items in a table format with delete actions
 */
@Component({
  selector: 'app-invoiceitemtable',
  imports: [CommonModule, SharedModule],
  templateUrl: './invoiceitemtable.html',
  styleUrl: './invoiceitemtable.scss',
})
export class Invoiceitemtable implements OnInit {
  // Injected services
  private invoiceItemService = inject(InvoiceItemService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  invoiceItems: InvoiceItem[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = [
    'id',
    'itemName',
    'invoiceId',
    'quantity',
    'metalAmount',
    'totalAmount',
    'hallmark',
    'actions',
  ];

  ngOnInit(): void {
    this.loadInvoiceItems();
  }

  /**
   * Load all invoice items from the API
   */
  loadInvoiceItems(): void {
    this.isLoading = true;
    this.invoiceItemService.getAllInvoiceItems().subscribe({
      next: (items) => {
        this.invoiceItems = items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Delete invoice item with confirmation dialog
   */
  onDeleteInvoiceItem(id: number): void {
    this.confirmationService
      .confirm(
        'Delete Invoice Item',
        'Are you sure you want to delete this invoice item? This action cannot be undone.',
        'Delete',
        'Cancel'
      )
      .then((confirmed) => {
        if (confirmed) {
          this.invoiceItemService.deleteInvoiceItem(id).subscribe({
            next: () => {
              this.loadInvoiceItems();
            },
          });
        }
      });
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number): string {
    return formatWeight(weight);
  }

  /**
   * Get hallmark label
   */
  getHallmarkLabel(isHallmarked: boolean): string {
    return getHallmarkLabel(isHallmarked);
  }

  /**
   * Get hallmark CSS class
   */
  getHallmarkClass(isHallmarked: boolean): string {
    return getHallmarkClass(isHallmarked);
  }

}
