// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { GeneralStatusService } from '../general-status.service';
import { GeneralStatus } from '../general-status.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * General Status Table Component
 * Displays general statuses in a table format with CRUD operations
 */
@Component({
  selector: 'app-generalstatustable',
  imports: [CommonModule, SharedModule],
  templateUrl: './generalstatustable.html',
  styleUrl: './generalstatustable.scss',
})
export class GeneralStatustable implements OnInit {
  // Injected services
  private generalStatusService = inject(GeneralStatusService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  generalStatuses: GeneralStatus[] = [];
  isLoading: boolean = false;

  ngOnInit(): void {
    this.loadGeneralStatuses();
  }

  /**
   * Load all general statuses from the API
   */
  loadGeneralStatuses(): void {
    this.isLoading = true;
    this.generalStatusService.getAllGeneralStatuses().subscribe({
      next: (statuses) => {
        this.generalStatuses = statuses;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add general status form
   */
  onAddGeneralStatus(): void {
    this.router.navigate(['jewelleryManagement/admin/generalstatus/add']);
  }

  /**
   * Navigate to edit general status form
   */
  onEditGeneralStatus(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/generalstatus/edit', id]);
  }

  /**
   * Delete general status with confirmation dialog
   */
  onDeleteGeneralStatus(id: number): void {
    this.confirmationService
      .confirm('Delete General Status', 'Are you sure you want to delete this general status? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.generalStatusService.deleteGeneralStatus(id).subscribe({
            next: () => {
              this.loadGeneralStatuses();
            },
          });
        }
      });
  }

  /**
   * Get status label based on isActive boolean
   */
  getStatusLabel(isActive: boolean): string {
    return isActive ? 'Active' : 'Inactive';
  }

  /**
   * Get status CSS class based on isActive boolean
   */
  getStatusClass(isActive: boolean): string {
    return isActive ? 'badge bg-success' : 'badge bg-danger';
  }
}