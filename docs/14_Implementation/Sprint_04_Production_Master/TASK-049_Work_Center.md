# TASK-049 — Work Center

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Manufacturing Resources

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Work Center module for Naswood OS.

The Work Center module defines the logical production units where manufacturing operations are performed. A Work Center groups one or more machines, operators and production resources that perform similar manufacturing processes.

It serves as the foundation for Routing, Capacity Planning, Scheduling, Production Orders, OEE and Cost Calculation.

---

# Objectives

- Centralized Work Center Management
- Capacity Planning
- Production Scheduling
- Machine Organization
- Labor Planning
- Cost Calculation
- Production Performance Monitoring

---

# Scope

The Work Center module includes

- Work Center Creation
- Capacity Definition
- Machine Assignment
- Labor Assignment
- Shift Calendar
- Cost Center Integration
- Performance Tracking
- OEE Aggregation
- Work Center Availability
- Work Center Lifecycle Management

Out of Scope

- Machine Maintenance
- Production Orders
- Shop Floor Execution
- Inventory Transactions

---

# Work Center Architecture

```
Factory

↓

Plant

↓

Production Area

↓

Work Center

↓

Machine

↓

Production Operation
```

---

# Work Center Lifecycle

```
Draft

↓

Configured

↓

Approved

↓

Active

↓

Maintenance

↓

Inactive

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Work Center Categories

Supports

- Timber Preparation
- Finger Joint
- Planer
- Lamella Production
- Glue Application
- Press Line
- CNC Processing
- Thermowood Line
- Pellet Production
- Sanding
- Packaging
- Quality Inspection

---

# Work Center Header

Each Work Center contains

## General Information

- Work Center Code
- Work Center Name
- Description
- Company
- Plant
- Production Area
- Cost Center
- Status

---

## Operational Information

- Work Center Type
- Production Line
- Shift Model
- Standard Calendar
- Supervisor
- Responsible Department

---

## Capacity Information

- Available Hours / Day
- Number of Machines
- Number of Operators
- Standard Capacity
- Maximum Capacity
- Utilization Target

Reference

Unit_Conversion.md

---

# Machine Assignment

Each Work Center may contain

- Primary Machines
- Alternative Machines
- Backup Machines

Supports

- One-to-Many relationship
- Dynamic machine allocation
- Capacity balancing

Reference

TASK-048_Machine.md

---

# Labor Assignment

Supports

- Operators
- Technicians
- Shift Leaders
- Maintenance Personnel

Stores

- Required Skill Group
- Minimum Operators
- Maximum Operators
- Standard Labor Hours

---

# Shift Management

Supports

- Single Shift
- Two Shifts
- Three Shifts
- Weekend Production
- Overtime

Example

```
Shift 1

08:00 - 16:00

↓

Shift 2

16:00 - 00:00

↓

Shift 3

00:00 - 08:00
```

---

# Capacity Planning

Calculates

- Hourly Capacity
- Daily Capacity
- Weekly Capacity
- Monthly Capacity

Based on

- Machine Capacity
- Available Hours
- Operator Availability
- Shift Calendar
- Efficiency Factor

---

# Routing Integration

Every Routing operation references one Work Center.

Example

```
Operation

↓

Finger Joint

↓

Work Center

FJ-01
```

Reference

TASK-047_Routing.md

---

# Production Integration

Production Orders are scheduled against Work Centers.

```
Production Order

↓

Routing Operation

↓

Work Center

↓

Machine Assignment

↓

Execution
```

Reference

Production Module

---

# Cost Center Integration

Each Work Center links to one Cost Center.

Tracks

- Labor Cost
- Machine Cost
- Energy Cost
- Overhead Cost
- Total Hourly Cost

Reference

Finance Module

---

# OEE Aggregation

Calculates Work Center OEE using all assigned machines.

Displays

- Availability
- Performance
- Quality
- Overall OEE

Supports

- Daily OEE
- Weekly OEE
- Monthly OEE

Reference

TASK-048_Machine.md

---

# Performance Monitoring

Tracks

- Production Output
- Downtime
- Utilization
- Efficiency
- Throughput
- Bottlenecks

---

# Availability

Supports

- Available
- Busy
- Planned Maintenance
- Breakdown
- Offline

Used by Production Planning.

---

# Quality Integration

Supports

- Inspection Work Centers
- Quality Approval Points
- Rework Centers

Reference

Quality Module

---

# Attachments

Supports

- Work Instructions
- Layout Drawings
- SOP Documents
- Safety Instructions
- Photos
- Videos

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Work Center Code
- Work Center Name
- Production Area
- Machine
- Supervisor
- Status
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active Work Centers
- Capacity Utilization
- Current Workload
- OEE
- Bottlenecks
- Downtime
- Production Efficiency

Reference

Production Dashboard

---

# Reports

Supports

- Work Center Register
- Capacity Report
- Utilization Report
- OEE Report
- Production Performance
- Cost Center Analysis
- Bottleneck Report

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/work-centers

GET /api/v1/work-centers/{id}

POST /api/v1/work-centers

PUT /api/v1/work-centers/{id}

DELETE /api/v1/work-centers/{id}

GET /api/v1/work-centers/capacity

GET /api/v1/work-centers/utilization

POST /api/v1/work-centers/{id}/activate

POST /api/v1/work-centers/{id}/deactivate

GET /api/v1/work-centers/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Work Center Code is unique.
- Company exists.
- Plant exists.
- Production Area exists.
- Capacity > 0.
- Assigned Machines belong to the same Plant.
- Active Work Centers require at least one Machine.
- Archived Work Centers cannot be assigned to Routing.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Production Authorization
- Engineering Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Work Center Created
- Updated
- Activated
- Deactivated
- Capacity Changed
- Machine Assigned
- Machine Removed
- Supervisor Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Capacity Overload
- Low Utilization
- Work Center Activated
- Maintenance Scheduled
- Bottleneck Detected

Reference

Notification_System.md

---

# Events

Publishes

- WorkCenterCreated
- WorkCenterUpdated
- WorkCenterActivated
- WorkCenterCapacityChanged
- MachineAssigned
- MachineRemoved
- WorkCenterDeactivated

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Work Center Lookup
- Capacity View
- Assigned Machines
- OEE Dashboard
- Production Status

Work Center editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Work Center Save < 1 second
- Capacity Calculation < 1 second
- Search < 300 ms
- OEE Aggregation < 2 seconds
- Support 100,000+ Work Centers
- Support real-time production monitoring

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Work Center

CLT-PRESS-01

↓

Machines

Ledinek X-Press

↓

Capacity

200 m³/Day

↓

3 Operators
```

---

### Example 2

```
Work Center

CNC-01

↓

Machine

Hundegger PBA

↓

Operations

CNC Machining

↓

OEE

92%
```

---

### Example 3

```
Work Center

THERMO-01

↓

Machines

Thermowood Kilns

↓

Capacity

120 m³/Batch

↓

24-Hour Operation
```

---

# Acceptance Criteria

The Work Center module shall

- Maintain centralized Work Center master data.
- Support machine and labor assignments.
- Calculate production capacity.
- Integrate with Routing, Production Planning and Machines.
- Aggregate OEE and utilization metrics.
- Track Work Center availability and performance.
- Publish Work Center lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-047_Routing.md
- TASK-048_Machine.md
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

TASK-047_Routing.md

TASK-048_Machine.md

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
