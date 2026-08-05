# TASK-054 — Operation

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Manufacturing Process

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Planned

---

# Purpose

Develop the Production Operation module for Naswood OS.

The Operation module defines standardized manufacturing operations that are used inside Routings and Production Orders. Every manufacturing step—from cutting to packaging—is represented as an Operation with its own execution parameters, resources, quality requirements and standard production times.

Operations serve as reusable production templates shared across products and routings.

---

# Objectives

- Standardized Manufacturing Operations
- Reusable Operation Library
- Process Standardization
- Resource Planning
- Capacity Calculation
- Quality Integration
- Production Traceability

---

# Scope

The Operation module includes

- Operation Master
- Standard Operations
- Operation Parameters
- Resource Requirements
- Standard Times
- Quality Checkpoints
- Safety Requirements
- Version Management
- Operation Approval
- Operation History

Out of Scope

- Production Orders
- Machine Maintenance
- Material Inventory
- Purchasing

---

# Operation Architecture

```
Product

↓

Routing

↓

Operation

↓

Work Center

↓

Machine

↓

Tool

↓

Execution
```

---

# Operation Lifecycle

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

# Operation Categories

Supports

- Material Preparation
- Cutting
- Cross Cutting
- Multi Rip Saw
- Finger Joint
- Planing
- Sanding
- Glue Application
- Pressing
- CNC Processing
- Profiling
- Drilling
- Assembly
- Quality Inspection
- Packaging
- Internal Transport
- Outsourced Operation

---

# Operation Header

Each Operation contains

## General Information

- Operation Code
- Operation Name
- Operation Category
- Company
- Plant
- Version
- Revision
- Status

---

## Production Information

- Standard Work Center
- Preferred Machine
- Standard Tool
- Standard Operator Count
- Batch Size
- Production Notes

---

# Standard Times

Stores

- Setup Time
- Run Time
- Cycle Time
- Changeover Time
- Queue Time
- Move Time
- Inspection Time
- Cleaning Time

Example

```
Setup

15 Minutes

+

Cycle

2.5 Minutes

×

200 Pieces

=

515 Minutes
```

---

# Resource Requirements

Each operation defines

- Work Center
- Machine
- Tool
- Operators
- Energy Requirement
- Compressed Air
- Hydraulic Requirement

Reference

TASK-048_Machine.md

TASK-049_Work_Center.md

TASK-053_Tooling.md

---

# Production Parameters

Supports

- Feed Speed
- Cutting Speed
- Spindle RPM
- Temperature
- Pressure
- Humidity
- Glue Consumption
- Press Time
- CNC Program

Parameters may vary by product.

---

# Material Consumption

Supports

- Primary Material
- Consumables
- Adhesives
- Packaging
- Auxiliary Materials

Reference

TASK-046_BOM.md

---

# Quality Control

Each operation may define

- Inspection Point
- Sampling Frequency
- Acceptance Criteria
- Measurement Method
- Tolerance
- Required Documentation

Reference

Quality Module

---

# Safety Requirements

Supports

- PPE Requirements
- Lockout / Tagout
- Safety Instructions
- Machine Safety Checks
- Environmental Controls

---

# Alternative Operations

Supports

- Alternative Machine
- Alternative Tool
- Alternative Work Center
- Alternative Process

Selection based on

- Capacity
- Availability
- Product Type

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

Each version stores

- Engineering Notes
- Revision Reason
- Approval History
- Effective Dates

---

# Routing Integration

Operations are reusable within multiple Routings.

```
Operation Library

↓

Routing

↓

Production Order

↓

Execution
```

Reference

TASK-047_Routing.md

---

# Production Planning Integration

Operations provide

- Standard Production Time
- Capacity Requirement
- Labor Requirement
- Machine Requirement

Reference

Production Planning Module

---

# MES Integration

Supports

- Operation Start
- Operation Finish
- Production Count
- Downtime
- Scrap Quantity
- Operator Login

Reference

MES Module

---

# Cost Calculation

Calculates

- Labor Cost
- Machine Cost
- Tool Cost
- Energy Cost
- Overhead Cost
- Standard Operation Cost

Reference

Finance Module

---

# Attachments

Supports

- SOP Documents
- Process Sheets
- Machine Setup Sheets
- Photos
- Videos
- Work Instructions

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Operation Code
- Operation Name
- Category
- Work Center
- Machine
- Status
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active Operations
- Standard Times
- Cycle Time Variance
- Operations Awaiting Approval
- Resource Utilization
- Engineering Changes

Reference

Production Dashboard

---

# Reports

Supports

- Operation Register
- Standard Time Report
- Resource Utilization
- Operation Cost Report
- Revision History
- Operation Performance

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/operations

GET /api/v1/operations/{id}

POST /api/v1/operations

PUT /api/v1/operations/{id}

DELETE /api/v1/operations/{id}

POST /api/v1/operations/{id}/approve

POST /api/v1/operations/{id}/release

POST /api/v1/operations/{id}/revise

GET /api/v1/operations/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Operation Code is unique.
- Category exists.
- Work Center exists.
- Machine exists.
- Cycle Time > 0.
- Setup Time ≥ 0.
- Released Operations cannot be edited.
- Obsolete Operations cannot be assigned to Routings.

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

- Operation Created
- Operation Updated
- Operation Approved
- Operation Released
- Operation Revised
- Resource Changed
- Standard Time Updated

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Operation Approval Required
- Operation Released
- Engineering Revision
- Standard Time Updated
- Resource Conflict

Reference

Notification_System.md

---

# Events

Publishes

- OperationCreated
- OperationApproved
- OperationReleased
- OperationRevised
- OperationUpdated
- StandardTimeChanged
- ResourceAssigned

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Operation Lookup
- SOP Viewing
- Work Instructions
- Machine Setup Guide
- Quality Checklist

Operation editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Operation Save < 1 second
- Search < 300 ms
- Standard Time Calculation < 500 ms
- Resource Lookup < 500 ms
- Support 1,000,000+ Operations

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1 — Finger Joint

```
Operation

Finger Joint

↓

Machine

Weinig HS120

↓

Cycle Time

2.8 min

↓

Operator

2
```

---

### Example 2 — CLT Pressing

```
Operation

Cross Layer Pressing

↓

Machine

Ledinek X-Press

↓

Press Time

45 Minutes

↓

Temperature Controlled
```

---

### Example 3 — CNC Processing

```
Operation

CNC Machining

↓

Machine

Hundegger PBA

↓

Program

CLT_WALL_001

↓

Automatic Tool Change
```

---

# Acceptance Criteria

The Operation module shall

- Maintain a reusable production operation library.
- Define standard resources and production parameters.
- Support standard production time calculations.
- Integrate with Routing, Machines, Work Centers and MES.
- Support quality checkpoints and safety requirements.
- Maintain complete revision history.
- Publish operation lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-046_BOM.md
- TASK-047_Routing.md
- TASK-048_Machine.md
- TASK-049_Work_Center.md
- TASK-053_Tooling.md
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

TASK-049_Work_Center.md

TASK-050_Production_Line.md

TASK-051_Shift.md

TASK-052_Calendar.md

TASK-053_Tooling.md

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
