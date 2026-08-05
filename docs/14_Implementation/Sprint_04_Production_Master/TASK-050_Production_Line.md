# TASK-050 — Production Line

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Manufacturing Resources

**Priority:** Critical

**Estimated Effort:** 9 Days

**Status:** Completed

---

# Purpose

Develop the Production Line module for Naswood OS.

The Production Line module manages complete manufacturing lines consisting of multiple work centers, machines, operators and automation systems. It defines production flow, line capacity, balancing, bottlenecks and real-time operational status.

Production Lines are the highest operational manufacturing units used by Production Planning, MES, Capacity Planning, OEE and Digital Twin.

---

# Objectives

- Centralized Production Line Management
- Production Flow Definition
- Line Capacity Planning
- Line Balancing
- OEE Monitoring
- Digital Twin Integration
- Real-Time Production Visibility

---

# Scope

The Production Line module includes

- Production Line Creation
- Line Configuration
- Work Center Assignment
- Machine Assignment
- Capacity Definition
- Line Balancing
- Production Flow
- OEE Aggregation
- Line Scheduling
- Line Lifecycle Management

Out of Scope

- Production Orders
- Machine Maintenance
- Inventory Transactions
- Material Planning

---

# Production Line Architecture

```
Company

↓

Plant

↓

Production Line

↓

Work Center

↓

Machine

↓

Production Operation
```

---

# Production Line Lifecycle

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

# Production Line Types

Supports

- CLT Production Line
- Glulam Production Line
- Thermowood Production Line
- Solid Panel Line
- Finger Joint Line
- Pellet Production Line
- Packaging Line
- Custom Production Line

---

# Production Line Header

Each Production Line contains

## General Information

- Production Line Code
- Production Line Name
- Description
- Company
- Plant
- Production Type
- Status

---

## Operational Information

- Line Manager
- Department
- Shift Model
- Standard Calendar
- Cost Center
- Target OEE

---

## Capacity Information

- Hourly Capacity
- Daily Capacity
- Weekly Capacity
- Monthly Capacity
- Annual Capacity
- Standard Cycle Time
- Target Throughput

Reference

Unit_Conversion.md

---

# Work Center Assignment

Each Production Line contains one or more Work Centers.

Example

```
CLT Line

↓

Timber Preparation

↓

Finger Joint

↓

Planer

↓

Glue Application

↓

Layer Assembly

↓

X-Press

↓

Calibration

↓

Hundegger CNC

↓

Packaging
```

Reference

TASK-049_Work_Center.md

---

# Machine Assignment

Each Work Center contains one or more Machines.

Supports

- Primary Machine
- Alternative Machine
- Backup Machine

Reference

TASK-048_Machine.md

---

# Production Flow

Defines the complete manufacturing sequence.

Supports

- Sequential Flow
- Parallel Flow
- Alternative Flow
- Rework Flow

---

# Line Balancing

Calculates

- Bottleneck Operation
- Idle Time
- Throughput
- Line Efficiency
- Resource Utilization

Supports automatic balancing recommendations.

---

# Capacity Planning

Calculates

- Available Capacity
- Planned Capacity
- Remaining Capacity
- Utilization %
- Production Forecast

Based on

- Shift Calendar
- Machine Availability
- Operator Availability
- Planned Downtime

Reference

Capacity Planning Module

---

# Routing Integration

Routing operations are executed on Production Lines.

```
Routing

↓

Production Line

↓

Work Centers

↓

Machines

↓

Execution
```

Reference

TASK-047_Routing.md

---

# Production Planning Integration

Production Orders are assigned to Production Lines.

Supports

- Automatic Line Assignment
- Manual Scheduling
- Capacity Validation
- Priority Scheduling

Reference

Production Planning Module

---

# OEE Aggregation

Calculates Production Line OEE using all assigned machines.

Displays

- Availability
- Performance
- Quality
- Overall OEE

Supports

- Shift OEE
- Daily OEE
- Weekly OEE
- Monthly OEE

---

# Digital Twin Integration

Supports

- Live Production Flow
- Machine Status
- Material Flow
- Capacity Simulation
- Bottleneck Simulation
- Predictive Analysis

Reference

Digital Twin Module

---

# MES Integration

Supports

- Live Production Monitoring
- Production Counts
- Downtime Tracking
- Operator Login
- Alarm Monitoring

Reference

MES Module

---

# Quality Integration

Supports

- In-Line Inspection
- Quality Gates
- Final Inspection
- Rework Stations

Reference

Quality Module

---

# Energy Monitoring

Tracks

- Total Energy Consumption
- Energy per Product
- Energy per Shift
- Line Efficiency

Supports sustainability reporting.

---

# Attachments

Supports

- Line Layout
- Process Flow Diagram
- SOP Documents
- Safety Procedures
- Electrical Drawings
- Videos

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Production Line Code
- Production Line Name
- Production Type
- Plant
- Work Center
- Status

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active Production Lines
- Production Status
- Capacity Utilization
- OEE
- Bottlenecks
- Downtime
- Throughput
- Shift Performance

Reference

Production Dashboard

---

# Reports

Supports

- Production Line Register
- Capacity Report
- OEE Report
- Throughput Analysis
- Bottleneck Analysis
- Energy Consumption
- Production Efficiency

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/production-lines

GET /api/v1/production-lines/{id}

POST /api/v1/production-lines

PUT /api/v1/production-lines/{id}

DELETE /api/v1/production-lines/{id}

GET /api/v1/production-lines/capacity

GET /api/v1/production-lines/oee

GET /api/v1/production-lines/status

POST /api/v1/production-lines/{id}/activate

POST /api/v1/production-lines/{id}/deactivate

GET /api/v1/production-lines/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Production Line Code is unique.
- Company exists.
- Plant exists.
- At least one Work Center is assigned.
- Capacity > 0.
- Work Centers belong to the same Plant.
- Active Production Lines require at least one Machine.
- Archived Production Lines cannot receive Production Orders.

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

- Production Line Created
- Updated
- Activated
- Deactivated
- Capacity Changed
- Work Center Assigned
- Work Center Removed
- OEE Target Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Line Capacity Exceeded
- OEE Below Target
- Production Delay
- Bottleneck Detected
- Line Activated
- Maintenance Scheduled

Reference

Notification_System.md

---

# Events

Publishes

- ProductionLineCreated
- ProductionLineUpdated
- ProductionLineActivated
- ProductionLineCapacityChanged
- WorkCenterAssigned
- BottleneckDetected
- ProductionLineDeactivated

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Production Line Overview
- Live Status
- OEE Dashboard
- Bottleneck Alerts
- Shift Performance
- QR Code Identification

Production Line editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Production Line Save < 1 second
- Capacity Calculation < 1 second
- OEE Aggregation < 2 seconds
- Search < 300 ms
- Support 10,000+ Production Lines
- Support Real-Time MES Monitoring

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1 — CLT Production Line

```
Log Sorting

↓

Saw Line

↓

Kiln Drying

↓

Finger Joint

↓

Planer

↓

Glue Application

↓

Cross Layer Assembly

↓

Ledinek X-Press

↓

Calibration

↓

Hundegger PBA CNC

↓

Packaging
```

---

### Example 2 — Thermowood Production Line

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

Profiling

↓

Quality Inspection

↓

Packaging
```

---

### Example 3 — Pellet Production Line

```
Wood Waste

↓

Crusher

↓

Dryer

↓

Hammer Mill

↓

Pellet Press

↓

Cooling

↓

Screening

↓

Bagging

↓

Palletizing
```

---

# Acceptance Criteria

The Production Line module shall

- Maintain centralized Production Line master data.
- Manage complete production flow definitions.
- Support Work Center and Machine assignments.
- Calculate line capacity and throughput.
- Aggregate OEE across the entire production line.
- Integrate with Routing, MES and Production Planning.
- Support Digital Twin simulations.
- Publish Production Line lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-047_Routing.md
- TASK-048_Machine.md
- TASK-049_Work_Center.md
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

TASK-051_Tool.md

TASK-052_Recipe.md

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
