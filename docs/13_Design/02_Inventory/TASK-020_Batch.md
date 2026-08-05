# TASK-020 — Batch

**Module:** Inventory

**Category:** Master Data

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Batch entity provides traceability for inventory by grouping materials produced, received or processed under the same manufacturing or supplier lot.

Batch management enables complete material traceability across purchasing, inventory, production, quality and sales while supporting regulatory compliance and production history.

Every batch maintains its own inventory, movement history and quality status.

---

# Objectives

- Complete Material Traceability
- Production History
- Supplier Traceability
- Quality Tracking
- Inventory Accuracy
- Manufacturing Compliance
- Recall Support

---

# Scope

Batch Management includes

- Supplier Batches
- Production Batches
- Thermowood Batches
- Internal Batches
- Batch Inventory
- Batch Status
- Batch Traceability
- Batch Genealogy

Batch Management does NOT include

- Material Definition
- Warehouse Definition
- Production Orders
- Financial Valuation

---

# Business Rules

- A material may have multiple batches.
- Every batch belongs to one material.
- Batch numbers shall be unique per material.
- Batch-controlled materials require a batch for every inventory transaction.
- Batch history is immutable.
- Batch quantities are calculated from inventory transactions.
- Closed batches cannot receive new inventory.

---

# Batch Types

The system supports the following batch types.

| Type | Description |
|-------|-------------|
| Supplier Batch | Received from supplier |
| Production Batch | Generated during manufacturing |
| Thermowood Batch | Heat treatment process batch |
| Rework Batch | Reprocessed material |
| Return Batch | Customer returned material |
| Sample Batch | Quality testing material |

---

# Batch Information

Each batch contains

- Batch Number
- Material
- Batch Type
- Production Date
- Receipt Date
- Expiration Date (Optional)
- Supplier Batch
- Production Order
- Warehouse
- Status

---

# Batch Status

Supported statuses

- Draft
- Available
- Reserved
- Quality Hold
- Blocked
- Consumed
- Closed
- Archived

Only Available batches may participate in normal inventory operations.

Reference

Status_Lifecycle.md

---

# Batch Lifecycle

```
Created

↓

Received

↓

Available

↓

Reserved

↓

Consumed

↓

Closed

↓

Archived
```

---

# Batch Inventory

Inventory is maintained independently for each batch.

Each batch stores

- On Hand
- Available
- Reserved
- Allocated
- Blocked
- Quality Hold

Batch inventory follows the same calculation rules as standard inventory.

Reference

TASK-019_Inventory.md

---

# Batch Traceability

The system provides complete traceability.

Supports

- Supplier → Customer
- Customer → Supplier
- Production History
- Consumption History
- Shipment History
- Quality History

---

# Batch Genealogy

The system shall maintain parent-child relationships.

Example

```
Supplier Batch

↓

Raw Material

↓

Finger Joint

↓

Thermowood

↓

Finished Product

↓

Customer Shipment
```

Genealogy enables complete forward and backward traceability.

---

# Batch Operations

Supported operations

- Create Batch
- Receive Batch
- Reserve Batch
- Transfer Batch
- Consume Batch
- Return Batch
- Split Batch
- Merge Batch
- Close Batch

---

# Batch Split

A batch may be divided into multiple child batches.

Example

```
Batch A

↓

Batch A-1

Batch A-2

Batch A-3
```

Parent-child relationships are preserved.

---

# Batch Merge

Multiple batches may be merged when permitted by business rules.

Merge operations shall preserve complete genealogy.

---

# Quality Integration

Each batch may contain

- Inspection Result
- Test Certificate
- Moisture Value
- Density
- Quality Status
- NCR Reference

Reference

06_Quality

---

# Thermowood Integration

Thermowood batches additionally maintain

- Kiln Number
- Recipe Version
- Heat Treatment Cycle
- Maximum Temperature
- Cycle Duration
- Final Moisture
- Operator
- Process Certificate

This enables complete traceability from kiln process to finished product.

---

# Warehouse Integration

Each batch may exist in

- Multiple Warehouses
- Multiple Locations

Inventory is maintained separately for each warehouse-location combination.

Reference

TASK-017_Warehouse.md

TASK-018_Location.md

---

# Inventory Transactions

Batch inventory changes only through

- Goods Receipt
- Goods Issue
- Stock Transfer
- Production Receipt
- Production Consumption
- Inventory Adjustment
- Inventory Count

Reference

TASK-021_Goods_Receipt.md

TASK-022_Goods_Issue.md

TASK-023_Stock_Transfer.md

---

# Batch Selection Rules

Supports

- FIFO
- FEFO
- Manual Selection
- Batch Priority
- Customer-Specific Batch
- Quality Approved Batch Only

Selection strategy is configurable.

---

# Shelf Life

When shelf life is enabled

The system maintains

- Production Date
- Expiration Date
- Remaining Shelf Life

Expired batches may be blocked automatically according to company policy.

---

# Barcode Support

Each batch may have

- Batch Barcode
- GS1 Barcode
- QR Code

Scanning a batch displays

- Material
- Warehouse
- Location
- Quantity
- Status
- Traceability

Reference

Barcode_Strategy.md

QRCode_Strategy.md

---

# Mobile Support

Supports

- Batch Scan
- Batch Lookup
- Batch Transfer
- Batch Inventory
- Batch Traceability

Reference

Inventory_Mobile.md

---

# AI Integration

AI may assist with

- Batch Risk Detection
- Shelf Life Prediction
- Batch Quality Prediction
- Batch Optimization
- Recall Impact Analysis
- Inventory Rotation Recommendations

Reference

AI_Copilot.md

---

# Dashboard

Batch contributes to

- Batch Inventory
- Batch Aging
- Batch Expiration
- Quality Hold
- Traceability
- Thermowood Production

Reference

Inventory_Dashboard.md

---

# Reports

Batch data appears in

- Batch Traceability Report
- Batch History
- Batch Inventory Report
- Batch Expiration Report
- Quality Report
- Stock Card

Reference

Inventory_Reports.md

---

# API

Primary endpoints

```
GET /batches

GET /batches/{id}

GET /batches/{id}/inventory

GET /batches/{id}/traceability

GET /batches/{id}/genealogy

POST /batches

PUT /batches/{id}

POST /batches/{id}/split

POST /batches/{id}/merge
```

Reference

Inventory_API.md

---

# Events

Batch publishes

- BatchCreated
- BatchReceived
- BatchReserved
- BatchTransferred
- BatchSplit
- BatchMerged
- BatchConsumed
- BatchClosed

Reference

Event_Model.md

Integration_Events.md

---

# Permissions

Typical permissions

- View Batch
- Create Batch
- Update Batch
- Close Batch
- View Traceability
- Split Batch
- Merge Batch

Reference

Permission_Model.md

---

# Validation Rules

The system validates

- Batch number uniqueness.
- Material assignment.
- Warehouse assignment.
- Batch status transition.
- Available quantity before issue.
- Shelf life (if enabled).
- Quality approval (if required).
- Parent-child relationships after split and merge.

Reference

Validation_Rules.md

---

# Audit

The following actions are audited

- Batch Created
- Batch Updated
- Batch Split
- Batch Merged
- Batch Closed
- Quality Status Changed

Reference

Audit_Log.md

---

# Performance

The system shall

- Support millions of batch records.
- Provide traceability lookup in less than 500 ms.
- Optimize genealogy queries.
- Cache frequently accessed batch metadata.

Reference

Performance.md

Caching.md

---

# Security

Batch management follows

- Role-Based Access Control
- Warehouse Authorization
- Secure API Access
- Complete Audit Logging

Reference

Security.md

Permission_Model.md

---

# Naswood Implementation

Typical batch examples

| Batch Type | Example |
|------------|---------|
| Supplier Batch | SUP-2026-004512 |
| Log Batch | LOG-20260805-001 |
| Kiln Batch | THW-K03-20260805 |
| Finger Joint Batch | FJ-20260805-014 |
| Panel Batch | MP-20260805-008 |
| Finished Product Batch | FG-20260805-102 |

Every finished product shall be traceable back to

- Supplier
- Log Batch
- Thermowood Cycle
- Production Order
- Quality Inspection
- Shipment

This provides complete end-to-end traceability for Naswood products.

---

# Acceptance Criteria

The Batch module shall

- Support multiple batch types.
- Maintain independent batch inventory.
- Provide complete forward and backward traceability.
- Support batch split and merge.
- Integrate with Quality and Production.
- Support barcode and mobile operations.
- Follow all shared platform standards.

---

# Related Documents

Inventory_Architecture.md

Inventory_Dashboard.md

Inventory_API.md

Inventory_Mobile.md

Inventory_Reports.md

TASK-016_Material.md

TASK-017_Warehouse.md

TASK-018_Location.md

TASK-019_Inventory.md

TASK-021_Goods_Receipt.md

TASK-022_Goods_Issue.md

TASK-023_Stock_Transfer.md

Barcode_Strategy.md

QRCode_Strategy.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Security.md

Audit_Log.md

Status_Lifecycle.md

AI_Copilot.md

Event_Model.md

Integration_Events.md
