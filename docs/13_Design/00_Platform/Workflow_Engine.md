# Workflow Engine

**Module:** Platform

**Domain:** Workflow

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Workflow Engine provides reusable, versioned and auditable process
orchestration for every NOS module.

It owns process execution mechanics. Business modules retain ownership of
business rules, authorization decisions and entity state changes.

---

# Scope

Included:

- Workflow definitions and versions
- Sequential and parallel paths
- Conditional routing
- Human approval tasks
- Service tasks
- Timers and timeouts
- Escalation and delegation
- Instance execution history
- Retry and manual intervention
- Workflow notifications

Excluded:

- Module business rules
- Direct writes to module tables
- User identity and permission ownership
- Notification delivery implementation
- Autonomous AI approvals

---

# Architecture

```
Business Module
↓
Workflow Port
↓
Workflow Application Service
↓
Workflow Domain
↓
Persistence / Scheduler / Event Bus Adapters
```

Modules start and interact with workflows through versioned contracts.
Workflow actions invoke module commands through module ports.

---

# Ownership

Workflow Engine owns:

- Definition
- Definition Version
- Node and transition graph
- Workflow Instance
- Work Item
- Assignment
- Delegation
- Escalation
- Timer
- Execution Attempt
- Execution History

Business modules own:

- Eligibility to start a workflow
- Business validation
- Business entity state
- Approval authority rules
- Resulting domain events

Platform Authorization owns permission evaluation. Notification Service owns
message delivery.

---

# Aggregates

## Workflow Definition

Aggregate Root: `WorkflowDefinition`

Contains:

- Stable workflow code
- Owning module
- Name and description
- Status
- Active version
- Version history

A published version is immutable. Changes create a new version.

## Workflow Instance

Aggregate Root: `WorkflowInstance`

Contains:

- Definition and version
- Business subject reference
- Current state
- Active work items
- Variables
- Correlation ID
- Start and completion metadata

An instance remains bound to the definition version with which it started.

---

# Node Types

- Start
- End
- Human Task
- Approval Task
- Service Task
- Exclusive Gateway
- Parallel Gateway
- Join
- Timer
- Wait for Event
- Manual Intervention

Node implementations are registered capabilities. A workflow definition cannot
execute arbitrary source code or database statements.

---

# Definition Lifecycle

```
Draft → Validated → Published → Deprecated → Retired
```

- Draft may be edited.
- Validated has passed structural and contract validation.
- Published is immutable and can start new instances.
- Deprecated remains available to existing instances.
- Retired cannot start new instances but remains readable.

Publication requires authorization and audit.

---

# Instance Lifecycle

```
Created → Running → Waiting → Completed
                    ↘ Failed → Manual Intervention
Running → Cancelled
```

Cancellation eligibility belongs to the initiating business module.
Workflow Engine records and executes an approved cancellation but does not
invent cancellation rules.

---

# Work Item Lifecycle

```
Created → Assigned → Claimed → Completed
          ↓          ↓
       Delegated   Returned
          ↓
       Escalated

Created or Assigned → Expired
Created or Assigned → Cancelled
```

Every transition validates expected version and authority.

---

# Approval Decision

An approval decision includes:

- Decision ID
- Workflow Instance ID
- Work Item ID
- Business subject type and ID
- Decision: Approve, Reject or Return
- Actor ID and actor type
- Evaluated permission/policy
- Reason
- Timestamp
- Expected subject version
- Correlation ID

Approval does not directly update the business entity. It invokes the owning
module command, which revalidates business state and authorization.

---

# Assignment

Supported assignment strategies:

- Named user
- Role
- Permission
- Organizational unit
- Manager hierarchy
- Subject owner
- Configured resolver port

Assignment resolvers return eligible identities; they do not grant permission.
Authorization is re-evaluated when the action is executed.

---

# Delegation and Escalation

Delegation records source user, delegate, scope, effective period, reason and
authorizer.

Escalation may:

- Notify
- Reassign
- Add approvers
- Create a supervisory task
- Move to manual intervention

Escalation cannot silently approve or reject a business decision.

---

# Service Tasks

A service task invokes a registered versioned module command.

Each invocation defines:

- Command contract
- Idempotency key
- Timeout
- Retry policy
- Success event
- Failure event
- Compensating command where business-valid

Direct HTTP URLs, SQL statements and secrets shall not be embedded in workflow
definitions.

---

# Database

Canonical tables:

- `workflow_definitions`
- `workflow_definition_versions`
- `workflow_nodes`
- `workflow_transitions`
- `workflow_instances`
- `workflow_work_items`
- `workflow_assignments`
- `workflow_delegations`
- `workflow_timers`
- `workflow_execution_attempts`
- `workflow_history`
- `workflow_outbox`
- `workflow_inbox`

Workflow tables store external business references as stable identifiers.
Cross-module foreign keys are prohibited.

Definition versions, decisions and history are immutable.

---

# API

Definition management:

```
GET  /api/v1/workflow-definitions
POST /api/v1/workflow-definitions
POST /api/v1/workflow-definitions/{id}/validate
POST /api/v1/workflow-definitions/{id}/publish
POST /api/v1/workflow-definitions/{id}/deprecate
```

Runtime:

```
POST /api/v1/workflow-instances
GET  /api/v1/workflow-instances/{id}
POST /api/v1/workflow-instances/{id}/cancel
GET  /api/v1/work-items
POST /api/v1/work-items/{id}/claim
POST /api/v1/work-items/{id}/complete
POST /api/v1/work-items/{id}/delegate
POST /api/v1/work-items/{id}/return
```

All APIs use canonical envelopes, concurrency and idempotency standards.

---

# Events

Published integration events:

- WorkflowDefinitionPublished
- WorkflowStarted
- WorkItemCreated
- WorkItemAssigned
- WorkItemDelegated
- WorkItemEscalated
- WorkItemCompleted
- WorkflowCompleted
- WorkflowFailed
- WorkflowCancelled

Business modules publish their own resulting business facts. Workflow Engine
shall not publish `PurchaseOrderApproved`, `SalesOrderReleased` or equivalent
facts on behalf of the owning module.

---

# Authorization

Required capability groups:

- Workflow Definition View
- Workflow Definition Manage
- Workflow Definition Publish
- Workflow Instance View
- Workflow Instance Cancel
- Work Item View
- Work Item Claim
- Work Item Decide
- Work Item Delegate
- Workflow Administration

Company, plant, module, subject and assignment scope are evaluated through the
Platform Authorization service.

---

# Audit

Audit includes:

- Definition creation and publication
- Version changes
- Instance start and cancellation
- Assignment and delegation
- Approval decisions
- Escalation
- Timer firing
- Service-task attempts
- Failure and manual intervention

Workflow history and audit records are not interchangeable. Both are retained
according to their governing standards.

---

# Reliability

- Timers use durable scheduling.
- Service tasks are idempotent.
- Events use outbox/inbox processing.
- Retries use bounded backoff.
- Exhausted retries create manual intervention.
- A process never reports completion until the owning module command succeeds.

---

# AI Restrictions

AI may:

- Recommend a route
- Summarize workflow context
- Identify delay risk
- Recommend an approver permitted by policy

AI shall not approve, reject, bypass authorization, modify definitions without
authorization or execute restricted module commands autonomously.

---

# Acceptance Criteria

- Definitions are versioned and published versions immutable.
- Running instances remain bound to their version.
- Workflow Engine never writes module tables.
- Business commands revalidate state and authorization.
- Decisions are traceable and auditable.
- Timers and retries survive process restarts.
- Consumers and service tasks are idempotent.
- No workflow definition contains executable source code, SQL or secrets.
- AI cannot finalize approval decisions.

---

# Related Documents

- `../../00_Project_Governance/Phase_0_Canonical_Contracts.md`
- `../../00_Project_Governance/Module_Boundaries_and_Ownership.md`
- `../99_Shared/Approval_Workflow.md`
- `../99_Shared/Transactions.md`
- `../99_Shared/Permission_Model.md`
- `../99_Shared/Notification_System.md`
