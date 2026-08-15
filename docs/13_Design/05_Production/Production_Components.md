# ==============================================================================
# PRODUCTION COMPONENTS
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.0
# Status: Active
# ==============================================================================

# PURPOSE

This document lists the **shared UI components** used by Production job screens.
Components are composed; they do not redefine platform laws.

Authority: `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`  
Component library target: `docs/18_Component_Library/`  
Job-first: `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`

Production screens **compose** these components. Numbering, genealogy, and
inventory rules are **referenced**, never restated here.

---

# AUTHORITY REFERENCES

| Topic | Authority |
|-------|-----------|
| Numbering | `docs/13_Design/99_Shared/Document_Numbering.md` |
| Genealogy | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Inventory ownership | `docs/13_Design/02_Inventory/Inventory_Architecture.md` |
| Execution process | `Production_Workflow.md` |
| UX jobs | `Production_Screens.md` · `Production_User_Flows.md` |

---

# COMPONENT CATALOG

## Shell / workspace

| Component | Purpose | Used by |
|-----------|---------|---------|
| Workspace Chrome | Module workspace tabs + screen strip | All Production routes |
| Breadcrumb | Module / Workspace / Screen | All |
| Context Header | Plant · Product · Qty · Due (sticky) | Wizard, Terminal, Detail |

## Planning / process

| Component | Purpose | Used by |
|-----------|---------|---------|
| **Wizard** | Multi-step job with gates | PRD-101 Production Planning Wizard |
| Stepper | Step progress / lock | Wizard |
| Approval Bar | Submit / Approve / Release | Wizard step 10, Detail |
| Availability Panel | Required vs on-hand / shortage | Wizard step 5 |
| Capacity Chart | Load vs capacity | Wizard step 7, PRD-104 |
| Scheduler / Gantt strip | Single-order or board timeline | Wizard step 8, PRD-103 |
| Risk / Constraint Badge | Overload, shortage, margin | Wizard |
| Comparison / Line Card | Line / WC picker with utilization | Wizard step 6 |
| Attribute Panel | Dimensions, species, grade | Wizard steps 3–4 |

## Boards

| Component | Purpose | Used by |
|-----------|---------|---------|
| Scheduling Board | Drag-drop multi-order schedule | PRD-103 |
| Capacity Load Board | Bottleneck / utilization | PRD-104 |
| Dispatch Board (Kanban) | Ready / Running / Delayed | PRD-105 |
| Live Production Board | Running counters / colors | PRD-401 |

## Execution / shop floor

| Component | Purpose | Used by |
|-----------|---------|---------|
| Operator Terminal Chrome | Large-touch job surface | PRD-301 / PRD-013 |
| Machine Status Panel | Running / Idle / Setup / Breakdown | PRD-302, PRD-402 |
| Scan Field (Barcode/QR) | Lot / material / package scan | PRD-303, PRD-304, consumption |
| Confirmation Qty Panel | Good / Scrap / Rework qty | PRD-203 |
| Package Builder | Pack / pallet / labels | PRD-205 |
| WIP Queue List | Op queues / waiting / running | PRD-204 |

## Data / master

| Component | Purpose | Used by |
|-----------|---------|---------|
| Entity Grid | Filterable library lists | Plan Library, Master Data lists |
| Master Detail | Header + tabs/panels | PO Detail, BOM, Routing, Machine |
| Tree View | BOM / line hierarchy | BOM Detail |
| Document Header + Lines | Structured documents | BOM lines, Material list |
| Filter Bar | Status / plant / date filters | All grids |
| Status Badge | Draft / Released / In Progress… | Everywhere |
| Split View | List + preview | Libraries |

## Dashboard / analytics

| Component | Purpose | Used by |
|-----------|---------|---------|
| Dashboard Card | Section container | PRD-001 |
| Metric Card | KPI value + trend | Dashboard, Analytics |
| Chart (trend / gauge / Pareto) | OEE, scrap, downtime | Dashboard, PRD-601–604 |
| Alert List | Delay / downtime / shortage | PRD-001, PRD-405 |
| Task Inbox | Approvals / follow-ups | Dashboard |
| Timeline | Events / shift / downtime | PRD-404 |
| Export Bar | PDF / Excel / CSV | Reports |

## Shared panels

| Component | Purpose | Used by |
|-----------|---------|---------|
| Attachment Panel | Typed technical files (DetailDrawing, CrossSection, …) | Planning Wizard step 4, Order Detail, Scrap, Quality |
| Drawing / PDF Preview | Preview drawings & cross-sections | Wizard · Detail · Shop packet |
| Print options / Shop packet compose | Select outputs (packet, dimension card, drawing set, labels) | Wizard step 11 · Order Detail |
| Audit Timeline | Compliance history | Detail screens |
| Genealogy Tracer | Forward/backward view *(data from Genealogy authority)* | Monitoring / FG |
| Notification toast | Action result | All mutations |

---

# COMPOSITION RULES

1. Prefer **Wizard / Board / Terminal** over bare Entity Grid for operational jobs.  
2. Entity Grid is for **libraries** (find & reopen), not for creating complex plans.  
3. Do not fork components per module — extend `18_Component_Library` when a pattern is shared.  
4. Components display identifiers; they never mint Material/Lot/Serial/Package/Pallet/Production IDs (Numbering Service).  
5. Stock-affecting actions call Inventory APIs; UI never “updates stock” locally.

---

# PRIORITY FOR IMPLEMENTATION

| Priority | Components | Why |
|----------|------------|-----|
| P0 | Wizard, Stepper, Approval Bar, Availability Panel, Entity Grid, Status Badge | Planning Wizard + Library |
| P0 | Operator Terminal Chrome, Confirmation Qty Panel, Scan Field | Shop floor |
| P1 | Scheduler, Capacity Chart, Dispatch Kanban | Planning boards |
| P1 | Metric Card, Dashboard Card, Alert List | Dashboard |
| P2 | Package Builder, Genealogy Tracer, Charts, Export Bar | Packaging / analytics |

---

# RELATED DOCUMENTS

- `Production_Screens.md`
- `Production_User_Flows.md`
- `Production_Workspaces.md`
- `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`
- `docs/18_Component_Library/`
