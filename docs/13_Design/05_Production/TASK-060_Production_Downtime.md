# ==============================================================================
# TASK-060 — PRODUCTION DOWNTIME
# Naswood Operating System (NOS)
# Module: Production Execution
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Production Downtime module records every planned and unplanned interruption
that affects manufacturing execution.

Downtime is a manufacturing event.

It measures production losses, supports OEE calculations and provides the basis
for continuous improvement.

Downtime never modifies Production Orders.

It records operational reality.

---

# 2. OWNERSHIP

Module Owner

```
Production Execution
```

Production owns downtime events.

Maintenance owns maintenance activities.

Planning consumes downtime information.

Analytics calculates performance indicators.

---

# 3. RESPONSIBILITIES

The Production Downtime module is responsible for:

- Downtime Recording
- Downtime Classification
- Root Cause Assignment
- Machine Impact
- Work Center Impact
- Duration Calculation
- OEE Input
- Downtime Analytics

The module is NOT responsible for:

- Maintenance Work Orders
- Production Orders
- Inventory
- Machine Master Data
- Shift Definitions

---

# 4. DEPENDENCIES

Depends on

- Production Order
- Operation
- Machine
- Work Center
- Shift
- Downtime Reason

Referenced by

- Maintenance
- Planning
- Analytics
- Dashboard

---

# 5. AGGREGATE ROOT

```
DowntimeEvent
```

Children

- Downtime Reason
- Root Cause
- Corrective Action
- Attachments
- Audit

---

# 6. ENTITY MODEL

```
DowntimeEvent
│
├── Reason
├── Root Cause
├── Corrective Action
├── Attachments
└── Audit
```

---

# 7. DOWNTIME HEADER

Every Downtime Event contains

- Downtime Number
- Production Order
- Operation
- Machine
- Work Center
- Shift
- Start Time
- End Time
- Duration
- Status

Downtime Number is unique.

---

# 8. DOWNTIME TYPES

Supported downtime types

- Planned Maintenance
- Unplanned Breakdown
- Mechanical Failure
- Electrical Failure
- Material Shortage
- Tool Change
- Setup
- Quality Hold
- Operator Waiting
- Utility Failure
- Safety Stop
- Other

Additional types may be configured.

---

# 9. DOWNTIME REASONS

Each event references one standardized reason.

Example

```
Mechanical

↓

Bearing Failure
```

```
Material

↓

Material Not Available
```

```
Setup

↓

Tool Change
```

Reason codes are centrally managed.

---

# 10. ROOT CAUSE ANALYSIS

Each downtime may include

- Root Cause
- Corrective Action
- Preventive Action
- Responsible Department
- Resolution Date

Root Cause Analysis supports continuous improvement.

---

# 11. DURATION

Downtime duration is calculated from

- Start Time
- End Time

System derives

- Total Duration
- Planned Duration
- Unplanned Duration

Manual duration entry is not permitted.

---

# 12. MACHINE IMPACT

Each downtime references one Machine.

Machine runtime excludes downtime automatically.

Maintenance receives machine breakdown events.

---

# 13. WORK CENTER IMPACT

Downtime affects

- Work Center Capacity
- Production Progress
- Schedule Adherence
- OEE

Planning may automatically reschedule affected Production Orders.

---

# 14. VALIDATION RULES

Before saving validate

- Active Machine
- Active Production Order
- Valid Reason Code
- End Time ≥ Start Time
- Valid Shift
- No overlapping downtime for the same Machine

Invalid events cannot be posted.

---

# 15. BUSINESS RULES

Mandatory rules

- Every Downtime references one Machine.
- Every Downtime belongs to one Production Order.
- Duration is system calculated.
- Root Cause is mandatory for unplanned downtime.
- Downtime history is immutable.
- OEE calculations use recorded downtime events only.

---

# 16. API ENDPOINTS

```
GET    /api/v1/production/downtime

GET    /api/v1/production/downtime/{id}

POST   /api/v1/production/downtime

PUT    /api/v1/production/downtime/{id}

GET    /api/v1/production/downtime/reasons

GET    /api/v1/production/downtime/{id}/audit
```

---

# 17. EVENTS

Publishes

```
DowntimeStarted

DowntimeEnded

DowntimeRecorded

MachineStopped

MachineResumed

RootCauseCompleted
```

---

# 18. PERMISSIONS

```
production.downtime.read

production.downtime.create

production.downtime.update

production.downtime.analyze

production.downtime.audit
```

---

# 19. USER INTERFACE

The Downtime screen contains

Header

↓

Machine

↓

Production Order

↓

Reason

↓

Timeline

↓

Root Cause

↓

Corrective Action

↓

Attachments

↓

Audit Timeline

Downtime timers update in real time.

---

# 20. SEARCH & FILTERS

Support filtering by

- Downtime Number
- Machine
- Work Center
- Production Order
- Shift
- Reason
- Root Cause
- Status
- Date Range

---

# 21. AUDIT

Every downtime action records

- User
- Timestamp
- Previous Status
- New Status
- Machine
- Production Order
- Correlation ID

Audit records are immutable.

---

# 22. CROSS MODULE INTEGRATION

Production

Pauses and resumes manufacturing execution.

Maintenance

Receives breakdown notifications and creates Work Orders.

Planning

Adjusts production schedules based on downtime.

Analytics

Calculates

- Availability
- MTBF
- MTTR
- Downtime %
- OEE
- Bottleneck Analysis

Dashboard

Displays real-time downtime alerts and machine status.

---

# 23. REPORTING

Downtime reporting supports

- Downtime History
- Downtime by Machine
- Downtime by Work Center
- Downtime by Reason
- MTBF
- MTTR
- Pareto Analysis
- OEE Loss Analysis

Reports are generated from recorded downtime events.

---

# 24. SUCCESS CRITERIA

The Production Downtime module is successful when

- Every production interruption is recorded.
- Downtime durations are calculated automatically.
- Root causes are traceable.
- OEE calculations use actual downtime events.
- Planning reacts to operational disruptions.
- Historical downtime remains immutable.

---

# 25. FINAL DESIGN STATEMENT

The Production Downtime module is the canonical record of manufacturing
interruptions within the Naswood Operating System.

It captures every planned and unplanned production stop while maintaining
complete traceability, standardized root cause analysis and seamless
integration with Production, Maintenance, Planning and Analytics.

Downtime data provides the authoritative foundation for availability analysis,
continuous improvement and Overall Equipment Effectiveness (OEE).
