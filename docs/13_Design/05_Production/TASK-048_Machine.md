# ==============================================================================
# TASK-048 — MACHINE
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Machine module manages the physical production equipment used during
manufacturing.

A Machine represents an individual production asset capable of executing one or
more manufacturing operations.

Machines are production resources.

They are not production orders, work centers or maintenance records.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Machine definitions are owned exclusively by the Production Master module.

Maintenance owns machine maintenance history.

Production owns machine execution history.

---

# 3. RESPONSIBILITIES

The Machine module is responsible for:

- Machine Master Data
- Machine Classification
- Machine Capabilities
- Capacity Definition
- Machine Parameters
- Machine Status
- Preferred Work Center Assignment
- Production Availability
- Technical Specifications

The Machine module is NOT responsible for:

- Maintenance Planning
- Production Orders
- Labor Tracking
- Inventory
- Scheduling
- Downtime History

---

# 4. DEPENDENCIES

Depends on

- Work Center
- Production Line
- Calendar
- Tooling

Referenced by

- Routing
- Production
- Planning
- Maintenance
- Quality
- Analytics

---

# 5. MACHINE TYPES

Examples

- Band Saw
- Circular Saw
- Finger Joint
- Four Side Planer
- Wide Belt Sander
- Cross Cut Saw
- Glue Spreader
- Cold Press
- Hot Press
- CLT Press
- CNC Router
- Packaging Line
- Palletizer

Machine Type defines general behavior.

Machine represents the physical asset.

---

# 6. AGGREGATE ROOT

```
Machine
```

Children

- Machine Capability
- Machine Parameter
- Machine Attachment
- Machine Certification

---

# 7. ENTITY MODEL

```
Machine
│
├── Capabilities
├── Parameters
├── Certifications
├── Attachments
└── Audit
```

---

# 8. MACHINE MASTER

Every Machine contains

- Machine Number
- Machine Name
- Machine Type
- Manufacturer
- Model
- Serial Number
- Asset Number
- Work Center
- Production Line
- Status

Machine Number is unique.

---

# 9. MACHINE STATUS

Supported statuses

```
Draft

Available

Reserved

Running

Idle

Setup

Maintenance

Breakdown

Out of Service

Retired
```

Status changes are event driven.

---

# 10. MACHINE CAPABILITIES

Capabilities define what the machine can perform.

Examples

- Cutting
- Finger Joint
- Planing
- Sanding
- Pressing
- CNC Machining
- Packaging

A machine may support multiple capabilities.

Routing references required capabilities.

Production selects a compatible machine.

---

# 11. TECHNICAL PARAMETERS

Machine Parameters include

- Maximum Width
- Maximum Thickness
- Maximum Length
- Minimum Length
- Feed Speed
- Spindle Speed
- Press Force
- Rated Power
- Capacity per Hour

Parameters are versioned.

Historical production remains linked to the parameter version used.

---

# 12. WORK CENTER RELATIONSHIP

A Machine belongs to exactly one Work Center.

A Work Center may contain multiple Machines.

```
Work Center

↓

Machine A

Machine B

Machine C
```

Production scheduling occurs at the Work Center level.

Machine assignment occurs during execution.

---

# 13. PRODUCTION LINE RELATIONSHIP

Machines may optionally belong to a Production Line.

```
Production Line

↓

Work Centers

↓

Machines
```

Production Lines group machines for operational visibility.

---

# 14. CALENDAR

Every Machine references an operational calendar.

Calendar defines

- Working Days
- Working Hours
- Planned Shutdowns
- Holidays

Planning respects machine availability.

---

# 15. VALIDATION RULES

System validates

- Unique Machine Number
- Valid Work Center
- Valid Production Line
- Valid Calendar
- Positive Capacity Values
- Supported Capability Definitions

Invalid machines cannot become Available.

---

# 16. APPROVAL WORKFLOW

```
Draft

↓

Technical Review

↓

Approved

↓

Available

↓

Active

↓

Retired

↓

Archived
```

Only Available machines may be scheduled.

---

# 17. BUSINESS RULES

Mandatory rules

- Machine belongs to one Work Center.
- Machine may belong to one Production Line.
- Machine capabilities are versioned.
- Machine parameters are versioned.
- Retired machines cannot be scheduled.
- Machines do not execute production independently.
- Production Orders assign machines during execution.

---

# 18. API ENDPOINTS

```
GET    /api/v1/machines

GET    /api/v1/machines/{id}

POST   /api/v1/machines

PUT    /api/v1/machines/{id}

POST   /api/v1/machines/{id}/approve

POST   /api/v1/machines/{id}/retire

GET    /api/v1/machines/{id}/history
```

---

# 19. EVENTS

Publishes

```
MachineCreated

MachineApproved

MachineActivated

MachineRetired

MachineCapabilityUpdated

MachineStatusChanged
```

---

# 20. PERMISSIONS

```
production.machine.read

production.machine.create

production.machine.update

production.machine.approve

production.machine.retire
```

---

# 21. USER INTERFACE

The Machine screen contains

Header

↓

General Information

↓

Capabilities

↓

Technical Parameters

↓

Work Center Assignment

↓

Production Line

↓

Calendar

↓

Attachments

↓

Audit Timeline

---

# 22. SEARCH & FILTERS

Support filtering by

- Machine Number
- Machine Name
- Machine Type
- Work Center
- Production Line
- Status
- Capability
- Manufacturer

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

Routing

Uses Machine capabilities to define preferred execution resources.

Production

Assigns actual machines during operation execution.

Planning

Uses machine calendars and capacities for scheduling.

Maintenance

Owns maintenance plans, work orders and breakdown history.

Quality

Associates inspections with the executing machine.

Analytics

Calculates:

- OEE
- Availability
- Utilization
- MTBF
- MTTR

---

# 25. SUCCESS CRITERIA

The Machine module is successful when

- Every production asset has a unique identity.
- Machine capabilities are centrally managed.
- Routing references machine capabilities instead of fixed assets.
- Planning respects machine availability.
- Production records actual machine usage.
- Maintenance and Production share the same machine master.

---

# 26. FINAL DESIGN STATEMENT

The Machine module is the canonical master of production equipment within the
Naswood Operating System.

It defines the technical characteristics, operational capabilities and
availability of manufacturing assets while remaining independent from
production execution and maintenance history.

Every manufacturing operation ultimately executes on a Machine defined by this
module, ensuring consistency, traceability and scalable manufacturing resource
management.
