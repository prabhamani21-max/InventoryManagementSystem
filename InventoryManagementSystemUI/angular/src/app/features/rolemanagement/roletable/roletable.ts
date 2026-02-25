// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { RoleService } from 'src/app/core/services/role.service';
import { Role, getStatusLabel, getStatusClass } from 'src/app/core/models/role.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * Role Table Component
 * Displays roles in a table format with CRUD operations
 */
@Component({
  selector: 'app-roletable',
  imports: [CommonModule, SharedModule],
  templateUrl: './roletable.html',
  styleUrl: './roletable.scss',
})
export class Roletable implements OnInit {
  // Injected services
  private roleService = inject(RoleService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  roles: Role[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'name', 'status', 'actions'];

  ngOnInit(): void {
    this.loadRoles();
  }

  /**
   * Load all roles from the API
   */
  loadRoles(): void {
    this.isLoading = true;
    this.roleService.getAllRoles().subscribe({
      next: (roles) => {
        this.roles = roles;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add role form
   */
  onAddRole(): void {
    this.router.navigate(['jewelleryManagement/admin/role/add']);
  }

  /**
   * Navigate to view role details
   */
  onViewRole(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/role/view', id]);
  }

  /**
   * Navigate to edit role form
   */
  onEditRole(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/role/edit', id]);
  }

  /**
   * Delete role with confirmation dialog
   */
  onDeleteRole(id: number): void {
    this.confirmationService
      .confirm('Delete Role', 'Are you sure you want to delete this role? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.roleService.deleteRole(id).subscribe({
            next: () => {
              this.loadRoles();
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
}
