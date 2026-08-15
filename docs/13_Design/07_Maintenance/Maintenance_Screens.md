# Maintenance Screens

**Module:** Maintenance  
**Status:** Active outline  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`  
**Screen types:** `docs/13_Design/Common/Screen_Types.md` — **no shared Create**

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Screen IDs | `NOS_SCREEN_MAP.md` § Maintenance |
| WO wizard | `docs/00_Product/Process_Screens/MNT_Work_Order_Wizard.md` |
| Spare stock | `Inventory_Architecture.md` |

---

# Screen index (job-oriented)

| ID | Screen (job name) | Workspace | Type | Job / CTA |
|----|-------------------|-----------|------|-----------|
| MNT-001 | Maintenance Dashboard | Dashboard | Dashboard | See open WOs, downtime, PM due |
| MNT-WO-001 | **Work Order Wizard** | Work Management | Wizard | **Open work order / İş emri aç** |
| MNT-WO-LIB | Work Order Library | Work Management | Explorer | Find & reopen WOs |
| MNT-REQ-001 | **Report breakdown** | Work Management | Wizard / Console | Capture request → may open WO |
| MNT-TECH | Technician Queue | Work Management | Terminal / Console | Start / complete assigned jobs |
| MNT-PM | PM Calendar | Planning | Planner | Schedule preventive maintenance |
| MNT-ASSET | Asset Library | Assets | Explorer | Maintain assets (master — **Add asset**) |
| MNT-SPARE | Spare Parts Desk | Spare Parts | Console | Reserve / issue spares via Inventory |
| MNT-REL | Reliability | Reliability | Dashboard / Workbench | MTBF / OEE joint views |
| MNT-RPT | Maintenance Reports | Reports | Explorer | Run reports |
| MNT-SET | Maintenance Settings | Settings | Explorer | Module parameters |

---

# Design rules

- Never “Yeni iş emri” → shared Create form.  
- CTA **İş emri aç** → `MNT_Work_Order_Wizard.md`.  
- Asset **Add** is Explorer master data only.

## Related

`Maintenance` pack README · `MNT_Work_Order_Wizard.md`
