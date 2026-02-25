import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { User, UserCreate } from '../models/user.model';
import { SaleOrder, SaleOrderCreate } from '../models/sale-order.model';
import { SaleOrderItem, SaleOrderItemWithCalculation } from '../models/sale-order-item.model';
import { Payment, PaymentCreate } from '../models/payment.model';
import { Invoice } from '../models/invoice.model';
import { UserService } from './user.service';
import { SaleOrderService } from './sale-order.service';
import { SaleOrderItemService } from './sale-order-item.service';
import { PaymentService } from './payment.service';
import { InvoiceService } from './invoice.service';
import { ToastrService } from 'ngx-toastr';

/**
 * Sale Wizard State Interface
 * Manages the complete state of the sale wizard flow
 */
export interface SaleWizardState {
  // Step 1: Customer
  selectedCustomer: User | null;
  isNewCustomer: boolean;
  
  // Step 2: Sale Order
  saleOrder: SaleOrder | null;
  
  // Step 3: Items
  saleOrderItems: SaleOrderItem[];
  orderSubtotal: number;
  orderDiscount: number;
  orderTaxAmount: number;
  orderTotal: number;
  
  // Step 4: Payment
  payments: Payment[];
  totalPaid: number;
  balanceDue: number;
  
  // Step 5: Invoice
  generatedInvoice: Invoice | null;
  
  // Navigation
  currentStep: number;
  completedSteps: number[];
  
  // Loading states
  isLoading: boolean;
  isSubmitting: boolean;
}

/**
 * Initial state for the wizard
 */
const initialState: SaleWizardState = {
  selectedCustomer: null,
  isNewCustomer: false,
  saleOrder: null,
  saleOrderItems: [],
  orderSubtotal: 0,
  orderDiscount: 0,
  orderTaxAmount: 0,
  orderTotal: 0,
  payments: [],
  totalPaid: 0,
  balanceDue: 0,
  generatedInvoice: null,
  currentStep: 1,
  completedSteps: [],
  isLoading: false,
  isSubmitting: false,
};

/**
 * SaleWizardService
 * Manages state and operations for the multi-step sale wizard
 */
@Injectable({
  providedIn: 'root',
})
export class SaleWizardService {
  private stateSubject = new BehaviorSubject<SaleWizardState>(initialState);
  public state$ = this.stateSubject.asObservable();

  // Customer role ID (roleId = 5 for Customer)
  private readonly CUSTOMER_ROLE_ID = 4;

  constructor(
    private userService: UserService,
    private saleOrderService: SaleOrderService,
    private saleOrderItemService: SaleOrderItemService,
    private paymentService: PaymentService,
    private invoiceService: InvoiceService,
    private toastr: ToastrService
  ) {}

  /**
   * Get current state value
   */
  getCurrentState(): SaleWizardState {
    return this.stateSubject.value;
  }

  /**
   * Update state partially
   */
  private updateState(partialState: Partial<SaleWizardState>): void {
    this.stateSubject.next({
      ...this.stateSubject.value,
      ...partialState,
    });
  }

  /**
   * Reset wizard to initial state
   */
  resetWizard(): void {
    this.stateSubject.next(initialState);
  }

  // ==================== STEP 1: Customer Methods ====================

  /**
   * Set selected customer
   */
  setCustomer(customer: User, isNew: boolean = false): void {
    this.updateState({
      selectedCustomer: customer,
      isNewCustomer: isNew,
    });
  }

  /**
   * Search customers by phone or name
   */
  searchCustomers(searchTerm: string): Observable<User[]> {
    return this.userService.getAllUsers();
  }

  /**
   * Create new customer
   */
  createCustomer(customerData: UserCreate): Observable<{ message: string; user?: User }> {
    // Ensure roleId is set to Customer (4)
    const customerWithRole: UserCreate = {
      ...customerData,
      roleId: this.CUSTOMER_ROLE_ID,
      statusId: 1, // Active status
    };
    return this.userService.registerUser(customerWithRole);
  }

  /**
   * Clear selected customer
   */
  clearCustomer(): void {
    this.updateState({
      selectedCustomer: null,
      isNewCustomer: false,
    });
  }

  // ==================== STEP 2: Sale Order Methods ====================

  /**
   * Create sale order
   */
  createSaleOrder(orderData: SaleOrderCreate): Observable<SaleOrder> {
    this.updateState({ isSubmitting: true });
    
    return new Observable<SaleOrder>((observer) => {
      this.saleOrderService.createSaleOrder(orderData).subscribe({
        next: (order) => {
          this.updateState({
            saleOrder: order,
            isSubmitting: false,
          });
          observer.next(order);
          observer.complete();
        },
        error: (error) => {
          this.updateState({ isSubmitting: false });
          observer.error(error);
        },
      });
    });
  }

  /**
   * Update sale order in state
   */
  setSaleOrder(order: SaleOrder): void {
    this.updateState({ saleOrder: order });
  }

  // ==================== STEP 3: Items Methods ====================

  /**
   * Add item to sale order
   */
  addItemToOrder(itemData: SaleOrderItemWithCalculation): Observable<SaleOrderItem> {
    this.updateState({ isSubmitting: true });

    return new Observable<SaleOrderItem>((observer) => {
      this.saleOrderItemService.createSaleOrderItemWithCalculation(itemData).subscribe({
        next: (item) => {
          const currentItems = this.stateSubject.value.saleOrderItems;
          const updatedItems = [...currentItems, item];
          
          this.updateState({
            saleOrderItems: updatedItems,
            isSubmitting: false,
          });
          
          this.recalculateOrderTotal();
          observer.next(item);
          observer.complete();
        },
        error: (error) => {
          this.updateState({ isSubmitting: false });
          observer.error(error);
        },
      });
    });
  }

  /**
   * Remove item from sale order
   */
  removeItemFromOrder(itemId: number): void {
    const currentItems = this.stateSubject.value.saleOrderItems;
    const updatedItems = currentItems.filter(item => item.id !== itemId);
    
    this.updateState({ saleOrderItems: updatedItems });
    this.recalculateOrderTotal();
  }

  /**
   * Load existing items for a sale order
   */
  loadSaleOrderItems(saleOrderId: number): void {
    this.updateState({ isLoading: true });
    
    this.saleOrderItemService.getSaleOrderItemsBySaleOrderId(saleOrderId).subscribe({
      next: (items) => {
        this.updateState({
          saleOrderItems: items,
          isLoading: false,
        });
        this.recalculateOrderTotal();
      },
      error: () => {
        this.updateState({ isLoading: false });
      },
    });
  }

  /**
   * Recalculate order totals
   */
  private recalculateOrderTotal(): void {
    const items = this.stateSubject.value.saleOrderItems;
    
    const subtotal = items.reduce((sum, item) => sum + item.itemSubtotal, 0);
    const discount = items.reduce((sum, item) => sum + item.discountAmount, 0);
    const taxAmount = items.reduce((sum, item) => sum + item.gstAmount, 0);
    const total = items.reduce((sum, item) => sum + item.totalAmount, 0);
    
    this.updateState({
      orderSubtotal: subtotal,
      orderDiscount: discount,
      orderTaxAmount: taxAmount,
      orderTotal: total,
      balanceDue: total - this.stateSubject.value.totalPaid,
    });
  }

  // ==================== STEP 4: Payment Methods ====================

  /**
   * Add payment
   */
  addPayment(paymentData: PaymentCreate): Observable<Payment> {
    this.updateState({ isSubmitting: true });

    return new Observable<Payment>((observer) => {
      this.paymentService.createPayment(paymentData).subscribe({
        next: (payment) => {
          const currentPayments = this.stateSubject.value.payments;
          const updatedPayments = [...currentPayments, payment];
          const totalPaid = updatedPayments.reduce((sum, p) => sum + p.amount, 0);
          const orderTotal = this.stateSubject.value.orderTotal;
          
          this.updateState({
            payments: updatedPayments,
            totalPaid: totalPaid,
            balanceDue: orderTotal - totalPaid,
            isSubmitting: false,
          });
          
          observer.next(payment);
          observer.complete();
        },
        error: (error) => {
          this.updateState({ isSubmitting: false });
          observer.error(error);
        },
      });
    });
  }

  /**
   * Remove payment
   */
  removePayment(paymentId: number): void {
    const currentPayments = this.stateSubject.value.payments;
    const updatedPayments = currentPayments.filter(p => p.id !== paymentId);
    const totalPaid = updatedPayments.reduce((sum, p) => sum + p.amount, 0);
    const orderTotal = this.stateSubject.value.orderTotal;
    
    this.updateState({
      payments: updatedPayments,
      totalPaid: totalPaid,
      balanceDue: orderTotal - totalPaid,
    });
  }

  // ==================== STEP 5: Invoice Methods ====================

  /**
   * Generate invoice from sale order
   */
  generateInvoice(saleOrderId: number, notes?: string): Observable<Invoice> {
    this.updateState({ isSubmitting: true });

    return new Observable<Invoice>((observer) => {
      this.invoiceService.generateInvoice({
        saleOrderId,
        notes,
        includeTermsAndConditions: true,
      }).subscribe({
        next: (invoice) => {
          this.updateState({
            generatedInvoice: invoice,
            isSubmitting: false,
          });
          observer.next(invoice);
          observer.complete();
        },
        error: (error) => {
          this.updateState({ isSubmitting: false });
          observer.error(error);
        },
      });
    });
  }

  // ==================== Navigation Methods ====================

  /**
   * Go to next step
   */
  nextStep(): void {
    const current = this.stateSubject.value.currentStep;
    const completed = this.stateSubject.value.completedSteps;
    
    if (current < 5) {
      this.updateState({
        currentStep: current + 1,
        completedSteps: [...completed, current],
      });
    }
  }

  /**
   * Go to previous step
   */
  previousStep(): void {
    const current = this.stateSubject.value.currentStep;
    
    if (current > 1) {
      this.updateState({ currentStep: current - 1 });
    }
  }

  /**
   * Go to specific step
   */
  goToStep(step: number): void {
    const current = this.stateSubject.value.currentStep;
    const completed = this.stateSubject.value.completedSteps;
    
    if (step >= 1 && step <= 5) {
      // Can only go to completed steps or current + 1
      if (completed.includes(step) || step === current || step === current + 1) {
        this.updateState({ currentStep: step });
      }
    }
  }

  /**
   * Check if current step is valid to proceed
   */
  canProceed(): boolean {
    const state = this.stateSubject.value;
    
    switch (state.currentStep) {
      case 1:
        return state.selectedCustomer !== null;
      case 2:
        return state.saleOrder !== null;
      case 3:
        return state.saleOrderItems.length > 0;
      case 4:
        return state.totalPaid >= state.orderTotal * 0.5; // At least 50% paid
      case 5:
        return state.generatedInvoice !== null;
      default:
        return false;
    }
  }

  /**
   * Get step validation status
   */
  isStepComplete(step: number): boolean {
    const state = this.stateSubject.value;
    
    switch (step) {
      case 1:
        return state.selectedCustomer !== null;
      case 2:
        return state.saleOrder !== null;
      case 3:
        return state.saleOrderItems.length > 0;
      case 4:
        return state.totalPaid >= state.orderTotal;
      case 5:
        return state.generatedInvoice !== null;
      default:
        return false;
    }
  }

  /**
   * Set loading state
   */
  setLoading(isLoading: boolean): void {
    this.updateState({ isLoading });
  }

  /**
   * Set submitting state
   */
  setSubmitting(isSubmitting: boolean): void {
    this.updateState({ isSubmitting });
  }
}
