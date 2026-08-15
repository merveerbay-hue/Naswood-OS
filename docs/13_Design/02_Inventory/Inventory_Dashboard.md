# Inventory Dashboard — Warehouse Command Center

**Module:** Inventory  
**Screen ID:** INV-001  
**Workspace:** Dashboard  
**Screen type:** Dashboard (**operational command center** — not Analytics)  
**Version:** 2.0  
**Status:** Product Architect — authoritative  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`

---

## Absolute rule

```text
Inventory Dashboard is NOT a KPI page.
Inventory Dashboard is NOT a historical analytics wall.
Inventory Dashboard is the operational command center of the warehouse.
```

**Forbidden:** Hero KPI strip as the primary job · “Inventory Value” as first viewport · chart-first layout · Create entity forms  
**Required:** Job CTAs · live queues · exceptions · dock / inbound pressure · one-click into Workbench / Wizard / Terminal

Analytics, valuation trends, and slow-moving studies live in **Reports** / **Analytics** workspaces — not here.

---

## Job to be done

> Depo şefi / operatör, **bugün depoyu yönetir**: hangi kamyon / kabul / çıkış / transfer / sayım bekliyor; nerede bloke / negatif / kapasite riski var; bir tıkla ilgili iş ekranına girer.

**Not the job:** “See inventory KPIs” or “Browse valuation charts.”

---

## CTA / entry

| Locale | Opens |
|--------|--------|
| TR | **Envanter · Komuta Merkezi** (module home) |
| EN | **Inventory · Command Center** |

From here, primary actions:

| CTA | Target |
|-----|--------|
| **Mal kabul başlat** | Receiving Workbench |
| **Mal çıkışı** | Issue Wizard |
| **Stok transfer** | Transfer Wizard |
| **Sayım başlat** | Cycle Count Session |

Never: “+ New Goods Receipt” from the command center.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Screen type / no Create | `Screen_Types.md` · `UI_Patterns.md` |
| Receiving | `INV_Receiving_Workbench.md` · `Material_Identity_Architecture.md` |
| Stock truth | `Inventory_Architecture.md` |
| Workspaces / nav | `Inventory_Workspaces.md` · `Inventory_Navigation.md` |
| Screen index | `Inventory_Screens.md` |

---

## Command center anatomy

```text
┌─────────────────────────────────────────────────────────────────┐
│ INV-001  Warehouse Command Center     Plant · Shift · Now       │
│ [Mal kabul başlat] [Mal çıkışı] [Transfer] [Sayım başlat]       │  ← ACTION BAR
├──────────────────────────────┬──────────────────────────────────┤
│ LIVE QUEUES                  │ EXCEPTIONS / BLOCKS              │
│ Open receiving (Draft/InProg)│ Negative stock                   │
│ Trucks at gate / dock        │ QI / Hold / Blocked              │
│ Open issues · transfers      │ Capacity critical locations      │
│ Open counts · putaway tasks  │ Overdue receipts / ASN           │
├──────────────────────────────┴──────────────────────────────────┤
│ TODAY’S PRESSURE (thin status — not hero KPIs)                  │
│ Available · Reserved · Inbound expected · Outbound due          │
│  → each cell drills to Stock / Operations job — not a report    │
├─────────────────────────────────────────────────────────────────┤
│ DOCK / INBOUND BOARD (optional plant)                           │
│ Gate · Truck · Supplier · Stage · Open Workbench                │
├─────────────────────────────────────────────────────────────────┤
│ AI / OPS HINTS (actionable only)                                │
│ “3 receipts waiting Post” · “Zone A full — suggest WH-FG”       │
└─────────────────────────────────────────────────────────────────┘
```

### Layout laws

| Zone | Purpose | Anti-pattern |
|------|---------|--------------|
| **Action bar** | Start warehouse jobs | Hidden behind menus |
| **Live queues** | Work waiting **now** | Monthly charts |
| **Exceptions** | What will stop the floor | Vanity metrics |
| **Thin status** | Context for decisions | Giant KPI cards as the page |
| **Dock board** | Physical inbound reality | Empty decorative widgets |
| **Hints** | Next best action | Passive “insights” without link |

Every row / card **opens a job screen** (Workbench, Wizard, Terminal, Explorer inquiry) within one click.

---

## Sections (operational)

### 1 — Action bar (primary)

Job verbs only — same CTAs as Operations workspace.

### 2 — Live queues

| Queue | Meaning | Opens |
|-------|---------|--------|
| Open receiving | Draft / InProgress Receiving sessions | Receipt library or resume Workbench |
| At gate / dock | Trucks registered, not Posted | Receiving Workbench |
| Open goods issues | Waiting pick / post | Issue Wizard / library |
| Open transfers | In motion | Transfer Wizard / library |
| Open counts | Sessions in progress | Count Session |
| Putaway / pick tasks | Directed work (INV-027) | Task Terminal |

Counts are **queue depth**, not executive KPIs.

### 3 — Exceptions / blocks

| Signal | Action |
|--------|--------|
| Negative stock | Balance inquiry → adjust / count |
| Hold / QI / Blocked | Lot / MI inquiry · Quality |
| Capacity critical | Warehouse / location Explorer |
| Overdue ASN / PO receipt | Start Receiving Workbench |
| Missing Material Identity on posted line | Controllers only — data fix |

### 4 — Thin status strip (secondary)

On-hand · Reserved · Available · Expected inbound · Due outbound.

These support the queues — they must not dominate the first viewport. No “Inventory Value” in the command center primary surface (Finance / Reports).

### 5 — Dock / inbound board

Truck plate · supplier · gate · Workbench stage · time since arrival · **Open Workbench**.

Ties to Receiving Workbench truck registration.

### 6 — Actionable hints (optional AI)

Only recommendations that deep-link to a job. No orphan insight cards.

---

## What lives elsewhere

| Content | Workspace / screen |
|---------|-------------------|
| Inventory value trends | Reports / Finance |
| Slow / fast movers analytics | Analytics |
| Historical movement charts | Reports |
| Master data counts as “footprint KPIs” | Master Data libraries — not command center hero |

---

## Roles

| Role | Command center focus |
|------|----------------------|
| Warehouse Operator | Action bar · my queues · dock |
| Warehouse Supervisor | All queues · exceptions · capacity |
| Inventory Controller | Exceptions · accuracy · holds |
| Planner / Purchasing | Inbound overdue · availability strip (read) |

Role-based widgets — same law: **jobs first**.

---

## Mobile

Rugged tablet: action bar + queues + exceptions. Charts optional / deferred. Scan deep-links into Receiving / Issue Terminal.

---

## Cursor implementation notes

1. Do **not** implement INV-001 as a KPI card grid with value/accuracy charts.  
2. Primary viewport = Action bar + Live queues + Exceptions.  
3. Queue counts from operational APIs (`openGoodsReceipts`, …) — label as **work waiting**, not KPI.  
4. CTAs navigate to job paths (`/inventory/operations/receive`, …).  
5. Reports link is secondary footer — not the page purpose.  
6. Screen type remains **Dashboard** in catalog, with Inventory specialization = **Command Center**.

---

## Related

`Inventory_Screens.md` · `Inventory_Workspaces.md` · `Inventory_Navigation.md` · `Inventory_User_Flows.md`  
`INV_Receiving_Workbench.md` · `UI_Patterns.md` § Dashboard · `Screen_Types.md`
