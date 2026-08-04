# Database Schema — Events

**Project:** Naswood OS
**Document:** Events Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Events module records every business event occurring within Naswood OS.

Events represent immutable business facts.

They enable complete traceability, workflow automation, integrations and AI-driven analytics.

Unlike Audit Logs, Events describe what happened in the business rather than who performed the action.

---

# Philosophy

Business activities generate Events.

Events are immutable.

Events are never updated.

Events are never deleted.

Every Event represents a fact that occurred at a specific moment.

---

# Entity List

BusinessEvent

EventPayload

EventSubscription

EventQueue

EventProcessing

---

# business_event

Represents one immutable business event.

| Field | Type |
|--------|------|
| id | UUID |
| event_id | UUID |
| event_type | VARCHAR(100) |
| event_version | INTEGER |
| source_module | VARCHAR(50) |
| entity_type | VARCHAR(50) |
| entity_id | UUID |
| entity_code | VARCHAR(50) |
| correlation_id | UUID |
| causation_id | UUID |
| event_time | TIMESTAMP |
| published_by | VARCHAR(50) |

---

# event_payload

Stores the event data.

| Field | Type |
|--------|------|
| id | UUID |
| business_event_id | UUID FK |
| payload_json | JSONB |

---

# event_subscription

Defines consumers interested in specific Events.

| Field | Type |
|--------|------|
| id | UUID |
| subscriber_name | VARCHAR(100) |
| event_type | VARCHAR(100) |
| active | BOOLEAN |

Examples

- Inventory
- Production
- ERP
- CRM
- Analytics
- AI Copilot
- Notification Service

---

# event_queue

Tracks asynchronous processing.

| Field | Type |
|--------|------|
| id | UUID |
| business_event_id | UUID FK |
| subscriber_name | VARCHAR(100) |
| processing_status | VARCHAR(30) |
| retry_count | INTEGER |
| processed_at | TIMESTAMP |

Processing Status

- Pending
- Processing
- Completed
- Failed
- Dead Letter

---

# event_processing

Execution history.

| Field | Type |
|--------|------|
| id | UUID |
| event_queue_id | UUID FK |
| started_at | TIMESTAMP |
| completed_at | TIMESTAMP |
| execution_time_ms | INTEGER |
| error_message | TEXT |

---

# Event Categories

Receiving

Inventory

Production

Transformation

Quality

Packaging

Shipment

Sales

Purchasing

Maintenance

Machine

Tooling

Finance

Security

Integration

AI

---

# Standard Event Types

ReceivingCreated

ReceivingCompleted

MaterialRegistered

MaterialMoved

MaterialSplit

MaterialMerged

MaterialReserved

MaterialReleased

TransformationStarted

TransformationCompleted

RecipeExecuted

MachineStarted

MachineStopped

MachineAlarmRaised

InspectionCreated

InspectionPassed

InspectionFailed

QualityApproved

QualityRejected

PackageCreated

PackageLoaded

ShipmentCreated

ShipmentCompleted

SalesOrderConfirmed

ProductionOrderCreated

PurchaseOrderApproved

MaintenanceStarted

MaintenanceCompleted

InventoryAdjusted

UserCreated

PermissionChanged

AIRecommendationGenerated

AIRecommendationApproved

ERPExportCompleted

---

# Relationships

Business Event

1 → 1 Event Payload

Business Event

1 → N Event Queue

Event Queue

1 → N Event Processing Records

---

# Business Rules

### BR-1401

Every critical business operation shall generate a Business Event.

---

### BR-1402

Business Events are immutable.

---

### BR-1403

Business Events shall never be deleted.

---

### BR-1404

Every Event shall contain a Correlation ID.

---

### BR-1405

Related Events shall reference the originating Causation ID.

---

### BR-1406

Subscribers process Events asynchronously.

---

### BR-1407

Failed processing attempts shall be retried according to configurable policies.

---

### BR-1408

Events shall be version-controlled.

---

### BR-1409

Every Event shall reference exactly one business entity.

---

### BR-1410

Event processing shall never modify historical Events.

---

# Event Flow

Business Action

↓

Business Event

↓

Event Queue

↓

Subscribers

↓

Processing

↓

Completion

---

# Integration

Events integrate with:

- Inventory
- Production
- Quality
- Machines
- Tooling
- Maintenance
- Sales
- Purchasing
- Finance
- ERP
- CRM
- Analytics
- AI Platform
- Notification Service

---

# Future Extensions

The architecture supports:

- Kafka
- RabbitMQ
- Azure Service Bus
- AWS EventBridge
- MQTT
- OPC-UA Events
- PLC Events
- IoT Sensors
- Digital Twin Synchronization
- Event Replay

---

# Event Philosophy

Business Events describe what happened.

Events are immutable facts.

They provide the foundation for traceability, automation, analytics and Artificial Intelligence.

Every important action in Naswood OS becomes a permanent business event that can be replayed, analyzed and integrated with other systems.
