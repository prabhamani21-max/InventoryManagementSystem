import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, ReactiveFormsModule } from '@angular/forms';
import { Subject, takeUntil, finalize } from 'rxjs';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';
import { InvoiceItem } from 'src/app/core/models/invoice.model';
import { Payment, getPaymentMethodLabel } from 'src/app/core/models/payment.model';
import { SaleOrderItem, formatCurrency } from 'src/app/core/models/sale-order-item.model';
import { ToastrService } from 'ngx-toastr';

interface DisplayInvoiceItem {
  id: number;
  itemName: string;
  itemCode?: string;
  description?: string;
  quantity: number;
  purityLabel?: string;
  grossWeight?: number;
  netMetalWeight?: number;
  metalRatePerGram?: number;
  metalAmount: number;
  stoneAmount: number;
  wastageAmount: number;
  makingCharges: number;
  discount: number;
  gstAmount: number;
  totalAmount: number;
  isHallmarked: boolean;
  huid?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;
}

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

  // Invoice preview data
  displayInvoiceItems: DisplayInvoiceItem[] = [];
  readonly standardTermsAndConditions: string[] = [
    'Goods once sold will not be taken back without original invoice.',
    'Exchange will be done as per prevailing gold rate and purity testing.',
    'Hallmark and HUID details must be verified at delivery.',
    'Any manufacturing defects should be reported within 7 days of purchase.',
    'Subject to local jurisdiction only.',
  ];

  ngOnInit(): void {
    this.initializeForm();
    
    // Subscribe to wizard state
    this.wizardService.state$
      .pipe(takeUntil(this.destroy$))
      .subscribe((state) => {
        this.wizardState = state;
        this.refreshInvoicePreviewData();
        // Auto-generate invoice when arriving at this step with complete payment
        this.autoGenerateInvoice();
      });
    
    // Try auto-generate on init
    this.refreshInvoicePreviewData();
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
    const saleOrderId = Number(this.wizardState.saleOrder?.id);

    if (!saleOrderId || saleOrderId <= 0) {
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
    
    if (this.isGenerating) {
      return;
    }

    this.isGenerating = true;
    
    const formValue = this.invoiceForm.value;
    
    this.wizardService.generateInvoice(
      saleOrderId,
      formValue.notes,
      !!formValue.includeTermsAndConditions
    )
    .pipe(
      finalize(() => {
        this.isGenerating = false;
      })
    )
    .subscribe({
      next: () => {
        this.toastr.success('Invoice generated successfully');
      },
      error: () => {},
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
    const invoiceElement = document.getElementById('invoice-preview');
    if (!invoiceElement) {
      this.toastr.error('Invoice preview is not available for printing');
      return;
    }

    const printWindow = window.open('', '_blank', 'width=1024,height=768');
    if (!printWindow) {
      this.toastr.error('Unable to open print preview. Please allow pop-ups and try again');
      return;
    }

    const copiedStyleLinks = Array.from(
      document.querySelectorAll('link[rel="stylesheet"]')
    )
      .map((node) => `<link rel="stylesheet" href="${(node as HTMLLinkElement).href}" />`)
      .join('\n');

    const copiedInlineStyles = Array.from(document.querySelectorAll('style'))
      .map((node) => node.outerHTML)
      .join('\n');

    const invoiceNumber = this.wizardState.generatedInvoice.invoiceNumber || '';
    const printThemeStyles = this.getPrintThemeStyles();

    printWindow.document.open();
    printWindow.document.write(`
      <!doctype html>
      <html>
        <head>
          <meta charset="utf-8" />
          <base href="${document.baseURI}" />
          <title>Invoice ${invoiceNumber}</title>
          ${copiedStyleLinks}
          ${copiedInlineStyles}
          <style>
            @page { margin: 8mm; }
            html, body { margin: 0; padding: 0; background: #fff; }
            body, #invoice-preview, #invoice-preview * {
              -webkit-print-color-adjust: exact !important;
              print-color-adjust: exact !important;
            }
            #invoice-preview {
              border: 0 !important;
              box-shadow: none !important;
              margin: 0 !important;
              width: 100% !important;
            }
            ${printThemeStyles}
          </style>
        </head>
        <body>
          ${invoiceElement.outerHTML}
        </body>
      </html>
    `);
    printWindow.document.close();

    const stylesheetLinks = Array.from(
      printWindow.document.querySelectorAll('link[rel="stylesheet"]')
    ) as HTMLLinkElement[];

    const stylesheetReadyPromises = stylesheetLinks.map((link) => {
      return new Promise<void>((resolve) => {
        if (link.sheet) {
          resolve();
          return;
        }

        const done = () => resolve();
        link.addEventListener('load', done, { once: true });
        link.addEventListener('error', done, { once: true });
        setTimeout(done, 2000);
      });
    });

    Promise.all(stylesheetReadyPromises).finally(() => {
      // Extra tick to ensure layout+paint finishes before opening print dialog.
      setTimeout(() => {
        const closeWindow = () => {
          try {
            printWindow.close();
          } catch {
            // no-op
          }
        };

        printWindow.addEventListener('afterprint', closeWindow, { once: true });
        printWindow.focus();
        printWindow.print();
        setTimeout(closeWindow, 800);
      }, 100);
    });
  }

  /**
   * Dedicated print theme for invoice preview
   * Keeps printed output visually close to on-screen UI.
   */
  private getPrintThemeStyles(): string {
    return `
      :root {
        --inv-brand: #2f80ed;
        --inv-text: #1f2937;
        --inv-muted: #4b5563;
        --inv-line: #d8e1ee;
        --inv-soft: #f7fbff;
        --inv-soft-2: #f4f7fb;
        --inv-success: #18b57c;
        --inv-danger: #d93025;
        --inv-info-bg: #d9f2fb;
        --inv-info-border: #90d8ef;
        --inv-info-text: #0a5876;
      }

      body {
        font-family: "Poppins", "Segoe UI", Arial, sans-serif !important;
        color: var(--inv-text) !important;
      }

      #invoice-preview {
        border: 1px solid var(--inv-line) !important;
        border-radius: 8px !important;
        overflow: hidden !important;
      }

      #invoice-preview .card-header {
        background: linear-gradient(90deg, #ecf4ff 0%, #f8fbff 100%) !important;
        border-bottom: 1px solid var(--inv-line) !important;
        padding: 12px 16px !important;
      }

      #invoice-preview .card-header h5,
      #invoice-preview .text-primary,
      #invoice-preview h3,
      #invoice-preview h4 {
        color: var(--inv-brand) !important;
      }

      #invoice-preview .badge {
        border-radius: 999px !important;
        padding: 0.35rem 0.65rem !important;
        font-weight: 600 !important;
      }

      #invoice-preview .bg-success {
        background-color: #16a34a !important;
        color: #fff !important;
      }

      #invoice-preview .bg-warning {
        background-color: #f59e0b !important;
        color: #fff !important;
      }

      #invoice-preview .invoice-header,
      #invoice-preview .customer-details,
      #invoice-preview .invoice-items,
      #invoice-preview .invoice-summary,
      #invoice-preview .payment-summary,
      #invoice-preview .terms-section,
      #invoice-preview .notes-section {
        color: var(--inv-text) !important;
      }

      #invoice-preview .text-muted {
        color: var(--inv-muted) !important;
      }

      #invoice-preview .text-success {
        color: var(--inv-success) !important;
      }

      #invoice-preview .text-danger {
        color: var(--inv-danger) !important;
      }

      #invoice-preview .bg-light {
        background: var(--inv-soft-2) !important;
      }

      #invoice-preview .invoice-meta-strip {
        display: grid !important;
        grid-template-columns: repeat(4, minmax(0, 1fr)) !important;
        gap: 10px !important;
      }

      #invoice-preview .meta-pill {
        border: 1px solid var(--inv-line) !important;
        border-radius: 8px !important;
        background: var(--inv-soft) !important;
        padding: 10px 12px !important;
      }

      #invoice-preview .meta-label {
        color: #6b7280 !important;
        font-size: 11px !important;
        letter-spacing: 0.4px !important;
        text-transform: uppercase !important;
      }

      #invoice-preview .meta-value {
        font-size: 15px !important;
        font-weight: 700 !important;
      }

      #invoice-preview .alert-info {
        background: var(--inv-info-bg) !important;
        border-color: var(--inv-info-border) !important;
        color: var(--inv-info-text) !important;
      }

      #invoice-preview .table-responsive {
        overflow: visible !important;
      }

      #invoice-preview .table {
        width: 100% !important;
        border-collapse: collapse !important;
      }

      #invoice-preview .table th,
      #invoice-preview .table td {
        border: 1px solid var(--inv-line) !important;
        padding: 8px 10px !important;
        vertical-align: top !important;
      }

      #invoice-preview .table thead th,
      #invoice-preview .table tfoot td {
        background: #edf4ff !important;
      }

      #invoice-preview .table thead th {
        font-size: 11px !important;
        text-transform: uppercase !important;
        letter-spacing: 0.4px !important;
      }

      #invoice-preview .amount-words {
        background: var(--inv-soft-2) !important;
      }

      #invoice-preview .signature-line {
        border-top: 1px solid #6b7280 !important;
      }

      #invoice-preview .terms-list {
        padding-left: 18px !important;
      }

      #invoice-preview tr,
      #invoice-preview td,
      #invoice-preview th {
        page-break-inside: avoid !important;
      }
    `;
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
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.totalAmount, 0);
  }

  /**
   * Get total quantity (pieces)
   */
  getTotalPieces(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.quantity, 0);
  }

  /**
   * Get total gross weight
   */
  getTotalGrossWeight(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + (item.grossWeight ?? 0), 0);
  }

  /**
   * Get total net weight
   */
  getTotalNetWeight(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + (item.netMetalWeight ?? 0), 0);
  }

  /**
   * Get total metal amount
   */
  getTotalMetalAmount(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.metalAmount, 0);
  }

  /**
   * Get total stone amount
   */
  getTotalStoneAmount(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.stoneAmount, 0);
  }

  /**
   * Get total wastage amount
   */
  getTotalWastageAmount(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.wastageAmount, 0);
  }

  /**
   * Get total making charges
   */
  getTotalMakingCharges(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.makingCharges, 0);
  }

  /**
   * Get total discount amount from line items
   */
  getTotalDiscountFromItems(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.discount, 0);
  }

  /**
   * Get total GST from line items
   */
  getTotalGstFromItems(): number {
    return this.displayInvoiceItems.reduce((sum, item) => sum + item.gstAmount, 0);
  }

  /**
   * Format metal weight
   */
  formatWeight(weight: number | undefined): string {
    if (weight === undefined || weight === null) {
      return '-';
    }
    return `${weight.toFixed(3)} g`;
  }

  /**
   * Get payment method label
   */
  getPaymentMethodLabel(method: Payment['paymentMethod']): string {
    return getPaymentMethodLabel(method);
  }

  /**
   * Show fallback notice when invoice line items are missing
   */
  isUsingSaleOrderItemsFallback(): boolean {
    const invoice = this.wizardState.generatedInvoice;
    if (!invoice) return false;
    return (!invoice.invoiceItems || invoice.invoiceItems.length === 0) && this.displayInvoiceItems.length > 0;
  }

  /**
   * Check if terms should be shown
   */
  shouldShowTermsAndConditions(): boolean {
    const invoice = this.wizardState.generatedInvoice;
    if (!invoice) return false;
    return invoice.includeTermsAndConditions ?? true;
  }

  /**
   * Refresh computed invoice preview data
   */
  private refreshInvoicePreviewData(): void {
    this.displayInvoiceItems = this.buildDisplayInvoiceItems();
  }

  /**
   * Build invoice line items for preview
   */
  private buildDisplayInvoiceItems(): DisplayInvoiceItem[] {
    const invoice = this.wizardState.generatedInvoice;
    if (invoice?.invoiceItems && invoice.invoiceItems.length > 0) {
      return invoice.invoiceItems.map((item, index) =>
        this.mapInvoiceItemToDisplayItem(item, index)
      );
    }

    if (this.wizardState.saleOrderItems.length > 0) {
      return this.wizardState.saleOrderItems.map((item) =>
        this.mapSaleOrderItemToDisplayItem(item)
      );
    }

    return [];
  }

  /**
   * Map invoice item to display item
   */
  private mapInvoiceItemToDisplayItem(item: InvoiceItem, index: number): DisplayInvoiceItem {
    const matchedSaleOrderItem = this.findMatchingSaleOrderItem(item);

    return {
      id: item.id || matchedSaleOrderItem?.id || index + 1,
      itemName: item.itemName,
      itemCode: matchedSaleOrderItem?.itemCode,
      description: matchedSaleOrderItem?.description,
      quantity: item.quantity,
      purityLabel: this.getPurityLabel(item.purityId, matchedSaleOrderItem?.purityId),
      grossWeight: matchedSaleOrderItem?.grossWeight,
      netMetalWeight: item.netMetalWeight ?? matchedSaleOrderItem?.netMetalWeight,
      metalRatePerGram: matchedSaleOrderItem?.metalRatePerGram,
      metalAmount: item.metalAmount ?? matchedSaleOrderItem?.metalAmount ?? 0,
      stoneAmount: item.stoneAmount ?? matchedSaleOrderItem?.stoneAmount ?? 0,
      wastageAmount: item.wastageAmount ?? matchedSaleOrderItem?.wastageAmount ?? 0,
      makingCharges: item.makingCharges ?? matchedSaleOrderItem?.totalMakingCharges ?? 0,
      discount: item.discount,
      gstAmount: item.gstAmount,
      totalAmount: item.totalAmount,
      isHallmarked: item.isHallmarked,
      huid: item.huid || matchedSaleOrderItem?.huid,
      hallmarkCenterName: item.hallmarkCenterName || matchedSaleOrderItem?.hallmarkCenterName,
      hallmarkDate: item.hallmarkDate || matchedSaleOrderItem?.hallmarkDate,
    };
  }

  /**
   * Map sale order item to display item (fallback when invoice item details are not available)
   */
  private mapSaleOrderItemToDisplayItem(item: SaleOrderItem): DisplayInvoiceItem {
    return {
      id: item.id,
      itemName: item.itemName,
      itemCode: item.itemCode,
      description: item.description,
      quantity: item.quantity,
      purityLabel: this.getPurityLabel(item.purityId),
      grossWeight: item.grossWeight,
      netMetalWeight: item.netMetalWeight,
      metalRatePerGram: item.metalRatePerGram,
      metalAmount: item.metalAmount,
      stoneAmount: item.stoneAmount ?? 0,
      wastageAmount: item.wastageAmount,
      makingCharges: item.totalMakingCharges,
      discount: item.discountAmount,
      gstAmount: item.totalGstAmount,
      totalAmount: item.totalAmount,
      isHallmarked: item.isHallmarked,
      huid: item.huid,
      hallmarkCenterName: item.hallmarkCenterName,
      hallmarkDate: item.hallmarkDate,
    };
  }

  /**
   * Find matching sale order item for an invoice item
   */
  private findMatchingSaleOrderItem(invoiceItem: InvoiceItem): SaleOrderItem | undefined {
    const saleOrderItems = this.wizardState.saleOrderItems;

    if (invoiceItem.referenceItemId) {
      const byReference = saleOrderItems.find((saleItem) => saleItem.id === invoiceItem.referenceItemId);
      if (byReference) {
        return byReference;
      }
    }

    const normalizedName = this.normalizeText(invoiceItem.itemName);
    const exactMatch = saleOrderItems.find(
      (saleItem) =>
        this.normalizeText(saleItem.itemName) === normalizedName &&
        saleItem.quantity === invoiceItem.quantity
    );

    if (exactMatch) {
      return exactMatch;
    }

    return saleOrderItems.find(
      (saleItem) => this.normalizeText(saleItem.itemName) === normalizedName
    );
  }

  /**
   * Create readable purity label
   */
  private getPurityLabel(invoicePurityId?: number, saleOrderPurityId?: number): string | undefined {
    const purityId = invoicePurityId ?? saleOrderPurityId;
    if (!purityId) {
      return undefined;
    }
    return `Purity #${purityId}`;
  }

  /**
   * Normalize text for matching
   */
  private normalizeText(value: string | undefined): string {
    return (value || '').trim().toLowerCase();
  }
}
