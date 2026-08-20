# EN 14081 — Structural Production Lot Master Schema

**Project:** Naswood OS  
**Document:** Structural Production Lot Master Schema  
**Module:** Production / Quality (EN 14081 structural timber)  
**Status:** Schema freeze (design authority)  
**Depends on:** Material `complianceScope=STRUCTURAL_TIMBER`, foundation `StructuralProductionLot` + `StructuralProductionLotInput`  
**Does not implement yet:** Visual grading UI, machine integration, CE/DoP, stock consumption

---

## 1. Governing principle

NOS does not exist to “enter a lot record.”

NOS exists to produce **audit evidence**.

For every critical field / child record, the test is:

> If an auditor asks for this, can NOS show the evidence?

If the answer is no, the data model for that layer is **not complete**.

This matches TS EN 14081-1 FPC / initial inspection expectations: control and test records must be linkable to product definition, production date, method, result, acceptance criteria, and responsible person.

---

## 2. Conceptual separation (do not collapse)

| Concept | Role | Not the same as |
|--------|------|-----------------|
| **Material** | Global product definition (`STRUCTURAL_TIMBER`, supported methods) | Production run |
| **Batch / Lot (stock)** | Receiving & inventory traceability | Production Lot |
| **StructuralProductionLot** | One structural timber production / grading run | Batch, Package, Balance |
| **Package** | Physical package | Production Lot |
| **InventoryBalance / Movement** | Physical stock ledger | Classification evidence |

A Production Lot is **homogeneous** for: Plant, Material, Actual grading method, production/classification period.

---

## 3. Aggregate tree (target master schema)

```text
StructuralProductionLot                          ← aggregate root (exists)
│
├── Lot Identity                                 ← partly exists
│     ProductionLotNumber (system)
│     PlantId
│     MaterialId / MaterialCode
│     Species / Origin (from Material DefinitionJson + lot overrides if needed)
│     ProductionDate, ShiftCode
│     WorkCenterCode (optional; not a stock location)
│     ClassificationMethod: VISUAL | MACHINE
│     Status, Notes, Created*/Cancelled*
│
├── Input Traceability                           ← exists (link-only)
│     StructuralProductionLotInput[]
│       SourceBatchId | SourceInventoryBalanceId | SourcePackageId
│       Quantity, Unit
│     (Phase later: real stock consumption / genealogy posting)
│
├── Classification                               ← schema target
│     ClassificationRecord (1..1 per lot method path)
│       Method = VISUAL | MACHINE
│       OperatorId / OperatorName
│       StartedAt / CompletedAt
│       CriteriaSetCode / StandardRef (e.g. EN 14081-1 Annex A path)
│       ClassificationResult: PASS | REJECT | DOWNGRADE
│       ↓
│       StrengthClassAssignment (only after result; not a naked lot field)
│         StrengthClassCode (e.g. C18, C24)
│         AssignedFromResultId
│
├── Moisture Control                             ← separate control object
│     MoistureControl
│       MeterId / MeterCode
│       MeterVerification (validity window)
│       Measurements[] (sample, value, species, datetime, operator)
│       AcceptanceCriteriaRef
│       Result: PASS | FAIL | HOLD
│
├── Visual Classification                        ← FAZ 1 implementation target
│     VisualClassificationRecord (when Method=VISUAL)
│       Species, Origin, Dimensions snapshot
│       MoistureRef (link to MoistureControl; not embedded meter master)
│       Characteristics (Annex A–oriented):
│         Knots, GrainDeviation, DensityOrGrowthRate,
│         Cracks, Wane, Warp, BiologicalDefects, …
│       CharacteristicEvaluations[] → vs limit / grade rule
│       Result: ACCEPTED | REJECTED | DOWNGRADED
│       Target / resulting StrengthClass (via ClassificationResult)
│
├── Machine Classification                       ← FAZ 6
│     MachineClassificationRecord (when Method=MACHINE)
│       MachineId
│       MachineSettingsSnapshot (required per graded parcel)
│       OperatorId
│       ClassificationResult / StrengthClass via ClassificationRecord
│
├── Product / Strength Class                     ← FAZ 3 (via result chain)
│     NOT: ProductionLot.StrengthClass = C24 (alone)
│     YES: Lot → ClassificationRecord → Result → StrengthClassAssignment
│
├── Dimensions & Tolerances                      ← schema target
│     LotDimensionRecord / ToleranceCheck
│       Nominal (from Material) vs measured
│       Tolerance class / result
│
├── FPC Controls                                 ← FAZ 4
│     FpcControlPlanRef
│     FpcControlExecution[]
│       ControlType, Frequency, AcceptanceCriteria
│       Result, ResponsiblePerson, RecordedAt
│
├── Nonconformity                                ← FAZ 5
│     NonConformity[] (lot-linked)
│       Reason, DetectionSource
│       Disposition: HOLD | REWORK | REJECT | RELEASE_WITH_CONCESSION | …
│       ↓
│       CorrectiveAction[]
│         Action, Owner, DueAt, CompletedAt
│         VerificationResult: PASS | FAIL | PENDING
│
├── Equipment / Measurement                      ← shared masters + lot links
│     MeasuringDevice (Meter, caliper, …)
│     DeviceVerification / Calibration
│     UsedBy MoistureControl / Visual / Machine / FPC lines
│
├── Marking                                      ← FAZ 8
│     MarkingRecord
│       ProductIdentification marks required by standard/market
│       AppliedAt, AppliedBy
│
├── Release                                      ← gate, not synonym of CLASSIFIED
│     ReleaseRecord
│       Prerequisites: CLASSIFIED + FPC complete + open NC disposition closed
│       ReleasedBy, ReleasedAt
│       (maps to Status=RELEASED)
│
└── Audit Evidence                               ← cross-cutting
      Append-only event / history projection
      Who / When / What / Before-After / Reason
      Cancel audit: CancelledAt, CancelledBy, CancellationReason (no hard delete)
```

---

## 4. Classification method branching

Keep on the lot (and on ClassificationRecord):

```text
ClassificationMethod
  VISUAL
  MACHINE
```

Material may declare **both** supported methods; each Production Lot uses **exactly one**.

```text
VISUAL
  → VisualClassificationRecord
  → ClassificationRecord / Result
  → StrengthClassAssignment

MACHINE
  → MachineClassificationRecord
  → Machine + MachineSettingsSnapshot + Operator
  → ClassificationRecord / Result
  → StrengthClassAssignment
```

Machine settings must be retained **per machine-graded parcel** (standard requirement). Settings are not “nice to have UI fields”; they are evidence.

---

## 5. Moisture as a separate control object

Do **not** bury meter/verification inside the visual form as free text.

```text
ProductionLot
  └── MoistureControl
        ├── Meter
        ├── MeterVerification (validity)
        ├── Measurement[] (sample, value, species, operator, datetime)
        ├── AcceptanceCriteria
        └── Result
```

Auditor questions answered by this object:

1. What was the lot moisture?
2. Which device measured it?
3. Was device verification valid at measurement time?
4. Who measured, and when?

(Annex C oriented — value alone is insufficient.)

---

## 6. Strength class is a result, not a lot stamp

Forbidden as the sole model:

```text
ProductionLot.StrengthClass = C24
```

Required chain:

```text
ProductionLot
  → ClassificationRecord
  → ClassificationResult (PASS | REJECT | DOWNGRADE)
  → StrengthClassAssignment (e.g. C24) + criteria/standard ref
```

Example evidence sentence:

> PLOT-F01-000125 — VISUAL — PASS — Strength Class C24 — criteria set X — operator Y — 2026-08-19

---

## 7. Status model (production / audit oriented)

Complex state machine is **not** required. Status vocabulary **is**.

### Target lot statuses

| Status | Meaning |
|--------|---------|
| `DRAFT` | Identity / inputs editable under plant rules |
| `IN_CLASSIFICATION` | Grading / moisture / related controls in progress |
| `CLASSIFIED` | Classification result recorded — **not** market release |
| `FPC_PENDING` | Classified; FPC executions incomplete or failing gate |
| `RELEASED` | Release gate passed (evidence complete for intended release) |
| `CANCELLED` | Soft cancel with reason/audit — never hard deleted |

### Critical distinction

```text
CLASSIFIED ≠ RELEASED
```

A lot may be classified while FPC (or open NC verification) still blocks release.

### Disposition vs status

`HOLD` / quarantine is primarily an **NC Disposition** (and/or package/stock hold), not a substitute for inventing endless lot statuses. Lot may remain `FPC_PENDING` or `IN_CLASSIFICATION` while NC disposition is `HOLD`.

### Mapping from foundation statuses (compat)

| Foundation (PR #75) | Target |
|---------------------|--------|
| `DRAFT` | `DRAFT` |
| `IN_PROGRESS` | `IN_CLASSIFICATION` |
| `PENDING_CLASSIFICATION` | `IN_CLASSIFICATION` |
| `PENDING_QUALITY` | `FPC_PENDING` |
| `RELEASED` | `RELEASED` |
| `CANCELLED` | `CANCELLED` |
| `QUARANTINED` | Prefer NC `Disposition=HOLD`; alias accepted in transition until NC module lands |

Immutability (unchanged):

- Outside `DRAFT`: Plant, Material, ClassificationMethod, ProductionLotNumber, source inputs locked.
- Correction path: controlled DRAFT edit **or** `CANCELLED` + new lot.
- No hard delete of historical lots.

---

## 8. Nonconformity + corrective action (lot-linked)

Aligned with EN 14081-1 §§ 6.3.2.7–6.3.2.8 intent (identify, segregate, record, correct):

```text
ProductionLot
  → NonConformity
       Reason, DetectionSource, RecordedBy, RecordedAt
       Disposition (HOLD | …)
  → CorrectiveAction
       Action, Owner, Due/Done
  → Verification
       PASS | FAIL | PENDING
  → (optional) Release reconsideration
```

Example:

```text
PLOT-F01-000125
NC-2026-00017
Reason: Moisture out of acceptance range
Disposition: HOLD
Corrective Action: Re-drying
Verification: PASS
Release: APPROVED
```

Reuse / link Quality NCR–CAPA capabilities where possible; **lot link is mandatory** for structural timber evidence. Do not leave NC floating without ProductionLotId for EN 14081 lots.

---

## 9. Plant authorization (keep)

```text
User → PlantClaims / PlantAccess → Plant → ProductionLot
```

- List SQL plant-scoped  
- Detail foreign plant → `403`  
- Create from trusted working plant (not free-text foreign PlantId)  
- Foreign plant source Batch/MI/Package/Balance → `403`  

---

## 10. Auditor single-pane target (evidence pack)

When auditor says “show PLOT-F01-000125”, NOS should project:

```text
PLOT-F01-000125
────────────────
Plant, Date, Shift, Product, Species, Origin

INPUTS
CLASSIFICATION (method, operator, class, record)
MOISTURE (meter, verification, measurements, result)
FPC (control, frequency, criteria, result)
NONCONFORMITY (NC, disposition, CA, verification)
MARKING
RELEASE (by / at)
AUDIT (complete history)
```

Implementation of the **projection UI** comes after child records exist; the schema must make the projection possible.

---

## 11. Implementation phases (revised order)

| Phase | Deliverable | Notes |
|-------|-------------|--------|
| **0** | Production Lot foundation | Done (identity + inputs + plant gate) |
| **0b** | **This master schema freeze** | Doc + status vocabulary + schema types |
| **1** | Visual classification | Standard-backed record, not operator note form |
| **2** | Moisture + meter verification | Separate control object |
| **3** | Classification result → strength class | Result chain only |
| **4** | FPC plan + criteria + executions | |
| **5** | NC + quarantine disposition + CA + verification | |
| **6** | Machine classification + settings snapshot | |
| **7** | Real stock consumption / input genealogy | |
| **8** | Marking + commercial identification | |
| **9** | CE / DoP | After evidence chain is solid |

Do **not** force CE/DoP before classification + FPC evidence is real.

---

## 12. What exists in code today vs schema gap

| Layer | Today | Gap |
|-------|-------|-----|
| Lot identity | Yes | Species/origin explicit lot snapshot optional |
| Input traceability | Link-only yes | No posting |
| Classification method | `ActualGradingMethod` on lot | No ClassificationRecord child |
| Moisture | No | Full MoistureControl |
| Visual / Machine records | No | Phase 1 / 6 |
| Strength class | No | Must not add as naked lot field |
| Dimensions / tolerances | Material nominal only | Lot measurement record |
| FPC | No | Plan + execution |
| NC / CA | Quality module exists conceptually | Lot-linked EN 14081 path |
| Marking / Release record | ReleasedAt/By on lot only | Structured ReleaseRecord + Marking |
| Audit | Cancel fields + timestamps | Evidence pack projection |

---

## 13. Schema freeze rules (for implementers)

1. Do not add `StrengthClass` directly on `StructuralProductionLot` as the sole grading outcome.
2. Do not embed meter verification only inside Visual form free-text.
3. Do not treat `CLASSIFIED` as `RELEASED`.
4. Do not hard-delete Production Lots.
5. Do not invent a second “stock lot” meaning for Production Lot.
6. Every new child table/API must answer the auditor evidence test (section 1).
7. Next code slice after this freeze: **FAZ 1 Visual Classification Record** (standard characteristics + result), still without CE/DoP/machine/stock consume.

---

## 14. Related code anchors

- Domain: `Naswood.Modules.Business.Domain/Production/StructuralProductionLot.cs`
- Schema constants: `.../Production/En14081/StructuralProductionLotSchema.cs`
- API: `/api/v1/structural-production-lots`
- Material gate: `MaterialCompliance` (`STRUCTURAL_TIMBER`, `VISUAL`/`MACHINE`)
