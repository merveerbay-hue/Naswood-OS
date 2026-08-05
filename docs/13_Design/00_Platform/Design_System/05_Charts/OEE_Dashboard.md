# OEE Dashboard

**Module:** Design System

**Category:** Charts

**Version:** 1.0

**Status:** Approved

---

# Purpose

The OEE Dashboard provides a standardized manufacturing performance dashboard for monitoring production efficiency, equipment utilization and operational effectiveness across Naswood OS.

The dashboard enables supervisors, production planners and executives to monitor manufacturing performance in real time.

---

# Objectives

- Real-Time Manufacturing Monitoring
- Improve Equipment Efficiency
- Reduce Downtime
- Support Production Decisions
- Standardize OEE Visualization
- Accessibility Compliance

---

# Design Principles

The OEE Dashboard should be

- Real-Time
- Actionable
- Production Focused
- Minimal
- Responsive

Production issues should be visible immediately.

---

# Standard Layout

```
Header

↓

Global Filters

↓

KPI Cards

↓

OEE Gauge

↓

Trend Charts

↓

Production Line Status

↓

Machine Status

↓

Downtime Analysis

↓

Alarms & Notifications
```

---

# Dashboard Sections

Production Summary

OEE Overview

Availability

Performance

Quality

Machine Status

Production Lines

Downtime

Alarms

Recent Events

---

# Global Filters

Plant

Factory

Production Line

Machine

Shift

Operator

Date Range

Product Family

---

# KPI Cards

Displays

Overall OEE

Availability

Performance

Quality

Production Output

Reject Rate

Machine Utilization

Energy Consumption

Downtime

Reference

KPI_Cards.md

---

# OEE Formula

```
OEE

=

Availability

×

Performance

×

Quality
```

Displayed as

Percentage

Example

85.6%

---

# Availability

Displays

Operating Time

Planned Time

Downtime

Availability %

---

# Performance

Displays

Ideal Cycle Time

Actual Output

Target Output

Performance %

---

# Quality

Displays

Good Parts

Rejected Parts

Rework

Quality %

---

# Production Metrics

Current Production

Target Production

Completed Orders

Remaining Orders

Cycle Time

Throughput

---

# Machine Status

Machine Name

Status

Current Job

Operator

Running Time

Downtime

OEE

Temperature

Energy

---

# Status Colors

Running

Green

Idle

Yellow

Maintenance

Blue

Alarm

Red

Offline

Gray

Reference

Color_Tokens.md

---

# Production Line View

Thermowood

Massive Panel

Pellet

Log Processing

Future Lines

CLT (Optional)

Glulam (Optional)

---

# Charts

Supports

OEE Trend

Availability Trend

Performance Trend

Quality Trend

Downtime Trend

Production Trend

Machine Utilization

Energy Usage

---

# Downtime Analysis

Displays

Planned Downtime

Unplanned Downtime

Maintenance

Setup

Material Waiting

Operator Waiting

Machine Failure

Power Failure

---

# Alarm Panel

Displays

Critical Alarms

Warnings

Maintenance Alerts

Machine Errors

Quality Alerts

Reference

Notifications.md

---

# AI Insights

Displays

Predicted Downtime

Recommended Maintenance

Efficiency Improvement

Production Bottlenecks

Energy Optimization

Forecasted OEE

---

# Timeline

Displays

Shift Events

Machine Stops

Production Events

Operator Changes

Maintenance Activities

---

# Dashboard Refresh

Real-Time

SignalR

WebSocket

Manual Refresh

Auto Refresh

Default

30 Seconds

---

# Responsive Behaviour

Desktop

Full Dashboard

Tablet

Adaptive Layout

Mobile

Summary Dashboard

Industrial Touch Panel

Optimized Touch Layout

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

High Contrast

Touch Targets

Focus Indicators

WCAG 2.1 AA

---

# Performance

Live Data Streaming

Lazy Loaded Widgets

Parallel Requests

Caching

Optimized Rendering

---

# Security

Dashboard respects

Role Permissions

Plant Permissions

Production Line Permissions

Machine Permissions

---

# React Structure

```tsx
<OeeDashboard>

    <DashboardFilters />

    <KpiCards />

    <OeeGauge />

    <TrendCharts />

    <MachineGrid />

    <DowntimePanel />

    <AlarmPanel />

    <AiInsights />

</OeeDashboard>
```

---

# User Preferences

Remember

Selected Plant

Production Line

Dashboard Layout

Refresh Interval

Visible Widgets

Theme

---

# Best Practices

✓ Display OEE prominently.

✓ Highlight critical alarms.

✓ Refresh data automatically.

✓ Use semantic colors.

✓ Show production targets.

✓ Keep charts readable.

---

# Do

✓ Show live production status

✓ Display OEE components

✓ Highlight downtime causes

✓ Provide drill-down

✓ Show AI recommendations

---

# Don't

✗ Hide production issues

✗ Mix unrelated KPIs

✗ Refresh excessively

✗ Display stale data

✗ Overload the dashboard

---

# Acceptance Criteria

Dashboard follows official layout.

OEE calculation displays correctly.

Machine status updates in real time.

Downtime analysis functions correctly.

Accessibility complies with WCAG 2.1 AA.

Responsive layout works across supported devices.

AI recommendations integrate correctly.

---

# Related Documents

Dashboard.md

Dashboard_Widgets.md

KPIs.md

KPI_Cards.md

Charts.md

Notifications.md

Responsive.md

Design_Tokens.md

Accessibility.md
