# API Contracts

**Project:** Naswood OS
**Document:** API Contracts
**Version:** 1.0
**Status:** Approved

---

# Purpose

This document defines the application-level API contracts for Naswood OS.

API Contracts specify how applications communicate with the system while remaining independent from internal implementation details.

The document complements the API Standards by defining request and response behaviors, resource contracts and integration principles.

---

# API Philosophy

Naswood OS follows an API-First architecture.

All clients use the same APIs.

Examples

- Web Application
- Mobile Application
- AI Copilot
- Dealer Portal
- Customer Portal
- ERP Integration
- PLC Gateway

Business logic never exists inside the client.

---

# API Categories

Master Data

Production

Inventory

Quality

Machines

Tooling

Maintenance

Sales

Purchasing

Finance

Logistics

Workflow

Notifications

Analytics

AI

Administration

---

# Resource Naming

Resources use plural nouns.

Examples

```
/materials
```

```
/production-orders
```

```
/inventory
```

```
/shipments
```

```
/machines
```

---

# Standard Operations

Every resource should support when applicable:

GET

Retrieve

POST

Create

PUT

Replace

PATCH

Partial Update

DELETE

Logical Delete

SEARCH

Advanced Filtering

EXPORT

Report Export

---

# Standard Request

Example

```http
POST /api/v1/materials
```

```json
{
  "materialType": "THERMOWOOD",
  "species": "PINE",
  "thickness": 26,
  "width": 140,
  "length": 3600
}
```

---

# Standard Response

```json
{
  "success": true,
  "data": {},
  "meta": {},
  "errors": []
}
```

---

# Error Response

```json
{
  "success": false,
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

Large collections shall support pagination.

Example

```
GET /materials?page=1&pageSize=100
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

---

# Sorting

```
GET /materials

?sort=createdAt

&order=desc
```

---

# Searching

Search shall support:

- Business Code
- Material Code
- Product Code
- Package Code
- Lot Number
- QR Code
- Barcode

---

# Authentication

Authentication uses JWT.

```
Authorization

Bearer <token>
```

API Keys are supported for system integrations.

---

# Authorization

Authorization follows RBAC.

Permissions are validated before execution.

Unauthorized requests return:

```
403 Forbidden
```

---

# Idempotency

Critical POST operations support

```
Idempotency-Key
```

Examples

- Material Registration
- Package Creation
- Shipment Creation
- Production Order Creation

---

# Validation

Validation occurs in multiple layers.

Client

↓

API

↓

Domain

↓

Database

Invalid requests never reach persistence.

---

# Standard Headers

```
Authorization
```

```
Content-Type
```

```
Accept
```

```
X-Correlation-ID
```

```
Idempotency-Key
```

---

# Correlation ID

Every request shall contain a Correlation ID.

The same ID shall propagate through:

- Events
- Audit Logs
- Workflow
- Notifications

---

# File Upload

Supported formats

PDF

JPG

PNG

DXF

DWG

STEP

IFC

XLSX

CSV

Maximum size is configurable.

---

# Event Behavior

Successful business operations publish Business Events.

Examples

MaterialRegistered

TransformationCompleted

InspectionPassed

PackageCreated

ShipmentCompleted

Events are asynchronous.

---

# Audit Behavior

Critical API operations generate Audit Logs.

Examples

Inventory Adjustment

Permission Change

Recipe Update

Price Update

User Management

---

# Async Operations

Long-running processes return

```
202 Accepted
```

Processing continues asynchronously.

Examples

Large Imports

Mass Label Printing

AI Analysis

Bulk Inventory Registration

---

# API Versioning

Current

```
/api/v1/
```

Breaking changes require:

```
/api/v2/
```

---

# Rate Limiting

Default

100 requests

per minute

per authenticated client.

Configurable by API Client.

---

# Standard Modules

The following resources expose APIs.

Materials

Products

Production Orders

Transformations

Inventory

Warehouse

Receiving

Machines

Tooling

Quality

Maintenance

Packaging

Shipments

Sales Orders

Purchase Orders

Customers

Suppliers

Workflow

Notifications

Analytics

AI

---

# Integration Contracts

APIs support integration with:

ERP

PLC

SCADA

MES

WMS

CRM

Power BI

AI Services

Digital Twin

IoT Devices

---

# Error Codes

Business Validation

```
MAT-xxx
```

Production

```
PRD-xxx
```

Inventory

```
INV-xxx
```

Quality

```
QLT-xxx
```

Machine

```
MAC-xxx
```

Maintenance

```
MNT-xxx
```

Sales

```
SAL-xxx
```

Purchasing

```
PUR-xxx
```

Finance

```
FIN-xxx
```

Security

```
SEC-xxx
```

Workflow

```
WF-xxx
```

AI

```
AI-xxx
```

---

# OpenAPI

All APIs shall be documented using OpenAPI 3.x.

Swagger documentation shall be generated automatically.

---

# Business Rules

### API-001

Every request shall be authenticated unless explicitly marked as public.

---

### API-002

Every response shall use the standard response model.

---

### API-003

Business logic shall never exist in Controllers.

---

### API-004

Controllers shall delegate processing to the Application Layer.

---

### API-005

Every successful business transaction publishes a Business Event.

---

### API-006

Critical operations generate Audit Logs.

---

### API-007

Every API shall support traceability using Correlation IDs.

---

### API-008

API Contracts shall remain backward compatible within the same major version.

---

### API-009

Internal database identifiers shall never be exposed to external systems unless explicitly required.

Business Codes should be preferred whenever possible.

---

### API-010

All APIs shall be self-documented and testable through OpenAPI.

---

# Future Extensions

The architecture supports:

- GraphQL
- gRPC
- WebSockets
- MQTT
- OPC-UA
- Event Streaming
- Kafka
- AI Function Calling
- MCP (Model Context Protocol)

---

# API Contract Philosophy

API Contracts define the public behavior of Naswood OS.

Applications interact through stable, versioned and secure contracts rather than internal database structures.

This ensures long-term maintainability, interoperability and compatibility across web, mobile, AI and enterprise integrations.
