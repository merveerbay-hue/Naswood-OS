# Quality Architecture

**Module:** Quality  
**Version:** 1.0  
**Status:** Active

---

# Purpose

Defines Quality module boundaries inside NOS: inspection planning, execution,
non-conformance, CAPA, certificates, and quality-facing traceability inquiry.

Quality **does not** own stock balances or mint material identities.

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `docs/13_Design/99_Shared/Document_Numbering.md` |
| Genealogy graph | `docs/05_Modules/02_Production/Material_Genealogy.md` |
| Inventory ownership / holds | `docs/13_Design/02_Inventory/Inventory_Architecture.md` |
| Production execution | `docs/13_Design/05_Production/Production_Workflow.md` |
| Capability | `docs/05_Modules/01_Master_Data/Products.md` |
| SSOT matrix | `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md` |

---

# Module position

```text
Purchasing / Inventory ──► Incoming Inspection
Production ──────────────► In-Process / Final Inspection
         │
         ▼
      NCR / CAPA
         │
         ├──► Inventory hold / release (via Inventory)
         ├──► Production rework / scrap (via Production)
         └──► Certificates / Traceability views
```

---

# Owns

- Inspection Plan (characteristics, sampling)
- Inspection instances (Incoming / In-Process / Final)
- Inspection Queue (worklist)
- Non-Conformance (NCR) + Root Cause
- CAPA
- Quality Certificate records
- Moisture / Lab result records (timber)
- Quality approvals inbox
- Quality dashboards, reports, analytics (views)

---

# Does not own

- Stock quantities / warehouse ledger → Inventory  
- Material/Lot/Serial minting → Numbering Service  
- Parent–child genealogy graph → Material Genealogy  
- Production Order release → Production  
- Supplier master → Purchasing  

---

# Core aggregates

| Aggregate | Purpose |
|-----------|---------|
| InspectionPlan | Spec + sampling for material/product/op |
| Inspection | Executed check against a plan / context |
| NonConformance | Quality incident + disposition |
| Capa | Corrective / preventive action |
| QualityCertificate | Compliance certificate for FG / lot |
| LabResult | Moisture / lab measurement |
| QualityApproval | Gate decisions |

Domain detail: `docs/05_Modules/07_Quality/*`.

---

# Integrations

| Module | Interaction |
|--------|-------------|
| Inventory | Block/release lots; GR triggers incoming inspection |
| Production | In-process/final gates; scrap/rework handoff |
| Purchasing | Supplier quality on inbound |
| Sales | Certificate on shipment |
| Genealogy | Trace views read genealogy service |

---

# Invariants

1. Non-conforming material must not proceed without disposition.  
2. Stock holds are posted through Inventory — Quality never adjusts balance directly.  
3. Identifiers for lots/serials/packages follow Numbering authority.  
4. NCR closure requires verified disposition (and CAPA link when policy requires).  

---

# Related

- `Quality_Workflow.md` · `Quality_Dashboard.md` · `Quality_API.md` · `Quality_Mobile.md`  
- Screen Map `QLT-001`…`QLT-020`
