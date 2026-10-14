# Inventory Reservation

**Module:** Inventory

**Domain:** Reservation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Reservation capability prevents the same available inventory from being
promised to multiple demands while preserving a single Inventory source of
truth.

Inventory owns reservations. Sales, Planning, Production and Maintenance
request and consume them through contracts.

---

# Scope

Included:

- Reservation request
- Candidate selection
- Quantity commitment
- Allocation to an execution unit
- Partial consumption
- Release and expiration
- Concurrency control
- Batch and status restrictions

Excluded:

- Sales-order ownership
- Production scheduling
- Material master ownership
- Physical stock posting
- Business priority rules owned by source modules

---

# Terminology

- **Demand:** External business requirement requesting inventory
- **Reservation:** Quantity protected for a demand
- **Allocation:** Reserved quantity assigned to a specific execution unit,
  batch, package or work order
- **Consumption:** Posted Inventory issue that reduces stock and reservation
- **Release:** Unused quantity returned to availability

Reserved and Allocated quantities are both unavailable to unrelated demands.

---

# Aggregate

Aggregate Root: `InventoryReservation`

Contains:

- Reservation ID
- Source Module
- Demand Type
- Demand ID and Line ID
- Demand Version
- Company and plant
- Material
- Requested quantity and unit
- Reserved quantity
- Allocated quantity
- Consumed quantity
- Released quantity
- Status
- Priority reference
- Required date
- Expiration date
- Selection policy reference
- Lines
- Version
- Correlation ID

Reservation lines identify warehouse, location, batch/serial dimension,
inventory status and quantity.

---

# Lifecycle

```
Requested → Reserved → Partially Allocated → Allocated
Reserved or Allocated → Partially Consumed → Consumed
Requested or Reserved or Allocated → Released
Requested or Reserved → Expired
Requested → Rejected
```

Every quantity transition preserves:

```
Reserved Total =
Available Reserved
+ Allocated
+ Consumed
+ Released
```

The exact field model may use signed movements, but the invariant must remain
testable.

---

# Availability

Reservation operates on the canonical Inventory availability projection:

```
Available =
On Hand
- Reserved
- Allocated
- Blocked
- Quality Hold
```

Incoming quantity cannot be reserved as On Hand. Planning may create a future
supply commitment, but it remains distinct from an Inventory reservation until
stock is posted.

---

# Candidate Selection

Inventory validates candidates against:

- Company and plant scope
- Warehouse and location eligibility
- Material
- Batch/serial requirements
- Inventory status
- Quality hold
- Expiration/shelf life
- Required date
- Unit compatibility

FIFO, FEFO, preferred warehouse and customer-specific selection are policy
inputs. Their business precedence must be configured or approved by the owning
domain; Inventory shall not invent it.

---

# Concurrency

Reservation creation and changes execute in one Inventory local transaction.

Affected availability rows use expected versions. If concurrent requests
compete for the same quantity, only transactions that satisfy the invariant at
commit may succeed.

Oversubscription is prohibited.

---

# Partial Fulfilment

A request declares whether partial reservation is allowed.

If partial reservation is not allowed, the command either reserves the full
quantity or creates no reservation.

If allowed, the result states requested, reserved and shortage quantities. It
shall not silently treat a partial reservation as complete.

---

# Allocation

Allocation binds reserved quantity to a specific execution reference such as:

- Production order or work order
- Shipment or picking task
- Maintenance order
- Approved manual execution reference

Allocation does not post stock. Consumption requires an Inventory transaction.

---

# Consumption

Consumption:

1. Validates reservation and allocation state.
2. Posts an Inventory issue through the Inventory Ledger.
3. Updates consumed reservation quantity in the same Inventory transaction.
4. Publishes reservation and ledger events atomically through the outbox.

Consumption cannot exceed remaining allocated or reserved quantity according
to the approved demand policy.

---

# Release and Expiration

Release requires a source command, authorized manual command or approved
workflow.

Expiration is configured by reservation type. Expiration creates an auditable
state transition and notification. It shall not delete the reservation.

Allocated reservations do not expire automatically unless the owning business
process explicitly permits it.

---

# API

```
GET  /api/v1/inventory-reservations
GET  /api/v1/inventory-reservations/{id}
POST /api/v1/inventory-reservations
POST /api/v1/inventory-reservations/{id}/allocate
POST /api/v1/inventory-reservations/{id}/consume
POST /api/v1/inventory-reservations/{id}/release
POST /api/v1/inventory-reservations/{id}/extend
```

Commands require an idempotency key and expected version.

---

# Events

- InventoryReservationRequested
- InventoryReserved
- InventoryPartiallyReserved
- InventoryReservationRejected
- InventoryAllocated
- InventoryReservationConsumed
- InventoryReservationReleased
- InventoryReservationExpired

Source modules publish their own lifecycle facts and shall not publish
Inventory reservation facts on behalf of Inventory.

---

# Database

Canonical tables:

- `inventory_reservations`
- `inventory_reservation_lines`
- `inventory_reservation_history`

Reservations reference external demand identifiers without cross-module
foreign keys.

Reservation history is append-only. Current state uses optimistic concurrency.

---

# Authorization

Permissions distinguish:

- View
- Request
- Allocate
- Consume
- Release
- Extend
- Override Selection Policy

Warehouse, plant and source-module scope are enforced server-side.

---

# Audit

Request, selection, reservation, rejection, allocation, consumption, release,
expiration and policy override are audited with actor, reason, quantities and
previous/new state.

---

# Acceptance Criteria

- Inventory is never over-reserved.
- Reservation has one owning module.
- Reserved and allocated quantities use distinct semantics.
- Partial results are explicit.
- Consumption and stock issue commit atomically inside Inventory.
- Duplicate commands are idempotent.
- Expiration and release remain traceable.
- Selection policies are configured, not embedded in UI or source modules.

---

# Related Documents

- `Inventory_Architecture.md`
- `Inventory_Ledger.md`
- `TASK-019_Inventory.md`
- `../99_Shared/Concurrency.md`
- `../99_Shared/Transactions.md`
- `../../00_Project_Governance/Module_Boundaries_and_Ownership.md`
