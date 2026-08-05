# Phase 0 Issue Register

**Project:** Naswood Operating System (NOS)

**Document:** Phase 0 Issue Register

**Code:** GOV-007

**Version:** 1.0

**Status:** Active

---

# 1. Purpose

This register records architecture and documentation conflicts discovered
during the complete Phase 0 repository review.

An issue is Closed only when every affected lower-level document conforms to
the approved decision.

---

# 2. Status Definitions

- **Open:** No approved resolution
- **Decision Approved:** Canonical decision exists; document alignment remains
- **Blocked:** Requires accountable business input
- **Closed:** Decision and all affected documentation are aligned

---

# 3. Critical Issues

| ID | Issue | Resolution | Status |
|---|---|---|---|
| P0-001 | Constitution/Architecture authority conflict | Constitution declared highest authority in ADR-001 | Decision Approved |
| P0-002 | Production and Manufacturing overlap | Manufacturing owns Production Master and genealogy; Production owns execution | Decision Approved |
| P0-003 | Sales, CRM and Customer ownership overlap | Sales owns Customer/commercial transactions; CRM owns pre-sale relationships | Decision Approved |
| P0-004 | Product, Material and genealogy ownership missing | Product Management owns Product; Inventory owns Material; Manufacturing owns genealogy | Decision Approved |
| P0-005 | Planning module lacks complete architecture/design/tasks | Architecture created; domain decisions and implementation tasks remain | Blocked |
| P0-006 | Reference Data standard empty | Standard completed | Closed |
| P0-007 | Transaction standard empty | Standard completed | Closed |
| P0-008 | Workflow Engine design missing | Canonical ownership, domain model and contract completed | Closed |
| P0-009 | Inventory ledger/stock movement has no dedicated canonical task | Canonical design created; implementation task remains required | Open |
| P0-010 | Reservation duplicated across Material and Inventory | Inventory ownership approved; dependent docs require alignment | Decision Approved |
| P0-011 | Purchasing GR and Inventory GR consistency undefined | Event-driven saga boundary approved | Decision Approved |
| P0-012 | SQL Server/PostgreSQL conflict | PostgreSQL approved in ADR-003 | Decision Approved |
| P0-013 | API envelopes and error shapes conflict | Canonical contract approved in ADR-005 | Decision Approved |
| P0-014 | Pagination and sorting syntax conflict | Canonical contract approved in ADR-005 | Decision Approved |
| P0-015 | Event names and payloads drift by module | Canonical event schema approved; event catalog alignment required | Decision Approved |
| P0-016 | RBAC versus RBAC+ABAC conflict | Hybrid RBAC+policy/ABAC approved | Decision Approved |
| P0-017 | Finance posting and reversal rules incomplete | Domain rules require Finance approval | Blocked |
| P0-018 | Negative inventory absolute versus configurable | Negative On Hand prohibited without exceptions by ADR-009 | Decision Approved |
| P0-019 | Audit retention conflicts | Compliance retention decision required | Blocked |
| P0-020 | Platform specs Draft while implementation tasks assume approval | Complete and approve Platform designs before implementation | Open |

---

# 4. Domain and Design Issues

| ID | Issue | Required action | Status |
|---|---|---|---|
| P0-021 | Warehouse, area and location hierarchies differ | Approve one hierarchy and migrate all designs | Open |
| P0-022 | Inventory and Logistics both own packages/shipments | Apply boundary in ownership document | Decision Approved |
| P0-023 | Multiple recipe models | Define canonical recipe aggregate and specializations | Blocked |
| P0-024 | Multiple batch concepts lack taxonomy | Define batch types and invariants | Blocked |
| P0-025 | Material status and inventory status overlap | Define separate lifecycle/state vocabularies | Blocked |
| P0-026 | Quality hold ownership duplicated | Quality decision/Inventory enforcement approved | Decision Approved |
| P0-027 | Shipment and Delivery lifecycles overlap | Define fulfillment aggregate boundaries | Blocked |
| P0-028 | Production Planning and Scheduling overlap | Resolve in Planning architecture | Open |
| P0-029 | Scrap and rework have duplicate entry paths | Define canonical command and projections | Blocked |
| P0-030 | OEE ownership duplicated | Assign KPI calculation authority | Blocked |
| P0-031 | Customer and Supplier tables duplicated across modules | Consolidate owners; convert other copies to projections | Decision Approved |
| P0-032 | Quality grade vocabularies conflict | Business classification mapping required | Blocked |
| P0-033 | Machine code standards conflict | Approve one numbering strategy | Open |
| P0-034 | Approval chains and thresholds differ | Define configurable approval matrices | Blocked |
| P0-035 | API rate limits conflict | Define consumer classes and quotas | Open |

---

# 5. Missing Documentation

| ID | Missing or incomplete artifact | Status |
|---|---|---|
| P0-036 | Planning Architecture, Domain and Design | Blocked |
| P0-037 | Workflow Engine Domain, API and persistence model | Closed |
| P0-038 | Inventory Ledger/Stock Movement canonical design | Closed |
| P0-039 | Reservation canonical design | Closed |
| P0-040 | Product-to-Material transition rules | Closed |
| P0-041 | Finance double-entry, posting, reversal and period-lock rules | Blocked |
| P0-042 | Log Yard specification | Blocked |
| P0-043 | Thermowood Certificate specification | Blocked |
| P0-044 | Event type registry and payload catalog | Open |
| P0-045 | Multi-tenancy and isolation model | Open |
| P0-046 | Backup retention and RPO/RTO | Blocked |
| P0-047 | AI model/provider governance and model registry | Open |
| P0-048 | Digital Twin synchronization and reconciliation | Open |
| P0-049 | Public API consumer/deprecation policy | Open |
| P0-050 | IoT controlled-write approval path | Blocked |

---

# 6. Implementation Task Integrity

| ID | Issue | Required action | Status |
|---|---|---|---|
| P0-051 | Sprint references do not match actual sprint contents | Rebuild dependency graph from task IDs | Open |
| P0-052 | Incorrect task filenames and IDs in related documents | Correct cross-references | Open |
| P0-053 | Java security annotations conflict with .NET baseline | Replace with technology-neutral authorization requirements | Decision Approved |
| P0-054 | Inventory tasks omit `/api/v1` | Align all endpoint specifications | Decision Approved |
| P0-055 | Tasks expose duplicate Role, Permission and Navigation APIs | Assign one Platform service owner per API | Decision Approved |
| P0-056 | Several tasks lack authorization requirements | Add permission and scope requirements | Open |
| P0-057 | Several tasks lack event contracts | Add canonical events or state explicitly that none are published | Open |
| P0-058 | Empty task/design placeholders treated as dependencies | Replace with approved content or remove dependency | Open |

---

# 7. Business Decision Queue

The accountable business owner must still approve:

1. Planning policies and delivery-commitment authority
2. Costing and valuation methods
3. Approval matrices and segregation of duties
4. Quality classifications and mappings
5. Recipe and batch taxonomy
6. Finance posting/reversal rules
7. Retention, RPO and RTO requirements

Architecture shall present options and consequences. It shall not invent these
rules.

---

# 8. Exit Criteria

Phase 0 cannot close while any Critical issue is Open or Blocked.

Affected modules cannot enter implementation while their Domain, Design,
contract or ownership issues remain Open or Blocked.

---

# 9. Related Documents

- `Phase_0_Architecture_Resolution.md`
- `Module_Boundaries_and_Ownership.md`
- `Phase_0_Canonical_Contracts.md`
- `Architecture_Decisions.md`
