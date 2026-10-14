# Finance Module — Design Pack

**Module:** Finance  
**Status:** Active outline  
**Screen map:** `docs/00_Product/NOS_SCREEN_MAP.md` § Finance (`FIN-*`)  
**SSOT:** `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`

## Workspaces

```text
Finance
├── Dashboard
├── Costing
├── Valuation
├── Budgets
├── Period Close
├── Reports
└── Settings
```

## Pack to author next

| Document | Owns |
|----------|------|
| `Finance_Architecture.md` | Cost / valuation boundaries |
| `Finance_Workflow.md` | Period close, cost roll |
| `Finance_Screens.md` | Job screens (`FIN-*`) |
| `Finance_User_Flows.md` | Controller journeys |
| `Finance_Dashboard.md` | Cockpit |
| `Finance_API.md` | HTTP sketch |

## Authority references

| Topic | Authority |
|-------|-----------|
| Inventory valuation inputs | `Inventory_Architecture.md` |
| Production cost simulation (planning) | references Finance; Wizard step 9 must not redefine costing laws |
| Numbering (documents) | `Document_Numbering.md` |

## Scope note (v1)

Factory finance first: manufacturing cost, inventory valuation, budgets, period close, ERP export — not a full banking suite.
