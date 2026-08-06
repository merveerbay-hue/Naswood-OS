# PRD-PLAN-001 — Production Planning Wizard

**Module:** Production  
**Workspace:** Planning  
**Screen type:** Job / Process (Wizard)  
**Status:** Product Architect draft — Phase 2 exemplar  
**Replaces as primary Planning entry:** “Production Order” / “New Production Order” entity create  
**Related flow:** `docs/13_Design/05_Production/Production_User_Flows.md` · FLOW-001 (supersedes step list)

---

## 1. Job to be done

> Planlamacı, satılabilir / üretilebilir bir ürün için **üretime hazır bir planı** kurar; hammadde, hat, kapasite, termin ve maliyet doğrulanır; yetkiye bağlı **Onay ve Release** ile işi bitirir.

**Finished when:** Production Order (ve gerekirse ilk Work Order paketi) **Released** durumundadır — veya taslak olarak kaydedilmiştir.

**Not the job:** “Production Order kaydı oluşturmak.” Kayıt, sürecin yan ürünüdür.

---

## 2. Role & entry

| | |
|--|--|
| **Primary role** | Production Planner |
| **Secondary** | Production Manager (approve / release) |
| **Starts from** | Production → Planning → **Plan production** (primary CTA) |
| **Also from** | Sales Order “Plan” action · Dashboard capacity alert · Duplicate plan |
| **Library (secondary)** | Released / Draft plans list — *find & reopen*, not the design center |

---

## 3. Market reference (patterns)

| Suite | Typical packaging |
|-------|-------------------|
| **SAP PP** | Planned order → convert; MRP; capacity (CM25); release |
| **IFS** | Shop order creation with structure/routing; material availability; scheduling |
| **D365** | Production order create guided + scheduling workspace |
| **Infor / Opcenter** | Plan → schedule → release; often wizard or guided panel for complex make-to-order |

**NOS better (timber-native):** Wizard steps include **ölçü**, **ağaç türü**, **revizyon** before generic BOM pick — so the planner finishes a *wood manufacturing plan*, not an abstract order header.

---

## 4. Wizard steps

```text
Production Planning Wizard
├── 1. Ürün seçimi
├── 2. Revizyon seçimi
├── 3. Ölçü seçimi
├── 4. Ağaç türü seçimi
├── 5. Hammadde uygunluğu
├── 6. Hat seçimi
├── 7. Kapasite kontrolü
├── 8. Termin planı
├── 9. Maliyet simülasyonu
└── 10. Onay ve Release
```

Each step: **intent · inputs · system checks · gate · UI regions · components**.

---

### Step 1 — Ürün seçimi

| | |
|--|--|
| **Intent** | Hangi satılabilir / üretilebilir ürün planlanıyor? |
| **Inputs** | Product (`PDT-*`), plant, quantity, UoM, demand ref (SO / forecast / manual) |
| **System** | Active products only; plant-allowed; ATP hint (soft) |
| **Gate** | Product + quantity > 0 |
| **UI** | Search, recent products, product card (family, default UoM) |
| **Components** | Entity picker, Metric Card (open demand) |

---

### Step 2 — Revizyon seçimi

| | |
|--|--|
| **Intent** | Hangi ürün / BOM / routing revizyonu geçerli? |
| **Inputs** | BOM revision, Routing revision (linked or selectable) |
| **System** | Default = effective revision for date; show alternatives |
| **Gate** | Valid BOM + Routing pair for plant |
| **UI** | Revision list with effective dates, “recommended” badge |
| **Components** | Master Detail (revision summary), Status Badge |

---

### Step 3 — Ölçü seçimi

| | |
|--|--|
| **Intent** | Sipariş / reçete ölçüleri (kalınlık × genişlik × boy, paket, adet) netleşsin. |
| **Inputs** | Dimension set / cut list profile / package size (timber-aware) |
| **System** | Validate against product dimension rules; yield hint |
| **Gate** | Required dimensions complete per product family rules |
| **UI** | Dimension form; presets from Product; yield preview |
| **Components** | Attribute Panel, Calculator strip |

---

### Step 4 — Ağaç türü seçimi

| | |
|--|--|
| **Intent** | Species / grade (ve gerekirse nem sınıfı) planı kilitle. |
| **Inputs** | Species, grade, optional moisture class |
| **System** | Allowed combinations for product; substitute rules |
| **Gate** | Species (+ grade if required) selected |
| **UI** | Species/grade pickers; constraint messages |
| **Components** | Attribute Panel, Constraint Alert |

---

### Step 5 — Hammadde uygunluğu

| | |
|--|--|
| **Intent** | Planlanan miktar için hammadde / yarı mamul **karşılanabilir mi?** |
| **Inputs** | Auto-exploded BOM for revision + dimensions + species |
| **System** | On-hand, reserved, incoming PO, shortage list; substitute suggestions |
| **Gate** | Soft: shortages warn. Hard (policy): block release if critical shortage |
| **UI** | Component grid: required / available / shortage / action (PR / transfer) |
| **Components** | Entity Grid, Availability Panel, Alert List |
| **Integrations** | Inventory (balance, reservation), Purchasing (raise PR) |

---

### Step 6 — Hat seçimi

| | |
|--|--|
| **Intent** | Hangi production line / work center zinciri üretecek? |
| **Inputs** | Line, primary work centers (from routing defaults) |
| **System** | Routing-compatible lines; current load preview |
| **Gate** | At least one valid line / WC set |
| **UI** | Line cards with utilization sparkline |
| **Components** | Card picker, Mini capacity chart |

---

### Step 7 — Kapasite kontrolü

| | |
|--|--|
| **Intent** | Seçilen hat/WC üzerinde plan **sığar mı?** |
| **Inputs** | Horizon, shift calendar, finite/infinite policy |
| **System** | Load vs capacity; bottleneck WC; conflict list |
| **Gate** | Soft warn on overload; Manager may override with reason |
| **UI** | Load chart by day/shift; bottleneck highlight |
| **Components** | Scheduler strip, Capacity chart, Alert List |
| **Integrations** | Production Calendar, Shifts, Maintenance downtime |

---

### Step 8 — Termin planı

| | |
|--|--|
| **Intent** | Start / finish / promise date net; müşteri veya iç termin ile uyum. |
| **Inputs** | Requested due date, scheduling direction (forward/backward) |
| **System** | Propose start/finish from capacity; slack vs due date |
| **Gate** | Feasible schedule or explicit risk accept |
| **UI** | Date fields + Gantt snippet for this order |
| **Components** | Scheduler, Date field, Risk Badge |

---

### Step 9 — Maliyet simülasyonu

| | |
|--|--|
| **Intent** | Planın tahmini maliyeti görünür olsun (karar için). |
| **Inputs** | Material, labor, machine, overhead (from cost rolls) |
| **System** | Simulated cost vs standard / last actual; margin vs sales price if SO-linked |
| **Gate** | Optional hard gate if margin below policy (Manager) |
| **UI** | Cost breakdown cards; variance vs standard |
| **Components** | Metric Card, Breakdown list |
| **Integrations** | Finance costing |

---

### Step 10 — Onay ve Release

| | |
|--|--|
| **Intent** | Planı **bitir**: kaydet (Draft) veya **Release**. |
| **Inputs** | Summary of steps 1–9; notes; approver if dual-control |
| **System** | Create/update Production Order; explode WO if policy; reserve materials if policy; emit events |
| **Gate** | All hard gates passed; `Production.Release` permission |
| **UI** | Read-only summary; **Save draft** · **Submit for approval** · **Release** |
| **Components** | Wizard summary, Approval Bar |
| **Outcome states** | `Draft` · `PendingApproval` · `Released` |

---

## 5. Wizard chrome (all steps)

| Element | Behavior |
|---------|----------|
| Stepper | 10 steps; completed / current / locked |
| Context header | Product · qty · due · plant (sticky) |
| Exit | Confirm discard if dirty |
| Save draft | Allowed from step 5+ (policy) |
| Permissions | Step 10 Release may require Manager |

**Primary component:** Wizard (shared library).

---

## 6. Secondary screens (not the job center)

| Screen | Role vs Wizard |
|--------|----------------|
| Plan / Order Library (list) | Find draft/released plans; **Open in wizard** or **Open detail** |
| Production Order Detail | Post-release monitoring, documents, genealogy — not create path |
| Scheduling Board | Multi-order balancing after/beside wizard |
| Capacity Board | Plant-wide; wizard embeds a slice at steps 7–8 |

---

## 7. Permissions (sketch)

| Action | Permission |
|--------|------------|
| Open wizard / save draft | `Production.Planning.Create` |
| Override capacity / shortage | `Production.Planning.Override` |
| Submit approval | `Production.Planning.Submit` |
| Release | `Production.Release` |
| Cancel released | `Production.Cancel` (separate job) |

---

## 8. Events (outcome)

- `ProductionPlanDraftSaved`
- `ProductionPlanSubmitted`
- `ProductionOrderReleased`
- `MaterialReservationRequested` *(if policy)*
- `WorkOrderPackageGenerated` *(if policy)*

---

## 9. Explicitly not this screen

- Operator start/complete (Operator Terminal)
- Shop-floor scrap/rework
- Master data BOM edit (BOM Detail job)
- Full plant scheduling of all orders (Scheduling Board)

---

## 10. Cursor implementation note

When implementing:

1. Build **Wizard steps 1→10**, not a single `ProductionOrder` create form.  
2. Persist draft between steps.  
3. List library is secondary navigation.  
4. Do **not** title the feature “Production Order CRUD”.

---

## 11. Product Architect checklist

- [x] Job named (not entity)  
- [x] Steps mirror real timber planning  
- [x] Gates & integrations listed  
- [x] Library vs wizard separated  
- [ ] Role walkthrough with Production Manager (session)  
- [ ] Align `Production_Screens.md` Planning section IDs (`PRD-101` → wizard-first)  
- [ ] Align `15_UI` PRD-010/011 family to wizard + library split  
