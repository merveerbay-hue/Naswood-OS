# Inventory Ledger

**Module:** Inventory

**Domain:** Stock Ledger

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Inventory Ledger is the authoritative record of every quantity change in
NOS. Inventory balances are projections derived from immutable posted ledger
entries and shall never be edited directly.

---

# Ownership

Inventory owns:

- Inventory transaction
- Ledger entry
- Stock balance projection
- Availability projection
- Inventory posting and reversal

Source modules own their business documents. Purchasing owns purchase receipts,
Production owns production confirmations, Sales/Logistics own fulfilment
requests and Maintenance owns spare-part demand.

---

# Core Invariant

For a stock key:

```
Company
+ Plant
+ Warehouse
+ Location
+ Material
+ Batch or Serial Dimension
+ Inventory Status
```

the projected balance equals the sum of posted ledger-entry quantities.

The projection is rebuildable from the ledger.

---

# Transaction and Entry Model

`InventoryTransaction` is the aggregate root.

It contains:

- Transaction ID
- Transaction Type
- Status
- Source Module
- Source Document Type
- Source Document ID
- Source Document Version
- Business Date
- Posting Date
- Idempotency Key
- Correlation ID
- Reversal Reference
- Entries

Each `InventoryLedgerEntry` contains:

- Entry ID
- Transaction ID
- Sequence
- Stock dimensions
- Signed quantity
- Unit
- Base quantity
- Base unit
- Movement reason
- Cost reference
- Created At

A transfer contains balanced issue and receipt entries in one Inventory local
transaction.

---

# Transaction Types

- Opening Balance
- Goods Receipt
- Goods Issue
- Transfer
- Production Consumption
- Production Output
- Return
- Adjustment
- Reservation Consumption
- Quality Status Change
- Scrap
- Reversal

New types require architecture review. Modules shall not create private
movement types with equivalent meaning.

---

# Lifecycle

```
Draft → Validated → Posted
Draft → Cancelled
Posted → Reversed
```

- Draft may be changed by its owning command workflow.
- Validated has passed quantity, dimension, policy and authorization checks.
- Posted is immutable.
- Cancelled has no ledger entries.
- Reversed remains posted and links to a compensating transaction.

Completed and Posted shall not be separate meanings. `Posted` is the canonical
state indicating that stock has changed.

---

# Posting

Posting performs one local Inventory transaction:

1. Validate source request and idempotency.
2. Authorize actor and scope.
3. Validate dimensions and reference-data values.
4. Lock or version-check affected stock keys.
5. Reject the transaction if any resulting On Hand quantity would be negative,
   then apply inventory-status rules.
6. Create immutable ledger entries.
7. Update balance and availability projections.
8. Append outbox events.
9. Commit.

No external module call occurs inside the database transaction.

---

# Reversal

A posted transaction cannot be edited or deleted.

Reversal:

- references the original transaction
- creates entries with opposite quantities
- preserves original dimensions and units
- records actor, reason and authorization
- is itself immutable after posting

If subsequent business activity makes reversal unsafe, the command is rejected
and a domain-specific corrective process is required.

---

# Balance Projection

The balance projection contains:

- On Hand
- Reserved
- Allocated
- Available
- Blocked
- Quality Hold
- Incoming
- Outgoing
- Projection Version
- Last Ledger Sequence

Quantity meanings are distinct:

- **On Hand:** physically posted quantity
- **Reserved:** promised but not yet assigned to an execution unit
- **Allocated:** assigned to a specific execution unit
- **Blocked:** unavailable by Inventory policy
- **Quality Hold:** unavailable by Quality decision
- **Available:** On Hand minus Reserved, Allocated, Blocked and Quality Hold

Incoming and Outgoing are planning projections and do not alter On Hand.

---

# Concurrency

Posting uses optimistic concurrency on each affected stock key. A transaction
that observes a stale projection version is re-evaluated or rejected.

Inventory shall not rely on application-process locks. Where contention is
high, narrowly scoped PostgreSQL locking may protect the stock key inside the
local transaction.

Negative On Hand is prohibited without configuration exceptions. Shortage is
represented as demand or planning state, not as an Inventory balance.

---

# Source Integration

Source modules request Inventory posting through versioned commands or
integration events.

Required fields:

- Source Module
- Source Document Type and ID
- Source Version
- Transaction Type
- Stock dimensions
- Quantity and unit
- Business date
- Actor/context
- Idempotency key
- Correlation ID

Inventory publishes success or rejection. The source module does not assume
stock changed until success is received.

---

# API

Queries:

```
GET /api/v1/inventory
GET /api/v1/inventory/availability
GET /api/v1/inventory/transactions
GET /api/v1/inventory/transactions/{id}
GET /api/v1/inventory/ledger
```

Commands:

```
POST /api/v1/inventory-transactions
POST /api/v1/inventory-transactions/{id}/post
POST /api/v1/inventory-transactions/{id}/reverse
```

Public commands expose DTOs and policy-driven transaction types. They never
accept direct balance updates.

---

# Events

- InventoryTransactionPosted
- InventoryTransactionReversed
- InventoryBalanceChanged
- InventoryPostingRejected
- NegativeInventoryPrevented

`InventoryUpdated` is too generic and shall not be used for new contracts.

---

# Database

Canonical tables:

- `inventory_transactions`
- `inventory_ledger_entries`
- `inventory_balances`
- `inventory_availability`
- `inventory_posting_idempotency`
- `inventory_outbox`
- `inventory_inbox`

Ledger entries are append-only. Balance and availability are projections.

Partitioning and indexes shall support organization, stock dimensions,
business date, source document and ledger sequence.

---

# Audit

Audit records command intent, authorization, posting, rejection and reversal.
The ledger remains the quantity history; audit records remain the actor/action
history.

---

# Acceptance Criteria

- Every stock change has an immutable ledger transaction.
- Balances can be rebuilt from ledger entries.
- Direct balance editing is impossible.
- No posted stock key can have negative On Hand.
- Duplicate source requests do not create duplicate entries.
- Transfers balance inside one Inventory transaction.
- Posted transactions are corrected only by reversal or compensating process.
- Available quantity uses one canonical formula.
- Cross-module posting is idempotent and observable.

---

# Related Documents

- `Inventory_Architecture.md`
- `TASK-019_Inventory.md`
- `Reservation.md`
- `../99_Shared/Negative_Stock.md`
- `../99_Shared/Transactions.md`
- `../../00_Project_Governance/Phase_0_Canonical_Contracts.md`
