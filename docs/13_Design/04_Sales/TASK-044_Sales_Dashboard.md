# TASK-044 — Sales Dashboard

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Sales Dashboard provides a real-time operational and strategic overview of the Sales organization.

It consolidates CRM, Sales, Production, Inventory, Logistics and Finance data into a single interface, enabling users to monitor sales performance, identify risks, manage pipelines and make informed decisions.

The dashboard serves as the primary workspace for Sales Representatives, Sales Managers, Commercial Directors and Executives.

---

# Design Goals

The module is designed to

- Provide real-time sales visibility
- Monitor sales pipeline
- Track quotation performance
- Monitor order fulfillment
- Analyze revenue and profitability
- Support executive decision making
- Deliver AI-powered insights

---

# Dashboard Layout

```
────────────────────────────────────────────────────────────

Global Filters

────────────────────────────────────────────────────────────

KPI Cards

────────────────────────────────────────────────────────────

Pipeline              Revenue Trend

────────────────────────────────────────────────────────────

Open Quotations       Sales Orders

────────────────────────────────────────────────────────────

Production Status     Shipment Status

────────────────────────────────────────────────────────────

Delivery Performance  Invoice Status

────────────────────────────────────────────────────────────

Top Customers         Top Products

────────────────────────────────────────────────────────────

Salespersons          AI Insights

────────────────────────────────────────────────────────────

Notifications

────────────────────────────────────────────────────────────
```

---

# Global Filters

Available filters

- Company
- Plant
- Salesperson
- Customer
- Dealer
- Region
- Product Group
- Currency
- Date Range
- Order Status
- Sales Channel

Changing filters refreshes every widget instantly.

---

# KPI Cards

Displays

- Total Revenue
- Revenue This Month
- Gross Margin
- Open Quotations
- Quotation Value
- Open Sales Orders
- Orders in Production
- Ready for Shipment
- Delivered Today
- Outstanding Invoices
- Collection Rate
- Forecast Revenue

Each KPI supports drill-down.

---

# Sales Pipeline Widget

Displays

```
Lead

↓

Qualified

↓

Opportunity

↓

Quotation

↓

Negotiation

↓

Won
```

Metrics

- Number of Opportunities
- Pipeline Value
- Average Deal Size
- Conversion %
- Win Rate
- Lost Rate

Reference

TASK-037_Lead.md

TASK-038_Opportunity.md

---

# Revenue Widget

Charts

- Daily Revenue
- Weekly Revenue
- Monthly Revenue
- Quarterly Revenue
- Annual Revenue

Comparisons

- Previous Period
- Budget
- Forecast
- Previous Year

Supports cumulative revenue visualization.

---

# Quotation Widget

Displays

- Draft Quotations
- Pending Approval
- Sent Quotations
- Accepted Quotations
- Rejected Quotations
- Expired Quotations

Metrics

- Acceptance Rate
- Average Approval Time
- Average Quotation Value
- Conversion Rate

Reference

TASK-039_Quotation.md

---

# Sales Order Widget

Displays

- Draft Orders
- Released Orders
- Orders Awaiting Production
- Orders in Production
- Ready for Shipment
- Delayed Orders
- Completed Orders

Metrics

- Open Order Value
- Fulfillment Rate
- Average Processing Time

Reference

TASK-040_Sales_Order.md

---

# Production Widget

Displays

- Production Requests
- Active Production Orders
- Delayed Production Orders
- Capacity Utilization
- Manufacturing Progress
- Estimated Completion Dates

Reference

Production Module

---

# Shipment Widget

Displays

- Planned Shipments
- Loaded Vehicles
- In Transit
- Delayed Shipments
- Delivered Today

Metrics

- On-Time Shipment %
- Shipment Accuracy
- Average Loading Time

Reference

TASK-041_Shipment.md

---

# Delivery Widget

Displays

- Planned Deliveries
- Completed Deliveries
- Partial Deliveries
- Rejected Deliveries
- Delivery Exceptions

Metrics

- On-Time Delivery %
- Customer Acceptance %
- POD Completion %

Reference

TASK-042_Delivery.md

---

# Invoice Widget

Displays

- Draft Invoices
- Issued Invoices
- Paid Invoices
- Outstanding Invoices
- Overdue Invoices

Metrics

- Collection Rate
- DSO
- Invoice Value
- Payment Performance

Reference

TASK-043_Customer_Invoice.md

---

# Customer Performance Widget

Displays

- Top Customers
- Highest Revenue
- Fastest Growing Customers
- Customer Profitability
- Outstanding Balance

Charts

- Revenue by Customer
- Customer Trend
- Customer Lifetime Value

Reference

TASK-036_Customer.md

---

# Product Performance Widget

Displays

- Best Selling Products
- Product Revenue
- Gross Margin
- Order Quantity
- Production Demand

Examples

- CLT Panels
- Glulam
- Thermowood
- Solid Panels
- Pellet

---

# Salesperson Performance

Displays

- Revenue
- Orders
- Quotations
- Win Rate
- Conversion Rate
- Customer Visits
- Forecast Achievement

Supports ranking and leaderboard.

---

# Regional Performance

Displays

- Revenue by Region
- Dealer Performance
- Export Markets
- Domestic Sales
- Active Projects

Maps supported.

---

# Forecast Widget

Displays

- Current Forecast
- Pipeline Forecast
- Revenue Forecast
- Forecast Accuracy
- Closing Probability

Forecast sources

- Open Opportunities
- Quotations
- Sales Orders

---

# AI Insights

Provides

- Revenue Prediction
- Opportunity Prioritization
- Churn Prediction
- Upselling Opportunities
- Cross-selling Suggestions
- Customer Risk Detection
- Delayed Order Prediction

Reference

AI_Copilot.md

---

# Notification Panel

Displays

- Approval Requests
- Delayed Quotations
- Delayed Orders
- Production Delays
- Shipment Delays
- Delivery Issues
- Overdue Invoices
- Credit Limit Warnings

Priority

- Critical
- High
- Medium
- Low

---

# Dashboard Personalization

Users may

- Rearrange Widgets
- Resize Widgets
- Hide Widgets
- Save Layout
- Create Personal Dashboard
- Restore Default Layout

---

# Drill Down

Every widget supports

```
Dashboard

↓

Summary

↓

Customer

↓

Document

↓

Transaction
```

Example

```
Revenue

↓

Customer

↓

Sales Order

↓

Invoice
```

---

# Auto Refresh

Supports

- Manual Refresh
- Every Minute
- Every 5 Minutes
- Every 15 Minutes
- Live Mode

---

# Export

Supports

- PDF
- Excel
- CSV
- PNG

Dashboard snapshots can be scheduled automatically.

---

# Search

Supports global search

- Customer
- Opportunity
- Quotation
- Sales Order
- Shipment
- Delivery
- Invoice
- Product

---

# Validation Rules

The system validates

- Company selection is required.
- Date range is valid.
- User may only view authorized companies.
- Dashboard layout is stored per user.
- KPI calculations follow company currency rules.

---

# Permissions

Supports

- View Dashboard
- View Financial KPIs
- View Sales KPIs
- View Production KPIs
- Export Dashboard
- Personalize Dashboard
- Schedule Reports

Reference

Permission_Model.md

---

# Notifications

Triggers

- KPI Threshold Exceeded
- Revenue Target Reached
- Revenue Below Target
- Order Delay
- Production Delay
- Delivery Delay
- Invoice Overdue

Reference

Notification_System.md

---

# Audit

Records

- Dashboard Layout Changed
- Widget Added
- Widget Removed
- Filters Saved
- Dashboard Exported

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- KPI Cards
- Pipeline
- Revenue
- Orders
- Shipments
- Deliveries
- Notifications
- AI Insights

Widgets automatically adapt to tablet and mobile screens.

Reference

Sales_Mobile.md

---

# API References

```http
GET /sales/dashboard

GET /sales/dashboard/kpis

GET /sales/dashboard/charts

GET /sales/dashboard/pipeline

GET /sales/dashboard/forecast

GET /sales/dashboard/notifications

GET /sales/dashboard/ai
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Lead
- Opportunity
- Quotation
- Sales Order
- Shipment
- Delivery
- Customer Invoice
- Production
- Inventory
- Finance
- Reports

---

# UI Components

Uses standard platform components

- KPI Cards
- Line Charts
- Bar Charts
- Pie Charts
- Funnel Charts
- Heat Maps
- Gauge Charts
- Data Grid
- Timeline
- Notification Center
- AI Insight Panel

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — Executive Dashboard

```
Revenue

₺148.5M

↓

Gross Margin

31%

↓

Orders in Production

42

↓

Forecast

₺165M
```

---

### Example 2 — Sales Manager

```
Pipeline

₺38.2M

↓

Open Quotations

27

↓

Conversion Rate

43%

↓

Top Salesperson

Ahmet Yılmaz
```

---

### Example 3 — Export Sales Dashboard

```
Export Revenue

€8.4M

↓

Germany

42%

↓

Middle East

31%

↓

Outstanding Export Orders

18
```
