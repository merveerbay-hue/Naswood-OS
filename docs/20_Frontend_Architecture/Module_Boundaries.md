# Module Boundaries (Frontend)

## Rule

Each business module owns its screens under a module folder. Shared only through:

- `shared/components` ← Component Library
- `shell` / `navigation` ← Navigation layer
- design tokens

## Cross-module navigation

Use routes and deep links (e.g. Production Order → related NCR).  
Do not import another module’s private screen internals.

## Anti-pattern

A single `pages/business/ResourcePage.tsx` parameterized by entity name as the
permanent architecture. Allowed only as debt while screens migrate to PRDs.
