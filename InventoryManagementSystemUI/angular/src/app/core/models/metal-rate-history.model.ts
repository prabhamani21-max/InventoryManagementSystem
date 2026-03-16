/**
 * Metal Rate History Model
 * Interfaces for Metal Rate History management based on backend DTOs
 */

/**
 * Main Metal Rate History interface matching MetalRateDto from backend
 */
export interface MetalRateHistory {
  id: number;
  purityId: number;
  purityName?: string;
  metalId: number;
  metalName?: string;
  ratePerGram: number;
  effectiveDate: Date | string;
  createdDate: Date | string;
  statusId: number;
}

/**
 * Interface for creating a new metal rate entry
 * Matches MetalRateCreateDto from backend
 */
export interface MetalRateHistoryCreate {
  purityId: number;
  ratePerGram: number;
  effectiveDate: Date | string;
}

/**
 * Interface for updating an existing metal rate entry
 * Matches MetalRateUpdateDto from backend
 */
export interface MetalRateHistoryUpdate {
  id: number;
  ratePerGram: number;
  effectiveDate: Date | string;
}

/**
 * Interface for metal rate response
 * Matches MetalRateResponseDto from backend
 */
export interface MetalRateResponse {
  purityId: number;
  purityName: string;
  metalId: number;
  metalName: string;
  percentage: number;
  currentRatePerGram: number;
  effectiveDate: Date | string;
  lastUpdated?: Date | string;
}

/**
 * Interface for metal rate history entry
 * Matches MetalRateHistoryDto from backend
 */
export interface MetalRateHistoryEntry {
  id: number;
  purityId: number;
  purityName: string;
  ratePerGram: number;
  effectiveDate: Date | string;
  createdDate: Date | string;
}

/**
 * Interface for date range query parameters
 */
export interface DateRangeQuery {
  startDate?: Date | string;
  endDate?: Date | string;
}
