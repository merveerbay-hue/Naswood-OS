# Event Model

**Module:** Shared

**Category:** Event-Driven Architecture

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Event Model defines how business events are created, published, consumed and tracked across Naswood OS.

Events enable loose coupling between modules, support real-time processing and provide a consistent foundation for integrations, notifications, AI services and Digital Twin.

All business events must follow the shared Event Model.

---

# Objectives

- Standardize Business Events
- Enable Event-Driven Architecture
- Support Real-Time Processing
- Improve Integration
- Increase Scalability
- Ensure Full Traceability

---

# Design Principles

Events should be

Immutable

Descriptive

Versioned

Traceable

Asynchronous

Business Focused

Events describe something that has already happened.

---

# Event Lifecycle

```
Business Action

↓

Domain Event

↓

Event Bus

↓

Subscribers

↓

Processing

↓

Audit
```

---

# Event Categories

Domain Events

Integration Events

System Events

Notification Events

AI Events

Digital Twin Events

Security Events

Workflow Events

---

# Event Structure

```
Header

↓

Metadata

↓

Payload

↓

Context
```

---

# Standard Event Schema

```json
{
  "eventId": "uuid",
  "eventType": "MaterialCreated",
  "eventVersion": "1.0",
  "occurredAt": "2026-08-05T12:00:00Z",
  "correlationId": "uuid",
  "causationId": "uuid",
  "source": "Inventory",
  "actor": {
    "userId": "USR-001"
  },
  "payload": {}
}
```

---

# Required Metadata

Event ID

Event Type

Version

Occurred At

Correlation ID

Causation ID

Source Module

User

Tenant (Future)

---

# Event Naming

Use

Past Tense

Examples

MaterialCreated

MaterialUpdated

PurchaseOrderApproved

InventoryAdjusted

ProductionStarted

ProductionCompleted

ShipmentCreated

MachineStopped

MachineRecovered

QualityInspectionPassed

MaintenanceCompleted

AIRecommendationGenerated

---

# Domain Events

Represent business facts.

Examples

CustomerCreated

SupplierApproved

MaterialArchived

PurchaseOrderReleased

InventoryTransferred

ProductionClosed

---

# Integration Events

Used for

ERP Integration

CRM Integration

Accounting

MES

IoT

External APIs

---

# Workflow Events

Examples

ApprovalSubmitted

ApprovalGranted

ApprovalRejected

WorkflowEscalated

WorkflowCompleted

Reference

Approval_Workflow.md

---

# AI Events

Examples

PromptSubmitted

PredictionCompleted

RecommendationAccepted

RecommendationRejected

KnowledgeIndexed

Reference

AI_Copilot.md

---

# Digital Twin Events

Examples

MachineStarted

MachineStopped

TemperatureExceeded

SensorOffline

ProductionRateChanged

Reference

Digital_Twin.md

---

# Security Events

Examples

LoginSucceeded

LoginFailed

PermissionGranted

PermissionRevoked

PasswordChanged

MFACompleted

---

# Notification Events

Examples

NotificationCreated

EmailSent

SMSDelivered

PushSent

ReminderTriggered

---

# Event Versioning

Events are immutable.

Breaking changes require

New Event Version.

Consumers must support version compatibility.

---

# Event Ordering

Ordering is guaranteed only within the same aggregate or entity.

Global ordering is not assumed.

Consumers must be resilient to out-of-order delivery.

---

# Event Delivery

Supports

At Least Once

Retry

Dead Letter Queue

Idempotent Consumers

Duplicate Detection

---

# Event Bus

Supports

RabbitMQ

Azure Service Bus

Kafka (Future)

Cloud Messaging

---

# Event Processing

Supports

Async Processing

Retry

Parallel Consumers

Batch Consumers

Delayed Delivery

---

# Event Retention

Supports

Archive

Replay

Retention Policies

Audit Integration

Reference

Audit_Log.md

---

# Event Replay

Supports

Recovery

System Rebuild

Analytics

AI Training

Debugging

---

# Event Correlation

Uses

Correlation ID

Causation ID

Trace ID

Reference

API_Standards.md

---

# Security

Events must

Respect permissions

Avoid sensitive data

Encrypt when required

Support auditing

---

# Monitoring

Track

Published Events

Failed Events

Retries

Processing Time

Queue Length

Consumer Health

Reference

Monitoring.md

---

# API Integration

Events may trigger

REST APIs

SignalR

Webhooks

Notifications

Background Jobs

---

# Error Handling

Supports

Retry

Dead Letter Queue

Poison Message Detection

Failure Logging

Reference

Error_Handling.md

---

# Performance

Supports

High Throughput

Horizontal Scaling

Message Compression

Batch Publishing

Async Dispatch

---

# Example Event

```json
{
  "eventType": "ProductionCompleted",
  "source": "Production",
  "occurredAt": "2026-08-05T14:20:00Z",
  "payload": {
    "productionOrder": "PRO-2026-001245",
    "quantity": 120
  }
}
```

---

# Best Practices

✓ Publish business facts only.

✓ Keep payloads small.

✓ Version every event.

✓ Use correlation IDs.

✓ Design consumers to be idempotent.

✓ Log every published event.

---

# Do

✓ Use past tense event names

✓ Publish immutable events

✓ Retry transient failures

✓ Monitor queues

✓ Archive important events

---

# Don't

✗ Publish commands as events

✗ Modify published events

✗ Include sensitive secrets

✗ Depend on delivery order globally

✗ Create duplicate event names

---

# Acceptance Criteria

Events follow the shared schema.

Event names are standardized.

Events are immutable.

Consumers support retries and idempotency.

Monitoring and auditing are enabled.

Versioning is maintained.

---

# Related Documents

Architecture.md

API_Standards.md

Audit_Log.md

Approval_Workflow.md

Error_Handling.md

Notifications.md

Digital_Twin.md

AI_Copilot.md

Monitoring.md

Security.md
