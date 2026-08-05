# 14 — Implementation

**Role:** Executable work packages (TASK-*)  
**Authority:** Lowest planning layer — below UI Architecture and Design

---

## Rule

```text
UI Architecture / Design / Flows
            ↓
    Implementation TASK   ← this folder
            ↓
         Source Code
```

Do **not** treat `TASK-046` as “the BOM screen” or `TASK-070` as “the NCR screen”.

TASKs implement **slices** of screen families defined in:

- `docs/15_UI_Architecture/`
- `docs/13_Design/`
- `docs/04_Application/Screen_Catalog.md`

See `docs/15_UI_Architecture/00_Governing_Principles.md` and `03_Screen_to_Task_Mapping.md`.

---

## Sprint folders

Platform and business sprints live under this directory (`Sprint_00_…`, `Sprint_01_…`, …).

When opening a new UI-facing TASK, require the **UI Architecture Mapping** block described in `15_UI_Architecture/03_Screen_to_Task_Mapping.md`.
