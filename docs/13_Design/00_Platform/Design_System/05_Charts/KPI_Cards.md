# KPI Cards

**Module:** Design System

**Category:** Charts

**Version:** 1.0

**Status:** Approved

---

# Purpose

KPI Cards provide a standardized visual representation of key business metrics across Naswood OS.

They enable users to quickly understand business performance, identify trends and navigate to detailed information.

All dashboards must use the official KPI Card component.

---

# Objectives

- Consistent KPI Presentation
- Fast Decision Making
- Real-Time Monitoring
- Responsive Layout
- Accessibility Compliance
- Reusable Component

---

# Design Principles

KPI Cards should be

- Simple
- Informative
- Actionable
- Consistent
- Lightweight

One card represents one business metric.

---

# Standard Structure

```
KPI Card

├── Icon (Optional)

├── Title

├── Value

├── Unit (Optional)

├── Trend

├── Target (Optional)

├── Status

└── Footer
```

---

# Card Layout

```
+----------------------------------+

📦 Inventory Value

₺ 24,580,000

▲ +4.8%

Updated 2 min ago

+----------------------------------+
```

---

# Required Elements

Title

Value

Status

---

# Optional Elements

Icon

Unit

Trend

Comparison

Target

Sparkline

Description

Action Button

---

# Card Sizes

| Size | Width | Height |
|-------|------:|--------:|
| Small | 240 px | 120 px |
| Medium | 320 px | 140 px |
| Large | 420 px | 180 px |

---

# Typography

Title

H6

Value

Display

Trend

Body

Footer

Caption

Reference

Typography.md

---

# Value Formatting

Supported

Integer

Decimal

Currency

Percentage

Duration

Weight

Length

Volume

Area

Temperature

Energy

Custom Unit

---

# Trend Indicators

Increase

↓

Decrease

↓

Stable

↓

Target Achieved

↓

Target Missed

---

# Trend Colors

Positive

Success Color

Negative

Danger Color

Neutral

Information Color

Reference

Color_Tokens.md

---

# Status Levels

Excellent

Good

Normal

Warning

Critical

No Data

---

# Icons

Supported

Lucide React

Examples

Boxes

Factory

ShoppingCart

Users

DollarSign

BarChart

Bot

Reference

Icons.md

---

# Footer

Displays

Last Updated

Comparison Period

Data Source

Refresh Status

---

# Refresh

Manual

Automatic

Real-Time

Refresh interval configurable.

---

# Interaction

Supported

Open Details

Refresh

Pin

Favorite

Fullscreen

Export

---

# Loading State

Skeleton Card

Loading Indicator

Placeholder Values

---

# Empty State

No Data

Description

Retry Action

---

# Error State

Error Icon

Message

Retry

---

# Responsive Behaviour

Desktop

Multiple Cards

Tablet

Two Cards per Row

Mobile

Single Column

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

Lazy Loading

Cached Queries

Real-Time Streaming

Memoized Rendering

---

# Security

Cards respect

Role Permissions

Module Permissions

Department Permissions

Data Visibility

---

# React Component

```tsx
<KpiCard
    title="Production Output"
    value={185}
    unit="m³"
    trend={+4.8}
    comparison="Yesterday"
    status="good"
    icon={<Factory />}
    lastUpdated="2 min ago"
/>
```

---

# Supported Modules

Inventory

Purchasing

Sales

Production

Quality

Maintenance

Finance

HR

Analytics

AI

Digital Twin

---

# Best Practices

✓ Display one KPI per card.

✓ Keep titles short.

✓ Always display units.

✓ Show trend direction.

✓ Display last update time.

✓ Link to detailed reports.

---

# Do

✓ Inventory Value

✓ Production Output

✓ OEE

✓ Revenue

✓ Machine Availability

✓ AI Confidence

---

# Don't

✗ Display multiple KPIs in one card

✗ Use inconsistent units

✗ Hide trend direction

✗ Omit refresh status

✗ Overload cards with actions

---

# Acceptance Criteria

Cards follow the official layout.

Values use correct formatting.

Trend indicators display correctly.

Status colors follow design tokens.

Accessibility complies with WCAG 2.1 AA.

Responsive layout works correctly.

Real-time updates function as expected.

---

# Related Documents

KPIs.md

Dashboard.md

Dashboard_Widgets.md

Cards.md

Charts.md

Typography.md

Color_Tokens.md

Design_Tokens.md

Accessibility.md
