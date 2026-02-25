// src/app/core/constants/claim-types.ts
import { JwtPayload } from 'jwt-decode';

/**
 * JWT Claim Type Constants
 */
export const ClaimTypes = {
  NAME_IDENTIFIER:
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
  NAME: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
  EXPIRATION: 'exp',
} as const;

/**
 * Custom JWT Payload with strongly-typed claims
 */
export interface CustomJwtPayload extends JwtPayload {
  [ClaimTypes.NAME_IDENTIFIER]: string;
  [ClaimTypes.NAME]: string;
  [ClaimTypes.EXPIRATION]: number;
  RoleId: string;
}
