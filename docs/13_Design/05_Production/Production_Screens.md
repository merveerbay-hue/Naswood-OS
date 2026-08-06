# ==============================================================================
# PRODUCTION SCREENS
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.1
# ==============================================================================

# PURPOSE

This document defines every Production screen within the Production module.

Screens are organized by Workspace.

A Screen represents a complete business function.

Screens are never generated directly from database entities.

Every screen belongs to exactly one Workspace.

---

# AUTHORITY REFERENCES (do not redefine)

This document owns **UX / job screens** only. Cross-cutting laws live elsewhere.
See `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`.

| Topic | Authority — reference only |
|-------|----------------------------|
| Numbering (Material, Lot, Serial, Package, Pallet, Production IDs) | `docs/13_Design/99_Shared/Document_Numbering.md` |
| Genealogy | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Inventory ownership / stock posting | `docs/13_Design/02_Inventory/Inventory_Architecture.md` |
| Production execution process | `docs/13_Design/05_Production/Production_Workflow.md` |
| Job-first naming | `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md` |

Do **not** repeat rules such as “lot number is system-generated” here — link the Numbering authority.

---

# SCREEN DESIGN PRINCIPLES

**Job-first (mandatory):**  
Before any screen: *Kullanıcı bu ekranda hangi işi bitirmek istiyor?*  
Name the screen after that job — not after the entity.  
See `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`.

Production screens shall

- Follow manufacturing workflows (Wizard / Builder / Designer / Configuration / Terminal / Planner)
- Be role-oriented (especially Production Engineer for Engineering workspace)
- Be process-driven (steps, facets, gates, release)
- Support desktop and tablet
- Minimize user interaction
- Display contextual KPIs
- Provide contextual actions
- Support drill-down navigation

Screens must never behave as generic CRUD pages.

```text
Primary Planning entry:     Production Planning Wizard  (Wizard)
Shop floor:                 Operator Terminal (Terminal)
Engineering (Master Data):  Builder / Designer / Configuration / Planner — NOT Create Form
Not the design center:      “New Machine” / “New BOM” / Code · Name · Save
```

```text
NOS'ta Master Data ekranları "Create Form" değildir.
```

Screen types authority: `docs/13_Design/Common/Screen_Types.md` § 1 · § 2b · § 3a  
Patterns: `docs/13_Design/Common/UI_Patterns.md`  
CTA ops: **Üretim planla / Plan production** — never bare “Yeni”.  
CTA engineering: **Build BOM** · **Configure machine** · **Design routing** · **Release**.

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
├── Engineering          ← was “Master Data”; knowledge Designers (not CRUD)
│
├── Analytics
│
└── Reports
```

*(Navigation may still label the workspace “Master Data”; product name = **Engineering**.)*

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

**Job to be done:** Planlamacı üretilebilir bir planı kurar; **ölçü + detay çizim + kesit** teknik paketini bağlar; Release eder ve istenirse **saha paketi (çıktı)** alır.

Full spec: `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`  
Screen type: **Wizard** · Files: Platform `File_Upload.md` (reference only)

Steps

1. Ürün seçimi  
2. Revizyon seçimi  
3. Ölçü seçimi (cut list / tolerans)  
4. **Teknik paket** — detay çizim · kesit · ekler  
5. Ağaç türü seçimi  
6. Hammadde uygunluğu  
7. Hat seçimi  
8. Kapasite kontrolü  
9. Termin planı  
10. Maliyet simülasyonu  
11. Onay, **Release** ve **çıktı** (shop packet / ölçü kartı / çizim seti)

Primary Users

- Production Planner  
- Production Manager (approve / release)

Outcome

- Draft plan saved, or Production Order **Released**  
- Optional: Shop packet PDF queued / printed

Components

- Wizard · Attachment Panel · Drawing/PDF Preview · Availability Panel · Capacity chart · Approval Bar · Print options

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

**Job to be done:** Release sonrası tek planı izle / belgele; teknik paketi yönet; **çıktı al** (oluşturma yolu değil).

Sections

- Overview · **Technical package** (ölçü özeti · detay çizim · kesit · ekler) · Materials · Routing · Schedule · Capacity · History

Actions

- **Print shop packet** / Export PDF / Download drawings · Reschedule (policy) · Duplicate → Wizard · Cancel  
- Attach / replace drawing or cross-section (permission) — files via Platform DMS

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

# ENGINEERING WORKSPACE

> **Law:** Master Data screens are not Create Forms.  
> They are **Builder · Designer · Configuration · Planner · Library** surfaces.  
> User **defines, validates, relates, simulates, and Releases** production knowledge.  
> Authority: `docs/13_Design/Common/Screen_Types.md` § 1 · § 3a.

Primary role: **Production Engineer** (also Process Engineer, Maintenance for machine facets).

| ID | Screen | Type | CTA | Replaces |
|----|--------|------|-----|----------|
| PRD-501 | **BOM Builder** | Builder | Build BOM / BOM oluştur | BOM Management CRUD |
| PRD-502 | **Routing Designer** | Designer | Design routing / Rota tasarla | Routing Management CRUD |
| PRD-503 | **Machine Configuration** | Configuration | Configure machine / Makine yapılandır | Machine Management CRUD |
| PRD-504 | **Work Center Designer** | Designer | Design work center / WC yerleştir | Work Center Management CRUD |
| PRD-505 | **Line Configuration** | Configuration | Configure line / Hat yapılandır | Production Lines CRUD |
| PRD-506 | **Operation Designer** | Designer | Design operation / Operasyon tasarla | Operations CRUD |
| PRD-507 | **Shift Planner** | Planner | Plan shifts / Vardiya planla | Shifts CRUD |
| PRD-508 | **Calendar Planner** | Planner | Plan calendar / Takvim planla | Calendars CRUD |
| PRD-509 | **Tool Library** | Library → Configuration | Manage tooling / Takım yönet | Tooling CRUD |

Each Engineering object also has a **Library** view (find & reopen) — Library never hosts Code · Name · Save.

---

## PRD-501 BOM Builder

**Type:** Builder  
**Job:** Üretim mühendisi, ürün revizyonu için malzeme ağacını kurar, alternatif/fire/operasyon bağlarını doğrular ve **Release** eder.  
**Not the job:** “Create BOM row” (Code · Description · Save).  
**Spec:** `docs/00_Product/Process_Screens/PRD_BOM_Builder.md`

```text
Ürün → Revizyon → Malzeme Ağacı → Alternatif Malzemeler
  → Fire → Operasyon Bağlantıları → Onay → Release
```

---

## PRD-502 Routing Designer

**Type:** Designer  
**Job:** Proses rotasını operasyon → makine → tool → süre → işçilik → QC → paralellik ile tasarlar; simüle eder; Release eder.  
**Not the job:** Routing CRUD.

```text
Operasyon → Makine → Tool → Cycle Time → Labor → QC
  → Paralel Operasyon → Simülasyon → Release
```

---

## PRD-503 Machine Configuration

**Type:** Configuration  
**Job:** Makineyi kimlik, yerleşim, teknik, üretim yeteneği, bakım ve dokümanlarıyla tanımlar; Validate → Release.  
**Not the job:** Machine Code · Name · Save.  
**Spec:** `docs/00_Product/Process_Screens/PRD_Machine_Configuration.md`

```text
Kimlik (Tip · Grup · Üretici · Model · Seri · Asset)
  → Yerleşim (Fabrika · Bina · Hat · Work Center · Pozisyon)
  → Teknik (Eksen · Max En/Boy/Kalınlık · Devir · Güç · Voltaj)
  → Üretim (Operasyonlar · Ağaç türleri · Ürünler · Tool Magazine · Setup · Cycle)
  → Bakım (PM · Yağlama · Sensörler · Sayaçlar)
  → Doküman (PDF · Manual · CAD · Fotoğraf)
  → Validate → Release
```

This does **not** fit on one flat form — Configuration uses facets / sections.

---

## PRD-504 Work Center Designer

**Type:** Designer (Layout)  
**Job:** Work Center yerleşimini ve kapasite bağlamını tasarla — Layout Builder.  
**Not the job:** Work Center CRUD.

```text
Work Center kimliği → Kapasite profili → Makine atamaları
  → Yerleşim / layout → Kuyruk kuralları → Release
```

---

## PRD-505 Line Configuration

**Type:** Configuration  
**Job:** Production Line’ı istasyonlar, akış ve kısıtlarla yapılandır.  
**Not the job:** Line CRUD.

```text
Hat kimliği → İstasyon sırası → Makine / WC bağları
  → Akış kuralları → Kısıtlar → Release
```

---

## PRD-506 Operation Designer

**Type:** Designer  
**Job:** Standart operasyonu parametreler, QC noktaları ve makine uygunluğu ile tasarla.  
**Not the job:** Operation CRUD.

```text
Operasyon kimliği → Parametreler → Makine uygunluğu
  → Tool / setup → QC noktaları → Release
```

---

## PRD-507 Shift Planner

**Type:** Planner  
**Job:** Vardiya şablonlarını ve atamaları planla.  
**Not the job:** Shift CRUD.

---

## PRD-508 Calendar Planner

**Type:** Planner  
**Job:** Çalışma / tatil / bakım takvimini planla.  
**Not the job:** Calendar CRUD.

---

## PRD-509 Tool Library

**Type:** Library → opens Tool Configuration  
**Job:** Takım / bıçak kütüphanesinde bul, yapılandır, ömür ve magazine ilişkisini yönet.  
**Not the job:** Tooling Code · Name · Save form as the product.

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
