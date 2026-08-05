# ID Generation

**Module:** Shared

**Category:** Identity Generation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The ID Generation standard defines how unique identifiers are generated, managed and used throughout Naswood OS.

It provides globally unique, immutable and scalable identifiers for business entities, integrations, events and distributed services.

Technical identifiers are independent from business document numbers.

---

# Objectives

- Globally Unique IDs
- Immutable Entity Identity
- Distributed System Support
- Predictable API Design
- High Performance
- Cross-System Compatibility

---

# Design Principles

Identifiers should be

Unique

Immutable

Opaque

Stable

Technology Independent

Identifiers must never contain business meaning.

---

# Identity Types

Entity ID

Document Number

Event ID

Correlation ID

Request ID

Session ID

File ID

Batch ID

Message ID

Notification ID

---

# Internal Entity ID

Every entity receives

One immutable identifier

Generated once

Never reused

Never modified

---

# Recommended Format

Preferred

UUID v7

Alternative

ULID

Technology-specific implementations are acceptable if they provide globally unique, sortable identifiers.

---

# Identifier Characteristics

Supports

Global Uniqueness

Sortability (Preferred)

Distributed Generation

Offline Generation

Collision Resistance

---

# Identifier Scope

Company

Plant

User

Machine

Order

Material

Document

Event

File

Notification

Each identifier is unique within the entire platform.

---

# Business IDs

Business document numbers are managed separately.

Reference

Document_Numbering.md

---

# Correlation IDs

Every incoming request receives

Correlation ID

Used across

API

Logs

Events

Background Jobs

Reference

Logging.md

---

# Event IDs

Every published event has

Unique Event ID

Occurred At

Correlation ID

Reference

Event_Model.md

Integration_Events.md

---

# File IDs

Every stored file has

Immutable File ID

Independent from file name

Reference

File_Storage.md

---

# Batch IDs

Supports

Production Batch

Inventory Batch

Shipment Batch

Quality Batch

---

# API

Supports

Client-generated IDs (where appropriate)

Server-generated IDs

Idempotency Keys

Reference

API_Standards.md

Concurrency.md

---

# Offline Support

Supports

Offline Mobile ID Generation

Synchronization

Conflict-Free Identity Creation

Reference

Offline_UI.md

---

# Security

Identifiers

Must not expose business information

Must not reveal record counts

Must not contain sensitive data

Reference

Security.md

---

# Database

Primary Keys should use the shared identifier strategy.

Foreign Keys reference immutable identifiers.

---

# AI

AI-generated entities must obtain identifiers through the shared platform service.

Reference

AI_Copilot.md

---

# Performance

Supports

Distributed Generation

No Central Bottleneck

Efficient Indexing

Sequential-Friendly Storage

Reference

Performance.md

---

# Monitoring

Track

Generated IDs

Collision Attempts

Generation Failures

Latency

Reference

Monitoring.md

---

# Audit

Track

Identifier Generation Failures

Manual Overrides (if any)

Reference

Audit_Log.md

---

# Best Practices

✓ Keep IDs immutable.

✓ Separate technical IDs from business numbers.

✓ Use globally unique identifiers.

✓ Generate IDs before persistence.

✓ Propagate correlation IDs.

✓ Never reuse identifiers.

---

# Do

✓ Use UUID v7 or ULID

✓ Keep identifiers opaque

✓ Generate immutable IDs

✓ Use correlation IDs

✓ Keep business numbering separate

---

# Don't

✗ Use auto-increment IDs in public APIs

✗ Embed business meaning in IDs

✗ Reuse deleted identifiers

✗ Expose sequential IDs externally

✗ Change identifiers after creation

---

# Acceptance Criteria

Identifiers are globally unique.

Technical IDs are immutable.

Business numbering is independent.

Distributed generation is supported.

Correlation IDs propagate across services.

Performance targets are achieved.

---

# Related Documents

Document_Numbering.md

API_Standards.md

Concurrency.md

Logging.md

Audit_Log.md

Event_Model.md

Integration_Events.md

File_Storage.md

Security.md

Performance.md
