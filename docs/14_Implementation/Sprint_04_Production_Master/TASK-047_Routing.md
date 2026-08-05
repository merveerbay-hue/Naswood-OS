# TASK-047 — Routing

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Manufacturing Engineering

**Priority:** Critical

**Estimated Effort:** 10 Days

**Status:** Planned

---

# Purpose

Develop the Routing module for Naswood OS.

The Routing module defines the complete manufacturing process required to produce a finished or semi-finished product. It specifies every production operation, work center, machine, tooling requirements, setup time, production time, labor requirements and quality checkpoints.

The Routing module is the operational backbone of Production Planning, Scheduling, Capacity Planning and Shop Floor Execution.

---

# Objectives

- Centralized Manufacturing Routing
- Standard Production Processes
- Capacity Planning Support
- Accurate Production Scheduling
- Standard Time Management
- Cost Calculation Support
- Manufacturing Traceability

---

# Scope

The Routing module includes

- Routing Creation
- Manufacturing Operations
- Work Center Assignment
- Machine Assignment
- Tool Assignment
- Labor Assignment
- Setup & Cycle Times
- Routing Revision
- Alternative Routing
- Routing Approval

Out of Scope

- Production Orders
- Machine Monitoring
- Production Reporting
- Maintenance Scheduling

---

# Routing Architecture

```
Product

↓

Routing

↓

Operations

↓

Work Centers

↓

Machines

↓

Production Order

↓

Shop Floor
```

---

# Routing Lifecycle

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

Revised

↓

Obsolete

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Routing Types

Supports

- Standard Routing
- Alternative Routing
- Prototype Routing
- Rework Routing
- Maintenance Routing
- Outsourcing Routing

---

# Routing Header

Each Routing contains

## General Information

- Routing Number
- Product Code
- Product Name
- Company
- Plant
- Routing Type
- Version
- Revision
- Status

---

## Validity

- Effective From
- Effective To
- Revision Date
- Approved By
- Approval Date

---

## Production Information

- Standard Lot Size
- Target Cycle Time
- Estimated Production Time
- Yield %
- Scrap %
- Notes

---

# Routing Operations

Each routing consists of ordered operations.

Each operation contains

- Operation Number
- Operation Name
- Sequence
- Work Center
- Machine
- Labor Group
- Setup Time
- Cycle Time
- Queue Time
- Move Time
- Inspection Required
- Notes

---

# Standard Operation Flow

```
0010

Raw Material Preparation

↓

0020

Cutting

↓

0030

Finger Joint

↓

0040

Planer

↓

0050

Assembly

↓

0060

Press

↓

0070

Calibration

↓

0080

CNC

↓

0090

Quality Inspection

↓

0100

Packaging
```

---

# Operation Types

Supports

- Cutting
- Machining
- Assembly
- Pressing
- Sanding
- Painting
- Packaging
- Inspection
- Transport
- Outsourced Operation

---

# Work Center Assignment

Each operation requires

- Work Center
- Capacity
- Shift Calendar
- Queue Rules

Reference

TASK-048_Work_Center.md

---

# Machine Assignment

Supports

- Primary Machine
- Alternative Machine
- Machine Priority
- Machine Capacity

Reference

TASK-049_Machine.md

---

# Tool Assignment

Supports

- Cutting Tools
- Saw Blades
- Milling Heads
- Drill Bits
- CNC Tools
- Fixtures

Reference

TASK-050_Tool.md

---

# Labor Assignment

Supports

- Operator
- Team
- Skill Group
- Minimum Operators
- Maximum Operators

---

# Time Management

Supports

- Setup Time
- Run Time
- Cycle Time
- Queue Time
- Move Time
- Waiting Time
- Cleaning Time

Example

```
Setup

20 min

+

Cycle

4 min

×

100 pcs

=

420 min
```

---

# Capacity Planning

Routing provides

- Machine Load
- Labor Load
- Production Duration
- Daily Capacity
- Weekly Capacity

Reference

Capacity Planning Module

---

# Alternative Routing

Supports

Alternative production methods.

Example

```
CLT Panel

↓

X-Press

or

Hydraulic Press
```

Selection depends on

- Machine Availability
- Capacity
- Product Type
- Customer Requirement

---

# Version Management

Supports

```
Version 1.0

↓

Version 1.1

↓

Version 2.0
```

Every version stores

- Engineering Notes
- Approval History
- Effective Dates

---

# Engineering Change

Supports

- Engineering Change Request
- Engineering Change Order
- Routing Revision
- Impact Analysis

Reference

Engineering Module

---

# Production Integration

Workflow

```
Sales Order

↓

Production Order

↓

Routing

↓

Operations

↓

Shop Floor
```

Reference

Production Module

---

# Cost Calculation

Supports

- Machine Cost
- Labor Cost
- Setup Cost
- Overhead Cost
- Operation Cost

Reference

Finance Module

---

# Quality Integration

Supports

Inspection Points

- Incoming Inspection
- In-Process Inspection
- Final Inspection

Each inspection links to Quality module.

Reference

Quality Module

---

# Attachments

Supports

- Process Sheets
- SOP Documents
- Machine Drawings
- Tool Lists
- Work Instructions
- Videos

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Routing Number
- Product
- Work Center
- Machine
- Revision
- Status
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active Routings
- Pending Approvals
- Engineering Changes
- Cycle Time Changes
- Capacity Utilization
- Obsolete Routings

Reference

Production Dashboard

---

# Reports

Supports

- Routing Register
- Operation List
- Cycle Time Analysis
- Capacity Report
- Routing Revision History
- Cost Analysis

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/routings

GET /api/v1/routings/{id}

POST /api/v1/routings

PUT /api/v1/routings/{id}

DELETE /api/v1/routings/{id}

POST /api/v1/routings/{id}/approve

POST /api/v1/routings/{id}/release

POST /api/v1/routings/{id}/revise

GET /api/v1/routings/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Routing Number is unique.
- Product exists.
- At least one operation exists.
- Operation sequence is unique.
- Work Center exists.
- Machine exists.
- Cycle Time > 0.
- Released Routings cannot be edited.
- Obsolete Routings cannot be assigned to Production Orders.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Engineering Authorization
- Production Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Routing Created
- Routing Updated
- Routing Approved
- Routing Released
- Routing Revised
- Operation Added
- Operation Removed
- Cycle Time Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Routing Approval Required
- Routing Released
- Engineering Change
- Routing Revision Published
- Capacity Warning

Reference

Notification_System.md

---

# Events

Publishes

- RoutingCreated
- RoutingApproved
- RoutingReleased
- RoutingRevised
- OperationAdded
- OperationUpdated
- RoutingObsolete

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Routing Lookup
- Operation List
- Work Instructions
- Machine Assignment
- Revision History

Routing editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Routing Save < 1 second
- Operation Load < 500 ms
- Search < 300 ms
- Revision Creation < 2 seconds
- Support 500,000+ Routings
- Support 500+ Operations per Routing

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1 — CLT Panel

```
Timber Preparation

↓

Finger Joint

↓

Planer

↓

Adhesive Application

↓

Layer Assembly

↓

X-Press

↓

Calibration

↓

Hundegger CNC

↓

Quality Control

↓

Packaging
```

---

### Example 2 — Thermowood

```
Timber Selection

↓

Kiln Drying

↓

Thermal Modification

↓

Cooling

↓

Conditioning

↓

Planer

↓

Profiling

↓

Quality Inspection

↓

Packaging
```

---

### Example 3 — Solid Panel

```
Lamella Preparation

↓

Finger Joint

↓

Edge Gluing

↓

Press

↓

Wide Belt Sanding

↓

CNC Processing

↓

Final Inspection

↓

Packaging
```

---

# Acceptance Criteria

The Routing module shall

- Define complete manufacturing operations.
- Support work center, machine and tool assignments.
- Support alternative routings.
- Manage engineering revisions and version history.
- Integrate with Production Planning and Capacity Planning.
- Calculate standard production times and operation costs.
- Publish routing lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-046_BOM.md
- TASK-048_Work_Center.md
- TASK-049_Machine.md
- TASK-050_Tool.md
- TASK-012_File_Upload.md
- Production_API.md
- Validation_Rules.md

---

# Related Documents

Production_Architecture.md

Production_API.md

Production_Workflow.md

TASK-046_BOM.md

TASK-048_Work_Center.md

TASK-049_Machine.md

TASK-050_Tool.md

TASK-051_Recipe.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Unit_Conversion.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md
