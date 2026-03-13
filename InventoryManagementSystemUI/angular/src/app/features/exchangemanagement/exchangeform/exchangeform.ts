// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, FormArray, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { ExchangeService } from 'src/app/core/services/exchange.service';
import { UserService } from 'src/app/core/services/user.service';
import {
  ExchangeOrder,
  ExchangeOrderCreate,
  ExchangeCalculateRequest,
  ExchangeCalculateResponse,
  ExchangeItemInput,
  ExchangeType,
  getExchangeTypeLabel,
  getStatusLabel,
  getStatusClass,
  formatDate,
  formatCurrency,
  formatWeight,
} from 'src/app/core/models/exchange.model';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/core/models/user.model';
import { Metal } from 'src/app/core/models/metal.model';
import { Purity } from 'src/app/core/models/purity.model';
import { MetalService } from 'src/app/core/services/metal.service';
import { PurityService } from 'src/app/core/services/purity.service';

/**
 * Exchange Form Component
 * Handles both create and view operations for exchange orders
 */
@Component({
  selector: 'app-exchangeform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './exchangeform.html',
  styleUrl: './exchangeform.scss',
})
export class Exchangeform implements OnInit {
  private readonly exchangeOnlyType = ExchangeType.EXCHANGE;

  // Injected services
  private fb = inject(FormBuilder);
  private exchangeService = inject(ExchangeService);
  private userService = inject(UserService);
  private metalService = inject(MetalService);
  private purityService = inject(PurityService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  exchangeForm!: UntypedFormGroup;
  isViewMode: boolean = false;
  isEditMode: boolean = false;
  exchangeOrderId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  isCalculating: boolean = false;
  calculatedResponse: ExchangeCalculateResponse | null = null;
  exchangeOrder: ExchangeOrder | null = null;

  // Exchange type options
  exchangeTypeOptions = [
    { id: ExchangeType.EXCHANGE, name: 'Exchange (For New Purchase)' },
  ];

  // Customer options for dropdown
  customers: User[] = [];
  filteredCustomers: User[] = [];
  customerSearchTerm: string = '';
  customerDropdownOpen: boolean = false;

  // Metal and Purity options
  metals: Metal[] = [];
  purities: Purity[] = [];

  // Track selected metal for each item
  selectedMetalIds: Map<number, number> = new Map();

  // Form validation messages
  validationMessages = {
    customerId: {
      required: 'Customer is required',
    },
    exchangeType: {
      required: 'Exchange type is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.checkMode();
    this.loadCustomers();
    this.loadMetalsAndPurities();
  }

  /**
   * Load metals and purities
   */
  loadMetalsAndPurities(): void {
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals;
      },
      error: () => {
        this.toastr.error('Failed to load metals');
      },
    });

    this.purityService.getAllPurities().subscribe({
      next: (purities) => {
        this.purities = purities;
      },
      error: () => {
        this.toastr.error('Failed to load purities');
      },
    });
  }

  /**
   * Get purities for a specific metal
   */
  getPuritiesForMetal(metalId: number | null | undefined): Purity[] {
    if (!metalId) return [];
    return this.purities.filter(p => p.metalId === metalId);
  }

  /**
   * Check if purity dropdown should be enabled for an item
   */
  isPurityEnabled(itemIndex: number): boolean {
    const metalId = this.selectedMetalIds.get(itemIndex);
    return metalId !== undefined && metalId !== null;
  }

  /**
   * Handle metal selection change for an item
   */
  onMetalChange(itemIndex: number): void {
    // Get the metalId from the form control (which has the correct numeric value)
    const item = this.itemsArray.at(itemIndex);
    const metalId = item.get('metalId')?.value;
    
    this.selectedMetalIds.set(itemIndex, metalId);
    
    // Reset purity when metal changes
    item.patchValue({ purityId: null });
  }

  /**
   * Get metal name by ID
   */
  getMetalName(metalId: number): string {
    const metal = this.metals.find(m => m.id === metalId);
    return metal ? metal.name : '';
  }

  /**
   * Get purity name by ID
   */
  getPurityName(purityId: number): string {
    const purity = this.purities.find(p => p.id === purityId);
    return purity ? purity.name : '';
  }

  /**
   * Load customers (users with roleId = 4) for the dropdown
   */
  loadCustomers(): void {
    this.userService.getCustomers().subscribe({
      next: (customers) => {
        this.customers = customers;
        this.filteredCustomers = customers;
        // If editing, set the customer name for display
        const customerId = this.exchangeForm?.get('customerId')?.value;
        if (customerId) {
          const customer = this.customers.find(c => c.id === customerId);
          if (customer) {
            this.customerSearchTerm = customer.name;
          }
        }
      },
      error: () => {
        this.toastr.error('Failed to load customers');
      },
    });
  }

  /**
   * Filter customers based on search term
   */
  filterCustomers(): void {
    const searchTerm = this.customerSearchTerm.toLowerCase().trim();
    if (!searchTerm) {
      this.filteredCustomers = this.customers;
    } else {
      this.filteredCustomers = this.customers.filter(
        (customer) =>
          customer.name.toLowerCase().includes(searchTerm) ||
          customer.contactNumber.includes(searchTerm) ||
          customer.email?.toLowerCase().includes(searchTerm)
      );
    }
  }

  /**
   * Toggle customer dropdown
   */
  toggleCustomerDropdown(): void {
    this.customerDropdownOpen = !this.customerDropdownOpen;
    if (this.customerDropdownOpen) {
      this.filteredCustomers = this.customers;
    }
  }

  /**
   * Close customer dropdown
   */
  closeCustomerDropdown(): void {
    this.customerDropdownOpen = false;
  }

  /**
   * Select a customer from the dropdown
   */
  selectCustomer(customer: User): void {
    this.exchangeForm.patchValue({ customerId: customer.id });
    this.customerSearchTerm = customer.name;
    this.closeCustomerDropdown();
  }

  /**
   * Get the selected customer name for display
   */
  getSelectedCustomerName(): string {
    const customerId = this.exchangeForm.get('customerId')?.value;
    if (!customerId) return '';
    const customer = this.customers.find((c) => c.id === customerId);
    return customer ? customer.name : '';
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.exchangeForm = this.fb.group({
      customerId: [null, [Validators.required]],
      exchangeType: [{ value: this.exchangeOnlyType, disabled: true }, [Validators.required]],
      notes: [''],
      items: this.fb.array([this.createItemFormGroup()]),
    });
  }

  /**
   * Create a form group for an exchange item
   */
  createItemFormGroup(): UntypedFormGroup {
    return this.fb.group({
      metalId: [null, [Validators.required]],
      purityId: [null, [Validators.required]],
      grossWeight: [0, [Validators.required, Validators.min(0)]],
      netWeight: [0, [Validators.required, Validators.min(0)]],
      makingChargeDeductionPercent: [0, [Validators.min(0), Validators.max(100)]],
      wastageDeductionPercent: [0, [Validators.min(0), Validators.max(100)]],
      itemDescription: [''],
    });
  }

  /**
   * Get the items form array
   */
  get itemsArray(): FormArray {
    return this.exchangeForm.get('items') as FormArray;
  }

  /**
   * Add a new item to the form
   */
  addItem(): void {
    this.itemsArray.push(this.createItemFormGroup());
  }

  /**
   * Remove an item from the form
   */
  removeItem(index: number): void {
    if (this.itemsArray.length > 1) {
      this.itemsArray.removeAt(index);
    } else {
      this.toastr.warning('At least one item is required');
    }
  }

  /**
   * Check if we're in view mode and load exchange order data
   */
  checkMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.exchangeOrderId = +id;
      const urlSegments = this.route.snapshot.url;
      const isEditRoute = urlSegments.some((segment) => segment.path === 'edit');

      this.isViewMode = isEditRoute || urlSegments.some((segment) => segment.path === 'view');
      this.isEditMode = false;

      if (isEditRoute) {
        this.toastr.info('Exchange orders can be viewed but not edited with the current backend service');
      }

      this.loadExchangeOrderData(this.exchangeOrderId);
    }
  }

  /**
   * Load exchange order data for viewing/editing
   */
  loadExchangeOrderData(id: number): void {
    this.isLoading = true;
    this.exchangeService.getExchangeOrderById(id).subscribe({
      next: (order) => {
        if (order) {
          this.exchangeOrder = order;
          // Clear existing items and populate with order items
          while (this.itemsArray.length) {
            this.itemsArray.removeAt(0);
          }
          
          // Populate form with order data
          this.exchangeForm.patchValue({
            customerId: order.customerId,
            exchangeType: this.mapExchangeType(order.exchangeType),
            notes: order.notes,
          });

          // Set customer name for display
          if (order.customerId) {
            const customer = this.customers.find(c => c.id === order.customerId);
            if (customer) {
              this.customerSearchTerm = customer.name;
            }
          }

          // Add items
          if (order.items && order.items.length > 0) {
            order.items.forEach((item, index) => {
              this.itemsArray.push(
                this.fb.group({
                  metalId: [item.metalId, [Validators.required]],
                  purityId: [item.purityId, [Validators.required]],
                  grossWeight: [item.grossWeight, [Validators.required, Validators.min(0)]],
                  netWeight: [item.netWeight, [Validators.required, Validators.min(0)]],
                  makingChargeDeductionPercent: [item.makingChargeDeductionPercent, [Validators.min(0), Validators.max(100)]],
                  wastageDeductionPercent: [item.wastageDeductionPercent, [Validators.min(0), Validators.max(100)]],
                  itemDescription: [item.itemDescription || ''],
                })
              );
              // Track the metal selection for this item
              this.selectedMetalIds.set(this.itemsArray.length - 1, item.metalId);
            });
          } else {
            this.itemsArray.push(this.createItemFormGroup());
          }

          // Disable form in view mode
          if (this.isViewMode) {
            this.exchangeForm.disable();
          }
        } else {
          this.toastr.error('Exchange order not found');
          this.router.navigate(['jewelleryManagement/admin/exchange']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/exchange']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.exchangeForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.exchangeForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Check if the form has minimum required data for enabling buttons
   * This is a custom validation check that is more flexible than form.invalid
   */
  get isFormValid(): boolean {
    const customerId = this.exchangeForm.get('customerId');
    const exchangeType = this.exchangeForm.get('exchangeType');
    
    // Check main form fields are filled
    const mainFieldsValid = customerId?.value && exchangeType?.value;
    
    // Check at least one item has the required fields (metalId and purityId)
    let hasValidItem = false;
    for (let i = 0; i < this.itemsArray.length; i++) {
      const item = this.itemsArray.at(i);
      const metalId = item.get('metalId')?.value;
      const purityId = item.get('purityId')?.value;
      const grossWeight = item.get('grossWeight')?.value;
      const netWeight = item.get('netWeight')?.value;
      
      if (metalId && purityId && grossWeight && netWeight) {
        hasValidItem = true;
        break;
      }
    }
    
    return mainFieldsValid && hasValidItem;
  }

  /**
   * Check if an item field has errors
   */
  hasItemError(index: number, fieldName: string): boolean {
    const item = this.itemsArray.at(index);
    const field = item.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.exchangeForm.get(fieldName);
    if (field && field.errors) {
      const errorKey = Object.keys(field.errors)[0];
      const messages = this.validationMessages[fieldName as keyof typeof this.validationMessages];
      if (messages && typeof messages === 'object') {
        return (messages as Record<string, string>)[errorKey] || 'Invalid value';
      }
    }
    return '';
  }

  /**
   * Calculate exchange value
   */
  onCalculate(): void {
    if (this.exchangeForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isCalculating = true;
    const request = this.buildExchangeRequest();

    this.exchangeService.calculateExchangeValue(request).subscribe({
      next: (response) => {
        this.calculatedResponse = response;
        this.isCalculating = false;
        this.toastr.success('Exchange value calculated successfully');
      },
      error: () => {
        this.isCalculating = false;
      },
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.exchangeForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const createData = this.buildExchangeRequest();

    this.exchangeService.createExchangeOrder(createData).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.router.navigate(['jewelleryManagement/admin/exchange']);
      },
      error: () => {
        this.isSubmitting = false;
      },
    });
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.exchangeForm.controls).forEach((key) => {
      if (key === 'items') {
        this.itemsArray.controls.forEach((item) => {
          Object.keys((item as UntypedFormGroup).controls).forEach((itemKey) => {
            item.get(itemKey)?.markAsTouched();
          });
        });
      } else {
        this.exchangeForm.get(key)?.markAsTouched();
      }
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/exchange']);
  }

  /**
   * Get exchange type label
   */
  getExchangeTypeLabel(type: ExchangeType): string {
    return getExchangeTypeLabel(type);
  }

  /**
   * Get status label
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }

  private buildExchangeRequest(): ExchangeOrderCreate & ExchangeCalculateRequest {
    const formValue = this.exchangeForm.getRawValue();

    return {
      customerId: Number(formValue.customerId),
      exchangeType: this.exchangeOnlyType,
      items: formValue.items.map((item: any) => this.mapExchangeItem(item)),
      notes: this.toOptionalText(formValue.notes),
    };
  }

  private mapExchangeItem(item: any): ExchangeItemInput {
    return {
      metalId: Number(item.metalId),
      purityId: Number(item.purityId),
      grossWeight: Number(item.grossWeight),
      netWeight: Number(item.netWeight),
      makingChargeDeductionPercent: this.toNumber(item.makingChargeDeductionPercent),
      wastageDeductionPercent: this.toNumber(item.wastageDeductionPercent),
      itemDescription: this.toOptionalText(item.itemDescription),
    };
  }

  private mapExchangeType(exchangeType: string): ExchangeType {
    return exchangeType?.toUpperCase() === 'BUYBACK' ? ExchangeType.BUYBACK : this.exchangeOnlyType;
  }

  private toNumber(value: unknown): number {
    return value === null || value === undefined || value === '' ? 0 : Number(value);
  }

  private toOptionalText(value: unknown): string | null {
    if (typeof value !== 'string') {
      return null;
    }

    const trimmedValue = value.trim();
    return trimmedValue ? trimmedValue : null;
  }

  /**
   * Calculate total deduction percentage for an item
   */
  getTotalDeductionPercent(item: any): number {
    const makingCharge = item.makingChargeDeductionPercent || 0;
    const wastage = item.wastageDeductionPercent || 0;
    return makingCharge + wastage;
  }

  /**
   * Format date for display
   */
  formatDate(date: Date | string | null): string {
    return formatDate(date);
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number | null | undefined): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number | null | undefined): string {
    return formatWeight(weight);
  }
}
