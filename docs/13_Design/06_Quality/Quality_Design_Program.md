# Quality Design Program

**Module:** Quality  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Owns:** Quality ops process design **sequence**, question template reuse, completion status  
**Does not own:** Screen layouts (each process PRD) · Shared Compliance / MI / Conversion algorithms · Inventory hold posting

---

## Working contract

```text
Foundation first — Quality_Foundation_Program.md GATE Q1.
One Quality process PRD at a time.
No Create Form / CRUD NCR.
Compose Material Definition · MI · Compliance · Evidence · Inventory holds.
```

---

## Design sequence (locked)

| # | Process (EN) | Process (TR) | Screen type | Spec / status |
|---|--------------|--------------|-------------|---------------|
| 1 | **Quality Dashboard (Command)** | Kalite Komuta | Dashboard | Queued deepen — spine `Quality_Dashboard.md` |
| 2 | **Incoming Inspection Workbench** | Giriş Muayene | Workbench / Terminal | Queued — domain `Incoming_Inspection.md` |
| 3 | **In-Process Inspection** | Proses Muayene | Terminal / Wizard | Queued — `Process_Inspection.md` |
| 4 | **Final Inspection** | Final Muayene | Terminal / Wizard | Queued — `Final_Inspection.md` |
| 5 | **Quality Hold Desk** | Kalite Hold | Console / Workbench | Queued — `Quality_Hold_Disposition_Architecture.md` |
| 6 | **NCR Wizard** | NCR Aç | Wizard | **Spine** — `QLT_NCR_Wizard.md` |
| 7 | **CAPA** | DÖF / CAPA | Wizard / Workbench | Queued |
| 8 | **Traceability Inquiry** | İzlenebilirlik | Workbench | Queued — joint INV + QLT |
| 9 | **Certificate Issue** | Sertifika | Wizard / Desk | Queued |
| 10 | **Moisture / Lab** | Nem / Lab | Terminal | Queued — `Moisture.md` |
| 11 | **Plans & Specs** | Plan / Spek | Designer / Explorer | Queued — engineering track |
| 12 | **Reports & Analytics** | Rapor | Explorer | Queued — not Command Center |

---

## Question template

Reuse Inventory Design Program 8 questions + job-first fields from `JOB_FIRST_SCREEN_DESIGN.md`.

---

## Related

`Quality_Foundation_Program.md` · `Quality_Architecture.md` · `Quality_Screens.md` · `Quality_Workflow.md` · `QLT_NCR_Wizard.md`
