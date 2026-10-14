# Quality — Screen Architecture (~20 screens)

**Target:** QMS class UX (inspection + NCR/CAPA + traceability)  
**Status:** Inventory specified — expand each ID to full PRD before UI build

---

## Navigation (target)

```text
Quality
├── Dashboard
├── Inspection Plans
├── Incoming Inspection
├── In-Process Inspection
├── Final Inspection
├── Non Conformance
├── CAPA
├── Certificates
├── Traceability
├── Reports
└── Analytics
```

---

## Screen index

| ID | Screen | Workspace | Notes |
|----|--------|-----------|-------|
| QLT-001 | Quality Dashboard | Dashboard | KPIs, open inspections, open NCRs |
| QLT-002 | Inspection Plan List | Plans | |
| QLT-003 | Inspection Plan Detail | Plans | Characteristics, sampling |
| QLT-004 | Inspection Queue | Operations | Cross-type worklist |
| QLT-005 | Incoming Inspection | Operations | List + detail family |
| QLT-006 | In-Process Inspection | Operations | |
| QLT-007 | Final Inspection | Operations | |
| QLT-008 | NCR List | Operations | |
| QLT-009 | NCR Detail | Operations | Defect, disposition, links |
| QLT-010 | Root Cause | Operations | Pane/screen of NCR family |
| QLT-011 | CAPA List | Operations | |
| QLT-012 | CAPA Detail | Operations | Owners, due dates, effectiveness |
| QLT-013 | Certificates | Compliance | |
| QLT-014 | Traceability | Traceability | Forward/backward |
| QLT-015 | Quality Reports | Reports | |
| QLT-016 | Quality Analytics | Analytics | |
| QLT-017 | Moisture / Lab Results | Laboratory | Optional timber-specific |
| QLT-018 | Attachments Desk | Shared | Evidence for NCR/Inspection |
| QLT-019 | Quality Settings | Settings | |
| QLT-020 | Approval Inbox | Operations | Quality approvals |

IA exemplar (NCR family): `docs/15_UI_Architecture/Quality/README.md`  
Entry TASKs: TASK-066–075 (slices only)
