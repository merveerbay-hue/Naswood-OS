# Job-First Screen Design

**Status:** Official — Product Architect Drive  
**Version:** 1.0.0  
**Authority:** [`04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)

---

## 1. The only screen question

Before naming or drawing any screen, answer:

> **Kullanıcı bu ekranda hangi işi bitirmek istiyor?**

Then:

> **Bu iş hangi ekran tipini kullanır?** (Wizard · Builder · Designer · Configuration · Terminal · Planner · …)

Authorities: [`docs/13_Design/Common/Screen_Types.md`](../13_Design/Common/Screen_Types.md) · [`UI_Patterns.md`](../13_Design/Common/UI_Patterns.md)

The answer is the screen — **not** a shared Create form.

```text
NOS'ta "New" diye tek tip ekran yoktur.
NOS'ta Master Data ekranları "Create Form" değildir.
```

Not:

```text
✘ Entity oluştur / ortak “Yeni” formu
✘ Machine Code · Name · Save
✘ BOM Code · Description · Save
✘ Production Order CRUD
✘ “TASK-056 ekranı”
✘ Tablo + Create formu (her modülde aynı)
✘ Entity → Form
```

But:

```text
✔ Business Object → Business Workspace → Business Designer
✔ Production Planning Wizard — planı oluştur, doğrula, release et
✔ Receiving Wizard — PO → miktar → depo → lokasyon → lot (otomatik) → kalite → post
✔ BOM Builder — ürün → ağaç → alternatif → fire → onay → Release
✔ Machine Configuration — kimlik → yerleşim → teknik → yetenek → bakım → doküman → Release
✔ Routing Designer — operasyon → makine → tool → simülasyon → Release
✔ Operator Terminal — iş emrini başlat / teyit / hurda
✔ Cycle Count Session — sayımı bitir ve farkı kapat
✔ NCR Wizard — kaynak → problem → CAPA
```

---

## 2. Entity vs Job screen

| Entity-shaped (forbidden default) | Job-shaped (required default) |
|-----------------------------------|-------------------------------|
| Production Order | **Production Planning Wizard** |
| Work Order | **Dispatch & Start Shift** / Operator Run |
| Goods Receipt | **Receive Against PO / Production** |
| NCR | **Raise & Disposition Non-Conformance** |
| Machine (Code·Name·Save) | **Machine Configuration** |
| BOM (Code·Description·Save) | **BOM Builder** |
| Routing CRUD | **Routing Designer** |
| Work Center CRUD | **Work Center Designer** |
| Asset | **Register Asset & Maintenance Profile** *(Configuration — allowed when job is master knowledge)* |

Master / Engineering primary surfaces are **Builder / Designer / Configuration / Planner** — not List + Create Form.  
Entity **libraries** exist to **find and reopen** work, then open the Designer.

---

## 3. Screen design template (mandatory)

Copy for every new screen:

```text
Screen ID:
Screen name:          <Job name — verb/outcome>
Screen type:          <Wizard | Terminal | Console | Explorer | Planner | Dashboard | Workbench | Approval Center>
Workspace:
Primary role:
Primary CTA:          <Plan production | Receive goods | Raise NCR | … — never bare “Yeni/Create”>

Job to be done:
  <One sentence — what is finished when the user leaves>

Starts from:
  <Workspace entry / alert / previous job>

Ends with:
  <Business outcome + state change>

Steps / regions:
  1. …
  2. …
  …

Gates (cannot proceed if):
  - …

Integrations:
  - Module · capability

Components:
  - Wizard / Board / Terminal / …

Permissions:
  - …

Not this screen:
  - <what belongs elsewhere>
```

---

## 4. Naming rules

| Prefer | Avoid |
|--------|-------|
| Production Planning Wizard | Production Order |
| Operator Terminal — Run Job | Work Order Edit |
| Capacity Load Board | Capacity Entity |
| Quotation Comparison | SupplierQuotation List |
| Cycle Count Session | InventoryCount CRUD |

If the name is only a noun from the data model, rename it to the **job**.

---

## 5. Thinking ladder (screen level)

```text
1. Job      Kullanıcı hangi işi bitiriyor?
2. Role     Kim?
3. Steps    İş hangi adımlarla ilerliyor? (gerçek fabrika)
4. Market   SAP / IFS / D365 / Infor bu işi nasıl paketler?
5. NOS      Biz adımları nasıl daha iyi sırlarız / azaltırız?
6. Document Job screen PRD
7. Code     Cursor implements the job screen — not entity CRUD
```

---

## 6. Exemplar

**Production Planning Wizard** — [`Process_Screens/PRD_Production_Planning_Wizard.md`](./Process_Screens/PRD_Production_Planning_Wizard.md)

Job: Planlamacı bir üretim planını ürün → hammadde → hat → kapasite → termin → maliyet üzerinden kurar ve **Release** eder.

---

## 7. Relationship to Screen Map

[`NOS_SCREEN_MAP.md`](./NOS_SCREEN_MAP.md) lists modules and IDs.

As Phase 2 continues, each operational ID must be rewritten as a **job screen** using this template.  
Until rewritten, treat entity-titled rows as **placeholders to be renamed**.
