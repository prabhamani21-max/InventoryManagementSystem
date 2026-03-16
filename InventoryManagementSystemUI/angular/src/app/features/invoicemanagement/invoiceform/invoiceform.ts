// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { InvoiceService } from 'src/app/core/services/invoice.service';
import { SaleOrderService } from 'src/app/core/services/sale-order.service';
import {
  Invoice,
  InvoiceRequest,
  formatCurrency,
  getStatusLabel,
  getStatusClass,
} from 'src/app/core/models/invoice.model';
import { SaleOrder } from 'src/app/core/models/sale-order.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Invoice Form Component
 * Handles invoice generation from sale orders
 */
@Component({
  selector: 'app-invoiceform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './invoiceform.html',
  styleUrl: './invoiceform.scss',
})
export class Invoiceform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private invoiceService = inject(InvoiceService);
  private saleOrderService = inject(SaleOrderService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  invoiceForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  invoiceNumber: string | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  generatedInvoice: Invoice | null = null;

  // Sale order lookup
  saleOrder: SaleOrder | null = null;
  saleOrderIdInput: number | null = null;

  // Form validation messages
  validationMessages = {
    saleOrderId: {
      required: 'Sale Order ID is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.checkViewMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.invoiceForm = this.fb.group({
      saleOrderId: [null, [Validators.required]],
      notes: [''],
      includeTermsAndConditions: [true],
      customTermsAndConditions: [''],
    });
  }

  /**
   * Check if we're in view mode and load invoice data
   */
  checkViewMode(): void {
    const invoiceNum = this.route.snapshot.paramMap.get('invoiceNumber');
    if (invoiceNum) {
      this.isViewMode = true;
      this.invoiceNumber = invoiceNum;
      this.loadInvoiceData(invoiceNum);
    }
  }

  /**
   * Load invoice data for viewing
   */
  loadInvoiceData(invoiceNumber: string): void {
    this.isLoading = true;
    this.invoiceService.getInvoiceByNumber(invoiceNumber).subscribe({
      next: (invoice) => {
        if (invoice) {
          this.generatedInvoice = invoice;
        } else {
          this.toastr.error('Invoice not found');
          this.router.navigate(['jewelleryManagement/admin/invoice']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/invoice']);
      },
    });
  }

  /**
   * Load sale order for preview
   */
  loadSaleOrderForPreview(): void {
    const saleOrderId = this.invoiceForm.get('saleOrderId')?.value;
    if (!saleOrderId) {
      this.toastr.warning('Please enter a Sale Order ID');
      return;
    }

    this.isLoading = true;
    this.saleOrderService.getSaleOrderById(saleOrderId).subscribe({
      next: (order) => {
        if (order) {
          this.saleOrder = order;
          this.saleOrderIdInput = saleOrderId;
        } else {
          this.toastr.warning('Sale order not found');
          this.saleOrder = null;
        }
        this.isLoading = false;
      },
      error: () => {
        this.saleOrder = null;
        this.isLoading = false;
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.invoiceForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.invoiceForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.invoiceForm.get(fieldName);
    if (field && field.errors) {
      const errorKey = Object.keys(field.errors)[0];
      const messages = this.validationMessages[fieldName as keyof typeof this.validationMessages];
      if (messages && typeof messages === 'object') {
        return (messages as Record<string, string>)[errorKey] || 'Invalid value';
      }
    }
    return '';
  }

  /**
   * Handle form submission - Generate Invoice
   */
  onSubmit(): void {
    if (this.invoiceForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.invoiceForm.value;

    const request: InvoiceRequest = {
      saleOrderId: formValue.saleOrderId,
      notes: formValue.notes || undefined,
      includeTermsAndConditions: formValue.includeTermsAndConditions,
      customTermsAndConditions: formValue.customTermsAndConditions || undefined,
    };

    this.invoiceService.generateInvoice(request).subscribe({
      next: (invoice) => {
        this.generatedInvoice = invoice;
        this.isSubmitting = false;
        this.toastr.success('Invoice generated successfully!');
      },
      error: () => {
        this.isSubmitting = false;
      },
    });
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.invoiceForm.controls).forEach((key) => {
      this.invoiceForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/invoice']);
  }

  /**
   * Generate another invoice
   */
  onGenerateAnother(): void {
    this.generatedInvoice = null;
    this.saleOrder = null;
    this.invoiceForm.reset({
      saleOrderId: null,
      notes: '',
      includeTermsAndConditions: true,
      customTermsAndConditions: '',
    });
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  hasExchangeAdjustment(invoice: Invoice | null | undefined = this.generatedInvoice): boolean {
    return (invoice?.exchangeCreditApplied ?? 0) > 0;
  }

  getExchangeCreditApplied(invoice: Invoice | null | undefined = this.generatedInvoice): number {
    return invoice?.exchangeCreditApplied ?? 0;
  }

  getNetAmountPayable(invoice: Invoice | null | undefined = this.generatedInvoice): number {
    return invoice?.netAmountPayable ?? invoice?.grandTotal ?? 0;
  }

  /**
   * Print invoice
   */
  onPrintInvoice(): void {
    window.print();
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
   * View full invoice details
   */
  onViewInvoice(invoiceNumber: string): void {
    this.router.navigate(['jewelleryManagement/admin/invoice/view', invoiceNumber]);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null | undefined): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  }
}
