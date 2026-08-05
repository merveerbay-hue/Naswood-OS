# TASK-034 — Purchasing Dashboard

**Module:** Purchasing

**Sprint:** Sprint 02 – Purchasing

**Category:** Analytics

**Priority:** High

**Estimated Effort:** 6 Days

**Status:** Planned

---

# Purpose

Develop the Purchasing Dashboard module for Naswood OS.

The Purchasing Dashboard provides real-time visibility into procurement activities, supplier performance, purchasing KPIs, approvals and operational risks.

It serves as the primary decision-support interface for buyers, purchasing managers and executives.

---

# Objectives

- Real-Time Purchasing Visibility
- Procurement KPI Monitoring
- Supplier Performance Tracking
- Approval Monitoring
- Cost Analysis
- AI Purchasing Insights
- Executive Reporting

---

# Scope

The Purchasing Dashboard includes

- Procurement KPIs
- Purchasing Summary
- Supplier Performance
- Purchase Order Status
- Purchase Request Status
- RFQ Monitoring
- Invoice Monitoring
- Cost Analysis
- Delivery Performance
- AI Recommendations

Out of Scope

- Transaction Processing
- Purchase Order Editing
- Supplier Master Maintenance
- Financial Posting

---

# Dashboard Architecture

```
Purchasing Modules

↓

Dashboard Service

↓

Analytics Engine

↓

KPI Aggregator

↓

Widget Engine

↓

Purchasing Dashboard
```

---

# Dashboard Layout

```
---------------------------------------------------------

Purchasing Dashboard

---------------------------------------------------------

KPI Cards

Charts

Supplier Performance

Open Approvals

Open Purchase Orders

Pending RFQs

Invoices

AI Insights

---------------------------------------------------------
```

---

# KPI Cards

Displays

- Purchase Requests
- Open RFQs
- Open Purchase Orders
- Goods Receipts Today
- Pending Supplier Invoices
- Active Suppliers
- Average Procurement Lead Time
- Procurement Spend

Each KPI supports drill-down.

---

# Purchase Request Widget

Displays

- Draft PR
- Submitted PR
- Approval Pending
- Approved
- Cancelled
- Department Breakdown

Reference

TASK-027_Purchase_Request.md

---

# RFQ Widget

Displays

- Draft RFQs
- Published RFQs
- Waiting Responses
- Closed RFQs
- Supplier Response Rate
- Average RFQ Cycle Time

Reference

TASK-028_RFQ.md

---

# Supplier Quotation Widget

Displays

- Quotations Received
- Technical Evaluation
- Commercial Evaluation
- Award Waiting
- Accepted Quotations
- Rejected Quotations

Reference

TASK-029_Supplier_Quotation.md

---

# Purchase Order Widget

Displays

- Draft Purchase Orders
- Approval Waiting
- Released Orders
- Partial Deliveries
- Delayed Deliveries
- Closed Orders

Reference

TASK-030_Purchase_Order.md

---

# Goods Receipt Widget

Displays

- Today's Receipts
- Partial Receipts
- Pending Receipts
- Quality Inspection Waiting
- Warehouse Receiving Volume

Reference

TASK-031_Goods_Receipt_PO.md

---

# Purchase Return Widget

Displays

- Open Returns
- Returns by Supplier
- Return Rate
- Credit Notes Waiting
- Return Cost

Reference

TASK-032_Purchase_Return.md

---

# Supplier Invoice Widget

Displays

- Pending Invoices
- Three-Way Matching Exceptions
- Approval Waiting
- Due Soon
- Overdue
- Paid

Reference

TASK-033_Supplier_Invoice.md

---

# Supplier Performance Widget

Displays

Supplier KPIs

- On-Time Delivery
- Quality Score
- Response Time
- Lead Time
- Purchase Volume
- Return Rate
- Overall Supplier Rating

Top 10 suppliers displayed.

Reference

TASK-026_Supplier.md

---

# Procurement Spend

Charts

- Monthly Spend
- Spend by Supplier
- Spend by Material Group
- Spend by Plant
- Spend by Department
- Spend by Currency

Supports drill-down analysis.

---

# Delivery Performance

Displays

- On-Time Deliveries
- Delayed Deliveries
- Early Deliveries
- Average Delivery Time
- Supplier Delivery Ranking

---

# Approval Center

Displays

- Purchase Requests Waiting
- Purchase Orders Waiting
- Invoice Approvals
- Return Approvals

Users can navigate directly to documents.

---

# Alerts

Displays

- Overdue RFQs
- Delayed Deliveries
- Expiring Supplier Certificates
- High Purchase Value
- Budget Exceeded
- Three-Way Matching Exceptions

---

# AI Insights

Displays

- Price Increase Forecast
- Supplier Risk
- Alternative Suppliers
- Demand Forecast
- Procurement Savings
- Inventory Replenishment Recommendation

Reference

AI_Copilot.md

---

# Search

Supports

- Purchase Order
- Purchase Request
- RFQ
- Supplier
- Material
- Invoice

Reference

Search_Filtering.md

---

# Filters

Supports

- Company
- Plant
- Buyer
- Supplier
- Material Group
- Department
- Status
- Date Range
- Currency

---

# Dashboard Refresh

Supports

- Automatic Refresh
- Manual Refresh
- Live Dashboard
- Background Updates

Refresh interval configurable.

---

# Export

Supports

- PDF
- Excel
- CSV
- Image

Reference

Printing.md

---

# API Endpoints

```
GET /api/v1/purchasing/dashboard

GET /api/v1/purchasing/dashboard/kpis

GET /api/v1/purchasing/dashboard/charts

GET /api/v1/purchasing/dashboard/alerts

GET /api/v1/purchasing/dashboard/ai
```

Reference

Purchasing_API.md

---

# Security

Supports

- Role-Based Dashboard
- Buyer Visibility
- Company Isolation
- Plant Isolation
- Financial Data Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Dashboard Viewed
- Dashboard Exported
- Filter Changed
- Widget Customized

Reference

Audit_Log.md

Logging.md

---

# Notifications

Displays

- Approval Requests
- Supplier Alerts
- Delivery Delays
- Invoice Exceptions
- Budget Warnings

Reference

Notification_System.md

---

# Events

Consumes

- PurchaseRequestCreated
- RFQPublished
- PurchaseOrderReleased
- GoodsReceiptPosted
- SupplierInvoiceApproved
- PurchaseReturnCreated

Publishes

- DashboardViewed
- DashboardExported

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- KPI Cards
- Approval Queue
- Supplier Performance
- Purchase Order Status
- Dashboard Alerts
- AI Insights

Reference

Purchasing_Mobile.md

---

# Performance

Targets

- Dashboard Load < 2 seconds
- KPI Load < 500 ms
- Chart Rendering < 1 second
- Live Refresh < 2 seconds
- Support 500 concurrent dashboard users

Reference

Performance.md

Caching.md

---

# Naswood Dashboard KPIs

Executive KPIs

- Total Purchasing Spend
- Monthly Procurement Value
- Cost Savings
- Active Suppliers
- Supplier Performance
- Open Purchase Orders
- Pending Approvals
- Delayed Deliveries

Operational KPIs

- Purchase Request Cycle Time
- RFQ Cycle Time
- Purchase Order Lead Time
- Goods Receipt Volume
- Return Rate
- Invoice Matching Rate

---

# Acceptance Criteria

The Purchasing Dashboard module shall

- Display real-time procurement KPIs.
- Provide drill-down navigation to purchasing documents.
- Monitor supplier performance and procurement costs.
- Display approval queues and operational alerts.
- Support AI-driven purchasing recommendations.
- Export dashboard information.
- Integrate with all Purchasing modules.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-026_Supplier.md
- TASK-027_Purchase_Request.md
- TASK-028_RFQ.md
- TASK-029_Supplier_Quotation.md
- TASK-030_Purchase_Order.md
- TASK-031_Goods_Receipt_PO.md
- TASK-032_Purchase_Return.md
- TASK-033_Supplier_Invoice.md
- Purchasing_API.md

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

Purchasing_Workflow.md

Purchasing_Mobile.md

TASK-026_Supplier.md

TASK-027_Purchase_Request.md

TASK-028_RFQ.md

TASK-029_Supplier_Quotation.md

TASK-030_Purchase_Order.md

TASK-031_Goods_Receipt_PO.md

TASK-032_Purchase_Return.md

TASK-033_Supplier_Invoice.md

TASK-035_Purchasing_Reports.md

Security.md

Permission_Model.md

Performance.md

Caching.md

Search_Filtering.md

Printing.md

Audit_Log.md

Logging.md

Notification_System.md

AI_Copilot.md

Event_Model.md

Integration_Events.md
