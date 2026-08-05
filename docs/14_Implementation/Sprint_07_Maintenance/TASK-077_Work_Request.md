# ==============================================================================
# TASK-077 — IMPLEMENTATION
# MAINTENANCE WORK REQUEST
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Maintenance Work Request aggregate responsible for recording,
tracking and managing maintenance requests originating from production,
quality, warehouse, utilities or other departments.

A Work Request represents a request for maintenance.

It is **not** a Maintenance Work Order.

Approved Work Requests may generate one or more Maintenance Work Orders.

---

# DOMAIN

Maintenance Management

Aggregate Root

```
WorkRequest
```

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_API.md
- TASK-076_Asset.md
- Organization Architecture
- Notification Architecture

---

# DEPENDENCIES

Requires completed modules:

- Asset
- Employee
- Organization
- Work Center
- Document Management
- Notification

---

# AGGREGATE

```
WorkRequest
```

Children

```
WorkRequestAttachment

WorkRequestComment

FailureInformation

ApprovalHistory

AuditEntry
```

---

# VALUE OBJECTS

```
WorkRequestNumber

RequestPriority

RequestStatus

FailureSeverity

RequestedDate
```

---

# ENUMS

## WorkRequestStatus

```text
Draft

Submitted

Assigned

UnderReview

Approved

Rejected

ConvertedToWorkOrder

Cancelled

Closed
```

---

## RequestPriority

```text
Low

Medium

High

Critical

Emergency
```

---

## FailureSeverity

```text
Minor

Major

Critical

Safety

Environmental
```

---

## RequestSource

```text
Production

Quality

Warehouse

Utilities

Maintenance

Administration

Customer

Supplier

IoT

Other
```

---

# ENTITY FIELDS

```text
Id

WorkRequestNumber

AssetId

RequestSource

Priority

Severity

Status

Title

Description

RequestedBy

AssignedTo

DepartmentId

ProductionOrderId

WorkOrderId

FailureCode

FailureDate

SubmittedAt

ApprovedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

# FAILURE INFORMATION

```text
Id

WorkRequestId

FailureCode

FailureCategory

FailureMode

Symptoms

RootCause

MachineState

OccurredAt
```

---

# DOMAIN INVARIANTS

Every Work Request references exactly one Asset.

Every Work Request has one Requestor.

Approved Work Requests become read-only.

Rejected Work Requests cannot be converted.

Converted Requests maintain reference to generated Work Orders.

Closed Work Requests are immutable.

---

# DOMAIN METHODS

```text
Create()

Submit()

Assign()

Approve()

Reject()

ConvertToWorkOrder()

AddComment()

AddAttachment()

Cancel()

Close()
```

---

# DOMAIN EVENTS

```text
WorkRequestCreated

WorkRequestSubmitted

WorkRequestAssigned

WorkRequestApproved

WorkRequestRejected

WorkOrderRequested

WorkRequestCancelled

WorkRequestClosed
```

---

# VALIDATIONS

Create

- Asset exists
- Requestor exists

Submit

- Title provided
- Description provided
- Priority selected

Approve

- Reviewer assigned

Convert To Work Order

- Status = Approved

Close

- All generated Work Orders completed or cancelled

---

# REPOSITORY

```text
IWorkRequestRepository
```

Methods

```csharp
Task<WorkRequest?> GetByIdAsync(Guid id);

Task<WorkRequest?> GetByNumberAsync(string requestNumber);

Task<IEnumerable<WorkRequest>> GetOpenAsync();

Task<IEnumerable<WorkRequest>> GetByAssetAsync(Guid assetId);

Task AddAsync(WorkRequest entity);

Task UpdateAsync(WorkRequest entity);
```

---

# COMMANDS

```text
CreateWorkRequestCommand

SubmitWorkRequestCommand

AssignWorkRequestCommand

ApproveWorkRequestCommand

RejectWorkRequestCommand

ConvertWorkRequestCommand

CancelWorkRequestCommand

CloseWorkRequestCommand
```

---

# QUERIES

```text
GetWorkRequestByIdQuery

GetWorkRequestsQuery

GetOpenWorkRequestsQuery

GetAssetWorkRequestsQuery

GetDepartmentWorkRequestsQuery
```

---

# API ENDPOINTS

```http
GET    /api/v1/maintenance/work-requests

GET    /api/v1/maintenance/work-requests/{id}

POST   /api/v1/maintenance/work-requests

PUT    /api/v1/maintenance/work-requests/{id}

POST   /api/v1/maintenance/work-requests/{id}/submit

POST   /api/v1/maintenance/work-requests/{id}/assign

POST   /api/v1/maintenance/work-requests/{id}/approve

POST   /api/v1/maintenance/work-requests/{id}/reject

POST   /api/v1/maintenance/work-requests/{id}/convert

POST   /api/v1/maintenance/work-requests/{id}/cancel

POST   /api/v1/maintenance/work-requests/{id}/close
```

---

# AUTHORIZATION

```text
maintenance.request.read

maintenance.request.create

maintenance.request.submit

maintenance.request.assign

maintenance.request.approve

maintenance.request.reject

maintenance.request.convert

maintenance.request.cancel

maintenance.request.close
```

---

# DATABASE TABLES

## WorkRequests

```text
Id

WorkRequestNumber

AssetId

RequestSource

Priority

Severity

Status

Title

Description

RequestedBy

AssignedTo

DepartmentId

ProductionOrderId

WorkOrderId

FailureCode

FailureDate

SubmittedAt

ApprovedAt

ClosedAt

CreatedAt

UpdatedAt
```

---

## WorkRequestComments

```text
Id

WorkRequestId

Comment

CommentedBy

CreatedAt
```

---

## WorkRequestAttachments

```text
Id

WorkRequestId

FileName

StorageReference

ContentType

UploadedBy

UploadedAt
```

---

# INDEXES

```text
IX_WorkRequestNumber (Unique)

IX_AssetId

IX_Status

IX_Priority

IX_RequestSource

IX_DepartmentId

IX_RequestedBy

IX_AssignedTo
```

---

# AUDIT

Audit every

- Request creation
- Submission
- Assignment
- Approval
- Rejection
- Conversion
- Cancellation
- Closure
- Comment creation
- Attachment upload

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

- Create Work Request
- Submit Request
- Assign Request
- Approve Request
- Reject Request
- Convert to Work Order
- Close Request
- Prevent conversion before approval
- Prevent modification after closure

## Integration Tests

- Repository
- Commands
- Queries
- REST API
- Asset integration
- Notification integration
- Work Order integration
- Domain Events
- Audit

---

# ACCEPTANCE CRITERIA

- Every Work Request references a valid Asset.
- Requests follow the defined approval workflow.
- Only approved requests can generate Maintenance Work Orders.
- Converted requests maintain Work Order references.
- Closed requests become immutable.
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
- Notification integration completed
- Work Order integration completed
- Authorization implemented
- Audit implemented
- Domain Events implemented
- Unit tests passing
- Integration tests passing
- Code review approved
