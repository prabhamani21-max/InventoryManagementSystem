/**
 * Role Model
 * Interfaces for Role management based on backend DTOs
 */

/**
 * Main Role interface matching RoleDto from backend
 */
export interface Role {
  id: number;
  name: string;
  statusId: number;
}

/**
 * Interface for creating a new role
 */
export interface RoleCreate {
  id?: number;
  name: string;
  statusId: number;
}

/**
 * Interface for updating an existing role
 */
export interface RoleUpdate {
  id: number;
  name: string;
  statusId: number;
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
    case 6:
      return 'Deleted';
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
    case 6:
      return 'badge bg-dark';
    default:
      return 'badge bg-secondary';
  }
}

/**
 * Helper function to get role label based on role ID
 */
export function getRoleLabel(roleId: number): string {
  switch (roleId) {
    case 1:
      return 'Super Admin';
    case 2:
      return 'Admin';
    case 3:
      return 'Manager';
    case 4:
      return 'Staff';
    case 5:
      return 'Customer';
    default:
      return 'Unknown';
  }
}

/**
 * Helper function to get role CSS class based on role ID
 */
export function getRoleClass(roleId: number): string {
  switch (roleId) {
    case 1:
      return 'badge bg-danger';
    case 2:
      return 'badge bg-primary';
    case 3:
      return 'badge bg-info';
    case 4:
      return 'badge bg-success';
    case 5:
      return 'badge bg-secondary';
    default:
      return 'badge bg-secondary';
  }
}
