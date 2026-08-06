# INV-ISS-001 — Goods Issue Workbench

**Module:** Inventory  
**Workspace:** Operations  
**Screen type:** **Workbench** (warehouse execution) — `Screen_Types.md` · `UI_Patterns.md`  
**Version:** 1.0 — Master Prompt aligned  
**Status:** Product Architect — authoritative issue UX  
**Supersedes as primary UX:** linear [`INV_Issue_Wizard.md`](./INV_Issue_Wizard.md) (retained as spine)  
**Stock truth:** `Inventory_Architecture.md` · `Inventory_Workflow.md`  
**Identity:** `Document_Numbering.md` · `Material_Identity_Architecture.md` · `Material_Genealogy.md`  
**Evidence / Document Library / Export:** [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md)  
**Design program:** `Inventory_Design_Program.md` § 7 (PA-directed ahead of Putaway)

---

## Absolute rules

```text
This is NOT a CRUD screen.
This is NOT a Create / Edit form.
This is a warehouse execution Workbench.
Inventory quantities are NEVER edited directly.
Every Goods Issue creates an Inventory Transaction.
```

```text
Never allow issuing materials without a business reason.
AI prepares · Operator scans · verifies · confirms · photographs when needed.
```

**Forbidden:** Bare Save form · typed Material / Lot / Package / Pallet / WH / Location / GI / txn codes · issue with no reference (except permission-controlled Manual)  
**Required:** Business document → load demand → AI pick recommend → scan validate → evidence → quality gate → **Post**

---

## Question template answers

| # | Question | Answer |
|---|----------|--------|
| 1 | **Who is the user?** | Warehouse Operator (picker) · Warehouse Supervisor · Inventory Controller (exceptions) · optionally Production / Maintenance / Shipping coordinators as request owners |
| 2 | **Real-life job?** | Takes an approved demand (PO production / WO / SO / …), walks or drives to bin, picks correct material/lot/qty, proves if needed, confirms issue so stock leaves Available (and reservation clears) |
| 3 | **Documents?** | Source: Production Order · Maintenance WO · Sales Order · Sample / R&D / Scrap / Transfer / Internal Consumption request · (rare) Manual GI. Output: GI document · pick list · loading note (if ship) |
| 4 | **Photos?** | Damage / missing / broken package · optional load / staging photos · exception evidence — linked to GI (Evidence First) |
| 5 | **AI support?** | Best WH/location/lot/package (FIFO/FEFO/reservation/quality) · pick-route hint · wrong material/lot/qty/mixed detect · shortage predict · repeated-error learning |
| 6 | **Auto-generated?** | GI number · inventory txn · movement / warehouse / material history · audit · scan & validation history · genealogy consumption link · suggested pick candidates |
| 7 | **Never manual?** | Material Code · Lot · Package · Pallet · Warehouse/Location codes · GI number · Inventory Transaction number · Material Identity strings · free-hand stock qty edit |
| 8 | **User decisions?** | Which business document to work · approve/override AI pick suggestion (within policy) · confirm scan matches · capture evidence when exception · loading/production destination assignment · **Approve Post** · raise NCR / block if wrong |

---

## Job to be done

> Depocu, **iş nedeni olan** talebe karşı doğru malzeme / lot / miktarı tarayarak çıkarır; kalite ve rezervasyon kapılarını geçer; gerekirse kanıt ekler; **Post** ile stok düşer ve izlenebilirlik güncellenir.

**Not the job:** “Create a GoodsIssue row” or type inventory down.

## CTA

| Locale | Label |
|--------|--------|
| EN | **Issue goods** / Open Goods Issue Workbench |
| TR | **Mal çıkışı** / Mal Çıkış Workbench |

Never: “Yeni çıkış” / “Create Goods Issue.”

Entry: Command Center queues · Operations · Production / Maintenance / Sales deep-link with reference · rugged Terminal into same session.

---

## Authority references

| Topic | Authority |
|-------|-----------|
| Identifiers | `Document_Numbering.md` |
| Material Identity / Lot | `Material_Identity_Architecture.md` |
| Genealogy (consume / link) | `Material_Genealogy.md` |
| Stock / reservations | `Inventory_Architecture.md` · `Inventory_Workflow.md` |
| Screen type | `Screen_Types.md` · `UI_Patterns.md` |
| Demand sources | Production / Maintenance / Sales / Quality process docs (reference only) |

```text
Identifiers are generated automatically according to the centralized
Numbering Architecture (docs/13_Design/99_Shared/Document_Numbering.md).
Manual entry is prohibited.
```

---

## Allowed business documents (issue source)

Goods Issue **can only** be created from:

| Source | Typical consumer |
|--------|------------------|
| Production Order | Production issue / backflush companion |
| Maintenance Work Order | Spare / consumable issue |
| Sales Order | Ship / stage for shipment |
| Sample Request | Quality / sales sample |
| Internal Consumption | Cost center / overhead |
| R&D Request | Lab / trial |
| Scrap Request | Controlled scrap issue |
| Transfer Request | Issue leg toward transfer (policy) |
| **Manual Goods Issue** | **Permission-controlled exception only** |

No orphan issues. Manual requires elevated permission + reason code.

---

## Workbench anatomy

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ INV-ISS-001  Goods Issue Workbench     GI-… (system) · Draft/InProgress  │
│ Ref: PO-… / WO-… / SO-… · Priority · Required date                       │
├────────────┬─────────────────────────────────────┬───────────────────────┤
│ TIMELINE   │ MAIN                                │ CONTEXT               │
│ 1 Document │ Task Panel · Material List          │ AI Recommendations    │
│ 2 Materials│ Warehouse Map · Location Panel      │ Validation / Warnings │
│ 3 AI Pick  │ Evidence Panel · Loading / Line     │ Reservation / Quality │
│ 4 Picking  │                                     │                       │
│ 5 Verify   │                                     │                       │
│ 6 Evidence │                                     │                       │
│ 7 Quality  │                                     │                       │
│ 8 Loading  │                                     │                       │
│ 9 Review   │                                     │                       │
│ 10 Post    │                                     │                       │
├────────────┴─────────────────────────────────────┴───────────────────────┤
│ STICKY: Save draft · Back · Next · Raise NCR · Post                      │
└──────────────────────────────────────────────────────────────────────────┘
```

Surfaces: Task Panel · Reference Document Panel · Material List · Warehouse Map · Location Panel · AI Recommendation Panel · Evidence Panel · Timeline · Validation Panel · Sticky Action Bar.

---

## Flow (10 steps)

```text
1 Select business document
2 Load required materials
3 AI material / location / lot recommendation
4 Picking (scan WH → zone → bin)
5 Verify material
6 Evidence collection
7 Quality validation
8 Loading / production destination
9 Final review
10 Posting → Inventory Transaction + history + genealogy
```

### 1 — Select business document

Show eligible open demands: Production Order · Maintenance WO · Sales Order · Sample / Internal / R&D / Scrap / Transfer request · Manual (if permitted).

Display: Request owner · Priority · Required date · Status.

**Gate:** One valid reference selected (or Manual + permission + reason).

---

### 2 — Load required materials

Auto-retrieve: Required materials · Required qty · Reserved qty · Available qty · Alternatives (policy).

**Do not** allow creating catalog materials here. Name-first display; codes secondary.

**Gate:** ≥1 open line with remaining qty.

---

### 3 — AI material recommendation

Recommend Warehouse · Location · Lot · Package · Pallet using:

FIFO · FEFO · Reservation · Quality status · Warehouse rules · Material status.

Operator **approves** or overrides within policy (still scan-validated later).

**Gate:** Each line has an approved pick proposal (or explicit supervisor override).

---

### 4 — Picking

Navigate: Warehouse → Zone → Rack → Shelf → Bin.

Support: Barcode · QR · RFID · Scanner · Voice picking (future).

Every scan validated against proposal / reservation.

**Gate:** Scans recorded for required identity level (lot/serial/package as configured).

---

### 5 — Verify material

Check: Correct material · Lot · Package · Qty · Quality status · Expiry · Reservation.

Detect and **block until resolved:** Wrong material/lot/qty · Blocked · Expired · Mixed lots (policy).

**Gate:** All lines green or supervisor waiver (audited).

---

### 6 — Evidence collection

Photos · optional video/voice · PDF/Excel notes · damage / missing / broken package photos.

Evidence **belongs to the Goods Issue transaction** (Evidence First — not orphan attachments).

**Gate:** Required when exception/damage policy says so; else optional.

---

### 7 — Quality validation

Auto-check: Blocked · Inspection hold · Quarantine · Expired · Rejected.

**Do not allow Post** if quality blocks the material (unless Quality releases — separate flow).

**Gate:** Quality clear for all issue lines.

---

### 8 — Loading / destination

| If | Assign |
|----|--------|
| Shipment-related | Vehicle · Loading dock · Pallet · Package · Loading sequence |
| Production issue | Production line · Work Center · Operation |

Name-first pickers; no code typing.

**Gate:** Destination required by issue type policy.

---

### 9 — Final review

Show: Business document · Materials · WH/locations · Lots/packages · Evidence · Inventory deltas · Warnings / differences.

Operator **confirms**.

**Gate:** Explicit approve for Post.

---

### 10 — Posting

**Post** (not Save) automatically creates:

| Artifact |
|----------|
| Goods Issue document (Posted) — number via Numbering |
| Inventory Transaction (immutable) — balance ↓ |
| Material movement · Warehouse history · Material history |
| Audit trail · Scan / validation / approval history |
| Evidence archive |
| Reservation clear (when applicable) |
| Genealogy update (MI consumed / linked to demand / FG path as rules allow) |

Nothing edited as a raw quantity field. Reverse only via compensating transaction.

---

## AI features (summary)

Recommend best lot/location · FIFO/FEFO · Detect pick errors (wrong material/lot/qty/mixed) · Predict shortage · Suggest pick route · Detect repeated operator mistakes (coach / supervisor alert).

AI **prepares**; operator **approves**.

---

## Evidence First

Preserve (immutable archive on Post): Photos · Documents · Issue notes · Operator comments · AI analysis · Scan history · Validation history · Approval history.

Nothing overwritten.

**Authority (do not redefine):** Document Library · Evidence Archive capabilities · history chain · search · export → [`Document_Management_Evidence_and_Export.md`](../../13_Design/99_Shared/Document_Management_Evidence_and_Export.md).

---

## Identity

Operator never enters: Material Code · Lot · Package · Pallet · Warehouse/Location codes · GI number · Inventory Transaction number · Material Identity strings.

Selection of **existing** Lot/MI/Package via scan or name-first picker is allowed. Minting new GI/txn IDs = Numbering Service only.

---

## Traceability

Every Goods Issue must support end-to-end reconstruction:

```text
Receiving root MI → Lot/package → Reservation (if any) → Goods Issue →
Production Order / WO / SO → (later) FG / Customer shipment
```

Genealogy + Inventory history jointly answer where it came from and what consumed it.

---

## Roles

| Role | Focus |
|------|--------|
| Warehouse Operator | Scan · verify · evidence · Post |
| Supervisor | Override AI · waive within policy · unblock |
| Inventory Controller | Reservation / shortage · Manual GI permission |
| Quality | Holds / release (blocks issue until clear) |

---

## Mobile / Terminal

Rugged Terminal may run steps 4–5 (pick/verify) inside the same GI session; full Workbench on tablet for document select · review · Post.

---

## Cursor implementation notes

1. Screen type = **Workbench** — not Issue Create form.  
2. FE: replace Issue Wizard shell with this Workbench; keep spine gates in `INV_Issue_Wizard.md`.  
3. Demand APIs load lines; never create materials.  
4. Post → inventory txn + reservation clear + evidence archive + genealogy event.  
5. Command Center queue “Open issues” opens this Workbench.  
6. Manual GI = permission flag + reason — audited.

---

## Related

`INV_Issue_Wizard.md` · `Inventory_Workflow.md` · FLOW-INV-002 · `Inventory_Screens.md`  
`Inventory_Design_Program.md` · `Material_Identity_Architecture.md` · `Document_Numbering.md` · `Inventory_Dashboard.md`
