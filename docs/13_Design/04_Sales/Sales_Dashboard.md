# Sales Dashboard

**Module:** Sales

**Version:** 1.0

**Status:** Approved

**Owner:** Naswood ERP Architecture Team

---

# Purpose

The Sales Dashboard provides a real-time operational and executive overview of the entire sales organization.

It consolidates information from CRM, Sales, Inventory, Production, Logistics and Finance into a single decision-support interface.

The dashboard is designed for

- Sales Representatives
- Sales Managers
- Commercial Directors
- General Managers
- Company Owners

---

# Dashboard Objectives

The dashboard enables users to

- Monitor Sales Pipeline
- Analyze Revenue
- Track Customer Performance
- Follow Production Status
- Monitor Deliveries
- Track Invoices
- Detect Business Risks
- Receive AI Recommendations

---

# Dashboard Architecture

```
CRM

↓

Sales

↓

Inventory

↓

Production

↓

Logistics

↓

Finance

↓

Analytics Engine

↓

Dashboard Widgets

↓

User
```

---

# Dashboard Layout

```
------------------------------------------------------------

Global Filters

------------------------------------------------------------

KPI Cards

------------------------------------------------------------

Sales Pipeline

Revenue Trend

------------------------------------------------------------

Quotations

Sales Orders

Production

------------------------------------------------------------

Shipments

Deliveries

Invoices

------------------------------------------------------------

Top Customers

Salespersons

Products

------------------------------------------------------------

Notifications

AI Insights

------------------------------------------------------------
```

---

# Global Filters

Available filters

- Company
- Plant
- Salesperson
- Customer
- Customer Group
- Region
- Product
- Product Group
- Currency
- Date Range
- Sales Channel

All widgets respond instantly to filter changes.

---

# KPI Cards

Displays

- Total Revenue
- Monthly Revenue
- Gross Margin
- Open Quotations
- Open Sales Orders
- Orders in Production
- Shipments Today
- Deliveries Today
- Outstanding Receivables
- Average Order Value
- Quotation Conversion Rate
- Opportunity Win Rate

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

↓

Sales Order
```

Shows

- Count
- Total Value
- Expected Revenue
- Conversion Rate
- Average Stage Duration

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

- Total Value
- Conversion Rate
- Average Approval Time

Reference

TASK-039_Quotation.md

---

# Sales Order Widget

Displays

- Draft Orders
- Approved Orders
- Orders Awaiting Production
- Orders in Production
- Ready for Shipment
- Delayed Orders
- Completed Orders

Metrics

- Open Value
- Order Count
- Average Processing Time

Reference

TASK-040_Sales_Order.md

---

# Production Widget

Displays

- Production Requests
- Active Production Orders
- Delayed Production
- Finished Products
- Capacity Utilization
- Bottlenecks

Reference

Production Module

---

# Shipment Widget

Displays

- Planned Shipments
- Loaded Vehicles
- In Transit
- Delivered Today
- Delayed Shipments

Metrics

- Shipment Accuracy
- On-Time Shipment %

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

- On-Time Delivery
- Customer Acceptance Rate

Reference

TASK-042_Delivery.md

---

# Customer Invoice Widget

Displays

- Today's Invoices
- Outstanding Invoices
- Overdue Invoices
- Paid Invoices

Metrics

- Collection Rate
- Days Sales Outstanding (DSO)
- Invoice Aging

Reference

TASK-043_Customer_Invoice.md

---

# Customer Performance Widget

Displays

- Top Customers
- Highest Revenue
- Largest Open Orders
- Customer Growth
- Customer Profitability
- Customer Satisfaction

Charts

- Revenue by Customer
- Revenue Trend
- Purchase Frequency

Reference

TASK-036_Customer.md

---

# Product Performance Widget

Displays

- Best Selling Products
- Product Revenue
- Product Margin
- Product Demand
- Slow Moving Products

Example

- CLT
- Glulam
- Thermowood
- Solid Panels
- Pellet

---

# Salesperson Performance

Displays

- Revenue
- Quotations
- Orders
- Win Rate
- Conversion Rate
- Average Order Value
- Customer Visits

Supports ranking.

---

# Regional Analysis

Displays

- Revenue by Region
- Customers by Region
- Sales Growth
- Active Projects
- Dealer Performance

---

# Forecast Widget

Displays

- Monthly Forecast
- Quarterly Forecast
- Annual Forecast
- Pipeline Forecast
- Forecast Accuracy

Uses

- Open Opportunities
- Sales Orders
- Historical Sales

---

# Notification Center

Displays

- Quotation Approval Required
- Customer Credit Limit Exceeded
- Delayed Production
- Delayed Shipment
- Delivery Issues
- Overdue Invoices
- Lost Opportunities

Priority

- Critical
- High
- Medium
- Low

---

# AI Insights

Provides

- Sales Forecast
- Churn Prediction
- Upselling Opportunities
- Cross-Selling Suggestions
- Pricing Optimization
- Customer Risk Analysis
- Next Best Action

Reference

AI_Copilot.md

---

# Charts

Supports

- Line Chart
- Bar Chart
- Pie Chart
- Donut Chart
- Area Chart
- Funnel Chart
- Heat Map
- KPI Cards
- Gauge Chart

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

# Dashboard Personalization

Users may

- Reorder Widgets
- Resize Widgets
- Hide Widgets
- Save Layout
- Create Personal Dashboards
- Reset Default Layout

---

# Auto Refresh

Supports

- Manual Refresh
- 1 Minute
- 5 Minutes
- 15 Minutes
- Live Refresh

---

# Export

Supports

- PDF
- Excel
- CSV
- PNG

Supports scheduled exports.

---

# Dashboard Security

Visibility depends on

- User Role
- Company
- Plant
- Sales Territory
- Customer Authorization

Managers can view subordinate data.

Reference

Permission_Model.md

---

# Mobile Dashboard

Supports

- KPI Cards
- Pipeline
- Revenue
- Notifications
- AI Insights
- Customer Summary

Optimized for tablet and mobile devices.

Reference

Sales_Mobile.md

---

# Performance Targets

| Widget | Target |
|----------|---------|
| Dashboard Load | < 2 sec |
| KPI Cards | < 500 ms |
| Charts | < 1 sec |
| Filters | < 300 ms |
| Drill Down | < 1 sec |
| Refresh | < 2 sec |

---

# Future Enhancements

Planned

- AI Chat Dashboard
- Voice Analytics
- Power BI Integration
- Predictive Sales Dashboard
- Customer Sentiment Analysis
- Live GPS Shipment Tracking
- Digital Twin Integration

---

# Related Documents

Sales_Architecture.md

Sales_Workflow.md

Sales_API.md

Sales_Mobile.md

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

AI_Copilot.md

Permission_Model.md

Notification_System.md

Performance.md
