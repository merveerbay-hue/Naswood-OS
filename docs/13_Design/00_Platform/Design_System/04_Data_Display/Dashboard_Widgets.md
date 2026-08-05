# Dashboard Widgets

**Module:** Design System

**Category:** Data Display

**Version:** 1.0

**Status:** Approved

---

# Purpose

Dashboard Widgets are reusable UI components that present business information, operational metrics and actionable insights within dashboards.

Widgets are modular, configurable and reusable across all Naswood OS modules.

Every dashboard must be composed of official Dashboard Widgets.

---

# Objectives

- Reusable Dashboard Components
- Consistent KPI Presentation
- Real-Time Monitoring
- Modular Architecture
- Responsive Layout
- Accessibility Compliance

---

# Design Principles

Widgets should be

- Focused
- Reusable
- Lightweight
- Configurable
- Responsive

One widget should communicate one business concept.

---

# Widget Types

KPI Widget

Chart Widget

Table Widget

List Widget

Timeline Widget

Calendar Widget

Gauge Widget

Progress Widget

Status Widget

Activity Widget

Notification Widget

AI Widget

Digital Twin Widget

Map Widget

Document Widget

---

# Standard Structure

```
Widget

├── Header
│     ├── Title
│     ├── Actions
│     └── Status
│
├── Content
│
└── Footer (Optional)
```

---

# Header

Contains

Title

Subtitle

Refresh

Expand

Settings

Export

Help

---

# Content

May contain

KPI

Chart

Table

Cards

List

Timeline

Progress

Image

Map

AI Summary

---

# Footer

Optional

Contains

Last Updated

Source

Trend

Navigation Link

---

# Widget Sizes

| Size | Grid Columns |
|--------|-------------:|
| XS | 2 |
| Small | 3 |
| Medium | 6 |
| Large | 9 |
| Full Width | 12 |

Widgets snap to the dashboard grid.

---

# Widget States

Loading

Ready

Refreshing

Empty

Error

Offline

---

# KPI Widget

Displays

Title

Value

Trend

Comparison

Status

Icon

Sparkline (Optional)

---

# Chart Widget

Supports

Line Chart

Bar Chart

Area Chart

Pie Chart

Donut Chart

Gauge

Heatmap

Reference

Charts.md

---

# Table Widget

Displays

Summary Tables

Top Records

Top Customers

Top Materials

Top Machines

Maximum

20 Rows

---

# List Widget

Examples

Pending Tasks

Recent Orders

Recent Production

Recent Alerts

Recent Activities

---

# Status Widget

Displays

Machine Status

Workflow Status

Inventory Status

Quality Status

System Status

---

# Progress Widget

Displays

Production Progress

Purchase Progress

Maintenance Progress

Delivery Progress

Project Progress

---

# Activity Widget

Displays

Latest Transactions

User Activities

Workflow Events

System Events

---

# Notification Widget

Displays

Unread Notifications

Workflow Alerts

AI Alerts

Machine Alarms

Reference

Notifications.md

---

# AI Widget

Displays

Recommendations

Predictions

Insights

Confidence Score

Suggested Actions

---

# Digital Twin Widget

Displays

Machine Health

Production Line

Energy Consumption

Equipment Status

Sensor Data

---

# Widget Actions

Refresh

Expand

Collapse

Fullscreen

Export

Print

Settings

Pin

Remove

---

# Personalization

Users may configure

Widget Position

Widget Size

Visible Widgets

Refresh Interval

Theme

Saved Layouts

---

# Refresh

Manual Refresh

Auto Refresh

Real-Time Refresh

Refresh interval is configurable.

---

# Responsive Behaviour

Desktop

Grid Layout

Tablet

Reduced Grid

Mobile

Single Column

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

Focus Indicators

High Contrast

---

# Performance

Lazy Load

Deferred Rendering

Virtualization

Memoization

Cached Queries

Parallel Requests

---

# Security

Widgets respect

Role Permissions

Module Permissions

Data Permissions

Sensitive Data Masking

Audit Logging

---

# React Structure

```tsx
<DashboardWidget
    type="kpi"
    title="Inventory Value"
    refreshable
    expandable
>
    <WidgetContent />
</DashboardWidget>
```

---

# Widget Registry

Every widget should register

Widget ID

Title

Category

Supported Sizes

Permissions

Refresh Strategy

Data Source

---

# Events

onRefresh

onExpand

onCollapse

onResize

onMove

onExport

onSettings

---

# Best Practices

✓ One business purpose per widget.

✓ Keep widgets lightweight.

✓ Load data lazily.

✓ Support personalization.

✓ Minimize refresh frequency.

✓ Display meaningful actions.

---

# Do

✓ Reuse widgets across modules.

✓ Keep headers concise.

✓ Display update timestamps.

✓ Show loading states.

✓ Support resizing.

---

# Don't

✗ Overload widgets.

✗ Mix unrelated metrics.

✗ Refresh continuously.

✗ Duplicate widgets.

✗ Hardcode widget sizes.

---

# Acceptance Criteria

Widgets use the official component.

Widget layout follows the grid.

Loading and error states are implemented.

Refresh functions correctly.

Accessibility complies with WCAG 2.1 AA.

Widgets support personalization.

Responsive behaviour works correctly.

---

# Related Documents

Dashboard.md

Cards.md

Charts.md

Tables.md

Data_Grid.md

Notifications.md

Responsive.md

Design_Tokens.md

Accessibility.md
