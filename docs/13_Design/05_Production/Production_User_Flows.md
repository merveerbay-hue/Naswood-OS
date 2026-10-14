# ==============================================================================
# PRODUCTION USER FLOWS
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.0
# ==============================================================================

# PURPOSE

This document defines the end-to-end business user flows within the Production
module.

User Flows describe how different production roles interact with the system to
complete business processes.

User Flows are independent from UI implementation and backend technology.

All Production screens, APIs and workflows shall conform to these user flows.

---

# DESIGN PRINCIPLES

Production User Flows shall

- follow real manufacturing processes
- minimize user interaction
- eliminate unnecessary navigation
- support role-based experiences
- provide contextual actions
- support desktop, tablet and mobile
- integrate seamlessly with Inventory, Quality and Maintenance

User flows always start from a Workspace.

Users never navigate through database entities.

---

# AUTHORITY REFERENCES (do not redefine)

This document owns **role journeys / flow steps**. Cross-cutting laws live elsewhere.
See `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`.

| Topic | Authority — reference only |
|-------|----------------------------|
| Numbering (Material, Lot, Serial, Package, Pallet, Production IDs) | `docs/13_Design/99_Shared/Document_Numbering.md` |
| Genealogy | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Inventory ownership | `docs/13_Design/02_Inventory/Inventory_Architecture.md` |
| Execution process truth | `Production_Workflow.md` |

When a flow step needs a new Lot / Serial / Package / Pallet / Production ID,
write: *“Identifier minted by NOS Numbering Service (Document_Numbering.md)”* —
never “system generates lot” as a local rule.

---

# PRIMARY USER ROLES

- Production Manager
- Production Planner
- Production Supervisor
- Manufacturing Engineer
- Machine Operator
- Warehouse Operator
- Quality Inspector
- Maintenance Technician

---

# FLOW-001
# Production Planning

Primary Role

Production Planner

Goal

**Finish a releasable production plan** (not “create a Production Order row”).

Primary screen

Production Planning Wizard  
(`docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`)

```text
Dashboard
    ↓
Planning Workspace
    ↓
Plan production  →  Production Planning Wizard
    ↓
1  Ürün seçimi
2  Revizyon seçimi
3  Ölçü seçimi
4  Teknik paket (detay çizim · kesit · ekler)  → File Upload / DMS
5  Ağaç türü seçimi
6  Hammadde uygunluğu
7  Hat seçimi
8  Kapasite kontrolü
9  Termin planı
10 Maliyet simülasyonu
11 Onay · Release · isteğe bağlı Saha paketi yazdır / PDF
    ↓
Released Production Order
  (+ optional WO package / reservations)
  (+ shop packet: ölçü + çizim + kesit çıktısı)
```

Secondary

- Plan / Order Library — find & reopen drafts  
- Scheduling / Capacity boards — multi-order balancing  

Integrations

- Product (catalog, revision, dimensions, species)
- Inventory (availability, reservation)
- Finance (cost simulation)
- Maintenance (downtime on capacity)
- Scheduling

---

# FLOW-002
# Work Order Execution

Primary Role

Production Supervisor

Goal

Execute Production Orders.

```text
Dashboard

↓

Execution Workspace

↓

Released Production Orders

↓

Generate Work Orders

↓

Assign Work Center

↓

Assign Machine

↓

Assign Operator

↓

Start Production
```

Integrations

- Machines
- Work Centers
- Shifts

---

# FLOW-003
# Shop Floor Production

Primary Role

Machine Operator

Goal

Execute production on the shop floor.

```text
Operator Terminal

↓

Login

↓

Assigned Work Orders

↓

Select Work Order

↓

Start Operation

↓

Scan Material

↓

Confirm Material

↓

Run Production

↓

Pause (optional)

↓

Resume

↓

Complete Operation

↓

Production Confirmation
```

Integrations

- Barcode
- QR
- Inventory
- Machine

---

# FLOW-004
# Material Consumption

Primary Role

Machine Operator

Goal

Consume production materials.

```text
Operator Terminal

↓

Material Consumption

↓

Scan Lot

↓

Validate Material

↓

Consume Material

↓

Inventory Posting

↓

Inventory Updated
```

Integrations

- Inventory
- Lot
- Warehouse

---

# FLOW-005
# Production Confirmation

Primary Role

Supervisor

Goal

Confirm production output.

```text
Execution Workspace

↓

Production Confirmation

↓

Good Quantity

↓

Scrap Quantity

↓

Rework Quantity

↓

Confirm

↓

Finished Goods Created
```

Integrations

- Inventory
- Quality

---

# FLOW-006
# WIP Tracking

Primary Role

Production Supervisor

Goal

Monitor work-in-progress.

```text
Dashboard

↓

Execution Workspace

↓

WIP

↓

Current Operation

↓

Waiting Queue

↓

Running Orders

↓

Completed Operations
```

---

# FLOW-007
# Packaging

Primary Role

Packaging Operator

Goal

Package production output.

```text
Packaging Workspace

↓

Select Finished Goods

↓

Create Package

↓

Assign Pallet

↓

Generate Labels

↓

Print Labels

↓

Complete Packaging
```

Integrations

- Inventory
- Logistics

---

# FLOW-008
# Finished Goods Posting

Primary Role

Warehouse Operator

Goal

Receive production output into inventory.

```text
Packaging Completed

↓

Finished Goods

↓

Assign Warehouse

↓

Assign Location

↓

Mint Lot ID via NOS Numbering Service
(see Document_Numbering.md — manual entry prohibited)

↓

Inventory Receipt

↓

Available Stock
```

Integrations

- Inventory
- Warehouse

---

# FLOW-009
# Production Quality

Primary Role

Quality Inspector

Goal

Perform production inspections.

```text
Production Confirmation

↓

Inspection Required

↓

In-Process Inspection

↓

Pass

↓

Continue Production
```

Alternative

```text
Inspection Failed

↓

NCR

↓

CAPA

↓

Rework

↓

Production Resume
```

Integrations

- Quality

---

# FLOW-010
# Machine Breakdown

Primary Role

Operator

Goal

Report machine failure.

```text
Operator Terminal

↓

Machine Alarm

↓

Stop Production

↓

Report Downtime

↓

Maintenance Request

↓

Maintenance Assigned

↓

Repair Completed

↓

Restart Production
```

Integrations

- Maintenance
- OEE

---

# FLOW-011
# Shift Change

Primary Role

Supervisor

Goal

Transfer production responsibility.

```text
Shift End

↓

Production Summary

↓

Pending Work Orders

↓

Downtime Summary

↓

Quality Summary

↓

Operator Handover

↓

Next Shift Starts
```

---

# FLOW-012
# Production Monitoring

Primary Role

Production Manager

Goal

Monitor production performance.

```text
Dashboard

↓

Machine Status

↓

Active Orders

↓

OEE

↓

Capacity

↓

Alerts

↓

Production Timeline
```

---

# FLOW-013
# Production Analytics

Primary Role

Plant Manager

Goal

Analyze production performance.

```text
Analytics Workspace

↓

OEE

↓

Capacity

↓

Yield

↓

Scrap

↓

Rework

↓

Productivity

↓

Loss Analysis
```

---

# FLOW-014
# Production Reporting

Primary Role

Management

Goal

Generate reports.

```text
Reports Workspace

↓

Select Report

↓

Apply Filters

↓

Preview

↓

Export

↓

PDF

Excel

CSV
```

---

# CROSS MODULE FLOWS

## Inventory Integration

```text
Production

↓

Material Consumption

↓

Inventory Transaction

↓

Inventory Balance Updated
```

---

## Quality Integration

```text
Production

↓

Inspection

↓

Quality Decision

↓

Continue

or

NCR
```

---

## Maintenance Integration

```text
Production

↓

Machine Failure

↓

Maintenance Request

↓

Repair

↓

Restart
```

---

## Planning Integration

```text
Sales Demand

↓

MRP

↓

Production Planning

↓

Production Order
```

---

# MOBILE FLOWS

Operator

```text
Login

↓

Assigned Work

↓

Scan

↓

Confirm

↓

Report Issue
```

Supervisor

```text
Dashboard

↓

Alerts

↓

Approve

↓

Monitor
```

---

# EXCEPTION FLOWS

Supports

- Machine Failure
- Material Shortage
- Missing Operator
- Quality Hold
- Production Delay
- Shift Change
- Emergency Stop
- Power Failure
- Inventory Shortage
- Maintenance Hold

Every exception flow shall provide guided recovery actions.

---

# DESIGN RULES

- Every flow begins in a Workspace.
- Navigation follows business processes.
- Wizards are used for complex operations.
- Operators focus on execution, not administration.
- Managers focus on dashboards and analytics.
- Cross-module transitions shall be seamless.
- Every flow supports audit and traceability.

---

# IMPLEMENTATION RULES

Frontend implementation shall

- implement Workspaces before Screens
- implement Screens before Components
- preserve workflow continuity
- minimize navigation depth
- support contextual actions
- support mobile execution
- support barcode and QR scanning
- support offline operation where applicable

User Flows are the primary reference for frontend behavior and interaction design.

Implementation Tasks shall implement these flows, never redefine them.
