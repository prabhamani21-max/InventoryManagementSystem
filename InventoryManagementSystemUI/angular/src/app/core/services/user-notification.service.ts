import { Injectable, inject, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User } from '../models/user.model';

/**
 * Interface for user registration notification
 */
export interface UserRegisteredNotification {
  Type: string;
  UserId: number;
  UserName: string;
  Email: string;
  Timestamp: string;
}

/**
 * Interface for general notification
 */
export interface UserNotification {
  Type: string;
  UserId?: number;
  Message: string;
  Timestamp: string;
}

/**
 * Interface for user status change notification
 */
export interface UserStatusChangeNotification {
  Type: string;
  UserId: number;
  Status: string;
  Timestamp: string;
}

/**
 * User Notification Service
 * Handles SignalR connection for real-time user notifications
 */
@Injectable({
  providedIn: 'root',
})
export class UserNotificationService implements OnDestroy {
  private hubConnection: HubConnection | null = null;
  private connectionId: string | null = null;

  // Subjects for different notification types
  private userRegisteredSubject = new Subject<UserRegisteredNotification>();
  private notificationSubject = new Subject<UserNotification>();
  private userStatusChangedSubject = new Subject<UserStatusChangeNotification>();
  private connectionStateChangedSubject = new Subject<boolean>();

  // Observables for components to subscribe to
  public userRegistered$ = this.userRegisteredSubject.asObservable();
  public notification$ = this.notificationSubject.asObservable();
  public userStatusChanged$ = this.userStatusChangedSubject.asObservable();
  public connectionStateChanged$ = this.connectionStateChangedSubject.asObservable();

  private readonly hubUrl = environment.signalRHubUrl || environment.apiUrl.replace('/api', '/hubs/user');

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
        console.log('SignalR UserHub connected:', this.connectionId);
      })
      .catch((error) => {
        console.error('Error starting SignalR connection:', error);
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
        console.log('SignalR UserHub disconnected');
      })
      .catch((error) => {
        console.error('Error stopping SignalR connection:', error);
        throw error;
      });
  }

  /**
   * Register SignalR event handlers
   */
  private registerEventHandlers(): void {
    if (!this.hubConnection) return;

    // Handle user registered event
    this.hubConnection.on('UserRegistered', (notification: UserRegisteredNotification) => {
      console.log('User registered notification received:', notification);
      this.userRegisteredSubject.next(notification);
    });

    // Handle general notifications
    this.hubConnection.on('ReceiveNotification', (notification: UserNotification) => {
      console.log('Notification received:', notification);
      this.notificationSubject.next(notification);
    });

    // Handle user status change
    this.hubConnection.on('UserStatusChanged', (notification: UserStatusChangeNotification) => {
      console.log('User status changed notification received:', notification);
      this.userStatusChangedSubject.next(notification);
    });

    // Handle reconnection
    this.hubConnection.onreconnecting((error) => {
      console.log('SignalR reconnecting:', error);
      this.connectionStateChangedSubject.next(false);
    });

    this.hubConnection.onreconnected((connectionId) => {
      this.connectionId = connectionId || null;
      this.connectionStateChangedSubject.next(true);
      console.log('SignalR reconnected:', connectionId);
    });

    this.hubConnection.onclose((error) => {
      console.log('SignalR connection closed:', error);
      this.connectionId = null;
      this.connectionStateChangedSubject.next(false);
    });
  }

  /**
   * Join a user-specific group for targeted notifications
   * @param userId The user ID to join the group for
   */
  joinUserGroup(userId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('JoinUserGroup', userId);
  }

  /**
   * Leave a user-specific group
   * @param userId The user ID to leave the group for
   */
  leaveUserGroup(userId: number): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('LeaveUserGroup', userId);
  }

  /**
   * Join a role-based group for role-specific notifications
   * @param roleName The role name to join the group for
   */
  joinRoleGroup(roleName: string): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('JoinRoleGroup', roleName);
  }

  /**
   * Leave a role-based group
   * @param roleName The role name to leave the group for
   */
  leaveRoleGroup(roleName: string): Promise<void> {
    if (!this.hubConnection) {
      return Promise.reject('SignalR connection not established');
    }
    return this.hubConnection.invoke('LeaveRoleGroup', roleName);
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
    this.userRegisteredSubject.complete();
    this.notificationSubject.complete();
    this.userStatusChangedSubject.complete();
    this.connectionStateChangedSubject.complete();
  }
}
