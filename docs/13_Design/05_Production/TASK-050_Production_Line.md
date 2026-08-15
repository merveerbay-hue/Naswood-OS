# ==============================================================================
# TASK-050 — PRODUCTION LINE
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Production Line represents a logical manufacturing flow consisting of one
or more Work Centers that collectively produce a family of products.

A Production Line provides the organizational structure for manufacturing
execution, production monitoring, capacity analysis and operational reporting.

Production Lines organize manufacturing.

Work Centers perform manufacturing.

Machines execute manufacturing.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Production Lines are owned exclusively by the Production Master module.

Planning schedules Work Centers.

Production executes Operations.

Production Lines organize manufacturing resources.

---

# 3. RESPONSIBILITIES

The Production Line module is responsible for:

- Production Line Master Data
- Work Center Organization
- Production Flow Definition
- Capacity Overview
- Line Availability
- Line Performance Monitoring
- Manufacturing Classification

The Production Line module is NOT responsible for:

- Production Orders
- Machine Configuration
- Routing
- Inventory
- Maintenance
- Scheduling

---

# 4. DEPENDENCIES

Depends on

- Plant
- Work Center
- Calendar

Referenced by

- Planning
- Production
- Dashboard
- Analytics
- Maintenance

---

# 5. AGGREGATE ROOT

```
ProductionLine
```

Children

- Assigned Work Centers
- Calendar Assignment
- Capacity Summary
- Attachments

---

# 6. ENTITY MODEL

```
ProductionLine
│
├── Work Centers
├── Calendar
├── Capacity
├── Attachments
└── Audit
```

---

# 7. PRODUCTION LINE MASTER

Every Production Line contains

- Line Code
- Line Name
- Description
- Plant
- Status
- Calendar
- Capacity Summary

Production Line Code is unique.

---

# 8. LINE TYPES

Examples

- CLT Line
- Glulam Line
- Thermowood Line
- Finger Joint Line
- Panel Line
- Packaging Line

Line Type is configurable.

---

# 9. WORK CENTER ASSIGNMENT

A Production Line contains one or more Work Centers.

Example

```
Thermowood Line

↓

Loading

↓

Kiln

↓

Cooling

↓

Sorting

↓

Packaging
```

A Work Center belongs to only one active Production Line.

---

# 10. CAPACITY SUMMARY

Capacity is calculated from assigned Work Centers.

Displayed values include

- Daily Capacity
- Weekly Capacity
- Monthly Capacity
- Utilization
- Bottleneck Work Center

Production Lines do not maintain independent capacity values.

Capacity is derived.

---

# 11. CALENDAR

Every Production Line references one operational calendar.

Calendar defines

- Working Days
- Working Hours
- Planned Shutdowns
- Holidays

Production Line availability depends on both the Line Calendar and the
availability of its Work Centers.

---

# 12. MANUFACTURING FLOW

Production Lines represent the logical manufacturing sequence.

Example

```
Work Center 10

↓

Work Center 20

↓

Work Center 30

↓

Work Center 40
```

Routing determines execution.

Production Line provides operational grouping.

---

# 13. DASHBOARD INTEGRATION

Production Dashboards display

- Active Orders
- Running Machines
- OEE
- Utilization
- Downtime
- Scrap
- Labor
- Capacity

All values are aggregated by Production Line.

---

# 14. VALIDATION RULES

System validates

- Unique Line Code
- Valid Plant
- Valid Calendar
- At least one Work Center
- No duplicate Work Centers
- Assigned Work Centers belong to the same Plant

Invalid Production Lines cannot become Active.

---

# 15. APPROVAL WORKFLOW

```
Draft

↓

Engineering Review

↓

Approved

↓

Active

↓

Inactive

↓

Archived
```

Only Active Production Lines participate in planning and reporting.

---

# 16. BUSINESS RULES

Mandatory rules

- A Production Line contains one or more Work Centers.
- Work Centers belong to only one active Production Line.
- Machines belong to Work Centers—not directly to Production Lines.
- Routing references Work Centers.
- Production Lines organize operational visibility only.
- Capacity is calculated from Work Centers.

---

# 17. API ENDPOINTS

```
GET    /api/v1/production-lines

GET    /api/v1/production-lines/{id}

POST   /api/v1/production-lines

PUT    /api/v1/production-lines/{id}

POST   /api/v1/production-lines/{id}/approve

POST   /api/v1/production-lines/{id}/activate

POST   /api/v1/production-lines/{id}/deactivate

GET    /api/v1/production-lines/{id}/capacity
```

---

# 18. EVENTS

Publishes

```
ProductionLineCreated

ProductionLineApproved

ProductionLineActivated

ProductionLineDeactivated

ProductionLineCapacityUpdated

ProductionLineWorkCenterAssigned

ProductionLineWorkCenterRemoved
```

---

# 19. PERMISSIONS

```
production.line.read

production.line.create

production.line.update

production.line.approve

production.line.activate

production.line.deactivate
```

---

# 20. USER INTERFACE

The Production Line screen contains

Header

↓

General Information

↓

Assigned Work Centers

↓

Calendar

↓

Capacity Summary

↓

Performance Indicators

↓

Attachments

↓

Audit Timeline

Drag-and-drop ordering of Work Centers is supported.

---

# 21. SEARCH & FILTERS

Support filtering by

- Line Code
- Line Name
- Plant
- Status
- Calendar
- Work Center
- Capacity

---

# 22. AUDIT

Every modification records

- User
- Timestamp
- Previous Value
- New Value
- Changed Fields
- Approval Action

Audit records are immutable.

---

# 23. CROSS MODULE INTEGRATION

Planning

Aggregates capacity by Production Line.

Production

Displays operational progress by Production Line.

Dashboard

Provides real-time monitoring and KPIs.

Maintenance

Coordinates shutdown planning by Production Line.

Analytics

Calculates

- OEE by Line
- Throughput
- Capacity Utilization
- Bottleneck Analysis
- Downtime by Line

---

# 24. REPORTING

Production Line reporting supports

- Capacity Analysis
- Production Summary
- Machine Utilization
- Labor Utilization
- Downtime Analysis
- Scrap Analysis
- OEE Trends

Historical reporting is based on transactional data.

---

# 25. SUCCESS CRITERIA

The Production Line module is successful when

- Manufacturing resources are logically organized.
- Capacity is accurately aggregated.
- Operational visibility is improved.
- Dashboards reflect real-time line performance.
- Planning identifies bottlenecks correctly.
- Work Centers remain the primary planning resource.

---

# 26. FINAL DESIGN STATEMENT

The Production Line is the canonical organizational structure for manufacturing
operations within the Naswood Operating System.

It groups Work Centers into logical manufacturing flows, enabling operational
visibility, performance monitoring and capacity analysis while remaining
independent from Routing, Production Execution and Machine ownership.

Production Lines provide the strategic view of manufacturing, while Work Centers
plan execution and Machines perform the physical work.
