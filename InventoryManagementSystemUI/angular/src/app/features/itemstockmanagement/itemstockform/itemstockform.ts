// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { ItemStockService } from 'src/app/core/services/item-stock.service';
import { JewelleryItemService } from 'src/app/core/services/jewellery-item.service';
import { WarehouseService } from 'src/app/core/services/warehouse.service';
import { ItemStock, ItemStockCreate, ItemStockUpdate } from 'src/app/core/models/item-stock.model';
import { JewelleryItem } from 'src/app/core/models/jewellery-item.model';
import { Warehouse } from 'src/app/core/models/warehouse.model';
import { ToastrService } from 'ngx-toastr';

/**
 * ItemStock Form Component
 * Handles both create and edit operations for item stocks
 */
@Component({
  selector: 'app-itemstockform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './itemstockform.html',
  styleUrl: './itemstockform.scss',
})
export class Itemstockform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private itemStockService = inject(ItemStockService);
  private jewelleryItemService = inject(JewelleryItemService);
  private warehouseService = inject(WarehouseService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  itemStockForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  itemStockId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Dropdown data
  jewelleryItems: JewelleryItem[] = [];
  warehouses: Warehouse[] = [];

  // Status options
  statusOptions = [
    { id: 1, name: 'Active' },
    { id: 2, name: 'Inactive' },
    { id: 3, name: 'Out of Stock' },
  ];

  // Form validation messages
  validationMessages = {
    jewelleryItemId: {
      required: 'Jewellery item is required',
    },
    warehouseId: {
      required: 'Warehouse is required',
    },
    quantity: {
      required: 'Quantity is required',
      min: 'Quantity must be 0 or greater',
    },
    reservedQuantity: {
      required: 'Reserved quantity is required',
      min: 'Reserved quantity must be 0 or greater',
    },
    statusId: {
      required: 'Status is required',
    },
  };

  ngOnInit(): void {
    this.initializeForm();
    this.loadDropdownData();
    this.checkEditMode();
  }

  /**
   * Initialize the reactive form
   */
  initializeForm(): void {
    this.itemStockForm = this.fb.group({
      jewelleryItemId: [null, [Validators.required]],
      warehouseId: [null, [Validators.required]],
      quantity: [0, [Validators.required, Validators.min(0)]],
      reservedQuantity: [0, [Validators.required, Validators.min(0)]],
      statusId: [1, [Validators.required]],
    });
  }

  /**
   * Load dropdown data from APIs
   */
  loadDropdownData(): void {
    // Load jewellery items
    this.jewelleryItemService.getAllJewelleryItems().subscribe({
      next: (items) => {
        this.jewelleryItems = items;
      },
    });

    // Load warehouses
    this.warehouseService.getAllWarehouses().subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
      },
    });
  }

  /**
   * Check if we're in edit mode and load item stock data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.itemStockId = +id;
      this.loadItemStockData(this.itemStockId);
    }
  }

  /**
   * Load item stock data for editing
   */
  loadItemStockData(id: number): void {
    this.isLoading = true;
    this.itemStockService.getItemStockById(id).subscribe({
      next: (item) => {
        if (item) {
          this.itemStockForm.patchValue({
            jewelleryItemId: item.jewelleryItemId,
            warehouseId: item.warehouseId,
            quantity: item.quantity,
            reservedQuantity: item.reservedQuantity,
            statusId: item.statusId,
          });
        } else {
          this.toastr.error('Item stock not found');
          this.router.navigate(['jewelleryManagement/admin/itemstock']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/itemstock']);
      },
    });
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.itemStockForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.itemStockForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.itemStockForm.get(fieldName);
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
    if (this.itemStockForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.itemStockForm.value;

    if (this.isEditMode && this.itemStockId) {
      // Update existing item stock
      const updateData: ItemStockUpdate = {
        id: this.itemStockId,
        jewelleryItemId: formValue.jewelleryItemId,
        warehouseId: formValue.warehouseId,
        quantity: formValue.quantity,
        reservedQuantity: formValue.reservedQuantity,
        statusId: formValue.statusId,
      };

      this.itemStockService.updateItemStock(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/itemstock']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new item stock
      const createData: ItemStockCreate = {
        jewelleryItemId: formValue.jewelleryItemId,
        warehouseId: formValue.warehouseId,
        quantity: formValue.quantity,
        reservedQuantity: formValue.reservedQuantity,
        statusId: formValue.statusId,
      };

      this.itemStockService.createItemStock(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/itemstock']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  markAllFieldsAsTouched(): void {
    Object.keys(this.itemStockForm.controls).forEach((key) => {
      this.itemStockForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/itemstock']);
  }
}