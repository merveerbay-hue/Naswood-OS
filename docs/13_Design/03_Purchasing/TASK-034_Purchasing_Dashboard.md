# TASK-034 — Purchasing Dashboard

**Module:** Purchasing

**Category:** Dashboard

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Purchasing Dashboard provides real-time visibility into procurement operations, supplier performance, purchasing commitments and approval workflows.

It enables Buyers, Purchasing Managers, Finance and Executive Management to monitor procurement activities through interactive KPIs, charts and operational widgets.

The dashboard serves as the operational control center of the Purchasing module.

---

# Objectives

- Real-Time Procurement Visibility
- Procurement KPI Monitoring
- Supplier Performance Analysis
- Approval Workflow Monitoring
- Budget Control
- Purchasing Trend Analysis
- Executive Decision Support

---

# Scope

The Purchasing Dashboard provides

- Operational KPIs
- Procurement Analytics
- Supplier Performance
- Open Procurement Documents
- Approval Queue
- Delivery Monitoring
- Spend Analysis
- AI Recommendations

The dashboard does NOT

- Edit procurement documents
- Execute transactions
- Manage approvals directly
- Modify financial records

---

# Dashboard Users

Supports

- Buyer
- Purchasing Manager
- Procurement Director
- Finance Manager
- Operations Manager
- Executive Management

Each role sees customized widgets based on permissions.

Reference

Permission_Model.md

---

# Dashboard Layout

```
------------------------------------------------------------
Purchasing Dashboard
------------------------------------------------------------

Top KPIs

Open PR
Open RFQ
Open PO
Pending Invoice

------------------------------------------------------------

Supplier Performance

Delivery Performance

Procurement Spend

------------------------------------------------------------

Approval Queue

Late Deliveries

AI Recommendations

------------------------------------------------------------

Charts

Spend Trend

Purchase Trend

Supplier Ranking

Budget Usage

------------------------------------------------------------
```

---

# KPI Widgets

## Purchase Requests

Displays

- Open PR
- Draft PR
- Pending Approval
- Approved Today
- Average Approval Time

---

## RFQ

Displays

- Open RFQ
- Published RFQ
- Pending Responses
- Awarded RFQ
- RFQ Cycle Time

---

## Purchase Orders

Displays

- Open PO
- Released PO
- Pending Delivery
- Late Delivery
- Completed PO

---

## Goods Receipt

Displays

- Today's Receipts
- Partial Deliveries
- Completed Deliveries
- Pending Receipts

Reference

TASK-031_Goods_Receipt_PO.md

---

## Supplier Invoice

Displays

- Pending Invoice
- Approved Invoice
- Three-Way Match Success
- Invoice Aging

Reference

TASK-033_Supplier_Invoice.md

---

# Supplier Performance

Displays

- On-Time Delivery %
- Delivery Accuracy
- Supplier Quality Score
- Average Lead Time
- Response Time
- Return Rate

Top Suppliers

Worst Performing Suppliers

Reference

TASK-026_Supplier.md

---

# Procurement Spend

Displays

- Monthly Spend
- Annual Spend
- Spend by Supplier
- Spend by Material Group
- Spend by Plant
- Spend by Department

Supports drill-down.

---

# Budget Monitoring

Displays

- Budget Used
- Remaining Budget
- Budget Utilization %
- Purchase Commitments
- Forecast Spend

Reference

08_Finance

---

# Approval Queue

Displays

Pending

- Purchase Requests
- RFQ
- Purchase Orders
- Supplier Invoices

Supports

- Priority
- Waiting Time
- Responsible Approver

Reference

Approval_Workflow.md

---

# Delivery Monitoring

Displays

- Expected Deliveries
- Late Deliveries
- Supplier Delays
- Partial Deliveries
- Delivery Calendar

---

# Purchasing Analytics

Charts

- Procurement Spend Trend
- Monthly Purchasing
- Supplier Ranking
- Purchase Order Trend
- Lead Time Trend
- Material Cost Trend

---

# AI Widgets

AI provides

- Supplier Recommendation
- Best Price Opportunity
- Budget Risk
- Supplier Risk
- Price Increase Warning
- Delivery Delay Prediction
- Procurement Optimization Suggestions

Reference

AI_Copilot.md

---

# Interactive Filters

Supports

- Company
- Plant
- Buyer
- Supplier
- Material Group
- Warehouse
- Currency
- Department
- Status
- Date Range

---

# Drill Down

Every widget supports navigation to source documents.

Example

```
Open Purchase Orders

↓

Purchase Order List

↓

Purchase Order Detail
```

---

# Alerts

Supports

- Budget Exceeded
- Late Approval
- Supplier Delay
- Invoice Mismatch
- Certificate Expiration
- Purchase Order Delay
- Missing Goods Receipt

Reference

Notification_System.md

---

# Mobile Dashboard

Displays

- Top KPIs
- Approval Queue
- Supplier Alerts
- Procurement Spend
- Today's Deliveries

Optimized for tablets and smartphones.

Reference

Purchasing_Mobile.md

---

# Refresh Strategy

Supports

- Manual Refresh
- Automatic Refresh
- Real-Time Events
- Cached Analytics

Recommended refresh intervals

| Widget | Refresh |
|----------|----------|
| KPIs | Real Time |
| Approval Queue | Real Time |
| Deliveries | 1 Minute |
| Spend Charts | 5 Minutes |
| AI Widgets | 10 Minutes |

Reference

Caching.md

---

# Security

Dashboard visibility follows

- Role-Based Access
- Company Authorization
- Plant Authorization
- Department Authorization

Reference

Security.md

Permission_Model.md

---

# Performance

Dashboard shall

- Load within 3 seconds.
- Support 100+ concurrent users.
- Cache analytical widgets.
- Load charts asynchronously.
- Support drill-down without reload.

Reference

Performance.md

Caching.md

---

# Events

Dashboard subscribes to

- PurchaseRequestCreated
- PurchaseOrderApproved
- GoodsReceiptPosted
- SupplierInvoicePosted
- SupplierPerformanceUpdated
- BudgetUpdated

Reference

Event_Model.md

Integration_Events.md

---

# Reports

Dashboard links to

- Purchase Request Report
- RFQ Report
- Purchase Order Report
- Goods Receipt Report
- Supplier Performance Report
- Procurement Spend Report
- Invoice Report

Reference

TASK-035_Purchasing_Reports.md

---

# API

Dashboard uses

```
GET /dashboard/purchasing

GET /dashboard/purchasing/kpis

GET /dashboard/purchasing/spend

GET /dashboard/purchasing/suppliers

GET /dashboard/purchasing/approvals

GET /dashboard/purchasing/deliveries

GET /dashboard/purchasing/ai
```

Reference

Purchasing_API.md

---

# Audit

Dashboard access logs

- User Login
- Dashboard View
- Filter Changes
- Export Actions
- Drill Down Actions

Reference

Audit_Log.md

---

# Naswood Implementation

Executive dashboard includes

Production Purchasing

↓

Timber Procurement

↓

Chemical Procurement

↓

Packaging Procurement

↓

Machinery Procurement

↓

Supplier Performance

↓

Budget Consumption

↓

AI Procurement Insights

Real-time KPIs monitor

- Procurement Spend
- Open Purchase Orders
- Delivery Performance
- Supplier Reliability
- Invoice Matching Rate
- Purchasing Efficiency

---

# Acceptance Criteria

The Purchasing Dashboard shall

- Display real-time procurement KPIs.
- Support role-based dashboards.
- Monitor procurement workflows.
- Display supplier performance metrics.
- Support drill-down navigation.
- Provide AI-assisted procurement insights.
- Integrate with Purchasing, Inventory and Finance.
- Follow all shared platform standards.

---

# Related Documents

Purchasing_Architecture.md

Purchasing_API.md

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

Approval_Workflow.md

Permission_Model.md

Notification_System.md

Performance.md

Caching.md

Security.md

Audit_Log.md

AI_Copilot.md

Event_Model.md

Integration_Events.md
