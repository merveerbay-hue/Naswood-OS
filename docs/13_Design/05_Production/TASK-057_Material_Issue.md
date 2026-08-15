# ==============================================================================
# TASK-057 — MATERIAL ISSUE
# Naswood Operating System (NOS)
# Module: Production Execution
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Material Issue module records the controlled consumption of materials from
Inventory into Production.

Material Issue is the only supported process for consuming inventory during
manufacturing.

Production never updates stock balances directly.

All inventory movements are performed through Inventory Transactions.

---

# 2. OWNERSHIP

Module Owner

```
Production Execution
```

Inventory owns stock balances.

Production owns the manufacturing context.

Material Issue creates Inventory Transactions but never modifies Inventory
Balances directly.

---

# 3. RESPONSIBILITIES

The Material Issue module is responsible for:

- Material Consumption
- Lot Selection
- Serial Selection
- Quantity Validation
- Warehouse Validation
- Operation Assignment
- Consumption Traceability
- Material Return Support

The module is NOT responsible for:

- Inventory Balances
- Warehouse Management
- Purchasing
- BOM Definition
- Production Output

---

# 4. DEPENDENCIES

Depends on

- Production Order
- Material Requirements
- Inventory
- Warehouse
- Product Revision
- Lot
- Serial Number

Referenced by

- Inventory
- Finance
- Genealogy
- Analytics

---

# 5. AGGREGATE ROOT

```
MaterialIssue
```

Children

- Material Issue Line
- Lot Allocation
- Serial Allocation
- Audit

---

# 6. ENTITY MODEL

```
MaterialIssue
│
├── Lines
├── Lots
├── Serials
└── Audit
```

---

# 7. MATERIAL ISSUE HEADER

Every Material Issue contains

- Issue Number
- Production Order
- Operation
- Warehouse
- Issue Date
- Status

Issue Number is unique.

---

# 8. MATERIAL ISSUE LINE

Each line contains

- Component Product Revision
- Inventory Material
- Warehouse
- Lot
- Serial Number (Optional)
- Planned Quantity
- Issued Quantity
- Unit

Material Issue references the Product Revision defined in the pinned BOM.

---

# 9. INVENTORY TRANSACTION

Posting Material Issue creates

```
Inventory Transaction

↓

Material Ledger Entry

↓

Inventory Balance Update

↓

Audit Record
```

Production never changes Inventory directly.

Inventory remains the single source of truth.

---

# 10. LOT & SERIAL TRACEABILITY

Every issued material supports

- Lot Tracking
- Serial Tracking (Optional)
- Expiration Validation (Optional)
- FIFO / FEFO Selection (Configurable)

Selections are stored permanently.

---

# 11. CONSUMPTION MODES

Supported modes

- Manual Issue
- Barcode Issue
- QR Code Issue
- Automatic Backflush *(Configurable)*
- Partial Issue
- Complete Issue

Backflush occurs only after successful Operation completion.

---

# 12. MATERIAL RETURN

Unused materials may be returned.

Posting Material Return creates

```
Reverse Inventory Transaction
```

Returns preserve

- Lot
- Serial
- Warehouse
- Production Order
- Operation

---

# 13. VALIDATION RULES

Before posting validate

- Released Production Order
- Active Operation
- Valid Warehouse
- Positive Quantity
- Available Inventory
- Valid Lot
- Valid Serial
- Compatible Unit of Measure

Negative inventory is prohibited unless explicitly configured.

---

# 14. BUSINESS RULES

Mandatory rules

- Material Issue always references a Production Order.
- Material Issue always creates Inventory Transactions.
- Inventory Balances are never modified directly.
- Every Lot remains traceable.
- Every Material Return creates a reverse transaction.
- Historical Material Issues are immutable.

---

# 15. API ENDPOINTS

```
GET    /api/v1/production/material-issues

GET    /api/v1/production/material-issues/{id}

POST   /api/v1/production/material-issues

POST   /api/v1/production/material-returns

GET    /api/v1/production/material-issues/{id}/audit
```

---

# 16. EVENTS

Publishes

```
MaterialIssued

MaterialReturned

MaterialIssueCancelled

InventoryTransactionCreated
```

---

# 17. PERMISSIONS

```
production.material.read

production.material.issue

production.material.return

production.material.audit
```

---

# 18. USER INTERFACE

The Material Issue screen contains

Header

↓

Production Order

↓

Operation

↓

Material Requirements

↓

Lot Selection

↓

Serial Selection

↓

Issue Quantity

↓

Validation Messages

↓

Audit Timeline

Barcode and QR scanning are supported.

---

# 19. SEARCH & FILTERS

Support filtering by

- Issue Number
- Production Order
- Product
- Warehouse
- Material
- Lot
- Status
- Date

---

# 20. AUDIT

Every transaction records

- User
- Timestamp
- Warehouse
- Lot
- Quantity
- Previous Inventory
- New Inventory
- Correlation ID

Audit records are immutable.

---

# 21. CROSS MODULE INTEGRATION

Inventory

Owns Inventory Transactions and Balance updates.

Production

Consumes materials according to the Production Order.

Finance

Records material consumption cost.

Genealogy

Links consumed material lots to finished products.

Analytics

Calculates

- Material Consumption
- Material Variance
- Yield
- Waste
- Inventory Accuracy

---

# 22. REPORTING

Material Issue reporting supports

- Material Consumption
- Component Usage
- Lot Traceability
- Material Variance
- Material Returns
- Warehouse Consumption

Reports are generated from Inventory Transactions.

---

# 23. SUCCESS CRITERIA

The Material Issue module is successful when

- Every material consumption is posted through Inventory.
- Every issued lot remains traceable.
- Inventory integrity is preserved.
- Production consumes only approved materials.
- Material Returns reverse inventory correctly.
- Historical consumption remains immutable.

---

# 24. FINAL DESIGN STATEMENT

The Material Issue module is the canonical interface between Production
Execution and Inventory within the Naswood Operating System.

It records controlled material consumption through Inventory Transactions while
maintaining complete lot traceability, inventory integrity and manufacturing
auditability.

No material may be consumed outside the Material Issue process.
