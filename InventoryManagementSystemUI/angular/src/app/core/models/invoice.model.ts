/**
 * Invoice Model
 * Interfaces for Invoice management based on backend DTOs
 */

/**
 * Party Type enum matching backend
 */
export enum PartyType {
  Customer = 0,
  Supplier = 1,
}

/**
 * Transaction Type enum matching backend InvoiceType
 */
export enum TransactionType {
  Sale = 0,
  Purchase = 1,
}

/**
 * Invoice Item interface matching InvoiceItem from backend
 */
export interface InvoiceItem {
  id: number;
  invoiceId: number;
  referenceItemId?: number;
  itemName: string;
  quantity: number;

  // Metal Details
  metalId: number;
  purityId: number;
  metalType?: string;
  purity?: string;
  netMetalWeight?: number;
  metalAmount?: number;

  // Stone Details
  stoneId?: number;
  stoneWeight?: number;
  stoneRate?: number;
  stoneAmount?: number;

  // Making Charges
  makingCharges?: number;
  wastageAmount?: number;

  // Pricing
  itemSubtotal: number;
  discount: number;
  taxableAmount: number;

  // GST Breakdown
  cgstAmount: number;
  sgstAmount: number;
  igstAmount: number;
  gstAmount: number;
  totalAmount: number;

  // Hallmark snapshot
  isHallmarked: boolean;
  hallmarkDetails?: string;
  // Hallmark Details
  huid?: string; // 6-digit alphanumeric Hallmark Unique ID
  bisCertificationNumber?: string;
  hallmarkCenterName?: string;
  hallmarkDate?: Date | string;
}

/**
 * Invoice Payment interface matching InvoicePayment from backend
 */
export interface InvoicePayment {
  id: number;
  invoiceId: number;
  paymentId: number;
  allocatedAmount: number;
  paymentDate: Date | string;
  createdDate: Date | string;
  createdBy: number;
}

/**
 * Main Invoice interface matching Invoice from backend
 */
export interface Invoice {
  id: number;
  invoiceNumber: string;
  invoiceDate: Date | string;
  invoiceType: TransactionType;

  // Company Details
  companyName: string;
  companyAddress?: string;
  companyPhone?: string;
  companyEmail?: string;
  companyGSTIN?: string;
  companyPAN?: string;
  companyHallmarkLicense?: string;

  // Party Details (Customer/Supplier)
  partyId: number;
  partyType: PartyType;
  partyName: string;
  partyAddress?: string;
  partyPhone?: string;
  partyEmail?: string;
  partyGSTIN?: string;
  partyPANNUmber?: string;

  // Order References
  saleOrderId?: number;
  purchaseOrderId?: number;

  // Pricing Summary
  subTotal: number;
  discountAmount: number;
  taxableAmount: number;
  
  // Metal GST (3% on metal value)
  cgstAmount: number; // Metal CGST
  sgstAmount: number; // Metal SGST
  igstAmount: number; // Metal IGST
  
  // Making Charges GST (5% on making charges)
  makingChargesCGSTAmount?: number; // Making Charges CGST
  makingChargesSGSTAmount?: number; // Making Charges SGST
  makingChargesIGSTAmount?: number; // Making Charges IGST
  makingChargesGSTAmount?: number; // Total Making Charges GST
  
  totalGSTAmount: number; // Metal GST + Making Charges GST
  roundOff: number;
  grandTotal: number;
  grandTotalInWords?: string;
  exchangeCreditApplied?: number;
  netAmountPayable?: number;

  // Payment Summary
  totalPaid: number;
  balanceDue: number;

  // Jewellery Summary
  totalGoldWeight?: number;
  totalStoneWeight?: number;
  totalPieces?: number;

  // Footer
  includeTermsAndConditions?: boolean;
  termsAndConditions?: string;
  returnPolicy?: string;
  notes?: string;
  declaration?: string;

  // Audit fields
  createdDate: Date | string;
  createdBy: number;
  updatedBy?: number;
  updatedDate?: Date | string;
  statusId: number;

  // E-Invoice Fields (GST Compliance) - Currently Not Required
  // irn?: string; // Invoice Reference Number
  // irnGeneratedDate?: Date | string;
  // qrCode?: string; // Base64 QR code
  // eInvoiceStatus?: string; // Generated, Cancelled, Error
  // eInvoiceCancelledDate?: Date | string;
  // eInvoiceCancelReason?: string;
  // acknowledgementNumber?: string;
  // acknowledgementDate?: Date | string;

  // Navigation properties
  items?: InvoiceItem[];
  payments?: InvoicePayment[];
  invoiceItems?: InvoiceItem[];
  invoicePayments?: InvoicePayment[];
}

/**
 * Interface for invoice generation request
 */
export interface InvoiceRequest {
  saleOrderId: number;
  notes?: string;
  includeTermsAndConditions?: boolean;
  customTermsAndConditions?: string;
}

/**
 * Interface for bulk invoice generation request
 */
export interface BulkInvoiceRequest {
  saleOrderIds: number[];
  notes?: string;
}

/**
 * Interface for bulk invoice generation result
 */
export interface BulkInvoiceResult {
  totalGenerated: number;
  totalFailed: number;
  errors: string[];
  invoices: Invoice[];
}

/**
 * Interface for number to words conversion response
 */
export interface NumberToWordsResponse {
  number: number;
  words: string;
}

/**
 * Helper function to get party type label
 */
export function getPartyTypeLabel(partyType: PartyType): string {
  switch (partyType) {
    case PartyType.Customer:
      return 'Customer';
    case PartyType.Supplier:
      return 'Supplier';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get party type CSS class
 */
export function getPartyTypeClass(partyType: PartyType): string {
  switch (partyType) {
    case PartyType.Customer:
      return 'badge bg-primary';
    case PartyType.Supplier:
      return 'badge bg-info';
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Helper function to get transaction type label
 */
export function getTransactionTypeLabel(invoiceType: TransactionType): string {
  switch (invoiceType) {
    case TransactionType.Sale:
      return 'Sale';
    case TransactionType.Purchase:
      return 'Purchase';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get transaction type CSS class
 */
export function getTransactionTypeClass(invoiceType: TransactionType): string {
  switch (invoiceType) {
    case TransactionType.Sale:
      return 'badge bg-success';
    case TransactionType.Purchase:
      return 'badge bg-warning';
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Helper function to get status label based on status ID
 */
export function getStatusLabel(statusId: number): string {
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
 * Helper function to get status CSS class based on status ID
 */
export function getStatusClass(statusId: number): string {
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
 * Helper function to format date for display
 */
export function formatDate(date: Date | string | null): string {
  if (!date) return '-';
  const d = new Date(date);
  return d.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

/**
 * Helper function to format currency for display
 */
export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}

/**
 * Helper function to format weight for display
 */
export function formatWeight(weight: number | null | undefined): string {
  if (weight === null || weight === undefined) return '-';
  return `${weight.toFixed(3)} g`;
}
