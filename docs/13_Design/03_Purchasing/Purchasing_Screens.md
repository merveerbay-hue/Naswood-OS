# Purchasing Screens

**Module:** Purchasing  
**Status:** Active  
**Job-first:** `docs/00_Product/JOB_FIRST_SCREEN_DESIGN.md`  
**Screen types:** `docs/13_Design/Common/Screen_Types.md` — **no shared Create**

---

# Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Process | `Purchasing_Workflow.md` |
| Screen IDs | `NOS_SCREEN_MAP.md` § Purchasing |
| PO wizard | `docs/00_Product/Process_Screens/PUR_Purchase_Order_Wizard.md` |
| Goods receipt | Inventory **Receiving Workbench** — not Purchasing Create GR |

---

# Screen index (job-oriented)

| ID | Screen (job name) | Workspace | Type | Job / CTA |
|----|-------------------|-----------|------|-----------|
| PUR-001 | Purchasing Dashboard | Dashboard | Dashboard | See open PR/PO, overdue receipts |
| PUR-PR-001 | **Raise purchase request** | Requests | Wizard | Satınalma talebi aç — not “Create PR” |
| PUR-RFQ-001 | **Request quotation** | Sourcing | Wizard | Teklif iste |
| PUR-PO-001 | **Purchase Order Wizard** | Orders | Wizard | **Sipariş ver / Place purchase order** |
| PUR-PO-LIB | PO Library | Orders | Explorer | Find & reopen POs |
| PUR-SUP | Supplier Library | Master | Explorer | **Add supplier** (master only) |
| PUR-RET | Purchase return | Orders | Wizard | Return to supplier |
| PUR-INV | Supplier invoice match | Invoices | Workbench / Approval | Match GR/IR |
| PUR-RPT | Purchasing Reports | Reports | Explorer | Run reports |
| PUR-SET | Purchasing Settings | Settings | Explorer | Module parameters |

**Inbound physical receipt** = Inventory CTA **Mal kabul başlat** → `INV_Receiving_Workbench.md` (truck · OCR · verify · count · inspect · operator selects Depo; lot by material category). **Not** a CRUD Create form.

---

# Design rules

- Dashboard / TASK wireframes saying “Create Purchase Order” → use **Sipariş ver**.  
- No Purchasing screen that is a shared Create ResourcePage for PO/PR/RFQ.

## Related

`Purchasing_Workflow.md` · `PUR_Purchase_Order_Wizard.md` · `INV_Receiving_Workbench.md`
