# TASK-048 — Machine

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Manufacturing Resources

**Priority:** Critical

**Estimated Effort:** 9 Days

**Status:** Planned

---

# Purpose

Develop the Machine Master module for Naswood OS.

The Machine module manages all production equipment used throughout the manufacturing process. It provides centralized management of machine specifications, production capacities, maintenance status, utilization and real-time operational data.

The Machine module is a core component of Production Planning, Capacity Planning, Maintenance, MES and OEE calculations.

---

# Objectives

- Centralized Machine Management
- Production Capacity Management
- Machine Availability Monitoring
- OEE Integration
- Maintenance Integration
- Production Scheduling Support
- Complete Equipment Traceability

---

# Scope

The Machine module includes

- Machine Registration
- Machine Classification
- Machine Specifications
- Capacity Management
- Machine Status
- Maintenance Integration
- OEE Tracking
- Tool Compatibility
- IoT Integration
- Machine Lifecycle Management

Out of Scope

- Production Orders
- Machine Maintenance Planning
- Spare Parts Management
- PLC Programming

---

# Machine Architecture

```
Factory

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

# Machine Lifecycle

```
Planned

↓

Installed

↓

Commissioned

↓

Active

↓

Maintenance

↓

Idle

↓

Out of Service

↓

Retired
```

Reference

Status_Lifecycle.md

---

# Machine Categories

Supports

- Cross Cut Saw
- Band Saw
- Multi Rip Saw
- Finger Joint Line
- Planer
- Wide Belt Sander
- Press
- CNC Machine
- Thermowood Kiln
- Pellet Line
- Packaging Machine
- Robot
- Conveyor

---

# Machine Header

Each machine contains

## General Information

- Machine Code
- Machine Name
- Machine Category
- Manufacturer
- Model
- Serial Number
- Asset Number
- Company
- Plant
- Work Center
- Status

---

## Technical Specifications

- Rated Power (kW)
- Voltage
- Air Pressure
- Hydraulic Pressure
- Maximum Capacity
- Working Width
- Working Height
- Maximum Length
- Maximum Weight
- Installation Date

---

## Production Information

- Standard Capacity
- Cycle Time
- Setup Time
- Maximum Daily Capacity
- Shift Capacity
- Production Notes

Reference

Unit_Conversion.md

---

# Machine Status

Supports

- Running
- Idle
- Setup
- Planned Stop
- Maintenance
- Breakdown
- Cleaning
- Calibration
- Offline

Real-time status updates supported.

---

# Capacity Management

Stores

- Hourly Capacity
- Daily Capacity
- Weekly Capacity
- Monthly Capacity
- Maximum Throughput
- Utilization %

Capacity is used by Scheduling and MRP.

---

# Machine Parameters

Supports

- Feed Speed
- Cutting Speed
- Spindle Speed
- Temperature
- Pressure
- Humidity
- Feed Rate
- Energy Consumption

Parameters may be collected automatically through IoT.

---

# Work Center Integration

Each machine belongs to

- One Work Center

Supports

- Primary Machine
- Backup Machine
- Alternative Machine

Reference

TASK-049_Work_Center.md

---

# Routing Integration

Each routing operation may specify

- Preferred Machine
- Alternative Machine
- Required Machine Type

Reference

TASK-047_Routing.md

---

# Tool Compatibility

Stores

- Compatible Tools
- Tool Holders
- Cutter Heads
- Saw Blades
- CNC Tool Library

Reference

TASK-050_Tool.md

---

# OEE Integration

Calculates

- Availability
- Performance
- Quality
- Overall Equipment Effectiveness

Formula

```
Availability

×

Performance

×

Quality

=

OEE
```

Supports

- Real-Time OEE
- Daily OEE
- Weekly OEE
- Monthly OEE

---

# Maintenance Integration

Displays

- Preventive Maintenance
- Predictive Maintenance
- Corrective Maintenance
- Calibration Schedule
- Maintenance History

Reference

Maintenance Module

---

# IoT Integration

Supports

- PLC Connection
- OPC-UA
- Modbus TCP
- MQTT
- REST API
- Machine Sensors

Real-time monitoring includes

- Machine Status
- Production Count
- Alarm Codes
- Temperature
- Energy Usage

---

# Alarm Management

Supports

- Machine Fault
- Emergency Stop
- Sensor Failure
- Overload
- Safety Door Open
- Temperature Alarm
- Pressure Alarm

Alarm history is retained.

---

# Energy Monitoring

Tracks

- Instant Consumption
- Daily Consumption
- Monthly Consumption
- Energy per Product
- Energy Efficiency

---

# Attachments

Supports

- Machine Manual
- Electrical Drawings
- Hydraulic Schematics
- PLC Backup
- Maintenance Instructions
- Certificates

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Machine Code
- Machine Name
- Manufacturer
- Model
- Work Center
- Status
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Running Machines
- Idle Machines
- Machines in Maintenance
- OEE
- Capacity Utilization
- Machine Availability
- Active Alarms

Reference

Production Dashboard

---

# Reports

Supports

- Machine Register
- Capacity Report
- OEE Report
- Machine Utilization
- Downtime Analysis
- Alarm History
- Energy Consumption

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/machines

GET /api/v1/machines/{id}

POST /api/v1/machines

PUT /api/v1/machines/{id}

DELETE /api/v1/machines/{id}

GET /api/v1/machines/status

GET /api/v1/machines/oee

GET /api/v1/machines/capacity

POST /api/v1/machines/{id}/activate

POST /api/v1/machines/{id}/deactivate

GET /api/v1/machines/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Machine Code is unique.
- Work Center exists.
- Machine Category exists.
- Capacity > 0.
- Installation Date is valid.
- Active machine cannot belong to multiple work centers.
- Retired machines cannot be assigned to routings.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Production Authorization
- Maintenance Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Machine Created
- Machine Updated
- Machine Activated
- Machine Deactivated
- Capacity Changed
- Status Changed
- Machine Retired

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Machine Breakdown
- Preventive Maintenance Due
- OEE Below Target
- Capacity Overload
- Machine Activated
- Machine Retired

Reference

Notification_System.md

---

# Events

Publishes

- MachineCreated
- MachineUpdated
- MachineActivated
- MachineStatusChanged
- MachineBreakdown
- MachineCapacityChanged
- MachineRetired

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Machine Lookup
- Live Status
- OEE Dashboard
- Alarm Viewing
- QR Code Identification
- Maintenance History

Machine editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Machine Lookup < 300 ms
- Machine Save < 1 second
- Status Refresh < 500 ms
- OEE Calculation < 2 seconds
- Support 100,000+ machines
- Support real-time telemetry

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Hundegger PBA

↓

Work Center

CNC Processing

↓

Capacity

120 Panels/Day

↓

Status

Running
```

---

### Example 2

```
Ledinek X-Press

↓

Capacity

200 m³/Day

↓

OEE

91%

↓

Active
```

---

### Example 3

```
Thermowood Kiln

↓

Temperature Monitoring

↓

IoT Sensors

↓

Energy Tracking

↓

Preventive Maintenance
```

---

# Acceptance Criteria

The Machine module shall

- Maintain centralized machine master data.
- Support production capacity management.
- Track machine status and availability.
- Calculate and display OEE metrics.
- Integrate with Routing, Work Centers and Maintenance.
- Support IoT-based real-time monitoring.
- Publish machine lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-047_Routing.md
- TASK-049_Work_Center.md
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

TASK-049_Work_Center.md

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
