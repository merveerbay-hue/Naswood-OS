# ==============================================================================
# TASK-049 — WORK CENTER
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Work Center represents a logical manufacturing resource where one or more
production operations are performed.

A Work Center groups production resources for planning, scheduling, costing and
capacity management.

It is the primary planning resource within the Production module.

Machines execute operations.

Work Centers plan and coordinate them.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

The Work Center is owned exclusively by the Production Master module.

Planning schedules Work Centers.

Production executes operations within Work Centers.

Maintenance maintains Machines belonging to Work Centers.

---

# 3. RESPONSIBILITIES

The Work Center module is responsible for:

- Work Center Master Data
- Capacity Definition
- Calendar Assignment
- Machine Assignment
- Labor Capacity
- Cost Center Assignment
- Scheduling Rules
- Production Availability

The Work Center module is NOT responsible for:

- Machine Maintenance
- Production Orders
- Inventory
- Product Definition
- Routing Definition

---

# 4. DEPENDENCIES

Depends on

- Production Line
- Calendar
- Machine
- Department
- Cost Center

Referenced by

- Routing
- Planning
- Production
- Costing
- Maintenance
- Analytics

---

# 5. AGGREGATE ROOT

```
WorkCenter
```

Children

- Capacity
- Calendar Assignment
- Machine Assignment
- Cost Assignment
- Attachments

---

# 6. ENTITY MODEL

```
WorkCenter
│
├── Capacity
├── Machines
├── Calendar
├── Cost Center
├── Attachments
└── Audit
```

---

# 7. WORK CENTER MASTER

Every Work Center contains:

- Work Center Code
- Work Center Name
- Description
- Department
- Production Line
- Cost Center
- Status

Work Center Code is unique.

---

# 8. WORK CENTER TYPES

Supported types

- Cutting
- Finger Joint
- Planing
- Gluing
- Pressing
- CNC
- Sanding
- Packaging
- Assembly
- Inspection

Custom types may be configured.

---

# 9. MACHINE ASSIGNMENT

One Work Center may contain multiple Machines.

```
Work Center

↓

Machine 01

Machine 02

Machine 03
```

Each Machine belongs to exactly one Work Center.

Routing references Work Centers.

Production selects the actual Machine.

---

# 10. CAPACITY

Capacity defines production capability.

Capacity includes

- Available Hours
- Operators
- Parallel Machines
- Daily Capacity
- Weekly Capacity
- Efficiency %

Capacity is versioned.

Planning always uses the active version.

---

# 11. CALENDAR

Each Work Center references one operational calendar.

Calendar defines

- Working Days
- Working Hours
- Breaks
- Holidays
- Shutdowns

Capacity planning respects calendar availability.

---

# 12. COST CENTER

Every Work Center references one Cost Center.

Cost collection supports

- Labor Cost
- Machine Cost
- Overhead
- Production Cost

Finance owns Cost Centers.

---

# 13. ROUTING RELATIONSHIP

Routing Operations reference Work Centers.

Example

```
Operation 010

↓

Cutting Work Center

↓

Operation 020

↓

Finger Joint Work Center

↓

Operation 030

↓

Press Work Center
```

Routing never references execution Machines directly.

---

# 14. PRODUCTION RELATIONSHIP

Production Orders assign Work Centers during planning.

Machine assignment occurs only during execution.

This allows machine substitution without changing Routing.

---

# 15. VALIDATION RULES

Validate

- Unique Work Center Code
- Valid Calendar
- Valid Production Line
- Valid Cost Center
- Positive Capacity
- Assigned Machines belong to the same plant

Invalid Work Centers cannot become Active.

---

# 16. APPROVAL WORKFLOW

```
Draft

↓

Engineering Review

↓

Approved

↓

Active

↓

Inactive

↓

Archived
```

Only Active Work Centers may receive production assignments.

---

# 17. BUSINESS RULES

Mandatory rules

- Work Center is the planning resource.
- Machine is the execution resource.
- Routing references Work Centers.
- Production assigns Machines.
- Capacity is versioned.
- Calendar controls availability.
- One Machine belongs to one Work Center.
- One Work Center may contain many Machines.

---

# 18. API ENDPOINTS

```
GET    /api/v1/work-centers

GET    /api/v1/work-centers/{id}

POST   /api/v1/work-centers

PUT    /api/v1/work-centers/{id}

POST   /api/v1/work-centers/{id}/approve

POST   /api/v1/work-centers/{id}/activate

POST   /api/v1/work-centers/{id}/deactivate

GET    /api/v1/work-centers/{id}/capacity
```

---

# 19. EVENTS

Publishes

```
WorkCenterCreated

WorkCenterApproved

WorkCenterActivated

WorkCenterDeactivated

WorkCenterCapacityUpdated

WorkCenterCalendarChanged
```

---

# 20. PERMISSIONS

```
production.workcenter.read

production.workcenter.create

production.workcenter.update

production.workcenter.approve

production.workcenter.activate

production.workcenter.deactivate
```

---

# 21. USER INTERFACE

The Work Center screen contains

Header

↓

General Information

↓

Capacity

↓

Assigned Machines

↓

Calendar

↓

Cost Center

↓

Attachments

↓

Audit Timeline

---

# 22. SEARCH & FILTERS

Support filtering by

- Work Center Code
- Name
- Department
- Production Line
- Status
- Capacity
- Calendar
- Cost Center

---

# 23. AUDIT

Every modification records

- User
- Timestamp
- Previous Value
- New Value
- Changed Fields
- Approval Action

Audit records are immutable.

---

# 24. CROSS MODULE INTEGRATION

Planning

Uses Work Center capacity for finite scheduling.

Routing

Assigns operations to Work Centers.

Production

Schedules operations to Work Centers and allocates Machines during execution.

Maintenance

Determines which production area a Machine belongs to.

Finance

Collects operational costs through Cost Centers.

Analytics

Calculates:

- Capacity Utilization
- Throughput
- Queue Time
- Work Center Efficiency
- OEE by Work Center

---

# 25. SUCCESS CRITERIA

The Work Center module is successful when:

- Planning schedules Work Centers instead of individual Machines.
- Routing remains independent from physical equipment.
- Machines can be substituted without engineering changes.
- Capacity planning is accurate.
- Cost collection is centralized.
- Production execution remains flexible.

---

# 26. FINAL DESIGN STATEMENT

The Work Center is the canonical production planning resource of the Naswood
Operating System.

It groups manufacturing resources into logical operational units for planning,
capacity management and costing while remaining independent from individual
Machines.

By separating planning resources from execution resources, the Work Center
architecture provides flexibility, scalability and accurate manufacturing
planning without compromising engineering integrity.
