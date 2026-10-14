# Maintenance — Screen Architecture (~25 screens)

**Target:** CMMS class UX  
**Status:** Inventory specified

---

## Navigation (target)

```text
Maintenance
├── Dashboard
├── Assets
├── Asset Tree
├── Work Requests
├── Work Orders
├── Preventive
├── Corrective
├── Downtime
├── Spare Parts
├── OEE
├── Reports
└── Analytics
```

---

## Screen index

| ID | Screen | Workspace |
|----|--------|-----------|
| MNT-001 | Maintenance Dashboard | Dashboard |
| MNT-002 | Asset Explorer | Assets |
| MNT-003 | Asset Tree / Hierarchy | Assets |
| MNT-004 | Asset Detail | Assets |
| MNT-005 | Asset Maintenance History | Assets |
| MNT-006 | Warranty | Assets |
| MNT-007 | Asset Documents | Assets |
| MNT-008 | Work Request List | Work Management |
| MNT-009 | Work Request Detail | Work Management |
| MNT-010 | Maintenance Order List | Work Management |
| MNT-011 | Maintenance Order Detail | Work Management |
| MNT-012 | Preventive Plans | Planning |
| MNT-013 | Preventive Calendar | Planning |
| MNT-014 | Corrective Desk | Work Management |
| MNT-015 | Downtime Events | Reliability |
| MNT-016 | Spare Parts List | Spare Parts |
| MNT-017 | Spare Parts Detail / BOM | Spare Parts |
| MNT-018 | OEE Board | Reliability |
| MNT-019 | Maintenance Reports | Reports |
| MNT-020 | Maintenance Analytics | Analytics |
| MNT-021 | Asset Costs | Assets |
| MNT-022 | Sensors / Condition | Assets |
| MNT-023 | Asset KPIs | Assets |
| MNT-024 | Maintenance Settings | Settings |
| MNT-025 | Technician Mobile Queue | Work Management |

IA exemplar (Asset family): `docs/15_UI_Architecture/Maintenance/README.md`  
Entry TASKs: TASK-076–085
