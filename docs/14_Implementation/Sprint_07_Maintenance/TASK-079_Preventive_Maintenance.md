# ==============================================================================
# TASK-079 — IMPLEMENTATION
# PREVENTIVE MAINTENANCE
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Preventive Maintenance aggregate responsible for planning,
scheduling and automatically generating preventive maintenance work for
enterprise assets.

Preventive Maintenance minimizes unplanned downtime through scheduled
maintenance activities based on time, usage or condition.

Preventive Maintenance generates Maintenance Work Orders.

It never executes maintenance directly.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
PreventiveMaintenancePlan
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- TASK-076_Asset.md
- TASK-078_Maintenance_Order.md
- Calendar Architecture
- Notification Architecture

---

# DEPENDENCIES

Requires completed modules:

- Asset
- Maintenance Work Order
- Employee
- Calendar
- Notification
- Organization

---

# AGGREGATE

```
PreventiveMaintenancePlan
```

Children

```
MaintenanceTask

MaintenanceChecklist

MaintenanceSchedule

MaintenanceTrigger

AuditEntry
```

---

# VALUE OBJECTS

```
PlanCode

PlanStatus

MaintenanceFrequency

TriggerType

NextExecutionDate
```

---

# ENUMS

## PlanStatus

```text
Draft

Approved

Active

Suspended

Completed

Archived

Cancelled
```

---

## TriggerType

```text
TimeBased

UsageBased

ConditionBased

CalendarBased

Hybrid
```

---

## MaintenanceFrequency

```text
Daily

Weekly

Monthly

Quarterly

SemiAnnual

Annual

OperatingHours

ProductionCycles

Custom
```

---

# ENTITY FIELDS

```text
Id

PlanCode

AssetId

PlanName

Description

Status

TriggerType

Frequency

StartDate

EndDate

NextExecutionDate

LastExecutionDate

EstimatedDuration

AssignedDepartmentId

SupervisorId

AutoGenerateWorkOrder

CreatedAt

UpdatedAt
```

---

# MAINTENANCE TASK

```text
Id

PreventiveMaintenancePlanId

Sequence

TaskName

Description

EstimatedMinutes

Mandatory
```

---

# MAINTENANCE CHECKLIST

```text
Id

PreventiveMaintenancePlanId

ChecklistItem

Mandatory

Sequence
```

---

# MAINTENANCE TRIGGER

```text
Id

PreventiveMaintenancePlanId

TriggerType

ThresholdValue

CurrentValue

LastTriggeredAt
```

---

# DOMAIN INVARIANTS

Every Preventive Maintenance Plan references one Asset.

Only one Active Plan of the same type may exist for an Asset.

Approved Plans are immutable except scheduling fields.

Generated Work Orders maintain reference to the originating Plan.

Inactive Assets cannot have Active Preventive Plans.

---

# DOMAIN METHODS

```text
Create()

Approve()

Activate()

Suspend()

Resume()

GenerateWorkOrder()

UpdateSchedule()

CompleteExecution()

Archive()

Cancel()
```

---

# DOMAIN EVENTS

```text
PreventivePlanCreated

PreventivePlanApproved

PreventivePlanActivated

PreventivePlanSuspended

PreventiveWorkOrderGenerated

PreventiveMaintenanceCompleted

PreventivePlanArchived

PreventivePlanCancelled
```

---

# VALIDATIONS

Create

- Asset exists
- Frequency defined

Approve

- At least one Maintenance Task exists
- Schedule configured

Activate

- Asset Active
- No conflicting Active Plan

Generate Work Order

- Next Execution Date reached
- Plan Active

Archive

- No pending Work Orders

---

# REPOSITORY

```text
IPreventiveMaintenanceRepository
```

Methods

```csharp
Task<PreventiveMaintenancePlan?> GetByIdAsync(Guid id);

Task<PreventiveMaintenancePlan?> GetByCodeAsync(string planCode);

Task<IEnumerable<PreventiveMaintenancePlan>> GetActiveAsync();

Task<IEnumerable<PreventiveMaintenancePlan>> GetDuePlansAsync(DateTime date);

Task AddAsync(PreventiveMaintenancePlan entity);

Task UpdateAsync(PreventiveMaintenancePlan entity);
```

---

# COMMANDS

```text
CreatePreventivePlanCommand

ApprovePreventivePlanCommand

ActivatePreventivePlanCommand

SuspendPreventivePlanCommand

ResumePreventivePlanCommand

GeneratePreventiveWorkOrderCommand

CompletePreventiveMaintenanceCommand

ArchivePreventivePlanCommand

CancelPreventivePlanCommand
```

---

# QUERIES

```text
GetPreventivePlanByIdQuery

GetPreventivePlansQuery

GetActivePreventivePlansQuery

GetDuePreventivePlansQuery

GetAssetPreventivePlansQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/preventive-plans

GET    /api/v1/maintenance/preventive-plans/{id}

POST   /api/v1/maintenance/preventive-plans

PUT    /api/v1/maintenance/preventive-plans/{id}

POST   /api/v1/maintenance/preventive-plans/{id}/approve

POST   /api/v1/maintenance/preventive-plans/{id}/activate

POST   /api/v1/maintenance/preventive-plans/{id}/suspend

POST   /api/v1/maintenance/preventive-plans/{id}/resume

POST   /api/v1/maintenance/preventive-plans/{id}/generate-work-order

POST   /api/v1/maintenance/preventive-plans/{id}/complete

POST   /api/v1/maintenance/preventive-plans/{id}/archive

POST   /api/v1/maintenance/preventive-plans/{id}/cancel
```

---

# AUTHORIZATION

```text
maintenance.preventive.read

maintenance.preventive.create

maintenance.preventive.update

maintenance.preventive.approve

maintenance.preventive.activate

maintenance.preventive.execute

maintenance.preventive.archive

maintenance.preventive.cancel
```

---

# DATABASE TABLES

## PreventiveMaintenancePlans

```text
Id

PlanCode

AssetId

PlanName

Description

Status

TriggerType

Frequency

StartDate

EndDate

NextExecutionDate

LastExecutionDate

EstimatedDuration

AssignedDepartmentId

SupervisorId

AutoGenerateWorkOrder

CreatedAt

UpdatedAt
```

---

## PreventiveMaintenanceTasks

```text
Id

PreventiveMaintenancePlanId

Sequence

TaskName

Description

EstimatedMinutes

Mandatory
```

---

## PreventiveMaintenanceChecklists

```text
Id

PreventiveMaintenancePlanId

ChecklistItem

Mandatory

Sequence
```

---

# INDEXES

```text
IX_PlanCode (Unique)

IX_AssetId

IX_Status

IX_TriggerType

IX_NextExecutionDate

IX_AssignedDepartmentId
```

---

# AUDIT

Audit every

- Plan creation
- Approval
- Activation
- Suspension
- Schedule update
- Work Order generation
- Completion
- Archive
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

- Create Preventive Plan
- Approve Plan
- Activate Plan
- Generate Work Order
- Update Schedule
- Suspend Plan
- Resume Plan
- Complete Maintenance
- Prevent duplicate Active Plans
- Archive Plan

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Asset integration
- Maintenance Work Order integration
- Notification integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Preventive Maintenance Plans are linked to Assets.
- Work Orders are generated automatically according to schedule.
- Only one active plan of the same type exists per Asset.
- Trigger-based scheduling is supported.
- Generated Work Orders preserve Plan references.
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
- Maintenance Work Order integration completed
- Notification integration completed
- Scheduler integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
