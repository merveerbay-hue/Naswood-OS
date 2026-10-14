# Quality Hold & Disposition Architecture

**Module:** Quality  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Location:** `docs/13_Design/06_Quality/Quality_Hold_Disposition_Architecture.md`  
**Owns:** Quality Hold / Quarantine / disposition laws · who may hold / release / scrap / rework · handoff to Inventory Transaction Engine · link to NCR / evidence · Goods Issue damage→hold composition  
**Does not own:** Stock balance posting implementation (→ Inventory Transaction Engine) · NCR wizard UX (→ `QLT_NCR_Wizard.md`) · Package Allocation disposition UI (→ PAW) · Compliance spine (→ `Compliance_Architecture.md`)

---

## Absolute laws

```text
1. Non-conforming material must not proceed without disposition.
2. Quality may REQUEST hold / quarantine / scrap / rework — Inventory POSTS the stock effect.
3. Only Quality (or authorized policy role) may release from Quality Hold.
4. Disposition always links Material Identity · Package (if any) · evidence · reason.
5. Scrap never silently returns to Available package balance without Scrap txn.
6. GI “Quality Hold” / Damaged paths compose this architecture — do not fork a second hold model.
```

---

## Disposition outcomes

| Outcome | Stock effect (via Inventory) | Typical next |
|---------|------------------------------|--------------|
| **Release** | Clear hold → Available / Reserved | Continue process |
| **Quality Hold** | Block pick/issue | NCR / inspection |
| **Quarantine** | Quarantine location / status | Investigation |
| **Rework** | Move to rework / Production handoff | Rework order |
| **Scrap** | Scrap Inventory Transaction | NCR close / CAPA |
| **Return to supplier** | Return flow (Purchasing) | Supplier NCR |

---

## Actors

| Role | May |
|------|-----|
| Inspector | Raise hold recommendation · capture evidence |
| Quality Engineer | Disposition · release · link NCR/CAPA |
| Supervisor | Policy waivers (audited) |
| Warehouse | Execute Inventory txn after Quality decision — not override Quality |

---

## Composition

| Topic | Authority |
|-------|-----------|
| Evidence | `Document_Management_Evidence_and_Export.md` |
| Compliance / audit | `Compliance_Architecture.md` |
| MI / Package | `Material_Identity_Architecture.md` · `Package_Architecture.md` |
| GI damage/hold during pick | `Package_Allocation_Workspace.md` · GI Workbench |
| Stock posting | `Inventory_Transaction_Engine.md` |

---

## Related

`Quality_Foundation_Program.md` · `Quality_Architecture.md` · `Quality_Workflow.md` · `Non_Conformance.md` · `Compliance_Architecture.md`
