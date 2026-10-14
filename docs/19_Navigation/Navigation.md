# Navigation Model

**Status:** Active  
**Audience:** Product, UX, frontend agents

---

## Purpose

Define how users move through NOS at the shell level — modules, workspaces,
and entry screens — independent of any Implementation TASK.

---

## Principles

1. **Module first** — Top-level nav is a business module (Production, Quality, …), not an entity.
2. **Workspace second** — Within a module, group by job (Planning, Execution, Master Data).
3. **Screen third** — Leaf items open named screens from `15_UI` (PRD-xxx, MNT-xxx, …).
4. **Role-aware** — Visibility follows Permissions; never show empty CRUD dumps to every role.
5. **No TASK-derived menus** — Menu labels come from UI Architecture + this layer, never from `TASK-0XX` filenames.

---

## Levels

```text
App Shell
  └── Module (e.g. Production)
        └── Workspace (e.g. Planning)
              └── Screen (e.g. Production Orders — PRD-010)
                    └── Context (Detail PRD-011, tabs, side panes)
```

---

## Forbidden patterns

- Flat list of every database entity under a module
- One nav item per historical TASK
- Duplicate entries for List vs Create when Create is an action on List/Detail
- Mixing Settings/Admin entities into operational workspaces without an Admin area

---

## Related

- Menu tree: [Menu.md](Menu.md)
- Permissions: [Permissions.md](Permissions.md)
- Workspace chrome: [Workspace.md](Workspace.md)
- IA map: `docs/15_UI_Architecture/02_Navigation_Map.md`
