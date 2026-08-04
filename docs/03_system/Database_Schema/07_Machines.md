# Database Schema — Machines

**Project:** Naswood OS
**Document:** Machines Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Machines module manages all production equipment, machine groups, operating parameters, runtime information and production capabilities.

Machines are the primary execution resources within the manufacturing process.

Each Transformation is executed on one Machine.

---

# Philosophy

Machines do not own production.

Machines execute Operations.

Machines consume Recipes.

Machines generate Transformations.

Machines produce Events.

Machines require Maintenance.

---

# Entity List

Machine

MachineGroup

MachineCapability

MachineStatus

MachineParameter

MachineRuntime

MachineEnergy

MachineAlarm

---

# machine

Represents a physical production machine.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(150) |
| machine_type_id | UUID FK |
| machine_group_id | UUID FK |
| factory_id | UUID FK |
| manufacturer | VARCHAR(100) |
| model | VARCHAR(100) |
| serial_number | VARCHAR(100) |
| installation_date | DATE |
| status | VARCHAR(30) |
| active | BOOLEAN |

---

# machine_group

Logical grouping of machines.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |

Examples

- Log Processing
- Sawing
- Kiln Drying
- Thermowood
- Profiling
- Finger Joint
- Four Side Planer
- Panel Press
- Packaging
- Pellet

---

# machine_capability

Defines production capabilities.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| operation_type_id | UUID FK |
| supported_species_id | UUID FK |
| minimum_thickness | NUMERIC |
| maximum_thickness | NUMERIC |
| minimum_width | NUMERIC |
| maximum_width | NUMERIC |
| minimum_length | NUMERIC |
| maximum_length | NUMERIC |

---

# machine_status

Current operating state.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| status | VARCHAR(30) |
| started_at | TIMESTAMP |

Status Values

- Idle
- Running
- Setup
- Waiting Material
- Waiting Operator
- Planned Stop
- Maintenance
- Alarm
- Offline

---

# machine_parameter

Machine configuration values.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| parameter_name | VARCHAR(100) |
| parameter_value | VARCHAR(255) |
| unit | VARCHAR(20) |
| effective_from | TIMESTAMP |

Examples

- Feed Speed
- Spindle Speed
- Temperature
- Pressure
- Conveyor Speed

---

# machine_runtime

Stores machine operating statistics.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| runtime_minutes | INTEGER |
| idle_minutes | INTEGER |
| downtime_minutes | INTEGER |
| setup_minutes | INTEGER |
| recorded_date | DATE |

---

# machine_energy

Energy consumption.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| energy_type | VARCHAR(30) |
| quantity | NUMERIC |
| unit | VARCHAR(20) |
| recorded_at | TIMESTAMP |

Energy Types

- Electricity
- Biomass
- Natural Gas
- Compressed Air

---

# machine_alarm

Machine alarms.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| alarm_code | VARCHAR(50) |
| alarm_type | VARCHAR(50) |
| severity | VARCHAR(20) |
| message | TEXT |
| started_at | TIMESTAMP |
| ended_at | TIMESTAMP |
| acknowledged_by | UUID FK |

Severity

- Information
- Warning
- Critical

---

# Relationships

Machine Group

1 → N Machines

Machine

1 → N Machine Capabilities

Machine

1 → N Machine Parameters

Machine

1 → N Machine Runtime Records

Machine

1 → N Machine Energy Records

Machine

1 → N Machine Alarms

Machine

1 → N Transformations

Machine

1 → N Maintenance Records

Machine

1 → N Tool Assignments

---

# Business Rules

### BR-701

Every Machine shall belong to exactly one Machine Group.

---

### BR-702

Every Transformation shall reference the executing Machine.

---

### BR-703

Machine capabilities determine whether a Routing Operation can be assigned.

---

### BR-704

Machine status changes shall generate Events.

---

### BR-705

Machine alarms shall be recorded permanently.

---

### BR-706

Machine parameters are version-controlled.

Changes create new parameter records.

---

### BR-707

Energy consumption shall be measurable per Machine.

---

### BR-708

Machine runtime statistics support OEE calculations.

---

### BR-709

A Machine may execute only one Operation at a time unless explicitly configured for parallel processing.

---

### BR-710

Inactive Machines cannot receive new Work Orders.

---

# Integration

Machines integrate with:

- Production
- Routing
- Recipes
- Tooling
- Maintenance
- Quality
- Inventory
- Events
- Audit Log
- AI Planning

---

# Machine Philosophy

Machines are execution resources within the Manufacturing Operating System.

They execute Operations, consume Recipes, generate Transformations and provide the operational data required for planning, traceability, maintenance and continuous improvement.
