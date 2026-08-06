# Quality API

**Module:** Quality  
**Base:** `/api/v1/quality`  
**Version:** 1.0 (sketch)  
**Status:** Active design

---

# Authority references

Business rules from `Quality_Architecture.md` / `Quality_Workflow.md`.  
Identifiers → Numbering Service. Stock holds → Inventory APIs.

---

# Resources (sketch)

| Area | Methods |
|------|---------|
| Inspection plans | `GET/POST /plans`, `GET/PUT /plans/{id}` |
| Inspections | `GET /inspections`, `POST /inspections`, `POST /inspections/{id}/complete` |
| Queue | `GET /queue` |
| NCR | `GET/POST /ncrs`, `GET /ncrs/{id}`, `POST /ncrs/{id}/disposition` |
| CAPA | `GET/POST /capas`, `POST /capas/{id}/verify` |
| Certificates | `GET/POST /certificates` |
| Lab / moisture | `GET/POST /lab-results` |
| Approvals | `GET /approvals`, `POST /approvals/{id}/decide` |
| Dashboard | `GET /dashboard` |
| Traceability view | `GET /traceability/{lotOrSerial}` *(reads Genealogy + Inventory; does not own graph)* |

---

# Permissions (sketch)

`quality.plan.read|write` · `quality.inspection.execute` · `quality.ncr.manage` · `quality.capa.manage` · `quality.certificate.issue` · `quality.dashboard.read` · `quality.approve`

---

# Rules

- API must not update Inventory balances directly — call Inventory hold/release.  
- API must not mint Lot/Serial — Numbering Service.  
- Completing a failed inspection may emit `NonConformanceCreated` (see Event Catalog).

---

# Related

`Quality_Architecture.md` · `docs/03_system/Event_Catalog.md` · `docs/03_system/API_Standards.md`
