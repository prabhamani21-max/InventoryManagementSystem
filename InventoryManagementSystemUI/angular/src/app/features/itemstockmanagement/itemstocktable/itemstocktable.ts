// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { ItemStockService } from 'src/app/core/services/item-stock.service';
import { JewelleryItemService } from 'src/app/core/services/jewellery-item.service';
import { WarehouseService } from 'src/app/core/services/warehouse.service';
import { ItemStock, getStatusLabel, getStatusClass } from 'src/app/core/models/item-stock.model';
import { JewelleryItem } from 'src/app/core/models/jewellery-item.model';
import { Warehouse } from 'src/app/core/models/warehouse.model';
import { ToastrService } from 'ngx-toastr';
import { ConfirmationService } from 'src/app/common/confirm-dialog/confirm-dialog.service';

/**
 * ItemStock Table Component
 * Displays item stocks in a table format with CRUD operations
 */
@Component({
  selector: 'app-itemstocktable',
  imports: [CommonModule, SharedModule],
  templateUrl: './itemstocktable.html',
  styleUrl: './itemstocktable.scss',
})
export class Itemstocktable implements OnInit {
  // Injected services
  private itemStockService = inject(ItemStockService);
  private jewelleryItemService = inject(JewelleryItemService);
  private warehouseService = inject(WarehouseService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  private confirmationService = inject(ConfirmationService);

  // Properties
  itemStocks: ItemStock[] = [];
  jewelleryItems: JewelleryItem[] = [];
  warehouses: Warehouse[] = [];
  isLoading: boolean = false;

  // Table columns
  displayedColumns: string[] = ['id', 'jewelleryItemId', 'warehouseId', 'quantity', 'reservedQuantity', 'availableQuantity', 'status', 'actions'];

  ngOnInit(): void {
    this.loadJewelleryItems();
    this.loadWarehouses();
    this.loadItemStocks();
  }

  /**
   * Load all jewellery items for name mapping
   */
  loadJewelleryItems(): void {
    this.jewelleryItemService.getAllJewelleryItems().subscribe({
      next: (items) => {
        this.jewelleryItems = items;
      },
    });
  }

  /**
   * Load all warehouses for name mapping
   */
  loadWarehouses(): void {
    this.warehouseService.getAllWarehouses().subscribe({
      next: (warehouses) => {
        this.warehouses = warehouses;
      },
    });
  }

  /**
   * Load all item stocks from the API
   */
  loadItemStocks(): void {
    this.isLoading = true;
    this.itemStockService.getAllItemStocks().subscribe({
      next: (items) => {
        this.itemStocks = items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  /**
   * Get jewellery item name by ID
   */
  getJewelleryItemName(jewelleryItemId: number): string {
    const item = this.jewelleryItems.find(i => i.id === jewelleryItemId);
    return item ? item.name : `Item #${jewelleryItemId}`;
  }

  /**
   * Get jewellery item code by ID
   */
  getJewelleryItemCode(jewelleryItemId: number): string | null {
    const item = this.jewelleryItems.find(i => i.id === jewelleryItemId);
    return item?.itemCode || null;
  }

  /**
   * Get warehouse name by ID
   */
  getWarehouseName(warehouseId: number): string {
    const warehouse = this.warehouses.find(w => w.id === warehouseId);
    return warehouse ? warehouse.name : `Warehouse #${warehouseId}`;
  }

  /**
   * Navigate to add item stock form
   */
  onAddItemStock(): void {
    this.router.navigate(['jewelleryManagement/admin/itemstock/add']);
  }

  /**
   * Navigate to edit item stock form
   */
  onEditItemStock(id: number): void {
    this.router.navigate(['jewelleryManagement/admin/itemstock/edit', id]);
  }

  /**
   * Delete item stock with confirmation dialog
   */
  onDeleteItemStock(id: number): void {
    this.confirmationService
      .confirm('Delete Item Stock', 'Are you sure you want to delete this item stock? This action cannot be undone.', 'Delete', 'Cancel')
      .then((confirmed) => {
        if (confirmed) {
          this.itemStockService.deleteItemStock(id).subscribe({
            next: () => {
              this.loadItemStocks();
            },
          });
        }
      });
  }

  /**
   * Get status label based on status ID
   */
  getStatusLabel(statusId: number): string {
    return getStatusLabel(statusId);
  }

  /**
   * Get status CSS class based on status ID
   */
  getStatusClass(statusId: number): string {
    return getStatusClass(statusId);
  }

  /**
   * Calculate available quantity
   */
  getAvailableQuantity(item: ItemStock): number {
    return item.quantity - item.reservedQuantity;
  }
}