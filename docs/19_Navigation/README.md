# 19 — Navigation

**Layer:** Product navigation & shell chrome  
**Status:** Active  
**Owns:** Menu tree, workspace entry, permissions mapping, breadcrumbs  
**Does not own:** Screen internals (see `15_UI`) or visual tokens (see `16_Design_System`)

---

## Contents

| Document | Role |
|----------|------|
| [Navigation.md](Navigation.md) | Global navigation model & rules |
| [Menu.md](Menu.md) | Module → workspace → screen menu tree |
| [Permissions.md](Permissions.md) | How permissions gate menu & actions |
| [Workspace.md](Workspace.md) | Workspace shell behavior |
| [Breadcrumb.md](Breadcrumb.md) | Breadcrumb & deep-link hierarchy |

Canonical IA tree also summarized in [`../15_UI_Architecture/02_Navigation_Map.md`](../15_UI_Architecture/02_Navigation_Map.md).  
This folder is the **product navigation layer** agents must read before changing `nav-config` or shell routes.

---

## Agent rule

Before changing frontend navigation:

1. Read this folder
2. Read `15_UI_Architecture` for the module
3. Confirm target screens exist under `15_UI`
4. Do **not** invent menu items from frozen TASK titles
