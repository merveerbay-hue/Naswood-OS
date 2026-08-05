# ==============================================================================
# TASK-054 — OPERATION
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Operation module defines the standardized manufacturing activities that
compose a Routing.

An Operation represents a reusable manufacturing step.

It describes **what** is performed during production.

Routing defines **when** the Operation occurs.

Production Execution records **how and when** the Operation was executed.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Operations are owned exclusively by the Production Master module.

Routing references Operations.

Production executes Operations.

Quality attaches inspection requirements to Operations.

---

# 3. RESPONSIBILITIES

The Operation module is responsible for:

- Operation Master Data
- Standard Manufacturing Activities
- Standard Times
- Resource Requirements
- Capability Requirements
- Quality Check Requirements
- Operation Parameters
- Version Management

The Operation module is NOT responsible for:

- Production Orders
- Routing Sequence
- Machine Assignment
- Labor Tracking
- Inventory Transactions
- Production Output

---

# 4. DEPENDENCIES

Depends on

- Work Center
- Tooling
- Production Parameters

Referenced by

- Routing
- Production
- Planning
- Quality
- Analytics

---

# 5. AGGREGATE ROOT

```
Operation
```

Children

- Operation Parameter
- Resource Requirement
- Quality Requirement
- Attachments

---

# 6. ENTITY MODEL

```
Operation
│
├── Parameters
├── Resource Requirements
├── Quality Requirements
├── Attachments
└── Audit
```

---

# 7. OPERATION MASTER

Every Operation contains

- Operation Code
- Operation Name
- Description
- Category
- Status

Operation Code is unique.

---

# 8. OPERATION TYPES

Examples

- Cross Cutting
- Rip Cutting
- Finger Joint
- Planing
- Sanding
- Glue Application
- Pressing
- CNC Processing
- Drilling
- Assembly
- Packaging
- Inspection

Organizations may define additional operation types.

---

# 9. STANDARD TIMES

Each Operation defines

- Setup Time
- Machine Time
- Labor Time
- Cycle Time
- Move Time
- Queue Time

Standard Times are planning values.

Actual execution times belong to Production Execution.

---

# 10. RESOURCE REQUIREMENTS

Each Operation may require

- Work Center Capability
- Machine Capability
- Tool Capability
- Operator Skill
- Production Parameters

Routing references Operations.

Production assigns actual resources.

---

# 11. QUALITY REQUIREMENTS

Operations may define

- Inspection Required
- Inspection Type
- Sampling Method
- Measurement Template
- Acceptance Criteria

Inspection execution belongs to the Quality module.

---

# 12. PRODUCTION PARAMETERS

Operations reference Production Parameters.

Examples

- Feed Speed
- Spindle Speed
- Temperature
- Pressure
- Moisture Target
- Glue Consumption
- Press Time

Parameter values are versioned.

---

# 13. VERSION MANAGEMENT

Operations support revision management.

Example

```
Operation

↓

Revision A

↓

Revision B

↓

Revision C
```

Only one revision may be Active.

Historical Routings remain pinned to the revision used.

---

# 14. VALIDATION RULES

System validates

- Unique Operation Code
- Positive Standard Times
- Valid Resource Requirements
- Valid Quality Requirements
- Valid Parameter Definitions

Invalid Operations cannot become Active.

---

# 15. APPROVAL WORKFLOW

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

Only Released Operations may be referenced by Routing.

---

# 16. BUSINESS RULES

Mandatory rules

- Operations are reusable.
- Routing references Operation Revisions.
- Production executes Operations.
- Standard Times are planning values only.
- Execution data never modifies Operation definitions.
- Active revisions are immutable.
- Engineering changes require new revisions.

---

# 17. API ENDPOINTS

```
GET    /api/v1/operations

GET    /api/v1/operations/{id}

POST   /api/v1/operations

PUT    /api/v1/operations/{id}

POST   /api/v1/operations/{id}/approve

POST   /api/v1/operations/{id}/release

GET    /api/v1/operations/{id}/revisions
```

---

# 18. EVENTS

Publishes

```
OperationCreated

OperationApproved

OperationReleased

OperationActivated

OperationSuperseded

OperationUpdated
```

---

# 19. PERMISSIONS

```
production.operation.read

production.operation.create

production.operation.update

production.operation.approve

production.operation.release
```

---

# 20. USER INTERFACE

The Operation screen contains

Header

↓

General Information

↓

Standard Times

↓

Resource Requirements

↓

Quality Requirements

↓

Production Parameters

↓

Attachments

↓

Revision History

↓

Audit Timeline

---

# 21. SEARCH & FILTERS

Support filtering by

- Operation Code
- Operation Name
- Category
- Status
- Revision
- Work Center Capability
- Quality Requirement

---

# 22. AUDIT

Every modification records

- User
- Timestamp
- Previous Value
- New Value
- Changed Fields
- Approval Action

Audit records are immutable.

---

# 23. CROSS MODULE INTEGRATION

Routing

Uses Operation definitions as reusable process steps.

Production

Executes Operation instances within Production Orders.

Planning

Uses Standard Times for scheduling and capacity planning.

Quality

Applies inspection rules defined by the Operation.

Analytics

Calculates

- Cycle Time
- Throughput
- Operation Efficiency
- Standard vs Actual Performance

---

# 24. REPORTING

Operation reporting supports

- Standard Time Analysis
- Actual vs Planned Duration
- Resource Utilization
- Operation Performance
- Quality Performance
- Bottleneck Analysis

Reports are generated from transactional production data.

---

# 25. SUCCESS CRITERIA

The Operation module is successful when

- Manufacturing activities are standardized.
- Routing reuses Operations instead of duplicating process definitions.
- Standard Times remain centrally managed.
- Resource requirements are reusable.
- Historical revisions remain immutable.
- Production Execution is fully separated from engineering definitions.

---

# 26. FINAL DESIGN STATEMENT

The Operation module is the canonical definition of reusable manufacturing
activities within the Naswood Operating System.

It defines standardized production processes, resource requirements,
quality checkpoints and production parameters while remaining independent
from Routing sequence, Production Execution and Inventory.

By separating engineering process definitions from operational execution,
NOS achieves reusable manufacturing knowledge, engineering consistency and
complete historical traceability.
