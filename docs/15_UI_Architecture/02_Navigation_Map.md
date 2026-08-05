# Product Navigation Map

**Status:** Active (target IA — Opcenter / SAP / D365 class)  
**Note:** Current React `nav-config.ts` is a **transitional** flat entity list. Converge to this map.

---

## Production

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

Screen PRDs: `docs/15_UI/Production/`

---

## Quality

```text
Quality
├── Dashboard
├── Inspection Plans
├── Incoming Inspection
├── In Process Inspection
├── Final Inspection
├── Non Conformance
├── CAPA
├── Certificates
├── Traceability
├── Reports
└── Analytics
```

Index: `docs/15_UI/Quality/`

---

## Maintenance

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

Index: `docs/15_UI/Maintenance/`

---

## Inventory

```text
Inventory
├── Dashboard
├── Operations
│     ├── Goods Receipt
│     ├── Goods Issue
│     └── Transfer
├── Stock
│     ├── Stock Balance
│     ├── Lots
│     ├── Serials
│     └── Reservations
├── Counts & Adjustments
│     ├── Cycle Count
│     ├── Physical Inventory
│     └── Adjustment
├── Master Data
│     ├── Materials
│     ├── Warehouses
│     ├── Locations
│     └── Products
└── Reports
```

Index: `docs/15_UI/Inventory/` · FE routes under `/inventory/:workspace/...`

---

## Sales

```text
Sales
├── Dashboard
├── CRM (Lead, Opportunity)
├── Quotation
├── Order
├── Shipment
├── Invoice
├── Reports
└── Analytics
```

Index: `docs/15_UI/Sales/`

---

## Purchasing

```text
Purchasing
├── Dashboard
├── Purchase Request
├── RFQ
├── Quotation Comparison
├── Purchase Order
├── Receiving
├── Supplier
├── Reports
└── Analytics
```

Index: `docs/15_UI/Purchasing/`

---

## Transition rule for current code

1. Keep existing routes until replaced.  
2. Regroup navigation under **workspaces**, not TASK numbers.  
3. Replace generic ResourcePage with List/Detail/Terminal screens from `docs/15_UI`.  
4. Never add a new entity as “one CRUD page = done”.
