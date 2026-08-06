# INV-ISS-001 — Goods Issue Workbench

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** **Workbench** (warehouse execution) — `Screen_Types.md` · `UI_Patterns.md`  
**Version:** 3.1 — Master Prompt v3.0 + Manual Package Selection / Smart Scan  

**Status:** Product Architect — authoritative issue UX  
**Supersedes as primary UX:** linear [`INV_Issue_Wizard.md`](./INV_Issue_Wizard.md) (retained as spine)  
**Inventory Workbench Design Standard:** [`Inventory_Workbench_Design_Standard.md`](../../13_Design/99_Shared/Inventory_Workbench_Design_Standard.md) — GI is first full consumer of v3.0  
**Stock truth:** `Inventory_Architecture.md` · `Inventory_Workflow.md`  
**Identity:** `Document_Numbering.md` · `Material_Identity_Architecture.md` · `Material_Genealogy.md`  
**Evidence / Document Library / Export:** [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md)  
**Package Allocation Workspace (shared pattern):** [`Package_Allocation_Workspace.md`](../../13_Design/99_Shared/Package_Allocation_Workspace.md) — GI is first consumer; same UX on Receiving · Transfer · Production · Shipping · Count  
**Audit / Approvals:** `Audit_Log.md` · `Approval_Workflow.md`  
**Package / barcode immutability (format refs):** `Barcode_QR_Model.md` · `Barcode_Strategy.md` · `Naming_Standards.md` · Packaging module  
**Design program:** `Inventory_Design_Program.md` § 7 (PA-directed ahead of Putaway)

---

## Changelog

| Version | What landed |
|---------|-------------|
| 1.0 | Demand-backed Workbench spine · AI pick · scan/verify · quality · thin loading · Post · Evidence First |
| **2.0** | Override Mode + Warehouse Explorer · Package Selection detail · **Partial Package Consumption** · Package Identity permanence · expanded Loading · Document Library / Export UX surfaces · Override History · gates table |
| **2.0.1** | **Accept / Override Recommendation** CTAs · AI Validation continues in Override · Partial pick auto-updates remaining · **optional company-policy package split** with full traceability |
| **2.0.2** | Canonical worked scenarios: SO-250001 multi-package AI pick · **Kabul Et / Yoksay** CTAs · partial PKG-00254 120→40→80 |
| **2.0.3** | **Multiple Package Picking** · Package Allocation Grid · mix AI Validation (lot/quality/moisture/dims/customer) · edit/add/remove packages |
| **2.0.4** | **Package Allocation Workspace** = center of Workbench (not a simple table) · live qty/volume/weight/pkg count · barcode · DnD · keyboard · Excel-like · sort/filter/group · bulk · AI/manual · live validation · inventory sync |
| **2.0.5** | **Damage & Scrap during picking** · Take From Package (Good/Damaged/Hold/Scrap/Rework) · damage/scrap evidence · separate Scrap/Hold txns · **Package Closing Checklist** · PKG-00254 120→40→37+2+1+80 |
| **3.0** | Master Prompt v3.0 · **COMPLIANCE BY DESIGN** · composes `Inventory_Workbench_Design_Standard.md` · canonical package grid columns (m³ · dates · supplier · photos · reservation) · package status + Quality Hold / Damaged · Audit Trail · Revision Management · Electronic Approvals · Immutable posted txns (Reverse / Correction) · compliance frameworks (ISO / FSC / PEFC / TSE) |
| **3.1** | **Manuel Paket Seç** replaces Yoksay/IGNORE AI as primary CTA · Manual Package Selection = Scan **or** Package Number Search (not warehouse tree) · **Smart Scan** one-screen confirm + **Paketi Kullan** · Warehouse Explorer optional advanced only |

v3 **extends** Inventory Architecture / Workflow / Screens / Shared PAW / Evidence / Design Standard — it does not replace stock ledger, numbering, or disposition algorithms.

---

## Absolute rules

```text
COMPLIANCE BY DESIGN (Master Prompt v3.0)
─────────────────────────────────────────
Built for real manufacturing · ISO / TSE / FSC / PEFC / customer audits.
Evidence in the flow · immutable posted history · system-generated IDs ·
continuous genealogy. Controls are structural — not optional checklists.
Authority: Inventory_Workbench_Design_Standard.md
```

```text
This is NOT a CRUD screen.
This is NOT a Create / Edit form.
This is NOT a database editor.
This is an AI-powered Warehouse Operations Workbench.
Inventory quantities are NEVER edited directly.
Every Goods Issue creates Inventory Transaction(s).
Posted Inventory Transactions are NEVER editable → Reverse / Correction only.
```

```text
Operator: Scan · Verify · Review · Approve
System:   Think · Compare · Recommend · Validate · Warn · Generate · Track
Never allow issuing materials without a business document
(except permission-controlled Manual Issue).
```

```text
PACKAGE IDENTITY LAW (wood manufacturing — Master Prompt v2.0)
─────────────────────────────────────────────────────────────
Packages are physical objects.
Default partial pick: same Package Identity / Barcode / QR remain;
system updates Remaining Quantity · Volume · Weight · Pieces · Status.
Optional: if company policy requires automatic package split,
Numbering mints child package(s) while preserving complete traceability
(parent → child · Inventory Transactions · Genealogy).
Never silently rename or reuse a barcode.
Traceability = Inventory Transactions + Material Genealogy —
never by overwriting Package IDs.
```

```text
AI VALIDATION LAW
─────────────────────────────────────────────────────────────
After Manuel Paket Seç (scan or number search), AI continues validating.
Operator may change package selection.
Operator may NOT violate validation rules without explicit authorization.
```

```text
MANUAL SELECTION LAW (v3.1)
─────────────────────────────────────────────────────────────
Default manual path is NOT Warehouse Explorer.
Preferred: Scan barcode/QR at the physical package (Smart Scan).
Fallback: Search existing Package / Barcode / QR number.
Browse warehouse tree only if operator explicitly requests it.
```

**Forbidden:** Bare Save form · inventing Material / Lot / Package / Pallet / WH / Location / GI / txn / MI codes · orphan issue · reusing package barcodes · bypassing AI validation without authorization · forcing warehouse-tree browse after rejecting AI  
**Required:** Business document → load demand → AI pick (default) → **Kabul Et** or **Manuel Paket Seç** (scan / search + Smart Scan) → verify (AI validation always on) → evidence → quality → destination → review (incl. manual selections) → **Post**

---

## Question template answers

| # | Question | Answer |
|---|----------|--------|
| 1 | **Who is the user?** | Warehouse Operator (picker) · Warehouse Supervisor · Inventory Controller (exceptions) · optionally Production / Maintenance / Shipping coordinators as request owners |
| 2 | **Real-life job?** | Takes approved demand, forklift to package, **scans barcode** (or types package no. if unreadable), AI validates, picks qty (often **partial**), confirms issue so stock ↓ — **same physical barcode stays on the package** |
| 3 | **Documents?** | Source: Production Order · Maintenance WO · Sales Order · Sample / R&D / Scrap / Transfer / Internal · Manual GI. Output: GI · picking list · loading note (if ship) · digital file / Document Library |
| 4 | **Photos?** | Damage / missing / broken package · loading · optional video / voice — Evidence Panel → permanent archive |
| 5 | **AI support?** | FIFO/FEFO · reservation · quality · customer reqs · WH rules · location optimize · package integrity · availability · pick route · wrong material/dims/species/moisture/quality/package/lot detect |
| 6 | **Auto-generated?** | GI · inventory txn(s) · histories · audit · scan/validation/override history · genealogy · remaining package qty/volume/weight/status · suggested pick |
| 7 | **Never manual?** | Inventing Material / Lot / Package / Pallet / WH/Location / GI / txn / MI codes · free-hand stock edit · **new package number on partial issue** — **allowed:** scan; **fallback:** search existing package/barcode/QR number |
| 8 | **User decisions?** | Which demand · **Kabul Et** or **Manuel Paket Seç** (scan / search) · issue qty · disposition · destination · evidence · **Approve Post** · raise NCR |

---

## Job to be done

> Depocu, **iş belgesine** bağlı talebe karşı AI önerisini **Kabul Et** eder veya **Manuel Paket Seç** ile sahada barkodu okutur (okunmuyorsa paket no arar); **Smart Scan** ile tek ekranda doğrular ve **Paketi Kullan** der; **kısmi paket**te kalan miktar güncellenir; manuel seçimde de AI validation çalışır; **Post** ile stok ve genealogy güncellenir.

**Not the job:** Create a GoodsIssue row · browse Depo→Zone→Rack tree after rejecting AI · bypass validation · invent package codes.

## CTA

| Locale | Label |
|--------|--------|
| EN | **Issue goods** / Open Goods Issue Workbench |
| TR | **Mal çıkışı** / Mal Çıkış Workbench |

Never: “Yeni çıkış” / “Create Goods Issue.”

Entry: Command Center queues · Operations · Production / Maintenance / Sales deep-link · rugged Terminal into same session · INV-017 library reopen.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| **Compliance by Design · Workbench laws** | [`Inventory_Workbench_Design_Standard.md`](../../13_Design/99_Shared/Inventory_Workbench_Design_Standard.md) |
| Identifiers | `Document_Numbering.md` |
| Material Identity / Lot (Package ≠ MI) | `Material_Identity_Architecture.md` |
| Genealogy | `Material_Genealogy.md` |
| Stock / reservations / txn immutability | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Evidence · Document Library · Export | `Document_Management_Evidence_and_Export.md` |
| Package Allocation Workspace | `Package_Allocation_Workspace.md` |
| Multi-UoM · pcs / lm / m² / m³ / kg / t | `Measurement_Conversion_Engine.md` |
| Audit trail engine | `Audit_Log.md` |
| Electronic approvals | `Approval_Workflow.md` |
| Package code immutability / QR | `Barcode_QR_Model.md` · `Barcode_Strategy.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |
| Manual Package Selection · Smart Scan | `Inventory_Workbench_Design_Standard.md` § 5–5b |
| Warehouse Explorer (optional advanced browse) | Design Program #4 — **not** the default after rejecting AI |
| Demand sources | Production / Maintenance / Sales / Quality process docs (reference only) |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited. Users work with names and scans; codes are display-only.
Selecting an existing Package / Lot / MI for consumption is allowed.
Minting a new Package Identity is NOT part of partial Goods Issue.
```

---

## Allowed business documents (issue source)

Goods Issue **can only** originate from:

| Source | Typical consumer |
|--------|------------------|
| Production Order | Production issue / backflush companion |
| Maintenance Work Order | Spare / consumable issue |
| Sales Order | Ship / stage for shipment |
| Sample Request | Quality / sales sample |
| Internal Consumption | Cost center / overhead |
| R&D Request | Lab / trial |
| Scrap Request | Controlled scrap issue |
| Warehouse Transfer | Issue leg toward transfer (policy) |
| **Manual Issue** | **Permission-controlled exception only** |

```text
Goods cannot leave inventory without a business document.
```

Manual requires elevated permission + reason code + audit.

---

## Workbench design (not a form)

```text
The CENTER of the Goods Issue Workbench is the Package Allocation Workspace.
This is NOT a simple table.
It is an interactive allocation workspace (professional warehouse planning class).
Highest design priority — primary operational screen.
```

Operational command center — compose:

| Surface | Role |
|---------|------|
| **Package Allocation Workspace** | **MAIN** — multi-package · partial · live calcs · AI/manual |
| Timeline / stage rail | Session progress (secondary to workspace) |
| Live totals strip | Required · Selected · Remaining · Volume · Weight · # Packages |
| Warehouse Map | AI route + current bin (context) |
| Material demand strip | Required lines · reserved · available |
| AI Recommendation Panel | Seed / reset allocation |
| Manual Package Selection / Smart Scan | Scan or number search → one-screen confirm |
| Warehouse Explorer | Optional advanced browse only (explicit request) |
| Evidence Panel | Photos · video · voice · docs |
| Document Viewer / Library | Session digital file |
| Sticky Action Bar | Draft · Back · Next · Kabul Et · Manuel Paket Seç · NCR · **Post** |

Enterprise refs: SAP EWM · Dynamics SCM · Infor WMS · IFS Cloud · Manhattan planning grids — adapted to NOS laws.

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ INV-ISS-001  Goods Issue Workbench     GI-… · SO-… · Live totals strip   │
├──────────┬───────────────────────────────────────────────┬───────────────┤
│ TIMELINE │ ★ PACKAGE ALLOCATION WORKSPACE (MAIN)         │ CONTEXT       │
│ 1–10     │ Toolbar · Filter/Sort/Group · Scan · DnD      │ AI / Validate │
│          │ Interactive grid · inline qty · bulk select   │ Explorer pool │
│          │ Live: Qty · Vol · Weight · #Pkg               │ Overrides     │
├──────────┴───────────────────────────────────────────────┴───────────────┤
│ STICKY: Draft · Back · Next · Kabul Et · Manuel Paket Seç · NCR · Post   │
└──────────────────────────────────────────────────────────────────────────┘
```

---

## Flow (10 steps)

```text
1 Select business document
2 Load material requirements
3 AI picking recommendation → Kabul Et  OR  Manuel Paket Seç (Scan / Number Search + Smart Scan)
4 Package Allocation Workspace (interactive grid · live calcs · scan)
5 Verify material & package  (AI Validation — also after manual select)
6 Evidence collection
7 Quality validation
8 Loading / production destination
9 Final review (+ Manual Selection / Override History)
10 Posting → Inventory Transaction(s) + history + genealogy + Evidence Archive
```

### 1 — Select business document

Display eligible open demands:

Production Order · Sales Order · Maintenance Order · Transfer Request · Internal Request · Sample · R&D · Scrap · Manual (if permitted).

Show per demand:

| Field |
|-------|
| Owner |
| Priority |
| Required Date |
| Status |
| Customer (when sales) |
| Project (when applicable) |

**Gate:** One valid reference selected (or Manual + permission + reason).

---

### 2 — Load material requirements

Automatically retrieve (no manual material creation):

| Field |
|-------|
| Required Materials |
| Required Quantity |
| Reserved Quantity |
| Available Quantity |
| Alternative Materials |
| Required Package Type |
| Required Quality |
| Required Dimensions |
| Required Moisture |

Name-first display; codes secondary / display-only.

**Gate:** ≥1 open line with remaining qty.

---

### 3 — AI picking recommendation

The system **shall automatically recommend** inventory according to:

| Driver |
|--------|
| FIFO |
| FEFO |
| Reservation |
| Quality Status |
| Warehouse Rules |
| Location Optimization |
| Package Integrity |
| Customer Requirements |

The recommendation is the **default option**.

Display:

| Recommendation |
|----------------|
| Recommended Warehouse |
| Recommended Location |
| Recommended Package |
| Recommended Quantity |
| Recommended Route |

Operator chooses exactly one path:

| Control (TR) | Control (EN) | Result |
|--------------|--------------|--------|
| **✓ Kabul Et** | **Accept** / Use AI recommendation | Proceed with AI default |
| **✓ Manuel Paket Seç** | **Manual Package Selection** | Open Manual Package Selection (scan / search) |

Also acceptable: **AI Önerisini Kullan** (= Kabul Et).  
Do **not** use “Yoksay / IGNORE AI” as the primary button label — **Manuel Paket Seç** is the operator language (Design Standard § 5).

**Gate:** Each line has **Kabul Et** recorded **or** an audited **Manuel Paket Seç** selection (method: scan | search | optional browse).

---

### Manual Package Selection

**Authority:** Design Standard § 5–5b.  
When AI recommendation is not used, **do not** open Warehouse Explorer by default.

Real life:

```text
Forklift → package → scan barcode → system validates
OR barcode unreadable → type package number → system finds → validates
```

UI presents **only two methods**:

```text
┌──────────────────────────────────┐
│ Paketi Nasıl Seçmek İstiyorsunuz? │
│ ◉ Barkod / QR Oku                │
│ ◉ Paket Numarası Ara             │
└──────────────────────────────────┘
```

#### 1 — Scan Barcode / QR (preferred)

Operator scans physical package → system retrieves → **Smart Scan** → AI validates Material · Dimensions · Species · Quality · Lot · Reservation · Customer Rules → **Paketi Kullan**.

#### 2 — Search Package Number (fallback)

If barcode cannot be scanned, operator searches by **Package Number · Barcode Number · QR Number** → same retrieval · Smart Scan · validations.

```text
Operators shall never browse complex warehouse trees unless explicitly requested
(e.g. link: “Depoyu Gez” — optional advanced, Design Program #4).
Barcode scanning is always preferred. Manual search is the fallback.
```

**Every manually selected package** is still validated by the AI engine before issue.  
**Every manual selection** is logged (actor · timestamp · AI proposal · chosen package · method).

---

### Smart Scan (Akıllı Barkod Tarama)

On successful scan or number search, show **one confirmation screen** (handheld-first) — not a second navigation hop:

```text
✓ Paket Doğru
📦 PKG-001245
🌲 Sarıçam
📏 26×140×4000
📦 120 Adet
📐 5.240 m³
📍 WH01 / A03 / R05
🟢 Kalite Uygun
🟢 Müşteriye Uygun
🟢 Lot Uygun

[ Paketi Kullan ]
```

| Outcome | Behavior |
|---------|----------|
| All green | **Paketi Kullan** adds package to Package Allocation Workspace |
| Soft warning (e.g. reserved for other customer) | Show ⚠ · “Devam etmek istiyor musunuz?” · confirm / waiver per policy |
| Hard fail | Block use until authorized waiver |

Operator adds the package with **one tap** — no list browsing required for the common path.

---

## Multiple Package Picking

A **single Goods Issue** may consume material from **multiple packages**.

The system shall automatically recommend the **minimum number of packages** while respecting:

| Driver |
|--------|
| FIFO / FEFO |
| Customer Requirements |
| Reservation Rules |
| Quality Status |
| Lot Consistency |
| Moisture Consistency |
| Dimension Consistency |
| Species Consistency |
| Warehouse Optimization |

The operator may:

| Action |
|--------|
| **Accept** the recommendation (**Kabul Et**) |
| **Modify** package quantities (Selected Quantity on the grid) |
| **Remove** packages from the allocation |
| **Add** additional packages (**Manuel Paket Seç** — scan / search / Smart Scan) |
| Build allocation manually via **Manuel Paket Seç** (not warehouse tree) |

Totals and remaining quantities recalculate in real time.  
Each allocated package still follows Partial Package Usage (barcode unchanged).

---

## Package Allocation Workspace

**Shared pattern authority:** [`Package_Allocation_Workspace.md`](../../13_Design/99_Shared/Package_Allocation_Workspace.md)  
**This PRD owns:** GI-specific demand mapping · Post · issue gates · worked scenarios.  
**Does not redefine:** workspace capabilities, live metrics, or cross-process UX language.

**Authority surface for picking.** Center of INV-ISS-001. Highest design priority.  
Strategic: same workspace language lands next on Receiving · Transfer · Production consumption · Shipping · Count.

```text
This is NOT a simple table.
This is an interactive allocation workspace
similar to professional warehouse planning software.
```

### Capabilities (shall support)

| Capability | Rule |
|------------|------|
| Multi-package allocation | One GI ← many packages |
| Partial package consumption | Selected ≤ Available; barcode unchanged |
| Real-time calculations | Every edit recalculates immediately |
| Inline quantity editing | Selected Quantity cells |
| Barcode-driven row selection | Scan focuses / selects / confirms row |
| Drag-and-drop package allocation | From Explorer / pool → workspace |
| Keyboard shortcuts | Arrow / Tab / Enter / Del (Excel-like) |
| Excel-like navigation | Cell focus · copy patterns where safe |
| Sorting | Any column |
| Filtering | Package · lot · location · quality · moisture · WH |
| Grouping | By WH · Zone · Lot · Quality (optional) |
| Bulk package selection | Multi-select → set qty / remove / waive |
| AI recommendations | Seed grid · Reset to AI · highlight deltas |
| Manual override | Manuel Paket Seç · Smart Scan · optional browse |
| Live validation | Mix + per-row rules as operator types |
| Inventory sync | Always synchronized with warehouse inventory (available qty live) |

### Live recalculation (every modification)

| Metric | Formula / meaning |
|--------|-------------------|
| Required Quantity | Demand line open qty |
| Selected Quantity | Σ Selected across rows |
| Remaining Quantity | Per row: Available − Selected · Σ remaining |
| Volume | Σ (selected × unit volume) / remaining volume |
| Weight | Σ (selected × unit weight) / remaining weight |
| Number of Packages | Count of rows with Selected > 0 |

### Columns (each row = one package)

Canonical set per [`Inventory_Workbench_Design_Standard.md`](../../13_Design/99_Shared/Inventory_Workbench_Design_Standard.md) § 7 — show/hide by role/config; GI default shows ops-critical columns first.

| Column | Content |
|--------|---------|
| Package Number | Display-only identity |
| Material Identity | Display-only |
| Warehouse | Name-first |
| Location | Zone / rack / shelf / bin |
| Lot | Display-only |
| Species | Spec |
| Dimensions | Spec |
| Quality Grade | Spec |
| Moisture | Spec |
| Available Quantity | Live from inventory |
| **Selected Quantity** | Inline editable (partial OK) |
| Remaining Quantity | Auto |
| Available m³ | Live via Measurement & Conversion Engine |
| Selected m³ | Auto via engine |
| Remaining m³ | Auto via engine |
| Weight | Selected & remaining via engine |
| Package Status | Available · Reserved · Picking · … |
| Production Date | When known |
| Receiving Date | From receiving root |
| Supplier | Name-first from receiving / PO |
| Photos | Thumbnail / open gallery |
| Reservation Status | Free / reserved / … |
| Customer Reservation | When bound to customer demand |

### Design priority

```text
1. Package Allocation Workspace  ← primary
2. Live totals + validation
3. AI seed / Override Explorer
4. Evidence / Loading / Review / Post
```

Document / materials stages prepare the session; once demand is loaded, the operator lives in the Allocation Workspace until Post.

---

## Partial Package Usage

Each package in the allocation **may be partially consumed**.

```text
Physical package barcode / QR / Package Identity → unchanged.
Only inventory balances update — via Inventory Transaction(s).
Remaining Quantity · Volume · Weight · Pieces · Status auto-update per package.
Picked quantity is classified (Good / Damaged / Hold / Scrap / Rework) — see Damage & Scrap.
```

Shared law: [`Package_Allocation_Workspace.md`](../../13_Design/99_Shared/Package_Allocation_Workspace.md) § 8–8c.  
See also § Partial Package Picking (PKG-00254) and optional company-policy split.

---

## Damage & Scrap during picking

**Authority (shared):** `Package_Allocation_Workspace.md` § 8b–8c.  
**This PRD:** GI Post mapping · worked scenario · gates.

When materials are removed from a package, **not everything picked is Good for the order**.

### Categories

Good · Damaged · Quality Hold · Scrap · Rework  

```text
Σ categories = Picked quantity (always).
Package total conserved: Good + Damaged + Hold + Scrap + Rework + Remaining = Original.
```

### Take From Package (right rail)

```text
Take From Package          PKG-00254
Requested                  40
Picked                     40
──────────────────────────
Good                       37   → shipment / demand
Damaged                     2   → evidence → Damage/Quality Hold
Scrap                       1   → evidence → Scrap txn
──────────────────────────
Remaining In Package       80
```

### Damage evidence

For every Damaged (and policy: Quality Hold) quantity: Photos · Damage type · Notes · optional Voice/Video.  
Linked to Goods Issue + Package + MI. Reasons: Broken · Cracked · Wet · Blue Stain · Warped · Forklift / Transport / Packaging Damage · Missing Material · Impact · Other.

### Scrap

```text
Scrap NEVER returns to the original package.
Scrap creates a separate Inventory Transaction.
Preserves: MI · Package Identity · Original Receiving · Operator · Date/Time · Reason · Evidence.
```

### Inventory / Genealogy outcome (example)

```text
PKG-00254 (120)
  ├─ 37  Good      → Sales Order / GI txn
  ├─ 2   Damaged   → Quality Hold / Damage Hold txn
  ├─ 1   Scrap     → Scrap txn
  └─ 80  Remaining → still Available on same Package Identity / barcode
```

Nothing overwritten. Full audit of shipped · scrapped · damaged · reworked · remained.

---

## Package Closing Checklist

After take-from-package on a physical package (esp. partial), before moving to next package / stage:

| Check |
|-------|
| ✅ Remaining material restacked properly? |
| ✅ Package strap / binding re-applied (if required)? |
| ✅ Package label still readable? |
| ✅ Update package photo? (optional capture → current physical state) |

Wood-yard value: next operator sees last package condition days/weeks later.

---

## AI Validation (allocation / mix)

**Even when** the operator Accepts, modifies quantities, Adds/Removes packages, or uses Override — AI **shall warn** if:

| Warning |
|---------|
| Multiple Lots are selected |
| Different Quality Grades are mixed |
| Different Moisture Levels are mixed |
| Different Dimensions are mixed |
| Customer-specific picking rules are violated |

```text
The operator may continue only if explicitly authorized
(Supervisor / Quality / policy role) — always audited.
Unauthorized → gate blocks Post / Next.
```

Wrong Material / Species / Package / Reservation / Moisture Class checks from § AI Validation still apply per row.

---

## Worked scenarios (canonical)

### Senaryo 1 — AI önerisi (varsayılan)

```text
Satış Siparişi     SO-250001
İstenen            Thermowood Deck 26×140×4000
Miktar             50 Paket
```

AI stokları tarar ve kurallara göre önerir:

| Paket | Miktar |
|-------|--------|
| Paket A | 20 Paket |
| Paket B | 15 Paket |
| Paket C | 15 Paket |
| **Toplam** | **50 Paket** |

Öneri kuralları: FIFO · FEFO · Rezervasyon · Kalite · Lokasyon · Aynı Lot · Aynı Nem · Aynı Kalite · Aynı Üretim Tarihi.

Operatör: **✓ Kabul Et**

→ Tarama / doğrulama yoluna devam (AI Validation açık).

---

### Senaryo 2 — Manuel Paket Seç + Smart Scan (çok kritik)

Operatör sahada biliyor ki **Paket D** forklift yanında / müşteriye daha uygun.

UI net iki seçenek sunar:

```text
[ Kabul Et ]
[ Manuel Paket Seç ]
```

**Manuel Paket Seç** → yöntem seçimi (Explorer **açılmaz**):

```text
◉ Barkod / QR Oku     ← tercih
◉ Paket Numarası Ara  ← barkod okunmuyorsa
```

Operatör PKG-D barkodunu okutur → **Smart Scan**:

```text
✓ Paket Doğru · PKG-… · spek · lokasyon · kalite/lot/müşteri bayrakları
[ Paketi Kullan ]
```

veya soft uyarı: “Bu paket farklı müşteriye rezervli. Devam?”  

Seçim audit’e yazılır. **AI Validation devam eder**.

---

### Senaryo 3 — Kısmi paket + hasar/fire (ahşap sektörü kritik)

```text
PKG-00254 · 120 adet
Operatör paketi açtı · 40 adet seçti (Picked)
  → 2 adet çatlak (Damaged) + foto + sebep
  → 1 adet köşe kırık / fire (Scrap) + foto + sebep
  → 37 adet Good → sevkiyat
  → 80 adet pakette kaldı (aynı barkod)
Toplam 37+2+1+80 = 120
```

Take From Package rail + Damage/Scrap evidence + Package Closing Checklist.  
Sistem Remaining / Hold / Scrap txn’lerini otomatik dengeler — paket kimliği değişmez.

---

### 4 — Picking / package selection

Follow AI route **or** use Manual Package Selection at the physical package. Support: Barcode · QR · RFID · Scanner · Voice (future). Prefer **Smart Scan** confirm.

Every scan/search runs through **AI Validation** against demand + proposal/manual selection + reservation.

Operator confirms **issue quantity** (may be less than package available — see Partial Package Picking).

**Gate:** Scans/searches recorded; issue qty > 0 and ≤ available (and ≤ reserved when reservation-bound); AI Validation clear or authorized waiver.

---

### 5 — Verify material

Operator review surface for AI Validation results (Accept and Override paths).

**Gate:** All lines green or explicitly authorized waiver (audited).

---

### 6 — Evidence collection

Every Goods Issue shall preserve (when captured), permanently linked to the GI transaction:

Photos · Videos · Voice Notes · Issue Documents · PDF · Excel · Operator Notes · Damage Photos · Loading Photos.

**Gate:** Required when exception/damage/loading policy says so; else optional.

Surfaces: Evidence Panel · Photo Gallery · Document Viewer — capabilities per Shared Document Management law (do not redefine).

---

### 7 — Quality validation

Hard quality state checks (Blocked · Quarantine · Inspection Hold · Expired · Rejected) plus AI Validation rules.

**Gate:** Quality clear for all issue lines (or authorized release recorded).

---

### 8 — Loading / destination

| If | Assign |
|----|--------|
| **Sales / shipment** | Vehicle · Loading Dock · Carrier · Shipment |
| **Production** | Production Line · Work Center · Operation |

Name-first pickers; no code typing. Optional loading photos → Evidence Panel.

**Gate:** Destination required by issue type policy.

---

### 9 — Final review

Display:

| Block |
|-------|
| Business Document |
| Requested Materials |
| Selected Packages (with issued / remaining) |
| Warehouse · Locations |
| Inventory Changes |
| Evidence |
| Warnings |
| AI Recommendations |
| **Manual Selection History** |

Operator confirms.

**Gate:** Explicit approve for Post.

---

### 10 — Posting

**Post** (not Save) automatically creates:

| Artifact |
|----------|
| Goods Issue document (Posted) — number via Numbering |
| Inventory Transaction(s) (immutable) — balance ↓ |
| Package quantity / volume / weight / status update (same Package ID) |
| Material movement · Warehouse history · Material history |
| Audit trail · Scan / validation / approval / **override** history |
| Evidence Archive + Document Library seal |
| Reservation clear (when applicable) |
| Genealogy update (MI consumed / linked to demand / FG path as rules allow) |

Nothing edited as a raw on-hand field outside the transaction. Reverse only via compensating transaction.

---

## Package Selection

Display complete package information in **Package Preview**:

| Field |
|-------|
| Package Number (display-only) |
| Material Identity |
| Species |
| Dimensions |
| Quality Grade |
| Moisture |
| Length · Width · Thickness |
| Available Pieces |
| Available Volume |
| Weight |
| Production Date |
| Current Warehouse |
| Current Location |
| Reservation Status |
| Customer Reservation |
| Photos |

Selection via scan of existing barcode/QR or Explorer pick — never typed Package Number.

---

## Partial Package Picking

Warehouse operators **shall be able to consume only part of a package**.

### Example (with disposition)

```text
Package 120
  → Picked 40
      → Good 37
      → Damaged 2
      → Scrap 1
  → Remaining 80
```

### Default behavior (same physical package)

The system **shall automatically update** on the **same** Package Identity:

| Field |
|-------|
| Remaining Quantity / Volume / Weight / Pieces |
| Package Status (Partially Used …) |
| Separate txns for Good · Damaged/Hold · Scrap |

```text
Original barcode / QR stays on the physical package.
No new barcode is printed for the remainder (default policy).
Scrap never returns into the package.
```

| Field | Example |
|-------|---------|
| Original | 120 |
| Picked | 40 (Good 37 + Damaged 2 + Scrap 1) |
| Remaining | 80 |
| Package Status | → Partially Used |

### Optional: company-policy package split

```text
If company policy requires,
the system may automatically split the package
while preserving complete traceability.
```

| Rule | Detail |
|------|--------|
| When | Configured company / plant policy only — not operator whim |
| How | Numbering Service mints **child** Package Identity for the remainder and/or issued handling unit (policy decides which side keeps the original barcode) |
| Traceability | Parent ↔ child package link · Inventory Transactions · Material Genealogy · audit — **complete chain** |
| Forbidden | Silent rename · barcode reuse · orphan remainder without link |

Default wood-yard policy: **no split** — one physical package keeps one barcode until fully consumed.  
Split policy is an explicit configuration, not the Workbench default.

### Inventory Transactions

Partial issue with disposition **shall** create Inventory Transaction(s) as needed:

```text
PKG-… (source)
  ├─ Good qty      → GI / demand txn (shipment or production)
  ├─ Damaged qty   → Damage Hold / Quality Hold txn + evidence
  ├─ Scrap qty     → Scrap txn + evidence
  └─ Remaining     → Available on same Package Identity
(+ child PKG link if policy split)
```

History is always preserved. Multiple Goods Issues may consume the same package over time until remaining reaches zero / Consumed / Closed (unless split moved remainder to a child package).

---

## AI Validation

**Even after Manuel Paket Seç and after grid edits**, AI **shall continue validating**.

### Per-package / demand checks

| Check |
|-------|
| Wrong Material |
| Wrong Dimension |
| Wrong Species |
| Wrong Quality |
| Wrong Customer Specification |
| Wrong Reservation |
| Wrong Lot |
| Wrong Moisture Class |
| Wrong Package |

### Allocation mix warnings (multi-package)

| Warning | Continue? |
|---------|-------------|
| Multiple Lots selected | Only if authorized |
| Different Quality Grades mixed | Only if authorized |
| Different Moisture Levels mixed | Only if authorized |
| Different Dimensions mixed | Only if authorized |
| Customer-specific picking rules violated | Only if authorized |

```text
The operator may override location / package selection and edit the allocation grid,
but may not violate business validation rules unless explicitly authorized.
```

| Path | AI Validation |
|------|----------------|
| Kabul Et | On — confirms allocation matches recommendation + demand |
| Grid modify / add / remove | On — re-validates mix + per-row rules |
| Manuel Paket Seç | **Still on** — validates manually chosen packages |

Authorization to waive = Supervisor / Quality / policy role · always audited · visible in Manual Selection History.

---

## Package Identity

| Property | Rule |
|----------|------|
| Package Identity | Immutable for that physical unit; child packages only via **policy split** (new ID, linked) |
| Package Barcode / QR | Never reused; default partial pick keeps original on physical package |
| Lifetime | While physical package exists |
| May change without new ID | Remaining Quantity · Volume · Weight · Pieces · **Status** |

### Package Status

Vocabulary per Design Standard § 10 (GI uses the full set):

| Status | Meaning |
|--------|---------|
| Available | Pickable |
| Reserved | Bound to demand |
| Picking | Active GI session |
| Partially Used | Remaining > 0 after one or more issues |
| Quality Hold | Blocked for Quality disposition |
| Damaged | Damaged classification / quarantine path |
| Consumed | Remaining = 0 (fully issued) |
| Closed | Business-closed (policy) |

Package ≠ Material Identity. Package **links to** MI (and may contain / represent MI units). See `Material_Identity_Architecture.md`.

---

## Traceability

**Authority:** Design Standard § 17 · `Material_Genealogy.md` · MI Architecture.

The system shall always reconstruct:

| Node | Answered by |
|------|-------------|
| Supplier | Receiving / PO link |
| Receiving Operation | Receiving root + txn history |
| Warehouse · Location | Stock position + txn |
| Package | Package Identity (unchanged on partial) |
| Production Order | GI → demand reference (when prod) |
| Finished Product | Genealogy |
| Shipment · Customer | SO / shipment chain |

```text
Receiving root MI → Lot / Package → Reservation (if any) → Goods Issue txn(s) →
Production Order / WO / SO → (later) FG / Customer shipment
```

```text
Complete genealogy is mandatory.
Traceability relies on Inventory Transactions and Material Genealogy.
NOT by changing Package IDs.
```

---

## Audit Trail

**Engine:** `Audit_Log.md` · **Composition:** Design Standard § 13.

Every GI action is recorded; nothing is overwritten. At minimum seal into audit:

| Event |
|-------|
| Created By / Created Date |
| Modified By / Modification Reason (draft) |
| AI Accept / Manuel Paket Seç (scan|search|browse) + Smart Scan confirm |
| Scan / verify / validation waiver |
| Disposition (Good / Damaged / Hold / Scrap / Rework) |
| Evidence attach |
| Electronic approval |
| Posting / Completion / Archive |

Manual Selection History (§ below) is the operator-facing slice of the same append-only truth.

---

## Revision Management

```text
Existing posted records shall never be edited silently.
Corrections create revisions and/or Reverse / Correction transactions.
```

Each revision stores: **Original Value · Corrected Value · Reason · User · Date · Approval** (when required).

Draft Workbench state may change until Post. After Post → § Immutable Transactions.

Compose with `Approval_Workflow.md` Return for Revision when approval is configured.

---

## Electronic Approvals

**Engine:** `Approval_Workflow.md` · Design Standard § 15.

Configurable electronic approvals on Goods Issue (examples):

| Role | Typical gate |
|------|----------------|
| Warehouse Operator | Execute pick / request Post |
| Supervisor | Validation waiver · shortage · override policy |
| Quality | Hold release · damage disposition path |
| Warehouse Manager | Manual Issue · high-value / exception Post |

Approval history is permanent and shown on Final Review + Document Library Timeline.

---

## Immutable Transactions

```text
Posted Inventory Transactions are never editable.
Mistake → Reverse Transaction and/or Correction Transaction.
Never modify history. Never Edit Posted GI as a form.
```

Aligns with `Inventory_Architecture.md`. Workbench UX after Post: open Reverse / Correction **new** controlled flow (permissioned), not silent field overwrite.

---

## Document Library & Evidence Archive

Every Goods Issue includes (surfaces on Workbench + INV-017 detail):

Document Library · Photo Gallery · Evidence Viewer · AI Analysis · OCR Results · Audit History · Timeline.

Support (by Shared law — do not redefine): Preview · Download · ZIP · Print · Search · Version History.

**Authority:** [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md).

---

## Export

Support Excel · CSV · PDF — **audit-ready** (Design Standard § 12 · Shared Evidence/Export).

Generate from library / reports (formats & columns → Shared law):

| Export |
|--------|
| Goods Issue Report |
| Inventory Transaction Report |
| Material Consumption Report |
| Picking Report / Picking List |
| Difference Report |
| Inventory History / Transaction History |

---

## Compliance

Compatible with ISO 9001 · ISO 14001 · ISO 45001 · ISO 27001 · FSC Chain of Custody · PEFC · TSE · Customer Quality Audits.

```text
Every GI session produces records suitable for internal and external audits:
digital file · Inventory Transactions · disposition evidence · approvals · genealogy.
```

FSC/PEFC claim rules live in Quality / CoC docs; GI **preserves** certificate evidence and MI/Package links without breaking the chain.  
Authority: Design Standard § 19.

---

## Manual Selection History

Append-only panel (Context + Final Review), sealed into audit on Post:

| Field |
|-------|
| When |
| Who |
| AI recommendation (WH · location · package · qty) |
| Operator selection |
| Method (scan · number search · optional browse) |
| Smart Scan outcome (ok · soft warning · hard fail / waiver) |
| Reason (if policy requires) |
| Authorization (if waiver) |

---

## Gates summary

| Stage | Gate |
|-------|------|
| 1 Document | Valid demand (or Manual + permission + reason) |
| 2 Materials | ≥1 open line with remaining qty |
| 3 AI | **Kabul Et** **or** logged **Manuel Paket Seç** (scan / search + Smart Scan) |
| 4 Picking | Scan OK · issue qty ≤ available · AI Validation clear (or authorized) |
| 5 Verify | AI Validation results green or audited waiver |
| 6 Evidence | Policy-required evidence present |
| 7 Quality | No block / hold / quarantine / expired without release |
| 8 Destination | Required destination assigned |
| 9 Review | Explicit approve · Manual Selection History visible |
| 10 Post | Creates txn(s) · updates package remaining · archive |

---

## AI features (summary)

Recommend (FIFO/FEFO/reservation/quality/WH rules/location/same lot/moisture/quality/production date/customer) · **Kabul Et** or **Manuel Paket Seç** · Smart Scan · AI Validation always on · shortage predict · pick route · repeated-manual coaching.

AI **recommends and validates**; operator **Kabul Et** or **Manuel Paket Seç** (selection only) — not business rules.

---

## Identity & Numbering

Operator **never invents**:

Goods Issue Number · Inventory Transaction Number · Warehouse Code · Location Code · Material Code · Lot Number · Material Identity strings · **new** Package Numbers.

All identifiers: Numbering Architecture.  
Existing Package: **scan preferred** · **Package / Barcode / QR number search** if unreadable · optional advanced browse.  
Partial pick default: **same Package ID**; policy split: Numbering mints **linked child** package(s) only.

---

## Roles

| Role | Focus |
|------|--------|
| Warehouse Operator | Scan · **Kabul Et** or **Manuel Paket Seç** · Smart Scan · partial qty · evidence · Post |
| Supervisor | Authorize validation waivers · unblock |
| Inventory Controller | Reservation / shortage · Manual GI · package-split policy config (with admin) |
| Quality | Holds / release (blocks issue until clear) |

---

## Mobile / Terminal

Rugged Terminal: **Smart Scan** + pick/verify/partial qty inside the same GI session (primary device for Manuel Paket Seç).  
Tablet / desk: document select · Kabul Et / Manuel Paket Seç · review · Document Library · Post.

---

## Cursor implementation notes

1. Screen type = **Workbench** — not Issue Create form · COMPLIANCE BY DESIGN.  
2. Compose `Inventory_Workbench_Design_Standard.md` § 5–5b — **Manuel Paket Seç** + **Smart Scan**.  
3. After rejecting AI: **do not** open Warehouse Explorer by default — show Scan vs Package Number Search.  
4. Smart Scan one-screen confirm → **Paketi Kullan** adds to Package Allocation Workspace.  
5. FE: **Package Allocation Workspace** MAIN + **Take From Package** rail (Good/Damaged/Scrap/Hold/Rework).  
6. Live strip + disposition totals; Damaged/Scrap require evidence before Post.  
7. Package Closing Checklist after partial take.  
8. Post → immutable GI (Good) + Hold txn(s) + Scrap txn(s) + remaining on same PKG · evidence · genealogy · audit seal.  
9. Corrections after Post = Reverse / Correction flows — never Edit Posted.  
10. Approvals / audit → shared engines; surface Manual Selection History on Review.  
11. Shared PAW / Evidence / Numbering / MI — reference only; do not redefine algorithms.  
12. Command Center “Open issues” opens this Workbench.

---

## Related

`INV_Issue_Wizard.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Inventory_Screens.md`  
`Inventory_Design_Program.md` · [`Inventory_Workbench_Design_Standard.md`](../../13_Design/99_Shared/Inventory_Workbench_Design_Standard.md)  
`Material_Identity_Architecture.md` · `Document_Numbering.md` · `Package_Allocation_Workspace.md`  
`Document_Management_Evidence_and_Export.md` · `Audit_Log.md` · `Approval_Workflow.md`  
`Inventory_Dashboard.md` · `Barcode_QR_Model.md`
