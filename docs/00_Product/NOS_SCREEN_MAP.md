# NOS Screen Map

**Document:** NOS Screen Map (Phase 2 — Product Architecture)  
**Status:** Active draft — Product Architect authority  
**Version:** 1.0.0  
**Owner:** Product (Naswood Technology)

**Thinking protocol:** [`AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)  
**Layer map:** [`docs/PRODUCT_LAYERS.md`](../PRODUCT_LAYERS.md)  
**Flat name registry (legacy):** [`docs/04_Application/Screen_Catalog.md`](../04_Application/Screen_Catalog.md)

---

## 1. Purpose

This is the **second major phase** of NOS product architecture:

> Design the full **job / process** map of the ERP/MES — before TASK-driven CRUD.

```text
NOS
  → Module
  → Workspace
  → Job Screen (Wizard / Board / Terminal / Cockpit)
  → Component
  → Workflow
  → Permissions
  → Code
```

**Mandatory questions for every screen:**  
1. *Kullanıcı bu ekranda hangi işi bitirmek istiyor?*  
2. *Bu iş hangi ekran tipini kullanır?* (Wizard / Terminal / Explorer / …)

→ [`JOB_FIRST_SCREEN_DESIGN.md`](./JOB_FIRST_SCREEN_DESIGN.md)  
→ [`docs/13_Design/Common/Screen_Types.md`](../13_Design/Common/Screen_Types.md) · [`UI_Patterns.md`](../13_Design/Common/UI_Patterns.md)  

```text
NOS'ta "New" diye tek tip ekran yoktur.
```

→ Process screens (all job Create CTAs): [`Process_Screens/`](./Process_Screens/) — Planning · Receiving · Issue · Transfer · Count · NCR · Maint WO · PO · SO  
→ Create → Job matrix: [`Screen_Types.md`](../13_Design/Common/Screen_Types.md) § 3b

Entity-titled rows in older indexes are **placeholders to rename** into jobs  
(e.g. “Production Order” → **Production Planning Wizard** + Plan Library).

Cursor implements **named job slices** from this map.  
This document is **not** a TASK backlog.

---

## 2. Top-level application tree

Canonical module navigation (Product Architect decision):

```text
NOS
├── Dashboard
├── Product
├── Sales
├── CRM
├── Purchasing
├── Inventory
├── Production
├── Quality
├── Maintenance
├── Finance
├── HR
├── Administration
└── Settings
```

### Naming decisions (locked for this map)

| Name | Meaning | Not |
|------|---------|-----|
| **Dashboard** | Shell home + executive / plant cockpit (cross-module) | Not “every module’s KPI page” only |
| **Product** | Sellable / catalog product master (SKU, family, specs) | **Not** Production (MES) |
| **Production** | Plan → execute → monitor manufacturing (MES + mfg master) | Not commercial Product |
| **CRM** | Relationship & pipeline (accounts, leads, opportunities) | Separate from Sales order-to-cash |
| **Sales** | Quote → order → ship → invoice | CRM feeds Sales; does not own CRM master |
| **Settings** | Platform + personal preferences | Module-local parameters stay under each module’s Settings workspace |

### Industry extensions (wood factory)

Timber Yard, Kiln, Thermowood are **Production industry workspaces**, not top-level modules — so the primary nav stays ERP-clean while NOS remains timber-native.

Machines & Tooling live primarily under **Production → Master Data** (with Maintenance asset links).

Logistics capabilities sit under **Inventory** (warehouse moves) and **Sales → Fulfillment** (outbound).

AI Copilot is a **shell capability** (header / Dashboard), not a top-level module in v1 of this map.

---

## 3. How to read this map

| Level | What it is | Example |
|-------|------------|---------|
| **Module** | Top nav product area | Production |
| **Workspace** | Job-shaped area inside a module | Planning, Execution |
| **Screen** | One navigable surface (often List/Detail family) | PRD-010 Production Order List |
| **Component** | Shared UI building block | Entity Grid, Kanban, Metric Card |

**Screen ID prefixes**

| Prefix | Module |
|--------|--------|
| HOM | Dashboard / Shell home |
| PDT | Product |
| SAL | Sales |
| CRM | CRM |
| PUR | Purchasing |
| INV | Inventory |
| PRD | Production |
| QLT | Quality |
| MNT | Maintenance |
| FIN | Finance |
| HR | HR |
| ADM | Administration |
| SET | Settings |
| AUTH | Authentication (pre-shell) |

**Maturity**

| Tag | Meaning |
|-----|---------|
| Spec’d | Screen PRD / design pack exists (or strong IA index) |
| Mapped | On this Screen Map; PRD still thin |
| Future | Reserved; do not implement yet |

---

## 4. Shared shell & authentication

### AUTH — Authentication (pre-module)

| ID | Screen | Notes |
|----|--------|-------|
| AUTH-001 | Login | Spec’d (platform) |
| AUTH-002 | Forgot Password | Mapped |
| AUTH-003 | Reset Password | Mapped |
| AUTH-004 | MFA Challenge | Future |
| AUTH-005 | Change Password | Mapped |
| AUTH-006 | Session Expired | Mapped |

### Shell chrome (all modules)

| Surface | Purpose | Components |
|---------|---------|------------|
| App Sidebar | Module → workspace nav | Nav Tree |
| App Header | Company/Plant, search, notifications, AI, user | Global Search, Notification Panel |
| Breadcrumb | Module / Workspace / Screen | Breadcrumb |
| Workspace Shell | Workspace tabs + screen strip | Workspace Chrome |
| Favorites / Recent | Personal shortcuts | List |

---

## 5. Module maps

---

### 5.1 Dashboard (`HOM`)

**Real life:** Plant / exec walks in → one place for “what needs attention.”  
**Roles:** CEO, Plant Manager, Ops Director (role-filtered widgets).  
**ERP ref:** SAP Fiori My Home / D365 workspaces / Infor homepages — role cockpits, not a blank KPI wall.  
**NOS better:** Role lenses + deep links into module workspaces; AI attention strip.

```text
Dashboard
├── Home / My Work
├── Executive Cockpit
├── Plant Overview
├── Alerts & Exceptions
├── My Tasks / Approvals
└── Favorites & Recents
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| HOM-001 | Home / My Work | Home | Mapped |
| HOM-002 | Executive Cockpit | Executive | Mapped |
| HOM-003 | Plant Overview | Plant | Mapped |
| HOM-004 | Alerts & Exceptions | Attention | Mapped |
| HOM-005 | My Tasks / Approvals Inbox | Tasks | Mapped |
| HOM-006 | Notifications Center | Shell | Mapped |
| HOM-007 | Global Search Results | Shell | Mapped |

**Primary components:** Metric Card, Dashboard Card, Alert List, Task Inbox, Chart, AI Attention Strip.

---

### 5.2 Product (`PDT`)

**Real life:** What we sell / stock as finished & semi-finished catalog — families, dimensions, species/grade for timber.  
**Roles:** Product Manager, Sales Engineer, Master Data.  
**ERP ref:** SAP Material Master (FERT) / D365 Product information management — **catalog**, not shop floor.  
**NOS better:** Timber-aware attributes (species, grade, dimension) first-class; clear split from Production BOM.

```text
Product
├── Dashboard
├── Catalog
├── Structure
├── Commercial
├── Master Data
└── Reports
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| PDT-001 | Product Dashboard | Dashboard | Mapped |
| PDT-002 | Product List | Catalog | Mapped |
| PDT-003 | Product Detail | Catalog | Mapped |
| PDT-004 | Product Family List | Catalog | Mapped |
| PDT-005 | Product Family Detail | Catalog | Mapped |
| PDT-006 | Variants / Dimensions | Structure | Mapped |
| PDT-007 | Product Specs / Attributes | Structure | Mapped |
| PDT-008 | Price List Bridge | Commercial | Future |
| PDT-009 | Product ↔ BOM Link | Structure | Mapped |
| PDT-010 | Product Reports | Reports | Mapped |
| PDT-011 | Product Settings | Master Data | Mapped |

**Primary components:** Entity Grid, Master Detail, Attribute Panel, Tree (family), Status Badge.

---

### 5.3 Sales (`SAL`)

**Real life:** Quote to cash — commercial commitment and fulfillment.  
**Roles:** Sales Rep, CSR, Sales Manager.  
**ERP ref:** SAP SD / D365 Sales Orders / IFS Customer Order.  
**NOS better:** Tight Inventory availability + Production ATP signals on order detail.

```text
Sales
├── Dashboard
├── Quotes
├── Orders
├── Fulfillment
├── Master Data
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| SAL-001 | Sales Dashboard | Dashboard | Spec’d |
| SAL-006 | Quotation List | Quotes | Spec’d |
| SAL-007 | Quotation Detail | Quotes | Spec’d |
| SAL-008 | Sales Order List | Orders | Spec’d |
| SAL-009 | Sales Order Detail | Orders | Spec’d |
| SAL-010 | Shipment List | Fulfillment | Spec’d |
| SAL-011 | Shipment Detail | Fulfillment | Spec’d |
| SAL-012 | Delivery | Fulfillment | Spec’d |
| SAL-013 | Customer Invoice List | Fulfillment | Spec’d |
| SAL-014 | Customer Invoice Detail | Fulfillment | Spec’d |
| SAL-015 | Customer List | Master Data | Spec’d |
| SAL-016 | Customer Detail | Master Data | Spec’d |
| SAL-017 | Sales Reports | Reports | Spec’d |
| SAL-018 | Sales Analytics | Analytics | Spec’d |
| SAL-019 | Sales Settings | Settings | Spec’d |

*(SAL-002…005 reserved historically for Lead/Opportunity — those live under CRM in this map.)*

**Primary components:** Entity Grid, Master Detail, Document Header/Lines, Availability Panel, Approval Bar.

---

### 5.4 CRM (`CRM`)

**Real life:** Who we talk to before/while we sell — pipeline, not invoices.  
**Roles:** Sales Rep, Sales Manager, Key Account.  
**ERP ref:** Dynamics Sales / Salesforce-class pipeline; SAP CRM or CX — often separate from SD.  
**NOS better:** First-class module (not buried only under Sales); one-click bridge to SAL quotations/orders.

```text
CRM
├── Dashboard
├── Accounts
├── Pipeline
├── Activities
├── Bridge to Sales
└── Reports
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| CRM-001 | CRM Dashboard | Dashboard | Spec’d (index) |
| CRM-002 | Account List | Accounts | Spec’d (index) |
| CRM-003 | Account Detail | Accounts | Mapped |
| CRM-004 | Contact List / Detail | Accounts | Spec’d (index) |
| CRM-005 | Lead List / Detail | Pipeline | Spec’d (index) |
| CRM-006 | Opportunity List | Pipeline | Spec’d (index) |
| CRM-007 | Opportunity Board (Kanban) | Pipeline | Spec’d (index) |
| CRM-008 | Activities / Calendar | Activities | Spec’d (index) |
| CRM-009 | Quotation Bridge | Bridge | Spec’d (index) |
| CRM-010 | CRM Reports | Reports | Mapped |

**Primary components:** Kanban, Entity Grid, Master Detail, Activity Timeline, Calendar.

---

### 5.5 Purchasing (`PUR`)

**Real life:** Need → source → order → receive → invoice.  
**Roles:** Buyer, Purchasing Manager, Approver.  
**ERP ref:** SAP MM-PUR / D365 Procurement / IFS Purchase.  
**NOS better:** RFQ comparison board; receiving handoff into Inventory GR.

```text
Purchasing
├── Dashboard
├── Sourcing
├── Orders
├── Inbound
├── Master Data
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| PUR-001 | Purchasing Dashboard | Dashboard | Spec’d |
| PUR-002 | Purchase Request List | Sourcing | Spec’d |
| PUR-003 | Purchase Request Detail | Sourcing | Spec’d |
| PUR-004 | RFQ List | Sourcing | Spec’d |
| PUR-005 | RFQ Detail | Sourcing | Spec’d |
| PUR-006 | Quotation Comparison | Sourcing | Spec’d |
| PUR-007 | Supplier Quotation Detail | Sourcing | Spec’d |
| PUR-008 | Purchase Order List | Orders | Spec’d |
| PUR-009 | Purchase Order Detail | Orders | Spec’d |
| PUR-010 | Receiving List (PO-GR) | Inbound | Spec’d |
| PUR-011 | Receiving Detail | Inbound | Spec’d |
| PUR-012 | Purchase Return | Inbound | Spec’d |
| PUR-013 | Supplier Invoice List | Inbound | Spec’d |
| PUR-014 | Supplier Invoice Detail | Inbound | Spec’d |
| PUR-015 | Supplier List | Master Data | Spec’d |
| PUR-016 | Supplier Detail | Master Data | Spec’d |
| PUR-017 | Purchasing Reports | Reports | Spec’d |
| PUR-018 | Purchasing Analytics | Analytics | Spec’d |
| PUR-019 | Purchasing Settings | Settings | Spec’d |

**Primary components:** Entity Grid, Master Detail, Comparison Matrix, Approval Wizard, Document Lines.

---

### 5.6 Inventory (`INV`)

**Real life:** Where is stock, can I issue it, did the count match?  
**Roles:** Warehouse, Inventory Controller, Planner.  
**ERP ref:** SAP WM/EWM / D365 WMS / Infor WMS — operations ≠ master data.  
**NOS better:** Workspace cut (Dashboard / Operations / Stock / Counts / Master / Reports) instead of flat entity CRUD.

```text
Inventory
├── Dashboard
├── Operations
├── Stock
├── Counts & Adjustments
├── Master Data
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| INV-001 | Inventory Dashboard | Dashboard | Spec’d |
| INV-015 | Goods Receipt List | Operations | Spec’d |
| INV-016 | Goods Receipt Detail | Operations | Spec’d |
| INV-017 | Goods Issue List | Operations | Spec’d |
| INV-018 | Goods Issue Detail | Operations | Spec’d |
| INV-019 | Transfer List | Operations | Spec’d |
| INV-020 | Transfer Detail | Operations | Spec’d |
| INV-027 | Putaway / Picking | Operations | Future |
| INV-028 | Package Management | Operations | Mapped |
| INV-014 | Stock Balance Inquiry | Stock | Spec’d |
| INV-010 | Lot List | Stock | Spec’d |
| INV-011 | Lot Detail / Trace | Stock | Spec’d |
| INV-012 | Serial List | Stock | Spec’d |
| INV-013 | Serial Detail | Stock | Spec’d |
| INV-030 | Reservation / Allocation | Stock | Spec’d |
| INV-021 | Cycle Count List | Counts | Spec’d |
| INV-022 | Cycle Count Detail | Counts | Spec’d |
| INV-023 | Physical Inventory | Counts | Spec’d |
| INV-024 | Inventory Adjustment | Counts | Spec’d |
| INV-002 | Product List *(bridge to PDT)* | Master Data | Spec’d |
| INV-003 | Product Detail *(bridge)* | Master Data | Spec’d |
| INV-004 | Material List | Master Data | Spec’d |
| INV-005 | Material Detail | Master Data | Spec’d |
| INV-006 | Warehouse List | Master Data | Spec’d |
| INV-007 | Warehouse Detail / Map | Master Data | Spec’d |
| INV-008 | Location List | Master Data | Spec’d |
| INV-009 | Location Detail | Master Data | Spec’d |
| INV-025 | Inventory Reports | Reports | Spec’d |
| INV-026 | Inventory Analytics | Analytics | Spec’d |
| INV-029 | Inventory Settings | Settings | Spec’d |

**Primary components:** Entity Grid, Master Detail, Warehouse Map, Metric Card, Lot/Serial Panel, Scan-friendly forms (mobile).

---

### 5.7 Production (`PRD`)

**Real life:** Plan the order, dispatch the floor, confirm output, watch OEE.  
**Roles:** Production Manager, Planner, Operator, Supervisor.  
**ERP ref:** SAP PP/ME / Opcenter / IFS Manufacturing / Infor LN+MES.  
**NOS better:** Planning vs Execution vs Shop Floor vs Monitoring separated; Operator Terminal is a first-class screen, not a CRUD form.

```text
Production
├── Dashboard
├── Planning
├── Execution
├── Shop Floor
├── Monitoring
├── Industry (Timber / Kiln / Thermowood)
├── Master Data
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| PRD-001 | Production Dashboard | Dashboard | Spec’d |
| PRD-PLAN-001 / PRD-101 | **Production Planning Wizard** (job) | Planning | ★ Exemplar |
| PRD-102 | Plan / Order Library | Planning | Mapped |
| PRD-010 / PRD-102b | Production Order Detail *(post-release)* | Planning | Spec’d → rename job |
| PRD-012 | Work Order List / Detail | Planning | Spec’d → job rename pending |
| PRD-021 / PRD-103 | Scheduling Board | Planning | Spec’d |
| PRD-022 / PRD-104 | Capacity Load Board | Planning | Spec’d |
| PRD-023 / PRD-105 | Dispatch Board | Planning | Spec’d |
| PRD-013 | Operator Terminal | Execution / Shop Floor | Spec’d |
| PRD-024 | Machine Panel | Execution / Shop Floor | Spec’d |
| PRD-014 | Material Consumption | Execution | Spec’d |
| PRD-015 | Production Confirmation | Execution | Spec’d |
| PRD-016 | WIP Tracking | Execution | Spec’d |
| PRD-017 | Packaging | Execution | Spec’d |
| PRD-018 | Finished Goods | Execution | Spec’d |
| PRD-027 | Scrap | Execution | Spec’d |
| PRD-028 | Rework | Execution | Spec’d |
| PRD-030 | Live Production Board | Monitoring | Mapped |
| PRD-031 | Bottleneck / Alerts | Monitoring | Mapped |
| PRD-032 | Shift Handover | Monitoring | Mapped |
| PRD-040 | Timber Yard Reception | Industry | Mapped |
| PRD-041 | Log Yard Map / Inventory | Industry | Mapped |
| PRD-042 | Kiln Schedule / Batch | Industry | Mapped |
| PRD-043 | Kiln Monitoring | Industry | Mapped |
| PRD-044 | Thermowood Batch / Curves | Industry | Mapped |
| PRD-002 | BOM List | Master Data | Spec’d |
| PRD-003 | BOM Detail | Master Data | Spec’d |
| PRD-004 | Routing List | Master Data | Spec’d |
| PRD-005 | Routing Detail | Master Data | Spec’d |
| PRD-025 | Operations Master | Master Data | Spec’d |
| PRD-006 | Work Center | Master Data | Spec’d |
| PRD-007 | Machine | Master Data | Spec’d |
| PRD-026 | Production Line | Master Data | Spec’d |
| PRD-008 | Shift | Master Data | Spec’d |
| PRD-009 | Production Calendar | Master Data | Spec’d |
| PRD-033 | Tooling / Knife Library | Master Data | Mapped |
| PRD-020 | Production Reports | Reports | Spec’d |
| PRD-019 | Production Analytics | Analytics | Spec’d |
| PRD-029 | Production Settings | Settings | Spec’d |

**Primary components:** Scheduler, Kanban/Dispatch Board, Operator Terminal layout, Machine Status, OEE Chart, Entity Grid, Master Detail, Timeline.

---

### 5.8 Quality (`QLT`)

**Real life:** Inspect, hold, disposition, CAPA, prove traceability.  
**Roles:** Quality Engineer, Inspector, Production (release).  
**ERP ref:** SAP QM / D365 Quality / IFS Quality.  
**NOS better:** Inspection queue + NCR/CAPA families; lab/moisture for timber.

```text
Quality
├── Dashboard
├── Plans & Specs
├── Operations
├── Laboratory
├── Compliance
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| QLT-001 | Quality Dashboard | Dashboard | Spec’d |
| QLT-002 | Inspection Plan List | Plans | Spec’d |
| QLT-003 | Inspection Plan Detail | Plans | Spec’d |
| QLT-004 | Inspection Queue | Operations | Spec’d |
| QLT-005 | Incoming Inspection | Operations | Spec’d |
| QLT-006 | In-Process Inspection | Operations | Spec’d |
| QLT-007 | Final Inspection | Operations | Spec’d |
| QLT-008 | NCR List | Operations | Spec’d |
| QLT-009 | NCR Detail | Operations | Spec’d |
| QLT-010 | Root Cause | Operations | Spec’d |
| QLT-011 | CAPA List | Operations | Spec’d |
| QLT-012 | CAPA Detail | Operations | Spec’d |
| QLT-020 | Approval Inbox | Operations | Spec’d |
| QLT-017 | Moisture / Lab Results | Laboratory | Spec’d |
| QLT-013 | Certificates | Compliance | Spec’d |
| QLT-014 | Traceability | Compliance | Spec’d |
| QLT-018 | Attachments Desk | Compliance | Spec’d |
| QLT-015 | Quality Reports | Reports | Spec’d |
| QLT-016 | Quality Analytics | Analytics | Spec’d |
| QLT-019 | Quality Settings | Settings | Spec’d |

**Primary components:** Entity Grid, Master Detail, Checklist / Wizard, Approval, Genealogy Tree, Attachment Gallery.

---

### 5.9 Maintenance (`MNT`)

**Real life:** Keep assets running — request → work order → PM → downtime → spare parts → OEE.  
**Roles:** Maintenance Manager, Technician, Production Supervisor.  
**ERP ref:** SAP PM / IFS Maintenance / Infor EAM.  
**NOS better:** Asset tree + WO family + OEE board linked to Production machines.

```text
Maintenance
├── Dashboard
├── Assets
├── Work Management
├── Planning
├── Reliability
├── Spare Parts
├── Reports
├── Analytics
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| MNT-001 | Maintenance Dashboard | Dashboard | Spec’d |
| MNT-002 | Asset Explorer | Assets | Spec’d |
| MNT-003 | Asset Tree | Assets | Spec’d |
| MNT-004 | Asset Detail | Assets | Spec’d |
| MNT-005 | Asset Maintenance History | Assets | Spec’d |
| MNT-006 | Warranty | Assets | Spec’d |
| MNT-007 | Asset Documents | Assets | Spec’d |
| MNT-021 | Asset Costs | Assets | Spec’d |
| MNT-022 | Sensors / Condition | Assets | Spec’d |
| MNT-023 | Asset KPIs | Assets | Spec’d |
| MNT-008 | Work Request List | Work Management | Spec’d |
| MNT-009 | Work Request Detail | Work Management | Spec’d |
| MNT-010 | Maintenance Order List | Work Management | Spec’d |
| MNT-011 | Maintenance Order Detail | Work Management | Spec’d |
| MNT-014 | Corrective Desk | Work Management | Spec’d |
| MNT-025 | Technician Mobile Queue | Work Management | Spec’d |
| MNT-012 | Preventive Plans | Planning | Spec’d |
| MNT-013 | Preventive Calendar | Planning | Spec’d |
| MNT-015 | Downtime Events | Reliability | Spec’d |
| MNT-018 | OEE Board | Reliability | Spec’d |
| MNT-016 | Spare Parts List | Spare Parts | Spec’d |
| MNT-017 | Spare Parts Detail / BOM | Spare Parts | Spec’d |
| MNT-019 | Maintenance Reports | Reports | Spec’d |
| MNT-020 | Maintenance Analytics | Analytics | Spec’d |
| MNT-024 | Maintenance Settings | Settings | Spec’d |

**Primary components:** Tree, Entity Grid, Master Detail, Calendar, OEE Chart, Timeline, Mobile Queue.

---

### 5.10 Finance (`FIN`)

**Real life:** Cost, valuation, period close, management reporting — factory finance, not full banking suite in v1.  
**Roles:** Controller, Cost Accountant, CFO.  
**ERP ref:** SAP CO/FI lite / D365 Finance (scoped) / IFS Financials.  
**NOS better:** Manufacturing cost & inventory valuation first; ERP export bridge.

```text
Finance
├── Dashboard
├── Costing
├── Valuation
├── Budgets
├── Period Close
├── Reports
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| FIN-001 | Finance Dashboard | Dashboard | Mapped |
| FIN-002 | Cost Centers | Master / Costing | Mapped |
| FIN-003 | Manufacturing Cost | Costing | Mapped |
| FIN-004 | Product Cost | Costing | Mapped |
| FIN-005 | Inventory Valuation | Valuation | Mapped |
| FIN-006 | Budget | Budgets | Mapped |
| FIN-007 | Cost Analysis | Costing | Mapped |
| FIN-008 | Period Close Cockpit | Period Close | Mapped |
| FIN-009 | ERP Export Status | Period Close | Mapped |
| FIN-010 | Financial Reports | Reports | Mapped |
| FIN-011 | Finance Settings | Settings | Mapped |

**Primary components:** Metric Card, Pivot/Report Grid, Wizard (period close), Entity Grid.

---

### 5.11 HR (`HR`)

**Real life:** Who works here, org structure, shift labor link to Production — not a full HCM suite in v1.  
**Roles:** HR Admin, Plant Manager, Production (labor booking).  
**ERP ref:** D365 HR lite / SAP HCM subset / IFS HR — employees + org + time.  
**NOS better:** Tight link to Shifts (Production) and Labor Tracking; keep payroll Future.

```text
HR
├── Dashboard
├── Organization
├── People
├── Time & Attendance
├── Reports
└── Settings
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| HR-001 | HR Dashboard | Dashboard | Mapped |
| HR-002 | Organization Chart / Units | Organization | Mapped |
| HR-003 | Employee List | People | Mapped |
| HR-004 | Employee Detail | People | Mapped |
| HR-005 | Positions / Jobs | Organization | Mapped |
| HR-006 | Time & Attendance | Time | Mapped |
| HR-007 | Labor Booking Bridge | Time | Mapped |
| HR-008 | HR Reports | Reports | Mapped |
| HR-009 | HR Settings | Settings | Mapped |
| HR-010 | Payroll Export | Time | Future |

**Primary components:** Tree (org), Entity Grid, Master Detail, Calendar.

---

### 5.12 Administration (`ADM`)

**Real life:** Who can do what; system health; audit — platform governance.  
**Roles:** System Admin, Security Admin.  
**ERP ref:** Every enterprise suite’s admin / security console.  
**NOS better:** Company/Plant tenancy explicit; permission matrix readable by humans.

```text
Administration
├── Dashboard
├── Identity
├── Authorization
├── Tenancy
├── Files
├── Audit & Health
└── Integrations
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| ADM-001 | Admin Dashboard | Dashboard | Mapped |
| ADM-002 | User List | Identity | Mapped |
| ADM-003 | User Detail | Identity | Mapped |
| ADM-004 | Role List | Authorization | Mapped |
| ADM-005 | Role Detail | Authorization | Mapped |
| ADM-006 | Permission Matrix | Authorization | Mapped |
| ADM-007 | Company / Legal Entity | Tenancy | Mapped |
| ADM-008 | Plant / Site | Tenancy | Mapped |
| ADM-009 | File Library | Files | Mapped |
| ADM-010 | Audit Log | Audit & Health | Mapped |
| ADM-011 | System Health | Audit & Health | Mapped |
| ADM-012 | Integration Endpoints | Integrations | Future |
| ADM-013 | Job / Queue Monitor | Integrations | Future |

**Primary components:** Entity Grid, Master Detail, Permission Matrix, Health Cards, Audit Timeline.

---

### 5.13 Settings (`SET`)

**Real life:** My preferences + platform defaults (locale, theme, notifications).  
**Roles:** All users (personal); Admin (platform defaults).  
**ERP ref:** User options vs system parameters split (SAP SU3 vs SPRO-lite).  
**NOS better:** Personal vs platform clearly separated; module parameters stay in-module.

```text
Settings
├── Personal
├── Platform Defaults
└── Notification Rules
```

| ID | Screen | Workspace | Maturity |
|----|--------|-----------|----------|
| SET-001 | Personal Preferences | Personal | Mapped |
| SET-002 | Locale & Language | Personal | Mapped |
| SET-003 | Theme | Personal | Mapped |
| SET-004 | Notification Preferences | Personal | Mapped |
| SET-005 | Platform Defaults | Platform | Mapped |
| SET-006 | Numbering Series | Platform | Future |
| SET-007 | Notification Rules (Admin) | Notification Rules | Mapped |

---

## 6. Component Library (cross-module)

These components are **shared**. Screens compose them; modules do not fork them.

| Component | Used by (examples) | Library doc target |
|-----------|--------------------|--------------------|
| Entity Grid | All list screens | `docs/18_Component_Library/` |
| Master Detail | Orders, NCR, Asset, PO, … | |
| Tree | Asset, Org, Product Family, Locations | |
| Kanban | Opportunity, Dispatch, WO boards | |
| Timeline | Audit, Asset history, Activities | |
| Scheduler | Production scheduling, PM calendar | |
| Wizard | Period close, RFQ, approvals | |
| Approval Bar / Inbox | PUR, QLT, SAL, FIN | |
| Dashboard Card | All dashboards | |
| Metric Card | KPIs / OEE / stock | |
| Document Header + Lines | SO, PO, GR, Production Order | |
| Status Badge | Everywhere | |
| Filter Bar | All grids | |
| Attachment Gallery | Quality, Maintenance, CRM | |
| Operator Terminal Chrome | PRD-013 | |
| Warehouse Map | INV-007 | |
| Genealogy / Trace Tree | QLT-014, Lots | |
| Comparison Matrix | PUR-006 | |
| Notification Panel | Shell | |
| Workspace Chrome | All modules | |

Detailed component specs remain under `docs/18_Component_Library/` (author per component as screens demand).

---

## 7. Counts (v1 map)

| Module | Workspaces (approx.) | Screens (approx.) | Maturity |
|--------|----------------------|-------------------|----------|
| Dashboard | 4 | 7 | Mapped |
| Product | 5 | 11 | Mapped |
| Sales | 7 | 15 | Spec’d index |
| CRM | 5 | 10 | Spec’d index |
| Purchasing | 7 | 19 | Spec’d index |
| Inventory | 7 | 30 | Spec’d index |
| Production | 9 | ~40 | Strong Spec’d |
| Quality | 7 | 20 | Spec’d index |
| Maintenance | 8 | 25 | Spec’d index |
| Finance | 6 | 11 | Mapped |
| HR | 5 | 10 | Mapped |
| Administration | 6 | 13 | Mapped |
| Settings | 3 | 7 | Mapped |
| **Auth (pre)** | — | 6 | Mapped |
| **Total** | — | **~220** | Phase 2 baseline |

Exact counts will move as Product Architect deep-dives refine List/Detail splits.

---

## 8. Role → entry surfaces (global)

| Role | Lands on | Deep links |
|------|----------|------------|
| CEO / Executive | HOM-002 Executive Cockpit | Module dashboards |
| Plant Manager | HOM-003 Plant Overview | PRD-001, MNT-001, QLT-001, INV-001 |
| Production Manager | PRD-001 | Planning, Monitoring |
| Planner | PRD Planning | PRD-010, PRD-021, INV-014 |
| Operator | PRD-013 Operator Terminal | Confirmation, Scrap |
| Warehouse | INV-001 | Operations, Stock |
| Buyer | PUR-001 | Sourcing, Orders |
| Sales Rep | CRM-001 / SAL-001 | Pipeline → Quotes → Orders |
| Quality | QLT-001 / QLT-004 | NCR, CAPA |
| Maintenance Tech | MNT-025 / MNT-001 | WO, Downtime |
| Controller | FIN-001 | Costing, Valuation |
| HR Admin | HR-001 | People, Org |
| System Admin | ADM-001 | Identity, Permissions |

---

## 9. Program sequence (after this map)

1. **Lock** this Screen Map (this PR / Product Architect review).  
2. Deep-dive modules one by one with role lenses (start: **Production** or **Dashboard + Product**).  
3. Expand thin screens into full PRDs under `docs/15_UI/<Module>/`.  
4. Align `docs/19_Navigation` + `nav-config` to this tree.  
5. Grow `docs/18_Component_Library` as screens demand.  
6. Cursor implements **named workspace / screen families** only.

---

## 10. Authority & conflicts

When documents disagree:

```text
04_PRODUCT_ARCHITECT → this Screen Map → 15_UI_Architecture → 15_UI PRDs
  → 19_Navigation → 18 Components → Code
```

- `04_Application/Screen_Catalog.md` is a **flat name registry**; this map owns hierarchy.  
- Historical TASK files never redefine this tree.  
- `PRD-*` = Production; `PDT-*` = Product (catalog) — do not conflate.

---

## 11. Final statement

This Screen Map is the skeleton of NOS as an enterprise ERP/MES — not a CRUD shopping list.

**Next Product Architect move:** pick the first module deep-dive  
*(recommended: Production — “Üretim Müdürü ne görmeli?” — or Dashboard — “CEO / Plant Manager ne görmeli?”).*
