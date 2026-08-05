# Product Navigation Map

**Status:** Active (target IA)  
**Note:** Current React `nav-config.ts` is a **transitional** flat entity list. This document is the **target** product navigation. Implementation must converge here; do not treat the current CRUD menu as final IA.

---

## Shell

- Global search  
- Company / Plant context  
- Notifications  
- Theme  
- User / session  

---

## Target module navigation (workspaces first)

### Production

```text
Production
├── Dashboard
├── Planning
│     ├── Production Calendar
│     ├── Capacity Planning
│     └── Shift Planning
├── Execution
│     ├── Production Orders
│     ├── Work Orders
│     ├── Dispatch Board
│     ├── Operator Terminal
│     └── Machine Terminal
├── Master Data
│     ├── BOM
│     ├── Routing
│     ├── Operations
│     ├── Work Centers
│     ├── Production Lines
│     ├── Machines
│     ├── Tooling
│     └── Production Parameters
├── Monitoring
│     ├── WIP
│     ├── Confirmations
│     ├── Scrap / Rework
│     └── Packaging / Finished Goods
└── Reports
      └── Production Reports / Analytics
```

### Inventory

```text
Inventory
├── Overview
├── Operations (Receipts, Issues, Transfers)
├── Stock (Balances, Batches, Locations)
├── Counts & Adjustments
├── Master Data (Materials, Warehouses, Locations)
└── Reports
```

### Purchasing

```text
Purchasing
├── Dashboard
├── Sourcing (PR, RFQ, Quotations)
├── Orders
├── Inbound (GR against PO, Returns)
├── Master Data (Suppliers)
└── Reports
```

### Sales

```text
Sales
├── Dashboard
├── Pipeline (Leads, Opportunities)
├── Orders & Quotations
├── Fulfillment (Shipments, Deliveries)
├── Master Data (Customers)
└── Reports
```

### Quality

```text
Quality
├── Dashboard
├── Operations
│     ├── Inspection Queue
│     ├── Incoming / In-Process / Final
│     └── Non-Conformance (NCR → CAPA)
├── Plans & Specs
├── Laboratory
└── Reports / Traceability
```

### Maintenance

```text
Maintenance
├── Dashboard
├── Assets (Explorer, Hierarchy, Detail)
├── Work Management (Requests, Orders)
├── Planning (Preventive calendar)
├── Spare Parts
└── Reports / OEE
```

---

## Transition rule for current code

Until workspaces are implemented:

1. Keep existing routes working.  
2. Group new nav items under workspace labels (not flat TASK lists).  
3. Prefer deep links: List → Detail → Actions over one mega CRUD form.  
4. Do not add another generic `ResourcePage` as the “finished” shape for a capability that UI Architecture defines as a family.

---

## Related

- Current impl nav: `apps/web/src/navigation/nav-config.ts`  
- DS Navigation: `docs/13_Design/00_Platform/Design_System/03_Layout/Navigation.md`  
- Screen names: `docs/04_Application/Screen_Catalog.md`
