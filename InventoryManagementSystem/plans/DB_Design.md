# Database Design for Jewellery Inventory Management System

## Existing Tables

### Core Entities
- **UserDb**: Users of the system (Id, Name, Email, Password, ContactNumber, Gender, Address, DOB, RoleId, statusId, KYCId, CreatedBy, UpdatedBy, CreatedDate, UpdatedDate, ProfileImage)
- **UserKyc**: KYC details for users (Id, UserId, PanCardNumber, AadhaarCardNumber, IsVerified, VerificationStatus, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId) - Required for transactions above 2 lakh rupees in cash or card.
- **RoleDb**: User roles (Id, Name, Status, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate)
- **StatusDb**: Statuses (Id, Name, Status, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate)

### Jewellery Related
- **Category**: Item categories (Id, Name, Description, CreatedBy, UpdatedBy, CreatedDate, UpdatedDate, StatusId)
- **JewelleryItemDb**: Jewellery items (Id, ItemCode, Name, Description, CategoryId, HasStone, StoneId, MakingCharges, Wastage, IsHallmarked, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **MetalDb**: Metals (Id, Name, Description, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **PurityDb**: Metal purities (Id, MetalId, Name, Percentage, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **StoneDb**: Stones (Id, Name, Unit, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **ItemMetalDb**: Association between items and metals (Id, JewelleryItemId, MetalId, PurityId, GrossWeight, NetWeight, RatePerGram, Amount, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **ItemStoneDb**: Association between items and stones (Id, ItemId, StoneId, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId) - Note: Added audit fields
- **GoldRateHistory**: Historical gold rates (Id, PurityId, RatePerGram, EffectiveDate, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)

## Proposed Additional Tables for Complete Transactions

### Suppliers
- **Supplier**: Suppliers for purchasing jewellery (Id, Name, ContactPerson, Email, Phone, Address, GSTNumber, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- Customers are managed via the UserDb table with appropriate roles.

### Purchase Transactions
- **PurchaseOrder**: Purchase orders (Id, SupplierId, OrderNumber, OrderDate, ExpectedDeliveryDate, TotalAmount, StatusId, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate)
- **PurchaseOrderItem**: Items in purchase orders (Id, PurchaseOrderId, JewelleryItemId, Quantity, UnitPrice, TotalPrice, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)

### Sale Transactions
- **SaleOrder**: Sale orders (Id, CustomerId (references UserDb.Id), OrderNumber, OrderDate, DeliveryDate, TotalAmount, Discount, Tax, NetAmount, StatusId, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate)
- **SaleOrderItem**: Items in sale orders (Id, SaleOrderId, JewelleryItemId, Quantity, UnitPrice, Discount, TotalPrice, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)

### Inventory Management
- **Warehouse**: Storage locations (Id, Name, Address, ManagerId, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **ItemStock**: Stock levels (Id, JewelleryItemId, WarehouseId, Quantity, ReservedQuantity, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)
- **InventoryTransaction**: Stock movements (Id, JewelleryItemId, WarehouseId, TransactionType, Quantity, ReferenceId, ReferenceType, TransactionDate, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)

### Payments
- **Payment**: Payments for orders (Id, OrderId, OrderType, Amount, PaymentMethod, PaymentDate, ReferenceNumber, CreatedDate, CreatedBy, UpdatedBy, UpdatedDate, StatusId)

## Entity Relationships

- User -> Role (many-to-one)
- User -> Status (many-to-one)
- UserKyc -> User (many-to-one)
- UserKyc -> Status (many-to-one)
- Category -> Status (many-to-one)
- JewelleryItem -> Category (many-to-one)
- JewelleryItem -> Stone (many-to-one, optional)
- Metal -> Status (many-to-one)
- Purity -> Metal (many-to-one)
- Purity -> Status (many-to-one)
- Stone -> Status (many-to-one)
- ItemMetal -> JewelleryItem (many-to-one)
- ItemMetal -> Metal (many-to-one)
- ItemMetal -> Purity (many-to-one)
- ItemStone -> JewelleryItem (many-to-one)
- ItemStone -> Stone (many-to-one)
- GoldRateHistory -> Purity (many-to-one)
- Supplier -> Status (many-to-one)
- PurchaseOrder -> Supplier (many-to-one)
- PurchaseOrder -> Status (many-to-one)
- PurchaseOrderItem -> PurchaseOrder (many-to-one)
- PurchaseOrderItem -> JewelleryItem (many-to-one)
- SaleOrder -> UserDb (Customer, many-to-one)
- SaleOrder -> Status (many-to-one)
- SaleOrderItem -> SaleOrder (many-to-one)
- SaleOrderItem -> JewelleryItem (many-to-one)
- Warehouse -> User (Manager, many-to-one)
- Warehouse -> Status (many-to-one)
- ItemStock -> JewelleryItem (many-to-one)
- ItemStock -> Warehouse (many-to-one)
- InventoryTransaction -> JewelleryItem (many-to-one)
- InventoryTransaction -> Warehouse (many-to-one)
- Payment -> Order (polymorphic, depending on OrderType)

## Notes
- All tables include audit fields: CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, StatusId
- StatusId references StatusDb
- TransactionType in InventoryTransaction: 'IN' for stock in, 'OUT' for stock out, 'ADJUST' for adjustments
- OrderType in Payment: 'PURCHASE' or 'SALE'
- Quantity in items can be 1 for unique pieces or more for bulk
- For jewellery, items are often unique, so stock tracking is per item