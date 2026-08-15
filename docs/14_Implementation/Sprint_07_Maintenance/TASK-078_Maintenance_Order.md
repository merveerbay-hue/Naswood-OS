# ==============================================================================
# TASK-078 — IMPLEMENTATION
# MAINTENANCE WORK ORDER
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Maintenance Work Order aggregate responsible for planning,
executing, controlling and closing maintenance activities for enterprise assets.

Maintenance Work Orders represent the executable maintenance document.

Work Orders may originate from Work Requests, Preventive Maintenance Plans,
Predictive Maintenance, Breakdown events or Manual creation.

Work Orders execute maintenance.

They never modify Asset master data directly.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
MaintenanceWorkOrder
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- TASK-076_Asset.md
- TASK-077_Work_Request.md
- Inventory Architecture
- Employee Architecture

---

# DEPENDENCIES

Requires completed modules:

- Asset
- Work Request
- Employee
- Inventory
- Warehouse
- Spare Parts
- Document Management

---

# AGGREGATE

```
MaintenanceWorkOrder
```

Children

```
MaintenanceOperation

LaborEntry

MaterialConsumption

ChecklistItem

Attachment

Comment

AuditEntry
```

---

# VALUE OBJECTS

```
WorkOrderNumber

WorkOrderType

Priority

Status

PlannedDuration

ActualDuration
```

---

# ENUMS

## WorkOrderStatus

```text
Draft

Planned

Released

Assigned

InProgress

WaitingForParts

WaitingForApproval

Completed

Closed

Cancelled
```

---

## WorkOrderType

```text
Corrective

Preventive

Predictive

Emergency

Inspection

Calibration

Shutdown

Improvement
```

---

## Priority

```text
Low

Medium

High

Critical

Emergency
```

---

# ENTITY FIELDS

```text
Id

WorkOrderNumber

AssetId

WorkRequestId

Type

Priority

Status

Description

PlannedStart

PlannedFinish

ActualStart

ActualFinish

EstimatedHours

ActualHours

AssignedTo

SupervisorId

DepartmentId

FailureCode

CompletionNotes

CreatedAt

UpdatedAt
```

---

# MAINTENANCE OPERATION

```text
Id

MaintenanceWorkOrderId

Sequence

OperationName

Description

Status

EstimatedHours

ActualHours

CompletedBy

CompletedAt
```

---

# LABOR ENTRY

```text
Id

MaintenanceWorkOrderId

EmployeeId

StartTime

EndTime

WorkedHours

OvertimeHours
```

---

# MATERIAL CONSUMPTION

```text
Id

MaintenanceWorkOrderId

InventoryMaterialId

LotId

Quantity

UnitOfMeasureId
```

---

# CHECKLIST ITEM

```text
Id

MaintenanceWorkOrderId

Description

IsMandatory

Completed

CompletedBy

CompletedAt
```

---

# DOMAIN INVARIANTS

Every Maintenance Work Order references one Asset.

A Work Order may reference one Work Request.

Released Work Orders become executable.

Completed Work Orders cannot be modified.

Closed Work Orders are immutable.

Inventory is updated only through Inventory Transactions.

---

# DOMAIN METHODS

```text
Create()

Plan()

Release()

Assign()

Start()

Pause()

Resume()

ConsumeMaterial()

RecordLabor()

CompleteOperation()

Complete()

Close()

Cancel()
```

---

# DOMAIN EVENTS

```text
MaintenanceWorkOrderCreated

MaintenanceWorkOrderReleased

MaintenanceStarted

MaintenancePaused

MaintenanceResumed

MaterialConsumed

LaborRecorded

MaintenanceCompleted

MaintenanceClosed

MaintenanceCancelled
```

---

# VALIDATIONS

Create

- Asset exists
- Work Order Type valid

Release

- Assigned technician exists
- Planned dates defined

Start

- Status = Released or Assigned

Consume Material

- Inventory available
- Material exists

Complete

- Mandatory checklist completed
- All operations completed

Close

- Labor posted
- Material consumption posted

---

# REPOSITORY

```text
IMaintenanceWorkOrderRepository
```

Methods

```csharp
Task<MaintenanceWorkOrder?> GetByIdAsync(Guid id);

Task<MaintenanceWorkOrder?> GetByNumberAsync(string workOrderNumber);

Task<IEnumerable<MaintenanceWorkOrder>> GetOpenAsync();

Task<IEnumerable<MaintenanceWorkOrder>> GetByAssetAsync(Guid assetId);

Task AddAsync(MaintenanceWorkOrder entity);

Task UpdateAsync(MaintenanceWorkOrder entity);
```

---

# COMMANDS

```text
CreateMaintenanceWorkOrderCommand

PlanMaintenanceCommand

ReleaseMaintenanceCommand

AssignMaintenanceCommand

StartMaintenanceCommand

PauseMaintenanceCommand

ResumeMaintenanceCommand

ConsumeMaterialCommand

RecordLaborCommand

CompleteMaintenanceCommand

CloseMaintenanceCommand

CancelMaintenanceCommand
```

---

# QUERIES

```text
GetMaintenanceWorkOrderByIdQuery

GetMaintenanceWorkOrdersQuery

GetOpenMaintenanceWorkOrdersQuery

GetAssetMaintenanceHistoryQuery

GetTechnicianWorkOrdersQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/work-orders

GET    /api/v1/maintenance/work-orders/{id}

POST   /api/v1/maintenance/work-orders

PUT    /api/v1/maintenance/work-orders/{id}

POST   /api/v1/maintenance/work-orders/{id}/plan

POST   /api/v1/maintenance/work-orders/{id}/release

POST   /api/v1/maintenance/work-orders/{id}/assign

POST   /api/v1/maintenance/work-orders/{id}/start

POST   /api/v1/maintenance/work-orders/{id}/pause

POST   /api/v1/maintenance/work-orders/{id}/resume

POST   /api/v1/maintenance/work-orders/{id}/consume-material

POST   /api/v1/maintenance/work-orders/{id}/record-labor

POST   /api/v1/maintenance/work-orders/{id}/complete

POST   /api/v1/maintenance/work-orders/{id}/close

POST   /api/v1/maintenance/work-orders/{id}/cancel
```

---

# AUTHORIZATION

```text
maintenance.workorder.read

maintenance.workorder.create

maintenance.workorder.plan

maintenance.workorder.release

maintenance.workorder.assign

maintenance.workorder.execute

maintenance.workorder.complete

maintenance.workorder.close

maintenance.workorder.cancel
```

---

# DATABASE TABLES

## MaintenanceWorkOrders

```text
Id

WorkOrderNumber

AssetId

WorkRequestId

Type

Priority

Status

Description

PlannedStart

PlannedFinish

ActualStart

ActualFinish

EstimatedHours

ActualHours

AssignedTo

SupervisorId

DepartmentId

FailureCode

CompletionNotes

CreatedAt

UpdatedAt
```

---

## MaintenanceOperations

```text
Id

MaintenanceWorkOrderId

Sequence

OperationName

Description

Status

EstimatedHours

ActualHours

CompletedBy

CompletedAt
```

---

## MaintenanceLaborEntries

```text
Id

MaintenanceWorkOrderId

EmployeeId

StartTime

EndTime

WorkedHours

OvertimeHours
```

---

## MaintenanceMaterialConsumptions

```text
Id

MaintenanceWorkOrderId

InventoryMaterialId

LotId

Quantity

UnitOfMeasureId
```

---

# INDEXES

```text
IX_WorkOrderNumber (Unique)

IX_AssetId

IX_WorkRequestId

IX_Status

IX_Type

IX_AssignedTo

IX_PlannedStart
```

---

# AUDIT

Audit every

- Creation
- Planning
- Release
- Assignment
- Start
- Pause
- Resume
- Material Consumption
- Labor Entry
- Completion
- Closure
- Cancellation

Capture

```text
UserId

Timestamp

Action

OldValue

NewValue

CorrelationId
```

---

# TESTS

## Unit Tests

- Create Work Order
- Plan Work Order
- Release Work Order
- Assign Technician
- Start Maintenance
- Record Labor
- Consume Material
- Complete Work Order
- Close Work Order
- Prevent modification after Close

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Asset integration
- Inventory integration
- Employee integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Maintenance Work Order references a valid Asset.
- Work Orders support the complete maintenance lifecycle.
- Labor and spare part usage are fully traceable.
- Inventory updates occur only through Inventory Transactions.
- Closed Work Orders become immutable.
- CQRS architecture is respected.
- Domain Events are published.
- API integration tests pass.
- Audit logging is complete.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- Domain implemented
- Application layer implemented
- Infrastructure implemented
- REST API completed
- CQRS completed
- Validation rules completed
- Asset integration completed
- Inventory integration completed
- Employee integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
