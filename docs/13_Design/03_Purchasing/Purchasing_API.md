# Purchasing API

**Module:** Purchasing

**Category:** API

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchasing API provides standardized RESTful endpoints for all procurement operations within Naswood OS.

It enables secure integration between Purchasing, Inventory, Production, Finance, Quality, Analytics, Mobile applications and third-party ERP systems.

All APIs follow the shared platform API standards.

Reference

API_Standards.md

---

# Objectives

- Standard REST API
- Secure Integration
- Event-Driven Communication
- Mobile Support
- High Performance
- Versioned APIs
- Extensible Architecture

---

# API Architecture

```
Mobile

↓

REST API

↓

Purchasing Service

↓

Business Logic

↓

Repository

↓

Database

↓

Event Bus
```

---

# API Principles

The Purchasing API follows

- RESTful Design
- Stateless Communication
- JSON Payloads
- OAuth2 Authentication
- JWT Authorization
- Optimistic Concurrency
- Pagination
- Filtering
- Sorting
- Versioning

Reference

API_Standards.md

Security.md

Versioning.md

Pagination.md

Sorting.md

Search_Filtering.md

Concurrency.md

---

# Base URL

```
/api/v1/purchasing
```

Future versions

```
/api/v2/purchasing
```

Reference

Versioning.md

---

# Authentication

Supports

- OAuth2
- JWT
- API Key (System Integration)
- Service Accounts

Every request requires

```
Authorization: Bearer <token>
```

Reference

Security.md

---

# Standard Response

Success

```json
{
  "success": true,
  "data": {},
  "message": "Operation completed successfully."
}
```

---

Validation Error

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Supplier is inactive."
  }
}
```

Reference

Error_Handling.md

---

# Supplier API

## List Suppliers

```
GET /suppliers
```

Supports

- Pagination
- Filtering
- Sorting
- Search

---

## Get Supplier

```
GET /suppliers/{id}
```

---

## Create Supplier

```
POST /suppliers
```

---

## Update Supplier

```
PUT /suppliers/{id}
```

---

## Activate Supplier

```
POST /suppliers/{id}/activate
```

---

## Suspend Supplier

```
POST /suppliers/{id}/suspend
```

---

## Supplier Performance

```
GET /suppliers/{id}/performance
```

Reference

TASK-026_Supplier.md

---

# Purchase Request API

## List

```
GET /purchase-requests
```

---

## Details

```
GET /purchase-requests/{id}
```

---

## Create

```
POST /purchase-requests
```

---

## Update

```
PUT /purchase-requests/{id}
```

---

## Submit

```
POST /purchase-requests/{id}/submit
```

---

## Approve

```
POST /purchase-requests/{id}/approve
```

---

## Reject

```
POST /purchase-requests/{id}/reject
```

Reference

TASK-027_Purchase_Request.md

---

# RFQ API

```
GET /rfqs

GET /rfqs/{id}

POST /rfqs

PUT /rfqs/{id}

POST /rfqs/{id}/publish

POST /rfqs/{id}/award

POST /rfqs/{id}/cancel

GET /rfqs/{id}/responses
```

Reference

TASK-028_RFQ.md

---

# Supplier Quotation API

```
GET /supplier-quotations

GET /supplier-quotations/{id}

POST /supplier-quotations

PUT /supplier-quotations/{id}

POST /supplier-quotations/{id}/submit

POST /supplier-quotations/{id}/evaluate

POST /supplier-quotations/{id}/award
```

Reference

TASK-029_Supplier_Quotation.md

---

# Purchase Order API

```
GET /purchase-orders

GET /purchase-orders/{id}

POST /purchase-orders

PUT /purchase-orders/{id}

POST /purchase-orders/{id}/submit

POST /purchase-orders/{id}/approve

POST /purchase-orders/{id}/release

POST /purchase-orders/{id}/cancel

POST /purchase-orders/{id}/close
```

Reference

TASK-030_Purchase_Order.md

---

# Purchase Goods Receipt API

```
GET /purchase-goods-receipts

GET /purchase-goods-receipts/{id}

POST /purchase-goods-receipts

POST /purchase-goods-receipts/{id}/post

POST /purchase-goods-receipts/{id}/reverse
```

Reference

TASK-031_Goods_Receipt_PO.md

---

# Purchase Return API

```
GET /purchase-returns

GET /purchase-returns/{id}

POST /purchase-returns

PUT /purchase-returns/{id}

POST /purchase-returns/{id}/submit

POST /purchase-returns/{id}/approve

POST /purchase-returns/{id}/close
```

Reference

TASK-032_Purchase_Return.md

---

# Supplier Invoice API

```
GET /supplier-invoices

GET /supplier-invoices/{id}

POST /supplier-invoices

PUT /supplier-invoices/{id}

POST /supplier-invoices/{id}/validate

POST /supplier-invoices/{id}/approve

POST /supplier-invoices/{id}/post
```

Reference

TASK-033_Supplier_Invoice.md

---

# Dashboard API

```
GET /dashboard/purchasing

GET /dashboard/purchasing/kpis

GET /dashboard/purchasing/spend

GET /dashboard/purchasing/suppliers

GET /dashboard/purchasing/approvals
```

Reference

TASK-034_Purchasing_Dashboard.md

---

# Reports API

```
GET /reports/purchasing

GET /reports/purchase-orders

GET /reports/suppliers

GET /reports/rfqs

GET /reports/invoices

GET /reports/spend
```

Reference

TASK-035_Purchasing_Reports.md

---

# Query Parameters

Filtering

```
?status=Approved

?supplierId=SUP001

?company=NASWOOD

?plant=BUCAK
```

Sorting

```
?sort=createdDate

?direction=desc
```

Pagination

```
?page=1

&pageSize=50
```

Reference

Search_Filtering.md

Sorting.md

Pagination.md

---

# Bulk Operations

Supports

```
POST /purchase-orders/bulk-release

POST /purchase-orders/bulk-close

POST /purchase-requests/bulk-approve

POST /supplier-invoices/bulk-validate
```

---

# File Upload

Supports

```
POST /attachments
```

Allowed

- PDF
- DOCX
- XLSX
- PNG
- JPG
- DWG
- IFC

Reference

File_Storage.md

---

# Mobile API

Optimized endpoints

```
GET /mobile/tasks

GET /mobile/dashboard

GET /mobile/notifications

POST /mobile/barcode
```

Reference

Purchasing_Mobile.md

---

# Event Publishing

Successful transactions publish events.

Examples

- SupplierCreated
- PurchaseRequestApproved
- RFQPublished
- SupplierQuotationSubmitted
- PurchaseOrderReleased
- GoodsReceiptPosted
- PurchaseReturnCreated
- SupplierInvoiceApproved

Reference

Event_Model.md

Integration_Events.md

---

# Rate Limiting

Default limits

| API | Limit |
|------|-------|
| Read | 1000 req/min |
| Write | 200 req/min |
| Upload | 50 req/min |

Limits are configurable.

Reference

Performance.md

---

# Security

Every endpoint enforces

- Authentication
- Authorization
- Company Isolation
- Plant Isolation
- Input Validation
- Audit Logging

Reference

Security.md

Permission_Model.md

Validation_Rules.md

---

# Performance

The Purchasing API shall

- Respond to standard GET requests in under 500 ms.
- Complete POST/PUT requests in under 2 seconds.
- Support asynchronous processing for long-running operations.
- Cache master data lookups.
- Support horizontal scaling.

Reference

Performance.md

Caching.md

---

# Audit

Every API request logs

- User
- Endpoint
- HTTP Method
- Timestamp
- Request ID
- Response Code
- Processing Time

Reference

Audit_Log.md

Logging.md

---

# API Versioning

Supports

```
v1

v2

v3
```

Older versions remain supported according to platform policy.

Reference

Versioning.md

---

# Future Extensions

Planned APIs

- Supplier Portal API
- EDI API
- OCR Invoice API
- AI Procurement API
- Procurement Analytics API
- Contract Management API

---

# Acceptance Criteria

The Purchasing API shall

- Provide complete REST coverage for all procurement processes.
- Follow shared API standards.
- Support filtering, sorting and pagination.
- Support secure authentication and authorization.
- Publish integration events.
- Support mobile applications.
- Maintain backward compatibility through versioning.
- Follow all shared platform standards.

---

# Related Documents

Purchasing_Architecture.md

Purchasing_Mobile.md

TASK-026_Supplier.md

TASK-027_Purchase_Request.md

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-032_Purchase_Return.md

TASK-033_Supplier_Invoice.md

TASK-034_Purchasing_Dashboard.md

TASK-035_Purchasing_Reports.md

API_Standards.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Pagination.md

Sorting.md

Search_Filtering.md

Versioning.md

Concurrency.md

File_Storage.md

Audit_Log.md

Logging.md

Event_Model.md

Integration_Events.md
