# Breadcrumbs

**Status:** Active

---

## Pattern

```text
Module > Workspace > Screen > Record context
```

Examples:

```text
Production > Planning > Production Orders
Production > Planning > Production Orders > PO-2026-0142
Maintenance > Assets > Asset Detail > CNC-04
Quality > Non-Conformance > NCR-00881 > CAPA
```

---

## Rules

1. Breadcrumb labels match Menu / Screen Architecture names — not route path segments or TASK ids.
2. Record context (order no, asset code) is the last crumb and may truncate.
3. Each crumb is navigable except the current leaf.
4. Terminal / kiosk modes may hide breadcrumbs; restore in standard workspace shell.

---

## Deep links

Deep links must resolve to the same hierarchy. If a screen moves workspace in IA,
update breadcrumbs and Menu together.
