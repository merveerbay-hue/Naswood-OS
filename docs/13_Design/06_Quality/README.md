# Quality Module — Design Pack

**Module:** Quality  
**Status:** Official — Foundation active  
**Authority matrix:** [`docs/00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md`](../../00_Product/DOCUMENTATION_AUTHORITY_MATRIX.md)

## Start here

| Document | Owns |
|----------|------|
| [`Quality_Foundation_Program.md`](./Quality_Foundation_Program.md) | Core architecture sequence · gates |
| [`Quality_Architecture.md`](./Quality_Architecture.md) | Boundaries, aggregates, integrations **v2** |
| [`Quality_Design_Program.md`](./Quality_Design_Program.md) | Ops process design sequence |
| [`Quality_Hold_Disposition_Architecture.md`](./Quality_Hold_Disposition_Architecture.md) | Hold / release / scrap disposition |
| [`Chain_of_Custody_Architecture.md`](../99_Shared/Chain_of_Custody_Architecture.md) | FSC / PEFC CoC continuity |

## Pack contents

| Document | Owns |
|----------|------|
| [`Quality_Workflow.md`](./Quality_Workflow.md) | Inspection → NCR → CAPA process truth |
| [`Quality_Screens.md`](./Quality_Screens.md) | Job screens & CTAs — **no shared Create** |
| [`Quality_Dashboard.md`](./Quality_Dashboard.md) | Cockpit widgets & KPIs |
| [`Quality_API.md`](./Quality_API.md) | HTTP surface sketch |
| [`Quality_Mobile.md`](./Quality_Mobile.md) | Inspector / operator mobile jobs |

NCR process screen: [`QLT_NCR_Wizard.md`](../../00_Product/Process_Screens/QLT_NCR_Wizard.md)

## Consumes (do not redefine)

- `Material_Definition_Architecture.md`
- `Material_Identity_Architecture.md`
- `Measurement_Conversion_Architecture.md`
- `Compliance_Architecture.md`
- `Document_Management_Evidence_and_Export.md`
- `Document_Numbering.md`

## UX & product map

- Screen IDs: `docs/00_Product/NOS_SCREEN_MAP.md` § Quality (`QLT-*`)
- Domain deep-dives: `docs/05_Modules/07_Quality/`
- Traceability inquiry: joint Quality + Inventory Architecture; genealogy graph → `Material_Genealogy.md`

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

Incoming materials ← Purchasing / Inventory · In-process / Final ← Production · Disposition ↔ Inventory holds · Certificates / CoC → Sales / Logistics
