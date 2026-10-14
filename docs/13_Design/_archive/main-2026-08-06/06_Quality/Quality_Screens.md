# ==============================================================================
# QUALITY SCREENS
# Naswood Operating System (NOS)
# Module: Quality
# Version: 2.0
# ==============================================================================

# PURPOSE

This document defines all operational and engineering screens of the Quality
module.

Quality screens are organized by Workspace.

Each screen supports a complete quality business process.

Quality screens are process-oriented rather than CRUD-oriented.

Generic Create / Edit / Delete pages are prohibited.

Quality operations are executed through dedicated Workbenches, Wizards,
Inspection Consoles and Decision Workflows.

---

# SCREEN DESIGN PRINCIPLES

Quality screens shall

- follow real quality processes
- minimize manual typing
- maximize barcode and QR usage
- support guided inspections
- support complete traceability
- provide contextual decisions
- integrate seamlessly with Inventory and Production

Quality Engineering screens shall use dedicated configuration workspaces.

Quality Execution screens shall use guided operational workflows.

---

# SCREEN HIERARCHY

```text
Quality

├── Dashboard

├── Quality Planning

├── Incoming Quality

├── In-Process Quality

├── Final Inspection

├── Non-Conformance

├── CAPA

├── Traceability

├── Certificates

├── Analytics

└── Reports
```

---

# DASHBOARD WORKSPACE

## QLT-001 Quality Dashboard

Purpose

Real-time quality overview.

Primary Users

- Quality Manager
- Plant Manager

Widgets

- Inspection Status
- Open NCR
- CAPA Status
- FPY
- PPM
- Supplier Performance
- Customer Complaints
- Quality Alerts

Primary Actions

- Start Inspection
- Open NCR
- Review CAPA
- View Traceability

---

# QUALITY PLANNING WORKSPACE

## QLT-101 Inspection Plan Designer

Purpose

Design and maintain inspection plans.

This is an Engineering Workspace.

It is not a Create Form.

Functions

- Product Selection
- Revision Selection
- Characteristics
- Sampling Plan
- Test Method
- Acceptance Rules
- Revision Management
- Approval
- Release

System identifiers are generated automatically.

---

## QLT-102 Control Plan Designer

Purpose

Configure production quality controls.

Functions

- Process Selection
- Control Characteristics
- Inspection Frequency
- Control Method
- Reaction Plan
- Approval

---

## QLT-103 Inspection Characteristic Designer

Purpose

Manage measurable quality characteristics.

Functions

- Characteristic Definition
- Target Value
- Tolerance
- Measuring Device
- SPC Rules

---

## QLT-104 Sampling Plan Manager

Purpose

Configure sampling strategies.

---

## QLT-105 Test Method Library

Purpose

Maintain laboratory and production test procedures.

---

# INCOMING QUALITY WORKSPACE

## QLT-201 Incoming Inspection Wizard

Purpose

Execute supplier material inspections.

Workflow

```text
Purchase Receipt

↓

Material

↓

Lot

↓

Inspection Plan

↓

Measurements

↓

Decision

↓

Accept

or

Reject

or

Quarantine
```

Actions

- Scan Barcode
- Capture Photos
- Record Measurements
- Generate NCR

---

## QLT-202 Material Acceptance Console

Purpose

Release accepted materials.

---

## QLT-203 Material Rejection Console

Purpose

Reject supplier materials.

---

## QLT-204 Quarantine Workbench

Purpose

Manage quarantined inventory.

Functions

- Hold Material
- Release
- Scrap
- Return Supplier
- Rework

---

# IN-PROCESS QUALITY WORKSPACE

## QLT-301 Process Inspection Console

Purpose

Execute production inspections.

Workflow

```text
Production Order

↓

Operation

↓

Measurement

↓

Tolerance Check

↓

Decision
```

---

## QLT-302 SPC Monitoring

Purpose

Monitor statistical process control.

Displays

- Control Charts
- Cp
- Cpk
- Trends
- Alerts

---

## QLT-303 Process Measurement Console

Purpose

Capture production measurements.

Supports

- Manual Entry
- Digital Gauges
- IoT Devices

---

## QLT-304 Quality Alert Center

Purpose

Monitor active quality issues.

---

# FINAL INSPECTION WORKSPACE

## QLT-401 Final Inspection Wizard

Purpose

Inspect finished products.

Workflow

```text
Finished Product

↓

Inspection Plan

↓

Measurements

↓

Visual Inspection

↓

Functional Test

↓

Decision

↓

Release

or

Reject
```

---

## QLT-402 Product Release Console

Purpose

Approve products for inventory or shipment.

---

## QLT-403 Functional Test Console

Purpose

Perform product function tests.

---

## QLT-404 Visual Inspection Console

Purpose

Perform visual quality inspections.

---

# NON-CONFORMANCE WORKSPACE

## QLT-501 NCR Workbench

Purpose

Manage non-conformance cases.

This is a Workbench.

It is not a Create Form.

Workflow

```text
Issue Detection

↓

Evidence

↓

Containment

↓

Disposition

↓

Root Cause

↓

CAPA

↓

Closure
```

---

## QLT-502 Material Hold Console

Purpose

Isolate non-conforming material.

---

## QLT-503 Disposition Manager

Purpose

Manage disposition decisions.

Supports

- Use As Is
- Rework
- Scrap
- Return Supplier

---

# CAPA WORKSPACE

## QLT-601 CAPA Workbench

Purpose

Manage corrective and preventive actions.

Functions

- Root Cause
- Corrective Actions
- Preventive Actions
- Responsible Persons
- Due Dates
- Verification
- Effectiveness
- Closure

---

## QLT-602 Root Cause Analysis

Purpose

Perform structured root cause analysis.

Supports

- 5 Why
- Fishbone
- Fault Tree

---

## QLT-603 Effectiveness Verification

Purpose

Verify CAPA effectiveness.

---

# TRACEABILITY WORKSPACE

## QLT-701 Traceability Explorer

Purpose

Explore complete product genealogy.

Supports

- Material
- Lot
- Serial
- Production
- Inspection
- Shipment

---

## QLT-702 Lot Traceability

Purpose

Track complete lot lifecycle.

---

## QLT-703 Serial Traceability

Purpose

Track serialized products.

---

## QLT-704 Genealogy Explorer

Purpose

Display parent-child relationships.

---

# CERTIFICATES WORKSPACE

## QLT-801 Certificate Generator

Purpose

Generate quality certificates.

Functions

- Certificate Selection
- Product
- Lot
- Customer
- Approval
- Export

---

## QLT-802 Test Report Generator

Purpose

Generate laboratory reports.

---

## QLT-803 Compliance Document Center

Purpose

Manage compliance documentation.

---

# ANALYTICS WORKSPACE

## QLT-901 FPY Dashboard

## QLT-902 PPM Dashboard

## QLT-903 Pareto Analysis

## QLT-904 SPC Dashboard

## QLT-905 Supplier Performance

## QLT-906 Customer Complaint Analysis

## QLT-907 Cost of Quality

---

# REPORTS WORKSPACE

## QLT-1001 Inspection Reports

## QLT-1002 NCR Reports

## QLT-1003 CAPA Reports

## QLT-1004 Supplier Reports

## QLT-1005 Audit Reports

## QLT-1006 Compliance Reports

## QLT-1007 Traceability Reports

---

# COMMON SCREEN COMPONENTS

Quality screens may use

- Dashboard Cards
- KPI Cards
- Inspection Wizard
- Inspection Console
- Workbench
- Decision Panel
- Measurement Grid
- SPC Charts
- Traceability Explorer
- Timeline
- Attachment Panel
- Audit Timeline
- Barcode Scanner
- QR Scanner

---

# SCREEN RELATIONSHIPS

```text
Inspection Plan

↓

Incoming Inspection

↓

Process Inspection

↓

Final Inspection

↓

Release

or

NCR

↓

CAPA

↓

Verification

↓

Closure
```

---

# DESIGN RULES

- Every screen belongs to one Workspace.
- Engineering screens are Designers, not Create Forms.
- Operational screens are Wizards, Consoles or Workbenches.
- Generic CRUD pages are prohibited.
- Manual identifier entry is prohibited.
- Inspection, NCR and CAPA numbers are generated automatically according to the centralized Numbering Architecture.
- Every quality decision must be auditable.
- Every screen supports complete traceability.

---

# IMPLEMENTATION RULES

Frontend implementation shall

- generate Workspaces before Screens
- generate Screens before Components
- optimize inspector workflows
- minimize manual typing
- support barcode and QR workflows
- reuse common UI components
- support responsive layouts
- preserve complete audit history
- preserve genealogy and traceability

Quality Screens shall be generated from Module, Workspace and User Flow definitions rather than from entities or implementation tasks.
