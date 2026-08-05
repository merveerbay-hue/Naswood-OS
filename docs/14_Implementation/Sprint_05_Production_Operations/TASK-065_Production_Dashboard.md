# ==============================================================================
# TASK-065 — IMPLEMENTATION
# PRODUCTION DASHBOARD
# Naswood Operating System (NOS)
# Module: Production Execution
# Sprint: Sprint 05 – Production Operations
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Production Dashboard module responsible for providing real-time
visibility into manufacturing execution across all Production Orders, Work
Orders, Machines, Production Lines and Plants.

The Production Dashboard is a read-only projection layer.

It aggregates operational data from Production Execution and presents live KPIs,
alerts and performance metrics for operators, supervisors and management.

The Dashboard never modifies transactional data.

---

# DOMAIN

Production Execution

Projection

```
ProductionDashboard
```

(Read Model / CQRS Projection)

---

# REFERENCES

Implementation must comply with:

- Constitution
- ADR-012 Product Capability Profile
- Production_Architecture.md
- Production_Workflow.md
- Production_Dashboard.md
- Production_API.md
- TASK-056_Production_Order.md
- TASK-057_Work_Order.md
- TASK-058_Production_Execution.md
- TASK-059_Production_Output.md
- TASK-060_Production_Downtime.md
- TASK-061_Labor_Tracking.md
- TASK-062_Production_Quality.md
- TASK-063_Production_Scrap.md
- TASK-065_Production_Analytics.md

---

# DEPENDENCIES

Consumes data from

- Production Order
- Work Order
- Production Execution
- Production Output
- Material Consumption
- Labor Tracking
- Downtime
- Scrap
- Quality
- Machine
- Work Center
- Production Line

---

# PROJECTION MODEL

```
ProductionDashboard

├── Executive KPIs
├── Live Production
├── Machine Status
├── Work Center Status
├── Production Orders
├── OEE
├── Capacity
├── Downtime
├── Quality
├── Scrap
├── Labor
├── Alerts
└── Trends
```

---

# KPI CARDS

Display

```
Running Orders

Completed Today

Running Machines

Available Machines

Active Operators

Produced Quantity

Scrap Quantity

OEE

Availability

Performance

Quality Rate

Labor Utilization

Capacity Utilization
```

Dashboard refreshes automatically.

---

# LIVE PRODUCTION

Display

```
Production Order

Current Operation

Machine

Operator

Work Center

Shift

Progress %

Produced Quantity

Remaining Quantity

Expected Finish Time
```

Supports live updates via SignalR/WebSocket.

---

# MACHINE STATUS

Supported Machine States

```text
Running

Idle

Setup

Maintenance

Downtime

Offline
```

Display

- Runtime
- Current Job
- Utilization
- OEE
- Downtime

---

# WORK CENTER STATUS

Display

- Active Orders
- Queue
- Capacity
- Utilization
- Bottleneck Indicator

---

# PRODUCTION LINE STATUS

Display

- Running Orders
- Active Machines
- Line OEE
- Throughput
- Bottleneck Work Center
- Capacity Utilization

---

# QUALITY PANEL

Display

```
Accepted Quantity

Rejected Quantity

Rework Quantity

Inspection Pass Rate

Open Holds
```

---

# SCRAP PANEL

Display

```
Scrap Quantity

Scrap %

Top Scrap Reasons

Scrap Trend

Scrap Cost
```

---

# DOWNTIME PANEL

Display

```
Current Downtime

Machine

Reason

Duration

MTTR

MTBF

Downtime Trend
```

---

# LABOR PANEL

Display

```
Operators Working

Labor Hours

Overtime

Labor Utilization

Productivity
```

---

# ALERT PANEL

Supported Alerts

```
Machine Down

Production Delayed

Low OEE

High Scrap

Quality Hold

Material Shortage

Maintenance Due

Operator Missing
```

Alerts are prioritized

```text
Critical

High

Medium

Low
```

---

# FILTERS

Dashboard supports filtering by

- Plant
- Production Line
- Work Center
- Machine
- Product
- Product Family
- Shift
- Production Order
- Planner
- Operator
- Date Range

Filters affect all dashboard widgets.

---

# REFRESH STRATEGY

Real-time widgets

```
SignalR / WebSocket
```

Operational KPIs

```
5–10 second refresh
```

Historical charts

```
30–60 second refresh
```

Heavy analytics

```
On-demand
```

---

# DATA SOURCES

Dashboard consumes read models from

```
ProductionExecutionProjection

MachineProjection

WorkCenterProjection

ProductionOrderProjection

OEEProjection

ScrapProjection

DowntimeProjection

LaborProjection

QualityProjection
```

Dashboard never queries aggregates directly.

---

# API ENDPOINTS

```http
GET /api/v1/dashboard/production

GET /api/v1/dashboard/production/kpis

GET /api/v1/dashboard/production/orders

GET /api/v1/dashboard/production/machines

GET /api/v1/dashboard/production/workcenters

GET /api/v1/dashboard/production/lines

GET /api/v1/dashboard/production/oee

GET /api/v1/dashboard/production/scrap

GET /api/v1/dashboard/production/downtime

GET /api/v1/dashboard/production/quality

GET /api/v1/dashboard/production/labor

GET /api/v1/dashboard/production/alerts
```

---

# SIGNALR HUB

```text
ProductionDashboardHub
```

Published updates

```
ProductionProgressUpdated

MachineStatusChanged

OrderCompleted

OrderStarted

ScrapRecorded

DowntimeStarted

DowntimeEnded

QualityUpdated

LaborUpdated

AlertRaised
```

---

# AUTHORIZATION

```text
production.dashboard.read

production.dashboard.kpi

production.dashboard.analytics

production.dashboard.export
```

---

# CACHING

Dashboard projections are cached.

Recommended cache duration

```
5–15 seconds
```

Historical analytics may use longer cache durations.

---

# AUDIT

Dashboard is read-only.

Audit only

- Export requests
- Dashboard access
- Filter changes (optional)
- Saved dashboard preferences

Capture

```text
UserId

Timestamp

DashboardView

ExportType

CorrelationId
```

---

# TESTS

## Unit Tests

- KPI calculation
- Projection mapping
- Alert prioritization
- Filter behavior
- Refresh logic

## Integration Tests

- Projection updates
- SignalR notifications
- REST API
- Authorization
- Cache invalidation

---

# ACCEPTANCE CRITERIA

- Dashboard displays real-time production status.
- Live Production Orders update automatically.
- Machine states refresh without page reload.
- OEE, Scrap and Downtime KPIs are accurate.
- Dashboard uses CQRS read models only.
- SignalR updates function correctly.
- API integration tests pass.
- Dashboard remains read-only.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- Read model projections implemented
- Dashboard API completed
- SignalR Hub implemented
- CQRS read side completed
- Caching implemented
- Authorization implemented
- Dashboard widgets completed
- Unit tests passing
- Integration tests passing
- Performance validated
- Code review approved
```
