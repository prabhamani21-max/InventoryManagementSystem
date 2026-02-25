/**
 * User Model
 * Interfaces for User management based on backend DTOs
 */

/**
 * Gender enum
 */
export enum Gender {
  Male = 1,
  Female = 2,
  Other = 3,
}

/**
 * Helper function to get gender label
 */
export function getGenderLabel(gender: number): string {
  switch (gender) {
    case Gender.Male:
      return 'Male';
    case Gender.Female:
      return 'Female';
    case Gender.Other:
      return 'Other';
    default:
      return 'Unknown';
  }
}

/**
 * Main User interface matching UserDto from backend
 */
export interface User {
  id: number;
  name: string;
  email: string;
  password?: string;
  roleId: number;
  statusId: number;
  gender: number;
  dob: Date | string;
  address: string | null;
  contactNumber: string;
  profileImage: string | null;
  createdDate?: Date | string;
  createdBy?: number;
}

/**
 * Interface for creating a new user
 */
export interface UserCreate {
  name: string;
  email: string;
  password: string;
  roleId: number;
  statusId: number;
  gender: number;
  dob: Date | string;
  address?: string | null;
  contactNumber: string;
  profileImage?: string | null;
}

/**
 * Interface for updating an existing user
 */
export interface UserUpdate {
  id: number;
  name: string;
  email: string;
  roleId: number;
  statusId: number;
  gender: number;
  dob: Date | string;
  address?: string | null;
  contactNumber: string;
  profileImage?: string | null;
}

/**
 * Interface for login request
 */
export interface LoginRequest {
  identifier: string;
  password: string;
}

/**
 * Interface for login response
 */
export interface LoginResponse {
  token: string;
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

/**
 * Helper function to format date for display
 */
export function formatDate(date: Date | string | null): string {
  if (!date) return '-';
  const d = new Date(date);
  return d.toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
}

/**
 * Helper function to format date for input field
 */
export function formatDateForInput(date: Date | string | null): string {
  if (!date) return '';
  const d = new Date(date);
  return d.toISOString().split('T')[0];
}