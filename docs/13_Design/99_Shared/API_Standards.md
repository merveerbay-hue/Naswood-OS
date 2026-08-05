# API Standards

**Module:** Shared

**Category:** API Standards

**Version:** 1.0

**Status:** Approved

---

# Purpose

The API Standards document defines the architectural principles, naming conventions, request and response formats, authentication rules and integration guidelines used throughout Naswood OS.

All backend services, mobile applications, AI services and third-party integrations must comply with this specification.

This document serves as the single source of truth for API design.

---

# Objectives

- Consistent API Design
- Predictable Developer Experience
- Secure Communication
- Scalable Architecture
- Easy Integration
- Standardized Error Handling

---

# API Architecture

Naswood OS follows

REST API

Event API

SignalR (Real-Time)

Background Jobs

Future GraphQL (Optional)

---

# Design Principles

APIs should be

Consistent

Versioned

Stateless

Secure

Documented

Backward Compatible

---

# Base URL

```
/api/v1
```

Future versions

```
/api/v2
```

---

# Resource Naming

Use plural nouns.

Examples

```
/materials

/orders

/customers

/suppliers

/warehouses

/users

/roles
```

Avoid verbs in URLs.

---

# HTTP Methods

GET

Retrieve

POST

Create

PUT

Replace

PATCH

Partial Update

DELETE

Soft Delete

---

# Standard Response

```json
{
  "success": true,
  "message": "Material created successfully.",
  "data": {},
  "errors": [],
  "metadata": {}
}
```

---

# Error Response

```json
{
  "success": false,
  "message": "Validation failed.",
  "errors": [
    {
      "field": "materialCode",
      "message": "Material Code already exists."
    }
  ]
}
```

---

# HTTP Status Codes

200 OK

201 Created

204 No Content

400 Bad Request

401 Unauthorized

403 Forbidden

404 Not Found

409 Conflict

422 Validation Error

500 Internal Server Error

---

# Pagination

Supports

Page

Page Size

Sorting

Filtering

Search

Example

```
GET /api/v1/materials?page=1&pageSize=50
```

---

# Sorting

Example

```
?sort=name

?sort=-createdDate
```

---

# Filtering

Example

```
?status=active

?warehouse=FG01

?category=raw-material
```

---

# Search

Example

```
?q=thermowood
```

Supports

Full Text Search

Barcode

QR

Material Code

AI Search (Future)

---

# Authentication

OAuth2

JWT

Refresh Token

API Key (Integrations)

---

# Authorization

Role Based Access Control (RBAC)

Department Permissions

Module Permissions

Record Permissions

Field Permissions

---

# Request Headers

Authorization

Accept

Content-Type

Accept-Language

Correlation-Id

Tenant-Id (Future)

---

# Validation

Server-side validation is mandatory.

Client-side validation improves UX only.

---

# File Upload

Supports

Multipart Form Data

Images

PDF

Excel

ZIP

CAD Files

Maximum size configurable.

---

# Date Format

ISO 8601

Example

```
2026-08-05T14:30:00Z
```

---

# Number Format

Decimal separator

.

Currency

ISO 4217

Units

SI preferred

---

# Idempotency

POST endpoints may support

Idempotency-Key

for safe retries.

---

# Concurrency

Supports

Optimistic Concurrency

RowVersion

ETag (Optional)

---

# Soft Delete

Resources are archived instead of permanently removed.

Example

```
isDeleted = true
```

---

# Audit

Track

Created By

Created At

Updated By

Updated At

Deleted By

Deleted At

---

# Versioning

URI Versioning

```
/api/v1
```

Breaking changes require a new major version.

---

# Documentation

All APIs must be documented using

OpenAPI 3.1

Swagger UI

Examples are required.

---

# Security

HTTPS Only

JWT Validation

Rate Limiting

Input Validation

Output Encoding

Audit Logging

---

# Performance

Compression

Caching

Pagination

Async Processing

Streaming (when required)

---

# Rate Limiting

Example

```
100 requests/minute
```

Configurable by endpoint.

---

# Real-Time

SignalR

Examples

Notifications

Production Status

Machine Monitoring

OEE Dashboard

AI Events

---

# Event Naming

Past tense.

Examples

MaterialCreated

PurchaseOrderApproved

InventoryAdjusted

ProductionCompleted

MachineStopped

QualityInspectionPassed

---

# API Naming Convention

Controllers

```
MaterialsController
```

Endpoints

```
GET /materials

POST /materials

PATCH /materials/{id}

DELETE /materials/{id}
```

---

# Error Codes

Use business error codes.

Examples

```
MAT001

PUR014

INV022

PRO009

AUTH003
```

---

# Logging

Log

Request

Response

Duration

User

Correlation ID

Exception

---

# Monitoring

Supports

Health Checks

Metrics

Tracing

Structured Logging

---

# Integration

Supports

ERP

CRM

MES

WMS

Accounting

IoT

AI Services

Digital Twin

Third-Party APIs

---

# AI APIs

Supports

Prompt Requests

Document Analysis

Prediction

Embeddings

Knowledge Search

Recommendation Engine

---

# Best Practices

✓ Use nouns.

✓ Version APIs.

✓ Validate requests.

✓ Return consistent responses.

✓ Support pagination.

✓ Log requests.

---

# Do

✓ Use HTTPS

✓ Return proper status codes

✓ Validate input

✓ Document endpoints

✓ Keep responses consistent

---

# Don't

✗ Return HTML

✗ Break contracts

✗ Expose stack traces

✗ Ignore authentication

✗ Hardcode business logic in controllers

---

# Acceptance Criteria

API follows the official standard.

Responses use the standard envelope.

Authentication and authorization are enforced.

OpenAPI documentation is complete.

Pagination and filtering are supported.

Audit logging is enabled.

Performance meets platform requirements.

---

# Related Documents

Authentication.md

Authorization.md

Error_Handling.md

Logging.md

Security.md

AI_Integration.md

Digital_Twin.md

Database_Standards.md
