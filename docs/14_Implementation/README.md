# 14 — Implementation

**Status: FROZEN**  
**Role:** Historical work packages (TASK-*) only  
**Authority:** Lowest planning layer — superseded as the *driver* of delivery

**Design thinking:** [`04_PRODUCT_ARCHITECT.md`](../../AI/NOS_CONSTITUTION/04_PRODUCT_ARCHITECT.md)  
**Product map:** [`docs/00_Product/`](../00_Product/)  
**Implement protocol:** [`00_AI_EXECUTION.md`](../../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

---

## Freeze policy (effective now)

```text
✔ No new TASK-*.md files
✔ No new sprint TASK catalogs as the product roadmap
✔ Existing TASK files remain as historical delivery records / acceptance traces
✘ Do not open work with “implement TASK-0XX” or “TASK yazalım”
✔ Open design with roles / workspaces (“Üretim Müdürü ne görmeli?”)
✔ Open implementation with “build <Module> Workspace / Screen / Flow”
```

`00_Product`, Design (`13`), UI Architecture, Screens, Navigation, Flows,
Components, and Frontend Architecture define **what NOS is**.

`14_Implementation` no longer defines **what to build next**.

**Create / Yeni wireframes in TASK files are not UX authority.**  
Live rule: [`Screen_Types.md`](../13_Design/Common/Screen_Types.md) § Create → Job CTA matrix · [`Process_Screens/`](../00_Product/Process_Screens/).

---

## Why frozen

The anti-pattern this freeze ends:

```text
Architecture → TASK → TASK → TASK → TASK
```

Target delivery chain:

```text
Architecture
  → Module
  → Workspace
  → Navigation
  → Screens
  → Components
  → User Flow
  → Frontend
```

See: [`AI/NOS_CONSTITUTION/00_AI_EXECUTION.md`](../../AI/NOS_CONSTITUTION/00_AI_EXECUTION.md)

---

## Historical rule (for existing TASK files)

Existing `TASK-*` documents are **work-package archives**. They may still be
referenced for acceptance criteria of already-scoped increments, but they:

- never define product, UI, workflow, navigation, or business architecture
- never authorize a new CRUD screen by themselves
- must yield to Screen Architecture (`15_UI`) and UI Architecture (`15_UI_Architecture`)

---

## Where to work instead

| Need | Layer |
|------|--------|
| Module / workspace structure | `docs/15_UI_Architecture/` |
| Named screens (PRD / QLT / MNT …) | `docs/15_UI/` |
| Design tokens & primitives | `docs/16_Design_System/` |
| End-to-end jobs | `docs/17_User_Flows/` |
| Enterprise UI building blocks | `docs/18_Component_Library/` |
| Menu, permissions, breadcrumbs | `docs/19_Navigation/` |
| Frontend app structure | `docs/20_Frontend_Architecture/` |

---

## Sprint folders

Sprint folders under this directory remain for archive / traceability only.
Do not add new TASK files into them.
