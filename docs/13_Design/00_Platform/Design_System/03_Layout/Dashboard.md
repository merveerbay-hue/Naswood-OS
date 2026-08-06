# Dashboard

**Module:** Design System

**Category:** Layout

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Dashboard defines the standard layout for presenting key business information, operational metrics, alerts and actionable insights across Naswood OS.

Dashboards provide users with a real-time overview of business operations and enable rapid decision making.

Every module may have its own dashboard, but all dashboards must follow the official layout and interaction standards.

---

# Objectives

- Present Key Business Information
- Improve Decision Making
- Standardize Dashboard Layout
- Support Real-Time Data
- Enterprise User Experience
- Accessibility Compliance

---

# Design Principles

Dashboards should be

- Informative
- Actionable
- Minimal
- Responsive
- Real-Time

Users should understand system status within seconds.

---

# Dashboard Types

Home Dashboard

Inventory Dashboard

Purchasing Dashboard

Sales Dashboard

Production Dashboard

Quality Dashboard

Maintenance Dashboard

Finance Dashboard

Analytics Dashboard

AI Dashboard

Digital Twin Dashboard

---

# Standard Layout

```
Dashboard

├── Header

├── KPI Row

├── Quick Actions

├── Charts

├── Operational Widgets

├── Recent Activities

├── Notifications

└── Footer (Optional)
```

---

# Header

Contains

Dashboard Title

Date

Refresh

Export

Settings

Filter

---

# KPI Section

Displays

Total Inventory

Orders

Revenue

Production

OEE

Efficiency

Downtime

Open Tasks

Users

Alerts

---

# KPI Cards

Each KPI card contains

Title

Value

Trend

Comparison

Status

Icon

Optional Sparkline

---

# Quick Actions

Quick actions open **job screens** — never a shared Create form.  
Authority: `docs/13_Design/Common/Screen_Types.md` § Create → Job CTA matrix.

Add material *(Explorer — master only)*

Receive goods → Receiving Wizard

Place purchase order → PO Wizard

Enter sales order → Sales Order Wizard

Plan production → Planning Wizard

Open work order → Maintenance WO Wizard

Open Reports

AI Assistant

Quick actions should be configurable per module.

---

# Charts

Supported

Bar Chart

Line Chart

Pie Chart

Area Chart

Stacked Bar

Donut Chart

Gauge

Heat Map

Timeline

---

# Operational Widgets

Inventory Levels

Machine Status

Production Queue

Purchase Status

Sales Pipeline

Maintenance Calendar

Quality Issues

Financial Summary

AI Insights

---

# Recent Activity

Displays

Latest Transactions

Recent Orders

Recent Production

Inventory Movements

Workflow Updates

User Activity

---

# Notifications

Displays

System Alerts

Workflow Notifications

Machine Alarms

Quality Alerts

AI Suggestions

Reference

Notifications.md

---

# Filters

Supports

Date Range

Warehouse

Machine

Production Line

Customer

Supplier

Shift

Status

---

# Widget Structure

```
Widget

├── Header

├── Content

├── Actions

└── Footer (Optional)
```

---

# Widget Types

KPI Widget

Chart Widget

Table Widget

List Widget

Timeline Widget

Calendar Widget

Map Widget

AI Widget

Digital Twin Widget

---

# Widget Actions

Refresh

Expand

Collapse

Export

Settings

Fullscreen

Pin

Remove

---

# Personalization

Users may customize

Widget Position

Widget Size

Visible Widgets

Dashboard Layout

Saved Views

Refresh Interval

Preferences are stored per user.

---

# Real-Time Updates

Supports

SignalR

WebSocket

Auto Refresh

Manual Refresh

Refresh intervals are configurable.

---

# Responsive Behaviour

Desktop

Multiple Columns

Tablet

Two Columns

Mobile

Single Column

Widgets stack vertically.

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

Lazy Loading

Deferred Widgets

Virtual Rendering

Data Caching

Parallel Requests

---

# Security

Widgets respect

Role Permissions

Module Permissions

Data Visibility

Audit Logging

---

# React Structure

```tsx
<Dashboard>

    <DashboardHeader />

    <KpiSection />

    <QuickActions />

    <DashboardGrid>

        <Widget />

        <Widget />

        <Widget />

    </DashboardGrid>

    <RecentActivity />

</Dashboard>
```

---

# Dashboard Grid

Desktop

12 Columns

Tablet

8 Columns

Mobile

4 Columns

Reference

Grid_System.md

---

# Widget Sizes

| Size | Columns |
|--------|--------:|
| Small | 3 |
| Medium | 6 |
| Large | 9 |
| Full | 12 |

Widgets should snap to the grid.

---

# User Preferences

Remember

Widget Order

Collapsed Widgets

Selected Filters

Refresh Interval

Theme

Dashboard Variant

---

# Best Practices

✓ Show only important KPIs.

✓ Keep widgets focused.

✓ Avoid excessive charts.

✓ Prioritize actionable information.

✓ Allow dashboard customization.

✓ Refresh data efficiently.

---

# Do

✓ Show real-time KPIs

✓ Use reusable widgets

✓ Group related information

✓ Support personalization

✓ Display operational alerts

---

# Don't

✗ Overload the dashboard

✗ Show unnecessary details

✗ Auto-refresh too frequently

✗ Mix unrelated widgets

✗ Duplicate information

---

# Acceptance Criteria

Dashboard follows the official layout.

Widgets align to the grid.

KPIs update correctly.

Real-time updates function properly.

Accessibility complies with WCAG 2.1 AA.

User preferences persist.

Responsive layout works across devices.

---

# Related Documents

Application_Shell.md

Cards.md

Data_Grid.md

Charts.md

Notifications.md

Grid_System.md

Design_Tokens.md

Accessibility.md
