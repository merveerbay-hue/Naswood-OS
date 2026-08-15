# INV-RCV-001 — Receiving Wizard — **rules spine**

**Module:** Inventory  
**Status:** Rules retained · **UX authority:** [`INV_Receiving_Workbench.md`](./INV_Receiving_Workbench.md) v1.1 Master  
**Material Identity:** `Material_Identity_Architecture.md`

---

## Supersession

Full UX = Receiving Workbench (Evidence First · 14 steps · AI).  
This file = Depo → Location → **Material Identity** (+ Lot) → Post gates only.

---

## Job

> Evidence → AI → validate → **Depo** → root **Material Identity** → **Post**. Not Create form.

---

## Spine (Workbench stages)

```text
Evidence + AI compare + count + photo + QI   → Stages 2–8
Select warehouse (Depo) — required           → Stage 9
Location                                     → Stage 9
Material Identity mint (root, class-aware)   → Stages 10–11
Optional Lot (operational)                   → Stages 10–12
Labels · Review · Post                       → Stages 12–14
```

Depo before MI mint / Post. Formats: `Document_Numbering.md`. Meaning: `Material_Identity_Architecture.md`.

---

## Related

`INV_Receiving_Workbench.md` · `Inventory_Workflow.md` · FLOW-INV-001
