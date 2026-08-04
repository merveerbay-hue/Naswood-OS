# Database Schema — Workflow

**Project:** Naswood OS
**Document:** Workflow Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Workflow module manages business processes, approvals, state transitions, task assignments and automated actions across Naswood OS.

It coordinates operations between departments while ensuring traceability, accountability and process consistency.

---

# Philosophy

Business processes consist of workflows.

Workflows consist of states.

State transitions are controlled by business rules.

Every workflow execution is fully traceable.

---

# Entity List

WorkflowDefinition

WorkflowState

WorkflowTransition

WorkflowInstance

WorkflowTask

WorkflowApproval

WorkflowComment

WorkflowAttachment

WorkflowAction

---

# workflow_definition

Defines a reusable workflow.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(50) |
| name | VARCHAR(150) |
| module | VARCHAR(50) |
| version | INTEGER |
| active | BOOLEAN |

Examples

- Purchase Approval
- Production Approval
- Quality Approval
- Maintenance Approval
- Sales Approval
- Engineering Change

---

# workflow_state

Possible states within a workflow.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_definition_id | UUID FK |
| state_code | VARCHAR(50) |
| state_name | VARCHAR(100) |
| sequence | INTEGER |
| is_initial | BOOLEAN |
| is_final | BOOLEAN |

Examples

Draft

Submitted

Under Review

Approved

Rejected

Completed

Cancelled

---

# workflow_transition

Defines allowed state changes.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_definition_id | UUID FK |
| from_state_id | UUID FK |
| to_state_id | UUID FK |
| action_name | VARCHAR(100) |
| required_role_id | UUID FK |
| approval_required | BOOLEAN |

Examples

Submit

Approve

Reject

Cancel

Complete

---

# workflow_instance

Represents a running workflow.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_definition_id | UUID FK |
| entity_type | VARCHAR(50) |
| entity_id | UUID |
| current_state_id | UUID FK |
| started_by | UUID FK |
| started_at | TIMESTAMP |
| completed_at | TIMESTAMP |
| status | VARCHAR(30) |

Status

- Active
- Completed
- Cancelled
- Suspended

---

# workflow_task

Task assigned to a user or role.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_instance_id | UUID FK |
| assigned_user_id | UUID FK |
| assigned_role_id | UUID FK |
| task_name | VARCHAR(150) |
| due_date | TIMESTAMP |
| priority | VARCHAR(20) |
| status | VARCHAR(30) |

Priority

- Low
- Normal
- High
- Critical

Status

- Open
- In Progress
- Completed
- Cancelled

---

# workflow_approval

Approval history.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_instance_id | UUID FK |
| approved_by | UUID FK |
| decision | VARCHAR(30) |
| decision_date | TIMESTAMP |
| remarks | TEXT |

Decision

- Approved
- Rejected
- Returned
- Escalated

---

# workflow_comment

Discussion history.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_instance_id | UUID FK |
| user_id | UUID FK |
| comment | TEXT |
| created_at | TIMESTAMP |

---

# workflow_attachment

Supporting documents.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_instance_id | UUID FK |
| file_name | VARCHAR(255) |
| file_reference | TEXT |
| uploaded_by | UUID FK |
| uploaded_at | TIMESTAMP |

---

# workflow_action

Automatic actions executed during transitions.

| Field | Type |
|--------|------|
| id | UUID |
| workflow_transition_id | UUID FK |
| action_type | VARCHAR(50) |
| target_module | VARCHAR(50) |
| configuration | JSONB |

Action Types

- Create Event
- Send Notification
- Generate Task
- Create Audit Log
- Create Production Order
- Reserve Inventory
- Release Inventory
- Call API
- Execute AI Agent

---

# Relationships

Workflow Definition

1 → N Workflow States

Workflow Definition

1 → N Workflow Transitions

Workflow Definition

1 → N Workflow Instances

Workflow Instance

1 → N Workflow Tasks

Workflow Instance

1 → N Workflow Approvals

Workflow Instance

1 → N Comments

Workflow Instance

1 → N Attachments

Workflow Transition

1 → N Workflow Actions

---

# Business Rules

### BR-1701

Every workflow shall have exactly one initial state.

---

### BR-1702

A workflow may have multiple final states.

---

### BR-1703

Transitions are allowed only if defined.

---

### BR-1704

Workflow approvals shall be immutable.

---

### BR-1705

Workflow actions execute automatically after successful transitions.

---

### BR-1706

Every workflow transition generates:

- Business Event
- Audit Log

---

### BR-1707

Workflow permissions follow the Role-Based Access Control model.

---

### BR-1708

Workflow history shall never be deleted.

---

### BR-1709

Completed workflows become read-only.

---

### BR-1710

Workflow execution shall remain traceable to the originating business entity.

---

# Standard Workflow Templates

Sales Quotation Approval

Sales Order Approval

Purchase Request Approval

Purchase Order Approval

Material Registration

Incoming Quality Inspection

Production Order Release

Production Completion

Maintenance Work Order

Engineering Change

Inventory Adjustment

Tool Change Approval

Customer Complaint

Shipment Approval

Financial Approval

---

# Integration

Workflow integrates with:

- Organization
- Security
- Production
- Inventory
- Quality
- Machines
- Tooling
- Maintenance
- Sales
- Purchasing
- Finance
- Logistics
- Events
- Audit Log
- Notifications
- AI

---

# Future Extensions

The architecture supports:

- BPMN 2.0
- Visual Workflow Designer
- Conditional Routing
- Parallel Approvals
- Sequential Approvals
- SLA Monitoring
- Escalation Rules
- Digital Signatures
- Low-Code Workflow Builder
- AI Workflow Assistant

---

# Workflow Philosophy

Workflow is the orchestration layer of Naswood OS.

It connects departments, automates business processes and ensures that every approval, decision and transition follows a controlled, traceable and configurable path.

Reliable workflows enable reliable manufacturing operations.
