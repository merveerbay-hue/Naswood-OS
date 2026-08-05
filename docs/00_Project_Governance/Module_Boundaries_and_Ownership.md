# Module Boundaries and Ownership

**Project:** Naswood Operating System (NOS)

**Document:** Module Boundaries and Ownership

**Code:** GOV-005

**Version:** 1.0

**Status:** Proposed

---

# 1. Purpose

This document defines the canonical NOS module catalog, dependency direction
and entity ownership model.

Approved entries are binding. Proposed entries require accountable business
approval before implementation.

---

# 2. Ownership Rules

1. Every business capability has one owning module.
2. Every business entity has one authoritative system of record.
3. Modules expose contracts, not database entities.
4. Modules do not write another module's persistence.
5. A local projection is read-only with respect to its owning module.
6. Shared Platform services do not absorb business-domain ownership.
7. An event describes a completed fact and does not transfer ownership.
8. The Workflow Engine orchestrates process progression but does not own the
   business decisions or entities participating in the process.

---

# 3. Canonical Module Catalog

| Module | Owns | Does not own | Status |
|---|---|---|---|
| Platform | Identity, permissions, configuration, audit, notifications, numbering, localization, observability | Commercial or operational business entities | Approved |
| Inventory | Warehouses, locations, stock ledger, availability, reservations, inventory movements | Material definition, purchase orders, sales orders, production orders | Approved |
| Purchasing | Suppliers, purchase requests, RFQs, supplier quotations, purchase orders, purchase returns, supplier invoices | Inventory balances, quality decisions, financial postings | Approved |
| Sales | Customer commercial master, quotations, sales orders and commercial commitments | Physical material, production execution, inventory balance, financial posting | Proposed |
| CRM | Leads, opportunities, activities, interactions and relationship history | Customer legal/commercial master, quotations and sales orders | Proposed |
| Planning | Demand plans, MRP results, capacity plans, schedules and recommendations | Source sales orders, inventory balances, machine master, production execution | Proposed |
| Manufacturing | Machine, work center, production line, tooling, process capability and process parameters | Production-order execution and inventory balance | Proposed |
| Production | Production orders, work orders, operations, confirmations, WIP, scrap and rework | Resource master, stock ledger and quality disposition | Proposed |
| Quality | Inspection plans, inspections, holds, nonconformance, CAPA and certificates | Inventory quantity, supplier master and production execution | Approved |
| Maintenance | Asset maintenance lifecycle, work requests, maintenance orders, plans, downtime and failure history | Machine capability master and spare-part inventory balance | Approved |
| Logistics | Picking, loading, shipment, transport, delivery, proof of delivery and export execution | Warehouse master, stock ledger and sales-order commercial terms | Approved |
| Finance | Chart of accounts, fiscal periods, journals, receivables, payables, payments, valuation and financial reporting | Operational source documents and operational approvals | Approved |
| HR | Employees, employment lifecycle, skills, attendance and workforce availability | User authentication, production execution and maintenance orders | Approved |
| Document Management | Files, versions, metadata, retention and document links | Business entities to which documents are linked | Approved |
| Workflow Engine | Definitions, versions, instances, tasks, escalation, delegation and execution history | Business rules, business approvals or source entities | Approved |
| AI Copilot | Conversations, governed knowledge indexes, recommendations and explanation records | Autonomous approval, financial posting or master-data authority | Approved |
| Digital Twin | Twin models, synchronization state, simulation scenarios and visualization | Transactional truth for inventory, production or assets | Approved |
| IoT | Device identity, connectivity, telemetry ingestion and controlled command delivery | Business interpretation of machine or production state | Approved |
| Analytics | Analytical models, KPI definitions, aggregates and dashboards | Transactional system of record | Approved |
| Public APIs | External contract publication, access policy, quotas and developer lifecycle | Domain logic or domain data ownership | Approved |

---

# 4. Entity Ownership Matrix

| Entity or capability | Owner | Consumers | Status |
|---|---|---|---|
| User identity and session | Platform | All modules | Approved |
| Role, permission and policy | Platform | All modules | Approved |
| Company, plant and organization structure | Platform | All modules | Approved |
| Customer | Sales | CRM, Planning, Logistics, Finance, Analytics | Proposed |
| Lead and opportunity | CRM | Sales, Analytics, AI Copilot | Proposed |
| Supplier | Purchasing | Quality, Inventory, Finance, Maintenance | Approved |
| Warehouse and location | Inventory | Purchasing, Sales, Production, Logistics | Approved |
| Inventory ledger and balance projection | Inventory | All operational modules | Approved |
| Reservation | Inventory | Sales, Planning, Production, Maintenance | Approved |
| Machine and production capability | Manufacturing | Planning, Production, Maintenance, IoT, Digital Twin | Proposed |
| Employee | HR | Platform identity link, Planning, Production, Maintenance | Approved |
| Product | Undecided | Sales, Planning, Production, Quality, Finance | Pending |
| Physical Material | Undecided | Purchasing, Inventory, Production, Quality, Traceability | Pending |
| Material genealogy | Undecided | Production, Inventory, Quality, Logistics, Digital Twin | Pending |
| BOM | Manufacturing or Planning | Sales, Planning, Production, Costing | Pending |
| Routing | Manufacturing | Planning, Production, Quality | Proposed |
| Production order | Production | Planning, Inventory, Quality, Finance | Proposed |
| Quality hold/disposition | Quality | Inventory, Production, Purchasing, Logistics | Approved |
| Shipment and delivery | Logistics | Sales, Inventory, Finance, CRM | Approved |
| Business document file | Document Management | All modules | Approved |
| Workflow definition and instance | Workflow Engine | All modules | Approved |
| Audit record | Platform | Compliance and Analytics | Approved |
| Financial journal | Finance | Analytics and source-document modules | Approved |

---

# 5. Boundary Clarifications

## 5.1 Inventory and Logistics

Inventory owns warehouse structure, stock state and movements. Logistics owns
fulfilment execution. A shipment may request an Inventory issue, but Logistics
shall not update stock tables.

Packages used only as stock-containment units belong to Inventory. Shipment
packages, pallets, containers and loading plans belong to Logistics and
reference Inventory containment identifiers.

## 5.2 Purchasing and Inventory Goods Receipt

Purchasing owns the supplier-facing receipt document and purchase-order
received quantity. Inventory owns the stock receipt transaction.

Posting the purchasing receipt publishes an idempotent
`InventoryReceiptRequested` integration event. Inventory records the request,
posts or rejects the stock transaction, and publishes the result. A shared
database transaction across the two modules is prohibited.

## 5.3 Quality Holds

Quality owns the decision and reason. Inventory owns enforcement on
availability. Quality publishes hold/release facts; Inventory applies them
idempotently without duplicating inspection rules.

## 5.4 Finance

Finance consumes approved source-document facts and records their financial
impact. It does not redefine Sales, Purchasing, Inventory or Production
lifecycles.

Finance may publish facts about its own completed work, such as
`JournalPosted`, `InvoiceSettled` and `FiscalPeriodClosed`. It shall not publish
operational facts on behalf of another module.

## 5.5 Digital Twin and Analytics

Digital Twin and Analytics are derived consumers. Neither is authoritative for
transactional state. Corrections occur in the owning source module and are
propagated through versioned events.

---

# 6. Pending Decision Options

## 6.1 Production and Manufacturing

**Recommended:** Manufacturing owns reusable resources and process definitions;
Production owns execution instances.

Alternative: merge both into one Manufacturing module. This reduces boundary
complexity but weakens independent replacement of planning/resource definition
and shop-floor execution.

## 6.2 Sales and CRM

**Recommended:** Sales owns Customer, Quotation and Sales Order; CRM owns
pre-sale relationship and interaction data.

Alternative: CRM owns Customer. This requires Sales to depend on CRM for every
commercial transaction and conflicts with the existing constitutional
Customer-to-Sales ownership statement.

## 6.3 Product and Material

**Recommended:** Product is the commercial/design definition; Material is the
physical traceable instance. The Product owner and Material owner require
business approval.

A production completion may create Material instances referencing the approved
Product revision. It shall not convert or mutate a Product record.

---

# 7. Approval Gate

No implementation may create Product, Material, genealogy, BOM, Production or
CRM persistence until the corresponding Pending or Proposed ownership entries
are Approved.

---

# 8. Related Documents

- `Phase_0_Architecture_Resolution.md`
- `Architecture_Decisions.md`
- `docs/13_Design/99_Shared/Architecture.md`
- `docs/13_Design/99_Shared/Event_Model.md`
- `docs/13_Design/99_Shared/Transactions.md`
