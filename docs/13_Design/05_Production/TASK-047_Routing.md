# ==============================================================================
# TASK-047 — ROUTING
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Routing defines the approved manufacturing process required to produce a
specific Product Revision.

A Routing describes **how** a product is manufactured.

It specifies:

- Manufacturing Operations
- Operation Sequence
- Work Centers
- Machines
- Standard Times
- Labor Requirements
- Tool Requirements
- Quality Checkpoints
- Production Parameters

The Routing is an engineering definition.

It never represents production execution.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Routing is owned exclusively by the Production Master module.

Other modules may reference Routing but never modify it.

---

# 3. RESPONSIBILITIES

Routing is responsible for:

- Process Definition
- Operation Definition
- Operation Sequence
- Resource Assignment
- Standard Time Definition
- Tool Assignment
- Quality Control Points
- Revision Management
- Approval Workflow

Routing is NOT responsible for:

- Production Orders
- Machine Scheduling
- Material Consumption
- Inventory
- Labor Tracking
- Production Output

---

# 4. DEPENDENCIES

Depends on

- Product Revision
- Work Center
- Machine
- Tooling
- Operation
- Calendar
- Production Parameters

Referenced by

- Planning
- Production
- Costing
- Maintenance
- Quality

---

# 5. ROUTING TYPES

Supported Routing Types

- Standard Routing
- Alternate Routing
- Prototype Routing
- Rework Routing
- Maintenance Routing *(Future)*

Current implementation:

```
Standard Routing
Alternate Routing
```

---

# 6. AGGREGATE ROOT

```
Routing
```

Children

- Routing Revision
- Routing Operation
- Routing Resource
- Routing Parameter
- Routing Attachment

---

# 7. ENTITY MODEL

```
Routing
│
├── Revision
│
│   ├── Operation
│   ├── Operation
│   ├── Operation
│   └── ...
│
├── Resources
│
├── Attachments
│
└── Audit
```

---

# 8. ROUTING HEADER

Header contains

- Routing Number
- Product Revision
- Revision
- Description
- Plant
- Routing Type
- Status
- Effective From
- Effective To
- Approval Status

---

# 9. ROUTING OPERATION

Each operation includes

- Sequence
- Operation Number
- Operation Name
- Work Center
- Preferred Machine
- Setup Time
- Cycle Time
- Queue Time
- Move Time
- Labor Time
- Quality Check Required
- Notes

Operations define process flow only.

No execution data is stored.

---

# 10. OPERATION SEQUENCE

Operations execute according to sequence.

Example

```
010 Cutting

↓

020 Finger Joint

↓

030 Planing

↓

040 Glue Application

↓

050 Press

↓

060 Calibration

↓

070 CNC

↓

080 Sanding

↓

090 Packaging
```

Parallel execution is supported when explicitly configured.

---

# 11. RESOURCE ASSIGNMENT

Each operation may reference

- Work Center
- Preferred Machine
- Required Tool
- Operator Skill
- Production Parameters

Resources are planning references.

Production selects actual resources during execution.

---

# 12. STANDARD TIMES

Routing defines

- Setup Time
- Machine Time
- Labor Time
- Cycle Time
- Queue Time
- Move Time
- Expected Duration

These values support

- Planning
- Scheduling
- Capacity
- Costing
- OEE Benchmarking

---

# 13. TOOL REQUIREMENTS

Operations may require

- Cutter Heads
- Saw Blades
- Milling Tools
- Press Plates
- CNC Programs
- Fixtures
- Measuring Equipment

Tool definitions reference Tooling Master.

---

# 14. QUALITY CONTROL

Operations may require inspections.

Inspection Types

- Incoming
- In Process
- Final
- Dimensional
- Visual

Quality checkpoints reference the Quality module.

---

# 15. REVISION MANAGEMENT

Every engineering change creates a new Routing Revision.

Example

```
Routing-001

↓

Rev A

↓

Rev B

↓

Rev C
```

Only one Routing Revision may be Active.

Historical revisions remain immutable.

---

# 16. EFFECTIVITY

Each Routing Revision supports

- Effective From
- Effective To

Production Orders always pin a specific Routing Revision.

Historical production never changes after release.

---

# 17. VALIDATION RULES

Before approval validate

- Product Revision exists
- Work Centers exist
- Operations exist
- Positive standard times
- Valid operation sequence
- No circular dependencies
- Required tools exist
- Calendar availability

Invalid Routing cannot be released.

---

# 18. APPROVAL WORKFLOW

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

Only Released Routing Revisions may be used by Production.

---

# 19. BUSINESS RULES

Mandatory rules

- Routing belongs to one Product Revision.
- Routing references immutable Operations.
- Routing references Work Centers, not execution machines.
- Production Orders pin Routing Revision.
- Active Routing Revisions are immutable.
- Engineering changes require new revisions.
- Routing and BOM revisions must be compatible.

---

# 20. API ENDPOINTS

```
GET    /api/v1/routings

GET    /api/v1/routings/{id}

POST   /api/v1/routings

PUT    /api/v1/routings/{id}

POST   /api/v1/routings/{id}/approve

POST   /api/v1/routings/{id}/release

POST   /api/v1/routings/{id}/supersede

GET    /api/v1/routings/{id}/revisions
```

---

# 21. EVENTS

Publishes

```
RoutingCreated

RoutingRevisionCreated

RoutingApproved

RoutingReleased

RoutingSuperseded

RoutingArchived
```

---

# 22. PERMISSIONS

```
production.routing.read

production.routing.create

production.routing.update

production.routing.approve

production.routing.release

production.routing.archive
```

---

# 23. USER INTERFACE

The Routing screen contains

Header

↓

Revision Selector

↓

Operation Sequence

↓

Resource Assignment

↓

Standard Times

↓

Quality Checkpoints

↓

Attachments

↓

Approval History

↓

Audit Timeline

Operations support drag-and-drop sequencing.

---

# 24. SEARCH & FILTERS

Support filtering by

- Product
- Product Revision
- Routing Number
- Revision
- Work Center
- Routing Type
- Status
- Effective Date
- Approval Status

---

# 25. AUDIT

Every modification records

- User
- Timestamp
- Previous Revision
- New Revision
- Changed Fields
- Approval
- Release Action

Audit records are immutable.

---

# 26. CROSS MODULE INTEGRATION

Planning

Uses Routing for capacity planning and scheduling.

Production

Pins Routing Revision during Production Order creation.

Maintenance

Uses preferred machine assignments for maintenance planning.

Quality

Uses operation checkpoints for inspection planning.

Costing

Uses standard times for labor and machine cost calculation.

Inventory

No direct dependency.

---

# 27. SUCCESS CRITERIA

The Routing module is successful when

- Every Product Revision has a controlled manufacturing process.
- Operations are fully versioned.
- Historical production references immutable Routing Revisions.
- Capacity planning uses standard times.
- Engineering changes never affect released production orders.
- Process definitions remain independent from execution.

---

# 28. FINAL DESIGN STATEMENT

Routing is the canonical manufacturing process definition of the Naswood
Operating System.

It defines **how** a product is manufactured by describing operations,
resources, standard times and process flow.

Routing belongs to the Production Master domain and is fully versioned,
approved and auditable.

Production Execution consumes Routing definitions but never modifies them,
ensuring engineering consistency, manufacturing repeatability and complete
historical traceability.
