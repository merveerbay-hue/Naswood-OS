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

**NOS better (timber-native):** Wizard carries **ölçü + detay çizim + kesit** as a first-class *technical package* (not an afterthought attachment), and can **print a shop packet** on release — so the floor gets dimensions and drawings with the order.

File storage / versioning: reference only → `docs/13_Design/00_Platform/File_Upload.md`  
Screen type: **Wizard** → `docs/13_Design/Common/Screen_Types.md`

---

## 4. Wizard steps

```text
Production Planning Wizard
├── 1. Ürün seçimi
├── 2. Revizyon seçimi
├── 3. Ölçü seçimi
├── 4. Teknik paket (detay çizim · kesit · ekler)
├── 5. Ağaç türü seçimi
├── 6. Hammadde uygunluğu
├── 7. Hat seçimi
├── 8. Kapasite kontrolü
├── 9. Termin planı
├── 10. Maliyet simülasyonu
└── 11. Onay, Release ve çıktı
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
| **Intent** | Sipariş / reçete ölçüleri netleşsin; saha ve kesim bu sayılara güvensin. |
| **Inputs** | Kalınlık × genişlik × boy; adet / paket; kesim listesi (cut list) satırları; tolerans |
| **System** | Product dimension rules; yield hint; optional link to SO dimension lines |
| **Gate** | Zorunlu ölçü alanları dolu (ürün ailesi kuralına göre) |
| **UI** | Ölçü formu; preset’ler; cut-list grid; özet şerit |
| **Components** | Attribute Panel, Calculator strip, Entity Grid (cut list) |
| **Print later** | Ölçü kartı / cut list sayfası shop packet’e girer (§5) |

---

### Step 4 — Teknik paket (detay çizim · kesit · ekler)

| | |
|--|--|
| **Intent** | Üretim emriyle birlikte **görsel / teknik bağlam** tamamlansın — sadece sayı değil, çizim ve kesit de taşınsın. |
| **Inputs** | |
| | • **Detay çizim** (PDF / DWG / DXF / görüntü — müşteri veya mühendislik) |
| | • **Kesit** (cross-section) çizimi veya şablon |
| | • Opsiyonel: montaj / profil / etiket şablonu, not sayfası |
| | • Her ek için: tip etiketi (`DetailDrawing` · `CrossSection` · `Other`), açıklama, revizyon notu |
| **System** | Dosyalar Platform File Upload / DMS’e bağlanır (`File_Upload.md`); plan/emir kaydına **link** edilir — dosya Production’da kopyalanmaz |
| **Gate** | Policy: ürün ailesi “çizim zorunlu” ise en az bir `DetailDrawing`; “kesit zorunlu” ise en az bir `CrossSection`. Aksi halde soft warn |
| **UI** | Ek listesi + sürükle-bırak; tip seçici; önizleme (PDF/görüntü); “ürün varsayılan çizimini getir” |
| **Components** | Attachment Panel, Drawing / PDF Preview, File picker |
| **Integrations** | Platform File Upload · Product default drawings (optional) |

Teknik paket, Release sonrası **Production Order Detail → Documents / Technical package** altında da yönetilir (ekleme / yeni revizyon).

---

### Step 5 — Ağaç türü seçimi

| | |
|--|--|
| **Intent** | Species / grade (ve gerekirse nem sınıfı) planı kilitle. |
| **Inputs** | Species, grade, optional moisture class |
| **System** | Allowed combinations for product; substitute rules |
| **Gate** | Species (+ grade if required) selected |
| **UI** | Species/grade pickers; constraint messages |
| **Components** | Attribute Panel, Constraint Alert |

---

### Step 6 — Hammadde uygunluğu

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

### Step 7 — Hat seçimi

| | |
|--|--|
| **Intent** | Hangi production line / work center zinciri üretecek? |
| **Inputs** | Line, primary work centers (from routing defaults) |
| **System** | Routing-compatible lines; current load preview |
| **Gate** | At least one valid line / WC set |
| **UI** | Line cards with utilization sparkline |
| **Components** | Card picker, Mini capacity chart |

---

### Step 8 — Kapasite kontrolü

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

### Step 9 — Termin planı

| | |
|--|--|
| **Intent** | Start / finish / promise date net; müşteri veya iç termin ile uyum. |
| **Inputs** | Requested due date, scheduling direction (forward/backward) |
| **System** | Propose start/finish from capacity; slack vs due date |
| **Gate** | Feasible schedule or explicit risk accept |
| **UI** | Date fields + Gantt snippet for this order |
| **Components** | Scheduler, Date field, Risk Badge |

---

### Step 10 — Maliyet simülasyonu

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

### Step 11 — Onay, Release ve çıktı

| | |
|--|--|
| **Intent** | Planı **bitir** ve sahaya / arşive **çıktı** verebil. |
| **Inputs** | Summary of steps 1–10; notes; approver if dual-control; **print options** |
| **System** | Create/update Production Order; explode WO if policy; reserve materials if policy; emit events; queue print jobs |
| **Gate** | All hard gates passed; `Production.Release` permission; technical-package policy gates from step 4 |
| **UI** | Read-only summary (ölçü + teknik paket küçük önizleme); **Save draft** · **Submit for approval** · **Release** · **Release & print shop packet** |
| **Components** | Wizard summary, Approval Bar, Print options checklist |
| **Outcome states** | `Draft` · `PendingApproval` · `Released` |

---

## 5. Teknik paket & çıktı (print / export)

Ölçü + çizim + kesit, emrin **üretime giden dili**dir. Release yalnızca durum değiştirmez; istenirse **shop packet** üretir.

### 5.1 Technical package contents

| Artifact | Role | Typical formats |
|----------|------|-----------------|
| Dimension / cut list | Kesim ve kontrol ölçüleri | Structured fields + PDF sayfası |
| Detail drawing | Detay çizim | PDF, DWG, DXF, PNG/JPG |
| Cross-section (kesit) | Kesit görünümü | PDF, DWG, DXF, PNG/JPG |
| Other | Montaj, etiket şablonu, müşteri notu | PDF, image |

Storage authority: Platform **File Upload / DMS** — Production only stores **links + typed roles** on the plan/order.

### 5.2 Print / export jobs (selectable)

| Output | Includes | When |
|--------|----------|------|
| **Shop packet (PDF)** | Cover (PO id, product, qty, due) · Ölçü / cut list · Detay çizim · Kesit · Routing özeti · Hat / termin | Release veya Detail → Print |
| **Dimension card** | Ölçü + tolerans (+ barkod/emir no) | Ayrı veya packet içinde |
| **Drawing set** | Yalnız çizim + kesit sayfaları | Atölye / tedarikçi |
| **Work order sheet** | WO listesi + op özeti (policy) | Release sonrası |
| **Labels** | Emir / paket etiketleri | Barcode strategy — reference `Barcode_Strategy.md` |

Actions (CTA — not “Create”):

- **Print shop packet** / **Saha paketi yazdır**  
- **Export PDF**  
- **Download drawings**  

Operator Terminal and Dispatch may **open/print** the same packet (read-only); they do not author drawings.

### 5.3 Permissions (print / package)

| Action | Permission |
|--------|------------|
| Attach / replace technical files | `Production.Planning.Attachments` |
| Print shop packet | `Production.Print.ShopPacket` |
| Print labels | `Production.Print.Labels` |

---

## 6. Wizard chrome (all steps)

| Element | Behavior |
|---------|----------|
| Stepper | 11 steps; completed / current / locked |
| Context header | Product · qty · due · plant · “teknik paket: n ek” (sticky) |
| Exit | Confirm discard if dirty |
| Save draft | Allowed from step 4+ (policy) — keeps files linked |
| Permissions | Step 11 Release may require Manager |

**Primary component:** Wizard (shared library).

---

## 7. Secondary screens (not the job center)

| Screen | Role vs Wizard |
|--------|----------------|
| Plan / Order Library (Explorer) | Find draft/released plans; **Open in wizard** or **Open detail** |
| Production Order Detail | Post-release: Overview · **Technical package** · Materials · Routing · Schedule · History; **Print shop packet** |
| Scheduling / Capacity boards | Multi-order balancing; open packet read-only |

---

## 8. Permissions (sketch)

| Action | Permission |
|--------|------------|
| Open wizard / save draft | `Production.Planning.Create` |
| Technical package attach | `Production.Planning.Attachments` |
| Override capacity / shortage | `Production.Planning.Override` |
| Submit approval | `Production.Planning.Submit` |
| Release | `Production.Release` |
| Print shop packet / labels | `Production.Print.*` |
| Cancel released | `Production.Cancel` (separate job) |

---

## 9. Events (outcome)

- `ProductionPlanDraftSaved`
- `ProductionPlanSubmitted`
- `ProductionOrderTechnicalPackageUpdated`
- `ProductionOrderReleased`
- `ProductionShopPacketRequested`
- `MaterialReservationRequested` *(if policy)*
- `WorkOrderPackageGenerated` *(if policy)*

---

## 10. Explicitly not this screen

- Operator start/complete (Operator Terminal)  
- Authoring CAD in-app (external CAD → upload)  
- Master data BOM structure edit  
- Full plant scheduling of all orders  

---

## 11. Cursor implementation note

When implementing:

1. Build **Wizard steps 1→11**, not a single `ProductionOrder` create form.  
2. Step 4 = typed attachments via Platform file APIs — do not reinvent DMS.  
3. Step 11 offers **Release & print shop packet** (PDF compose from ölçü + drawings).  
4. Detail screen reuses the same technical package + print actions.  
5. Do **not** title the feature “Production Order CRUD”.

---

## 12. Product Architect checklist

- [x] Job named (not entity)  
- [x] Steps mirror timber planning **including çizim / kesit**  
- [x] Print / shop packet defined  
- [x] File storage referenced (not redefined)  
- [x] Library vs wizard separated  
- [ ] Role walkthrough with Production Manager (session)  
- [ ] FE: Wizard + Attachment Panel + PDF packet compose  

