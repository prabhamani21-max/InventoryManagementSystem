// angular import
import { Component, OnInit, inject, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { SaleOrderItemService } from 'src/app/core/services/sale-order-item.service';
import { ItemStockService } from 'src/app/core/services/item-stock.service';
import { JewelleryItemService } from 'src/app/core/services/jewellery-item.service';
import { MetalRateHistoryService } from 'src/app/core/services/metal-rate-history.service';
import { SaleWizardService, SaleWizardState } from 'src/app/core/services/sale-wizard.service';
import {
  SaleOrderItem,
  SaleOrderItemCreate,
  SaleOrderItemUpdate,
  SaleOrderItemWithCalculation,
  MakingChargeType,
  getMakingChargeTypeLabel,
  getStatusLabel,
  formatCurrency,
  formatWeight,
} from 'src/app/core/models/sale-order-item.model';
import { JewelleryItem, JewelleryItemWithDetails } from 'src/app/core/models/jewellery-item.model';
import { ToastrService } from 'ngx-toastr';

/**
 * SaleOrderItem Form Component
 * Handles both create and edit operations for sale order items
 * Supports wizard mode for integration with sale wizard
 */
@Component({
  selector: 'app-saleorderitemform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule, FormsModule],
  templateUrl: './saleorderitemform.html',
  styleUrl: './saleorderitemform.scss',
})
export class Saleorderitemform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private saleOrderItemService = inject(SaleOrderItemService);
  private itemStockService = inject(ItemStockService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);
  private wizardService = inject(SaleWizardService);
  private jewelleryItemService = inject(JewelleryItemService);
  private metalRateHistoryService = inject(MetalRateHistoryService);

  // Wizard mode inputs
  @Input() wizardMode: boolean = false;
  @Input() preselectedSaleOrderId: number | null = null;
  @Output() formSubmitted = new EventEmitter<SaleOrderItem>();
  @Output() cancelClicked = new EventEmitter<void>();

  // Properties
  saleOrderItemForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  saleOrderItemId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;
  useAutoCalculation: boolean = true;
  isCheckingStock: boolean = false;
  stockAvailable: boolean | null = null;
  availableQuantity: number = 0;

  // Existing item data for display
  existingItem: SaleOrderItem | null = null;
  
  // Wizard mode state
  wizardState: SaleWizardState = this.wizardService.getCurrentState();
  jewelleryItems: JewelleryItemWithDetails[] = [];
  filteredItems: JewelleryItemWithDetails[] = [];
  searchQuery: string = '';
  isSearching: boolean = false;
  showAddForm: boolean = false;
  selectedItem: JewelleryItemWithDetails | null = null;

  // Price breakdown for real-time preview
  priceBreakdown = {
    metalAmount: 0,
    makingCharges: 0,
    wastageAmount: 0,
    stoneAmount: 0,
    subtotal: 0,
    discountAmount: 0,
    taxableAmount: 0,
    gstAmount: 0,
    totalAmount: 0
  };

  // Making charge type options
  makingChargeTypeOptions = [
    { id: MakingChargeType.PER_GRAM, name: 'Per Gram' },
    { id: MakingChargeType.PERCENTAGE, name: 'Percentage' },
    { id: MakingChargeType.FIXED, name: 'Fixed' },
  ];

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Pending' },
    { id: 4, name: 'Completed' },
    { id: 5, name: 'Cancelled' },
  ];

  // Form validation messages
  validationMessages = {
    saleOrderId: {
      required: 'Sale Order is required',
    },
    jewelleryItemId: {
      required: 'Jewellery Item is required',
    },
    quantity: {
      required: 'Quantity is required',
      min: 'Quantity must be at least 1',
    },
    gstPercentage: {
      required: 'GST Percentage is required',
      min: 'GST Percentage cannot be negative',
      max: 'GST Percentage cannot exceed 100',
    },
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    
    // Check if wizardMode is set via route data
    this.route.data.subscribe((data) => {
      if (data['wizardMode']) {
        this.wizardMode = true;
      }
    });
    
    if (this.wizardMode) {
      // In wizard mode, load jewellery items for selection
      this.loadJewelleryItems();
      // Automatically fetch the saleOrderId from wizard service state
      const wizardState = this.wizardService.getCurrentState();
      if (wizardState.saleOrder && wizardState.saleOrder.id) {
        this.saleOrderItemForm.patchValue({ saleOrderId: wizardState.saleOrder.id });
        this.preselectedSaleOrderId = wizardState.saleOrder.id;
      } else if (this.preselectedSaleOrderId) {
        // Fallback to preselectedSaleOrderId if provided
        this.saleOrderItemForm.patchValue({ saleOrderId: this.preselectedSaleOrderId });
      }
    } else {
      this.checkEditMode();
    }
    
    this.setupStockValidation();
    this.setupPricePreviewListeners();
  }

  /**
   * Load jewellery items for wizard mode with calculated prices and stock
   */
  loadJewelleryItems(): void {
    this.isSearching = true;
    
    // Load jewellery items, metal rates, and stock info
    this.jewelleryItemService.getAllJewelleryItems().subscribe({
      next: (items) => {
        // Load current metal rates for price calculation
        this.metalRateHistoryService.getAllCurrentRates().subscribe({
          next: (rates) => {
            // Create a map of purityId to rate for quick lookup
            const rateMap = new Map<number, number>();
            rates.forEach(rate => {
              rateMap.set(rate.purityId, rate.currentRatePerGram);
            });
            
            // Transform items to include calculated price and stock
            const itemsWithDetails: JewelleryItemWithDetails[] = items.map(item => ({
              ...item,
              sellingPrice: this.calculateSellingPrice(item, rateMap.get(item.purityId) || 0),
              stockQuantity: 0 // Will be loaded on demand when item is selected
            }));
            
            this.jewelleryItems = itemsWithDetails;
            this.filteredItems = [...itemsWithDetails];
            this.isSearching = false;
          },
          error: () => {
            // If rates fail to load, still show items with 0 price
            const itemsWithDetails: JewelleryItemWithDetails[] = items.map(item => ({
              ...item,
              sellingPrice: 0,
              stockQuantity: 0
            }));
            this.jewelleryItems = itemsWithDetails;
            this.filteredItems = [...itemsWithDetails];
            this.isSearching = false;
            this.toastr.warning('Could not load metal rates, prices may not be accurate');
          }
        });
      },
      error: () => {
        this.isSearching = false;
        this.toastr.error('Failed to load jewellery items');
      },
    });
  }
  
  /**
   * Calculate selling price for a jewellery item
   * Price = (Net Weight × Metal Rate) + Making Charges + Wastage Charges
   */
  private calculateSellingPrice(item: JewelleryItem, metalRatePerGram: number): number {
    // Base metal amount
    const netMetalWeight = item.netMetalWeight || item.grossWeight || 0;
    const metalAmount = netMetalWeight * metalRatePerGram;
    
    // Calculate making charges based on type
    let makingCharges = 0;
    switch (item.makingChargeType) {
      case 1: // PER_GRAM
        makingCharges = netMetalWeight * (item.makingChargeValue || 0);
        break;
      case 2: // PERCENTAGE
        makingCharges = metalAmount * ((item.makingChargeValue || 0) / 100);
        break;
      case 3: // FIXED
        makingCharges = item.makingChargeValue || 0;
        break;
    }
    
    // Calculate wastage charges
    const wastageWeight = netMetalWeight * ((item.wastagePercentage || 0) / 100);
    const wastageAmount = wastageWeight * metalRatePerGram;
    
    // Total selling price
    return metalAmount + makingCharges + wastageAmount;
  }

  /**
   * Calculate price preview with all components including stone amount and GST
   * This provides a real-time breakdown before the user adds the item
   */
  calculatePricePreview(): void {
    if (!this.selectedItem) {
      this.resetPriceBreakdown();
      return;
    }
    
    const item = this.selectedItem;
    const stoneAmount = Number(this.saleOrderItemForm.get('stoneAmount')?.value) || 0;
    const discountAmount = Number(this.saleOrderItemForm.get('discountAmount')?.value) || 0;
    const gstPercentage = Number(this.saleOrderItemForm.get('gstPercentage')?.value) || 0;
    const quantity = Number(this.saleOrderItemForm.get('quantity')?.value) || 1;
    
    // Get net weight
    const netMetalWeight = item.netMetalWeight || item.grossWeight || 0;
    
    // Calculate metal rate from selling price (sellingPrice = metalAmount + makingCharges + wastageAmount)
    // We need to reverse-engineer the metal rate from the selling price
    // For more accuracy, we should fetch the actual metal rate, but for preview we estimate
    const basePrice = item.sellingPrice || 0;
    
    // Calculate making charges based on type to subtract from selling price
    let makingChargesPerUnit = 0;
    switch (item.makingChargeType) {
      case 1: // PER_GRAM
        makingChargesPerUnit = netMetalWeight * (item.makingChargeValue || 0);
        break;
      case 2: // PERCENTAGE - we need metal amount first, so estimate
        makingChargesPerUnit = 0; // Will be recalculated
        break;
      case 3: // FIXED
        makingChargesPerUnit = item.makingChargeValue || 0;
        break;
    }
    
    // Calculate wastage to subtract from selling price
    const wastageWeight = netMetalWeight * ((item.wastagePercentage || 0) / 100);
    
    // Estimate metal rate: (basePrice - makingCharges) / (netMetalWeight + wastageWeight)
    // This is an approximation for preview purposes
    let estimatedMetalRate = 0;
    if (netMetalWeight > 0) {
      if (item.makingChargeType === 2) {
        // For percentage type, we need a different approach
        // Assume metal rate is the dominant factor
        estimatedMetalRate = basePrice / (netMetalWeight * (1 + (item.wastagePercentage || 0) / 100) + netMetalWeight * (item.makingChargeValue || 0) / 100);
      } else {
        estimatedMetalRate = (basePrice - makingChargesPerUnit) / (netMetalWeight + wastageWeight);
      }
    }
    
    // Now calculate all components with the estimated rate
    const metalAmount = netMetalWeight * estimatedMetalRate;
    
    // Recalculate making charges for percentage type
    if (item.makingChargeType === 2) {
      makingChargesPerUnit = metalAmount * ((item.makingChargeValue || 0) / 100);
    }
    
    const wastageAmount = wastageWeight * estimatedMetalRate;
    
    // Calculate subtotal and tax
    const subtotal = metalAmount + makingChargesPerUnit + wastageAmount + stoneAmount;
    const taxableAmount = Math.max(0, subtotal - discountAmount);
    const gstAmount = taxableAmount * (gstPercentage / 100);
    const totalAmount = taxableAmount + gstAmount;
    
    // Apply quantity multiplier
    this.priceBreakdown = {
      metalAmount: metalAmount * quantity,
      makingCharges: makingChargesPerUnit * quantity,
      wastageAmount: wastageAmount * quantity,
      stoneAmount: stoneAmount * quantity,
      subtotal: subtotal * quantity,
      discountAmount: discountAmount * quantity,
      taxableAmount: taxableAmount * quantity,
      gstAmount: gstAmount * quantity,
      totalAmount: totalAmount * quantity
    };
  }

  /**
   * Reset price breakdown to zeros
   */
  private resetPriceBreakdown(): void {
    this.priceBreakdown = {
      metalAmount: 0,
      makingCharges: 0,
      wastageAmount: 0,
      stoneAmount: 0,
      subtotal: 0,
      discountAmount: 0,
      taxableAmount: 0,
      gstAmount: 0,
      totalAmount: 0
    };
  }

  /**
   * Search items in wizard mode
   */
  searchItems(): void {
    if (!this.searchQuery || this.searchQuery.trim() === '') {
      this.filteredItems = [...this.jewelleryItems];
      return;
    }
    
    const query = this.searchQuery.toLowerCase().trim();
    this.filteredItems = this.jewelleryItems.filter(item => 
      item.itemCode?.toLowerCase().includes(query) ||
      item.name?.toLowerCase().includes(query) ||
      item.description?.toLowerCase().includes(query)
    );
  }

  /**
   * Clear search
   */
  clearSearch(): void {
    this.searchQuery = '';
    this.filteredItems = [...this.jewelleryItems];
  }

  /**
   * Show add item form in wizard mode
   */
  showAddItemForm(): void {
    this.showAddForm = true;
    this.saleOrderItemForm.reset({
      quantity: 1,
      discountAmount: 0,
      gstPercentage: 3,
      statusId: 1,
    });
    // Get saleOrderId from wizard state
    const wizardState = this.wizardService.getCurrentState();
    if (wizardState.saleOrder && wizardState.saleOrder.id) {
      this.saleOrderItemForm.patchValue({ saleOrderId: wizardState.saleOrder.id });
    } else if (this.preselectedSaleOrderId) {
      this.saleOrderItemForm.patchValue({ saleOrderId: this.preselectedSaleOrderId });
    }
  }

  /**
   * Hide add item form in wizard mode
   */
  hideAddItemForm(): void {
    this.showAddForm = false;
    this.selectedItem = null;
    this.saleOrderItemForm.reset({
      quantity: 1,
      discountAmount: 0,
      gstPercentage: 3,
      statusId: 1,
    });
    // Get saleOrderId from wizard state
    const wizardState = this.wizardService.getCurrentState();
    if (wizardState.saleOrder && wizardState.saleOrder.id) {
      this.saleOrderItemForm.patchValue({ saleOrderId: wizardState.saleOrder.id });
    } else if (this.preselectedSaleOrderId) {
      this.saleOrderItemForm.patchValue({ saleOrderId: this.preselectedSaleOrderId });
    }
  }

  /**
   * Handle item selection in wizard mode
   */
  onItemSelect(itemId: number): void {
    this.selectedItem = this.jewelleryItems.find(item => item.id === itemId) || null;
    if (this.selectedItem) {
      this.checkStockAvailability();
    }
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.saleOrderItemForm = this.fb.group({
      saleOrderId: [null, [Validators.required]],
      jewelleryItemId: [null, [Validators.required]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      discountAmount: [0, [Validators.min(0)]],
      gstPercentage: [3, [Validators.required, Validators.min(0), Validators.max(100)]],
      stoneAmount: [null, [Validators.min(0)]],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Setup stock validation on form changes
   */
  setupStockValidation(): void {
    // Watch for changes to jewelleryItemId or quantity
    this.saleOrderItemForm.get('jewelleryItemId')?.valueChanges.subscribe(() => {
      this.checkStockAvailability();
    });
    
    this.saleOrderItemForm.get('quantity')?.valueChanges.subscribe(() => {
      this.checkStockAvailability();
    });
  }

  /**
   * Setup price preview listeners for real-time calculation
   */
  setupPricePreviewListeners(): void {
    // Watch for changes to form fields that affect price
    this.saleOrderItemForm.get('stoneAmount')?.valueChanges.subscribe(() => {
      this.calculatePricePreview();
    });
    
    this.saleOrderItemForm.get('discountAmount')?.valueChanges.subscribe(() => {
      this.calculatePricePreview();
    });
    
    this.saleOrderItemForm.get('gstPercentage')?.valueChanges.subscribe(() => {
      this.calculatePricePreview();
    });
    
    this.saleOrderItemForm.get('quantity')?.valueChanges.subscribe(() => {
      this.calculatePricePreview();
    });
  }

  /**
   * Check stock availability for selected item and quantity
   */
  checkStockAvailability(): void {
    const jewelleryItemId = this.saleOrderItemForm.get('jewelleryItemId')?.value;
    const quantity = this.saleOrderItemForm.get('quantity')?.value;

    if (!jewelleryItemId || !quantity || quantity < 1) {
      this.stockAvailable = null;
      this.availableQuantity = 0;
      return;
    }

    this.isCheckingStock = true;
    this.itemStockService.getItemStockByJewelleryItemId(jewelleryItemId).subscribe({
      next: (stock) => {
        if (stock) {
          this.availableQuantity = stock.quantity - stock.reservedQuantity;
          this.stockAvailable = this.availableQuantity >= quantity;
        } else {
          this.availableQuantity = 0;
          this.stockAvailable = false;
        }
        this.isCheckingStock = false;
      },
      error: () => {
        this.availableQuantity = 0;
        this.stockAvailable = false;
        this.isCheckingStock = false;
      },
    });
  }

  /**
   * Check if we're in edit mode and load sale order item data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.saleOrderItemId = +id;
      this.loadSaleOrderItemData(this.saleOrderItemId);
    }
  }

  /**
   * Load sale order item data for editing
   */
  loadSaleOrderItemData(id: number): void {
    this.isLoading = true;
    this.saleOrderItemService.getSaleOrderItemById(id).subscribe({
      next: (item) => {
        if (item) {
          this.existingItem = item;
          this.saleOrderItemForm.patchValue({
            saleOrderId: item.saleOrderId,
            jewelleryItemId: item.jewelleryItemId,
            quantity: item.quantity,
            discountAmount: item.discountAmount,
            gstPercentage: item.gstPercentage,
            stoneAmount: item.stoneAmount,
            statusId: item.statusId,
          });
        } else {
          this.toastr.error('Sale order item not found');
          this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.saleOrderItemForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.saleOrderItemForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.saleOrderItemForm.get(fieldName);
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
   * Handle form submission
   */
  onSubmit(): void {
    if (this.saleOrderItemForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    // Check stock availability before submitting
    if (!this.isEditMode && this.stockAvailable === false) {
      this.toastr.error('Insufficient stock available for the selected item');
      return;
    }

    this.isSubmitting = true;
    const formValue = this.saleOrderItemForm.value;

    if (this.wizardMode) {
      // In wizard mode, add item via wizard service
      const itemData: SaleOrderItemWithCalculation = {
        saleOrderId: formValue.saleOrderId,
        jewelleryItemId: formValue.jewelleryItemId,
        quantity: formValue.quantity,
        discountAmount: formValue.discountAmount || 0,
        gstPercentage: formValue.gstPercentage,
        stoneAmount: formValue.stoneAmount,
      };

      this.wizardService.addItemToOrder(itemData).subscribe({
        next: (item) => {
          this.isSubmitting = false;
          this.toastr.success('Item added to order');
          // Update wizard state reference
          this.wizardState = this.wizardService.getCurrentState();
          // Reset form for adding more items
          this.resetItemForm();
          this.formSubmitted.emit(item);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else if (this.isEditMode && this.saleOrderItemId) {
      // Update existing sale order item
      const updateData: SaleOrderItemUpdate = {
        id: this.saleOrderItemId,
        saleOrderId: formValue.saleOrderId,
        jewelleryItemId: formValue.jewelleryItemId,
        quantity: formValue.quantity,
        discountAmount: formValue.discountAmount || 0,
        gstPercentage: formValue.gstPercentage,
        stoneAmount: formValue.stoneAmount,
        statusId: formValue.statusId,
      };

      this.saleOrderItemService.updateSaleOrderItem(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new sale order item with automatic calculation
      if (this.useAutoCalculation) {
        const createData: SaleOrderItemWithCalculation = {
          saleOrderId: formValue.saleOrderId,
          jewelleryItemId: formValue.jewelleryItemId,
          quantity: formValue.quantity,
          discountAmount: formValue.discountAmount || 0,
          gstPercentage: formValue.gstPercentage,
          stoneAmount: formValue.stoneAmount,
        };

        this.saleOrderItemService.createSaleOrderItemWithCalculation(createData).subscribe({
          next: () => {
            this.isSubmitting = false;
            this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
          },
          error: () => {
            this.isSubmitting = false;
          },
        });
      } else {
        // Create without automatic calculation
        const createData: SaleOrderItemCreate = {
          saleOrderId: formValue.saleOrderId,
          jewelleryItemId: formValue.jewelleryItemId,
          quantity: formValue.quantity,
          discountAmount: formValue.discountAmount || 0,
          gstPercentage: formValue.gstPercentage,
          stoneAmount: formValue.stoneAmount,
        };

        this.saleOrderItemService.createSaleOrderItem(createData).subscribe({
          next: () => {
            this.isSubmitting = false;
            this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
          },
          error: () => {
            this.isSubmitting = false;
          },
        });
      }
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.saleOrderItemForm.controls).forEach((key) => {
      this.saleOrderItemForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    if (this.wizardMode) {
      this.cancelClicked.emit();
    } else {
      this.router.navigate(['jewelleryManagement/admin/saleorderitem']);
    }
  }

  /**
   * Toggle auto calculation mode
   */
  toggleAutoCalculation(): void {
    this.useAutoCalculation = !this.useAutoCalculation;
  }

  /**
   * Get making charge type label
   */
  getMakingChargeTypeLabel(type: MakingChargeType): string {
    return getMakingChargeTypeLabel(type);
  }

  /**
   * Get status label
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Format currency for display
   */
  formatCurrency(amount: number): string {
    return formatCurrency(amount);
  }

  /**
   * Format weight for display
   */
  formatWeight(weight: number): string {
    return formatWeight(weight);
  }

  /**
   * Get stock status class
   */
  getStockStatusClass(): string {
    if (this.stockAvailable === null) return '';
    return this.stockAvailable ? 'text-success' : 'text-danger';
  }

  /**
   * Get stock status message
   */
  getStockStatusMessage(): string {
    if (this.stockAvailable === null) return '';
    if (this.stockAvailable) {
      return `In Stock (${this.availableQuantity} available)`;
    }
    return `Insufficient Stock (Only ${this.availableQuantity} available)`;
  }

  /**
   * Select a jewellery item from search results
   */
  selectJewelleryItem(item: JewelleryItemWithDetails): void {
    this.selectedItem = item;
    this.searchQuery = item.itemCode || item.name;
    this.saleOrderItemForm.patchValue({ jewelleryItemId: item.id });
    this.filteredItems = [];
    this.checkStockAvailability();
    // Fetch stock quantity for the selected item
    this.loadStockForItem(item.id);
    // Calculate initial price preview
    this.calculatePricePreview();
  }
  
  /**
   * Load stock quantity for a specific item
   */
  private loadStockForItem(jewelleryItemId: number): void {
    this.itemStockService.getItemStockByJewelleryItemId(jewelleryItemId).subscribe({
      next: (stock) => {
        if (stock && this.selectedItem) {
          // Update the selected item with stock info
          this.selectedItem = {
            ...this.selectedItem,
            stockQuantity: stock.quantity - stock.reservedQuantity
          };
          // Also update in the main list
          const index = this.jewelleryItems.findIndex(i => i.id === jewelleryItemId);
          if (index !== -1) {
            this.jewelleryItems[index] = {
              ...this.jewelleryItems[index],
              stockQuantity: stock.quantity - stock.reservedQuantity
            };
          }
        }
      },
      error: () => {
        // Stock info not available, keep default 0
      }
    });
  }

  /**
   * Clear selected item
   */
  clearSelectedItem(): void {
    this.selectedItem = null;
    this.searchQuery = '';
    this.saleOrderItemForm.patchValue({ jewelleryItemId: null });
    this.stockAvailable = null;
    this.availableQuantity = 0;
    // Reset price breakdown
    this.resetPriceBreakdown();
  }

  /**
   * Reset the item form for adding a new item
   */
  resetItemForm(): void {
    this.selectedItem = null;
    this.searchQuery = '';
    this.stockAvailable = null;
    this.availableQuantity = 0;
    // Reset price breakdown
    this.resetPriceBreakdown();
    this.saleOrderItemForm.reset({
      quantity: 1,
      discountAmount: 0,
      gstPercentage: 3,
      statusId: 1,
    });
    // Re-set the saleOrderId from wizard state
    const wizardState = this.wizardService.getCurrentState();
    if (wizardState.saleOrder && wizardState.saleOrder.id) {
      this.saleOrderItemForm.patchValue({ saleOrderId: wizardState.saleOrder.id });
    }
  }

  /**
   * Remove an item from the order
   */
  removeItemFromOrder(itemId: number): void {
    this.wizardService.removeItemFromOrder(itemId);
    this.wizardState = this.wizardService.getCurrentState();
    this.toastr.success('Item removed from order');
  }

  /**
   * Proceed to payment step
   */
  proceedToPayment(): void {
    if (this.wizardState.saleOrderItems.length === 0) {
      this.toastr.error('Please add at least one item to the order');
      return;
    }
    this.wizardService.nextStep();
    this.router.navigate(['../payment'], { relativeTo: this.route });
  }
}
