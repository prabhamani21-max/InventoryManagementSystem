import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, ReactiveFormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';
import { Invoice, formatCurrency as formatInvoiceCurrency } from 'src/app/core/models/invoice.model';
import { formatCurrency } from 'src/app/core/models/sale-order-item.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Step5InvoiceComponent
 * Handles invoice generation for the sale wizard
 * Auto-generates invoice when payment is complete
 */
@Component({
  selector: 'app-step5-invoice',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './step5-invoice.component.html',
  styleUrl: './step5-invoice.component.scss',
})
export class Step5InvoiceComponent implements OnInit, OnDestroy {
  private wizardService = inject(SaleWizardService);
  private fb = inject(FormBuilder);
  private toastr = inject(ToastrService);
  
  private destroy$ = new Subject<void>();
  
  // State
  wizardState: SaleWizardState = this.wizardService.getCurrentState();
  
  // Invoice form
  invoiceForm!: UntypedFormGroup;
  isGenerating: boolean = false;
  autoGenerateAttempted: boolean = false;
  
  // Print options
  showPrintPreview: boolean = false;

  ngOnInit(): void {
    this.initializeForm();
    
    // Subscribe to wizard state
    this.wizardService.state$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.wizardState = state;
        // Auto-generate invoice when arriving at this step with complete payment
        this.autoGenerateInvoice();
      });
    
    // Try auto-generate on init
    this.autoGenerateInvoice();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Initialize invoice form
   */
  initializeForm(): void {
    this.invoiceForm = this.fb.group({
      notes: [''],
      includeTermsAndConditions: [true],
    });
  }

  /**
   * Auto-generate invoice when payment is complete
   */
  private autoGenerateInvoice(): void {
    // Only attempt once and only if invoice not already generated
    if (this.autoGenerateAttempted || this.wizardState.generatedInvoice) {
      return;
    }
    
    // Check if payment is complete and we have a sale order
    if (this.isPaymentComplete() && this.wizardState.saleOrder) {
      this.autoGenerateAttempted = true;
      this.generateInvoice();
    }
  }

  /**
   * Generate invoice
   */
  generateInvoice(): void {
    if (!this.wizardState.saleOrder) {
      this.toastr.error('No sale order found');
      return;
    }
    
    if (!this.isPaymentComplete()) {
      this.toastr.warning('Please complete payment before generating invoice');
      return;
    }
    
    // Don't regenerate if already generated
    if (this.wizardState.generatedInvoice) {
      this.toastr.info('Invoice already generated');
      return;
    }
    
    this.isGenerating = true;
    
    const formValue = this.invoiceForm.value;
    
    this.wizardService.generateInvoice(
      this.wizardState.saleOrder.id,
      formValue.notes
    ).subscribe({
      next: (invoice) => {
        this.isGenerating = false;
        this.toastr.success('Invoice generated successfully');
      },
      error: () => {
        this.isGenerating = false;
      },
    });
  }

  /**
   * Check if payment is complete
   */
  isPaymentComplete(): boolean {
    return this.wizardState.balanceDue <= 0;
  }

  /**
   * Format currency
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Print invoice
   */
  printInvoice(): void {
    if (!this.wizardState.generatedInvoice) return;
    
    // Open print dialog
    window.print();
  }

  /**
   * Download invoice as PDF
   */
  downloadInvoice(): void {
    if (!this.wizardState.generatedInvoice) return;
    
    // TODO: Implement PDF generation
    this.toastr.info('PDF download feature coming soon');
  }

  /**
   * Get invoice items total
   */
  getInvoiceItemsTotal(): number {
    if (!this.wizardState.generatedInvoice?.invoiceItems) return 0;
    return this.wizardState.generatedInvoice.invoiceItems.reduce((sum, item) => sum + item.totalAmount, 0);
  }
}
