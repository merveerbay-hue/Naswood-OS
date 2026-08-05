# ==============================================================================
# TASK-056 — PRODUCTION ORDER
# Naswood Operating System (NOS)
# Module: Production Execution
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Production Order represents the executable manufacturing instruction within
the Naswood Operating System.

A Production Order transforms approved engineering definitions into controlled
manufacturing execution.

The Production Order is the execution contract between Planning and Production.

It is immutable with respect to engineering references after release.

---

# 2. OWNERSHIP

Module Owner

```
Production Execution
```

Production Orders are owned exclusively by the Production Execution module.

Planning creates planned demand.

Production executes approved Production Orders.

Engineering definitions remain owned by Production Master.

---

# 3. RESPONSIBILITIES

The Production Order module is responsible for:

- Production Order Lifecycle
- Manufacturing Execution
- Material Reservation Reference
- Execution Status
- Progress Tracking
- Quantity Tracking
- Operation Tracking
- Completion
- Closure

The module is NOT responsible for:

- Product Definitions
- BOM Editing
- Routing Editing
- Inventory Management
- Cost Calculation

---

# 4. DEPENDENCIES

Depends on

- Product Revision
- Capability Profile
- BOM Revision
- Routing Revision
- Planning
- Work Center
- Warehouse

Referenced by

- Inventory
- Quality
- Genealogy
- Finance
- Analytics

---

# 5. AGGREGATE ROOT

```
ProductionOrder
```

Children

- Operations
- Material Requirements
- Labor Entries
- Downtime
- Scrap
- Output
- Genealogy
- Audit

---

# 6. ENTITY MODEL

```
ProductionOrder
│
├── Operations
├── Material Requirements
├── Labor
├── Scrap
├── Downtime
├── Output
├── Genealogy
└── Audit
```

---

# 7. PRODUCTION ORDER MASTER

Every Production Order contains

- Production Order Number
- Product Revision
- Capability Profile
- BOM Revision
- Routing Revision
- Warehouse
- Planned Quantity
- Completed Quantity
- Scrap Quantity
- Unit
- Priority
- Due Date
- Planner
- Status

Production Order Number is unique.

---

# 8. ORDER LIFECYCLE

```
Draft

↓

Planned

↓

Released

↓

Ready

↓

In Progress

↓

Paused

↓

Completed

↓

Closed

↓

Archived
```

Alternative states

```
Cancelled

On Hold
```

State transitions are controlled by Workflow Engine.

---

# 9. ENGINEERING PINNING

At Release the Production Order permanently pins

- Product Revision
- Capability Profile
- BOM Revision
- Routing Revision

Pinned references are immutable.

Engineering changes never affect released orders.

---

# 10. MATERIAL REQUIREMENTS

Material Requirements are generated from the pinned BOM Revision.

Each requirement includes

- Component Product Revision
- Planned Quantity
- Issued Quantity
- Remaining Quantity
- Unit
- Assigned Operation

Material Requirements do not create Inventory.

Inventory Transactions perform material movement.

---

# 11. OPERATION EXECUTION

Operations are instantiated from the pinned Routing Revision.

Each Operation records

- Planned Start
- Planned Finish
- Actual Start
- Actual Finish
- Assigned Work Center
- Assigned Machine
- Status
- Progress

Operations execute independently while remaining linked to the parent Production
Order.

---

# 12. QUANTITY TRACKING

The Production Order tracks

- Planned Quantity
- Produced Quantity
- Accepted Quantity
- Scrap Quantity
- Remaining Quantity

Quantities are derived from transactional production events.

---

# 13. VALIDATION RULES

Before Release validate

- Active Product Revision
- Active Capability Profile
- Active BOM Revision
- Active Routing Revision
- Valid Warehouse
- Positive Quantity
- Valid Due Date

Released Production Orders cannot change engineering references.

---

# 14. APPROVAL WORKFLOW

```
Draft

↓

Planning Review

↓

Released

↓

Execution

↓

Completed

↓

Closed

↓

Archived
```

Workflow Engine governs transitions.

---

# 15. BUSINESS RULES

Mandatory rules

- Every Production Order references one Product Revision.
- Engineering references are immutable after Release.
- Inventory is updated only through Inventory Transactions.
- Finished Goods are created only by Production Output.
- Every Production Order supports complete genealogy.
- Historical Production Orders are never recalculated.

---

# 16. API ENDPOINTS

```
GET    /api/v1/production/orders

GET    /api/v1/production/orders/{id}

POST   /api/v1/production/orders

PUT    /api/v1/production/orders/{id}

POST   /api/v1/production/orders/{id}/release

POST   /api/v1/production/orders/{id}/pause

POST   /api/v1/production/orders/{id}/resume

POST   /api/v1/production/orders/{id}/complete

POST   /api/v1/production/orders/{id}/close
```

---

# 17. EVENTS

Publishes

```
ProductionOrderCreated

ProductionOrderReleased

ProductionOrderStarted

ProductionOrderPaused

ProductionOrderResumed

ProductionOrderCompleted

ProductionOrderClosed

ProductionOrderCancelled
```

---

# 18. PERMISSIONS

```
production.order.read

production.order.create

production.order.release

production.order.execute

production.order.complete

production.order.close

production.order.cancel
```

---

# 19. USER INTERFACE

The Production Order screen contains

Header

↓

Engineering References

↓

Operations

↓

Material Requirements

↓

Execution Progress

↓

Labor

↓

Scrap

↓

Downtime

↓

Production Output

↓

Genealogy

↓

Audit Timeline

---

# 20. SEARCH & FILTERS

Support filtering by

- Production Order Number
- Product
- Status
- Planner
- Priority
- Work Center
- Due Date
- Warehouse

---

# 21. AUDIT

Every modification records

- User
- Timestamp
- Previous Status
- New Status
- Workflow Action
- Correlation ID

Audit records are immutable.

---

# 22. CROSS MODULE INTEGRATION

Planning

Creates executable Production Orders.

Inventory

Posts Material Issue, Material Return and Production Output transactions.

Quality

Records inspections against Production Orders.

Finance

Collects production costs.

Genealogy

Creates complete material traceability.

Analytics

Calculates

- Throughput
- Yield
- OEE
- Cycle Time
- On-Time Completion

---

# 23. REPORTING

Production Order reporting supports

- Order Progress
- Completion History
- Production Performance
- Material Consumption
- Scrap Analysis
- Downtime Analysis
- Cost Summary

Reports are generated from transactional execution data.

---

# 24. SUCCESS CRITERIA

The Production Order module is successful when

- Every manufacturing activity is executed from an approved order.
- Engineering definitions remain immutable after release.
- Material consumption is fully traceable.
- Production progress is visible in real time.
- Historical execution remains reproducible.
- Complete genealogy is preserved.

---

# 25. FINAL DESIGN STATEMENT

The Production Order is the canonical execution document of the Naswood
Operating System.

It transforms approved engineering definitions into controlled manufacturing
execution while preserving immutable engineering references, complete
traceability and seamless integration with Planning, Inventory, Quality,
Genealogy and Analytics.

It is the authoritative source for all production execution activities.
