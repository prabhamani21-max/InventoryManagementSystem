import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  SaleOrderCreatedNotification,
  SaleOrderStatusChangedNotification,
  SaleOrderDeletedNotification,
  DeliveryDateUpdatedNotification,
  CustomerNotification,
} from '../models/sale-order-notification.model';

/**
 * Sale Order Notification Service
 * Handles SignalR connection for real-time sale order notifications
 */
@Injectable({
  providedIn: 'root',
})
export class SaleOrderNotificationService implements OnDestroy {
  private hubConnection: HubConnection | null = null;
  private connectionId: string | null = null;

  // Subjects for different notification types
  private saleOrderCreatedSubject = new Subject<SaleOrderCreatedNotification>();
  private saleOrderStatusChangedSubject = new Subject<SaleOrderStatusChangedNotification>();
  private saleOrderDeletedSubject = new Subject<SaleOrderDeletedNotification>();
  private deliveryDateUpdatedSubject = new Subject<DeliveryDateUpdatedNotification>();
  private customerNotificationSubject = new Subject<CustomerNotification>();
  private connectionStateChangedSubject = new Subject<boolean>();

  // Observables for components to subscribe to
  public saleOrderCreated$ = this.saleOrderCreatedSubject.asObservable();
  public saleOrderStatusChanged$ = this.saleOrderStatusChangedSubject.asObservable();
  public saleOrderDeleted$ = this.saleOrderDeletedSubject.asObservable();
  public deliveryDateUpdated$ = this.deliveryDateUpdatedSubject.asObservable();
  public customerNotification$ = this.customerNotificationSubject.asObservable();
  public connectionStateChanged$ = this.connectionStateChangedSubject.asObservable();

  private readonly hubUrl = environment.saleOrderHubUrl || environment.apiUrl.replace('/api', '/hubs/saleorder');

  /**
   * Start the SignalR connection
   */
  startConnection(): Promise<void> {
    if (this.hubConnection) {
      return Promise.resolve();
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        withCredentials: true,
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
          }
          return null; // Stop retrying after 60 seconds
        },
      })
      .build();

    this.registerEventHandlers();

    return this.hubConnection
      .start()
      .then(() => {
        this.connectionId = this.hubConnection?.connectionId || null;
        this.connectionStateChangedSubject.next(true);
        console.log('SignalR SaleOrderHub connected:', this.connectionId);
      })
      .catch((error) => {
        console.error('Error starting SaleOrderHub SignalR connection:', error);
        this.connectionStateChangedSubject.next(false);
        throw error;
      });
  }

  /**
   * Stop the SignalR connection
   */
  stopConnection(): Promise<void> {
    if (!this.hubConnection) {
      return Promise.resolve();
    }

    return this.hubConnection
      .stop()
      .then(() => {
        this.connectionId = null;
        this.hubConnection = null;
        this.connectionStateChangedSubject.next(false);
        console.log('SignalR SaleOrderHub disconnected');
      })
      .catch((error) => {
        console.error('Error stopping SaleOrderHub SignalR connection:', error);
        throw error;
      });
  }

  /**
   * Register SignalR event handlers
   */
  private registerEventHandlers(): void {
    if (!this.hubConnection) return;

    // Handle sale order created event
    this.hubConnection.on('SaleOrderCreated', (notification: SaleOrderCreatedNotification) => {
      console.log('Sale order created notification received:', notification);
      this.saleOrderCreatedSubject.next(notification);
    });

    // Handle sale order status changed event
    this.hubConnection.on('SaleOrderStatusChanged', (notification: SaleOrderStatusChangedNotification) => {
      console.log('Sale order status changed notification received:', notification);
      this.saleOrderStatusChangedSubject.next(notification);
    });

    // Handle sale order deleted event
    this.hubConnection.on('SaleOrderDeleted', (notification: SaleOrderDeletedNotification) => {
      console.log('Sale order deleted notification received:', notification);
      this.saleOrderDeletedSubject.next(notification);
    });

    // Handle delivery date updated event
    this.hubConnection.on('DeliveryDateUpdated', (notification: DeliveryDateUpdatedNotification) => {
      console.log('Delivery date updated notification received:', notification);
      this.deliveryDateUpdatedSubject.next(notification);
    });

    // Handle customer notifications
    this.hubConnection.on('ReceiveNotification', (notification: CustomerNotification) => {
      console.log('Customer notification received:', notification);
      this.customerNotificationSubject.next(notification);
    });

    // Handle reconnection
    this.hubConnection.onreconnecting((error) => {
      console.log('SaleOrderHub SignalR reconnecting:', error);
      this.connectionStateChangedSubject.next(false);
    });

    this.hubConnection.onreconnected((connectionId) => {
      this.connectionId = connectionId || null;
      this.connectionStateChangedSubject.next(true);
      console.log('SaleOrderHub SignalR reconnected:', connectionId);
    });

    this.hubConnection.onclose((error) => {
      console.log('SaleOrderHub SignalR connection closed:', error);
      this.connectionId = null;
      this.connectionStateChangedSubject.next(false);
    });
  }

  /**
   * Join a specific sale order group for targeted notifications
   * @param saleOrderId The sale order ID to join the group for
   */
  joinSaleOrderGroup(saleOrderId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('JoinSaleOrderGroup', saleOrderId);
  }

  /**
   * Leave a specific sale order group
   * @param saleOrderId The sale order ID to leave the group for
   */
  leaveSaleOrderGroup(saleOrderId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('LeaveSaleOrderGroup', saleOrderId);
  }

  /**
   * Join a customer-specific group for all their sale order notifications
   * @param customerId The customer ID to join the group for
   */
  joinCustomerGroup(customerId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('JoinCustomerGroup', customerId);
  }

  /**
   * Leave a customer-specific group
   * @param customerId The customer ID to leave the group for
   */
  leaveCustomerGroup(customerId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('LeaveCustomerGroup', customerId);
  }

  /**
   * Join the sales team group for all sale order notifications
   */
  joinSalesTeamGroup(): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('JoinSalesTeamGroup');
  }

  /**
   * Leave the sales team group
   */
  leaveSalesTeamGroup(): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('LeaveSalesTeamGroup');
  }

  /**
   * Check if the connection is active
   */
  isConnected(): boolean {
    return this.hubConnection?.state === 'Connected';
  }

  /**
   * Get the current connection ID
   */
  getConnectionId(): string | null {
    return this.connectionId;
  }

  /**
   * Cleanup on service destruction
   */
  ngOnDestroy(): void {
    this.stopConnection();
    this.saleOrderCreatedSubject.complete();
    this.saleOrderStatusChangedSubject.complete();
    this.saleOrderDeletedSubject.complete();
    this.deliveryDateUpdatedSubject.complete();
    this.customerNotificationSubject.complete();
    this.connectionStateChangedSubject.complete();
  }
}
