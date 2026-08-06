# NOS Screen Types

**Document:** Screen Types (UX law)  
**Status:** Official  
**Version:** 1.2.0  
**Location:** `docs/13_Design/Common/Screen_Types.md`  
**Companion:** [`UI_Patterns.md`](./UI_Patterns.md)  
**Process screens:** [`docs/00_Product/Process_Screens/`](../../00_Product/Process_Screens/)  
**SSOT matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

---

## 1. Absolute rules

```text
NOS'ta "New" / "Create" diye tek tip ekran YOKTUR.
NOS'ta Master Data ekranları "Create Form" değildir.
Kullanıcı kodlarla değil isimlerle çalışır; teknik kimlikler Numbering Service ile üretilir.
```

Master Data screens are **Designer · Builder · Planner · Configuration · Workbench** surfaces.  
The user does not merely create a record — they **define, validate, relate, simulate, and Release** production (or domain) knowledge.

**Identifiers:** Authority `Document_Numbering.md` § System Generated Identifiers · Constitution § 2.3

| Wrong | Right |
|-------|--------|
| Shared `Create` ResourcePage for every entity | Screen type chosen per job |
| `Code *` ________ | System Code — auto after save / on Release (read-only info) |
| Product Code typed in picker | **Ürün** 🔍 Thermowood Deck 26×140×3000 (name-first) |
| Machine Code · Name · Save | **Machine Configuration** (business facets → Release; `MC-…` auto) |
| BOM Code · Description · Save | **BOM Builder** → Release |
| Entity → Form | **Business Object → Business Workspace → Business Designer** |

Reusable **components** (grid, filter, stepper, tree, canvas) are shared.  
Reusable **CRUD create/edit screens** are forbidden as the default — for **operations and master/engineering data**.

---

## 2. Screen type catalog

### 2a. Transaction & operations

| Screen type | Purpose | Typical jobs |
|-------------|---------|--------------|
| **Wizard** | Finish a multi-step business transaction with gates | Production Planning, Goods Receipt, Maintenance WO, NCR, Purchase Order |
| **Console** | Run a continuous operational desk (many short actions) | Receiving desk, Shipping desk, Shop-floor supervisor console |
| **Terminal** | Execute a single focused operational job (often touch / scan) | Operator Terminal, Receiving scan terminal, Shipping terminal |
| **Approval Center** | Decide pending approvals across documents | PO / NCR / Release / Adjustment approvals |
| **Dashboard** | See status, KPIs, exceptions; drill into jobs | Module / executive cockpits |

### 2b. Engineering & master knowledge (not Create Form)

| Screen type | Purpose | Typical jobs |
|-------------|---------|--------------|
| **Builder** | Construct a structured engineering object (tree / structure) with validate & Release | BOM Builder, Package structure |
| **Designer** | Design a process / graph / operation sequence with relations & simulation | Routing Designer, Operation Designer, Work Center Layout Designer |
| **Configuration** | Define a rich multi-facet asset/resource (Studio facets → Release) | Machine Configuration Studio, Line Designer, Product Wizard |
| **Planner** | Balance time / capacity / resources visually | Capacity, Scheduling, Dispatch, Shift Planner, Calendar Planner, PM calendar |
| **Workbench** | Knowledge / analysis / multi-object engineering surface | Quality engineering, CAPA analysis, genealogy inquiry |
| **Library** | Find & reopen engineering objects — **not** a Create Form | Tool Library Manager, Machine Library → opens Studio |
| **Explorer** | Find, inspect, navigate hierarchical / catalog data | Warehouse map, Lot library, Product catalog browse |

```text
Library / Explorer  =  find & reopen
Builder / Designer / Configuration / Planner  =  define knowledge → Release
```

A Library list CTA never opens “Code · Name · Save”. It opens the matching **Builder / Designer / Configuration**.

---

## 3. Operational “Yeni” examples

### Inventory — not “New GoodsReceipt entity”

**CTA:** Receive goods / Mal kabul başlat · **Type:** Wizard  
Full: `docs/00_Product/Process_Screens/INV_Receiving_Wizard.md`

### Production planning — not “New ProductionOrder entity”

**CTA:** Plan production / Üretim planla · **Type:** Wizard  
Full: `docs/00_Product/Process_Screens/PRD_Production_Planning_Wizard.md`

### Quality / Maintenance

**Raise NCR** → Wizard · `QLT_NCR_Wizard.md`  
**Open work order** → Wizard · `MNT_Work_Order_Wizard.md`

---

## 3a. Master Data / Engineering examples (Production)

### Not “New Machine” (Code · Name · Save)

**CTA:** Configure machine / Makine yapılandır  
**Type:** **Configuration** (Studio)  
**Screen:** PRD-503 Machine Configuration Studio

```text
Genel Bilgiler → Teknik → Kapasiteler → Operasyonlar → Tool Magazine
  → Bakım → Sensörler → Dokümanlar → IoT → Devreye Alma → Release
```

Full: `docs/00_Product/Process_Screens/PRD_Machine_Configuration.md`  
Code `MC-…` auto — never `Code *` input.

### Not “New BOM” (Code · Description · Save)

**CTA:** Build BOM / BOM oluştur  
**Type:** **Builder**  
**Screen:** PRD-501 BOM Builder

```text
Ürün → Revizyon → Malzeme Ağacı → Alternatifler → Fire
  → Operasyon Bağlantıları → Versiyon Karşılaştırma → Etki Analizi → Onay → Release
```

Full: `docs/00_Product/Process_Screens/PRD_BOM_Builder.md`  
Code `BOM-…` auto.

### Production engineering matrix (authoritative)

| Entity (data) | Forbidden UI | Screen | Type | Auto ID |
|---------------|--------------|--------|------|---------|
| BOM | BOM CRUD | **PRD-501 BOM Builder** | Builder | `BOM-…` |
| Routing | Routing CRUD | **PRD-502 Routing Designer** | Designer | `RT-…` |
| Machine | Machine CRUD | **PRD-503 Machine Configuration Studio** | Configuration | `MC-…` |
| Work Center | WC CRUD | **PRD-504 Work Center Designer** | Designer | `WC-…` |
| Production Line | Line CRUD | **PRD-505 Line Designer** | Designer | `LINE-…` |
| Operation | Operation CRUD | **PRD-506 Operation Designer** | Designer | `OP-…` |
| Shift | Shift CRUD | **PRD-507 Shift Planner** | Planner | `SHIFT-…` |
| Calendar | Calendar CRUD | **PRD-508 Calendar Planner** | Planner | `CAL-…` |
| Tooling | Tooling CRUD | **PRD-509 Tool Library Manager** | Library → Configuration | `TL-…` |

Principle (full text): `Production_Screens.md` § **ENGINEERING MASTER DATA PRINCIPLE**.  
None are Create · Edit · Delete screens. No editable Code fields.

---

## 3b. Create → Job CTA matrix (operations)

Every operational “create” intent maps to a **job CTA** + **screen type**.  
Shared `ResourcePage` / `EntityListScreen` create panels are **forbidden** for rows marked Wizard / Terminal / Console / Builder / Designer / Configuration.

| Module | Forbidden CTA | Correct CTA (TR / EN) | Type | Process / Screens |
|--------|---------------|----------------------|------|-------------------|
| Inventory | Yeni kabul / Create GR | **Mal kabul başlat** / Receive goods | Wizard | [`INV_Receiving_Wizard.md`](../../00_Product/Process_Screens/INV_Receiving_Wizard.md) |
| Inventory | Yeni çıkış | **Mal çıkışı** / Issue goods | Wizard | [`INV_Issue_Wizard.md`](../../00_Product/Process_Screens/INV_Issue_Wizard.md) |
| Inventory | Yeni transfer | **Stok transfer** / Transfer stock | Wizard | [`INV_Transfer_Wizard.md`](../../00_Product/Process_Screens/INV_Transfer_Wizard.md) |
| Inventory | Yeni sayım | **Sayım başlat** / Start count | Wizard | [`INV_Cycle_Count_Session.md`](../../00_Product/Process_Screens/INV_Cycle_Count_Session.md) |
| Inventory | Yeni düzeltme | **Düzeltme onayla** / Post adjustment | Approval / Workbench | `Inventory_Screens.md` |
| Inventory | Yeni malzeme | **Malzeme tanımla** (Configuration — not Code·Name·Save) | Configuration | Inventory Screens *(evolve)* |
| Production | Yeni emir | **Üretim planla** / Plan production | Wizard | [`PRD_Production_Planning_Wizard.md`](../../00_Product/Process_Screens/PRD_Production_Planning_Wizard.md) |
| Production | Yeni BOM / makine / rota… | See **§ 3a engineering matrix** | Builder / Designer / Configuration | `Production_Screens.md` |
| Production | Yeni iş emri | **İş emri aç / Dispatch** | Planner / Terminal | `Production_Screens.md` |
| Quality | Yeni NCR | **NCR aç** / Raise NCR | Wizard | [`QLT_NCR_Wizard.md`](../../00_Product/Process_Screens/QLT_NCR_Wizard.md) |
| Maintenance | Yeni iş emri | **İş emri aç** / Open work order | Wizard | [`MNT_Work_Order_Wizard.md`](../../00_Product/Process_Screens/MNT_Work_Order_Wizard.md) |
| Purchasing | Create PO | **Sipariş ver** / Place purchase order | Wizard | [`PUR_Purchase_Order_Wizard.md`](../../00_Product/Process_Screens/PUR_Purchase_Order_Wizard.md) |
| Sales | + New Sales Order | **Sipariş gir** / Enter sales order | Wizard | [`SAL_Sales_Order_Wizard.md`](../../00_Product/Process_Screens/SAL_Sales_Order_Wizard.md) |
| Platform | Create user | **Kullanıcı ekle** / Add user | Explorer / Admin | Administration |

Historical `docs/14_Implementation/**` TASK wireframes that say `+ New` / `Create X` are **not authority**.

---

## 4. CTA naming

| Avoid | Prefer (examples) |
|-------|-------------------|
| Yeni / Create / New / Save | **Plan production** · **Receive goods** · **Build BOM** · **Configure machine** · **Design routing** · **Release** |
| Create · Edit · Delete as the product | Library find + Designer/Builder **Release** |

Finish actions for engineering: **Validate** · **Submit for approval** · **Release** (not bare Save).

---

## 4b. Identifier fields (never editable Code *)

**Authority:** `Document_Numbering.md` § System Generated Identifiers

```text
❌  Code *
    _______________

✅  System Code / Identifier
    Automatically generated after save
    (or on Release — process-dependent)
```

Pickers are **name-first**. Internal IDs and codes are not user input.  
Any generated Create UI with an editable Code/Number/Lot field is a **defect**.

---

## 5. How Cursor must choose a screen type

```text
1. What job is the user finishing?          (JOB_FIRST_SCREEN_DESIGN)
2. Is this a transaction or engineering knowledge?
3. Which screen type fits?                  (this document §2)
4. What sections / steps / gates?           (module Screens + Process_Screens)
5. Which laws are reference-only?           (Authority Matrix)
```

If step 2–3 are skipped → defaulting to Entity Grid + Create form is a **defect**.  
If Master Data is generated as Code · Name · Save → **defect**.

Mental model:

```text
✘  Entity  →  Form
✔  Business Object  →  Business Workspace  →  Business Designer
```

---

## 6. Type vs component vs entity

| Layer | Shared across modules? | Customized per process? |
|-------|------------------------|-------------------------|
| Screen **type** | Pattern yes | Steps, facets, CTAs **always** process-specific |
| **Components** (Grid, Tree, Canvas, Stepper, File) | Yes | Configuration / slots |
| **Entity** (Machine, BOM, PO) | Data only | Never defines the screen alone |

---

## 7. Authority

This file is the **authority for screen type selection** (including Master Data ≠ Create Form).  
Module Screens / User Flows **reference** a type and define process steps — they do not invent a parallel “Create” pattern.

Related:

- [`UI_Patterns.md`](./UI_Patterns.md) — pattern anatomy (Builder / Designer / Configuration)  
- [`JOB_FIRST_SCREEN_DESIGN.md`](../../00_Product/JOB_FIRST_SCREEN_DESIGN.md)  
- `Production_Screens.md` — PRD-501…509 engineering surfaces  
- `Document_Numbering.md` — identity minting (reference only)
