// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { MetalRateHistoryService } from 'src/app/core/services/metal-rate-history.service';
import { MetalService } from 'src/app/core/services/metal.service';
import { PurityService } from 'src/app/core/services/purity.service';
import { MetalRateResponse, MetalRateHistoryEntry } from 'src/app/core/models/metal-rate-history.model';
import { Metal } from 'src/app/core/models/metal.model';
import { Purity } from 'src/app/core/models/purity.model';
import { ToastrService } from 'ngx-toastr';

/**
 * Metal Rate History Table Component
 * Displays metal rate history in a table format with CRUD operations
 */
@Component({
  selector: 'app-metalratehistorytable',
  imports: [CommonModule, SharedModule],
  templateUrl: './metalratehistorytable.html',
  styleUrl: './metalratehistorytable.scss',
})
export class Metalratehistorytable implements OnInit {
  // Injected services
  private metalRateHistoryService = inject(MetalRateHistoryService);
  private metalService = inject(MetalService);
  private purityService = inject(PurityService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  // Data properties
  currentRates: MetalRateResponse[] = [];
  allRateHistory: MetalRateHistoryEntry[] = [];
  filteredCurrentRates: MetalRateResponse[] = [];
  filteredHistoryRates: MetalRateHistoryEntry[] = [];
  metals: Metal[] = [];
  purities: Purity[] = [];
  
  // UI state
  isLoading: boolean = false;
  showCurrentRatesOnly: boolean = true;
  
  // Filter properties
  searchTerm: string = '';
  selectedMetalId: number | null = null;
  selectedPurityId: number | null = null;
  startDate: string = '';
  endDate: string = '';

  // Table columns for current rates
  currentRateColumns: string[] = ['metalName', 'purityName', 'percentage', 'currentRatePerGram', 'effectiveDate', 'lastUpdated', 'actions'];
  
  // Table columns for all rates history
  historyColumns: string[] = ['metalName', 'purityName', 'ratePerGram', 'effectiveDate', 'createdDate', 'actions'];

  ngOnInit(): void {
    this.loadMetals();
    this.loadPurities();
    this.loadCurrentRates();
  }

  /**
   * Load all metals for filter dropdown
   */
  loadMetals(): void {
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals.filter(m => m.statusId === 1);
      },
    });
  }

  /**
   * Load all purities for filter dropdown
   */
  loadPurities(): void {
    this.purityService.getAllPurities().subscribe({
      next: (purities) => {
        this.purities = purities.filter(p => p.statusId === 1);
      },
    });
  }

  /**
   * Load current metal rates
   */
  loadCurrentRates(): void {
    this.isLoading = true;
    this.showCurrentRatesOnly = true;
    this.metalRateHistoryService.getAllCurrentRates().subscribe({
      next: (rates) => {
        this.currentRates = rates;
        this.filteredCurrentRates = rates;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Load all rate history with optional filters
   */
  loadAllRateHistory(): void {
    this.isLoading = true;
    this.showCurrentRatesOnly = false;
    
    const dateRange = this.getDateRange();
    
    // If a specific metal is selected, load history for that metal only
    if (this.selectedMetalId) {
      this.metalRateHistoryService.getRateHistoryByMetal(this.selectedMetalId, dateRange).subscribe({
        next: (history) => {
          this.allRateHistory = history;
          this.applyFilters();
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        },
      });
    } else {
      // Load history for all metals
      this.metalRateHistoryService.getAllRateHistory(this.metals, dateRange).subscribe({
        next: (history) => {
          this.allRateHistory = history;
          this.applyFilters();
          this.isLoading = false;
        },
        error: () => {
          this.isLoading = false;
        },
      });
    }
  }

  /**
   * Get date range object from filter inputs
   */
  private getDateRange(): { startDate?: string; endDate?: string } | undefined {
    const range: { startDate?: string; endDate?: string } = {};
    if (this.startDate) {
      range.startDate = this.startDate;
    }
    if (this.endDate) {
      range.endDate = this.endDate;
    }
    return Object.keys(range).length > 0 ? range : undefined;
  }

  /**
   * Toggle between current rates and all history
   */
  onToggleView(): void {
    if (this.showCurrentRatesOnly) {
      this.loadCurrentRates();
    } else {
      this.loadAllRateHistory();
    }
  }

  /**
   * Apply filters to the data
   */
  applyFilters(): void {
    if (this.showCurrentRatesOnly) {
      this.filteredCurrentRates = this.filterCurrentRates(this.currentRates);
    } else {
      this.filteredHistoryRates = this.filterHistoryRates(this.allRateHistory);
    }
  }

  /**
   * Filter current rates
   */
  private filterCurrentRates(rates: MetalRateResponse[]): MetalRateResponse[] {
    let filtered = [...rates];
    
    // Filter by search term
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      filtered = filtered.filter(
        (rate) =>
          (rate.metalName && rate.metalName.toLowerCase().includes(term)) ||
          (rate.purityName && rate.purityName.toLowerCase().includes(term)) ||
          rate.currentRatePerGram.toString().includes(term) ||
          rate.percentage.toString().includes(term)
      );
    }
    
    // Filter by metal
    if (this.selectedMetalId) {
      filtered = filtered.filter(rate => rate.metalId === this.selectedMetalId);
    }
    
    // Filter by purity
    if (this.selectedPurityId) {
      filtered = filtered.filter(rate => rate.purityId === this.selectedPurityId);
    }
    
    return filtered;
  }

  /**
   * Filter history rates
   */
  private filterHistoryRates(rates: MetalRateHistoryEntry[]): MetalRateHistoryEntry[] {
    let filtered = [...rates];
    
    // Filter by search term
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      filtered = filtered.filter(
        (rate) =>
          (rate.purityName && rate.purityName.toLowerCase().includes(term)) ||
          rate.ratePerGram.toString().includes(term)
      );
    }
    
    // Filter by purity
    if (this.selectedPurityId) {
      filtered = filtered.filter(rate => rate.purityId === this.selectedPurityId);
    }
    
    return filtered;
  }

  /**
   * Handle search input
   */
  onSearch(): void {
    this.applyFilters();
  }

  /**
   * Clear all filters
   */
  onClearFilters(): void {
    this.searchTerm = '';
    this.selectedMetalId = null;
    this.selectedPurityId = null;
    this.startDate = '';
    this.endDate = '';
    this.applyFilters();
  }

  /**
   * Handle metal filter change
   */
  onMetalFilterChange(): void {
    // Reset purity filter when metal changes
    if (this.selectedMetalId) {
      // Filter purities to show only those belonging to selected metal
      const metalPurities = this.purities.filter(p => p.metalId === this.selectedMetalId);
      if (this.selectedPurityId && !metalPurities.find(p => p.id === this.selectedPurityId)) {
        this.selectedPurityId = null;
      }
    }
    this.applyFilters();
  }

  /**
   * Handle purity filter change
   */
  onPurityFilterChange(): void {
    this.applyFilters();
  }

  /**
   * Handle date filter change
   */
  onDateFilterChange(): void {
    if (!this.showCurrentRatesOnly) {
      this.loadAllRateHistory();
    }
  }

  /**
   * Navigate to add metal rate form
   */
  onAddMetalRate(): void {
    this.router.navigate(['jewelleryManagement/admin/metalratehistory/add']);
  }

  /**
   * Navigate to edit metal rate form
   */
  onEditMetalRate(purityId: number): void {
    this.router.navigate(['jewelleryManagement/admin/metalratehistory/edit', purityId]);
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

  /**
   * Format percentage for display
   */
  formatPercentage(value: number): string {
    return `${value}%`;
  }

  /**
   * Get purity name by ID for history entries
   */
  getPurityName(purityId: number): string {
    const purity = this.purities.find(p => p.id === purityId);
    return purity ? purity.name : '-';
  }

  /**
   * Get metal name by purity ID for history entries
   */
  getMetalNameByPurity(purityId: number): string {
    const purity = this.purities.find(p => p.id === purityId);
    if (purity) {
      const metal = this.metals.find(m => m.id === purity.metalId);
      return metal ? metal.name : '-';
    }
    return '-';
  }

  /**
   * Get filtered purities based on selected metal
   */
  getFilteredPurities(): Purity[] {
    if (this.selectedMetalId) {
      return this.purities.filter(p => p.metalId === this.selectedMetalId);
    }
    return this.purities;
  }
}
