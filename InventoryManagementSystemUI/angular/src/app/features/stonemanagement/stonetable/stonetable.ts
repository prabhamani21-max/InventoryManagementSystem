// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { StoneService } from 'src/app/core/services/stone.service';
import { Stone } from 'src/app/core/models/stone.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Stone Table Component
 * Displays stones in a table format with CRUD operations
 */
@Component({
  selector: 'app-stonetable',
  imports: [CommonModule, SharedModule],
  templateUrl: './stonetable.html',
  styleUrl: './stonetable.scss',
})
export class Stonetable implements OnInit {
  // Injected services
  private stoneService = inject(StoneService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  stones: Stone[] = [];
  filteredStones: Stone[] = [];
  isLoading: boolean = false;
  searchTerm: string = '';

  // Table columns
  displayedColumns: string[] = ['name', 'unit', 'status', 'actions'];

  ngOnInit(): void {
    this.loadStones();
  }

  /**
   * Load all stones from the API
   */
  loadStones(): void {
    this.isLoading = true;
    this.stoneService.getAllStones().subscribe({
      next: (stones) => {
        this.stones = stones;
        this.filteredStones = stones;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Search stones by name
   */
  onSearch(): void {
    if (this.searchTerm.trim()) {
      this.stoneService.searchStonesByName(this.searchTerm.trim()).subscribe({
        next: (stones) => {
          this.filteredStones = stones;
        },
      });
    } else {
      this.filteredStones = this.stones;
    }
  }

  /**
   * Clear search and show all stones
   */
  onClearSearch(): void {
    this.searchTerm = '';
    this.filteredStones = this.stones;
  }

  /**
   * Navigate to add stone form
   */
  onAddStone(): void {
    this.router.navigate(['jewelleryManagement/admin/stone/add']);
  }

  /**
   * Navigate to edit stone form
   */
  onEditStone(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/stone/edit', id]);
  }

  /**
   * Delete stone with confirmation dialog
   */
  onDeleteStone(id: number): void {
    this.confirmationService
      .confirm('Delete Stone', 'Are you sure you want to delete this stone? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.stoneService.deleteStone(id).subscribe({
            next: () => {
              this.loadStones();
            },
          });
        }
      });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    // Assuming 1 = Active, 2 = Inactive based on common patterns
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
}
