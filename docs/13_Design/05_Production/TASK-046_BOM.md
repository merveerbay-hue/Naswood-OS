# ==============================================================================
# TASK-046 — BILL OF MATERIALS (BOM)
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Bill of Materials (BOM) defines the engineering structure required to
manufacture a Product Revision.

A BOM represents the approved manufacturing definition of a product.

It specifies:

- Components
- Quantities
- Units of Measure
- Consumption Rules
- Operation Assignment
- Effectivity
- Revision

The BOM is an engineering definition.

It never represents inventory.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

The BOM is owned exclusively by the Production Master module.

Other modules may reference a BOM but never modify it.

---

# 3. RESPONSIBILITIES

The BOM module is responsible for:

- BOM Definition
- BOM Revision Management
- Component Structure
- Operation Assignment
- Consumption Rules
- Effectivity
- Approval Workflow
- Version History

The BOM module is NOT responsible for:

- Inventory
- Material Availability
- Purchasing
- Product Master
- Routing Execution
- Production Orders

---

# 4. DEPENDENCIES

Depends on

- Product
- Product Revision
- Capability Profile
- Unit of Measure
- Routing
- Operation

Referenced by

- Planning
- Production
- Costing
- Purchasing
- Quality

---

# 5. BOM TYPES

Supported BOM types:

- Manufacturing BOM (MBOM)
- Engineering BOM (EBOM) *(Optional Future)*
- Template BOM
- Phantom BOM *(Future)*
- Configurable BOM *(Future)*

Current implementation:

```
Manufacturing BOM (MBOM)
```

---

# 6. AGGREGATE ROOT

```
BillOfMaterial
```

Children:

- BOM Revision
- BOM Line
- BOM Effectivity
- BOM Attachment

---

# 7. ENTITY MODEL

```
BillOfMaterial
│
├── BOM Revision
│
│   ├── Line
│   ├── Line
│   ├── Line
│   └── ...
│
├── Approval
│
├── Attachments
│
└── Audit
```

---

# 8. BOM HEADER

Header contains:

- BOM Number
- ProductId
- ProductRevisionId
- Revision Number
- Description
- Plant
- Status
- Effective From
- Effective To
- Approval Status

---

# 9. BOM LINE

Each BOM Line references:

- Component ProductRevisionId
- Quantity
- Unit
- OperationId
- Scrap Factor
- Yield Factor
- Sequence
- Notes

A BOM Line never references Inventory.

Only Product Revision.

---

# 10. PRODUCT RELATIONSHIP

The BOM references:

```
Finished Product Revision

↓

Component Product Revisions
```

The BOM never owns Products.

Product revisions remain immutable.

---

# 11. REVISION MANAGEMENT

Every engineering change creates a new BOM Revision.

Old revisions remain immutable.

Example

```
BOM-001

↓

Rev A

↓

Rev B

↓

Rev C
```

Only one revision may be Active.

---

# 12. EFFECTIVITY

Every BOM Revision supports:

- Effective From
- Effective To

Planning and Production always use the effective revision.

Historical Production Orders remain pinned to their original revision.

---

# 13. OPERATION ASSIGNMENT

Each BOM Line references one Routing Operation.

Example

```
Operation 10

↓

Glue

↓

Operation 20

↓

Panel Assembly

↓

Operation 30

↓

Press

↓

Operation 40

↓

Packaging
```

Material consumption occurs during the assigned operation.

---

# 14. CONSUMPTION RULES

Supported:

- Fixed Quantity
- Variable Quantity
- Percentage Loss
- Scrap Allowance

Future:

- Formula Based Consumption

Consumption rules affect planning and production only.

---

# 15. VALIDATION RULES

System validates:

- Product Revision exists
- Component Revision exists
- UoM compatibility
- Positive quantity
- Duplicate sequence
- Circular references
- Active routing
- Valid operation

Invalid BOMs cannot be approved.

---

# 16. APPROVAL WORKFLOW

```
Draft

↓

Engineering Review

↓

Approved

↓

Released

↓

Active

↓

Superseded

↓

Archived
```

Only Released revisions may be used in production.

---

# 17. BUSINESS RULES

Mandatory rules:

- BOM belongs to one Product Revision.
- BOM references Product Revisions only.
- Inventory Items are never referenced.
- Active revisions are immutable.
- Changes require new revisions.
- Production Orders pin BOM Revision.
- Routing Revision must be compatible.
- Capability Profile must be active.

---

# 18. API ENDPOINTS

```
GET    /api/v1/bom

GET    /api/v1/bom/{id}

POST   /api/v1/bom

PUT    /api/v1/bom/{id}

POST   /api/v1/bom/{id}/approve

POST   /api/v1/bom/{id}/release

POST   /api/v1/bom/{id}/supersede

GET    /api/v1/bom/{id}/revisions
```

---

# 19. EVENTS

Publishes:

```
BomCreated

BomRevisionCreated

BomApproved

BomReleased

BomSuperseded

BomArchived
```

---

# 20. PERMISSIONS

Permissions:

```
production.bom.read

production.bom.create

production.bom.update

production.bom.approve

production.bom.release

production.bom.archive
```

---

# 21. USER INTERFACE

The BOM screen contains:

Header

↓

Revision Selector

↓

Component Grid

↓

Operation Assignment

↓

Attachments

↓

Approval History

↓

Audit Timeline

Users may compare revisions side by side.

---

# 22. SEARCH & FILTERS

Support filtering by:

- Product
- Product Revision
- BOM Number
- Revision
- Status
- Plant
- Effective Date
- Approval Status

---

# 23. AUDIT

Every change records:

- User
- Timestamp
- Previous Revision
- New Revision
- Changed Fields
- Approval
- Release Action

Audit records are immutable.

---

# 24. CROSS MODULE INTEGRATION

Planning

Uses BOM for MRP explosion.

Production

Pins BOM Revision during Production Order creation.

Inventory

Never stores BOM information.

Costing

Calculates standard cost using BOM structure.

Quality

Uses BOM Revision for inspection context.

---

# 25. SUCCESS CRITERIA

The BOM module is successful when:

- Engineering definitions are fully versioned.
- Every Production Order references an immutable BOM Revision.
- Material structure is traceable.
- Component revisions are preserved.
- Engineering changes never affect historical production.
- BOM data remains independent from Inventory and Production Execution.

---

# 26. FINAL DESIGN STATEMENT

The Bill of Materials is the canonical engineering definition of how a product
is manufactured.

It belongs to the Production Master domain and references immutable Product
Revisions rather than physical inventory.

Every manufacturing process in the Naswood Operating System depends on an
approved, versioned and auditable BOM, ensuring engineering integrity,
production consistency and complete historical traceability.
