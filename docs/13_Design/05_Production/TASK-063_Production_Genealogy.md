# ==============================================================================
# TASK-063 — PRODUCTION SCRAP
# Naswood Operating System (NOS)
# Module: Production Execution
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Production Scrap module records all material and product losses generated
during manufacturing.

Scrap represents production output that cannot be accepted as conforming
product.

Recording Scrap is mandatory for production accuracy, cost analysis,
genealogy, yield calculation and continuous improvement.

Scrap never disappears from the system.

Every rejected quantity remains traceable.

---

# 2. OWNERSHIP

Module Owner

```
Production Execution
```

Production owns Scrap recording.

Quality owns disposition decisions.

Inventory owns stock movements.

Finance owns scrap valuation.

---

# 3. RESPONSIBILITIES

The Production Scrap module is responsible for:

- Scrap Recording
- Scrap Classification
- Scrap Quantity
- Scrap Reason
- Scrap Cost Collection
- Genealogy Preservation
- Yield Calculation
- Scrap Analytics

The module is NOT responsible for:

- NCR Workflow
- Inventory Valuation
- Accounting
- Product Definitions
- Routing Definitions

---

# 4. DEPENDENCIES

Depends on

- Production Order
- Operation
- Product Revision
- Machine
- Work Center
- Scrap Reason

Referenced by

- Inventory
- Finance
- Quality
- Analytics
- Genealogy

---

# 5. AGGREGATE ROOT

```
ProductionScrap
```

Children

- Scrap Line
- Scrap Reason
- Attachments
- Audit

---

# 6. ENTITY MODEL

```
ProductionScrap
│
├── Scrap Lines
├── Reasons
├── Attachments
└── Audit
```

---

# 7. SCRAP HEADER

Every Scrap record contains

- Scrap Number
- Production Order
- Operation
- Product Revision
- Machine
- Work Center
- Scrap Date
- Status

Scrap Number is unique.

---

# 8. SCRAP LINE

Each Scrap Line contains

- Product Revision
- Lot Number (Optional)
- Quantity
- Unit
- Scrap Reason
- Notes

Scrap may reference Finished Goods or Semi-Finished Goods.

---

# 9. SCRAP TYPES

Supported Scrap Types

- Material Scrap
- Process Scrap
- Setup Scrap
- Machine Scrap
- Quality Scrap
- Rework Scrap
- Packaging Scrap
- Customer Return Scrap *(Future)*

Organizations may define additional Scrap Types.

---

# 10. SCRAP REASONS

Each Scrap record references one standardized reason.

Examples

```
Dimension Out of Tolerance

Surface Defect

Machine Failure

Incorrect Setup

Glue Failure

Moisture Out of Range

Operator Error

Material Defect
```

Reason Codes are centrally managed.

---

# 11. QUALITY RELATIONSHIP

Scrap may originate from

- Production
- Inspection
- Rework
- Final Quality Control

Rejected inspections may automatically create Scrap records when disposition is
"Scrap".

---

# 12. INVENTORY RELATIONSHIP

Scrap recording does not directly change inventory.

Inventory movements occur through Inventory Transactions.

Depending on company policy:

```
Scrap Recorded

↓

Inventory Adjustment

↓

Material Ledger

↓

Accounting
```

Inventory remains the single source of truth.

---

# 13. GENEALOGY

Every Scrap record preserves genealogy.

Traceability includes

```
Supplier Lot

↓

Material Lot

↓

Production Order

↓

Operation

↓

Scrap Record
```

Scrapped products remain historically traceable.

---

# 14. VALIDATION RULES

Before saving validate

- Released Production Order
- Valid Operation
- Positive Quantity
- Valid Scrap Reason
- Quantity does not exceed available production quantity
- Valid Unit of Measure

Invalid Scrap cannot be recorded.

---

# 15. BUSINESS RULES

Mandatory rules

- Every Scrap belongs to one Production Order.
- Scrap quantities are immutable after posting.
- Every Scrap has one standardized Reason.
- Scrap contributes to Yield calculations.
- Scrap history is never deleted.
- Inventory adjustments occur only through Inventory.

---

# 16. API ENDPOINTS

```
GET    /api/v1/production/scrap

GET    /api/v1/production/scrap/{id}

POST   /api/v1/production/scrap

PUT    /api/v1/production/scrap/{id}

GET    /api/v1/production/scrap/reasons

GET    /api/v1/production/scrap/{id}/audit
```

---

# 17. EVENTS

Publishes

```
ScrapRecorded

ScrapApproved

ScrapInventoryAdjustmentRequested

YieldUpdated

ScrapReasonAssigned
```

---

# 18. PERMISSIONS

```
production.scrap.read

production.scrap.record

production.scrap.update

production.scrap.approve

production.scrap.audit
```

---

# 19. USER INTERFACE

The Production Scrap screen contains

Header

↓

Production Order

↓

Operation

↓

Scrap Lines

↓

Reason Selection

↓

Photos & Attachments

↓

Quality Reference

↓

Audit Timeline

Supports barcode and QR code identification.

---

# 20. SEARCH & FILTERS

Support filtering by

- Scrap Number
- Production Order
- Product
- Machine
- Work Center
- Scrap Reason
- Scrap Type
- Status
- Date Range

---

# 21. AUDIT

Every Scrap transaction records

- User
- Timestamp
- Quantity
- Scrap Reason
- Previous Status
- New Status
- Correlation ID

Audit records are immutable.

---

# 22. CROSS MODULE INTEGRATION

Production

Records manufacturing losses.

Quality

Determines Scrap disposition following failed inspections.

Inventory

Processes inventory adjustments through Inventory Transactions.

Finance

Calculates Scrap Cost and Manufacturing Loss.

Analytics

Calculates

- Scrap Rate
- Yield
- Cost of Poor Quality
- Scrap by Machine
- Scrap by Product
- Scrap Trend

---

# 23. REPORTING

Production Scrap reporting supports

- Scrap History
- Scrap by Product
- Scrap by Machine
- Scrap by Work Center
- Scrap by Reason
- Scrap Cost
- Yield Analysis
- Pareto Analysis

Reports are generated from posted Scrap records.

---

# 24. SUCCESS CRITERIA

The Production Scrap module is successful when

- Every production loss is recorded.
- Scrap reasons are standardized.
- Yield calculations remain accurate.
- Scrap history is immutable.
- Inventory integrity is preserved.
- Manufacturing losses are fully traceable.

---

# 25. FINAL DESIGN STATEMENT

The Production Scrap module is the canonical record of manufacturing losses
within the Naswood Operating System.

It captures all non-conforming production quantities while preserving complete
traceability, standardized classification and seamless integration with
Production, Quality, Inventory, Finance and Analytics.

Every unit of Scrap contributes to operational transparency, cost visibility
and continuous manufacturing improvement.
