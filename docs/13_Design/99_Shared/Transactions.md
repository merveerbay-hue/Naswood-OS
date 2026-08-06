# Transactions

**Module:** Shared

**Category:** Transaction Management

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Transactions standard defines atomicity, consistency, idempotency and
failure-handling rules for state changes throughout Naswood Operating System.

---

# Principles

- One local transaction per owning module
- Aggregate consistency
- Explicit transaction boundary
- Optimistic concurrency
- Idempotent commands
- Outbox-based event publication
- Inbox-based event consumption
- Compensating actions for distributed processes
- Immutable posted transactions
- No hidden database behavior

---

# Local Transaction Boundary

A command executes inside one transaction owned by one module:

```
Validate Request
↓
Authorize
↓
Load Aggregate
↓
Execute Domain Behavior
↓
Persist Aggregate
↓
Append Outbox Events
↓
Commit
```

The aggregate and its outbox records commit atomically.

A command handler or application transaction coordinator owns the transaction
boundary. Repositories do not independently commit.

---

# Isolation

The default isolation level is PostgreSQL `READ COMMITTED`.

Higher isolation is selected only for a documented invariant that cannot be
protected with aggregate boundaries, constraints or optimistic concurrency.

Inventory allocation, numbering and financial posting require explicit
concurrency analysis. Serializable execution shall be narrowly scoped and
measured.

---

# Optimistic Concurrency

Mutable aggregates include a version.

Updates include the expected version. A mismatch aborts the transaction and
returns a concurrency conflict. The server shall not silently overwrite newer
state.

Automatic retries are allowed only when the command is idempotent and domain
behavior is safe to re-evaluate.

---

# Idempotency

Commands that may be retried include an idempotency key.

The owning module records:

- Caller identity
- Operation
- Idempotency key
- Request fingerprint
- Result reference
- Status
- Created At
- Expires At

Reusing a key with a different request is rejected. Reusing a completed key
with the same request returns the recorded outcome.

---

# Posted Business Transactions

Posted inventory, financial, quality, production and audit transactions are not
edited or deleted.

Corrections use a linked reversal, cancellation or compensating transaction.
The original and correction remain traceable.

Draft business documents may be edited according to their domain lifecycle and
concurrency rules.

---

# Cross-Module Processes

Distributed database transactions across modules are prohibited.

Cross-module processes use versioned commands/events and a saga or process
manager:

```
Request
↓
Owning Module Commit + Outbox
↓
Integration Event
↓
Consumer Inbox
↓
Consumer Local Commit
↓
Success or Failure Event
```

Every distributed process defines timeout, retry, failure and manual-
intervention behavior. Compensation is used only when the business operation
is legitimately reversible.

---

# Outbox

Outbox records include:

- Outbox ID
- Event ID
- Event Type
- Event Version
- Aggregate Type
- Aggregate ID
- Payload
- Correlation ID
- Causation ID
- Occurred At
- Published At
- Attempt Count
- Last Error

Publishing is retried without changing the business event identity.

---

# Inbox

Inbox records include:

- Consumer
- Event ID
- Event Version
- Received At
- Processed At
- Status
- Attempt Count
- Last Error

The unique Consumer and Event ID pair prevents duplicate side effects.

---

# Failure Handling

- A failed local transaction commits no partial state.
- A failed event publication leaves the outbox record pending.
- A failed consumer retains its inbox state and retries according to policy.
- Poison messages move to controlled manual intervention after the retry
  policy is exhausted.
- Failures are logged, monitored and correlated.
- A client never receives success before the owning local transaction commits.

---

# Database Rules

- Foreign keys enforce relationships inside a module boundary.
- Cross-module foreign keys are prohibited.
- Business rules do not belong in database triggers or stored procedures.
- Unique constraints protect natural invariants such as stable codes and
  idempotency keys.
- Migrations are version controlled.
- Production database changes are never applied manually.

---

# Transaction Observability

Every transaction records or propagates:

- Correlation ID
- Causation ID where applicable
- Module
- Command
- Actor
- Aggregate ID
- Duration
- Outcome

Sensitive payloads are not logged.

---

# Testing

Required tests include:

- Successful atomic commit
- Rollback on domain failure
- Rollback on persistence failure
- Optimistic concurrency conflict
- Duplicate idempotency key
- Outbox recovery
- Duplicate event delivery
- Consumer retry and poison-message handling
- Valid compensation
- Invalid compensation rejection

---

# Acceptance Criteria

- No command produces a partial local commit.
- Business state and outbox events commit atomically.
- Consumers are idempotent.
- Posted transactions remain immutable.
- Cross-module processes do not use distributed database transactions.
- Failures are observable and recoverable.
- Transaction ownership is explicit.

---

# Related Documents

- `Concurrency.md`
- `Event_Model.md`
- `Integration_Events.md`
- `Audit_Log.md`
- `Error_Handling.md`
- `../../00_Project_Governance/Phase_0_Canonical_Contracts.md`
