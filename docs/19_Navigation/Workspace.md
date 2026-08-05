# Workspace Shell

**Status:** Active  
**Visual/layout canon:** `docs/13_Design/00_Platform/Design_System/03_Layout/Workspace.md`

---

## Definition

A **workspace** is an operational area inside a module that groups related
screens for one class of jobs (e.g. Production → Planning).

It is not a single page and not a CRUD resource.

---

## Shell contents

```text
Workspace
├── Workspace header (title, primary actions, context plant/shift)
├── Sub-navigation (screens in this workspace)
├── Main surface (active screen)
└── Optional utility rail (filters summary, AI copilot, alerts)
```

---

## Rules

1. Entering a module lands on its **Dashboard** or default workspace home — not a random entity grid.
2. Sub-nav lists screens from Menu.md for that workspace only.
3. Cross-workspace jumps use breadcrumbs / deep links, not duplicated menu trees.
4. Operator / Machine terminals may use a **focused shell** (minimal chrome) while remaining part of Execution workspace.

---

## Related

- Layout tokens: Design System `03_Layout/Workspace.md`
- IA: `15_UI_Architecture/01_Information_Architecture.md`
