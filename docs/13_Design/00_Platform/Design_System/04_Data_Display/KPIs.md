# KPIs

**Module:** Design System

**Category:** Data Display

**Version:** 1.0

**Status:** Approved

---

# Purpose

The KPI (Key Performance Indicator) component provides a standardized way to display critical business metrics throughout Naswood OS.

KPIs enable users to quickly understand operational performance, identify trends and take immediate action.

Every module must use the official KPI component.

---

# Objectives

- Standardize KPI Presentation
- Improve Decision Making
- Support Real-Time Monitoring
- Enterprise Consistency
- Responsive Design
- Accessibility Compliance

---

# Design Principles

KPIs should be

- Simple
- Actionable
- Readable
- Consistent
- Real-Time

One KPI represents one measurable business outcome.

---

# KPI Categories

Inventory

Purchasing

Sales

Production

Quality

Maintenance

Finance

Logistics

HR

AI

System

Digital Twin

---

# Standard KPI Structure

```
KPI

├── Icon
├── Title
├── Value
├── Unit
├── Trend
├── Comparison
├── Status
└── Last Updated
```

---

# Required Elements

Title

Value

Status

---

# Optional Elements

Icon

Trend

Comparison

Target

Sparkline

Description

Action

---

# KPI Sizes

| Size | Width | Height |
|-------|------:|--------:|
| Small | 240 px | 120 px |
| Medium | 320 px | 140 px |
| Large | 480 px | 180 px |

---

# KPI Layout

```
+----------------------------------+

Inventory Value

₺ 24,580,000

▲ +4.8%

Updated 2 min ago

+----------------------------------+
```

---

# Supported Value Types

Integer

Decimal

Currency

Percentage

Duration

Weight

Length

Area

Volume

Temperature

Energy

Count

Custom Units

---

# Trend Indicators

Increase

Decrease

No Change

Target Achieved

Target Missed

---

# Trend Periods

Today

Yesterday

This Week

Last Week

This Month

Last Month

Quarter

Year

Custom Range

---

# Status Levels

Excellent

Good

Normal

Warning

Critical

Status colors follow Color_Tokens.md.

---

# Comparison Types

Previous Period

Target

Budget

Forecast

Industry Benchmark

Previous Year

---

# KPI Refresh

Manual

Automatic

Real-Time

Refresh interval is configurable.

---

# KPI Examples

## Inventory

Inventory Value

Available Stock

Reserved Stock

Low Stock Items

Stock Accuracy

Inventory Turnover

---

## Purchasing

Open Purchase Orders

Supplier Lead Time

Pending Receipts

Purchase Spend

Supplier Performance

---

## Sales

Sales Revenue

Orders Today

Gross Margin

Open Quotations

Conversion Rate

---

## Production

Production Output

OEE

Machine Utilization

Downtime

Scrap Rate

Production Efficiency

---

## Quality

Inspection Pass Rate

Rejected Parts

Open NCR

Customer Complaints

CAPA Completion

---

## Maintenance

Machine Availability

Preventive Maintenance

Open Work Orders

MTBF

MTTR

---

## Finance

Revenue

Cash Flow

Accounts Receivable

Accounts Payable

Profit Margin

Budget Utilization

---

## AI

Prediction Accuracy

AI Recommendations

Accepted Suggestions

Optimization Score

Confidence Level

---

# KPI States

Loading

Ready

Refreshing

Offline

Error

No Data

---

# User Actions

Open Details

Refresh

View Report

Export

Pin

Favorite

---

# Responsive Behaviour

Desktop

Multiple KPI Cards

Tablet

Two Cards per Row

Mobile

Single Card

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

High Contrast

Focus Indicators

---

# Performance

Cached Data

Incremental Updates

Real-Time Streaming

Lazy Loading

---

# Security

KPIs respect

Role Permissions

Department Permissions

Module Permissions

Data Visibility

---

# React Structure

```tsx
<KpiCard
    title="Production Output"
    value={1250}
    unit="m³"
    trend="+5.2%"
    status="good"
    comparison="Yesterday"
/>
```

---

# Widget Integration

KPIs may appear inside

Dashboard

Cards

Reports

Detail Views

Digital Twin

AI Dashboard

---

# Best Practices

✓ Display one metric per card.

✓ Show update time.

✓ Use semantic colors.

✓ Keep titles short.

✓ Highlight trends.

✓ Link to detailed reports.

---

# Do

✓ Show trend

✓ Show comparison

✓ Display unit

✓ Keep values readable

✓ Refresh automatically

---

# Don't

✗ Mix multiple KPIs in one card

✗ Use unclear abbreviations

✗ Hide update time

✗ Display stale data

✗ Overuse colors

---

# Acceptance Criteria

KPIs follow the official layout.

Values display correctly.

Status colors follow design tokens.

Trends update correctly.

Accessibility complies with WCAG 2.1 AA.

Responsive behaviour works across devices.

Real-time updates are supported.

---

# Related Documents

Dashboard.md

Dashboard_Widgets.md

Cards.md

Charts.md

Color_Tokens.md

Typography.md

Design_Tokens.md

Accessibility.md
