# Quality Module — Design Pack

**Module:** Quality  
**Status:** Active  
**Authority matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

## Pack contents

| Document | Owns |
|----------|------|
| [`Quality_Architecture.md`](./Quality_Architecture.md) | Boundaries, aggregates, integrations |
| [`Quality_Workflow.md`](./Quality_Workflow.md) | Inspection → NCR → CAPA process truth |
| [`Quality_Dashboard.md`](./Quality_Dashboard.md) | Cockpit widgets & KPIs |
| [`Quality_API.md`](./Quality_API.md) | HTTP surface sketch |
| [`Quality_Mobile.md`](./Quality_Mobile.md) | Inspector / operator mobile jobs |

## UX & product map

- Screen IDs: `docs/00_Product/NOS_SCREEN_MAP.md` § Quality (`QLT-*`)
- Domain deep-dives: `docs/05_Modules/07_Quality/`
- Traceability inquiry: joint Quality + Inventory Architecture; genealogy graph → `Material_Genealogy.md`
- Numbering: `docs/13_Design/99_Shared/Document_Numbering.md` (reference only)

## Workspaces (product shape)

```text
Quality
├── Dashboard
├── Plans & Specs
├── Operations
├── Laboratory
├── Compliance
├── Reports
├── Analytics
└── Settings
```

## Related modules

Incoming materials ← Purchasing / Inventory · In-process / Final ← Production · Disposition ↔ Inventory holds · Certificates → Sales / Logistics
