# ==============================================================================
# TASK-083 — IMPLEMENTATION
# MAINTENANCE DASHBOARD
# Naswood Operating System (NOS)
# Module: Maintenance Management
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Maintenance Dashboard projection responsible for providing
real-time operational visibility into enterprise maintenance activities.

The Maintenance Dashboard aggregates data from Assets, Work Requests,
Maintenance Work Orders, Preventive Maintenance, Corrective Maintenance,
Downtime and Spare Parts.

The Dashboard is a CQRS Read Model.

It never modifies transactional data.

---

# DOMAIN

Maintenance Management

Projection

```
MaintenanceDashboard
```

(Read Model / CQRS Projection)

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Maintenance_Workflow.md
- Maintenance_Dashboard.md
- Maintenance_API.md
- TASK-076_Asset.md
- TASK-077_Work_Request.md
- TASK-078_Maintenance_Order.md
- TASK-079_Preventive_Maintenance.md
- TASK-080_Corrective_Maintenance.md
- TASK-081_Downtime.md
- TASK-082_Spare_Parts.md

---

# DEPENDENCIES

Consumes CQRS projections from

- Asset
- Work Request
- Maintenance Work Order
- Preventive Maintenance
- Corrective Maintenance
- Downtime
- Spare Parts
- Inventory
- Production
- Employee

---

# PROJECTION MODEL

```
MaintenanceDashboard

├── Executive KPIs
├── Live Assets
├── Active Work Orders
├── Preventive Maintenance
├── Corrective Maintenance
├── Downtime
├── Technician Workload
├── Spare Parts
├── Maintenance Cost
├── MTBF / MTTR
├── Alerts
└── Trends
```

---

# KPI CARDS

Display

```
Active Assets

Running Assets

Assets Under Maintenance

Open Work Requests

Open Work Orders

Preventive Maintenance Due

Corrective Maintenance Open

Open Downtime

Maintenance Backlog

MTBF

MTTR

Maintenance Compliance %

Preventive Completion %

Maintenance Cost

Technician Utilization
```

KPIs refresh automatically.

---

# LIVE ASSET STATUS

Display

```
Asset Code

Asset Name

Current Status

Current Work Order

Work Center

Production Line

Criticality

Last Maintenance

Next Preventive Maintenance
```

Supports live updates through SignalR/WebSocket.

---

# WORK REQUEST PANEL

Display

```
Submitted Requests

Pending Approval

Approved

Rejected

Converted to Work Order

Average Approval Time
```

---

# MAINTENANCE WORK ORDER PANEL

Display

```
Open Work Orders

Released

In Progress

Waiting for Parts

Completed Today

Closed Today

Overdue Work Orders
```

---

# PREVENTIVE MAINTENANCE PANEL

Display

```
Due Today

Due This Week

Overdue

Completed

Compliance %

Auto Generated Work Orders
```

---

# CORRECTIVE MAINTENANCE PANEL

Display

```
Open Failures

Critical Failures

Emergency Repairs

Verification Pending

Closed Today
```

---

# DOWNTIME PANEL

Display

```
Open Downtime

Current Downtime

Total Downtime

Average Downtime

Top Downtime Reasons

Downtime by Asset

Downtime Trend
```

---

# SPARE PARTS PANEL

Display

```
Critical Spare Parts

Low Stock

Reserved Parts

Issued Today

Backordered Parts

Lead Time Warnings
```

---

# TECHNICIAN PANEL

Display

```
Technicians Available

Technicians Working

Assigned Work Orders

Completed Today

Average Repair Time

Labor Utilization
```

---

# MAINTENANCE COST PANEL

Display

```
Labor Cost

Material Cost

External Service Cost

Downtime Cost

Total Maintenance Cost

Cost by Asset

Cost by Work Center
```

---

# PERFORMANCE METRICS

Display

```
MTBF

MTTR

Maintenance Compliance

Asset Availability

Preventive vs Corrective Ratio

Maintenance Response Time

Repair Effectiveness
```

---

# ALERT PANEL

Supported Alerts

```
Critical Asset Failure

Emergency Work Order

Overdue Preventive Maintenance

Overdue Corrective Maintenance

Downtime Threshold Exceeded

Critical Spare Part Shortage

Warranty Expiring

Technician Overload
```

Alert priorities

```text
Critical

High

Medium

Low
```

---

# TREND ANALYSIS

Historical trends

```
Maintenance Trend

Failure Trend

Downtime Trend

MTBF Trend

MTTR Trend

Maintenance Cost Trend

Preventive Compliance Trend

Asset Availability Trend
```

Supports daily, weekly, monthly and yearly analysis.

---

# FILTERS

Dashboard supports filtering by

- Plant
- Production Line
- Work Center
- Asset
- Asset Category
- Criticality
- Technician
- Department
- Work Order Type
- Maintenance Status
- Date Range

All dashboard widgets respond to filters.

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

Dashboard consumes CQRS projections

```
AssetProjection

MaintenanceProjection

PreventiveProjection

CorrectiveProjection

DowntimeProjection

InventoryProjection

EmployeeProjection

ProductionProjection
```

Dashboard never queries aggregates directly.

---

# API ENDPOINTS

```http
GET /api/v1/dashboard/maintenance

GET /api/v1/dashboard/maintenance/kpis

GET /api/v1/dashboard/maintenance/assets

GET /api/v1/dashboard/maintenance/work-orders

GET /api/v1/dashboard/maintenance/preventive

GET /api/v1/dashboard/maintenance/corrective

GET /api/v1/dashboard/maintenance/downtime

GET /api/v1/dashboard/maintenance/spare-parts

GET /api/v1/dashboard/maintenance/cost

GET /api/v1/dashboard/maintenance/alerts

GET /api/v1/dashboard/maintenance/trends
```

---

# SIGNALR HUB

```
MaintenanceDashboardHub
```

Publishes

```
AssetStatusChanged

WorkRequestSubmitted

WorkOrderCreated

WorkOrderReleased

MaintenanceStarted

MaintenanceCompleted

PreventiveMaintenanceDue

CorrectiveMaintenanceCreated

DowntimeStarted

DowntimeEnded

SparePartLowStock

MaintenanceAlertRaised
```

---

# AUTHORIZATION

```text
maintenance.dashboard.read

maintenance.dashboard.kpi

maintenance.dashboard.analytics

maintenance.dashboard.export
```

---

# EXPORT

Supported exports

```
PDF

Excel

CSV

Power BI

REST API
```

Supports scheduled dashboard snapshots.

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

- Dashboard access
- Export requests
- Saved filters
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

- KPI calculations
- Projection mapping
- Alert prioritization
- Filter behavior
- Trend calculations
- MTBF calculation
- MTTR calculation

## Integration Tests

- CQRS projections
- SignalR notifications
- REST API
- Authorization
- Cache invalidation
- Export functionality

---

# ACCEPTANCE CRITERIA

- Dashboard displays real-time maintenance KPIs.
- Asset status updates automatically.
- Work Orders, Preventive Maintenance and Downtime are synchronized.
- MTBF and MTTR calculations are accurate.
- Spare Parts information reflects Inventory projections.
- Dashboard uses CQRS read models only.
- SignalR updates function correctly.
- Export functionality operates successfully.
- Dashboard remains read-only.
- API integration tests pass.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- Read model projections implemented
- Dashboard API completed
- SignalR Hub implemented
- CQRS read side completed
- Caching implemented
- Export completed
- Authorization implemented
- Dashboard widgets completed
- Unit tests passing
- Integration tests passing
- Performance validated
- Code review approved
