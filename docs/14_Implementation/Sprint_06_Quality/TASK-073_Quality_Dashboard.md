# ==============================================================================
# TASK-073 — IMPLEMENTATION
# QUALITY DASHBOARD
# Naswood Operating System (NOS)
# Module: Quality Management
# Sprint: Sprint 06 – Quality
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Quality Dashboard projection responsible for providing real-time
visibility into quality performance across Incoming Inspection,
In-Process Inspection, Final Inspection, Non-Conformance, CAPA and Quality
Certificates.

The Quality Dashboard is a read-only CQRS projection.

It aggregates quality information for Operators, Quality Engineers,
Production Managers and Executive Management.

The Dashboard never modifies transactional data.

---

# DOMAIN

Quality Management

Projection

```
QualityDashboard
```

(Read Model / CQRS Projection)

---

# REFERENCES

Implementation must comply with:

- Constitution
- Quality_Architecture.md
- Quality_Workflow.md
- Quality_Dashboard.md
- Quality_API.md
- TASK-066_Inspection_Plan.md
- TASK-067_Inspection_Execution.md
- TASK-068_In_Process_Inspection.md
- TASK-069_Final_Inspection.md
- TASK-070_Non_Conformance.md
- TASK-071_CAPA.md
- TASK-072_Quality_Certificate.md

---

# DEPENDENCIES

Consumes read models from

- Inspection Plan
- Inspection Execution
- In-Process Inspection
- Final Inspection
- Non-Conformance
- CAPA
- Quality Certificate
- Production
- Inventory
- Genealogy

---

# PROJECTION MODEL

```
QualityDashboard

├── Executive KPIs
├── Live Inspections
├── Quality Status
├── NCR Summary
├── CAPA Summary
├── Inspection Trends
├── FPY
├── PPM
├── Customer Quality
├── Certificates
├── Alerts
└── Trends
```

---

# KPI CARDS

Display

```
Open Inspections

Completed Today

Open NCR

Critical NCR

Open CAPA

Overdue CAPA

FPY %

PPM

Pass Rate

Reject Rate

Rework Rate

Scrap Rate

Certificates Issued
```

KPIs refresh automatically.

---

# LIVE INSPECTIONS

Display

```
Inspection Number

Inspection Type

Production Order

Work Order

Product

Inspector

Status

Decision

Elapsed Time
```

Supports live updates through SignalR/WebSocket.

---

# QUALITY STATUS

Display

```
Incoming Inspection

In-Process Inspection

Final Inspection

Products On Hold

Released Products

Blocked Products
```

---

# NON-CONFORMANCE PANEL

Display

```
Open NCR

Critical NCR

Severity Distribution

Disposition Status

Root Cause Pending

Corrective Actions Pending
```

---

# CAPA PANEL

Display

```
Open CAPA

Implementation

Verification

Effectiveness Review

Overdue CAPA

Completed CAPA
```

---

# CERTIFICATE PANEL

Display

```
Certificates Generated

Certificates Released

Pending Signatures

Revoked Certificates

Expired Certificates
```

---

# QUALITY METRICS

Display

```
First Pass Yield

Inspection Pass Rate

Defect Rate

Customer Reject Rate

Supplier Reject Rate

Scrap %

Rework %

Cost of Poor Quality
```

---

# ALERT PANEL

Supported Alerts

```
Critical NCR

Overdue CAPA

Failed Final Inspection

Production On Hold

Quality Hold

Certificate Expiring

High Reject Rate

Inspection Overdue
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
Inspection Trend

NCR Trend

CAPA Trend

FPY Trend

PPM Trend

Scrap Trend

Rework Trend

Customer Complaints
```

Supports daily, weekly, monthly and yearly analysis.

---

# FILTERS

Dashboard supports filtering by

- Plant
- Product
- Product Family
- Production Order
- Work Order
- Customer
- Supplier
- Inspector
- Quality Engineer
- Department
- Date Range

All widgets respond to filters.

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

Dashboard consumes CQRS read models

```
InspectionProjection

QualityProjection

NcrProjection

CapaProjection

CertificateProjection

ProductionProjection

InventoryProjection

GenealogyProjection
```

Dashboard never queries aggregates directly.

---

# API ENDPOINTS

```http
GET /api/v1/dashboard/quality

GET /api/v1/dashboard/quality/kpis

GET /api/v1/dashboard/quality/inspections

GET /api/v1/dashboard/quality/ncr

GET /api/v1/dashboard/quality/capa

GET /api/v1/dashboard/quality/certificates

GET /api/v1/dashboard/quality/fpy

GET /api/v1/dashboard/quality/ppm

GET /api/v1/dashboard/quality/alerts

GET /api/v1/dashboard/quality/trends
```

---

# SIGNALR HUB

```
QualityDashboardHub
```

Publishes

```
InspectionCompleted

InspectionFailed

InspectionApproved

FinalInspectionReleased

NcrCreated

NcrClosed

CapaCreated

CapaClosed

CertificateGenerated

CertificateReleased

QualityAlertRaised
```

---

# AUTHORIZATION

```text
quality.dashboard.read

quality.dashboard.kpi

quality.dashboard.analytics

quality.dashboard.export
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

# EXPORT

Supported exports

```
PDF

Excel

CSV

Power BI

REST API
```

Scheduled reporting is supported.

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

## Integration Tests

- CQRS projections
- SignalR notifications
- REST API
- Authorization
- Cache invalidation
- Export functionality

---

# ACCEPTANCE CRITERIA

- Dashboard displays real-time Quality KPIs.
- Live inspections update automatically.
- NCR and CAPA status are always current.
- FPY, PPM and Pass Rate calculations are accurate.
- Dashboard uses CQRS read models only.
- SignalR updates work correctly.
- Export functions operate successfully.
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
