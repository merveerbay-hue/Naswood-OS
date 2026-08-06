# Frontend Architecture — Overview

## Mission

Realize NOS product layers in `apps/web` (and related clients) as an enterprise
application shell — not a collection of generic Resource pages.

## Principles

1. **Product-driven structure** — folders follow Module / Workspace / Screen, not TASK numbers.
2. **Screen = composition** — Entity Grid, Master Detail, Terminal layouts from Component Library.
3. **Server authority** — Workflow transitions and permissions enforced by API; UI mirrors them.
4. **Debt honesty** — Existing `ResourcePage` CRUD is transitional; new screens do not extend that pattern as the target.
5. **No TASK folders in source** — Do not create `pages/task-078`; create `modules/maintenance/assets/...`.

## Target shape (illustrative)

```text
apps/web/src/
  shell/                 # App shell, auth, theme
  navigation/            # Menu from docs/19_Navigation
  modules/
    production/
      planning/
      execution/
      master-data/
    quality/
    maintenance/
    inventory/
    sales/
    purchasing/
  shared/
    components/          # maps to 18_Component_Library
    design-system/       # tokens from 16
```
