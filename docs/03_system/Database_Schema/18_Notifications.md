# Database Schema — Notifications

**Project:** Naswood OS
**Document:** Notifications Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Notifications module delivers real-time information to users, departments and integrated systems based on Business Events and Workflow actions.

Notifications improve operational awareness without changing business data.

Every notification is traceable and configurable.

---

# Philosophy

Business Events trigger Notifications.

Workflow determines recipients.

Notifications inform users.

Notifications never execute business logic.

---

# Entity List

Notification

NotificationRecipient

NotificationTemplate

NotificationChannel

NotificationPreference

NotificationRule

NotificationDelivery

NotificationGroup

---

# notification

Represents a generated notification.

| Field | Type |
|--------|------|
| id | UUID |
| notification_type | VARCHAR(50) |
| title | VARCHAR(200) |
| message | TEXT |
| source_module | VARCHAR(50) |
| source_entity | VARCHAR(50) |
| source_entity_id | UUID |
| priority | VARCHAR(20) |
| status | VARCHAR(30) |
| created_at | TIMESTAMP |

Priority

- Low
- Normal
- High
- Critical

Status

- Pending
- Sent
- Delivered
- Read
- Failed
- Cancelled

---

# notification_recipient

Recipients of a notification.

| Field | Type |
|--------|------|
| id | UUID |
| notification_id | UUID FK |
| user_id | UUID FK |
| delivered_at | TIMESTAMP |
| read_at | TIMESTAMP |

---

# notification_template

Reusable templates.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(50) |
| name | VARCHAR(150) |
| channel | VARCHAR(30) |
| subject | VARCHAR(200) |
| body | TEXT |
| active | BOOLEAN |

---

# notification_channel

Delivery channels.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |
| active | BOOLEAN |

Supported Channels

- In-App
- Email
- SMS
- Microsoft Teams
- WhatsApp
- Push Notification
- Webhook

---

# notification_preference

User notification preferences.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| notification_type | VARCHAR(50) |
| channel_id | UUID FK |
| enabled | BOOLEAN |

---

# notification_rule

Rules determining recipients and channels.

| Field | Type |
|--------|------|
| id | UUID |
| event_type | VARCHAR(100) |
| workflow_id | UUID FK |
| role_id | UUID FK |
| priority | VARCHAR(20) |
| channel_id | UUID FK |
| active | BOOLEAN |

---

# notification_delivery

Delivery history.

| Field | Type |
|--------|------|
| id | UUID |
| notification_id | UUID FK |
| channel_id | UUID FK |
| delivery_status | VARCHAR(30) |
| delivered_at | TIMESTAMP |
| error_message | TEXT |

Delivery Status

- Queued
- Sent
- Delivered
- Failed
- Expired

---

# notification_group

Logical recipient groups.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |
| description | TEXT |

Examples

- Production Managers
- Warehouse Team
- Maintenance Team
- Quality Team
- Sales Team
- Executive Management
- AI Supervisors

---

# Relationships

Notification

1 → N Notification Recipients

Notification

1 → N Notification Deliveries

Notification Channel

1 → N Templates

Notification Channel

1 → N Preferences

Notification Rule

1 → N Notifications

Notification Group

1 → N Users

---

# Notification Categories

Production

Inventory

Quality

Machine

Maintenance

Tooling

Sales

Purchasing

Finance

Logistics

Security

Workflow

AI

System

---

# Standard Notification Types

Production Started

Production Completed

Production Delayed

Machine Alarm

Machine Stopped

Maintenance Due

Maintenance Completed

Quality Hold

Quality Approved

Quality Rejected

Material Received

Inventory Low

Inventory Reserved

Package Ready

Shipment Ready

Shipment Dispatched

Purchase Order Approved

Sales Order Confirmed

Workflow Approval Required

Workflow Completed

AI Recommendation Available

System Error

Security Alert

---

# Business Rules

### BR-1801

Notifications shall be generated from Business Events or Workflow Actions.

---

### BR-1802

Notifications never modify business data.

---

### BR-1803

Notification delivery shall be asynchronous.

---

### BR-1804

Users may configure notification preferences.

---

### BR-1805

Critical notifications shall support multiple delivery channels.

---

### BR-1806

Delivery failures shall be retried automatically.

---

### BR-1807

Notification history shall never be deleted.

---

### BR-1808

Read status is tracked per recipient.

---

### BR-1809

Notification templates are version-controlled.

---

### BR-1810

Every critical notification shall reference its originating Business Event.

---

# Integration

Notifications integrate with:

- Workflow
- Events
- Audit Log
- Security
- Production
- Inventory
- Quality
- Machines
- Tooling
- Maintenance
- Sales
- Purchasing
- Finance
- Logistics
- AI

---

# Future Extensions

The architecture supports:

- Mobile Push Notifications
- Microsoft Teams Integration
- Slack Integration
- WhatsApp Business API
- SMS Gateway
- Email Providers
- Voice Calls
- Digital Signage
- Smart Watch Alerts
- IoT Alarm Panels

---

# Notification Escalation

Notifications may be escalated automatically.

Example

Operator

↓

Supervisor

↓

Department Manager

↓

Factory Manager

↓

Executive Management

Escalation conditions include:

- No acknowledgement
- SLA exceeded
- Critical priority
- Safety-related event

---

# Notification Philosophy

Notifications are the communication layer of Naswood OS.

They deliver the right information to the right people through the right channel at the right time.

Every notification is triggered by real manufacturing events and remains fully traceable throughout its lifecycle.
