# ==============================================================================
# PRODUCTION SCREENS
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.0
# ==============================================================================

# PURPOSE

This document defines every Production screen within the Production module.

Screens are organized by Workspace.

A Screen represents a complete business function.

Screens are never generated directly from database entities.

Every screen belongs to exactly one Workspace.

---

# SCREEN DESIGN PRINCIPLES

**Job-first (mandatory):**  
Before any screen: *Kullanıcı bu ekranda hangi işi bitirmek istiyor?*  
Name the screen after that job — not after the entity.  
See `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`.

Production screens shall

- Follow manufacturing workflows (Wizard / Board / Terminal)
- Be role-oriented
- Be process-driven (steps, gates, release)
- Support desktop and tablet
- Minimize user interaction
- Display contextual KPIs
- Provide contextual actions
- Support drill-down navigation

Screens must never behave as generic CRUD pages.

```text
Primary Planning entry:  Production Planning Wizard
Secondary:               Plan / Order Library (find & reopen)
Not the design center:   “New Production Order” entity form
```

---

# SCREEN HIERARCHY

```text
Production

├── Dashboard
│
├── Planning
│
├── Execution
│
├── Shop Floor
│
├── Monitoring
│
├── Master Data
│
├── Analytics
│
└── Reports
```

---

# DASHBOARD WORKSPACE

## PRD-001 Production Dashboard

Purpose

Real-time production overview.

Primary Users

- Plant Manager
- Production Manager

Widgets

- Production KPIs
- OEE
- Active Orders
- Shift Summary
- Capacity
- Alerts
- Machine Status
- Production Timeline

Primary Actions

- Open Planning
- Open Monitoring
- Open Active Orders

---

# PLANNING WORKSPACE

## PRD-101 Production Planning Wizard  ★ primary

**Job to be done:** Planlamacı üretilebilir bir planı kurar ve Release eder.

Full spec: `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`

Steps

1. Ürün seçimi  
2. Revizyon seçimi  
3. Ölçü seçimi  
4. Ağaç türü seçimi  
5. Hammadde uygunluğu  
6. Hat seçimi  
7. Kapasite kontrolü  
8. Termin planı  
9. Maliyet simülasyonu  
10. Onay ve Release  

Primary Users

- Production Planner  
- Production Manager (approve / release)

Outcome

- Draft plan saved, or Production Order **Released**

Components

- Wizard · Availability Panel · Capacity chart · Approval Bar

---

## PRD-102 Plan / Order Library

**Job to be done:** Draft / released planları bul, aç, izle (wizard’ın ikincil yüzeyi).

Features

- Plan list (Draft · PendingApproval · Released · …)
- Filters · Search · Bulk actions

Actions

- **Plan production** → opens PRD-101 Wizard  
- Open in Wizard (draft)  
- Open Detail (released)  
- Cancel · Archive  

---

## PRD-102b Production Order Detail

**Job to be done:** Release sonrası tek planı izle / belgele (oluşturma yolu değil).

Sections

- Overview · Materials · Routing · Schedule · Capacity · History · Documents

Actions

- Reschedule (policy) · Print · Duplicate → Wizard · Cancel

---

## PRD-103 Scheduling Board

**Job to be done:** Çoklu emirleri zaman/kaynak ekseninde dengele; çatışmaları çöz.

Display

- Timeline · Machines · Capacity · Work Orders · Conflicts

Supports Drag & Drop.

---

## PRD-104 Capacity Load Board

**Job to be done:** Hat / WC yükünü gör; darboğazı bul; plana geri dön.

Display

- Available vs Planned · Bottlenecks · Utilization

---

## PRD-105 Dispatch Board

**Job to be done:** Sahaya verilecek işi önceliklendir ve sevk et.

Display

- Ready · Running · Delayed · Priorities

---

# EXECUTION WORKSPACE

## PRD-201 Work Orders

Purpose

Manage production execution.

Display

- Work Orders
- Status
- Machine
- Operator

Actions

- Start
- Pause
- Resume
- Complete

---

## PRD-202 Material Consumption

Purpose

Consume production materials.

Display

- BOM
- Required
- Consumed
- Remaining

Actions

- Scan Barcode
- Scan Lot
- Post Consumption

---

## PRD-203 Production Confirmation

Purpose

Confirm production output.

Display

- Good Quantity
- Scrap
- Rework
- Production Time

Actions

- Confirm
- Save Draft

---

## PRD-204 WIP Tracking

Purpose

Track Work In Progress.

Display

- Current Operation
- Waiting
- Running
- Completed

---

## PRD-205 Packaging

Purpose

Packaging operations.

Display

- Package Builder
- Labels
- Pallets
- Packages

---

## PRD-206 Finished Goods

Purpose

Production output posting.

Display

- Finished Goods
- Lot
- Serial
- Warehouse

Actions

- Post Output
- Print Label

---

## PRD-207 Scrap & Rework

Purpose

Manage production losses.

Display

- Scrap Reasons
- Rework Orders
- Cost Impact

---

# SHOP FLOOR WORKSPACE

## PRD-301 Operator Terminal

Purpose

Primary operator interface.

Display

- Assigned Work Orders
- Machine Status
- Current Operation

Actions

- Start
- Stop
- Pause
- Confirm
- Request Maintenance

Touch optimized.

---

## PRD-302 Machine Terminal

Purpose

Machine-centric interface.

Display

- Machine KPIs
- OEE
- Status
- Active Operator

---

## PRD-303 Barcode Scanner

Purpose

Material and production scanning.

Supports

- Material
- Lot
- Serial
- Package

---

## PRD-304 QR Scanner

Purpose

QR-based production operations.

---

# MONITORING WORKSPACE

## PRD-401 Live Production

Purpose

Real-time production monitoring.

Display

- Running Orders
- Machine Status
- Production Counters

Auto Refresh

---

## PRD-402 Machine Status

Display

- Running
- Idle
- Setup
- Breakdown

---

## PRD-403 Work Center Status

Display

- Utilization
- Queue
- Capacity
- Availability

---

## PRD-404 Production Timeline

Display

- Timeline
- Events
- Downtime
- Shift Changes

---

## PRD-405 Alerts

Display

- Production Delay
- Downtime
- Material Shortage
- Quality Hold

---

# MASTER DATA WORKSPACE

## PRD-501 BOM Management

Purpose

Manage BOM revisions.

---

## PRD-502 Routing Management

Purpose

Manage Routings.

---

## PRD-503 Machine Management

Purpose

Manage production machines.

---

## PRD-504 Work Center Management

Purpose

Manage Work Centers.

---

## PRD-505 Production Lines

Purpose

Manage Production Lines.

---

## PRD-506 Operations

Purpose

Manage Operations.

---

## PRD-507 Shifts

Purpose

Manage Shifts.

---

## PRD-508 Calendars

Purpose

Manage Production Calendars.

---

## PRD-509 Tooling

Purpose

Manage production tooling.

---

# ANALYTICS WORKSPACE

## PRD-601 OEE Analytics

Display

- Availability
- Performance
- Quality
- OEE

---

## PRD-602 Productivity Analysis

Display

- Output
- Utilization
- Labor

---

## PRD-603 Capacity Analysis

Display

- Planned
- Actual
- Lost Capacity

---

## PRD-604 Loss Analysis

Display

- Downtime
- Scrap
- Rework
- Waiting

---

# REPORTS WORKSPACE

## PRD-701 Production Reports

## PRD-702 Shift Reports

## PRD-703 Machine Reports

## PRD-704 KPI Reports

## PRD-705 Cost Reports

## PRD-706 WIP Reports

---

# COMMON SCREEN COMPONENTS

Every Production screen may use

- Dashboard Cards
- KPI Cards
- Entity Grid
- Timeline
- Tree View
- Kanban
- Scheduler
- Wizard
- Split View
- Charts
- Document Panel
- Attachment Panel
- Audit Timeline

---

# SCREEN RELATIONSHIPS

```text
Dashboard
      │
      ▼
Planning
      │
      ▼
Production Orders
      │
      ▼
Work Orders
      │
      ▼
Execution
      │
      ▼
Packaging
      │
      ▼
Finished Goods
      │
      ▼
Reports
```

---

# DESIGN RULES

- Every screen belongs to one Workspace.
- Every screen has a clear business purpose.
- Screens are workflow-oriented.
- Navigation follows business processes.
- CRUD-only pages are prohibited.
- Wizards are preferred for complex transactions.
- Contextual actions replace generic action bars.
- Dashboards provide entry points into operational workflows.

---

# IMPLEMENTATION RULES

Frontend generation shall:

- Generate Workspaces first.
- Generate Screens inside Workspaces.
- Reuse Component Library.
- Apply role-based visibility.
- Support responsive layouts.
- Support keyboard shortcuts.
- Support deep links.
- Preserve navigation state.

Production screens shall be generated from Module, Workspace and Workflow definitions, never directly from entities or implementation tasks.
