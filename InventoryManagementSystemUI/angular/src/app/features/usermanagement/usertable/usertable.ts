// angular import
import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { UserService } from 'src/app/core/services/user.service';
import { UserNotificationService, UserRegisteredNotification } from 'src/app/core/services/user-notification.service';
import {
  User,
  getStatusLabel,
  getStatusClass,
  getRoleLabel,
  getRoleClass,
  getGenderLabel,
  formatDate,
} from 'src/app/core/models/user.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * User Table Component
 * Displays users in a table format with CRUD operations
 * Supports real-time updates via SignalR
 */
@Component({
  selector: 'app-usertable',
  imports: [CommonModule, SharedModule],
  templateUrl: './usertable.html',
  styleUrl: './usertable.scss',
})
export class Usertable implements OnInit, OnDestroy {
  // Injected services
  private userService = inject(UserService);
  private userNotificationService = inject(UserNotificationService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  users: User[] = [];
  isLoading: boolean = false;
  isConnected: boolean = false;

  // Subject for managing subscriptions
  private destroy$ = new Subject<void>();

  // Table columns
  displayedColumns: string[] = ['id', 'name', 'email', 'contactNumber', 'role', 'gender', 'status', 'actions'];

  ngOnInit(): void {
    this.loadUsers();
    this.initializeSignalR();
  }

  /**
   * Initialize SignalR connection and subscribe to notifications
   */
  private initializeSignalR(): void {
    // Start SignalR connection
    this.userNotificationService
      .startConnection()
      .then(() => {
        this.isConnected = true;
        this.toastr.success('Real-time notifications connected', 'Connected');
      })
      .catch((error) => {
        console.error('Failed to connect to SignalR:', error);
        this.toastr.warning('Real-time notifications unavailable', 'Warning');
      });

    // Subscribe to connection state changes
    this.userNotificationService.connectionStateChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe((connected) => {
        this.isConnected = connected;
      });

    // Subscribe to user registered notifications
    this.userNotificationService.userRegistered$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification: UserRegisteredNotification) => {
        this.handleUserRegistered(notification);
      });

    // Subscribe to general notifications
    this.userNotificationService.notification$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.toastr.info(notification.Message, 'Notification');
      });

    // Subscribe to user status change notifications
    this.userNotificationService.userStatusChanged$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        this.handleUserStatusChanged(notification);
      });
  }

  /**
   * Handle user registered notification
   * Refreshes the user list and shows a toast notification
   */
  private handleUserRegistered(notification: UserRegisteredNotification): void {
    this.toastr.success(
      `New user registered: ${notification.UserName} (${notification.Email})`,
      'New User Registration'
    );
    // Refresh the user list to include the new user
    this.loadUsers();
  }

  /**
   * Handle user status change notification
   * Updates the user in the local list
   */
  private handleUserStatusChanged(notification: { UserId: number; Status: string }): void {
    const userIndex = this.users.findIndex((u) => u.id === notification.UserId);
    if (userIndex !== -1) {
      // Update the user's status in the local list
      // Note: The status string would need to be converted back to status ID
      // For now, we'll refresh the list
      this.loadUsers();
    }
    this.toastr.info(`User status changed to: ${notification.Status}`, 'Status Update');
  }

  /**
   * Load all users from the API
   */
  loadUsers(): void {
    this.isLoading = true;
    this.userService.getAllUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Navigate to add user form
   */
  onAddUser(): void {
    this.router.navigate(['jewelleryManagement/admin/user/add']);
  }

  /**
   * Navigate to view user details
   */
  onViewUser(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/user/view', id]);
  }

  /**
   * Navigate to edit user form
   */
  onEditUser(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/user/edit', id]);
  }

  /**
   * Delete user with confirmation dialog
   */
  onDeleteUser(id: number): void {
    this.confirmationService
      .confirm('Delete User', 'Are you sure you want to delete this user? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.userService.deleteUser(id).subscribe({
            next: () => {
              this.loadUsers();
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
   * Get role label based on role ID
   */
  getRoleLabel(roleId: number): string {
    return getRoleLabel(roleId);
  }

  /**
   * Get role CSS class based on role ID
   */
  getRoleClass(roleId: number): string {
    return getRoleClass(roleId);
  }

  /**
   * Get gender label
   */
  getGenderLabel(gender: number): string {
    return getGenderLabel(gender);
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Cleanup on component destruction
   */
  ngOnDestroy(): void {
    // Complete the destroy subject to clean up subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }
}