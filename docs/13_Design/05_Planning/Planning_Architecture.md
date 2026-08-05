# Planning Architecture

**Module:** Planning

**Domain:** Enterprise and Manufacturing Planning

**Version:** 1.0

**Status:** Proposed

---

# Purpose

The Planning module synchronizes demand, supply, inventory, capacity, workforce
and operating constraints. It produces explainable plans and recommendations;
it does not mutate the authoritative source data owned by other modules.

This document defines architecture and ownership boundaries only. Planning
algorithms, priority policies, horizons and optimization objectives require
approved domain rules before implementation.

---

# Ownership

Planning owns:

- Demand Plan
- Supply Plan
- Material Requirements Plan run and result
- Capacity Plan
- Production Schedule
- Planning Scenario
- Planning Exception
- Planning Recommendation
- Delivery Commitment proposal

Planning does not own:

- Sales orders or forecasts submitted by Sales
- Inventory balances or reservations
- Purchase orders
- Product, BOM, routing or resource master
- Production orders after release
- Employee records
- Maintenance orders
- Financial budgets

---

# Inputs

Planning consumes versioned contracts or projections from:

| Source | Inputs |
|---|---|
| Sales | Sales orders, forecasts, required dates, priorities |
| Inventory | On hand, available, reserved, allocated, incoming, outgoing |
| Purchasing | Open purchase orders, supplier confirmations, lead-time evidence |
| Manufacturing | BOM, routing, machines, work centers, lines, tooling, parameters |
| Production | Released orders, progress, WIP, yield and actual times |
| Quality | Holds, release status and quality constraints |
| Maintenance | Planned downtime and asset availability |
| HR | Workforce availability and skills |
| Finance | Approved budget constraints where applicable |
| Platform | Calendar, organization, configuration and workflow |

Inputs retain source identifiers, source versions and effective timestamps.

Every Product-dependent input retains Product Revision ID and Capability
Profile ID. A completed Planning run never reinterprets historical inputs using
the Product's current profile.

---

# Outputs

Planning produces proposals:

- Material requirement
- Purchase recommendation
- Production-order proposal
- Capacity adjustment recommendation
- Schedule
- Reschedule recommendation
- Shortage exception
- Delivery-date proposal

A proposal does not alter another module. Conversion requires an authorized
command accepted by the owning module.

---

# Architecture

```
Source Module Events and APIs
↓
Planning Input Projections
↓
Planning Run
↓
Requirements and Capacity Evaluation
↓
Schedule / Scenario / Exceptions
↓
Approval or User Decision
↓
Commands to Owning Modules
```

Planning runs are reproducible from immutable input snapshots, algorithm
version and configuration version.

---

# Aggregates

## Planning Run

Contains:

- Run ID
- Planning type
- Company and plant scope
- Horizon
- Input snapshot
- Product Revision and Capability Profile references
- Algorithm version
- Configuration version
- Status
- Start/end metadata
- Results and exceptions

## Planning Scenario

Contains:

- Scenario ID
- Base run
- Assumption set
- Overrides
- Result comparison
- Owner
- Status

Scenario overrides never update source modules.

## Production Schedule

Contains:

- Schedule ID and version
- Horizon
- Resource assignments
- Operation sequence
- Planned start/end
- Constraint references
- Approval/release state

---

# Planning Types

- Demand Planning
- Material Requirements Planning
- Supply Planning
- Rough-Cut Capacity Planning
- Detailed Capacity Planning
- Finite Scheduling
- Scenario Planning

Each planning type declares required inputs, constraints, outputs and
explainability information.

---

# Lifecycle

Planning run:

```
Requested → Snapshotting → Running → Completed
                            ↓
                          Failed
Completed → Archived
```

Plan or schedule:

```
Draft → Reviewed → Approved → Released → Superseded
```

Approval requirements are configurable through the Workflow Engine and require
business-approved matrices.

---

# Constraint Model

Planning may evaluate:

- Material availability
- BOM and routing validity
- Machine/work-center capacity
- Production calendar
- Shift and workforce availability
- Tool availability
- Maintenance downtime
- Quality holds
- Supplier lead times
- Warehouse constraints
- Delivery commitments

The authoritative source module owns each constraint. Planning stores the
input snapshot and result, not a competing master.

---

# MRP Boundary

MRP calculates dependent demand from approved demand inputs and approved
product/BOM revisions.

MRP output is a recommendation. It does not directly create purchase or
production orders. Authorized conversion invokes Purchasing or Production
commands with the originating planning-result identifier and idempotency key.

Lot-sizing, safety stock, scrap factors, lead times and planning horizons are
domain configuration. Values are not defined by this architecture document.

---

# Scheduling Boundary

Planning owns planned operation sequence and time. Production owns execution,
actual timestamps and execution-state transitions.

Manufacturing owns resource capability definitions. Maintenance and HR own
availability inputs.

Rescheduling creates a new schedule version. It does not rewrite the historical
schedule used by released execution.

---

# Explainability

Every recommendation records:

- Input snapshot
- Constraints considered
- Binding constraints
- Configuration and algorithm version
- Reason codes
- Alternatives where available
- Confidence for probabilistic inputs
- Expected effect

AI may propose planning inputs or alternatives but cannot release a plan or
create operational orders without authorized confirmation.

---

# Database

Canonical tables:

- `planning_runs`
- `planning_input_snapshots`
- `planning_results`
- `planning_requirements`
- `capacity_plans`
- `production_schedules`
- `production_schedule_operations`
- `planning_scenarios`
- `planning_scenario_assumptions`
- `planning_exceptions`
- `planning_recommendations`
- `planning_outbox`
- `planning_inbox`

Source references are stable external identifiers without cross-module foreign
keys.

Large result sets may be partitioned by company, plant and planning date.

---

# API

```
POST /api/v1/planning-runs
GET  /api/v1/planning-runs/{id}
GET  /api/v1/planning-runs/{id}/results
GET  /api/v1/planning-exceptions
POST /api/v1/planning-scenarios
POST /api/v1/planning-scenarios/{id}/run
GET  /api/v1/production-schedules
POST /api/v1/production-schedules/{id}/submit
POST /api/v1/production-schedules/{id}/approve
POST /api/v1/planning-recommendations/{id}/convert
```

Long-running planning commands return an operation reference. Completion is
reported through status queries and events.

---

# Events

Published:

- PlanningRunRequested
- PlanningRunCompleted
- PlanningRunFailed
- MaterialShortageDetected
- CapacityConstraintDetected
- ProductionScheduleCreated
- ProductionScheduleApproved
- ProductionScheduleReleased
- PurchaseRecommendationCreated
- ProductionOrderProposalCreated
- DeliveryCommitmentProposed

Consumed event names are defined by the owning source modules and recorded in
the integration event catalog.

---

# Authorization

Permissions distinguish:

- View plans
- Run planning
- Create scenarios
- Modify assumptions
- Review schedule
- Approve schedule
- Release schedule
- Convert recommendations
- View sensitive cost or customer data

Company, plant and planning scope are enforced server-side.

---

# Workflow Dependencies

Workflow Engine may orchestrate:

- Plan review
- Schedule approval
- Capacity override approval
- Recommendation conversion

Planning remains responsible for validation and result state. Workflow Engine
does not edit Planning tables directly.

---

# Audit

Audit includes:

- Run request
- Input snapshot identity
- Assumption and configuration changes
- Manual overrides
- Approval and release
- Recommendation conversion
- Failed or cancelled runs

Historical plan versions and released schedules are retained.

---

# Acceptance Criteria

- Planning never owns or edits source-module truth.
- Every run is reproducible from its recorded inputs and versions.
- MRP outputs recommendations, not direct operational writes.
- Planning and Production states remain separate.
- Scheduling and Manufacturing resource ownership remain separate.
- Manual overrides are explicit and audited.
- AI recommendations remain explainable and approval-gated.
- Cross-module commands are idempotent.

---

# Pending Domain Decisions

Implementation remains blocked until approval of:

- Planning horizon hierarchy
- Forecast ownership
- Demand priority rules
- Lot-sizing policies
- Safety-stock policies
- Lead-time source and override rules
- Finite-capacity objectives
- Schedule-freeze rules
- Delivery-commitment authority
- Approval matrices

---

# Related Documents

- `../../00_Project_Governance/Module_Boundaries_and_Ownership.md`
- `../../00_Project_Governance/Phase_0_Canonical_Contracts.md`
- `../02_Inventory/Inventory_Ledger.md`
- `../02_Inventory/Reservation.md`
- `../00_Platform/Workflow_Engine.md`
- `../99_Shared/Transactions.md`
