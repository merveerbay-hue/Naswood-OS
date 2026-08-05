# ==============================================================================
# TASK-071 — IMPLEMENTATION
# CAPA (CORRECTIVE & PREVENTIVE ACTION)
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the CAPA (Corrective and Preventive Action) aggregate responsible for
eliminating the root causes of quality problems and preventing recurrence.

CAPA manages improvement activities initiated from Non-Conformance Reports
(NCR), Customer Complaints, Internal Audits, Supplier Issues and Risk
Assessments.

CAPA is the canonical continuous improvement process of the Quality Management
System.

---

# DOMAIN

Quality Management

Aggregate Root

```
CAPA
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_API.md
- TASK-070_Non_Conformance.md
- Risk Management
- Audit Management

---

# DEPENDENCIES

Requires completed modules:

- Non-Conformance
- Employee
- Department
- Document Management
- Notification
- Workflow

---

# AGGREGATE

```
CAPA
```

Children

```
CorrectiveAction

PreventiveAction

RootCauseAnalysis

Verification

EffectivenessReview

Attachment

AuditEntry
```

---

# VALUE OBJECTS

```
CapaNumber

CapaStatus

CapaType

Priority

DueDate
```

---

# ENUMS

## CapaStatus

```text
Draft

Open

Investigation

Planning

Implementation

Verification

EffectivenessReview

Completed

Closed

Cancelled
```

---

## CapaType

```text
Corrective

Preventive

Combined
```

---

## Priority

```text
Low

Medium

High

Critical
```

---

# ENTITY FIELDS

```text
Id

CapaNumber

NcrId

SourceType

SourceReferenceId

Title

Description

Type

Priority

Status

AssignedTo

DepartmentId

TargetCompletionDate

ActualCompletionDate

VerifiedBy

ClosedBy

CreatedAt

UpdatedAt
```

---

# ROOT CAUSE ANALYSIS

```text
Id

CapaId

Method

Category

Description

PerformedBy

PerformedAt
```

Supported Methods

- 5 Why
- Fishbone
- Fault Tree
- Pareto
- Custom

---

# CORRECTIVE ACTION

```text
Id

CapaId

Description

ResponsiblePersonId

DueDate

CompletedAt

Status
```

---

# PREVENTIVE ACTION

```text
Id

CapaId

Description

ResponsiblePersonId

DueDate

CompletedAt

Status
```

---

# EFFECTIVENESS REVIEW

```text
Id

CapaId

ReviewerId

ReviewDate

Result

Comments
```

Result

```text
Effective

PartiallyEffective

NotEffective
```

---

# DOMAIN INVARIANTS

Every CAPA has one Source.

Every CAPA has one Responsible Person.

Completed CAPA requires completed Root Cause Analysis.

Closed CAPA requires successful Effectiveness Review.

Critical CAPA requires management approval.

Closed CAPA is immutable.

---

# DOMAIN METHODS

```text
Create()

Assign()

StartInvestigation()

CompleteRootCause()

AddCorrectiveAction()

AddPreventiveAction()

StartImplementation()

CompleteImplementation()

Verify()

ReviewEffectiveness()

Close()

Cancel()
```

---

# DOMAIN EVENTS

```text
CapaCreated

RootCauseCompleted

CorrectiveActionCreated

PreventiveActionCreated

ImplementationStarted

ImplementationCompleted

CapaVerified

EffectivenessReviewed

CapaClosed

CapaCancelled
```

---

# VALIDATIONS

Create

- Source exists
- Responsible Person assigned

Implementation

- Root Cause completed
- At least one Action exists

Verification

- All Actions completed

Close

- Effectiveness Review = Effective

Critical CAPA

- Management Approval required

---

# REPOSITORY

```text
ICapaRepository
```

Methods

```csharp
Task<CAPA?> GetByIdAsync(Guid id);

Task<CAPA?> GetByNumberAsync(string capaNumber);

Task<IEnumerable<CAPA>> GetOpenAsync();

Task<IEnumerable<CAPA>> GetByNcrAsync(Guid ncrId);

Task AddAsync(CAPA entity);

Task UpdateAsync(CAPA entity);
```

---

# COMMANDS

```text
CreateCapaCommand

AssignCapaCommand

CompleteRootCauseCommand

AddCorrectiveActionCommand

AddPreventiveActionCommand

StartImplementationCommand

CompleteImplementationCommand

VerifyCapaCommand

ReviewEffectivenessCommand

CloseCapaCommand

CancelCapaCommand
```

---

# QUERIES

```text
GetCapaByIdQuery

GetCapasQuery

GetOpenCapasQuery

GetNcrCapasQuery

GetOverdueCapasQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/quality/capas

GET    /api/v1/quality/capas/{id}

POST   /api/v1/quality/capas

POST   /api/v1/quality/capas/{id}/assign

POST   /api/v1/quality/capas/{id}/root-cause

POST   /api/v1/quality/capas/{id}/corrective-action

POST   /api/v1/quality/capas/{id}/preventive-action

POST   /api/v1/quality/capas/{id}/implement

POST   /api/v1/quality/capas/{id}/verify

POST   /api/v1/quality/capas/{id}/effectiveness

POST   /api/v1/quality/capas/{id}/close

POST   /api/v1/quality/capas/{id}/cancel
```

---

# AUTHORIZATION

```text
quality.capa.read

quality.capa.create

quality.capa.assign

quality.capa.implement

quality.capa.verify

quality.capa.close

quality.capa.cancel
```

---

# DATABASE TABLES

## CAPAs

```text
Id

CapaNumber

NcrId

SourceType

SourceReferenceId

Title

Description

Type

Priority

Status

AssignedTo

DepartmentId

TargetCompletionDate

ActualCompletionDate

VerifiedBy

ClosedBy

CreatedAt

UpdatedAt
```

---

## CorrectiveActions

```text
Id

CapaId

Description

ResponsiblePersonId

DueDate

CompletedAt

Status
```

---

## PreventiveActions

```text
Id

CapaId

Description

ResponsiblePersonId

DueDate

CompletedAt

Status
```

---

## EffectivenessReviews

```text
Id

CapaId

ReviewerId

ReviewDate

Result

Comments
```

---

# INDEXES

```text
IX_CapaNumber (Unique)

IX_NcrId

IX_Status

IX_Priority

IX_AssignedTo

IX_DepartmentId

IX_TargetCompletionDate
```

---

# AUDIT

Audit every

- CAPA creation
- Assignment
- Root Cause completion
- Action creation
- Implementation
- Verification
- Effectiveness Review
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

- Create CAPA
- Complete Root Cause
- Add Corrective Action
- Add Preventive Action
- Verify implementation
- Complete Effectiveness Review
- Close CAPA
- Prevent closure without effectiveness review
- Prevent modification after closure

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- NCR integration
- Notification integration
- Workflow integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every CAPA is linked to a valid source.
- Root Cause Analysis is mandatory.
- Corrective and Preventive Actions are fully traceable.
- Effectiveness Review is required before closure.
- Closed CAPAs become immutable.
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
- NCR integration completed
- Workflow integration completed
- Notification integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
