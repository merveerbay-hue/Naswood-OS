# ==============================================================================
# PRODUCTION NAVIGATION
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.0
# ==============================================================================

# PURPOSE

This document defines the navigation architecture of the Production module.

Navigation is task-oriented rather than entity-oriented.

Users navigate through business processes, not CRUD pages.

Every navigation item belongs to a Workspace.

Navigation must be role-aware and permission-driven.

---

# DESIGN PRINCIPLES

Production navigation shall:

- Follow manufacturing workflows
- Minimize user clicks
- Separate Planning from Execution
- Support desktop and mobile
- Provide contextual actions
- Be role-based
- Be responsive
- Support breadcrumbs
- Support favorites
- Support global search

Navigation must never expose database entities directly.

---

# NAVIGATION HIERARCHY

```text
Production

├── Dashboard

├── Planning

│   ├── Production Orders
│   ├── Work Orders
│   ├── Scheduling
│   ├── Capacity Planning
│   ├── Material Availability
│   └── Dispatch Board

├── Execution

│   ├── Material Consumption
│   ├── Production Confirmation
│   ├── WIP
│   ├── Packaging
│   ├── Finished Goods
│   ├── Scrap
│   └── Rework

├── Shop Floor

│   ├── Operator Terminal
│   ├── Machine Terminal
│   ├── Barcode Scanner
│   ├── QR Scanner
│   ├── Downtime
│   └── Live Production

├── Monitoring

│   ├── Machine Status
│   ├── Work Center Status
│   ├── Production Timeline
│   ├── Active Orders
│   └── Alerts

├── Master Data

│   ├── BOM
│   ├── Routing
│   ├── Machines
│   ├── Work Centers
│   ├── Production Lines
│   ├── Operations
│   ├── Shifts
│   ├── Calendars
│   └── Tooling

├── Analytics

│   ├── OEE
│   ├── Productivity
│   ├── Capacity
│   ├── Yield
│   ├── Efficiency
│   └── Loss Analysis

└── Reports

    ├── Production Reports
    ├── Shift Reports
    ├── Machine Reports
    ├── Cost Reports
    └── KPI Reports
```

---

# NAVIGATION LEVELS

Level 1

Module

Example

```
Production
```

---

Level 2

Workspace

Example

```
Planning
```

---

Level 3

Business Function

Example

```
Production Orders
```

---

Level 4

Contextual Screen

Example

```
Production Order Detail

↓

Scheduling

↓

Material Availability

↓

History
```

---

# USER ROLE NAVIGATION

## Production Manager

Landing Page

```
Production Dashboard
```

Primary Navigation

```
Dashboard

Planning

Monitoring

Analytics

Reports
```

---

## Production Planner

Landing Page

```
Planning Workspace
```

Primary Navigation

```
Production Orders

Scheduling

Capacity Planning

Dispatch Board
```

---

## Production Supervisor

Landing Page

```
Execution Workspace
```

Primary Navigation

```
Work Orders

Material Consumption

Production Confirmation

WIP

Monitoring
```

---

## Machine Operator

Landing Page

```
Shop Floor Terminal
```

Primary Navigation

```
Assigned Work Orders

Machine Status

Production Start

Production Stop

Downtime

Quality Check
```

---

## Manufacturing Engineer

Landing Page

```
Master Data
```

Primary Navigation

```
BOM

Routing

Operations

Machines

Tooling
```

---

# BREADCRUMB EXAMPLES

Production Order

```
Production

>

Planning

>

Production Orders

>

PO-240001
```

---

Machine

```
Production

>

Master Data

>

Machines

>

CNC-01
```

---

Material Consumption

```
Production

>

Execution

>

Material Consumption

>

WO-240045
```

---

# CONTEXTUAL ACTIONS

Production Orders

```
Create

Schedule

Release

Copy

Cancel

Archive
```

---

Work Orders

```
Start

Pause

Resume

Complete

Close
```

---

Machines

```
View Status

Maintenance

History

OEE

Documents
```

---

BOM

```
Create Revision

Compare

Approve

Export
```

---

# QUICK ACCESS

Favorites

Recent Items

Pinned Work Orders

Pinned Machines

Pinned Production Orders

Pinned Reports

---

# GLOBAL SEARCH

Supports

```
Production Order

Work Order

Machine

Work Center

Production Line

BOM

Routing

Operation

Barcode

Lot

Serial
```

Global Search returns

- Entity
- Current Status
- Related Workspace
- Available Actions

---

# MOBILE NAVIGATION

Bottom Navigation

```
Home

Tasks

Scanner

Alerts

Profile
```

Quick Actions

```
Start Production

Stop Production

Scan Barcode

Confirm Production

Report Downtime
```

---

# NOTIFICATIONS

Display

```
Released Work Orders

Production Delays

Machine Failures

Material Shortages

Downtime Alerts

Quality Holds
```

Notifications provide direct navigation to the related workspace.

---

# DESIGN RULES

Navigation is process-driven.

Navigation is workspace-based.

Navigation is permission-aware.

Navigation must never expose CRUD menus.

Entity management is always accessed through a business workspace.

All pages must support breadcrumb navigation.

All detail pages must provide contextual actions.

Users must reach any operational function within three navigation levels.

---

# IMPLEMENTATION RULES

Frontend implementation shall:

- Build the menu from Workspace definitions.
- Apply role-based visibility.
- Apply permission filtering.
- Support responsive navigation.
- Support deep linking.
- Preserve navigation state.
- Support keyboard shortcuts.
- Support favorites and recent items.

Navigation must be generated from the Production module definition and Workspace architecture rather than from individual entities or implementation tasks.
