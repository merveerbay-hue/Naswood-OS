# Quality Screens

**Module:** Quality  
**Status:** Active  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`  
**Screen types:** `docs/13_Design/Common/Screen_Types.md` — **no shared Create**

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Process | `Quality_Workflow.md` |
| Screen IDs | `NOS_SCREEN_MAP.md` § Quality |
| NCR wizard | `docs/00_Product/Process_Screens/QLT_NCR_Wizard.md` |

---

# Screen index (job-oriented)

| ID | Screen (job name) | Workspace | Type | Job / CTA |
|----|-------------------|-----------|------|-----------|
| QLT-001 | Quality Dashboard | Dashboard | Dashboard | See open inspections, NCRs, CAPAs |
| QLT-NCR-001 | **NCR Wizard** | Operations | Wizard | **Raise NCR / NCR aç** — not “Yeni NCR” |
| QLT-NCR-LIB | NCR Library | Operations | Explorer | Find & reopen NCRs |
| QLT-INSP-001 | **Start inspection** | Operations | Terminal / Wizard | Execute inspection plan |
| QLT-CAPA-001 | **Open CAPA** | Operations | Wizard / Workbench | Drive corrective action |
| QLT-HOLD | Quality Hold Desk | Operations | Console | Place / release holds (Inventory handoff) |
| QLT-TRACE | Traceability Inquiry | Compliance | Workbench | Forward / backward lot inquiry |
| QLT-PLAN | Inspection Plans | Plans & Specs | Explorer | Maintain plans (master) |
| QLT-SPEC | Specs / AQL | Plans & Specs | Explorer | Maintain specs |
| QLT-RPT | Quality Reports | Reports | Explorer | Run reports |
| QLT-SET | Quality Settings | Settings | Explorer | Module parameters |

---

# Design rules

- Primary ops CTAs are job verbs (`Screen_Types.md` § Create matrix).  
- Libraries reopen work; they do not define Create chrome.  
- Frozen TASK “Create NCR” wireframes are not authority.

## Related

`Quality_Workflow.md` · `Quality_Architecture.md` · `QLT_NCR_Wizard.md`
