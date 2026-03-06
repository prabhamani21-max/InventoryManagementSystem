/**
 * UserKYC Model
 * Interfaces for UserKYC management based on backend DTOs
 */

/**
 * Main UserKYC interface matching UserKycDto from backend
 */
export interface UserKyc {
  id: number;
  userId: number;
  panCardNumber: string | null;
  aadhaarCardNumber: string;
  isVerified: boolean;
  statusId: number;
}

/**
 * Interface for creating a new UserKYC
 */
export interface UserKycCreate {
  userId: number;
  panCardNumber: string | null;
  aadhaarCardNumber: string;
  isVerified: boolean;
  statusId: number;
}

/**
 * Interface for updating an existing UserKYC
 */
export interface UserKycUpdate {
  id: number;
  userId: number;
  panCardNumber: string | null;
  aadhaarCardNumber: string;
  isVerified: boolean;
  statusId: number;
}

/**
 * Interface for UserKYC with user details
 */
export interface UserKycWithUser extends UserKyc {
  userName?: string;
  userEmail?: string;
  userContactNumber?: string;
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
 * Helper function to get verification label
 */
export function getVerificationLabel(isVerified: boolean): string {
  return isVerified ? 'Verified' : 'Not Verified';
}

/**
 * Helper function to get verification CSS class
 */
export function getVerificationClass(isVerified: boolean): string {
  return isVerified ? 'badge bg-success' : 'badge bg-warning';
}

/**
 * Helper function to mask PAN card number
 */
export function maskPanCardNumber(panCardNumber: string | null): string {
  if (!panCardNumber) return '-';
  if (panCardNumber.length < 4) return panCardNumber;
  return panCardNumber.substring(0, 2) + 'XXXX' + panCardNumber.substring(panCardNumber.length - 2);
}

/**
 * Helper function to mask Aadhaar card number
 */
export function maskAadhaarCardNumber(aadhaarCardNumber: string | null): string {
  if (!aadhaarCardNumber) return '-';
  if (aadhaarCardNumber.length < 4) return aadhaarCardNumber;
  return aadhaarCardNumber.substring(0, 4) + 'XXXX' + aadhaarCardNumber.substring(aadhaarCardNumber.length - 4);
}
