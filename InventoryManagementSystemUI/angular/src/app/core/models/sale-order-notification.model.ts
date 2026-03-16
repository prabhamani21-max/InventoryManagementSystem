/**
 * Interface for sale order created notification
 */
export interface SaleOrderCreatedNotification {
  Type: string;
  SaleOrderId: number;
  OrderNumber: string;
  CustomerId: number;
  OrderDate: string;
  DeliveryDate?: string;
  IsExchangeSale: boolean;
  Timestamp: string;
}

/**
 * Interface for sale order status changed notification
 */
export interface SaleOrderStatusChangedNotification {
  Type: string;
  SaleOrderId: number;
  Status: string;
  Timestamp: string;
}

/**
 * Interface for sale order deleted notification
 */
export interface SaleOrderDeletedNotification {
  Type: string;
  SaleOrderId: number;
  Timestamp: string;
}

/**
 * Interface for delivery date updated notification
 */
export interface DeliveryDateUpdatedNotification {
  Type: string;
  SaleOrderId: number;
  DeliveryDate?: string;
  Timestamp: string;
}

/**
 * Interface for customer notification
 */
export interface CustomerNotification {
  Type: string;
  CustomerId: number;
  Message: string;
  Timestamp: string;
}

/**
 * Union type for all sale order notification types
 */
export type SaleOrderNotification =
  | SaleOrderCreatedNotification
  | SaleOrderStatusChangedNotification
  | SaleOrderDeletedNotification
  | DeliveryDateUpdatedNotification
  | CustomerNotification;
