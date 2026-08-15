# Sales Screens

**Module:** Sales (CRM surfaces included where sales-owned)  
**Status:** Active  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`  
**Screen types:** `docs/13_Design/Common/Screen_Types.md` — **no shared Create**

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Process | `Sales_Workflow.md` |
| Screen IDs | `NOS_SCREEN_MAP.md` § Sales / CRM |
| SO wizard | `docs/00_Product/Process_Screens/SAL_Sales_Order_Wizard.md` |

Historical TASK wireframes (`TASK-036`…`TASK-045`) that show `+ New X` are **not UX authority** — CTAs below win.

---

# Screen index (job-oriented)

| ID | Screen (job name) | Workspace | Type | Job / CTA |
|----|-------------------|-----------|------|-----------|
| SAL-001 | Sales Dashboard | Dashboard | Dashboard | Pipeline, open orders, late shipments |
| CRM-LEAD | **Capture lead** | CRM | Wizard / Workbench | Lead kaydet — not “+ New Lead” |
| CRM-OPP | **Open opportunity** | CRM | Workbench | Fırsat aç |
| SAL-QT-001 | **Prepare quotation** | Quotes | Wizard | Teklif hazırla — not “+ New Quotation” |
| SAL-SO-001 | **Sales Order Wizard** | Orders | Wizard | **Sipariş gir / Enter sales order** |
| SAL-SO-LIB | Order Library | Orders | Explorer | Find & reopen orders |
| SAL-SHIP-001 | **Plan shipment** | Logistics | Wizard / Console | Sevkiyat planla — not “+ New Shipment” |
| SAL-DEL | Delivery confirmation | Logistics | Terminal / Console | Confirm delivery |
| SAL-INV-001 | **Issue invoice** | Billing | Wizard | Fatura kes — not “+ New Invoice” |
| SAL-CUS | Customer Library | Master | Explorer | **Add customer** (master only) |
| SAL-RPT | Sales Reports | Reports | Explorer | Run reports |
| SAL-SET | Sales Settings | Settings | Explorer | Module parameters |

---

# Design rules

- Replace every `+ New …` in Sales TASK mockups with the CTA column above.  
- Primary order entry = `SAL_Sales_Order_Wizard.md`, not entity Create form.

## Related

`Sales_Workflow.md` · `SAL_Sales_Order_Wizard.md`
