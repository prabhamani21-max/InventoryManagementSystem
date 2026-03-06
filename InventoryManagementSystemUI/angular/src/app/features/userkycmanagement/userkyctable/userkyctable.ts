// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { UserKycService } from 'src/app/core/services/user-kyc.service';
import { UserKyc, getStatusLabel, getStatusClass, getVerificationLabel, getVerificationClass, maskPanCardNumber, maskAadhaarCardNumber } from 'src/app/core/models/user-kyc.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * UserKyc Table Component
 * Displays user KYCs in a table format with CRUD operations
 */
@Component({
  selector: 'app-userkyctable',
  imports: [CommonModule, SharedModule],
  templateUrl: './userkyctable.html',
  styleUrl: './userkyctable.scss',
})
export class UserKyctable implements OnInit {
  // Injected services
  private userKycService = inject(UserKycService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  userKycs: UserKyc[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'userId', 'panCardNumber', 'aadhaarCardNumber', 'isVerified', 'status', 'actions'];

  ngOnInit(): void {
    this.loadUserKycs();
  }

  /**
   * Load all user KYCs from the API
   */
  loadUserKycs(): void {
    this.isLoading = true;
    this.userKycService.getAllUserKycs().subscribe({
      next: (userKycs) => {
        this.userKycs = userKycs;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add user KYC form
   */
  onAddUserKyc(): void {
    this.router.navigate(['jewelleryManagement/admin/userkyc/add']);
  }

  /**
   * Navigate to view user KYC details
   */
  onViewUserKyc(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/userkyc/view', id]);
  }

  /**
   * Navigate to edit user KYC form
   */
  onEditUserKyc(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/userkyc/edit', id]);
  }

  /**
   * Delete user KYC with confirmation dialog
   */
  onDeleteUserKyc(id: number): void {
    this.confirmationService
      .confirm('Delete User KYC', 'Are you sure you want to delete this user KYC? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.userKycService.deleteUserKyc(id).subscribe({
            next: () => {
              this.loadUserKycs();
            },
          });
        }
      });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }

  /**
   * Get verification label
   */
  getVerificationLabel(isVerified: boolean): string {
    return getVerificationLabel(isVerified);
  }

  /**
   * Get verification CSS class
   */
  getVerificationClass(isVerified: boolean): string {
    return getVerificationClass(isVerified);
  }

  /**
   * Mask PAN card number for display
   */
  maskPanCard(panCardNumber: string | null): string {
    return maskPanCardNumber(panCardNumber);
  }

  /**
   * Mask Aadhaar card number for display
   */
  maskAadhaarCard(aadhaarCardNumber: string | null): string {
    return maskAadhaarCardNumber(aadhaarCardNumber);
  }
}
