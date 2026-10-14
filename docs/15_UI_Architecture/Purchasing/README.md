# Purchasing — UI Information Architecture

**Module:** Purchasing  
**Status:** Draft workspace map

---

## Workspaces

```text
Purchasing
├── Dashboard
├── Sourcing          (Purchase Requests, RFQ, Supplier Quotations)
├── Orders            (Purchase Orders)
├── Inbound           (GR against PO, Returns, Supplier Invoices)
├── Master Data       (Suppliers)
└── Reports
```

---

## Capability family pattern (apply to each document)

Example — Purchase Order:

```text
Purchase Order List
Purchase Order Detail (header, lines, approvals, receipts, invoices)
Create / Change PO
Approval workflow
```

Not: `TASK-030 → one ResourcePage`.

**Entry TASKs:** TASK-026–035 (slices under workspaces above)
