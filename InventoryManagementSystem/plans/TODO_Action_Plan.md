# Jewellery Inventory Management System - Action Plan

## Document Information
- **Created**: February 2026
- **Purpose**: Comprehensive to-do list for system completion
- **Target Market**: Indian Jewellery Industry

---

## Executive Summary

The system is approximately **55% complete** with a solid technical foundation. This document outlines all items that need to be added, modified, or removed to achieve a production-ready jewellery inventory management system for the Indian market.

---

## Part 1: Items to Add

### 1.1 Missing Frontend Modules (Backend Ready)

| Module | Priority | Effort | Backend Status | Frontend Status |
|--------|----------|--------|----------------|-----------------|
| Exchange Management | P0 | 2-3 days | ✅ Complete | ❌ Missing |
| User Management | P0 | 2 days | ✅ Complete | ❌ Missing |
| Role Management | P0 | 1 day | ✅ Complete | ❌ Missing |
| User KYC Management | P1 | 1-2 days | ✅ Complete | ❌ Missing |
| Warehouse Management | P1 | 1-2 days | ✅ Complete | ❌ Missing |
| Purchase Order Management | P1 | 2-3 days | ✅ Complete | ❌ Missing |
| Purchase Order Items | P1 | 1 day | ✅ Complete | ❌ Missing |

**Action Items:**
- [ ] Create `exchange-management` feature module with table and form components
- [ ] Create `user-management` feature module with CRUD operations
- [ ] Create `role-management` feature module
- [ ] Create `user-kyc-management` feature module
- [ ] Create `warehouse-management` feature module
- [ ] Create `purchase-order-management` feature module
- [ ] Create `purchase-order-item` integrated into purchase order workflow

---

### 1.2 India-Specific Compliance Features

#### 1.2.1 BIS Hallmark HUID Tracking (Critical - Mandatory since April 2023)

**Backend Changes Required:**

```csharp
// Add to JewelleryItemDb.cs
[Column("huid")]
public string? HUID { get; set; } // 6-digit alphanumeric Hallmark Unique ID

[Column("bis_certification_number")]
public string? BISCertificationNumber { get; set; }

[Column("hallmark_center_name")]
public string? HallmarkCenterName { get; set; }

[Column("hallmark_date")]
public DateTime? HallmarkDate { get; set; }
```

**Action Items:**
- [ ] Add HUID field to `JewelleryItemDb`
- [ ] Add HUID field to `InvoiceItemDb` (snapshot)
- [ ] Create migration for new columns
- [ ] Update frontend forms to capture HUID
- [ ] Add HUID to invoice print format

#### 1.2.2 E-Invoice Integration (GST Compliance)

**Action Items:**
- [ ] Create `EInvoiceService` for IRN generation
- [ ] Integrate with NIC portal for e-invoice
- [ ] Add QR code generation on invoices
- [ ] Store IRN in `InvoiceDb`
- [ ] Add e-invoice cancellation handling

**Backend Model Addition:**
```csharp
// Add to InvoiceDb.cs
[Column("irn")]
public string? IRN { get; set; } // Invoice Reference Number

[Column("irn_generated_date")]
public DateTime? IRNGeneratedDate { get; set; }

[Column("qr_code")]
public string? QRCode { get; set; } // Base64 QR code
```

#### 1.2.3 TCS (Tax Collected at Source)

**Action Items:**
- [ ] Add TCS calculation for sales > ₹10 lakh
- [ ] Track customer PAN for TCS exemption
- [ ] Generate TCS report data (Form 26Q format)
- [ ] Add TCS fields to invoice

#### 1.2.4 E-Way Bill Generation

**Action Items:**
- [ ] Create `EWayBillService`
- [ ] Integrate with NIC e-way bill portal
- [ ] Add e-way bill fields to invoice
- [ ] Handle inter-state sales e-way bill requirements

---

### 1.3 Business Features

#### 1.3.1 Repair/Job Work Management

**New Backend Models Required:**

```csharp
// RepairOrderDb.cs
public class RepairOrderDb
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime ReceivedDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string ItemDescription { get; set; }
    public string RepairType { get; set; } // RESIZING, POLISHING, REPAIR, REFINISH
    public decimal EstimatedCost { get; set; }
    public decimal ActualCost { get; set; }
    public string WorkStatus { get; set; } // RECEIVED, IN_PROGRESS, READY, DELIVERED
    public int? ArtisanId { get; set; } // Karigar assignment
    // ... audit fields
}
```

**Action Items:**
- [ ] Create `RepairOrderDb` model
- [ ] Create `RepairOrderItemDb` model
- [ ] Create repository and service layers
- [ ] Create API controller
- [ ] Create frontend module

#### 1.3.2 Customer Loyalty & Wallet

**Action Items:**
- [ ] Add `LoyaltyPoints` to `UserDb`
- [ ] Add `WalletBalance` to `UserDb`
- [ ] Create `WalletTransactionDb` for wallet history
- [ ] Create loyalty points accrual logic
- [ ] Create redemption logic at checkout

#### 1.3.3 Barcode/QR Code Generation

**Action Items:**
- [ ] Install `QRCoder` NuGet package
- [ ] Create `IBarcodeService`
- [ ] Generate unique item codes (format: JJM-YYYY-NNNNNN)
- [ ] Generate QR codes with item details
- [ ] Create printable label format
- [ ] Add barcode scanning support in frontend

#### 1.3.4 Stock Alerts & Reorder Management

**Action Items:**
- [ ] Add `ReorderLevel` to `ItemStockDb`
- [ ] Create `StockAlertDb` model
- [ ] Create `IStockAlertService`
- [ ] Implement low stock detection
- [ ] Create notification system (email/SMS)

---

### 1.4 Dashboard & Reporting

#### 1.4.1 Dashboard

**Action Items:**
- [ ] Create `DashboardController`
- [ ] Create `IDashboardService`
- [ ] Implement metrics:
  - [ ] Today's sales
  - [ ] Monthly sales
  - [ ] Low stock items count
  - [ ] Pending orders
  - [ ] Total inventory value
  - [ ] Top selling items
- [ ] Create dashboard frontend component

#### 1.4.2 Reports

**Reports to Implement:**

| Report | Purpose | Priority |
|--------|---------|----------|
| Daily Sales Report | Day-wise sales summary | P0 |
| GST Report (GSTR-1 format) | Tax filing compliance | P0 |
| GST Report (GSTR-3B format) | Tax filing compliance | P0 |
| Stock Valuation Report | Current inventory value | P1 |
| Customer Outstanding Report | Pending payments | P1 |
| Metal-wise Sales Summary | Category performance | P1 |
| Exchange/Buyback Summary | Exchange tracking | P2 |
| Profit/Loss Analysis | Business intelligence | P2 |

**Action Items:**
- [ ] Create `ReportsController`
- [ ] Create `IReportService`
- [ ] Implement each report endpoint
- [ ] Add export to Excel functionality
- [ ] Add export to PDF functionality
- [ ] Create reports frontend module

---

### 1.5 Architectural Improvements

#### 1.5.1 Exchange-Sale Order Link

**Current Issue:** `SaleOrderDb.IsExchangeSale` is just a boolean flag

**Required Change:**
```csharp
// Add to SaleOrderDb.cs
[Column("exchange_order_id")]
public long? ExchangeOrderId { get; set; }

[ForeignKey(nameof(ExchangeOrderId))]
public virtual ExchangeOrderDb? ExchangeOrder { get; set; }
```

**Action Items:**
- [ ] Add `ExchangeOrderId` foreign key to `SaleOrderDb`
- [ ] Update `SaleOrderService` to link exchange orders
- [ ] Update frontend to show linked exchange details

#### 1.5.2 Stock Validation & Deduction

**Action Items:**
- [ ] Add stock validation before order confirmation
- [ ] Implement stock reservation during order creation
- [ ] Implement stock deduction on invoice generation
- [ ] Handle stock restoration on order cancellation

#### 1.5.3 Soft Delete Implementation

**Action Items:**
- [ ] Add `IsDeleted` flag to all entities
- [ ] Update repositories to filter deleted records
- [ ] Update services to use soft delete
- [ ] Create migration for new column

#### 1.5.4 Caching Layer

**Action Items:**
- [ ] Add Redis or MemoryCache configuration
- [ ] Cache metal rates (refresh daily)
- [ ] Cache stone rates
- [ ] Cache frequently accessed master data

#### 1.5.5 API Versioning

**Action Items:**
- [ ] Add API versioning package
- [ ] Version all controllers (v1)
- [ ] Update Swagger documentation
- [ ] Update frontend API calls

---

## Part 2: Items to Remove/Modify

### 2.1 Code Issues to Fix

| Issue | Location | Action |
|-------|----------|--------|
| Duplicate service registration | `Program.cs` line 115 & 122 | Remove duplicate `IMetalRateService` registration |

**Action Items:**
- [ ] Remove duplicate `IMetalRateService` registration in `Program.cs`

---

### 2.2 Frontend Modules to Evaluate

| Module | Current State | Recommendation |
|--------|---------------|----------------|
| `demo` folder | Contains demo components | Remove or replace with actual components |
| `invoicepaymentmanagement` | Separate module | Evaluate if needed - may overlap with `paymentmanagement` |

**Action Items:**
- [ ] Remove `demo` folder and components
- [ ] Evaluate `invoicepaymentmanagement` vs `paymentmanagement` overlap
- [ ] Consolidate if redundant

---

### 2.3 UX Flow Improvements

**Current Issue:** SaleOrderItem and InvoiceItem are separate CRUD modules

**Recommendation:** Consider integrating into parent workflows:
- SaleOrderItem should be embedded in SaleOrder form
- InvoiceItem should be view-only within Invoice details

**Action Items:**
- [ ] Evaluate embedding SaleOrderItem in SaleOrder workflow
- [ ] Make InvoiceItem view-only (no separate CRUD)

---

## Part 3: Implementation Priority

### Phase 1: Critical Fixes (Week 1-2)

| Task | Priority | Effort |
|------|----------|--------|
| Remove duplicate service registration | P0 | 5 min |
| Create Exchange frontend module | P0 | 2-3 days |
| Create User/Role frontend modules | P0 | 2-3 days |
| Add ExchangeOrderId FK to SaleOrder | P0 | 1 hour |
| Implement stock validation | P0 | 1 day |

### Phase 2: India Compliance (Week 3-4)

| Task | Priority | Effort |
|------|----------|--------|
| Add HUID tracking | P0 | 1-2 days |
| Implement e-invoice IRN generation | P1 | 1 week |
| Add TCS calculation | P1 | 2-3 days |
| Create GST reports | P0 | 3-4 days |

### Phase 3: Business Features (Week 5-8)

| Task | Priority | Effort |
|------|----------|--------|
| Implement Dashboard | P1 | 3-4 days |
| Create Reports module | P1 | 1-2 weeks |
| Add Barcode/QR generation | P2 | 3-4 days |
| Implement Repair module | P2 | 1 week |
| Add Loyalty/Wallet | P3 | 1 week |

### Phase 4: Infrastructure (Week 9-10)

| Task | Priority | Effort |
|------|----------|--------|
| Add API versioning | P2 | 1 day |
| Implement caching | P2 | 2-3 days |
| Add soft delete | P2 | 1-2 days |
| Remove demo components | P3 | 1 hour |

---

## Part 4: Module Completion Status

| Module | Backend | Frontend | Integration | Status |
|--------|---------|----------|-------------|--------|
| Category | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Metal | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Purity | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Stone | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Stone Rate | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Metal Rate | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Supplier | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Jewellery Item | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Item Stock | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Sale Order | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Sale Order Item | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Invoice | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Invoice Item | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Payment | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Invoice Payment | ✅ 100% | ✅ 100% | ✅ Complete | Done |
| Exchange | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| User | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| Role | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| User KYC | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| Warehouse | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| Purchase Order | ✅ 100% | ❌ 0% | ❌ Missing | **TODO** |
| Dashboard | ❌ 0% | ❌ 0% | ❌ Missing | **TODO** |
| Reports | ❌ 0% | ❌ 0% | ❌ Missing | **TODO** |
| Repair/Job Work | ❌ 0% | ❌ 0% | ❌ Missing | **TODO** |
| Barcode/QR | ❌ 0% | ❌ 0% | ❌ Missing | **TODO** |

**Overall Completion: ~55%**

---

## Part 5: Quick Reference Checklist

### Immediate Actions (Do First)
- [ ] Remove duplicate `IMetalRateService` registration in `Program.cs`
- [ ] Add `ExchangeOrderId` FK to `SaleOrderDb`
- [ ] Create Exchange frontend module
- [ ] Create User/Role frontend modules

### High Priority (Week 1-2)
- [ ] Add HUID tracking for hallmark compliance
- [ ] Implement stock validation before order confirmation
- [ ] Create Dashboard with basic metrics
- [ ] Create GST reports (GSTR-1, GSTR-3B format)

### Medium Priority (Week 3-4)
- [ ] Implement e-invoice IRN generation
- [ ] Add TCS calculation for high-value sales
- [ ] Create remaining frontend modules (Warehouse, Purchase Order, KYC)
- [ ] Add barcode/QR code generation

### Low Priority (Week 5+)
- [ ] Implement Repair/Job Work module
- [ ] Add Customer Loyalty/Wallet
- [ ] Add API versioning
- [ ] Implement caching layer
- [ ] Add soft delete to all entities

### Cleanup (As Needed)
- [ ] Remove `demo` folder
- [ ] Evaluate `invoicepaymentmanagement` redundancy
- [ ] Consolidate overlapping modules

---

## Document History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Feb 2026 | Initial comprehensive action plan |

---

*This document should be updated as tasks are completed and new requirements are identified.*
