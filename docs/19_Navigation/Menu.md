# Menu

**Status:** Active (target product menu)

---

## Production

| Workspace | Menu items | Screen IDs |
|-----------|------------|------------|
| Dashboard | Production Dashboard | PRD-001 |
| Planning | Production Orders, Work Orders, Scheduling, Capacity, Dispatch | PRD-010, PRD-012, PRD-021, PRD-022, PRD-023 |
| Execution | Operator Terminal, Machine Panel, Consumption, Confirmation, WIP, Packaging, FG, Scrap, Rework | PRD-013, PRD-024, PRD-014…018, PRD-027, PRD-028 |
| Master Data | BOM, Routing, Operations, Machines, Work Centers, Lines, Shifts, Calendars | PRD-002…009, PRD-025, PRD-026 |
| Insights | Reports, Analytics | PRD-020, PRD-019 |
| Settings | Production Settings | PRD-029 |

---

## Quality

Dashboard · Inspection Plans · Incoming · In-Process · Final · Non-Conformance · CAPA · Certificates · Traceability · Reports · Analytics  
→ Screen IDs: `docs/15_UI/Quality/`

---

## Maintenance

Dashboard · Assets · Asset Tree · Work Requests · Work Orders · Preventive · Corrective · Downtime · Spare Parts · OEE · Reports · Analytics  
→ Screen IDs: `docs/15_UI/Maintenance/`

---

## Inventory

| Workspace | Menu items | Screen IDs |
|-----------|------------|------------|
| Dashboard | Inventory Dashboard | INV-001 |
| Operations | Goods Receipt, Goods Issue, Transfer | INV-015–020 |
| Stock | Stock Balance, Lots, Serials, Reservations | INV-010–014, INV-030 |
| Counts & Adjustments | Cycle Count, Physical Inventory, Adjustment | INV-021–024 |
| Master Data | Materials, Warehouses, Locations, Products | INV-002–009 |
| Reports | Reports, Analytics | INV-025–026 |

Routes: `/inventory/dashboard`, `/inventory/operations/...`, `/inventory/stock/...`, `/inventory/counts/...`, `/inventory/master-data/...`, `/inventory/reports`  
→ Specs: `docs/15_UI/Inventory/`

---

## Sales / Purchasing / CRM

See `15_UI_Architecture` module READMEs and `15_UI` inventories.  
CRM menu lives under Sales or a dedicated CRM module entry when CRM screens are authored.

---

## Implementation note

Current `apps/web` `nav-config.ts` is transitional debt. Converge toward this menu;
do not extend the flat entity list as the product direction.
