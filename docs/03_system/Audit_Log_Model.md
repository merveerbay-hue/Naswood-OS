# Audit Log Model

**Project:** Naswood OS
**Document:** Audit Log Model
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Audit Log Model records security-sensitive and business-critical user actions performed within Naswood OS.

Audit Logs provide accountability, regulatory compliance and operational transparency.

Unlike Business Events, Audit Logs focus on **who performed an action**, **when**, **where** and **under which authorization**.

Audit Logs are immutable.

---

# Philosophy

Every critical user action must be traceable.

Audit Logs are not business transactions.

Audit Logs are not production events.

Audit Logs record user accountability.

---

# Entity List

AuditLog

AuditFieldChange

AuditSession

AuditAttachment

---

# audit_log

Represents one auditable action.

| Field | Type |
|--------|------|
| id | UUID |
| correlation_id | UUID |
| user_id | UUID FK |
| employee_id | UUID FK |
| session_id | UUID FK |
| action | VARCHAR(100) |
| entity_type | VARCHAR(100) |
| entity_id | UUID |
| entity_code | VARCHAR(50) |
| module | VARCHAR(50) |
| severity | VARCHAR(20) |
| result | VARCHAR(20) |
| ip_address | VARCHAR(50) |
| device_name | VARCHAR(100) |
| browser | VARCHAR(100) |
| operating_system | VARCHAR(100) |
| request_method | VARCHAR(10) |
| endpoint | VARCHAR(255) |
| reason | TEXT |
| created_at | TIMESTAMP |

---

# audit_field_change

Stores field-level modifications.

| Field | Type |
|--------|------|
| id | UUID |
| audit_log_id | UUID FK |
| field_name | VARCHAR(100) |
| old_value | TEXT |
| new_value | TEXT |

---

# audit_session

Tracks authenticated user sessions.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| login_time | TIMESTAMP |
| logout_time | TIMESTAMP |
| ip_address | VARCHAR(50) |
| device_name | VARCHAR(100) |
| browser | VARCHAR(100) |
| operating_system | VARCHAR(100) |
| authentication_method | VARCHAR(50) |

---

# audit_attachment

Optional supporting evidence.

Examples

- Approval Document
- Signed PDF
- Image
- External Reference

| Field | Type |
|--------|------|
| id | UUID |
| audit_log_id | UUID FK |
| file_name | VARCHAR(255) |
| file_type | VARCHAR(50) |
| storage_url | TEXT |

---

# Audit Categories

Authentication

Authorization

Configuration

Master Data

Production

Inventory

Quality

Maintenance

Sales

Purchasing

Finance

Administration

AI

Integration

---

# Severity Levels

Information

Warning

Critical

Security

Compliance

---

# Result Values

Success

Failed

Denied

Cancelled

Automatic

---

# Auditable Actions

Examples

User Login

User Logout

Password Change

Permission Change

Role Assignment

Recipe Approval

Production Approval

Inventory Adjustment

Material Merge

Material Split

Quality Approval

Shipment Confirmation

Price Change

Configuration Update

Machine Parameter Change

AI Recommendation Approval

AI Recommendation Rejection

---

# Field-Level Auditing

The following entities should support field-level tracking.

Material

Recipe

Machine

Product

Customer

Supplier

Permission

User

Configuration

Master Data

---

# Retention Policy

Audit Logs are never physically deleted.

Retention periods are configurable.

Recommended minimum retention:

10 years

---

# Search Capabilities

Audit Logs shall support searching by:

- User
- Employee
- Module
- Entity
- Business Code
- Correlation ID
- Action
- Date Range
- Severity
- Result
- IP Address

---

# Relationships

User

1 → N Audit Logs

Audit Session

1 → N Audit Logs

Audit Log

1 → N Field Changes

Audit Log

1 → N Attachments

---

# Business Rules

- Every critical action shall generate an Audit Log.
- Audit Logs are immutable.
- Audit Logs cannot be modified.
- Audit Logs cannot be deleted.
- Audit Logs are generated automatically.
- Every Audit Log shall reference the responsible user.
- Audit Logs shall store the Correlation ID of the originating request.
- Audit Logs shall support compliance reporting.
- Sensitive information (passwords, tokens, secrets) shall never be stored in Audit Logs.
- Audit Logs shall be readable only by authorized roles.

---

# Integration

Audit Logs integrate with:

- Authentication
- Authorization
- Production
- Inventory
- Quality
- Maintenance
- Sales
- Purchasing
- Finance
- AI Services
- API Gateway

---

# Audit vs Event

| Audit Log | Business Event |
|------------|----------------|
| Records user accountability | Records business activity |
| User focused | Process focused |
| Security and compliance | Manufacturing and operations |
| Immutable | Immutable |
| References User | References Business Entity |
| Used for investigations | Used for process traceability |

---

# Compliance

The Audit Log Model is designed to support:

- ISO 9001
- ISO 27001
- FSC Chain of Custody
- PEFC Chain of Custody
- Digital Product Passport (DPP)
- Internal Audit Requirements

---

# Final Principle

If an action can change business data, system configuration or operational decisions, it shall be auditable.

Every audit record must answer:

- Who performed the action?
- What changed?
- When did it happen?
- Where did it originate?
- Why was it performed?
- Was it successful?
