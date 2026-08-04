# Database Schema — Production

**Project:** Naswood OS
**Document:** Production Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Production module manages manufacturing execution.

It is responsible for:

- Production Planning
- Production Orders
- Work Orders
- Operations
- Routing Execution
- Recipe Execution
- Machine Execution
- Operator Assignment
- Production Scheduling
- Production Monitoring

Production execution creates Transformations.

Transformations create Materials.

---

# Entity List

ProductionOrder

ProductionOrderLine

WorkOrder

Operation

OperationDependency

ProductionBatch

ProductionSchedule

RoutingExecution

RecipeExecution

MachineExecution

OperatorExecution

Transformation

TransformationInput

TransformationOutput

ProductionDelay

ProductionLoss

ProductionProgress

---

# production_order

Represents manufacturing demand.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| sales_order_id | UUID FK |
| production_strategy_id | UUID FK |
| factory_id | UUID FK |
| status | VARCHAR(20) |
| priority | INTEGER |
| requested_date | DATE |
| planned_start | TIMESTAMP |
| planned_finish | TIMESTAMP |
| created_by | UUID FK |

---

# production_order_line

Each Production Order may contain multiple products.

| Field | Type |
|--------|------|
| id | UUID |
| production_order_id | UUID FK |
| product_id | UUID FK |
| product_variant_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_id | UUID FK |
| quality_grade_id | UUID FK |
| due_date | DATE |

---

# work_order

Represents executable work.

One Production Order generates one or more Work Orders.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| production_order_id | UUID FK |
| routing_id | UUID FK |
| machine_group_id | UUID FK |
| shift_id | UUID FK |
| planned_start | TIMESTAMP |
| planned_finish | TIMESTAMP |
| actual_start | TIMESTAMP |
| actual_finish | TIMESTAMP |
| status | VARCHAR(20) |

---

# operation

Represents one production step.

Examples

Sawing

Kiln Drying

Thermowood

Profiling

Finger Joint

Press

Packaging

| Field | Type |
|--------|------|
| id | UUID |
| work_order_id | UUID FK |
| operation_type_id | UUID FK |
| sequence | INTEGER |
| machine_id | UUID FK |
| recipe_id | UUID FK |
| planned_duration | INTEGER |
| actual_duration | INTEGER |
| status | VARCHAR(20) |

---

# operation_dependency

Defines operation sequence.

Example

Sawing

↓

Kiln

↓

Thermowood

↓

Profiling

| Field | Type |
|--------|------|
| id | UUID |
| predecessor_operation_id | UUID FK |
| successor_operation_id | UUID FK |

---

# production_batch

Represents one execution batch.

Examples

Kiln Charge

Thermowood Batch

Press Batch

Packaging Batch

| Field | Type |
|--------|------|
| id | UUID |
| work_order_id | UUID FK |
| batch_number | VARCHAR(40) |
| batch_type | VARCHAR(50) |
| start_time | TIMESTAMP |
| end_time | TIMESTAMP |
| status | VARCHAR(20) |

---

# production_schedule

Scheduling information.

| Field | Type |
|--------|------|
| id | UUID |
| work_order_id | UUID FK |
| machine_id | UUID FK |
| shift_id | UUID FK |
| scheduled_start | TIMESTAMP |
| scheduled_finish | TIMESTAMP |
| priority | INTEGER |

---

# routing_execution

Stores actual routing followed.

Routing may differ from planned routing.

| Field | Type |
|--------|------|
| id | UUID |
| work_order_id | UUID FK |
| routing_id | UUID FK |
| version | INTEGER |
| execution_status | VARCHAR(20) |

---

# recipe_execution

Stores actual recipe used.

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| recipe_id | UUID FK |
| recipe_version | INTEGER |
| approved_by | UUID FK |
| started_at | TIMESTAMP |

---

# machine_execution

Stores machine runtime.

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| machine_id | UUID FK |
| runtime_minutes | INTEGER |
| idle_minutes | INTEGER |
| downtime_minutes | INTEGER |
| oee | NUMERIC(5,2) |

---

# operator_execution

Operators participating in execution.

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| employee_id | UUID FK |
| role | VARCHAR(50) |
| labor_minutes | INTEGER |

---

# transformation

Actual production process.

Detailed model defined in:

Transformation_Model.md

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(40) |
| work_order_id | UUID FK |
| operation_id | UUID FK |
| machine_id | UUID FK |
| recipe_execution_id | UUID FK |
| production_batch_id | UUID FK |
| status | VARCHAR(20) |
| started_at | TIMESTAMP |
| completed_at | TIMESTAMP |

---

# transformation_input

Input Materials.

| Field | Type |
|--------|------|
| id | UUID |
| transformation_id | UUID FK |
| material_id | UUID FK |
| quantity | NUMERIC(18,3) |

---

# transformation_output

Output Materials.

| Field | Type |
|--------|------|
| id | UUID |
| transformation_id | UUID FK |
| material_id | UUID FK |
| quantity | NUMERIC(18,3) |

---

# production_delay

Production interruptions.

Examples

Machine Failure

Material Shortage

Operator Missing

Quality Hold

Power Failure

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| delay_reason | VARCHAR(100) |
| duration_minutes | INTEGER |

---

# production_loss

Manufacturing losses.

Examples

Waste

Recovery

Downtime

Setup

Micro Stops

| Field | Type |
|--------|------|
| id | UUID |
| transformation_id | UUID FK |
| loss_type | VARCHAR(50) |
| quantity | NUMERIC(18,3) |
| cost | NUMERIC(18,2) |

---

# production_progress

Real-time production progress.

| Field | Type |
|--------|------|
| id | UUID |
| work_order_id | UUID FK |
| planned_quantity | NUMERIC |
| completed_quantity | NUMERIC |
| rejected_quantity | NUMERIC |
| recovery_quantity | NUMERIC |
| completion_percentage | NUMERIC(5,2) |
| updated_at | TIMESTAMP |

---

# Relationships

Production Order

1 → N Production Order Lines

Production Order

1 → N Work Orders

Work Order

1 → N Operations

Operation

1 → N Transformations

Transformation

1 → N Inputs

Transformation

1 → N Outputs

Operation

1 → N Operators

Operation

1 → N Machine Executions

Operation

1 → N Recipe Executions

---
# production_batch_material

Materials assigned to a production batch.

A production batch may contain materials from different Production Orders when business rules allow.

| Field | Type |
|--------|------|
| id | UUID |
| production_batch_id | UUID FK |
| material_id | UUID FK |
| production_order_line_id | UUID FK |
| sequence | INTEGER |
| loaded_at | TIMESTAMP |
| unloaded_at | TIMESTAMP |

Business Rules

- One material may belong to only one active batch.
- A batch may contain materials from multiple Production Orders.
- Batch composition must remain traceable.
- Material genealogy shall not be broken.
 # production_batch_rule

Defines batch compatibility rules.

| Field | Type |
|--------|------|
| id | UUID |
| batch_type | VARCHAR |
| species_id | UUID FK |
| thickness_min | NUMERIC |
| thickness_max | NUMERIC |
| moisture_min | NUMERIC |
| moisture_max | NUMERIC |
| recipe_id | UUID FK |

The Planning Engine uses these rules to build optimized production batches.

# operation_resource

Resources required during an operation.

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| resource_type | VARCHAR |
| resource_id | UUID |
| planned_quantity | NUMERIC |
| actual_quantity | NUMERIC |

# operation_checklist

Operational verification before execution.

| Field | Type |
|--------|------|
| id | UUID |
| operation_id | UUID FK |
| checklist_item | VARCHAR |
| result | BOOLEAN |
| checked_by | UUID FK |
| checked_at | TIMESTAMP |

# General Rules

- Production Orders represent demand.
- Work Orders represent executable work.
- Operations represent manufacturing steps.
- Transformations represent completed production.
- Every Transformation creates Events.
- Every Material references its originating Transformation.
- Routing may differ from the planned route.
- Recipe versions are preserved.
- Historical execution data is immutable.
- Soft Delete is preferred.
