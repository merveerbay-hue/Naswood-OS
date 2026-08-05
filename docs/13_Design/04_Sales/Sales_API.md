# Sales API

**Module:** Sales

**Version:** 1.0

**Status:** Approved

**Owner:** Naswood ERP Architecture Team

---

# Purpose

The Sales API provides standardized RESTful services for all Sales module operations within Naswood ERP.

It enables secure communication between

- Web Application
- Mobile Application
- Customer Portal
- Dealer Portal
- ERP Modules
- MES
- WMS
- Finance
- Third-party Systems

The API follows an API-First architecture and is designed for scalability, security and interoperability.

---

# API Principles

The Sales API follows

- REST Architecture
- JSON Format
- HTTPS Only
- JWT Authentication
- OpenAPI 3.1 Compatible
- Stateless Requests
- Versioned Endpoints
- Event Driven Integration
- Multi Company Support
- Multi Plant Support

---

# Base URL

```
https://api.naswood.com/api/v1
```

Future versions

```
/api/v2
/api/v3
```

---

# Authentication

Supports

- JWT Token
- OAuth2
- OpenID Connect
- Microsoft Entra ID
- API Key (System Integrations)

Example

```
Authorization

Bearer <JWT_TOKEN>
```

---

# Standard Headers

```
Authorization

Content-Type

Accept

Company-Id

Plant-Id

Warehouse-Id

Accept-Language

X-Correlation-Id
```

---

# Standard Response Format

Successful response

```json
{
  "success": true,
  "data": {},
  "message": "",
  "timestamp": "2026-08-01T10:00:00Z"
}
```

Validation error

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Customer Code is required."
  }
}
```

---

# Pagination

Request

```
?page=1

&pageSize=50
```

Response

```json
{
  "page": 1,
  "pageSize": 50,
  "totalPages": 24,
  "totalRecords": 1192
}
```

---

# Filtering

Supports

```
?status=Active

?customer=NAS001

?salesperson=15

?dateFrom=2026-01-01

?dateTo=2026-01-31

?company=1

?plant=2
```

---

# Sorting

Example

```
?sort=createdDate

?direction=desc
```

Supports multiple fields

```
?sort=customerName,revenue
```

---

# Customer API

```
GET    /customers

GET    /customers/{id}

POST   /customers

PUT    /customers/{id}

DELETE /customers/{id}

GET    /customers/search

POST   /customers/import

GET    /customers/export
```

Reference

TASK-036_Customer.md

---

# Lead API

```
GET    /leads

GET    /leads/{id}

POST   /leads

PUT    /leads/{id}

DELETE /leads/{id}

POST   /leads/{id}/assign

POST   /leads/{id}/convert

GET    /leads/search
```

Reference

TASK-037_Lead.md

---

# Opportunity API

```
GET    /opportunities

GET    /opportunities/{id}

POST   /opportunities

PUT    /opportunities/{id}

DELETE /opportunities/{id}

POST   /opportunities/{id}/close

POST   /opportunities/{id}/win

POST   /opportunities/{id}/lose
```

Reference

TASK-038_Opportunity.md

---

# Quotation API

```
GET    /quotations

GET    /quotations/{id}

POST   /quotations

PUT    /quotations/{id}

DELETE /quotations/{id}

POST   /quotations/{id}/approve

POST   /quotations/{id}/reject

POST   /quotations/{id}/send

POST   /quotations/{id}/duplicate

GET    /quotations/{id}/pdf
```

Reference

TASK-039_Quotation.md

---

# Sales Order API

```
GET    /sales-orders

GET    /sales-orders/{id}

POST   /sales-orders

PUT    /sales-orders/{id}

DELETE /sales-orders/{id}

POST   /sales-orders/{id}/approve

POST   /sales-orders/{id}/release

POST   /sales-orders/{id}/cancel

POST   /sales-orders/{id}/reserve
```

Reference

TASK-040_Sales_Order.md

---

# Shipment API

```
GET    /shipments

GET    /shipments/{id}

POST   /shipments

PUT    /shipments/{id}

DELETE /shipments/{id}

POST   /shipments/{id}/dispatch

POST   /shipments/{id}/complete

GET    /shipments/tracking/{id}
```

Reference

TASK-041_Shipment.md

---

# Delivery API

```
GET    /deliveries

GET    /deliveries/{id}

POST   /deliveries

PUT    /deliveries/{id}

DELETE /deliveries/{id}

POST   /deliveries/{id}/confirm

POST   /deliveries/{id}/accept

POST   /deliveries/{id}/reject

POST   /deliveries/{id}/complete
```

Reference

TASK-042_Delivery.md

---

# Customer Invoice API

```
GET    /customer-invoices

GET    /customer-invoices/{id}

POST   /customer-invoices

PUT    /customer-invoices/{id}

DELETE /customer-invoices/{id}

POST   /customer-invoices/{id}/approve

POST   /customer-invoices/{id}/issue

POST   /customer-invoices/{id}/cancel

POST   /customer-invoices/{id}/credit-note
```

Reference

TASK-043_Customer_Invoice.md

---

# Dashboard API

```
GET /sales/dashboard

GET /sales/dashboard/kpis

GET /sales/dashboard/charts

GET /sales/dashboard/forecast

GET /sales/dashboard/alerts

GET /sales/dashboard/ai
```

Reference

TASK-044_Sales_Dashboard.md

---

# Reports API

```
GET  /sales/reports

POST /sales/reports/generate

POST /sales/reports/export

POST /sales/reports/schedule

GET  /sales/reports/history
```

Reference

TASK-045_Sales_Reports.md

---

# File Upload API

Supports

- Quotation PDF
- Customer Documents
- Delivery Photos
- Digital Signatures
- Technical Drawings

```
POST /files/upload

GET /files/{id}

DELETE /files/{id}
```

Reference

TASK-012_File_Upload.md

---

# Bulk Operations

Supports

```
POST /customers/import

POST /quotations/import

POST /sales-orders/import

POST /deliveries/import
```

Export

```
GET /customers/export

GET /sales-orders/export

GET /reports/export
```

Formats

- Excel
- CSV
- PDF
- JSON

---

# Search API

Global search

```
GET /search
```

Example

```
GET /search?q=CLT
```

Returns

- Customers
- Leads
- Quotations
- Orders
- Deliveries
- Invoices

Reference

Search_Filtering.md

---

# Notifications API

```
GET /notifications

POST /notifications/read

POST /notifications/read-all

DELETE /notifications/{id}
```

Reference

Notification_System.md

---

# Event API

Published Events

```
LeadCreated

LeadConverted

OpportunityCreated

QuotationCreated

QuotationApproved

SalesOrderCreated

ShipmentCreated

DeliveryCompleted

CustomerInvoiceIssued
```

Consumed Events

```
InventoryReserved

ProductionCompleted

ShipmentDelivered

InvoicePaid

CustomerUpdated
```

Reference

Integration_Events.md

---

# Error Codes

| Code | Description |
|--------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content |
| 400 | Validation Error |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Not Found |
| 409 | Conflict |
| 422 | Business Rule Error |
| 429 | Rate Limit |
| 500 | Internal Server Error |

---

# Rate Limiting

Default

```
1000 requests/minute
```

Burst

```
5000 requests
```

Headers

```
X-RateLimit-Limit

X-RateLimit-Remaining

Retry-After
```

---

# Idempotency

Supports

```
POST

PUT

PATCH
```

Header

```
Idempotency-Key
```

---

# Webhooks

Supports

```
QuotationApproved

SalesOrderCreated

ShipmentDispatched

DeliveryCompleted

InvoiceIssued

PaymentReceived
```

Example

```
POST

https://partner.company.com/webhooks
```

---

# API Versioning

Current

```
v1
```

Future

```
v2

v3
```

Backward compatibility maintained.

---

# Security

Supports

- JWT Authentication
- OAuth2
- MFA
- HTTPS Only
- CORS
- CSRF Protection
- API Gateway
- WAF
- Audit Logging

Reference

Security.md

---

# Performance Targets

| Endpoint | Target |
|------------|---------|
| GET | <300 ms |
| POST | <1 sec |
| PUT | <1 sec |
| DELETE | <500 ms |
| Search | <300 ms |
| Dashboard | <2 sec |

---

# Future Integrations

Planned

- SAP
- Microsoft Dynamics
- Logo ERP
- Mikro ERP
- E-Invoice
- Dealer Portal
- Customer Portal
- AI Copilot
- Power BI
- Azure Event Hub

---

# Related Documents

Sales_Architecture.md

Sales_Workflow.md

Sales_Mobile.md

Sales_Dashboard.md

Sales_Reports.md

TASK-036_Customer.md

TASK-037_Lead.md

TASK-038_Opportunity.md

TASK-039_Quotation.md

TASK-040_Sales_Order.md

TASK-041_Shipment.md

TASK-042_Delivery.md

TASK-043_Customer_Invoice.md

TASK-044_Sales_Dashboard.md

TASK-045_Sales_Reports.md

Security.md

Permission_Model.md

Notification_System.md

Audit_Log.md

Integration_Events.md

Performance.md

Search_Filtering.md
