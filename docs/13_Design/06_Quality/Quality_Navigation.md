# ==============================================================================
# QUALITY NAVIGATION
# Naswood Operating System (NOS)
# Module: Quality
# Version: 2.0
# ==============================================================================

# PURPOSE

This document defines the navigation architecture of the Quality module.

Quality Navigation is process-oriented rather than entity-oriented.

Users navigate through quality assurance workflows instead of database records.

Every navigation item belongs to a Quality Workspace.

Navigation is role-aware, permission-driven and fully integrated with
Production, Inventory, Purchasing and Sales.

---

# DESIGN PRINCIPLES

Quality Navigation shall

- follow real quality assurance processes
- separate planning from execution
- support laboratory, production and warehouse users
- minimize navigation depth
- provide contextual actions
- support desktop and tablet devices
- support barcode and QR workflows
- provide complete traceability
- support audit navigation

Navigation must never expose generic CRUD menus.

---

# NAVIGATION HIERARCHY

```text
Quality

├── Dashboard

├── Quality Planning

│   ├── Inspection Plans
│   ├── Control Plans
│   ├── Inspection Characteristics
│   ├── Sampling Plans
│   ├── Test Methods
│   └── Acceptance Criteria

├── Incoming Quality

│   ├── Purchase Inspection
│   ├── Incoming Inspection
│   ├── Material Acceptance
│   ├── Material Rejection
│   └── Quarantine

├── In-Process Quality

│   ├── Process Inspection
│   ├── Operator Inspection
│   ├── SPC Monitoring
│   ├── Process Measurements
│   └── Quality Alerts

├── Final Inspection

│   ├── Product Inspection
│   ├── Functional Tests
│   ├── Visual Inspection
│   ├── Dimensional Inspection
│   └── Release Decision

├── Non-Conformance

│   ├── Open NCR
│   ├── Material Hold
│   ├── Containment
│   ├── Disposition
│   └── Root Cause

├── CAPA

│   ├── Corrective Actions
│   ├── Preventive Actions
│   ├── Verification
│   ├── Effectiveness Review
│   └── Closure

├── Traceability

│   ├── Material Traceability
│   ├── Lot Traceability
│   ├── Serial Traceability
│   ├── Genealogy
│   └── Product History

├── Certificates

│   ├── Quality Certificates
│   ├── Test Reports
│   ├── Material Certificates
│   ├── Compliance Documents
│   └── Customer Certificates

├── Analytics

│   ├── FPY
│   ├── PPM
│   ├── Pareto
│   ├── SPC
│   ├── Supplier Quality
│   ├── Customer Complaints
│   └── Cost of Quality

└── Reports

    ├── Inspection Reports
    ├── NCR Reports
    ├── CAPA Reports
    ├── Supplier Reports
    ├── Audit Reports
    └── Compliance Reports
```

---

# NAVIGATION LEVELS

## Level 1

Module

```text
Quality
```

---

## Level 2

Workspace

Example

```text
Incoming Quality
```

---

## Level 3

Business Function

Example

```text
Incoming Inspection
```

---

## Level 4

Contextual Screen

Example

```text
Quality

>

Incoming Quality

>

Incoming Inspection

>

Inspection Result
```

---

# USER ROLE NAVIGATION

## Quality Manager

Landing Page

```text
Quality Dashboard
```

Primary Navigation

```text
Dashboard

Analytics

CAPA

Non-Conformance

Reports
```

---

## Quality Engineer

Landing Page

```text
Quality Planning
```

Primary Navigation

```text
Inspection Plans

Control Plans

Inspection Characteristics

CAPA

Traceability
```

---

## Incoming Inspector

Landing Page

```text
Incoming Quality
```

Primary Navigation

```text
Incoming Inspection

Material Acceptance

Material Rejection

Quarantine
```

---

## Process Quality Engineer

Landing Page

```text
In-Process Quality
```

Primary Navigation

```text
Process Inspection

SPC

Measurements

Quality Alerts
```

---

## Final Inspector

Landing Page

```text
Final Inspection
```

Primary Navigation

```text
Final Inspection

Functional Tests

Visual Inspection

Release Decision
```

---

## Auditor

Landing Page

```text
Reports
```

Primary Navigation

```text
Audit Reports

Compliance Reports

Certificates

Traceability
```

---

# BREADCRUMB EXAMPLES

Incoming Inspection

```text
Quality

>

Incoming Quality

>

Incoming Inspection

>

Inspection Result
```

---

Final Inspection

```text
Quality

>

Final Inspection

>

Product Inspection

>

Release Decision
```

---

CAPA

```text
Quality

>

CAPA

>

Corrective Actions

>

Verification
```

---

Traceability

```text
Quality

>

Traceability

>

Lot Traceability

>

Material History
```

---

# CONTEXTUAL ACTIONS

Incoming Inspection

```text
Inspect

Accept

Reject

Quarantine

Create NCR
```

---

Process Inspection

```text
Measure

Record

Continue Production

Hold Production
```

---

Final Inspection

```text
Approve

Reject

Release

Hold

Generate Certificate
```

---

NCR

```text
Contain

Assign

Root Cause

Create CAPA

Close
```

---

CAPA

```text
Assign

Approve

Verify

Complete

Close
```

---

Certificates

```text
Generate

Preview

Export

Email

Archive
```

---

# QUICK ACCESS

Favorites

Recent Inspections

Open NCR

Open CAPA

Pending Approvals

Recently Viewed Lots

Pinned Reports

---

# GLOBAL SEARCH

Supports

```text
Inspection

Lot

Serial

Material

Production Order

NCR

CAPA

Certificate

Supplier

Customer
```

Global Search displays

- Current Status
- Related Documents
- Inspection History
- Available Actions

---

# MOBILE NAVIGATION

Bottom Navigation

```text
Home

Tasks

Scanner

Notifications

Profile
```

Quick Actions

```text
Start Inspection

Scan Material

Approve

Reject

Create NCR

View Certificate
```

---

# NOTIFICATIONS

Displays

```text
Pending Inspection

Quality Alert

Inspection Failed

Open NCR

CAPA Due

Supplier Issue

Customer Complaint

Calibration Due
```

Notifications provide direct navigation to the related Quality workflow.

---

# DESIGN RULES

Navigation is quality process-driven.

Navigation is workspace-based.

Navigation is permission-aware.

Navigation must never expose generic CRUD menus.

Quality actions are executed through guided workflows.

Inspection, NCR and CAPA are business processes rather than editable records.

Every quality decision shall remain fully traceable.

---

# IMPLEMENTATION RULES

Frontend implementation shall

- generate navigation from Workspace definitions
- apply role-based visibility
- apply permission filtering
- support responsive layouts
- support barcode and QR workflows
- preserve navigation state
- support deep linking
- provide contextual actions based on quality status

Quality Navigation shall be generated from Module, Workspace and Workflow definitions rather than from entities or implementation tasks.
