/**
 * TCS (Tax Collected at Source) Models
 * Indian GST compliance - Section 206C(1H)
 * Applicable for B2C jewellery sales exceeding ₹10 lakh threshold
 */

/**
 * TCS Calculation Request
 */
export interface TcsCalculationRequest {
  customerId: number;
  saleAmount: number;
  transactionDate: Date | string;
}

/**
 * TCS Calculation Response
 */
export interface TcsCalculationResponse {
  isTcsApplicable: boolean;
  tcsRate: number;
  tcsAmount: number;
  tcsType: string;
  cumulativeSaleAmount: number;
  thresholdLimit: number;
  hasValidPAN: boolean;
  panNumber: string | null;
  isExempted: boolean;
  exemptionReason: string | null;
  financialYear: string;
}

/**
 * TCS Transaction
 */
export interface TcsTransaction {
  id: number;
  invoiceId: number;
  invoiceNumber: string;
  customerId: number;
  customerName: string;
  customerPhone: string | null;
  customerAddress: string | null;
  financialYear: string;
  panNumber: string | null;
  saleAmount: number;
  cumulativeSaleAmount: number;
  tcsRate: number;
  tcsAmount: number;
  tcsType: string;
  isExempted: boolean;
  exemptionReason: string | null;
  transactionDate: Date | string;
  quarter: number;
  createdDate: Date | string;
}

/**
 * Form 26Q Report
 */
export interface Form26QReport {
  financialYear: string;
  quarter: number;
  quarterDescription: string;
  entries: Form26QEntry[];
  totalTcsCollected: number;
  totalTransactions: number;
  totalSaleAmount: number;
  generatedDate: Date | string;
}

/**
 * Form 26Q Entry
 */
export interface Form26QEntry {
  serialNumber: number;
  collecteePAN: string;
  collecteeName: string;
  collecteeAddress: string | null;
  collecteePhone: string | null;
  transactionDate: Date | string;
  invoiceNumber: string;
  amountReceived: number;
  tcsRate: number;
  tcsAmount: number;
  natureOfGoods: string;
  remarks: string | null;
}

/**
 * Customer TCS Summary
 */
export interface CustomerTcsSummary {
  customerId: number;
  customerName: string;
  customerPhone: string | null;
  customerAddress: string | null;
  panNumber: string | null;
  hasValidPAN: boolean;
  financialYear: string;
  totalSales: number;
  totalTcsCollected: number;
  transactionCount: number;
  remainingThreshold: number;
  thresholdLimit: number;
  recentTransactions: TcsTransaction[];
}

/**
 * TCS Dashboard Summary
 */
export interface TcsDashboardSummary {
  financialYear: string;
  totalTcsCollected: number;
  totalTransactions: number;
  totalCustomers: number;
  totalSaleAmount: number;
  quarterlySummaries: TcsQuarterlySummary[];
}

/**
 * TCS Quarterly Summary
 */
export interface TcsQuarterlySummary {
  quarter: number;
  quarterDescription: string;
  tcsCollected: number;
  transactionCount: number;
  saleAmount: number;
}

/**
 * TCS Report Filter
 */
export interface TcsReportFilter {
  financialYear?: string;
  quarter?: number;
  customerId?: number;
  fromDate?: Date | string;
  toDate?: Date | string;
  tcsType?: string;
}

// ==================== Helper Functions ====================

/**
 * Format currency in Indian format
 */
export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    maximumFractionDigits: 2
  }).format(amount);
}

/**
 * Format number in Indian format (with lakhs/crores)
 */
export function formatNumber(amount: number): string {
  return new Intl.NumberFormat('en-IN', {
    maximumFractionDigits: 2
  }).format(amount);
}

/**
 * Get financial year for a date
 * Indian FY: April 1 to March 31
 */
export function getFinancialYear(date: Date): string {
  const month = date.getMonth() + 1;
  const year = date.getFullYear();
  if (month >= 4) {
    return `${year}-${(year + 1).toString().slice(-2)}`;
  }
  return `${year - 1}-${year.toString().slice(-2)}`;
}

/**
 * Get current financial year
 */
export function getCurrentFinancialYear(): string {
  return getFinancialYear(new Date());
}

/**
 * Get quarter for a date
 * Q1: Apr-Jun, Q2: Jul-Sep, Q3: Oct-Dec, Q4: Jan-Mar
 */
export function getQuarter(date: Date): number {
  const month = date.getMonth() + 1;
  if (month >= 4 && month <= 6) return 1;
  if (month >= 7 && month <= 9) return 2;
  if (month >= 10 && month <= 12) return 3;
  return 4;
}

/**
 * Get quarter description
 */
export function getQuarterDescription(quarter: number): string {
  switch (quarter) {
    case 1: return 'Q1 (Apr-Jun)';
    case 2: return 'Q2 (Jul-Sep)';
    case 3: return 'Q3 (Oct-Dec)';
    case 4: return 'Q4 (Jan-Mar)';
    default: return `Q${quarter}`;
  }
}

/**
 * Get TCS type description
 */
export function getTcsTypeDescription(tcsType: string): string {
  switch (tcsType) {
    case 'WithPAN': return 'TCS @ 0.1% (With PAN)';
    case 'WithoutPAN': return 'TCS @ 1% (Without PAN)';
    case 'BelowThreshold': return 'Below Threshold (No TCS)';
    case 'Exempted': return 'Exempted';
    default: return tcsType;
  }
}

/**
 * Get TCS rate display
 */
export function getTcsRateDisplay(rate: number): string {
  return `${(rate * 100).toFixed(1)}%`;
}

/**
 * Available quarters for selection
 */
export const QUARTERS = [
  { value: 1, label: 'Q1 (Apr-Jun)' },
  { value: 2, label: 'Q2 (Jul-Sep)' },
  { value: 3, label: 'Q3 (Oct-Dec)' },
  { value: 4, label: 'Q4 (Jan-Mar)' }
];

/**
 * TCS threshold limit
 */
export const TCS_THRESHOLD = 1000000; // ₹10 lakh
