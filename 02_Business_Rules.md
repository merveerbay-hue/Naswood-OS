
# Business Rules
**Project:** Naswood OS  
**Version:** 1.0  
**Status:** Draft  
**Last Updated:** 2026-08-01

---

# 1. Purpose

This document defines the business rules that govern warehouse and inventory operations in Naswood OS.

These rules are independent of software implementation and represent how the factory operates.

---

# 2. Core Principles

- Every physical movement must have a digital record.
- No stock can exist without a defined warehouse location.
- Every product must have a unique identity.
- Stock quantities can never become negative unless explicitly authorized.
- All inventory movements are traceable and auditable.
- Historical records are immutable.

---

# 3. Warehouse Structure

The system supports multiple warehouses.

Initial warehouses include:

- Raw Log Yard
- Sawmill Warehouse
- Kiln Warehouse
- Dry Lumber Warehouse
- Thermowood Warehouse
- Panel Warehouse
- Finished Goods Warehouse
- Pellet Warehouse
- Spare Parts Warehouse
- Scrap Warehouse

Each warehouse has independent inventory.

---

# 4. Location Rules

Every warehouse contains locations.

Location format:

Warehouse → Aisle → Rack → Shelf → Position

Example:

```
THM-A-03-B-12
```

Rules:

- Every stock item must belong to exactly one location.
- Empty locations are allowed.
- Multiple batches may exist in one location if permitted.
- Locations may be locked.

---

# 5. Product Rules

Every product must contain:

- Product Code
- Product Name
- Category
- Species
- Grade
- Thickness
- Width
- Length
- Moisture
- Unit
- Barcode
- QR Code
- Status

Product codes cannot be duplicated.

Deleted products remain archived.

---

# 6. Inventory Rules

Inventory is always calculated from movements.

Current Stock = Total In - Total Out

Manual editing of stock quantity is prohibited.

Corrections must be recorded as adjustment transactions.

---

# 7. Inventory Movement Types

Supported movement types:

- Purchase Receipt
- Production Receipt
- Warehouse Transfer
- Shipment
- Consumption
- Inventory Adjustment
- Stock Count Difference
- Return
- Scrap

Every movement records:

- Date
- Time
- User
- Warehouse
- Location
- Quantity
- Batch
- Reference Document
- Notes

Movements cannot be deleted.

---

# 8. Barcode Rules

Every product receives:

- QR Code
- Code128 Barcode

Barcodes are unique.

Duplicate barcodes are not allowed.

Barcode scanning must identify the product instantly.

---

# 9. Inventory Counting Rules

Stock counting can be performed using:

- Mobile Phone
- Tablet
- Desktop

Workflow:

1. Scan barcode
2. Verify product
3. Enter quantity
4. Save

Differences generate an adjustment proposal.

Approval is required before updating inventory.

---

# 10. User Permissions

Warehouse Operator

- View inventory
- Perform counting
- Transfer inventory

Warehouse Manager

- Approve adjustments
- Create warehouses
- Create locations

Administrator

- Full access

---

# 11. Inventory Adjustments

Inventory adjustments require:

Reason

Examples:

- Counting Difference
- Damage
- Measurement Error
- Data Migration
- Initial Balance

Every adjustment must be approved.

---

# 12. Batch Rules

Products may belong to batches.

Batch information includes:

- Batch Number
- Production Date
- Supplier
- Species
- Quality

Inventory movements preserve batch traceability.

---

# 13. Inventory Status

Possible inventory states:

Available

Reserved

Blocked

Damaged

Quality Hold

Scrap

Only Available inventory can be shipped.

---

# 14. Audit Rules

The following actions are logged:

Login

Logout

Product Creation

Product Update

Warehouse Creation

Transfer

Inventory Count

Adjustment

Deletion Requests

Logs cannot be modified.

---

# 15. Initial Data Import

The first deployment supports importing:

- Products
- Warehouses
- Locations
- Initial Inventory

Supported formats:

- Excel (.xlsx)
- CSV

Imported records are validated before saving.

---

# 16. Dashboard Rules

Dashboard displays:

- Total Products
- Total Inventory
- Inventory Value
- Low Stock Items
- Today's Movements
- Pending Adjustments
- Active Warehouses

Dashboard data refreshes automatically.

---

# 17. Security Rules

Every request requires authentication.

Permissions are role-based.

Sensitive actions require confirmation.

Passwords are encrypted.

Sessions expire automatically.

---

# 18. Business Objectives

The MVP is considered successful when:

- All warehouses are registered.
- All products have barcodes.
- Inventory can be counted via mobile devices.
- Every movement is traceable.
- Excel import/export works.
- Users can perform warehouse transfers.
- Management can monitor inventory in real time.

---

# 19. Future Business Rules

Future versions will introduce rules for:

- Production Orders
- Work Orders
- Machine Tracking
- Quality Control
- Purchasing
- Sales Orders
- CRM
- Maintenance
- AI Predictions
- IoT Integration
- Automated Warehouse Operations

These modules will extend the current business rules without changing the existing inventory principles.
