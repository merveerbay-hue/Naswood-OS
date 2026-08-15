# ==============================================================================
# TASK-085 — IMPLEMENTATION
# OVERALL EQUIPMENT EFFECTIVENESS (OEE)
# Naswood Operating System (NOS)
# Module: Maintenance Management / Manufacturing Intelligence
# Sprint: Sprint 07 – Maintenance
# Status: Ready for Development
# ==============================================================================

# OBJECTIVE

Implement the Overall Equipment Effectiveness (OEE) module responsible for
measuring, calculating and visualizing manufacturing performance across all
production assets.

OEE is calculated from real production events.

The OEE module consumes Production, Downtime, Quality and Maintenance
projections.

It never modifies transactional data.

---

# DOMAIN

Manufacturing Intelligence

Projection

```
OEE
```

(Read Model / CQRS Projection)

---

# REFERENCES

Implementation must comply with:

- Constitution
- Maintenance_Architecture.md
- Production_Architecture.md
- Quality_Architecture.md
- Maintenance_Dashboard.md
- Production_Dashboard.md
- TASK-059_Production_Confirmation.md
- TASK-062_Production_Quality.md
- TASK-081_Downtime.md

---

# DEPENDENCIES

Consumes CQRS projections from

- Production Execution
- Production Confirmation
- Downtime
- Machine
- Work Center
- Production Line
- Quality
- Scrap
- Maintenance
- Shift
- Calendar

---

# PROJECTION MODEL

```
OEEProjection

├── Availability
├── Performance
├── Quality
├── OEE
├── Shift KPI
├── Machine KPI
├── Work Center KPI
├── Production Line KPI
├── Plant KPI
└── Historical Trends
```

---

# OEE FORMULA

```
OEE = Availability × Performance × Quality
```

All calculations use decimal precision.

Example

```
Availability = 0.92

Performance = 0.96

Quality = 0.98

OEE = 86.58%
```

---

# AVAILABILITY

Formula

```
Availability = Run Time / Planned Production Time
```

Run Time

```
Planned Production Time

-

Downtime
```

Downtime includes

```
Mechanical

Electrical

Setup

Material Waiting

Operator Waiting

Utility Failure

Maintenance

Quality Hold
```

---

# PERFORMANCE

Formula

```
Performance

=

Ideal Cycle Time

×

Good Pieces

/

Run Time
```

Alternative calculation

```
Actual Production Rate

/

Ideal Production Rate
```

---

# QUALITY

Formula

```
Quality

=

Good Quantity

/

Total Produced Quantity
```

Rejected products

```
Rejected

Scrap

Rework Pending
```

are excluded from Good Quantity.

---

# KPI LEVELS

Calculated for

```
Machine

↓

Work Center

↓

Production Line

↓

Plant

↓

Enterprise
```

Aggregation is weighted.

---

# OEE STATUS

```text
Excellent

Good

Acceptable

Poor

Critical
```

Recommended thresholds

```text
Excellent ≥ 85%

Good ≥ 75%

Acceptable ≥ 60%

Poor ≥ 40%

Critical < 40%
```

Thresholds are configurable.

---

# OEE RECORD

```
Id

MachineId

WorkCenterId

ProductionLineId

PlantId

ShiftId

ProductionDate

PlannedProductionTime

RunTime

DowntimeMinutes

IdealCycleTime

ActualCycleTime

ProducedQuantity

GoodQuantity

RejectedQuantity

Availability

Performance

Quality

OEE

CalculatedAt
```

---

# LIVE KPI PANEL

Display

```
Current OEE

Availability

Performance

Quality

Machine Status

Current Shift

Current Production Order

Current Downtime

Current Operator
```

Updates automatically.

---

# TREND ANALYSIS

Supports

```
Hourly

Shift

Daily

Weekly

Monthly

Quarterly

Yearly
```

Historical comparisons

```
Current vs Previous

Current vs Target

Current vs Last Year
```

---

# LOSS ANALYSIS

Supports Six Big Losses

```
Equipment Failure

Setup & Adjustment

Idling

Reduced Speed

Process Defects

Startup Losses
```

Display

```
Minutes Lost

Percentage

Cost

Trend
```

---

# BENCHMARKS

Compare

```
Machine

Work Center

Production Line

Plant

Factory

Enterprise
```

Supports ranking.

---

# ALERTS

Generate alerts when

```
OEE below threshold

Availability below threshold

Performance below threshold

Quality below threshold

Downtime exceeded

Production speed reduced

Repeated failures detected
```

Priorities

```text
Critical

High

Medium

Low
```

---

# FILTERS

Supports filtering by

- Plant
- Production Line
- Work Center
- Machine
- Product
- Product Family
- Shift
- Operator
- Production Order
- Date Range

---

# DATA SOURCES

Consumes CQRS projections

```
ProductionProjection

DowntimeProjection

QualityProjection

MachineProjection

MaintenanceProjection

ShiftProjection
```

Never queries aggregates directly.

---

# API ENDPOINTS

```http
GET /api/v1/oee

GET /api/v1/oee/current

GET /api/v1/oee/history

GET /api/v1/oee/machines

GET /api/v1/oee/work-centers

GET /api/v1/oee/production-lines

GET /api/v1/oee/plants

GET /api/v1/oee/loss-analysis

GET /api/v1/oee/trends

GET /api/v1/oee/benchmark
```

---

# SIGNALR HUB

```
OeeDashboardHub
```

Publishes

```
OeeUpdated

AvailabilityChanged

PerformanceChanged

QualityChanged

DowntimeStarted

DowntimeEnded

ProductionConfirmed

ShiftCompleted
```

---

# AUTHORIZATION

```text
maintenance.oee.read

maintenance.oee.analytics

maintenance.oee.export

maintenance.oee.admin
```

---

# EXPORT

Supports

```
PDF

Excel

CSV

JSON

Power BI

REST API
```

---

# CACHING

Current Shift

```
5 seconds
```

Historical Data

```
1–5 minutes
```

---

# AUDIT

OEE is read-only.

Audit

- Dashboard access
- Export
- Benchmark generation

Capture

```text
UserId

Timestamp

DashboardView

ExportFormat

CorrelationId
```

---

# TESTS

## Unit Tests

- Availability calculation
- Performance calculation
- Quality calculation
- OEE calculation
- Threshold evaluation
- Loss calculation
- Weighted aggregation

## Integration Tests

- CQRS projections
- REST API
- SignalR updates
- Export
- Dashboard integration
- Authorization
- Cache invalidation

---

# ACCEPTANCE CRITERIA

- OEE is calculated using Availability × Performance × Quality.
- Machine, Work Center, Line and Plant OEE are available.
- Downtime automatically affects Availability.
- Scrap automatically affects Quality.
- Production rate automatically affects Performance.
- Six Big Losses analysis is supported.
- Dashboard updates in real time.
- CQRS read models are used exclusively.
- API integration tests pass.
- Audit logging is complete.
- All unit and integration tests succeed.

---

# DEFINITION OF DONE

- OEE projection implemented
- Calculation engine completed
- Dashboard API completed
- SignalR Hub implemented
- CQRS read side completed
- Loss analysis completed
- Benchmark engine completed
- Export implemented
- Authorization implemented
- Audit implemented
- Unit tests passing
- Integration tests passing
- Performance validated
- Code review approved
