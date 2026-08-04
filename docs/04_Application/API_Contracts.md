# API Contracts

**Project:** Naswood OS

**Document:** API Contracts

**Version:** 2.0

**Status:** Approved

---

# 1. Purpose

This document defines all REST API contracts exposed by Naswood OS.

The API layer provides standardized access to all business capabilities across Production, Inventory, Warehouse, Logistics, Sales, AI, Analytics and Digital Twin services.

All APIs are versioned, documented and secured.

---

# 2. API Architecture

```
Client
      │
      ▼
API Gateway
      │
 ├── Authentication
 ├── Authorization
 ├── Rate Limiting
 ├── Validation
 ├── Logging
 ├── Monitoring
 └── Versioning
      │
      ▼
Business Services
      │
      ▼
Database
```

---

# 3. API Principles

RESTful

JSON

Stateless

Versioned

Secure

Documented

Idempotent where applicable

OpenAPI Compatible

---

# 4. Base URL

```
/api/v1
```

Future

```
/api/v2
```

---

# 5. Authentication

JWT

OAuth2

OpenID Connect

API Key

Bearer Token

Refresh Token

MFA Supported

---

# 6. Standard Headers

Authorization

Content-Type

Accept

X-Correlation-ID

X-Request-ID

X-Organization-ID

X-Factory-ID

X-Language

X-Timezone

---

# 7. Standard Response

```json
{
  "success": true,
  "message": "",
  "data": {},
  "errors": [],
  "metadata": {}
}
```

---

# 8. Standard Error

```json
{
 "success":false,
 "code":"VALIDATION_ERROR",
 "message":"Validation failed",
 "errors":[]
}
```

---

# 9. API Versioning

URI Versioning

```
/api/v1/
```

Breaking changes require new versions.

---

# 10. Pagination

page

pageSize

sort

order

filter

search

---

# 11. Master Data APIs

## Materials

GET /materials

POST /materials

PATCH /materials/{id}

DELETE /materials/{id}

GET /materials/search

GET /materials/{id}/genealogy

GET /materials/{id}/events

---

## Products

GET /products

POST /products

PATCH /products/{id}

---

## Customers

GET /customers

POST /customers

PATCH /customers/{id}

GET /customers/{id}/packaging-rules

GET /customers/{id}/quality-profile

GET /customers/{id}/certificates

---

## Suppliers

GET /suppliers

POST /suppliers

PATCH /suppliers/{id}

GET /suppliers/{id}/performance

GET /suppliers/{id}/certificates

---

## Warehouses

GET /warehouses

POST /warehouses

PATCH /warehouses/{id}

GET /warehouses/map

GET /warehouses/{id}/locations

GET /warehouses/{id}/capacity

---

# 12. Production APIs

## Production Orders

GET /production-orders

POST /production-orders

PATCH /production-orders/{id}

POST /production-orders/{id}/release

POST /production-orders/{id}/schedule

POST /production-orders/{id}/cancel

---

## Operations

GET /operations

POST /operations

PATCH /operations/{id}

POST /operations/{id}/start

POST /operations/{id}/pause

POST /operations/{id}/resume

POST /operations/{id}/complete

---

## Routing

GET /routing

POST /routing

PATCH /routing/{id}

---

## Recipes

GET /recipes

POST /recipes

PATCH /recipes/{id}

---

# 13. Inventory APIs

GET /inventory

GET /inventory/movements

GET /inventory/availability

POST /inventory/transfer

POST /inventory/reserve

POST /inventory/count

---

# 14. Warehouse APIs

GET /warehouse/map

GET /warehouse/utilization

GET /warehouse/heatmap

POST /warehouse/move

POST /warehouse/allocate

---

# 15. Packaging APIs

GET /packages

POST /packages

PATCH /packages/{id}

POST /packages/{id}/verify

POST /packages/{id}/close

GET /packages/{id}/labels

GET /packages/{id}/genealogy

---

# 16. Finished Goods APIs

GET /finished-goods

POST /finished-goods

GET /finished-goods/{id}

GET /finished-goods/{id}/dpp

GET /finished-goods/{id}/history

---

# 17. Logistics APIs

GET /shipments

POST /shipments

PATCH /shipments/{id}

POST /shipments/{id}/dispatch

POST /shipments/{id}/deliver

GET /containers

GET /routes

GET /carriers

---

# 18. Quality APIs

GET /quality/inspections

POST /quality/inspection

POST /quality/approve

POST /quality/reject

GET /quality/certificates

---

# 19. Maintenance APIs

GET /maintenance

POST /maintenance

POST /maintenance/workorder

POST /maintenance/complete

GET /machines/{id}/health

---

# 20. Barcode & Printing APIs

POST /barcode/generate

POST /qr/generate

POST /labels/print

POST /printing/reprint

GET /printing/jobs

---

# 21. Analytics APIs

GET /analytics/kpi

GET /analytics/dashboard

GET /analytics/trends

GET /analytics/oee

GET /analytics/forecast

---

# 22. AI APIs

POST /ai/chat

POST /ai/copilot

POST /ai/recommendation

POST /ai/predict

POST /ai/anomaly

POST /ai/root-cause

POST /ai/forecast

---

# 23. Digital Twin APIs

GET /digital-twin/factory

GET /digital-twin/material-flow

GET /digital-twin/machines

GET /digital-twin/wip

GET /digital-twin/warehouse

---

# 24. Event APIs

GET /events

GET /events/{id}

POST /events/replay

GET /events/subscriptions

---

# 25. Notification APIs

POST /notifications/email

POST /notifications/push

POST /notifications/sms

GET /notifications/history

---

# 26. Mobile APIs

GET /mobile/tasks

GET /mobile/worklist

POST /mobile/scan

POST /mobile/photo

POST /mobile/signature

---

# 27. Security APIs

POST /login

POST /logout

POST /refresh

GET /users/me

GET /permissions

GET /roles

---

# 28. Webhooks

ProductionCompleted

MaterialCreated

PackageCreated

ShipmentDispatched

ShipmentDelivered

MachineAlarm

InventoryLow

QualityRejected

MaintenanceCompleted

---

# 29. Rate Limits

Authentication

10 req/min

Read

1000 req/min

Write

300 req/min

Bulk Operations

50 req/min

AI APIs

60 req/min

---

# 30. Idempotency

The following operations require idempotency keys:

Production Order Creation

Inventory Transfer

Shipment Creation

Package Creation

Payment Operations

---

# 31. API Security

HTTPS Only

JWT Validation

Role Based Access

Organization Isolation

Factory Isolation

Request Logging

Audit Logging

API Throttling

IP Restrictions

Token Expiration

---

# 32. OpenAPI

Every endpoint shall be documented using OpenAPI 3.x.

Swagger UI shall be generated automatically.

---

# 33. Monitoring

Request Count

Average Response Time

Error Rate

Latency

Availability

API Usage

Authentication Failures

---

# 34. Related Documents

System Architecture

Database Schema

Security Model

Workflow

Events

Analytics

Barcode & QR

Printing Model

Digital Twin

AI

---

# 35. Future Extensions

GraphQL Gateway

gRPC Services

MQTT API

OPC-UA Gateway

WebSocket Streaming

Server Sent Events

Kafka Event API

FHIR (if healthcare integration required)

EDI Integration

GS1 Digital Link API

---

# 36. Module Philosophy

The API layer is the integration backbone of Naswood OS.

Every module exposes standardized, secure and versioned services, enabling seamless interaction between web applications, mobile devices, PLCs, AI services, Digital Twin, ERP systems and external partners.

The API Contracts ensure consistency, interoperability and scalability across the Manufacturing Operating System.
