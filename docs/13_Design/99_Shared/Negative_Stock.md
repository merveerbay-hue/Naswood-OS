# Negative Stock

**Module:** Shared

**Category:** Inventory Integrity

**Version:** 1.0

**Status:** Approved

---

# Purpose

This standard defines the NOS invariant that posted physical inventory never
becomes negative.

---

# Governing Rule

For every Inventory stock key:

```
On Hand >= 0
```

The rule has no configuration exception.

Company, plant, warehouse, location, material, role, user, integration and AI
settings shall not override it.

---

# Scope

The invariant applies to:

- Goods issues
- Production consumption
- Transfers
- Shipment issues
- Maintenance spare-part issues
- Scrap postings
- Returns that reduce stock
- Inventory adjustments
- Reversals
- Imported and integrated transactions

It applies at the canonical stock-key granularity defined by
`Inventory_Ledger.md`.

---

# Shortage Representation

Insufficient inventory is represented as:

- Reservation shortage
- Backorder
- Material shortage
- Planning exception
- Purchase recommendation
- Production exception
- Rejected Inventory posting

A shortage is demand state, not physical stock.

Incoming, planned or expected supply shall not increase On Hand before its
Inventory receipt is posted.

---

# Posting Behavior

Before posting, Inventory:

1. Loads the affected stock-key projection and expected version.
2. Applies all proposed ledger entries.
3. Rejects the complete local transaction if any resulting On Hand quantity is
   below zero.
4. Records the failure with correlation and source-document context.
5. Publishes `NegativeInventoryPrevented` where operational notification is
   required.

The server shall not partially post a transaction to avoid the invariant.

---

# Concurrency

Concurrent transactions use the Inventory Ledger concurrency rules.

If two valid requests compete for the final available quantity, only the
transaction that commits against the valid stock version may succeed. The
other request is re-evaluated and rejected when insufficient stock remains.

---

# Reservations

Reservations and allocations reduce Available quantity but do not reduce On
Hand. Oversubscription is prohibited by the Reservation aggregate.

Consumption posts the stock issue and reservation consumption atomically in
Inventory.

---

# Corrections

A reversal or correction is also subject to the invariant.

If reversing a historical receipt would create negative stock because the
quantity was subsequently consumed, the reversal is rejected. The owning
business process must use an approved corrective transaction that reflects
physical reality.

---

# API Error

The canonical failure uses HTTP 409:

```json
{
  "success": false,
  "data": null,
  "message": "Inventory posting would create negative stock.",
  "errors": [
    {
      "code": "INV-409",
      "category": "Conflict",
      "field": null,
      "message": "Insufficient on-hand quantity for the requested posting.",
      "details": {
        "stockKey": "protected-reference",
        "requestedQuantity": 10,
        "onHandQuantity": 7
      }
    }
  ],
  "metadata": {
    "correlationId": "uuid",
    "timestamp": "UTC"
  }
}
```

Sensitive stock dimensions are returned only when the caller is authorized.

---

# Audit and Monitoring

Rejected attempts record:

- Source module and document
- Actor
- Stock key
- Requested and available quantities
- Transaction type
- Reason
- Correlation ID
- Timestamp

Repeated failures are observable as operational metrics and may generate
alerts. They do not create ledger entries.

---

# AI Restrictions

AI may explain the shortage and recommend purchasing, rescheduling,
substitution or authorized reallocation.

AI shall not bypass the invariant, fabricate stock or silently change quantity
or dimensions.

---

# Acceptance Criteria

- No posted transaction can create negative On Hand.
- No configuration can weaken the invariant.
- Concurrent requests cannot oversubscribe stock.
- Shortages are represented separately from physical inventory.
- Failed postings are atomic, traceable and observable.
- Reversals obey the same invariant.
- UI and integrations cannot bypass server-side validation.

---

# Related Documents

- `../02_Inventory/Inventory_Ledger.md`
- `../02_Inventory/Reservation.md`
- `Concurrency.md`
- `Transactions.md`
- `Validation_Rules.md`
- `../../00_Project_Governance/Architecture_Decisions.md`
