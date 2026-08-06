# Production — Screen Architecture

**Target product level:** SAP / Dynamics / Infor / IFS / Opcenter class MES+ERP  
**Not:** a single Production page or flat CRUD list

---

## Navigation (target)

```text
Production
├── Dashboard
├── Planning
│     ├── Production Orders
│     ├── Work Orders
│     ├── Scheduling
│     ├── Capacity Planning
│     └── Dispatch List
├── Execution
│     ├── Operator Panel
│     ├── Machine Panel
│     ├── Material Consumption
│     ├── Production Confirmation
│     ├── WIP Tracking
│     ├── Packaging
│     ├── Finished Goods
│     ├── Scrap
│     └── Rework
├── Master Data
│     ├── BOM
│     ├── Routing
│     ├── Operations
│     ├── Machines
│     ├── Work Centers
│     ├── Production Lines
│     ├── Shifts
│     └── Calendars
├── Reports
├── Analytics
└── Settings
```

≈ **35–40 screens** when List/Detail pairs and terminals are counted separately.

---

## Screen index

| ID | Screen | Workspace | Status | Spec |
|----|--------|-----------|--------|------|
| PRD-001 | Production Dashboard | Dashboard | Specified | [Screens/PRD-001_Dashboard.md](Screens/PRD-001_Dashboard.md) |
| PRD-002 | BOM List | Master Data | Specified | [Screens/PRD-002_BOM_List.md](Screens/PRD-002_BOM_List.md) |
| PRD-003 | BOM Detail | Master Data | Specified | [Screens/PRD-003_BOM_Detail.md](Screens/PRD-003_BOM_Detail.md) |
| PRD-004 | Routing List | Master Data | Specified | [Screens/PRD-004_Routing_List.md](Screens/PRD-004_Routing_List.md) |
| PRD-005 | Routing Detail | Master Data | Specified | [Screens/PRD-005_Routing_Detail.md](Screens/PRD-005_Routing_Detail.md) |
| PRD-006 | Work Center | Master Data | Specified | [Screens/PRD-006_Work_Center.md](Screens/PRD-006_Work_Center.md) |
| PRD-007 | Machine | Master Data | Specified | [Screens/PRD-007_Machine.md](Screens/PRD-007_Machine.md) |
| PRD-008 | Shift | Master Data | Specified | [Screens/PRD-008_Shift.md](Screens/PRD-008_Shift.md) |
| PRD-009 | Production Calendar | Master Data | Specified | [Screens/PRD-009_Calendar.md](Screens/PRD-009_Calendar.md) |
| PRD-010 | Production Order List | Planning | Specified | [Screens/PRD-010_Production_Order_List.md](Screens/PRD-010_Production_Order_List.md) |
| PRD-011 | Production Order Detail | Planning | Specified | [Screens/PRD-011_Production_Order_Detail.md](Screens/PRD-011_Production_Order_Detail.md) |
| PRD-012 | Work Order | Planning | Specified | [Screens/PRD-012_Work_Order.md](Screens/PRD-012_Work_Order.md) |
| PRD-013 | Operator Panel | Execution | Specified | [Screens/PRD-013_Operator_Terminal.md](Screens/PRD-013_Operator_Terminal.md) |
| PRD-014 | Material Consumption | Execution | Specified | [Screens/PRD-014_Material_Consumption.md](Screens/PRD-014_Material_Consumption.md) |
| PRD-015 | Production Confirmation | Execution | Specified | [Screens/PRD-015_Production_Confirmation.md](Screens/PRD-015_Production_Confirmation.md) |
| PRD-016 | WIP Tracking | Execution | Specified | [Screens/PRD-016_WIP.md](Screens/PRD-016_WIP.md) |
| PRD-017 | Packaging | Execution | Specified | [Screens/PRD-017_Packaging.md](Screens/PRD-017_Packaging.md) |
| PRD-018 | Finished Goods | Execution | Specified | [Screens/PRD-018_Finished_Goods.md](Screens/PRD-018_Finished_Goods.md) |
| PRD-019 | Production Analytics | Analytics | Specified | [Screens/PRD-019_Analytics.md](Screens/PRD-019_Analytics.md) |
| PRD-020 | Production Reports | Reports | Specified | [Screens/PRD-020_Reports.md](Screens/PRD-020_Reports.md) |
| PRD-021 | Scheduling | Planning | Planned | [Screens/PRD-021_Scheduling.md](Screens/PRD-021_Scheduling.md) |
| PRD-022 | Capacity Planning | Planning | Planned | [Screens/PRD-022_Capacity_Planning.md](Screens/PRD-022_Capacity_Planning.md) |
| PRD-023 | Dispatch List | Planning | Planned | [Screens/PRD-023_Dispatch_List.md](Screens/PRD-023_Dispatch_List.md) |
| PRD-024 | Machine Panel | Execution | Planned | [Screens/PRD-024_Machine_Panel.md](Screens/PRD-024_Machine_Panel.md) |
| PRD-025 | Operations | Master Data | Planned | [Screens/PRD-025_Operations_Master.md](Screens/PRD-025_Operations_Master.md) |
| PRD-026 | Production Line | Master Data | Planned | [Screens/PRD-026_Production_Line.md](Screens/PRD-026_Production_Line.md) |
| PRD-027 | Scrap | Execution | Planned | [Screens/PRD-027_Scrap.md](Screens/PRD-027_Scrap.md) |
| PRD-028 | Rework | Execution | Planned | [Screens/PRD-028_Rework.md](Screens/PRD-028_Rework.md) |
| PRD-029 | Production Settings | Settings | Planned | [Screens/PRD-029_Settings.md](Screens/PRD-029_Settings.md) |

Further List/Detail splits (e.g. Work Order List vs Detail, Machine List vs Detail) push the count into the **35–40** range as implementation deepens.

---

## Relationship to Implementation TASKs

| TASK | Screens it may slice (examples) |
|------|----------------------------------|
| TASK-046 | PRD-002, PRD-003 |
| TASK-047 | PRD-004, PRD-005 |
| TASK-048–055 | PRD-006–009, PRD-025–026 |
| TASK-056 | PRD-010, PRD-011 |
| TASK-057 | PRD-012, PRD-023 |
| TASK-058–064 | PRD-013–018, PRD-027–028 |
| TASK-065 | PRD-001, PRD-019, PRD-020 |

Workspace IA overview: `docs/15_UI_Architecture/Production/`
