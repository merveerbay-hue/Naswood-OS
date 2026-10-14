# Maintenance Module — Design Pack

**Module:** Maintenance  
**Status:** Active outline  
**Screen map:** `docs/00_Product/NOS_SCREEN_MAP.md` § Maintenance (`MNT-*`)  
**SSOT:** `docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`

## Workspaces

```text
Maintenance
├── Dashboard
├── Assets
├── Work Management
├── Planning
├── Reliability
├── Spare Parts
├── Reports
├── Analytics
└── Settings
```

## Pack to author next (same pattern as Production / Quality)

| Document | Owns |
|----------|------|
| `Maintenance_Architecture.md` | Boundaries, asset model, integrations |
| `Maintenance_Workflow.md` | Request → WO → PM → Downtime → Close |
| [`Maintenance_Screens.md`](./Maintenance_Screens.md) | Job screens (`MNT-*`) — **no shared Create** |

WO process screen: [`MNT_Work_Order_Wizard.md`](../../00_Product/Process_Screens/MNT_Work_Order_Wizard.md)
| `Maintenance_User_Flows.md` | Role journeys |
| `Maintenance_Dashboard.md` | Cockpit |
| `Maintenance_API.md` | HTTP sketch |
| `Maintenance_Mobile.md` | Technician queue |

## Authority references

| Topic | Authority |
|-------|-----------|
| Numbering | `Document_Numbering.md` |
| Spare stock | `Inventory_Architecture.md` |
| Machine master (production) | `Production_Architecture.md` |
| OEE | Reliability screens + Production Monitoring (joint) |

## Domain seeds

Historical TASK archive: `docs/14_Implementation/Sprint_07_Maintenance/` (not product authority).
