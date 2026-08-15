# ==============================================================================
# PRODUCTION API
# Naswood Operating System (NOS)
# Module: Production
# Document: Production API
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

This document defines the REST API specification for the Production module.

The Production API exposes manufacturing execution capabilities while preserving
the architectural principles defined by the NOS Constitution.

All APIs are versioned.

All endpoints are protected.

Business rules remain inside the Domain Layer.

---

# 2. API PRINCIPLES

Production APIs follow:

- RESTful Design
- CQRS
- Clean Architecture
- Versioned Contracts
- JWT Authentication
- Permission-Based Authorization
- Audit Logging
- OpenAPI Documentation

Base URL

```
/api/v1/production
```

---

# 3. RESOURCE MAP

```
Production Orders

Operations

Material Issues

Material Returns

Production Output

Labor Entries

Downtime

Scrap

Genealogy

Dashboards

Reports
```

---

# 4. PRODUCTION ORDERS

## List Production Orders

```
GET /api/v1/production/orders
```

Supports:

- Pagination
- Sorting
- Filtering
- Search

Filters

- Status
- Product
- Work Center
- Planner
- Priority
- Due Date

---

## Production Order Detail

```
GET /api/v1/production/orders/{id}
```

Returns

- Header
- Operations
- Materials
- Labor
- Scrap
- Downtime
- Output
- Genealogy
- Audit

---

## Create Production Order

```
POST /api/v1/production/orders
```

Request

```json
{
  "productRevisionId": "",
  "capabilityProfileId": "",
  "bomRevisionId": "",
  "routingRevisionId": "",
  "quantity": 100,
  "warehouseId": "",
  "plannedStart": "",
  "plannedFinish": ""
}
```

Response

```
201 Created
```

---

## Release Production Order

```
POST /api/v1/production/orders/{id}/release
```

Publishes

```
ProductionOrderReleased
```

---

## Cancel Production Order

```
POST /api/v1/production/orders/{id}/cancel
```

Approval may be required.

---

## Close Production Order

```
POST /api/v1/production/orders/{id}/close
```

Allowed only after completion.

---

# 5. OPERATIONS

## List Operations

```
GET /api/v1/production/operations
```

---

## Operation Detail

```
GET /api/v1/production/operations/{id}
```

---

## Start Operation

```
POST /api/v1/production/operations/{id}/start
```

---

## Pause Operation

```
POST /api/v1/production/operations/{id}/pause
```

---

## Resume Operation

```
POST /api/v1/production/operations/{id}/resume
```

---

## Complete Operation

```
POST /api/v1/production/operations/{id}/complete
```

Publishes

```
OperationCompleted
```

---

# 6. MATERIAL ISSUE

## Material Issue

```
POST /api/v1/production/material-issues
```

Request

```json
{
  "productionOrderId": "",
  "operationId": "",
  "warehouseId": "",
  "lines": [
    {
      "materialId": "",
      "lotId": "",
      "quantity": 25
    }
  ]
}
```

Creates

- Inventory Transaction
- Material Ledger Entry

Publishes

```
MaterialIssued
```

---

## Material Return

```
POST /api/v1/production/material-returns
```

Creates reverse inventory transaction.

---

# 7. PRODUCTION OUTPUT

## Production Receipt

```
POST /api/v1/production/output
```

Request

```json
{
  "productionOrderId": "",
  "operationId": "",
  "quantity": 100,
  "warehouseId": "",
  "lotNumber": ""
}
```

Creates

- Inventory Receipt
- Material Record
- Genealogy Link
- Cost Collection

Publishes

```
ProductionOutputPosted
```

---

# 8. SCRAP

## Record Scrap

```
POST /api/v1/production/scrap
```

Request

```json
{
  "productionOrderId": "",
  "operationId": "",
  "reasonId": "",
  "quantity": 5
}
```

Publishes

```
ScrapRecorded
```

---

# 9. DOWNTIME

## Record Downtime

```
POST /api/v1/production/downtime
```

Request

```json
{
  "machineId": "",
  "productionOrderId": "",
  "reasonId": "",
  "startedAt": "",
  "endedAt": ""
}
```

Publishes

```
DowntimeRecorded
```

---

# 10. LABOR

## Start Labor

```
POST /api/v1/production/labor/start
```

---

## Stop Labor

```
POST /api/v1/production/labor/stop
```

---

## Labor History

```
GET /api/v1/production/labor
```

Supports:

- Employee
- Shift
- Machine
- Production Order

---

# 11. GENEALOGY

## Forward Trace

```
GET /api/v1/production/genealogy/forward/{lotId}
```

Returns

```
Raw Material

↓

Operations

↓

Semi Finished

↓

Finished Goods

↓

Shipment
```

---

## Backward Trace

```
GET /api/v1/production/genealogy/backward/{lotId}
```

Returns

```
Shipment

↓

Finished Lot

↓

Production

↓

Raw Material

↓

Supplier
```

---

# 12. DASHBOARD

```
GET /api/v1/production/dashboard
```

Returns

- OEE
- Throughput
- Active Orders
- Downtime
- Scrap
- Yield
- Capacity
- Labor Utilization

---

# 13. REPORTS

```
GET /api/v1/production/reports
```

Available Reports

- Production Summary
- Machine Performance
- Labor Productivity
- Scrap Analysis
- Downtime Analysis
- Yield Analysis
- Genealogy Report
- Order History

---

# 14. EVENTS

Production publishes:

```
ProductionOrderReleased

MaterialIssued

MaterialReturned

OperationStarted

OperationPaused

OperationResumed

OperationCompleted

ProductionOutputPosted

ScrapRecorded

DowntimeRecorded

LaborStarted

LaborStopped

ProductionCompleted

ProductionOrderClosed

GenealogyCreated
```

---

# 15. AUTHORIZATION

Permissions

```
production.order.read

production.order.create

production.order.release

production.order.close

production.operation.execute

production.material.issue

production.material.return

production.output.post

production.scrap.record

production.downtime.record

production.labor.record

production.genealogy.read

production.dashboard.read

production.report.read
```

Every endpoint requires authentication.

---

# 16. RESPONSE STANDARD

Successful response

```json
{
  "success": true,
  "data": {},
  "metadata": {},
  "errors": null
}
```

Validation error

```json
{
  "success": false,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Quantity must be greater than zero."
    }
  ]
}
```

---

# 17. API VERSIONING

Current version

```
/api/v1/
```

Future breaking changes require

```
/api/v2/
```

Backward compatibility should be preserved whenever possible.

---

# 18. AUDIT

Every write operation generates an audit entry including:

- User
- Timestamp
- Module
- Entity
- Action
- Previous Value
- New Value
- Correlation ID

---

# 19. DESIGN RULES

Production APIs must never:

- Update Inventory directly
- Modify BOMs
- Modify Routings
- Change Product Revisions
- Bypass Workflow
- Ignore Authorization

All business changes occur through documented domain services.

---

# 20. FINAL API STATEMENT

The Production API provides a complete interface for manufacturing execution
while preserving architectural integrity, inventory consistency and full
traceability.

All manufacturing transactions are versioned, auditable and event-driven,
ensuring reliable integration with Inventory, Planning, Quality, Maintenance,
Logistics and Finance.
