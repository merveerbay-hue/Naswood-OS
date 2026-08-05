# Sales — UI Information Architecture

**Module:** Sales / CRM  
**Status:** Draft workspace map

---

## Workspaces

```text
Sales
├── Dashboard
├── Pipeline          (Leads, Opportunities)
├── Orders            (Quotations, Sales Orders)
├── Fulfillment       (Shipments, Deliveries, Customer Invoices)
├── Master Data       (Customers)
└── Reports
```

---

## Capability family pattern

Example — Sales Order:

```text
Sales Order List
Sales Order Detail (header, lines, schedule, shipments, invoices)
Create / Change
Credit / approval checks
```

Not: `TASK-040 → one ResourcePage`.

**Entry TASKs:** TASK-036–045
