import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { TcsService } from 'src/app/core/services/tcs.service';
import {
  TcsTransaction,
  Form26QReport,
  TcsDashboardSummary,
  formatCurrency,
  formatNumber,
  getCurrentFinancialYear,
  getQuarterDescription,
  getTcsTypeDescription,
  getTcsRateDisplay,
  QUARTERS
} from 'src/app/core/models/tcs.model';

/**
 * TCS Report Component
 * Displays TCS transactions, dashboard summary, and Form 26Q report
 */
@Component({
  selector: 'app-tcsreport',
  standalone: true,
  imports: [CommonModule, FormsModule, SharedModule],
  templateUrl: './tcsreport.html',
  styleUrl: './tcsreport.scss'
})
export class Tcsreport implements OnInit {
  // Filter options
  financialYear: string = getCurrentFinancialYear();
  selectedQuarter: number | null = null;
  quarters = QUARTERS;

  // Data
  transactions: TcsTransaction[] = [];
  dashboardSummary: TcsDashboardSummary | null = null;
  form26QReport: Form26QReport | null = null;

  // UI State
  isLoading = false;
  activeTab: 'dashboard' | 'transactions' | 'form26q' = 'dashboard';

  // Helper functions for template
  formatCurrency = formatCurrency;
  formatNumber = formatNumber;
  getQuarterDescription = getQuarterDescription;
  getTcsTypeDescription = getTcsTypeDescription;
  getTcsRateDisplay = getTcsRateDisplay;

  constructor(private tcsService: TcsService) {}

  ngOnInit(): void {
    this.loadDashboardSummary();
  }

  /**
   * Load dashboard summary
   */
  loadDashboardSummary(): void {
    this.isLoading = true;
    this.tcsService.getDashboardSummary(this.financialYear).subscribe({
      next: (data) => {
        this.dashboardSummary = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading TCS dashboard:', error);
        this.isLoading = false;
      }
    });
  }

  /**
   * Load TCS transactions
   */
  loadTransactions(): void {
    this.isLoading = true;
    this.tcsService.getTransactions(this.financialYear, this.selectedQuarter ?? undefined).subscribe({
      next: (data) => {
        this.transactions = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading TCS transactions:', error);
        this.isLoading = false;
      }
    });
  }

  /**
   * Generate Form 26Q report
   */
  generateForm26Q(): void {
    if (!this.selectedQuarter) {
      alert('Please select a quarter to generate Form 26Q');
      return;
    }

    this.isLoading = true;
    this.tcsService.generateForm26Q(this.financialYear, this.selectedQuarter).subscribe({
      next: (data) => {
        this.form26QReport = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error generating Form 26Q:', error);
        this.isLoading = false;
        alert('Failed to generate Form 26Q report');
      }
    });
  }

  /**
   * Handle tab change
   */
  onTabChange(tab: 'dashboard' | 'transactions' | 'form26q'): void {
    this.activeTab = tab;
    if (tab === 'dashboard') {
      this.loadDashboardSummary();
    } else if (tab === 'transactions') {
      this.loadTransactions();
    }
  }

  /**
   * Handle filter change
   */
  onFilterChange(): void {
    if (this.activeTab === 'dashboard') {
      this.loadDashboardSummary();
    } else if (this.activeTab === 'transactions') {
      this.loadTransactions();
    }
  }

  /**
   * Export Form 26Q to CSV
   */
  exportToCSV(): void {
    if (!this.form26QReport) return;

    const headers = [
      'S.No',
      'PAN',
      'Name',
      'Address',
      'Phone',
      'Invoice Number',
      'Transaction Date',
      'Amount Received',
      'TCS Rate (%)',
      'TCS Amount',
      'Nature of Goods',
      'Remarks'
    ];

    const rows = this.form26QReport.entries.map((entry) => [
      entry.serialNumber,
      entry.collecteePAN,
      entry.collecteeName,
      entry.collecteeAddress || '',
      entry.collecteePhone || '',
      entry.invoiceNumber,
      this.formatDate(entry.transactionDate),
      entry.amountReceived,
      entry.tcsRate,
      entry.tcsAmount,
      entry.natureOfGoods,
      entry.remarks || ''
    ]);

    const csvContent = [headers, ...rows]
      .map((row) => row.map((cell) => `"${cell}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Form26Q_${this.financialYear}_Q${this.selectedQuarter}.csv`;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string): string {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('en-IN', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Get TCS type badge class
   */
  getTcsTypeBadgeClass(tcsType: string): string {
    switch (tcsType) {
      case 'WithPAN':
        return 'badge bg-success';
      case 'WithoutPAN':
        return 'badge bg-warning';
      case 'BelowThreshold':
        return 'badge bg-secondary';
      case 'Exempted':
        return 'badge bg-info';
      default:
        return 'badge bg-secondary';
    }
  }
}
