/**
 * Stone Rate History Model
 * Interfaces for Stone Rate History management based on backend DTOs
 */

/**
 * Main Stone Rate History interface matching StoneRateDto from backend
 */
export interface StoneRateHistory {
  id: number;
  stoneId: number;
  stoneName?: string;
  stoneUnit?: string;
  carat: number;
  cut?: string;
  color?: string;
  clarity?: string;
  grade?: string;
  ratePerUnit: number;
  effectiveDate: Date | string;
  createdDate: Date | string;
  statusId: number;
}

/**
 * Interface for creating a new stone rate entry
 */
export interface StoneRateHistoryCreate {
  stoneId: number;
  carat: number;
  cut?: string;
  color?: string;
  clarity?: string;
  grade?: string;
  ratePerUnit: number;
  effectiveDate: Date | string;
}

/**
 * Interface for updating an existing stone rate entry
 */
export interface StoneRateHistoryUpdate {
  id: number;
  carat: number;
  cut?: string;
  color?: string;
  clarity?: string;
  grade?: string;
  ratePerUnit: number;
  effectiveDate: Date | string;
}

/**
 * Interface for searching stone rates
 */
export interface StoneRateSearch {
  stoneId?: number;
  carat?: number;
  cut?: string;
  color?: string;
  clarity?: string;
  grade?: string;
}

/**
 * Interface for diamond 4Cs rate
 */
export interface Diamond4CsRate {
  carat: number;
  cut: string;
  color: string;
  clarity: string;
  ratePerCarat: number;
}

/**
 * Interface for stone rate response
 */
export interface StoneRateResponse {
  stoneId: number;
  stoneName: string;
  stoneUnit: string;
  carat: number;
  cut?: string;
  color?: string;
  clarity?: string;
  grade?: string;
  currentRatePerUnit: number;
  effectiveDate: Date | string;
  lastUpdated?: Date | string;
}

/**
 * Options for Cut quality (for diamonds)
 */
export const CUT_OPTIONS: string[] = [
  'Excellent',
  'Very Good',
  'Good',
  'Fair'
];

/**
 * Options for Color grade (for diamonds)
 */
export const COLOR_OPTIONS: string[] = [
  'D',
  'E',
  'F',
  'G',
  'H',
  'I',
  'J'
];

/**
 * Options for Clarity grade (for diamonds)
 */
export const CLARITY_OPTIONS: string[] = [
  'IF',
  'VVS1',
  'VVS2',
  'VS1',
  'VS2',
  'SI1',
  'SI2'
];

/**
 * Options for Grade (for colored stones)
 */
export const GRADE_OPTIONS: string[] = [
  'AAA',
  'AA',
  'A',
  'Lab-Grown'
];
