# Routing

## Target URL shape

```text
/:module/:workspace/:screen
/:module/:workspace/:screen/:id
/:module/:workspace/:screen/:id/:pane
```

Examples:

```text
/production/planning/orders
/production/planning/orders/PO-2026-0142
/production/execution/operator-terminal
/maintenance/assets/tree
/maintenance/assets/CNC-04
```

## Rules

1. Routes name **screens**, not entities only and never TASK ids.
2. Menu.md and Routing stay in sync.
3. Unknown workspace/screen → not found, not silent fallback to a CRUD grid.
4. Permission check before render (see `19_Navigation/Permissions.md`).
