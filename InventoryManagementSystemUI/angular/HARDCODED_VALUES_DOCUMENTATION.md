# Hardcoded Values Documentation

This document highlights all hardcoded values found in the Inventory Management System UI (Angular) codebase. These values should ideally be moved to configuration files, constants, or environment variables for better maintainability.

---

## Table of Contents

1. [API URLs and Endpoints](#1-api-urls-and-endpoints)
2. [Route Paths](#2-route-paths)
3. [Toast/Notification Messages](#3-toastnotification-messages)
4. [HTTP Status Codes](#4-http-status-codes)
5. [Numeric Constants](#5-numeric-constants)
6. [UI Labels and Placeholders](#6-ui-labels-and-placeholders)
7. [Validation Messages](#7-validation-messages)
8. [Configuration Values](#8-configuration-values)
9. [Claim Types and JWT Constants](#9-claim-types-and-jwt-constants)
10. [Recommendations](#recommendations)

---

## 1. API URLs and Endpoints

### Environment Files
**File:** [`src/environments/environment.ts`](src/environments/environment.ts)
```typescript
apiUrl: 'https://localhost:7192/api'
```

**File:** [`src/environments/environment.prod.ts`](src/environments/environment.prod.ts)
```typescript
apiUrl: 'http://localhost:4200'
```

### Service Files - API Endpoint Paths

All service files use hardcoded API endpoint paths. Here's a summary:

| Service File | Hardcoded Endpoints |
|-------------|---------------------|
| [`auth.service.ts`](src/app/core/services/auth.service.ts) | `/User/login`, `/User/register`, `/Roles/GetAllRoles`, `/User/checkEmail`, `/User/checkContact` |
| [`category.service.ts`](src/app/core/services/category.service.ts) | `/Category`, `/Category/{id}`, `/Category/name/{name}`, `/Category/active`, `/Category/parent/{parentId}` |
| [`exchange.service.ts`](src/app/core/services/exchange.service.ts) | `/Exchange`, `/Exchange/calculate`, `/Exchange/{id}`, `/Exchange/orderNumber/{orderNumber}`, `/Exchange/customer/{customerId}` |
| [`invoice.service.ts`](src/app/core/services/invoice.service.ts) | `/Invoice`, `/Invoice/generate`, `/Invoice/{invoiceNumber}`, `/Invoice/saleorder/{saleOrderId}`, `/EInvoice` |
| [`item-stock.service.ts`](src/app/core/services/item-stock.service.ts) | `/ItemStock`, `/ItemStock/by-item/{id}`, `/ItemStock/check-availability/{id}`, `/ItemStock/validate-order-stock` |
| [`jewellery-item.service.ts`](src/app/core/services/jewellery-item.service.ts) | `/JewelleryItem` |
| [`metal-rate-history.service.ts`](src/app/core/services/metal-rate-history.service.ts) | `/MetalRate`, `/MetalRate/current`, `/MetalRate/current/purity/{purityId}`, `/MetalRate/history/{id}` |
| [`payment.service.ts`](src/app/core/services/payment.service.ts) | `/Payment`, `/Payment/order/{orderId}` |
| [`purity.service.ts`](src/app/core/services/purity.service.ts) | `/Purity` |
| [`role.service.ts`](src/app/core/services/role.service.ts) | `/Role`, `/Role/GetAllRoles`, `/Role/GetByID/{id}`, `/Role/AddEdit`, `/Role/Delete/{id}` |
| [`sale-order.service.ts`](src/app/core/services/sale-order.service.ts) | `/SaleOrder` |
| [`sale-order-item.service.ts`](src/app/core/services/sale-order-item.service.ts) | `/SaleOrderItem`, `/SaleOrderItem/by-sale-order/{id}`, `/SaleOrderItem/calculate` |
| [`stone-rate-history.service.ts`](src/app/core/services/stone-rate-history.service.ts) | `/StoneRate`, `/StoneRate/all`, `/StoneRate/stone/{stoneId}`, `/StoneRate/current`, `/StoneRate/diamond`, `/StoneRate/diamond-rate-card` |
| [`stone.service.ts`](src/app/core/services/stone.service.ts) | `/Stone`, `/Stone/search` |
| [`supplier.service.ts`](src/app/core/services/supplier.service.ts) | `/Supplier` |
| [`tcs.service.ts`](src/app/core/services/tcs.service.ts) | `/tcs`, `/tcs/calculate`, `/tcs/customer/{id}/summary`, `/tcs/report/form26q` |
| [`user.service.ts`](src/app/core/services/user.service.ts) | `/User`, `/User/login`, `/User/register`, `/User/GetAllUsers`, `/User/GetUserById/{id}`, `/User/check-email`, `/User/check-contact` |
| [`warehouse.service.ts`](src/app/core/services/warehouse.service.ts) | `/Warehouse` |
| [`invoice-item.service.ts`](src/app/core/services/invoice-item.service.ts) | `/Invoice`, `/Invoice/items`, `/Invoice/{invoiceId}/items` |
| [`invoice-payment.service.ts`](src/app/core/services/invoice-payment.service.ts) | `/Payment`, `/Invoice` |
| [`metal.service.ts`](src/app/core/services/metal.service.ts) | `/Metal` |

---

## 2. Route Paths

### Navigation Routes
All route paths are hardcoded throughout the application:

**Base Route Pattern:** `jewelleryManagement/admin/{module}`

| Module | Hardcoded Routes | File Location |
|--------|-----------------|---------------|
| User | `jewelleryManagement/admin/user`, `jewelleryManagement/admin/user/add`, `jewelleryManagement/admin/user/edit/{id}`, `jewelleryManagement/admin/user/view/{id}` | [`usertable.ts`](src/app/features/usermanagement/usertable/usertable.ts), [`userform.ts`](src/app/features/usermanagement/userform/userform.ts) |
| Role | `jewelleryManagement/admin/role`, `jewelleryManagement/admin/role/add`, `jewelleryManagement/admin/role/edit/{id}`, `jewelleryManagement/admin/role/view/{id}` | [`roletable.ts`](src/app/features/rolemanagement/roletable/roletable.ts), [`roleform.ts`](src/app/features/rolemanagement/roleform/roleform.ts) |
| Category | `jewelleryManagement/admin/category`, `jewelleryManagement/admin/category/add`, `jewelleryManagement/admin/category/edit/{id}` | [`categorytable.ts`](src/app/features/categorymanagement/categorytable/categorytable.ts), [`categoryform.ts`](src/app/features/categorymanagement/categoryform/categoryform.ts) |
| Supplier | `jewelleryManagement/admin/supplier`, `jewelleryManagement/admin/supplier/add`, `jewelleryManagement/admin/supplier/edit/{id}` | [`suppliertable.ts`](src/app/features/suppliermanagement/suppliertable/suppliertable.ts), [`supplierform.ts`](src/app/features/suppliermanagement/supplierform/supplierform.ts) |
| Stone | `jewelleryManagement/admin/stone`, `jewelleryManagement/admin/stone/add`, `jewelleryManagement/admin/stone/edit/{id}` | [`stonetable.ts`](src/app/features/stonemanagement/stonetable/stonetable.ts), [`stoneform.ts`](src/app/features/stonemanagement/stoneform/stoneform.ts) |
| Metal | `jewelleryManagement/admin/metal`, `jewelleryManagement/admin/metal/add`, `jewelleryManagement/admin/metal/edit/{id}` | [`metaltable.ts`](src/app/features/metalmanagement/metaltable/metaltable.ts), [`metalform.ts`](src/app/features/metalmanagement/metalform/metalform.ts) |
| Purity | `jewelleryManagement/admin/purity`, `jewelleryManagement/admin/purity/add`, `jewelleryManagement/admin/purity/edit/{id}` | [`puritytable.ts`](src/app/features/puritymanagement/puritytable/puritytable.ts), [`purityform.ts`](src/app/features/puritymanagement/purityform/purityform.ts) |
| Jewellery Item | `jewelleryManagement/admin/jewellery`, `jewelleryManagement/admin/jewellery/add`, `jewelleryManagement/admin/jewellery/edit/{id}` | [`jewelleryitemtable.ts`](src/app/features/jewelleryitemmanagement/jewelleryitemtable/jewelleryitemtable.ts), [`jewelleryitemform.ts`](src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts) |
| Item Stock | `jewelleryManagement/admin/itemstock`, `jewelleryManagement/admin/itemstock/add`, `jewelleryManagement/admin/itemstock/edit/{id}` | [`itemstocktable.ts`](src/app/features/itemstockmanagement/itemstocktable/itemstocktable.ts), [`itemstockform.ts`](src/app/features/itemstockmanagement/itemstockform/itemstockform.ts) |
| Sale Order | `jewelleryManagement/admin/saleorder`, `jewelleryManagement/admin/saleorder/add`, `jewelleryManagement/admin/saleorder/edit/{id}` | [`saleordertable.ts`](src/app/features/saleordermanagement/saleordertable/saleordertable.ts), [`saleorderform.ts`](src/app/features/saleordermanagement/saleorderform/saleorderform.ts) |
| Sale Order Item | `jewelleryManagement/admin/saleorderitem`, `jewelleryManagement/admin/saleorderitem/add`, `jewelleryManagement/admin/saleorderitem/edit/{id}` | [`saleorderitemtable.ts`](src/app/features/saleorderitemmanagement/saleorderitemtable/saleorderitemtable.ts), [`saleorderitemform.ts`](src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts) |
| Invoice | `jewelleryManagement/admin/invoice`, `jewelleryManagement/admin/invoice/generate`, `jewelleryManagement/admin/invoice/view/{invoiceNumber}` | [`invoicetable.ts`](src/app/features/invoicemanagement/invoicetable/invoicetable.ts), [`invoiceform.ts`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts) |
| Invoice Item | `jewelleryManagement/admin/invoiceitem`, `jewelleryManagement/admin/invoiceitem/add`, `jewelleryManagement/admin/invoiceitem/edit/{id}` | [`invoiceitemtable.ts`](src/app/features/invoiceitemmanagement/invoiceitemtable/invoiceitemtable.ts), [`invoiceitemform.ts`](src/app/features/invoiceitemmanagement/invoiceitemform/invoiceitemform.ts) |
| Payment | `jewelleryManagement/admin/payment`, `jewelleryManagement/admin/payment/add`, `jewelleryManagement/admin/payment/edit/{id}` | [`paymenttable.ts`](src/app/features/paymentmanagement/paymenttable/paymenttable.ts), [`paymentform.ts`](src/app/features/paymentmanagement/paymentform/paymentform.ts) |
| Invoice Payment | `jewelleryManagement/admin/invoicepayment`, `jewelleryManagement/admin/invoicepayment/add`, `jewelleryManagement/admin/invoicepayment/edit/{id}` | [`invoicepaymenttable.ts`](src/app/features/invoicepaymentmanagement/invoicepaymenttable/invoicepaymenttable.ts), [`invoicepaymentform.ts`](src/app/features/invoicepaymentmanagement/invoicepaymentform/invoicepaymentform.ts) |
| Exchange | `jewelleryManagement/admin/exchange`, `jewelleryManagement/admin/exchange/add`, `jewelleryManagement/admin/exchange/edit/{id}`, `jewelleryManagement/admin/exchange/view/{id}` | [`exchangetable.ts`](src/app/features/exchangemanagement/exchangetable/exchangetable.ts), [`exchangeform.ts`](src/app/features/exchangemanagement/exchangeform/exchangeform.ts) |
| Metal Rate History | `jewelleryManagement/admin/metalratehistory`, `jewelleryManagement/admin/metalratehistory/add`, `jewelleryManagement/admin/metalratehistory/edit/{purityId}` | [`metalratehistorytable.ts`](src/app/features/metalratehistorymanagement/metalratehistorytable/metalratehistorytable.ts), [`metalratehistoryform.ts`](src/app/features/metalratehistorymanagement/metalratehistoryform/metalratehistoryform.ts) |
| Stone Rate History | `jewelleryManagement/admin/stoneratehistory`, `jewelleryManagement/admin/stoneratehistory/add`, `jewelleryManagement/admin/stoneratehistory/edit/{id}` | [`stoneratehistorytable.ts`](src/app/features/stoneratehistorymanagement/stoneratehistorytable/stoneratehistorytable.ts), [`stoneratehistoryform.ts`](src/app/features/stoneratehistorymanagement/stoneratehistoryform/stoneratehistoryform.ts) |
| Auth | `jewelleryManagement/auth/sign-in` | [`auth.service.ts`](src/app/core/services/auth.service.ts) |
| Analytics (Dashboard) | `jewelleryManagement/admin/analytics` | [`sign-in.component.ts`](src/app/demo/pages/authentication/sign-in/sign-in.component.ts) |

---

## 3. Toast/Notification Messages

### Success Messages
All success messages are hardcoded in service files:

| Message | File Location |
|---------|--------------|
| `'Login successful!'` | [`sign-in.component.ts:57`](src/app/demo/pages/authentication/sign-in/sign-in.component.ts:57) |
| `'You have been logged out successfully'` | [`auth.service.ts:66`](src/app/core/services/auth.service.ts:66) |
| `'Category created successfully'` | [`category.service.ts:159`](src/app/core/services/category.service.ts:159) |
| `'Category updated successfully'` | [`category.service.ts:185`](src/app/core/services/category.service.ts:185) |
| `'Category deleted successfully'` | [`category.service.ts:213`](src/app/core/services/category.service.ts:213) |
| `'Category activated successfully'` | [`category.service.ts:241`](src/app/core/services/category.service.ts:241) |
| `'Category deactivated successfully'` | [`category.service.ts:267`](src/app/core/services/category.service.ts:267) |
| `'Exchange order created successfully'` | [`exchange.service.ts:123`](src/app/core/services/exchange.service.ts:123) |
| `'Exchange order completed successfully'` | [`exchange.service.ts:144`](src/app/core/services/exchange.service.ts:144) |
| `'Exchange order cancelled successfully'` | [`exchange.service.ts:167`](src/app/core/services/exchange.service.ts:167) |
| `'Exchange value calculated successfully'` | [`exchangeform.ts:287`](src/app/features/exchangemanagement/exchangeform/exchangeform.ts:287) |
| `'Invoice generated successfully!'` | [`invoiceform.ts:202`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:202) |
| `'Invoice cancelled successfully'` | [`invoice.service.ts:145`](src/app/core/services/invoice.service.ts:145) |
| `'Invoice regenerated successfully'` | [`invoice-payment.service.ts:232`](src/app/core/services/invoice-payment.service.ts:232) |
| `'Jewellery item created successfully'` | [`jewellery-item.service.ts:68`](src/app/core/services/jewellery-item.service.ts:68) |
| `'Jewellery item updated successfully'` | [`jewellery-item.service.ts:89`](src/app/core/services/jewellery-item.service.ts:89) |
| `'Jewellery item deleted successfully'` | [`jewellery-item.service.ts:113`](src/app/core/services/jewellery-item.service.ts:113) |
| `'Metal created successfully'` | [`metal.service.ts:77`](src/app/core/services/metal.service.ts:77) |
| `'Metal updated successfully'` | [`metal.service.ts:103`](src/app/core/services/metal.service.ts:103) |
| `'Metal deleted successfully'` | [`metal.service.ts:131`](src/app/core/services/metal.service.ts:131) |
| `'Metal rate created successfully'` | [`metal-rate-history.service.ts:177`](src/app/core/services/metal-rate-history.service.ts:177) |
| `'Metal rate updated successfully'` | [`metal-rate-history.service.ts:199`](src/app/core/services/metal-rate-history.service.ts:199) |
| `'Payment created successfully'` | [`payment.service.ts:84`](src/app/core/services/payment.service.ts:84) |
| `'Payment updated successfully'` | [`payment.service.ts:105`](src/app/core/services/payment.service.ts:105) |
| `'Payment deleted successfully'` | [`payment.service.ts:129`](src/app/core/services/payment.service.ts:129) |
| `'Purity created successfully'` | [`purity.service.ts:71`](src/app/core/services/purity.service.ts:71) |
| `'Purity updated successfully'` | [`purity.service.ts:95`](src/app/core/services/purity.service.ts:95) |
| `'Purity deleted successfully'` | [`purity.service.ts:123`](src/app/core/services/purity.service.ts:123) |
| `'Role created successfully'` | [`role.service.ts:67`](src/app/core/services/role.service.ts:67) |
| `'Role updated successfully'` | [`role.service.ts:65`](src/app/core/services/role.service.ts:65) |
| `'Role deleted successfully'` | [`role.service.ts:91`](src/app/core/services/role.service.ts:91) |
| `'Sale order created successfully'` | [`sale-order.service.ts:68`](src/app/core/services/sale-order.service.ts:68) |
| `'Sale order updated successfully'` | [`sale-order.service.ts:89`](src/app/core/services/sale-order.service.ts:89) |
| `'Sale order deleted successfully'` | [`sale-order.service.ts:113`](src/app/core/services/sale-order.service.ts:113) |
| `'Sale order item created successfully'` | [`sale-order-item.service.ts:85`](src/app/core/services/sale-order-item.service.ts:85) |
| `'Sale order item created with automatic calculation'` | [`sale-order-item.service.ts:108`](src/app/core/services/sale-order-item.service.ts:108) |
| `'Sale order item updated successfully'` | [`sale-order-item.service.ts:131`](src/app/core/services/sale-order-item.service.ts:131) |
| `'Sale order item deleted successfully'` | [`sale-order-item.service.ts:155`](src/app/core/services/sale-order-item.service.ts:155) |
| `'Stone created successfully'` | [`stone.service.ts:102`](src/app/core/services/stone.service.ts:102) |
| `'Stone updated successfully'` | [`stone.service.ts:130`](src/app/core/services/stone.service.ts:130) |
| `'Stone deleted successfully'` | [`stone.service.ts:160`](src/app/core/services/stone.service.ts:160) |
| `'Stone rate created successfully'` | [`stone-rate-history.service.ts:220`](src/app/core/services/stone-rate-history.service.ts:220) |
| `'Stone rate updated successfully'` | [`stone-rate-history.service.ts:241`](src/app/core/services/stone-rate-history.service.ts:241) |
| `'Supplier created successfully'` | [`supplier.service.ts:81`](src/app/core/services/supplier.service.ts:81) |
| `'Supplier updated successfully'` | [`supplier.service.ts:103`](src/app/core/services/supplier.service.ts:103) |
| `'Supplier deleted successfully'` | [`supplier.service.ts:125`](src/app/core/services/supplier.service.ts:125) |
| `'User registered successfully'` | [`user.service.ts:58`](src/app/core/services/user.service.ts:58) |
| `'User updated successfully'` | [`user.service.ts:126`](src/app/core/services/user.service.ts:126) |
| `'User deleted successfully'` | [`user.service.ts:151`](src/app/core/services/user.service.ts:151) |
| `'Warehouse created successfully'` | [`warehouse.service.ts:64`](src/app/core/services/warehouse.service.ts:64) |
| `'Warehouse updated successfully'` | [`warehouse.service.ts:85`](src/app/core/services/warehouse.service.ts:85) |
| `'Warehouse deleted successfully'` | [`warehouse.service.ts:109`](src/app/core/services/warehouse.service.ts:109) |
| `'Item stock created successfully'` | [`item-stock.service.ts:93`](src/app/core/services/item-stock.service.ts:93) |
| `'Item stock updated successfully'` | [`item-stock.service.ts:114`](src/app/core/services/item-stock.service.ts:114) |
| `'Item stock deleted successfully'` | [`item-stock.service.ts:138`](src/app/core/services/item-stock.service.ts:138) |
| `'Invoice item created successfully'` | [`invoice-item.service.ts:84`](src/app/core/services/invoice-item.service.ts:84) |
| `'Invoice item updated successfully'` | [`invoice-item.service.ts:105`](src/app/core/services/invoice-item.service.ts:105) |
| `'Invoice item deleted successfully'` | [`invoice-item.service.ts:129`](src/app/core/services/invoice-item.service.ts:129) |
| `'IRN generated successfully'` | [`invoice.service.ts:186`](src/app/core/services/invoice.service.ts:186) |
| `'E-Invoice cancelled successfully'` | [`invoice.service.ts:208`](src/app/core/services/invoice.service.ts:208) |
| `'Synced with NIC portal successfully'` | [`invoice.service.ts:285`](src/app/core/services/invoice.service.ts:285) |

### Error Messages
All error messages are hardcoded:

| Message | File Location |
|---------|--------------|
| `'User not found'` | [`userform.ts:176`](src/app/features/usermanagement/userform/userform.ts:176) |
| `'Supplier not found'` | [`supplierform.ts:154`](src/app/features/suppliermanagement/supplierform/supplierform.ts:154) |
| `'Category not found'` | [`categoryform.ts:111`](src/app/features/categorymanagement/categoryform/categoryform.ts:111) |
| `'Role not found'` | [`roleform.ts:107`](src/app/features/rolemanagement/roleform/roleform.ts:107) |
| `'Stone not found'` | [`stoneform.ts:95`](src/app/features/stonemanagement/stoneform/stoneform.ts:95) |
| `'Metal not found'` | [`metalform.ts:90`](src/app/features/metalmanagement/metalform/metalform.ts:90) |
| `'Purity not found'` | [`purityform.ts:118`](src/app/features/puritymanagement/purityform/purityform.ts:118) |
| `'Jewellery item not found'` | [`jewelleryitemform.ts:230`](src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts:230) |
| `'Item stock not found'` | [`itemstockform.ts:142`](src/app/features/itemstockmanagement/itemstockform/itemstockform.ts:142) |
| `'Sale order not found'` | [`saleorderform.ts:114`](src/app/features/saleordermanagement/saleorderform/saleorderform.ts:114) |
| `'Sale order item not found'` | [`saleorderitemform.ts:194`](src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts:194) |
| `'Invoice not found'` | [`invoiceform.ts:108`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:108) |
| `'Invoice item not found'` | [`invoiceitemform.ts:176`](src/app/features/invoiceitemmanagement/invoiceitemform/invoiceitemform.ts:176) |
| `'Payment not found'` | [`paymentform.ts:134`](src/app/features/paymentmanagement/paymentform/paymentform.ts:134) |
| `'Exchange order not found'` | [`exchangeform.ts:202`](src/app/features/exchangemanagement/exchangeform/exchangeform.ts:202) |
| `'Metal rate not found'` | [`metalratehistoryform.ts:185`](src/app/features/metalratehistorymanagement/metalratehistoryform/metalratehistoryform.ts:185) |
| `'Stone rate not found'` | [`stoneratehistoryform.ts:186`](src/app/features/stoneratehistorymanagement/stoneratehistoryform/stoneratehistoryform.ts:186) |
| `'Invalid email/phone number or password.'` | [`sign-in.component.ts:73`](src/app/demo/pages/authentication/sign-in/sign-in.component.ts:73) |
| `'Login failed. Please try again.'` | [`sign-in.component.ts:75`](src/app/demo/pages/authentication/sign-in/sign-in.component.ts:75) |
| `'Authentication failed. Please try again.'` | [`sign-in.component.ts:65`](src/app/demo/pages/authentication/sign-in/sign-in.component.ts:65) |
| `'Your session has expired. Please log in again.'` | [`auth.service.ts:138`](src/app/core/services/auth.service.ts:138) |
| `'Failed to load categories'` | [`category.service.ts:51`](src/app/core/services/category.service.ts:51) |
| `'Failed to load exchange orders'` | [`exchange.service.ts:54`](src/app/core/services/exchange.service.ts:54) |
| `'Failed to load jewellery items'` | [`jewellery-item.service.ts:35`](src/app/core/services/jewellery-item.service.ts:35) |
| `'Failed to load metal rates'` | [`metal-rate-history.service.ts:44`](src/app/core/services/metal-rate-history.service.ts:44) |
| `'Failed to load payments'` | [`payment.service.ts:35`](src/app/core/services/payment.service.ts:35) |
| `'Failed to load purities'` | [`purity.service.ts:34`](src/app/core/services/purity.service.ts:34) |
| `'Failed to load roles'` | [`role.service.ts:31`](src/app/core/services/role.service.ts:31) |
| `'Failed to load sale orders'` | [`sale-order.service.ts:35`](src/app/core/services/sale-order.service.ts:35) |
| `'Failed to load sale order items'` | [`sale-order-item.service.ts:36`](src/app/core/services/sale-order-item.service.ts:36) |
| `'Failed to load stones'` | [`stone.service.ts:40`](src/app/core/services/stone.service.ts:40) |
| `'Failed to load stone rates'` | [`stone-rate-history.service.ts:41`](src/app/core/services/stone-rate-history.service.ts:41) |
| `'Failed to load suppliers'` | [`supplier.service.ts:40`](src/app/core/services/supplier.service.ts:40) |
| `'Failed to load users'` | [`user.service.ts:92`](src/app/core/services/user.service.ts:92) |
| `'Failed to load warehouses'` | [`warehouse.service.ts:31`](src/app/core/services/warehouse.service.ts:31) |
| `'Failed to load item stocks'` | [`item-stock.service.ts:38`](src/app/core/services/item-stock.service.ts:38) |
| `'Failed to load invoice items'` | [`invoice-item.service.ts:35`](src/app/core/services/invoice-item.service.ts:35) |
| `'Failed to load invoices'` | [`invoice.service.ts:66`](src/app/core/services/invoice.service.ts:66) |
| `'Failed to create category'` | [`category.service.ts:168`](src/app/core/services/category.service.ts:168) |
| `'Failed to update category'` | [`category.service.ts:196`](src/app/core/services/category.service.ts:196) |
| `'Failed to delete category'` | [`category.service.ts:224`](src/app/core/services/category.service.ts:224) |
| `'Failed to activate category'` | [`category.service.ts:250`](src/app/core/services/category.service.ts:250) |
| `'Failed to deactivate category'` | [`category.service.ts:276`](src/app/core/services/category.service.ts:276) |
| `'Failed to create exchange order'` | [`exchange.service.ts:130`](src/app/core/services/exchange.service.ts:130) |
| `'Failed to complete exchange order'` | [`exchange.service.ts:153`](src/app/core/services/exchange.service.ts:153) |
| `'Failed to cancel exchange order'` | [`exchange.service.ts:176`](src/app/core/services/exchange.service.ts:176) |
| `'Failed to generate invoice'` | [`invoice.service.ts:46`](src/app/core/services/invoice.service.ts:46) |
| `'Failed to generate bulk invoices'` | [`invoice.service.ts:110`](src/app/core/services/invoice.service.ts:110) |
| `'Failed to regenerate invoice'` | [`invoice.service.ts:131`](src/app/core/services/invoice.service.ts:131) |
| `'Failed to cancel invoice'` | [`invoice.service.ts:152`](src/app/core/services/invoice.service.ts:152) |
| `'Failed to convert number to words'` | [`invoice.service.ts:171`](src/app/core/services/invoice.service.ts:171) |
| `'Failed to generate IRN'` | [`invoice.service.ts:193`](src/app/core/services/invoice.service.ts:193) |
| `'Failed to cancel e-invoice'` | [`invoice.service.ts:215`](src/app/core/services/invoice.service.ts:215) |
| `'Failed to get IRN details'` | [`invoice.service.ts:235`](src/app/core/services/invoice.service.ts:235) |
| `'Failed to generate QR code'` | [`invoice.service.ts:255`](src/app/core/services/invoice.service.ts:255) |
| `'Failed to check e-invoice eligibility'` | [`invoice.service.ts:272`](src/app/core/services/invoice.service.ts:272) |
| `'Failed to sync with NIC portal'` | [`invoice.service.ts:292`](src/app/core/services/invoice.service.ts:292) |
| `'Role name already exists'` | [`role.service.ts:73`](src/app/core/services/role.service.ts:73) |
| `'A stone with this name already exists'` | [`stone.service.ts:109`](src/app/core/services/stone.service.ts:109) |
| `'GST number already exists'` | [`supplier.service.ts:150`](src/app/core/services/supplier.service.ts:150) |
| `'TAN number already exists'` | [`supplier.service.ts:152`](src/app/core/services/supplier.service.ts:152) |
| `'Email already registered'` | [`user.service.ts:65`](src/app/core/services/user.service.ts:65) |
| `'Contact number already registered'` | [`user.service.ts:67`](src/app/core/services/user.service.ts:67) |
| `'Invalid email or phone number'` | [`user.service.ts:38`](src/app/core/services/user.service.ts:38) |
| `'Invalid password'` | [`user.service.ts:40`](src/app/core/services/user.service.ts:40) |
| `'User is inactive'` | [`user.service.ts:42`](src/app/core/services/user.service.ts:42) |
| `'Insufficient stock available for the selected item'` | [`saleorderitemform.ts:247`](src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts:247) |
| `'An error occurred. Please try again.'` | Multiple files |

### Warning Messages
| Message | File Location |
|---------|--------------|
| `'Please enter an invoice number to search'` | [`invoicetable.ts:66`](src/app/features/invoicemanagement/invoicetable/invoicetable.ts:66) |
| `'No invoice found with this number'` | [`invoicetable.ts:77`](src/app/features/invoicemanagement/invoicetable/invoicetable.ts:77) |
| `'Please enter a Sale Order ID'` | [`invoiceform.ts:126`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:126) |
| `'Sale order not found'` | [`invoiceform.ts:137`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:137) |
| `'Please provide a reason for cancellation'` | [`invoiceform.ts:339`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:339) |
| `'At least one item is required'` | [`exchangeform.ts:137`](src/app/features/exchangemanagement/exchangeform/exchangeform.ts:137) |
| `'Exchange order not found'` | [`exchange.service.ts:71`](src/app/core/services/exchange.service.ts:71) |
| `'Category not found'` | [`category.service.ts:73`](src/app/core/services/category.service.ts:73) |
| `'Metal rate not found for this purity'` | [`metal-rate-history.service.ts:66`](src/app/core/services/metal-rate-history.service.ts:66) |
| `'No stone rate found for the given criteria'` | [`stone-rate-history.service.ts:88`](src/app/core/services/stone-rate-history.service.ts:88) |
| `'No diamond rate found for the given 4Cs'` | [`stone-rate-history.service.ts:124`](src/app/core/services/stone-rate-history.service.ts:124) |
| `'Invoice not found'` | [`invoice.service.ts:64`](src/app/core/services/invoice.service.ts:64) |
| `'Invoice not found for this sale order'` | [`invoice.service.ts:84`](src/app/core/services/invoice.service.ts:84) |
| `'IRN not found'` | [`invoice.service.ts:233`](src/app/core/services/invoice.service.ts:233) |

---

## 4. HTTP Status Codes

HTTP status codes are hardcoded throughout the services:

| Status Code | Usage | File Locations |
|-------------|-------|----------------|
| `200` | Success response check | Multiple service files |
| `204` | No content success | Multiple service files |
| `400` | Bad request/validation error | Multiple service files |
| `401` | Unauthorized | [`error.interceptor.ts:49`](src/app/core/interceptors/error.interceptor.ts:49), [`user.service.ts:39`](src/app/core/services/user.service.ts:39) |
| `403` | Forbidden | [`error.interceptor.ts:54`](src/app/core/interceptors/error.interceptor.ts:54), [`user.service.ts:41`](src/app/core/services/user.service.ts:41) |
| `404` | Not found | Multiple service files |
| `409` | Conflict/duplicate | Multiple service files |
| `422` | Unprocessable entity | [`error.interceptor.ts:71`](src/app/core/interceptors/error.interceptor.ts:71) |
| `500` | Server error | [`error.interceptor.ts:75`](src/app/core/interceptors/error.interceptor.ts:75) |

**File:** [`src/app/core/interceptors/error.interceptor.ts`](src/app/core/interceptors/error.interceptor.ts)
```typescript
case 400:
  errorMessage = 'Bad Request: Invalid data provided.';
case 401:
  errorMessage = 'Unauthorized';
case 403:
  errorMessage = 'Forbidden: You do not have permission to access this resource.';
case 404:
  errorMessage = 'Not Found: The requested resource was not found.';
case 409:
  errorMessage = 'The resource already exists.';
case 422:
  errorMessage = 'User inactive Please contact admin ';
case 500:
  errorMessage = 'Server error: Please try again later.';
```

---

## 5. Numeric Constants

### Validation Constraints

| Constant | Value | Usage | File Location |
|----------|-------|-------|---------------|
| Min Name Length | `2` | Category, Stone, Purity name validation | [`categoryform.ts:65`](src/app/features/categorymanagement/categoryform/categoryform.ts:65), [`stoneform.ts:63`](src/app/features/stonemanagement/stoneform/stoneform.ts:63), [`purityform.ts:74`](src/app/features/puritymanagement/purityform/purityform.ts:74) |
| Max Name Length | `100` | Category, Stone, Supplier name validation | Multiple form files |
| Max Description Length | `500` | Category description | [`categoryform.ts:66`](src/app/features/categorymanagement/categoryform/categoryform.ts:66) |
| Max Description Length | `1000` | Jewellery item description | [`jewelleryitemform.ts:121`](src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts:121) |
| Min Supplier Name Length | `3` | Supplier name validation | [`supplierform.ts:95`](src/app/features/suppliermanagement/supplierform/supplierform.ts:95) |
| Max Address Length | `255` | Address validation | [`supplierform.ts:99`](src/app/features/suppliermanagement/supplierform/supplierform.ts:99) |
| Phone Pattern | `^\d{10,15}$` | Phone number validation (10-15 digits) | [`supplierform.ts:98`](src/app/features/suppliermanagement/supplierform/supplierform.ts:98) |
| Max GST/TAN Length | `15` | GST/TAN number validation | [`supplierform.ts:100`](src/app/features/suppliermanagement/supplierform/supplierform.ts:100) |
| Max Percentage | `100` | Purity, Wastage, GST percentage | Multiple form files |
| Min Weight | `0.001` | Gross/Net weight validation | [`jewelleryitemform.ts:127-128`](src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts:127) |
| Default GST | `3` | Default GST percentage | [`saleorderitemform.ts:110`](src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts:110) |
| E-Invoice Threshold | `500000` | Grand total threshold for e-invoice | [`invoiceform.ts:390`](src/app/features/invoicemanagement/invoiceform/invoiceform.ts:390) |
| TCS Threshold | `1000000` | ₹10 lakh TCS threshold | [`tcs.model.ts:244`](src/app/core/models/tcs.model.ts:244) |

### UI/Animation Constants

| Constant | Value | Usage | File Location |
|----------|-------|-------|---------------|
| Toast Timeout | `3000` | Toast notification duration (ms) | [`app.config.ts:67`](src/app/app.config.ts:67) |
| Animation Duration | `300` | Slide animation duration (ms) | [`nav-right.component.ts:20-25`](src/app/theme/layout/admin/nav-bar/nav-right/nav-right.component.ts:20) |
| Mobile Breakpoint | `992` | Responsive breakpoint (px) | [`navigation.component.ts:26`](src/app/theme/layout/admin/navigation/navigation.component.ts:26), [`nav-bar.component.ts:40`](src/app/theme/layout/admin/nav-bar/nav-bar.component.ts:40) |
| Menu Animation Delay | `500` | Menu collapse delay (ms) | [`nav-content.component.ts:51`](src/app/theme/layout/admin/navigation/nav-content/nav-content.component.ts:51) |
| Menu Toggle Delay | `100` | Mobile menu toggle delay (ms) | [`admin.component.ts:52`](src/app/theme/layout/admin/admin.component.ts:52) |

### Role Enum

**File:** [`src/app/core/enums/role.enum.ts`](src/app/core/enums/role.enum.ts)
```typescript
export enum RoleEnum {
  SuperAdmin = 1,
}
```

### Status Enum

**File:** [`src/app/core/enums/generic-status.enums.ts`](src/app/core/enums/generic-status.enums.ts)
```typescript
export enum GenericStatus {
  Inactive = 0,
  Active = 1,
  Deleted = 3
}
```

---

## 6. UI Labels and Placeholders

### Card Titles
All card titles in HTML templates are hardcoded:

| Title | File Location |
|-------|---------------|
| `'User Management'` | [`usertable.html:1`](src/app/features/usermanagement/usertable/usertable.html:1) |
| `'Role Management'` | [`roletable.html:1`](src/app/features/rolemanagement/roletable/roletable.html:1) |
| `'Category Management'` | [`categorytable.html:1`](src/app/features/categorymanagement/categorytable/categorytable.html:1) |
| `'Supplier Management'` | [`suppliertable.html:1`](src/app/features/suppliermanagement/suppliertable/suppliertable.html:1) |
| `'Stone Management'` | [`stonetable.html:1`](src/app/features/stonemanagement/stonetable/stonetable.html:1) |
| `'Metal Management'` | [`metaltable.html:1`](src/app/features/metalmanagement/metaltable/metaltable.html:1) |
| `'Purity Management'` | [`puritytable.html:1`](src/app/features/puritymanagement/puritytable/puritytable.html:1) |
| `'Jewellery Item Management'` | [`jewelleryitemtable.html:1`](src/app/features/jewelleryitemmanagement/jewelleryitemtable/jewelleryitemtable.html:1) |
| `'Item Stock Management'` | [`itemstocktable.html:1`](src/app/features/itemstockmanagement/itemstocktable/itemstocktable.html:1) |
| `'Sale Order Management'` | [`saleordertable.html:1`](src/app/features/saleordermanagement/saleordertable/saleordertable.html:1) |
| `'Sale Order Item Management'` | [`saleorderitemtable.html:1`](src/app/features/saleorderitemmanagement/saleorderitemtable/saleorderitemtable.html:1) |
| `'Invoice Management'` | [`invoicetable.html:1`](src/app/features/invoicemanagement/invoicetable/invoicetable.html:1) |
| `'Invoice Item Management'` | [`invoiceitemtable.html:1`](src/app/features/invoiceitemmanagement/invoiceitemtable/invoiceitemtable.html:1) |
| `'Payment Management'` | [`paymenttable.html:1`](src/app/features/paymentmanagement/paymenttable/paymenttable.html:1), [`invoicepaymenttable.html:1`](src/app/features/invoicepaymentmanagement/invoicepaymenttable/invoicepaymenttable.html:1) |
| `'Exchange Order Management'` | [`exchangetable.html:1`](src/app/features/exchangemanagement/exchangetable/exchangetable.html:1) |
| `'Metal Rate History Management'` | [`metalratehistorytable.html:1`](src/app/features/metalratehistorymanagement/metalratehistorytable/metalratehistorytable.html:1) |
| `'Stone Rate History Management'` | [`stoneratehistorytable.html:1`](src/app/features/stoneratehistorymanagement/stoneratehistorytable/stoneratehistorytable.html:1) |

### Form Titles (Dynamic)
```html
{{ isEditMode ? 'Edit User' : 'Add User' }}
{{ isViewMode ? 'View User' : (isEditMode ? 'Edit User' : 'Add User') }}
{{ isEditMode ? 'Edit Payment' : 'Add Payment' }}
```

### Empty State Messages
| Message | File Location |
|---------|---------------|
| `'No users found. Click "Add User" to create one.'` | [`usertable.html:41`](src/app/features/usermanagement/usertable/usertable.html:41) |
| `'No roles found. Click "Add Role" to create one.'` | [`roletable.html:38`](src/app/features/rolemanagement/roletable/roletable.html:38) |
| `'No categories found. Click "Add Category" to create one.'` | [`categorytable.html:39`](src/app/features/categorymanagement/categorytable/categorytable.html:39) |
| `'No suppliers found. Click "Add Supplier" to create one.'` | [`suppliertable.html:66`](src/app/features/suppliermanagement/suppliertable/suppliertable.html:66) |
| `'No stones found. Click "Add Stone" to create one.'` | [`stonetable.html:62`](src/app/features/stonemanagement/stonetable/stonetable.html:62) |
| `'No metals found. Click "Add Metal" to create one.'` | [`metaltable.html:38`](src/app/features/metalmanagement/metaltable/metaltable.html:38) |
| `'No purities found. Click "Add Purity" to create one.'` | [`puritytable.html:39`](src/app/features/puritymanagement/puritytable/puritytable.html:39) |
| `'No jewellery items found. Click "Add Jewellery Item" to create one.'` | [`jewelleryitemtable.html:45`](src/app/features/jewelleryitemmanagement/jewelleryitemtable/jewelleryitemtable.html:45) |
| `'No item stocks found. Click "Add Item Stock" to create one.'` | [`itemstocktable.html:42`](src/app/features/itemstockmanagement/itemstocktable/itemstocktable.html:42) |
| `'No sale orders found. Click "Add Sale Order" to create one.'` | [`saleordertable.html:42`](src/app/features/saleordermanagement/saleordertable/saleordertable.html:42) |
| `'No sale order items found. Click "Add Sale Order Item" to create one.'` | [`saleorderitemtable.html:43`](src/app/features/saleorderitemmanagement/saleorderitemtable/saleorderitemtable.html:43) |
| `'No invoices found. Search by invoice number or click "Generate Invoice" to create one.'` | [`invoicetable.html:64`](src/app/features/invoicemanagement/invoicetable/invoicetable.html:64) |
| `'No invoice items found. Click "Add Invoice Item" to create one.'` | [`invoiceitemtable.html:43`](src/app/features/invoiceitemmanagement/invoiceitemtable/invoiceitemtable.html:43) |
| `'No payments found. Click "Add Payment" to create one.'` | [`paymenttable.html:43`](src/app/features/paymentmanagement/paymenttable/paymenttable.html:43) |
| `'No exchange orders found. Click "Add Exchange Order" to create one.'` | [`exchangetable.html:42`](src/app/features/exchangemanagement/exchangetable/exchangetable.html:42) |
| `'No metal rates found. Click "Add Metal Rate" to create one.'` | [`metalratehistorytable.html:138`](src/app/features/metalratehistorymanagement/metalratehistorytable/metalratehistorytable.html:138) |
| `'No stone rates found. Click "Add Stone Rate" to create one.'` | [`stoneratehistorytable.html:68`](src/app/features/stoneratehistorymanagement/stoneratehistorytable/stoneratehistorytable.html:68) |

### Input Placeholders
All input placeholders are hardcoded in HTML templates. Examples:

| Placeholder | File Location |
|-------------|---------------|
| `'Enter full name'` | [`userform.html:102`](src/app/features/usermanagement/userform/userform.html:102) |
| `'Enter email address'` | [`userform.html:122`](src/app/features/usermanagement/userform/userform.html:122) |
| `'Enter password'` | [`userform.html:143`](src/app/features/usermanagement/userform/userform.html:143) |
| `'Enter 10-digit contact number'` | [`userform.html:165`](src/app/features/usermanagement/userform/userform.html:165) |
| `'Search by name, GST, TAN...'` | [`suppliertable.html:11`](src/app/features/suppliermanagement/suppliertable/suppliertable.html:11) |
| `'Enter supplier name (min 3 characters)'` | [`supplierform.html:26`](src/app/features/suppliermanagement/supplierform/supplierform.html:26) |
| `'Enter Invoice Number to search...'` | [`invoicetable.html:17`](src/app/features/invoicemanagement/invoicetable/invoicetable.html:17) |
| `'Enter Sale Order ID'` | [`invoiceform.html:323`](src/app/features/invoicemanagement/invoiceform/invoiceform.html:323) |
| `'Search here'` | [`nav-search.component.html:4`](src/app/theme/layout/admin/nav-bar/nav-left/nav-search/nav-search.component.html:4) |
| `'Email Address or Phone Number'` | [`sign-in.component.html:19`](src/app/demo/pages/authentication/sign-in/sign-in.component.html:19) |
| `'Enter your password'` | [`sign-in.component.html:37`](src/app/demo/pages/authentication/sign-in/sign-in.component.html:37) |

---

## 7. Validation Messages

### Form Validation Error Messages

**File:** [`src/app/features/categorymanagement/categoryform/categoryform.ts`](src/app/features/categorymanagement/categoryform/categoryform.ts)
```typescript
validationMessages = {
  name: {
    required: 'Name is required',
    minlength: 'Name must be at least 2 characters',
    maxlength: 'Name cannot exceed 100 characters',
  },
  description: {
    maxlength: 'Description cannot exceed 500 characters',
  },
}
```

**File:** [`src/app/features/stonemanagement/stoneform/stoneform.ts`](src/app/features/stonemanagement/stoneform/stoneform.ts)
```typescript
validationMessages = {
  name: {
    required: 'Name is required',
    minlength: 'Name must be at least 2 characters',
    maxlength: 'Name cannot exceed 100 characters',
  },
}
```

**File:** [`src/app/features/suppliermanagement/supplierform/supplierform.ts`](src/app/features/suppliermanagement/supplierform/supplierform.ts)
```typescript
validationMessages = {
  name: {
    required: 'Name is required',
    minlength: 'Name must be at least 3 characters',
    maxlength: 'Name cannot exceed 100 characters',
  },
  contactPerson: {
    maxlength: 'Contact person name cannot exceed 100 characters',
  },
  address: {
    maxlength: 'Address cannot exceed 255 characters',
  },
}
```

**File:** [`src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts`](src/app/features/jewelleryitemmanagement/jewelleryitemform/jewelleryitemform.ts)
```typescript
validationMessages = {
  name: {
    required: 'Name is required',
    minlength: 'Name must be at least 2 characters',
    maxlength: 'Name cannot exceed 200 characters',
  },
  description: {
    maxlength: 'Description cannot exceed 1000 characters',
  },
  wastagePercentage: {
    required: 'Wastage percentage is required',
    min: 'Wastage percentage must be 0 or greater',
    max: 'Wastage percentage cannot exceed 100',
  },
}
```

**File:** [`src/app/features/puritymanagement/purityform/purityform.ts`](src/app/features/puritymanagement/purityform/purityform.ts)
```typescript
validationMessages = {
  name: {
    required: 'Name is required',
    minlength: 'Name must be at least 2 characters',
    maxlength: 'Name cannot exceed 50 characters',
  },
  percentage: {
    required: 'Percentage is required',
    min: 'Percentage must be at least 0',
    max: 'Percentage cannot exceed 100',
  },
}
```

**File:** [`src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts`](src/app/features/saleorderitemmanagement/saleorderitemform/saleorderitemform.ts)
```typescript
validationMessages = {
  gstPercentage: {
    required: 'GST Percentage is required',
    min: 'GST Percentage cannot be negative',
    max: 'GST Percentage cannot exceed 100',
  },
}
```

---

## 8. Configuration Values

### Application Configuration

**File:** [`src/app/app.config.ts`](src/app/app.config.ts)
```typescript
provideToastr({
  timeOut: 3000,
  positionClass: 'toast-top-right',
})
```

### Spinner Configuration

**File:** [`src/app/theme/shared/components/spinner/spinner.component.ts`](src/app/theme/shared/components/spinner/spinner.component.ts)
```typescript
backgroundColor = input('#2689E2');
spinner = input(Spinkit.skLine);
```

### Application Metadata

**File:** [`src/index.html`](src/index.html)
```html
<title>Jewellery Management</title>
<meta name="description" title="Jewellery Management" content="Jewellery Management 21 built with Bootstrap 5..." />
<meta name="author" content="Jewellery Management" />
```

---

## 9. Claim Types and JWT Constants

**File:** [`src/app/common/claim-types.ts`](src/app/common/claim-types.ts)
```typescript
export const ClaimTypes = {
  NAME_IDENTIFIER:
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
  NAME: 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name',
  EXPIRATION: 'exp',
} as const;

export interface CustomJwtPayload extends JwtPayload {
  [ClaimTypes.NAME_IDENTIFIER]: string;
  [ClaimTypes.NAME]: string;
  [ClaimTypes.EXPIRATION]: number;
  RoleId: string;
}
```

---

## 10. Recommendations

### 1. Create a Constants File Structure
```
src/app/core/constants/
├── api-endpoints.ts      # All API endpoint paths
├── routes.ts             # All route paths
├── messages.ts           # All toast/notification messages
├── validation.ts         # All validation constraints
├── http-status.ts        # HTTP status code constants
└── app-config.ts         # Application configuration
```

### 2. Example Implementation

**api-endpoints.ts:**
```typescript
export const API_ENDPOINTS = {
  USER: {
    BASE: '/User',
    LOGIN: '/User/login',
    REGISTER: '/User/register',
    GET_ALL: '/User/GetAllUsers',
    GET_BY_ID: '/User/GetUserById',
    CHECK_EMAIL: '/User/checkEmail',
    CHECK_CONTACT: '/User/checkContact',
  },
  CATEGORY: {
    BASE: '/Category',
    BY_NAME: '/Category/name',
    ACTIVE: '/Category/active',
    BY_PARENT: '/Category/parent',
  },
  // ... other endpoints
} as const;
```

**routes.ts:**
```typescript
export const APP_ROUTES = {
  BASE: 'jewelleryManagement',
  ADMIN: 'admin',
  MODULES: {
    USER: 'user',
    ROLE: 'role',
    CATEGORY: 'category',
    // ... other modules
  },
} as const;

// Helper function to generate routes
export const getRoute = (module: string, action?: string, id?: number | string) => {
  let route = `${APP_ROUTES.BASE}/${APP_ROUTES.ADMIN}/${module}`;
  if (action) route += `/${action}`;
  if (id !== undefined) route += `/${id}`;
  return route;
};
```

**messages.ts:**
```typescript
export const MESSAGES = {
  SUCCESS: {
    LOGIN: 'Login successful!',
    LOGOUT: 'You have been logged out successfully',
    CREATED: (entity: string) => `${entity} created successfully`,
    UPDATED: (entity: string) => `${entity} updated successfully`,
    DELETED: (entity: string) => `${entity} deleted successfully`,
  },
  ERROR: {
    NOT_FOUND: (entity: string) => `${entity} not found`,
    FAILED_LOAD: (entity: string) => `Failed to load ${entity}`,
    FAILED_CREATE: (entity: string) => `Failed to create ${entity}`,
    FAILED_UPDATE: (entity: string) => `Failed to update ${entity}`,
    FAILED_DELETE: (entity: string) => `Failed to delete ${entity}`,
    UNAUTHORIZED: 'Unauthorized',
    FORBIDDEN: 'Forbidden: You do not have permission to access this resource.',
    SERVER_ERROR: 'Server error: Please try again later.',
  },
  WARNING: {
    SESSION_EXPIRED: 'Your session has expired. Please log in again.',
    NO_RESULTS: (entity: string) => `No ${entity} found.`,
  },
} as const;
```

**validation.ts:**
```typescript
export const VALIDATION_CONSTANTS = {
  NAME: {
    MIN_LENGTH: 2,
    MAX_LENGTH: 100,
  },
  DESCRIPTION: {
    MAX_LENGTH: 500,
  },
  PHONE: {
    PATTERN: /^\d{10,15}$/,
    MIN_LENGTH: 10,
    MAX_LENGTH: 15,
  },
  PERCENTAGE: {
    MIN: 0,
    MAX: 100,
  },
  WEIGHT: {
    MIN: 0.001,
  },
  GST: {
    DEFAULT: 3,
  },
} as const;
```

### 3. Use i18n for Internationalization
Consider using Angular's i18n or a third-party library like `ngx-translate` for all UI labels, placeholders, and messages to support multiple languages.

### 4. Environment-Specific Configuration
Move all environment-specific values to environment files:
```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7192/api',
  toastTimeout: 3000,
  mobileBreakpoint: 992,
  eInvoiceThreshold: 500000,
  tcsThreshold: 1000000,
};
```

---

## Summary Statistics

| Category | Count |
|----------|-------|
| API Endpoints | ~80+ |
| Route Paths | ~130+ |
| Toast Messages | ~238+ |
| HTTP Status Codes | 9 |
| Numeric Constants | ~30+ |
| UI Labels/Placeholders | ~249+ |
| Validation Messages | ~50+ |

---

*Document generated on: February 22, 2026*
*Last updated: February 22, 2026*
