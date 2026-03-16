// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { StoneRateHistoryService } from 'src/app/core/services/stone-rate-history.service';
import { StoneRateHistory } from 'src/app/core/models/stone-rate-history.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Stone Rate History Table Component
 * Displays stone rate history in a table format with CRUD operations
 */
@Component({
  selector: 'app-stoneratehistorytable',
  imports: [CommonModule, SharedModule],
  templateUrl: './stoneratehistorytable.html',
  styleUrl: './stoneratehistorytable.scss',
})
export class Stoneratehistorytable implements OnInit {
  // Injected services
  private stoneRateHistoryService = inject(StoneRateHistoryService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  stoneRates: StoneRateHistory[] = [];
  filteredStoneRates: StoneRateHistory[] = [];
  isLoading: boolean = false;
  searchTerm: string = '';

  // Table columns
  displayedColumns: string[] = ['stoneName', 'carat', 'cut', 'color', 'clarity', 'grade', 'ratePerUnit', 'effectiveDate', 'status', 'actions'];

  ngOnInit(): void {
    this.loadStoneRates();
  }

  /**
   * Load all stone rates from the API
   */
  loadStoneRates(): void {
    this.isLoading = true;
    this.stoneRateHistoryService.getAllCurrentRates().subscribe({
      next: (stoneRates) => {
        this.stoneRates = stoneRates;
        this.filteredStoneRates = stoneRates;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Search stone rates by stone name or other criteria
   */
  onSearch(): void {
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      this.filteredStoneRates = this.stoneRates.filter(
        (rate) =>
          (rate.stoneName && rate.stoneName.toLowerCase().includes(term)) ||
          (rate.cut && rate.cut.toLowerCase().includes(term)) ||
          (rate.color && rate.color.toLowerCase().includes(term)) ||
          (rate.clarity && rate.clarity.toLowerCase().includes(term)) ||
          (rate.grade && rate.grade.toLowerCase().includes(term)) ||
          rate.carat.toString().includes(term) ||
          rate.ratePerUnit.toString().includes(term)
      );
    } else {
      this.filteredStoneRates = this.stoneRates;
    }
  }

  /**
   * Clear search and show all stone rates
   */
  onClearSearch(): void {
    this.searchTerm = '';
    this.filteredStoneRates = this.stoneRates;
  }

  /**
   * Navigate to add stone rate form
   */
  onAddStoneRate(): void {
    this.router.navigate(['jewelleryManagement/admin/stoneratehistory/add']);
  }

  /**
   * Navigate to edit stone rate form
   */
  onEditStoneRate(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/stoneratehistory/edit', id]);
  }

  /**
   * View rate history for a specific stone
   */
  onViewHistory(stoneId: number): void {
    this.isLoading = true;
    this.stoneRateHistoryService.getRateHistoryByStoneId(stoneId).subscribe({
      next: (history) => {
        this.filteredStoneRates = history;
        this.isLoading = false;
        this.toastr.info(`Showing rate history for stone ID: ${stoneId}`);
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'Active';
      case 2:
        return 'Inactive';
      default:
        return 'Unknown';
    }
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    switch (statusId) {
      case 1:
        return 'badge bg-success';
      case 2:
        return 'badge bg-danger';
      default:
        return 'badge bg-secondary';
    }
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string): string {
    if (!date) return '-';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  /**
   * Format currency for display
   */
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-IN', {
      style: 'currency',
      currency: 'INR',
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    }).format(value);
  }
}
