# Database Schema — Maintenance

**Project:** Naswood OS
**Document:** Maintenance Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Maintenance module manages all maintenance activities related to production assets, machines, tooling and supporting equipment.

Its objectives are:

- Maximize equipment availability
- Minimize unplanned downtime
- Preserve production quality
- Extend equipment life
- Support predictive maintenance

Maintenance integrates directly with Production, Machines, Tooling and Inventory.

---

# Philosophy

Machines execute production.

Maintenance ensures machines remain capable of production.

Maintenance is preventive rather than reactive.

Every maintenance activity contributes to production reliability.

---

# Entity List

Asset

AssetComponent

MaintenancePlan

MaintenanceTask

MaintenanceWorkOrder

MaintenanceExecution

MaintenanceDowntime

MaintenanceFailure

MaintenanceSparePart

MaintenanceHistory

ConditionMonitoring

---

# asset

Represents a maintainable asset.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| asset_code | VARCHAR(30) |
| name | VARCHAR(150) |
| asset_type | VARCHAR(50) |
| manufacturer | VARCHAR(100) |
| model | VARCHAR(100) |
| serial_number | VARCHAR(100) |
| installation_date | DATE |
| expected_life_hours | INTEGER |
| status | VARCHAR(30) |

Asset Types

- Machine
- Kiln
- Thermowood Kiln
- Panel Press
- Conveyor
- Compressor
- Dust Collection
- Hydraulic Unit
- Electrical Panel
- PLC
- Sensor

---

# asset_component

Replaceable component belonging to an Asset.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| component_name | VARCHAR(150) |
| component_code | VARCHAR(50) |
| expected_life | INTEGER |
| life_unit | VARCHAR(20) |
| status | VARCHAR(30) |

Examples

- Bearing
- Belt
- Motor
- Chain
- Cylinder
- Valve
- Sensor
- Hydraulic Pump

---

# maintenance_plan

Defines recurring maintenance plans.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| plan_name | VARCHAR(150) |
| maintenance_type | VARCHAR(30) |
| frequency_type | VARCHAR(30) |
| frequency_value | INTEGER |
| estimated_duration_minutes | INTEGER |
| active | BOOLEAN |

Maintenance Types

- Preventive
- Predictive
- Corrective
- Inspection
- Calibration

Frequency Types

- Hours
- Days
- Weeks
- Months
- Production Cycles
- Running Meters

---

# maintenance_task

Individual tasks within a maintenance plan.

| Field | Type |
|--------|------|
| id | UUID |
| maintenance_plan_id | UUID FK |
| sequence | INTEGER |
| description | TEXT |
| estimated_minutes | INTEGER |
| requires_shutdown | BOOLEAN |

---

# maintenance_work_order

Generated maintenance work order.

| Field | Type |
|--------|------|
| id | UUID |
| work_order_number | VARCHAR(30) |
| asset_id | UUID FK |
| maintenance_plan_id | UUID FK |
| priority | VARCHAR(20) |
| status | VARCHAR(30) |
| planned_start | TIMESTAMP |
| planned_finish | TIMESTAMP |

Priority

- Low
- Normal
- High
- Critical

Status

- Planned
- Released
- In Progress
- Completed
- Cancelled

---

# maintenance_execution

Actual execution of maintenance.

| Field | Type |
|--------|------|
| id | UUID |
| maintenance_work_order_id | UUID FK |
| technician_id | UUID FK |
| started_at | TIMESTAMP |
| completed_at | TIMESTAMP |
| duration_minutes | INTEGER |
| result | VARCHAR(30) |
| remarks | TEXT |

Results

- Completed
- Partial
- Failed

---

# maintenance_downtime

Records production downtime caused by maintenance.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| work_order_id | UUID FK |
| downtime_reason | VARCHAR(100) |
| started_at | TIMESTAMP |
| ended_at | TIMESTAMP |
| duration_minutes | INTEGER |

Downtime Reasons

- Planned Maintenance
- Mechanical Failure
- Electrical Failure
- Hydraulic Failure
- Pneumatic Failure
- PLC Failure
- Tool Change
- Calibration

---

# maintenance_failure

Root cause analysis.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| maintenance_execution_id | UUID FK |
| failure_code | VARCHAR(50) |
| failure_category | VARCHAR(50) |
| root_cause | TEXT |
| corrective_action | TEXT |

Failure Categories

- Mechanical
- Electrical
- Hydraulic
- Pneumatic
- Software
- Operator
- Tooling
- Material

---

# maintenance_spare_part

Spare parts consumed during maintenance.

| Field | Type |
|--------|------|
| id | UUID |
| maintenance_execution_id | UUID FK |
| material_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_id | UUID FK |

---

# maintenance_history

Historical maintenance records.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| maintenance_execution_id | UUID FK |
| summary | TEXT |
| recorded_at | TIMESTAMP |

---

# condition_monitoring

Stores machine condition data.

| Field | Type |
|--------|------|
| id | UUID |
| asset_id | UUID FK |
| parameter | VARCHAR(100) |
| value | NUMERIC |
| unit | VARCHAR(20) |
| recorded_at | TIMESTAMP |

Examples

- Vibration
- Bearing Temperature
- Motor Temperature
- Hydraulic Pressure
- Oil Level
- Current
- Voltage
- Humidity

---

# Relationships

Asset

1 → N Components

Asset

1 → N Maintenance Plans

Maintenance Plan

1 → N Maintenance Tasks

Maintenance Plan

1 → N Maintenance Work Orders

Maintenance Work Order

1 → 1 Maintenance Execution

Maintenance Execution

1 → N Spare Parts

Maintenance Execution

1 → N Failures

Asset

1 → N Downtime Records

Asset

1 → N Condition Monitoring Records

Asset

1 → N Maintenance History

---

# Business Rules

### BR-901

Every production asset shall have a maintenance strategy.

---

### BR-902

Preventive maintenance shall be planned automatically.

---

### BR-903

Corrective maintenance shall record the root cause.

---

### BR-904

Maintenance activities affecting production shall create Production Downtime records.

---

### BR-905

Spare parts consumed during maintenance shall create Inventory Movements.

---

### BR-906

Completed maintenance shall update Asset History.

---

### BR-907

Condition Monitoring data is immutable.

---

### BR-908

Maintenance Work Orders shall generate Audit Logs.

---

### BR-909

Critical failures shall generate Business Events.

---

### BR-910

Maintenance history shall never be deleted.

---

# Integration

Maintenance integrates with:

- Machines
- Tooling
- Production
- Inventory
- Quality
- Purchasing
- Audit Log
- Events
- AI Predictive Maintenance

---

# Future Extensions

The architecture supports:

- PLC Integration
- OPC-UA
- Vibration Sensors
- Thermal Cameras
- Automatic Lubrication Monitoring
- Oil Analysis
- AI Predictive Maintenance
- Remaining Useful Life (RUL) Estimation
- Digital Maintenance Manuals
- QR Code Asset Identification

---

# Maintenance Philosophy

Maintenance is not a repair activity.

Maintenance is a production capability.

Reliable maintenance enables reliable manufacturing.

Every maintenance action preserves equipment performance, production continuity and product quality.
