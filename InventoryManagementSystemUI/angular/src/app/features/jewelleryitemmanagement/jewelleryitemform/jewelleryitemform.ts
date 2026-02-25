// angular import
import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, UntypedFormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';

// project import
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { JewelleryItemService } from 'src/app/core/services/jewellery-item.service';
import { CategoryService } from 'src/app/core/services/category.service';
import { MetalService } from 'src/app/core/services/metal.service';
import { PurityService } from 'src/app/core/services/purity.service';
import { StoneService } from 'src/app/core/services/stone.service';
import { JewelleryItem, JewelleryItemCreate, JewelleryItemUpdate, MakingChargeType } from 'src/app/core/models/jewellery-item.model';
import { Category } from 'src/app/core/models/category.model';
import { Metal } from 'src/app/core/models/metal.model';
import { Purity } from 'src/app/core/models/purity.model';
import { Stone } from 'src/app/core/models/stone.model';
import { ToastrService } from 'ngx-toastr';

/**
 * JewelleryItem Form Component
 * Handles both create and edit operations for jewellery items
 */
@Component({
  selector: 'app-jewelleryitemform',
  imports: [CommonModule, SharedModule, ReactiveFormsModule],
  templateUrl: './jewelleryitemform.html',
  styleUrl: './jewelleryitemform.scss',
})
export class Jewelleryitemform implements OnInit {
  // Injected services
  private fb = inject(FormBuilder);
  private jewelleryItemService = inject(JewelleryItemService);
  private categoryService = inject(CategoryService);
  private metalService = inject(MetalService);
  private purityService = inject(PurityService);
  private stoneService = inject(StoneService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private toastr = inject(ToastrService);

  // Properties
  jewelleryItemForm!: UntypedFormGroup;
  isEditMode: boolean = false;
  jewelleryItemId: number | null = null;
  isLoading: boolean = false;
  isSubmitting: boolean = false;

  // Dropdown data
  categories: Category[] = [];
  metals: Metal[] = [];
  purities: Purity[] = [];
  filteredPurities: Purity[] = [];
  stones: Stone[] = [];

  // Making charge type options
  makingChargeTypes = [
    { value: MakingChargeType.PER_GRAM, label: 'Per Gram' },
    { value: MakingChargeType.PERCENTAGE, label: 'Percentage' },
    { value: MakingChargeType.FIXED, label: 'Fixed Amount' },
  ];

  // Form validation messages
  validationMessages = {
    name: {
      required: 'Item name is required',
      minlength: 'Name must be at least 2 characters',
      maxlength: 'Name cannot exceed 200 characters',
    },
    description: {
      maxlength: 'Description cannot exceed 1000 characters',
    },
    categoryId: {
      required: 'Category is required',
    },
    metalId: {
      required: 'Metal is required',
    },
    purityId: {
      required: 'Purity is required',
    },
    grossWeight: {
      required: 'Gross weight is required',
      min: 'Gross weight must be greater than 0',
    },
    netMetalWeight: {
      required: 'Net metal weight is required',
      min: 'Net metal weight must be greater than 0',
    },
    makingChargeType: {
      required: 'Making charge type is required',
    },
    makingChargeValue: {
      required: 'Making charge value is required',
      min: 'Making charge value must be 0 or greater',
    },
    wastagePercentage: {
      required: 'Wastage percentage is required',
      min: 'Wastage percentage must be 0 or greater',
      max: 'Wastage percentage cannot exceed 100',
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
    this.jewelleryItemForm = this.fb.group({
      itemCode: [''],
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      categoryId: [null, [Validators.required]],
      hasStone: [false],
      stoneId: [null],
      metalId: [null, [Validators.required]],
      purityId: [null, [Validators.required]],
      grossWeight: [0, [Validators.required, Validators.min(0.001)]],
      netMetalWeight: [0, [Validators.required, Validators.min(0.001)]],
      makingChargeType: [MakingChargeType.PER_GRAM, [Validators.required]],
      makingChargeValue: [0, [Validators.required, Validators.min(0)]],
      wastagePercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      isHallmarked: [false],
      // Hallmark Details
      huid: ['', [Validators.maxLength(6), Validators.pattern('^[A-Za-z0-9]{6}$')]],
      bisCertificationNumber: ['', [Validators.maxLength(50)]],
      hallmarkCenterName: ['', [Validators.maxLength(100)]],
      hallmarkDate: [null],
      statusId: [1, [Validators.required]],
    });

    // Subscribe to metalId changes to filter purities
    this.jewelleryItemForm.get('metalId')?.valueChanges.subscribe((metalId) => {
      this.onMetalChange(metalId);
    });

    // Subscribe to hasStone changes
    this.jewelleryItemForm.get('hasStone')?.valueChanges.subscribe((hasStone) => {
      this.onHasStoneChange(hasStone);
    });
  }

  /**
   * Load dropdown data from APIs
   */
  loadDropdownData(): void {
    // Load categories
    this.categoryService.getAllCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
      },
    });

    // Load metals
    this.metalService.getAllMetals().subscribe({
      next: (metals) => {
        this.metals = metals;
      },
    });

    // Load all purities
    this.purityService.getAllPurities().subscribe({
      next: (purities) => {
        this.purities = purities;
      },
    });

    // Load stones
    this.stoneService.getAllStones().subscribe({
      next: (stones) => {
        this.stones = stones;
      },
    });
  }

  /**
   * Check if we're in edit mode and load item data
   */
  checkEditMode(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.jewelleryItemId = +id;
      this.loadJewelleryItemData(this.jewelleryItemId);
    }
  }

  /**
   * Load jewellery item data for editing
   */
  loadJewelleryItemData(id: number): void {
    this.isLoading = true;
    this.jewelleryItemService.getJewelleryItemById(id).subscribe({
      next: (item) => {
        if (item) {
          // Filter purities based on selected metal
          this.filteredPurities = this.purities.filter((p) => p.metalId === item.metalId);
          
          this.jewelleryItemForm.patchValue({
            itemCode: item.itemCode || '',
            name: item.name,
            description: item.description || '',
            categoryId: item.categoryId,
            hasStone: item.hasStone,
            stoneId: item.stoneId,
            metalId: item.metalId,
            purityId: item.purityId,
            grossWeight: item.grossWeight,
            netMetalWeight: item.netMetalWeight,
            makingChargeType: item.makingChargeType,
            makingChargeValue: item.makingChargeValue,
            wastagePercentage: item.wastagePercentage,
            isHallmarked: item.isHallmarked,
            huid: item.huid || '',
            bisCertificationNumber: item.bisCertificationNumber || '',
            hallmarkCenterName: item.hallmarkCenterName || '',
            hallmarkDate: item.hallmarkDate || null,
            statusId: item.statusId,
          });
        } else {
          this.toastr.error('Jewellery item not found');
          this.router.navigate(['jewelleryManagement/admin/jewellery']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['jewelleryManagement/admin/jewellery']);
      },
    });
  }

  /**
   * Handle metal selection change - filter purities by metal
   */
  onMetalChange(metalId: number | null): void {
    if (metalId) {
      this.filteredPurities = this.purities.filter((p) => p.metalId === metalId);
    } else {
      this.filteredPurities = [];
    }
    // Reset purity selection when metal changes
    this.jewelleryItemForm.get('purityId')?.setValue(null);
  }

  /**
   * Handle hasStone checkbox change
   */
  onHasStoneChange(hasStone: boolean): void {
    if (!hasStone) {
      this.jewelleryItemForm.get('stoneId')?.setValue(null);
    }
  }

  /**
   * Get form controls for template access
   */
  get formControls() {
    return this.jewelleryItemForm.controls;
  }

  /**
   * Check if a field has errors
   */
  hasError(fieldName: string): boolean {
    const field = this.jewelleryItemForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Get error message for a field
   */
  getErrorMessage(fieldName: string): string {
    const field = this.jewelleryItemForm.get(fieldName);
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
    if (this.jewelleryItemForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.jewelleryItemForm.value;

    if (this.isEditMode && this.jewelleryItemId) {
      // Update existing jewellery item
      const updateData: JewelleryItemUpdate = {
        id: this.jewelleryItemId,
        itemCode: formValue.itemCode || undefined,
        name: formValue.name,
        description: formValue.description || '',
        categoryId: formValue.categoryId,
        hasStone: formValue.hasStone,
        stoneId: formValue.hasStone ? formValue.stoneId : undefined,
        metalId: formValue.metalId,
        purityId: formValue.purityId,
        grossWeight: formValue.grossWeight,
        netMetalWeight: formValue.netMetalWeight,
        makingChargeType: formValue.makingChargeType,
        makingChargeValue: formValue.makingChargeValue,
        wastagePercentage: formValue.wastagePercentage,
        isHallmarked: formValue.isHallmarked,
        huid: formValue.huid || undefined,
        bisCertificationNumber: formValue.bisCertificationNumber || undefined,
        hallmarkCenterName: formValue.hallmarkCenterName || undefined,
        hallmarkDate: formValue.hallmarkDate || undefined,
        statusId: formValue.statusId,
      };

      this.jewelleryItemService.updateJewelleryItem(updateData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/jewellery']);
        },
        error: () => {
          this.isSubmitting = false;
        },
      });
    } else {
      // Create new jewellery item
      const createData: JewelleryItemCreate = {
        itemCode: formValue.itemCode || undefined,
        name: formValue.name,
        description: formValue.description || '',
        categoryId: formValue.categoryId,
        hasStone: formValue.hasStone,
        stoneId: formValue.hasStone ? formValue.stoneId : undefined,
        metalId: formValue.metalId,
        purityId: formValue.purityId,
        grossWeight: formValue.grossWeight,
        netMetalWeight: formValue.netMetalWeight,
        makingChargeType: formValue.makingChargeType,
        makingChargeValue: formValue.makingChargeValue,
        wastagePercentage: formValue.wastagePercentage,
        isHallmarked: formValue.isHallmarked,
        huid: formValue.huid || undefined,
        bisCertificationNumber: formValue.bisCertificationNumber || undefined,
        hallmarkCenterName: formValue.hallmarkCenterName || undefined,
        hallmarkDate: formValue.hallmarkDate || undefined,
        statusId: formValue.statusId,
      };

      this.jewelleryItemService.createJewelleryItem(createData).subscribe({
        next: () => {
          this.isSubmitting = false;
          this.router.navigate(['jewelleryManagement/admin/jewellery']);
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
    Object.keys(this.jewelleryItemForm.controls).forEach((key) => {
      this.jewelleryItemForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Cancel and navigate back to list
   */
  onCancel(): void {
    this.router.navigate(['jewelleryManagement/admin/jewellery']);
  }
}
