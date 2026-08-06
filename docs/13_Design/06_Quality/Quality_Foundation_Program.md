# Quality Foundation Program

**Module:** Quality  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/06_Quality/Quality_Foundation_Program.md`  
**Owns:** Quality **core architecture** sequence · completion status · composition of Inventory / Shared backbone · pause rule for inventing Quality screens that redefine material / compliance / identity laws  
**Does not own:** Inventory stock ledger · Material Definition field catalog · Numbering formats · NCR wizard step copy (→ `QLT_NCR_Wizard.md`)

---

## Working contract

```text
Quality CONSUMES Inventory Foundation + Shared backbone.
It does not invent parallel Material Identity, Conversion, or Audit laws.

Foundation before deep Production Quality coupling.
Inspection / NCR / CAPA / Hold / CoC / Certificate compose Compliance Architecture.
```

---

## Shared backbone (mandatory consume)

| Authority | Quality use |
|-----------|-------------|
| `Material_Definition_Architecture.md` | Grade · moisture · inspection · CoC bindings |
| `Material_Identity_Architecture.md` | Inspect / hold / disposition against MI · Package · Lot |
| `Measurement_Conversion_Architecture.md` | Sample qty · moisture/density inputs |
| `Compliance_Architecture.md` | Audit · revision · evidence · e-sign · immutable dispositions |
| `Document_Management_Evidence_and_Export.md` | Inspection / NCR digital file |
| `Document_Numbering.md` | NCR / CAPA / Certificate / Inspection IDs |
| `Inventory_Transaction_Engine.md` | Hold / scrap / release stock effects via Inventory |

---

## Aşama 1 — Quality Foundation (locked)

| # | Authority | Status | Document |
|---|-----------|--------|----------|
| 1 | **Quality Architecture** (module boundaries) | **Landed v2.0** | `Quality_Architecture.md` |
| 2 | **Quality Design Program** (ops sequence) | **Landed v1.0** | `Quality_Design_Program.md` |
| 3 | **Chain of Custody (FSC/PEFC) Architecture** | **Landed v1.0** | `99_Shared/Chain_of_Custody_Architecture.md` |
| 4 | **Quality Hold & Disposition Architecture** | **Landed v1.0** | `Quality_Hold_Disposition_Architecture.md` |
| 5 | Inspection Plan / Spec depth | Queued — deepen `05_Modules/07_Quality/*` | Module deep-dives |
| 6 | Certificate Architecture | Queued | TBD under Quality pack |
| 7 | CAPA Architecture depth | Queued | Compose Workflow + Compliance |

### Ops UX already started (do not redefine laws)

| Item | Status |
|------|--------|
| NCR Wizard | Spine — `QLT_NCR_Wizard.md` |
| Quality Screens index | Spec’d — `Quality_Screens.md` |
| Incoming / Process / Final / Moisture / NCR modules | Domain deep-dives in `05_Modules/07_Quality/` |

---

## Gate rules

```text
GATE Q1 — Quality Architecture v2 + Hold/Disposition + CoC Architecture Official.
GATE Q2 — FE Quality module shell (Dashboard · Operations · Compliance workspaces) landed.
GATE Q3 — Deepen Inspection Workbench / Hold Desk as process PRDs (one at a time).

Until GATE Q1: do not invent Quality CRUD Create forms or parallel identity systems.
```

---

## Related

`Quality_Architecture.md` · `Quality_Design_Program.md` · `Quality_Hold_Disposition_Architecture.md` · `Chain_of_Custody_Architecture.md` · `Compliance_Architecture.md` · `Inventory_Foundation_Program.md` · `Material_Definition_Architecture.md` · `Material_Identity_Architecture.md`
