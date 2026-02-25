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
  cgstAmount: number;
  sgstAmount: number;
  igstAmount: number;
  totalGSTAmount: number;
  roundOff: number;
  grandTotal: number;
  grandTotalInWords?: string;

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

  // E-Invoice Fields (GST Compliance)
  irn?: string; // Invoice Reference Number
  irnGeneratedDate?: Date | string;
  qrCode?: string; // Base64 QR code
  eInvoiceStatus?: string; // Generated, Cancelled, Error
  eInvoiceCancelledDate?: Date | string;
  eInvoiceCancelReason?: string;
  acknowledgementNumber?: string;
  acknowledgementDate?: Date | string;

  // Navigation properties
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

/**
 * E-Invoice Response interface for GST Compliance
 */
export interface EInvoiceResponse {
  irn?: string;
  acknowledgementNumber?: string;
  acknowledgementDate?: Date | string;
  qrCode?: string;
  signedInvoice?: string;
  status: string;
  errorMessage?: string;
  errorCode?: string;
  irnGeneratedDate?: Date | string;
  cancelledDate?: Date | string;
  cancelReason?: string;
}

/**
 * E-Invoice Cancel Request interface
 */
export interface EInvoiceCancelRequest {
  cancelReason: string;
}

/**
 * E-Invoice Eligibility Response interface
 */
export interface EInvoiceEligibilityResponse {
  invoiceId: number;
  isEligible: boolean;
  message: string;
}

/**
 * QR Code Response interface
 */
export interface QRCodeResponse {
  invoiceId: number;
  qrCode: string;
  qrCodeUrl: string;
}

/**
 * Helper function to get e-invoice status label
 */
export function getEInvoiceStatusLabel(status: string | undefined): string {
  switch (status) {
    case 'Generated':
      return 'IRN Generated';
    case 'Cancelled':
      return 'E-Invoice Cancelled';
    case 'Error':
      return 'Error';
    case 'Pending':
      return 'Pending';
    default:
      return 'Not Generated';
  }
}

/**
 * Helper function to get e-invoice status CSS class
 */
export function getEInvoiceStatusClass(status: string | undefined): string {
  switch (status) {
    case 'Generated':
      return 'badge bg-success';
    case 'Cancelled':
      return 'badge bg-danger';
    case 'Error':
      return 'badge bg-warning';
    case 'Pending':
      return 'badge bg-info';
    default:
      return 'badge bg-secondary';
  }
}