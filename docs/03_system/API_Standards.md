# API Standards

**Project:** Naswood OS
**Document:** API Standards
**Version:** 1.0
**Status:** Approved

---

# Purpose

This document defines the API design standards for Naswood OS.

The objectives are:

- Consistency
- Scalability
- Security
- Maintainability
- AI Readiness
- Integration Readiness

All internal and external APIs shall comply with these standards.

---

# API Philosophy

Naswood OS follows an API-First architecture.

Business logic belongs to the Domain Layer.

APIs expose business capabilities.

User Interfaces consume the same APIs as external systems.

---

# Architectural Principles

- REST First
- Resource Oriented
- Stateless
- Versioned
- Secure by Design
- Event Driven
- Backward Compatible
- Documentation First

---

# Base URL

```
/api/v1/
```

Future versions

```
/api/v2/
```

Older versions remain supported according to the deprecation policy.

---

# Resource Naming

Use plural nouns.

Correct

```
/materials
```

```
/production-orders
```

```
/transformations
```

Incorrect

```
/getMaterial
```

```
/createMaterial
```

---

# HTTP Methods

GET

Retrieve data

POST

Create new resource

PUT

Replace resource

PATCH

Partial update

DELETE

Soft Delete only

---

# HTTP Status Codes

200 OK

201 Created

202 Accepted

204 No Content

400 Bad Request

401 Unauthorized

403 Forbidden

404 Not Found

409 Conflict

422 Validation Error

429 Too Many Requests

500 Internal Server Error

503 Service Unavailable

---

# Standard Response

Successful response

```json
{
  "success": true,
  "data": {},
  "meta": {},
  "errors": []
}
```

---

Error response

```json
{
  "success": false,
  "data": null,
  "errors": [
    {
      "code": "MAT-001",
      "message": "Material not found."
    }
  ]
}
```

---

# Pagination

Large collections shall be paginated.

Example

```
GET /materials?page=1&pageSize=50
```

Response

```json
{
  "data": [],
  "meta": {
    "page": 1,
    "pageSize": 50,
    "totalPages": 12,
    "totalItems": 589
  }
}
```

---

# Filtering

Example

```
GET /materials

?species=PINE

&status=AVAILABLE

&warehouse=RAW01
```

Multiple filters are supported.

---

# Sorting

Example

```
GET /materials

?sort=createdAt

&order=desc
```

---

# Searching

Example

```
GET /materials

?search=ASH
```

Search shall support:

- Business Code
- Material Code
- Product Code
- Barcode
- QR Code

---

# Versioning

APIs are versioned.

Example

```
/api/v1/materials
```

Breaking changes require a new version.

---

# Authentication

Authentication uses JWT.

Every request includes

```
Authorization

Bearer <token>
```

---

# Authorization

Role-Based Access Control (RBAC)

Permissions are validated before business logic executes.

Unauthorized requests return

```
403 Forbidden
```

---

# Validation

Validation occurs at multiple levels.

Client

↓

API

↓

Domain

↓

Database

Invalid requests never reach the database.

---

# Business Codes

Users search using

- Material Code
- Product Code
- Package Code

Internal UUIDs remain hidden.

---

# Date Format

ISO-8601

Example

```
2026-08-05T14:35:00Z
```

---

# Units

API never assumes measurement units.

Every numeric value requiring units shall include:

Example

```json
{
  "value": 24,
  "unit": "mm"
}
```

---

# Events

Important business actions generate Events.

Examples

MaterialReceived

TransformationCompleted

PackageCreated

ShipmentCompleted

MachineStopped

Events are immutable.

---

# Idempotency

POST endpoints supporting retries shall use

```
Idempotency-Key
```

This prevents duplicate transactions.

---

# File Upload

Supported types

- PDF
- JPG
- PNG
- DXF
- STEP
- XLSX

Maximum size is configurable.

Files are stored separately from transactional data.

---

# Rate Limiting

Default

100 requests

per minute

per user

Limits are configurable.

---

# Audit

Every API request records

- User
- Timestamp
- IP Address
- Endpoint
- Request ID
- Correlation ID

Critical operations generate Audit Logs.

---

# Performance

Recommended response time

GET

< 300 ms

POST

< 500 ms

Search

< 1 second

Long-running operations execute asynchronously.

---

# Documentation

Every endpoint must include:

- Purpose
- Request
- Response
- Error Codes
- Permissions
- Examples

OpenAPI documentation is mandatory.

---

# Security

All APIs require HTTPS.

Sensitive fields are encrypted.

Passwords are never returned.

SQL Injection protection is mandatory.

Input validation is mandatory.

---

# Integration

APIs support integration with

- ERP
- PLC
- SCADA
- MES
- WMS
- CRM
- Accounting
- AI Services
- Mobile Applications
- Customer Portal
- Dealer Portal

---

# AI Integration

AI services access APIs using the same authorization model.

AI may

- Read
- Analyze
- Recommend

AI may not execute critical business actions without user approval.

---

# API Design Rules

- Use nouns instead of verbs.
- Keep endpoints predictable.
- Never expose internal database structure.
- Separate business logic from controllers.
- Return meaningful error messages.
- Preserve backward compatibility.
- Use UUID internally.
- Use Business Codes for human interaction.

---

# Examples

## Materials

```
GET /api/v1/materials
```

```
POST /api/v1/materials
```

```
GET /api/v1/materials/{id}
```

---

## Production Orders

```
GET /api/v1/production-orders
```

```
POST /api/v1/production-orders
```

---

## Transformations

```
GET /api/v1/transformations
```

---

## Inventory

```
GET /api/v1/inventory
```

---

## Machines

```
GET /api/v1/machines
```

---

# Final Principle

APIs are part of the Manufacturing Operating System.

They expose business capabilities rather than database tables.

Every API should reflect real manufacturing processes while preserving security, traceability and consistency.
