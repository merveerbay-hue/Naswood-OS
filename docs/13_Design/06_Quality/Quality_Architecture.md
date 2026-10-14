# Quality Architecture

**Module:** Quality  
**Version:** 2.0.0  
**Status:** Official — Product Architect  
**Foundation:** [`Quality_Foundation_Program.md`](./Quality_Foundation_Program.md)

---

# Purpose

Defines Quality module boundaries inside NOS: inspection planning, execution,
non-conformance, CAPA, certificates, moisture/lab, quality-facing traceability,
**Hold/Disposition**, and **FSC/PEFC Chain of Custody stewardship**.

```text
Quality CONSUMES Material Definition · Material Identity · Measurement/Conversion ·
Compliance · Evidence — it does not invent parallel masters or audit systems.
Quality does NOT own stock balances or mint Material Identities.
```

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Quality Foundation sequence | `Quality_Foundation_Program.md` |
| Ops design sequence | `Quality_Design_Program.md` |
| Hold / disposition | `Quality_Hold_Disposition_Architecture.md` |
| FSC / PEFC CoC | `Chain_of_Custody_Architecture.md` |
| Compliance spine | `Compliance_Architecture.md` |
| Material Definition (grade/moisture/inspection bindings) | `Material_Definition_Architecture.md` |
| Material Identity / Package / Lot | `Material_Identity_Architecture.md` |
| Measurement / sample qty | `Measurement_Conversion_Architecture.md` |
| Evidence / Document Library | `Document_Management_Evidence_and_Export.md` |
| Numbering | `Document_Numbering.md` |
| Genealogy graph | `Material_Genealogy.md` |
| Inventory ownership / holds posting | `Inventory_Architecture.md` · `Inventory_Transaction_Engine.md` |
| Production execution | `Production_Workflow.md` |
| SSOT matrix | `DOCUMENTATION_AUTHORITY_MATRIX.md` |

---

# Module position

```text
Purchasing / Inventory ──► Incoming Inspection
Production ──────────────► In-Process / Final Inspection
         │
         ▼
   Hold / Disposition (Quality decides → Inventory posts)
         │
         ▼
      NCR / CAPA
         │
         ├──► Inventory hold / release / scrap (via Inventory txn)
         ├──► Production rework / scrap (via Production)
         ├──► Certificates / CoC validation
         └──► Traceability views (read Genealogy + MI)
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
- Quality Hold / Quarantine **decisions** (stock via Inventory)
- CoC stewardship (FSC/PEFC claim validation)
- Quality approvals inbox
- Quality dashboards, reports, analytics (views)

---

# Does not own

- Stock quantities / warehouse ledger → Inventory  
- Material Definition catalog → Material Definition Architecture  
- Material/Lot/Serial/Package minting → Numbering + MI Architecture  
- Parent–child genealogy graph → Material Genealogy  
- Production Order release → Production  
- Supplier master → Purchasing  
- Compliance engine payloads → Audit_Log / Approval / Evidence Shared docs  

---

# Core aggregates

| Aggregate | Purpose |
|-----------|---------|
| InspectionPlan | Spec + sampling for material/product/op (reads Material Definition) |
| Inspection | Executed check against a plan / context |
| NonConformance | Quality incident + disposition |
| Capa | Corrective / preventive action |
| QualityCertificate | Compliance certificate for FG / lot / MI |
| LabResult | Moisture / lab measurement |
| QualityApproval | Gate decisions |
| QualityHoldRequest | Hold/quarantine/release intent → Inventory txn |

Domain detail: `docs/05_Modules/07_Quality/*`.

---

# Integrations

| Module | Interaction |
|--------|-------------|
| Inventory | GR triggers incoming inspection · hold/release/scrap via Transaction Engine · GI damage→hold |
| Production | In-process/final gates · scrap/rework handoff · new MI on transform |
| Purchasing | Supplier quality on inbound · certificates |
| Sales / Shipping | Certificate + CoC claim on shipment |
| Genealogy | Trace views read genealogy service (MI nodes) |
| Material Definition | Grade · moisture · inspection · CoC bindings |

---

# Invariants

1. Non-conforming material must not proceed without disposition.  
2. Stock holds are posted through Inventory — Quality never adjusts balance directly.  
3. Identifiers for MI / lots / packages follow Numbering + MI Architecture.  
4. NCR closure requires verified disposition (and CAPA link when policy requires).  
5. CoC claims require unbroken MI genealogy + certificate evidence on file.  
6. Evidence First on inspection exceptions — Compliance Architecture.  
7. No Quality Create Form — Wizard / Workbench / Terminal / Designer only (`Screen_Types.md`).

---

# Workspaces (product shape)

```text
Quality
├── Dashboard          — Command / queues (not vanity KPI wall)
├── Plans & Specs      — Inspection plans · specs (engineering)
├── Operations         — Inspect · Hold Desk · NCR · CAPA
├── Laboratory         — Moisture / lab
├── Compliance         — Traceability · CoC · Certificates
├── Reports
├── Analytics
└── Settings
```

---

# Related

- `Quality_Foundation_Program.md` · `Quality_Design_Program.md` · `Quality_Hold_Disposition_Architecture.md`  
- `Quality_Workflow.md` · `Quality_Screens.md` · `Quality_Dashboard.md` · `Quality_API.md` · `Quality_Mobile.md`  
- `QLT_NCR_Wizard.md` · `Chain_of_Custody_Architecture.md` · `Compliance_Architecture.md`  
- Screen Map `QLT-*`
