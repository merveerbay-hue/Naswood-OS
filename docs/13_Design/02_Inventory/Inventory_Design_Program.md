# Inventory Design Program

**Module:** Inventory  
**Status:** Official — Product Architect  
**Version:** 1.0.0  
**Owns:** Design **sequence**, process PRD **question template**, completion status  
**Does not own:** Screen layouts (each process PRD) · Numbering · Material Identity · stock ledger  

---

## Working contract

```text
We design Inventory processes ONE AT A TIME — in the order below.
We do not invent all screens in parallel.
We do not generate CRUD / Create forms.
We answer the question template before drawing UI.
We extend Architecture / Workflow / Screens — we do not replace them.
```

Human (Product Architect) decides the next process.  
AI drafts the process PRD only for the **active** row.

---

## Design sequence (locked)

| # | Process (EN) | Process (TR) | Screen type (typical) | Spec / status |
|---|--------------|--------------|----------------------|---------------|
| 1 | **Dashboard (Operations Center)** | Depo Komuta Merkezi | Dashboard / Command Center | **Done** — `Inventory_Dashboard.md` |
| 2 | **Receiving** | Mal Kabul | Workbench (Evidence First) | **Done** — `INV_Receiving_Workbench.md` |
| 3 | **Putaway** | Depolama / Yerleştirme | Terminal / Workbench | **Next** — TBD |
| 4 | **Warehouse Explorer** | Depo Gezgini | Explorer | Queued |
| 5 | **Stock Transfer** | Stok Transfer | Wizard | Queued — spine exists `INV_Transfer_Wizard.md` (evolve under this program) |
| 6 | **Material Reservation** | Malzeme Rezervasyonu | Desk / Workbench | Queued — INV-030 |
| 7 | **Goods Issue** | Mal Çıkışı | Wizard / Terminal | Queued — spine `INV_Issue_Wizard.md` |
| 8 | **Cycle Count** | Çevrim Sayımı | Wizard / Session | Queued — spine `INV_Cycle_Count_Session.md` |
| 9 | **Physical Inventory** | Envanter Sayımı | Session / Workbench | Queued — INV-023 |
| 10 | **Shipping** | Sevkiyat | Workbench / Console | Queued |
| 11 | **Material Traceability** | Malzeme İzlenebilirlik | Workbench / Explorer | Queued — MI + Genealogy |
| 12 | **Analytics & Reports** | Analitik & Raporlar | Explorer / Reports | Queued — **not** Command Center |

Master Data (Malzeme / Depo / Lokasyon tanımla) is **out of this operations sequence** — separate engineering / configuration track.

---

## Mandatory question template (every process)

Before any wireframe or FE, the process PRD **must** answer:

| # | Question | TR |
|---|----------|----|
| 1 | **Who is the user?** | Kullanıcı kim? |
| 2 | **What do they do in real life?** | Gerçek hayatta ne yapıyor? |
| 3 | **Which documents are used?** | Hangi belgeler kullanılıyor? |
| 4 | **Which photos are taken?** | Hangi fotoğraflar çekiliyor? |
| 5 | **What AI support is possible?** | Hangi AI desteği olabilir? |
| 6 | **What must be auto-generated?** | Hangi bilgiler otomatik oluşturulmalı? |
| 7 | **What must never be typed manually?** | Hangi bilgiler kesinlikle manuel girilmemeli? |
| 8 | **Which decisions stay with the user?** | Hangi kararlar kullanıcı tarafından verilmeli? |

Plus job-first fields from `JOB_FIRST_SCREEN_DESIGN.md`:

```text
Job to be done:
Screen type:
CTA (verb):
Workspace:
Finish action (Post / Release / Complete — never bare Save as the job):
```

### Cross-cutting laws (always reference — never restate algorithms)

| Topic | Authority |
|-------|-----------|
| Identifiers | `Document_Numbering.md` |
| Material Identity vs Lot | `Material_Identity_Architecture.md` |
| Genealogy | `Material_Genealogy.md` |
| Stock truth | `Inventory_Architecture.md` |
| Screen types / no Create | `Screen_Types.md` |
| Evidence First (where ops capture proof) | Established on Receiving; apply when process uses photos/docs |

---

## Process PRD skeleton

Create under `docs/00_Product/Process_Screens/` as `INV_<Process>.md`:

```markdown
# INV-… — <Job name>

## Absolute rule
NOT a CRUD / Create form.

## Answers (question template)
1. User: …
2. Real-life job: …
3. Documents: …
4. Photos: …
5. AI: …
6. Auto-generated: …
7. Never manual: …
8. User decisions: …

## Job / CTA / Screen type / Workspace
## Flow
## Gates
## UI anatomy
## Posting / finish
## Related authorities
```

---

## Active focus

```text
NEXT → 3. Putaway (Depolama / Yerleştirme)
```

When Putaway is requested, draft `INV_Putaway_Workbench.md` (or Terminal) using the template above, then update `Inventory_Screens.md` · `Inventory_Workflow.md` · `Inventory_Navigation.md`.

---

## Related

`Inventory_Screens.md` · `Inventory_Workspaces.md` · `Inventory_Navigation.md` · `Inventory_Dashboard.md` · `INV_Receiving_Workbench.md` · `JOB_FIRST_SCREEN_DESIGN.md`
