# Soft Delete

**Module:** Shared

**Category:** Data Lifecycle Management

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Soft Delete standard defines how business entities are logically removed from active use while preserving historical integrity, auditability and referential consistency throughout Naswood OS.

Soft deletion prevents accidental data loss and supports recovery, compliance and long-term traceability.

Physical deletion is the exception, not the default behavior.

---

# Objectives

- Preserve Business History
- Prevent Accidental Data Loss
- Maintain Referential Integrity
- Support Data Recovery
- Enable Regulatory Compliance
- Standardize Entity Lifecycle

---

# Design Principles

Deletion should be

Logical

Recoverable

Auditable

Consistent

Secure

Configurable

Deleted records remain part of the business history unless permanently purged according to retention policies.

---

# Data Lifecycle

```
Created

↓

Active

↓

Inactive (Optional)

↓

Archived (Optional)

↓

Soft Deleted

↓

Retention Period

↓

Permanent Purge (Optional)
```

---

# Lifecycle States

Draft

Active

Inactive

Archived

Soft Deleted

Purged

---

# Soft Delete Model

Every soft-deletable entity includes

```
isDeleted

deletedAt

deletedBy

deleteReason

restoreAt (Optional)

retentionUntil (Optional)
```

---

# Required Fields

isDeleted

Boolean

deletedAt

UTC Timestamp

deletedBy

User Identifier

deleteReason

Optional

retentionUntil

Optional

---

# Entity Behavior

Soft deleted entities

Remain in the database

Are excluded from normal searches

Remain auditable

Can participate in historical reports

May be restored (subject to business rules)

---

# Query Rules

Default queries exclude soft deleted records.

Administrative queries may include

Active

Deleted

Archived

All Records

---

# Restore

Supports

Manual Restore

Bulk Restore

Approval-Based Restore

Reference

Approval_Workflow.md

---

# Permanent Purge

Permanent deletion requires

Administrative Permission

Retention Validation

Audit Logging

Approval (Configurable)

Once purged, recovery is not guaranteed.

---

# Cascade Rules

Soft delete should not automatically cascade unless explicitly configured.

Examples

Deleting a Customer

↓

Does NOT delete

Invoices

Orders

Payments

Production Records

Business history must remain intact.

---

# Referential Integrity

Soft deleted entities continue to satisfy foreign key relationships.

No orphaned records should be created.

---

# Archive vs Soft Delete

Archive

Hidden from operational workflows but still active for historical use.

Soft Delete

Marked as deleted and excluded from standard operations.

Purge

Physically removed after retention rules are satisfied.

---

# Retention Policy

Retention periods are configurable by entity type.

Examples

Audit Records

Never Purged Automatically

Notifications

90 Days

Temporary Files

30 Days

Documents

7 Years

Production Records

10 Years

Financial Records

According to legal requirements

---

# Search

Supports

Include Deleted

Only Deleted

Restore Candidates

Deleted By

Deletion Date

Reference

Search_Filtering.md

---

# User Interface

Supports

Deleted Badge

Restore Action

Deletion Details

Confirmation Dialog

Bulk Restore

Recycle Bin View

---

# Security

Supports

Role-Based Delete

Role-Based Restore

Purge Permission

Audit Logging

Reference

Permission_Model.md

Security.md

---

# Audit

Track

Delete

Restore

Purge

Delete Reason

Retention Expiry

Reference

Audit_Log.md

---

# API

Example Endpoints

```
DELETE /materials/{id}

POST /materials/{id}/restore

DELETE /materials/{id}/purge

GET /materials?includeDeleted=true
```

DELETE performs a soft delete by default.

Permanent purge requires a dedicated endpoint and elevated permissions.

---

# Performance

Supports

Indexed isDeleted Field

Filtered Indexes

Query Optimization

Archival Optimization

Reference

Performance.md

---

# AI Integration

AI may

Explain deletion history

Identify restore candidates

Recommend archival

Detect unusual deletion patterns

Reference

AI_Copilot.md

---

# Notifications

Supports

Deletion Confirmation

Restore Notification

Retention Expiry Reminder

Reference

Notification_System.md

---

# Monitoring

Track

Deleted Records

Restore Count

Purge Count

Retention Expiry

Deletion Trends

Reference

Monitoring.md

---

# Best Practices

✓ Soft delete by default.

✓ Keep deletion metadata.

✓ Preserve referential integrity.

✓ Define retention periods.

✓ Audit every lifecycle transition.

✓ Require elevated permissions for purge.

---

# Do

✓ Exclude deleted records from normal queries

✓ Allow controlled restoration

✓ Log every deletion

✓ Protect historical business data

✓ Validate retention before purge

---

# Don't

✗ Physically delete business records by default

✗ Cascade deletes without explicit rules

✗ Remove audit history

✗ Hide deletion metadata

✗ Allow unrestricted purge operations

---

# Acceptance Criteria

Soft delete is implemented consistently across all entities.

Deleted records remain recoverable when allowed.

Retention policies are enforced.

Purge operations are controlled and audited.

Referential integrity is preserved.

Performance remains acceptable with large datasets.

---

# Related Documents

Entity_Rules.md

Audit_Log.md

Permission_Model.md

Security.md

Search_Filtering.md

Notification_System.md

Performance.md

Approval_Workflow.md

Architecture.md
