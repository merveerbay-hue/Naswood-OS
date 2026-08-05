# ==============================================================================
# TASK-070 — IMPLEMENTATION
# NON-CONFORMANCE (NCR)
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Non-Conformance (NCR) aggregate responsible for managing all
products, materials or processes that fail to meet defined quality
requirements.

The NCR module controls the complete lifecycle of non-conforming products,
from detection through disposition, corrective actions and final closure.

NCR is the canonical quality record for every quality failure.

It does not modify production or inventory directly.

---

# DOMAIN

Quality Management

Aggregate Root

```
NonConformance
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_API.md
- TASK-067_Inspection_Execution.md
- TASK-068_In_Process_Inspection.md
- TASK-069_Final_Inspection.md
- Production Architecture
- Inventory Architecture

---

# DEPENDENCIES

Requires completed modules:

- Inspection Execution
- In-Process Inspection
- Final Inspection
- Product Revision
- Lot
- Serial Number
- Production Order
- Work Order
- Employee

---

# AGGREGATE

```
NonConformance
```

Children

```
Defect

Disposition

CorrectiveAction

ContainmentAction

RootCauseAnalysis

Attachment

AuditEntry
```

---

# VALUE OBJECTS

```
NcrNumber

NcrStatus

Severity

Priority

DispositionType

RootCauseCategory
```

---

# ENUMS

## NcrStatus

```text
Draft

Open

UnderInvestigation

Containment

DispositionPending

Approved

Implemented

Verified

Closed

Cancelled
```

---

## Severity

```text
Minor

Major

Critical

SafetyCritical
```

---

## Priority

```text
Low

Medium

High

Urgent
```

---

## DispositionType

```text
UseAsIs

Repair

Rework

Scrap

ReturnToSupplier

Replace

CustomerApprovalRequired
```

---

# ENTITY FIELDS

```text
Id

NcrNumber

ProductRevisionId

ProductionOrderId

WorkOrderId

InspectionExecutionId

LotId

SerialId

Status

Severity

Priority

DispositionType

Description

DetectedBy

AssignedTo

OpenedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

# DEFECT

```text
Id

NonConformanceId

DefectCode

DefectCategory

Description

Quantity

UnitOfMeasureId

Location

ImageReference
```

---

# ROOT CAUSE ANALYSIS

```text
Id

NonConformanceId

Category

Description

Method

PerformedBy

PerformedAt
```

Supported Methods

- 5 Why
- Fishbone
- Fault Tree
- Custom

---

# CORRECTIVE ACTION

```text
Id

NonConformanceId

Description

ResponsiblePerson

DueDate

CompletedAt

VerificationStatus
```

---

# DOMAIN INVARIANTS

Every NCR references at least one detected defect.

Every NCR has one Severity.

Every NCR has one Disposition.

Closed NCRs are immutable.

Critical NCRs require Root Cause Analysis.

Corrective Actions must be completed before Closure.

---

# DOMAIN METHODS

```text
Create()

Assign()

AddDefect()

SetSeverity()

StartInvestigation()

AddRootCause()

AddContainmentAction()

AssignDisposition()

ApproveDisposition()

CreateCorrectiveAction()

CompleteCorrectiveAction()

Verify()

Close()

Cancel()
```

---

# DOMAIN EVENTS

```text
NonConformanceCreated

DefectRecorded

ContainmentStarted

DispositionAssigned

DispositionApproved

CorrectiveActionCreated

CorrectiveActionCompleted

RootCauseCompleted

NonConformanceVerified

NonConformanceClosed

NonConformanceCancelled
```

---

# VALIDATIONS

Create

- Product exists
- Inspection exists
- At least one Defect exists

Approve Disposition

- Investigation completed

Close

- Corrective Actions completed
- Verification completed

Critical NCR

- Root Cause mandatory

---

# REPOSITORY

```text
INonConformanceRepository
```

Methods

```csharp
Task<NonConformance?> GetByIdAsync(Guid id);

Task<NonConformance?> GetByNumberAsync(string ncrNumber);

Task<IEnumerable<NonConformance>> GetOpenAsync();

Task<IEnumerable<NonConformance>> GetByProductionOrderAsync(Guid productionOrderId);

Task AddAsync(NonConformance entity);

Task UpdateAsync(NonConformance entity);
```

---

# COMMANDS

```text
CreateNonConformanceCommand

AssignNonConformanceCommand

AddDefectCommand

AssignDispositionCommand

ApproveDispositionCommand

AddRootCauseCommand

CreateCorrectiveActionCommand

CompleteCorrectiveActionCommand

VerifyNonConformanceCommand

CloseNonConformanceCommand

CancelNonConformanceCommand
```

---

# QUERIES

```text
GetNonConformanceByIdQuery

GetNonConformancesQuery

GetOpenNonConformancesQuery

GetProductionOrderNcrQuery

GetCriticalNcrQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/quality/non-conformances

GET    /api/v1/quality/non-conformances/{id}

POST   /api/v1/quality/non-conformances

POST   /api/v1/quality/non-conformances/{id}/assign

POST   /api/v1/quality/non-conformances/{id}/defect

POST   /api/v1/quality/non-conformances/{id}/disposition

POST   /api/v1/quality/non-conformances/{id}/approve-disposition

POST   /api/v1/quality/non-conformances/{id}/root-cause

POST   /api/v1/quality/non-conformances/{id}/corrective-action

POST   /api/v1/quality/non-conformances/{id}/verify

POST   /api/v1/quality/non-conformances/{id}/close

POST   /api/v1/quality/non-conformances/{id}/cancel
```

---

# AUTHORIZATION

```text
quality.ncr.read

quality.ncr.create

quality.ncr.assign

quality.ncr.disposition

quality.ncr.correctiveaction

quality.ncr.verify

quality.ncr.close

quality.ncr.cancel
```

---

# DATABASE TABLES

## NonConformances

```text
Id

NcrNumber

ProductRevisionId

ProductionOrderId

WorkOrderId

InspectionExecutionId

LotId

SerialId

Status

Severity

Priority

DispositionType

Description

DetectedBy

AssignedTo

OpenedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

## NonConformanceDefects

```text
Id

NonConformanceId

DefectCode

DefectCategory

Description

Quantity

UnitOfMeasureId

Location

ImageReference
```

---

## CorrectiveActions

```text
Id

NonConformanceId

Description

ResponsiblePerson

DueDate

CompletedAt

VerificationStatus
```

---

# INDEXES

```text
IX_NcrNumber (Unique)

IX_Status

IX_Severity

IX_Priority

IX_ProductRevisionId

IX_ProductionOrderId

IX_WorkOrderId

IX_LotId
```

---

# AUDIT

Audit every

- NCR creation
- Defect creation
- Severity changes
- Disposition approval
- Root Cause completion
- Corrective Action updates
- Verification
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

- Create NCR
- Add Defect
- Assign Severity
- Complete Root Cause
- Approve Disposition
- Complete Corrective Action
- Verify NCR
- Close NCR
- Prevent closing without corrective actions
- Prevent editing after closure

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Inspection integration
- Rework integration
- Scrap integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every quality failure creates an NCR.
- NCR lifecycle follows defined workflow.
- Root Cause Analysis is mandatory for Critical NCRs.
- Corrective Actions are tracked to completion.
- Closed NCRs become immutable.
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
- Inspection integration completed
- Rework integration completed
- Scrap integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
