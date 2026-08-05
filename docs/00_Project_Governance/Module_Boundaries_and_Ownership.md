# Module Boundaries and Ownership

**Project:** Naswood Operating System (NOS)

**Document:** Module Boundaries and Ownership

**Code:** GOV-005

**Version:** 1.0

**Status:** Active

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
| Product Management | Product definitions, product revisions, classifications and product lifecycle | Physical Material, sales transactions, production execution and inventory balance | Approved |
| Inventory | Warehouses, locations, stock ledger, availability, reservations, inventory movements | Material definition, purchase orders, sales orders, production orders | Approved |
| Purchasing | Suppliers, purchase requests, RFQs, supplier quotations, purchase orders, purchase returns, supplier invoices | Inventory balances, quality decisions, financial postings | Approved |
| Sales | Customer commercial master, quotations, sales orders and commercial commitments | Product definition, physical material, production execution, inventory balance, financial posting | Approved |
| CRM | Leads, opportunities, activities, interactions and relationship history | Customer legal/commercial master, quotations and sales orders | Approved |
| Planning | Demand plans, MRP results, capacity plans, schedules and recommendations | Source sales orders, inventory balances, machine master, production execution | Proposed |
| Manufacturing | Physical Material, material genealogy, machine, work center, production line, tooling, process capability and process parameters | Production-order execution and inventory balance | Approved |
| Production | Production orders, work orders, operations, confirmations, WIP, scrap and rework | Resource master, stock ledger and quality disposition | Approved |
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
| Customer | Sales | CRM, Planning, Logistics, Finance, Analytics | Approved |
| Lead and opportunity | CRM | Sales, Analytics, AI Copilot | Approved |
| Supplier | Purchasing | Quality, Inventory, Finance, Maintenance | Approved |
| Warehouse and location | Inventory | Purchasing, Sales, Production, Logistics | Approved |
| Inventory ledger and balance projection | Inventory | All operational modules | Approved |
| Reservation | Inventory | Sales, Planning, Production, Maintenance | Approved |
| Machine and production capability | Manufacturing | Planning, Production, Maintenance, IoT, Digital Twin | Approved |
| Employee | HR | Platform identity link, Planning, Production, Maintenance | Approved |
| Product | Product Management | Sales, Planning, Manufacturing, Production, Quality, Finance | Approved |
| Physical Material | Manufacturing | Purchasing, Inventory, Production, Quality, Logistics | Approved |
| Material genealogy | Manufacturing | Production, Inventory, Quality, Logistics, Digital Twin | Approved |
| BOM | Manufacturing or Planning | Sales, Planning, Production, Costing | Pending |
| Routing | Manufacturing | Planning, Production, Quality | Approved |
| Production order | Production | Planning, Inventory, Quality, Finance | Approved |
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

# 6. Approved Business Boundaries

## 6.1 Production and Manufacturing

Manufacturing owns reusable resources, physical Material, material genealogy
and process definitions;
Production owns execution instances.

## 6.2 Sales and CRM

Sales owns Customer, Quotation and Sales Order; CRM owns
pre-sale relationship and interaction data.

Sales does not own Product definition. It references the authoritative Product
identifier and manages only sales-specific terms such as quotation price,
discount, delivery commitment and customer-specific commercial conditions.

## 6.3 Material

Material is the physical traceable instance owned by Manufacturing. Inventory
owns its quantity, location, reservation and stock status without owning or
mutating Material identity or genealogy.

## 6.4 Product

Product Management owns Product identity, definition, classification, revision
history and lifecycle.

Sales references released Product identifiers and manages customer-specific
commercial terms without modifying Product definitions.

Manufacturing and Production consume released Product revisions. The exact
Product-to-Material transition and BOM ownership remain pending business
decisions.

---

# 7. Approval Gate

No implementation may create BOM persistence or Product-to-Material commands
until the corresponding Pending decisions are Approved. Planning remains
blocked until its Proposed domain scope and pending business policies are
Approved.

---

# 8. Related Documents

- `Phase_0_Architecture_Resolution.md`
- `Architecture_Decisions.md`
- `docs/13_Design/99_Shared/Architecture.md`
- `docs/13_Design/99_Shared/Event_Model.md`
- `docs/13_Design/99_Shared/Transactions.md`
