# Standard Charts

**Module:** Design System

**Category:** Charts

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Standard Charts specification defines the official visualization standards used throughout Naswood OS.

Charts communicate business performance, operational trends and analytical insights using a consistent visual language.

All modules must use the official chart components.

---

# Objectives

- Standardize Data Visualization
- Improve Decision Making
- Ensure Visual Consistency
- Support Real-Time Data
- Accessibility Compliance
- Responsive Design

---

# Design Principles

Charts should be

- Simple

- Accurate

- Readable

- Consistent

- Actionable

Charts should emphasize data, not decoration.

---

# Supported Chart Types

Line Chart

Bar Chart

Stacked Bar Chart

Horizontal Bar Chart

Area Chart

Stacked Area Chart

Pie Chart

Donut Chart

Gauge Chart

Progress Chart

Heat Map

Timeline Chart

Scatter Plot

Radar Chart

Waterfall Chart

Tree Map

---

# Usage Guidelines

| Chart | Recommended Usage |
|---------|------------------|
| Line | Trends over time |
| Bar | Category comparison |
| Horizontal Bar | Rankings |
| Stacked Bar | Composition comparison |
| Area | Cumulative trends |
| Pie | Part-to-whole (max 6 slices) |
| Donut | Composition with central KPI |
| Gauge | OEE / Capacity |
| Heat Map | Utilization / Density |
| Scatter | Correlation |
| Timeline | Production events |
| Waterfall | Financial analysis |
| Tree Map | Hierarchical values |

---

# Standard Layout

```
Chart

├── Header

├── Filters (Optional)

├── Visualization

├── Legend

└── Footer
```

---

# Header

Displays

Chart Title

Subtitle

Refresh

Export

Fullscreen

Help

---

# Legend

Position

Bottom

Default

Interactive

Supports

Hide Series

Show Series

Highlight Series

---

# Tooltip

Displays

Series Name

Value

Unit

Timestamp

Comparison

Additional Metadata

---

# Axis

Supports

Title

Ticks

Labels

Grid Lines

Units

Automatic Scaling

---

# Colors

Use semantic colors.

Reference

Color_Tokens.md

Avoid custom colors unless required.

---

# Grid

Subtle horizontal grid lines.

Vertical grid lines optional.

---

# Labels

Keep concise.

Avoid overlapping labels.

Use abbreviations only when necessary.

---

# Animations

Supported

Initial Load

Hover

Refresh

Transition

Animation duration

≤ 300 ms

Reference

Animation.md

---

# Data Refresh

Manual

Automatic

Real-Time

Refresh interval configurable.

---

# Empty State

Display

No Data

Description

Retry Action

---

# Loading State

Skeleton Chart

Spinner

Progress Indicator

---

# Error State

Chart Error

Retry Button

Diagnostic Message

---

# Drill Down

Supported

Summary

↓

Detail

↓

Transaction

---

# Drill Through

Supported

Open Detail View

Open Report

Open Dashboard

Open Data Grid

---

# Export

Supported

PNG

SVG

PDF

Excel

CSV

Print

---

# Interaction

Zoom

Pan

Hover

Selection

Legend Toggle

Fullscreen

Context Menu

---

# Responsive Behaviour

Desktop

Full Chart

Tablet

Adaptive Layout

Mobile

Simplified Layout

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

High Contrast

Color Independent Indicators

WCAG 2.1 AA

---

# Performance

Lazy Loading

Data Caching

Incremental Updates

Virtual Rendering

Optimized Rendering

---

# Security

Charts respect

Role Permissions

Department Permissions

Module Permissions

Sensitive Data Masking

---

# Recommended Libraries

Primary

Recharts

Secondary

Apache ECharts

Specialized

Plotly (Engineering)

Charts should use a common wrapper component.

---

# React Structure

```tsx
<Chart>

    <ChartHeader />

    <ChartContent />

    <ChartLegend />

</Chart>
```

---

# Standard Wrapper

```tsx
<NsChart
    type="line"
    title="Production Trend"
    data={data}
    exportable
    fullscreen
/>
```

---

# Best Practices

✓ Select the correct chart type.

✓ Display units.

✓ Keep legends simple.

✓ Minimize colors.

✓ Enable drill-down.

✓ Support export.

---

# Do

✓ Show trends

✓ Compare categories

✓ Display real-time production

✓ Visualize OEE

✓ Show inventory movement

---

# Don't

✗ Use 3D charts

✗ Overload charts

✗ Use excessive colors

✗ Mix unrelated metrics

✗ Hide axis labels

---

# Acceptance Criteria

Charts follow official standards.

Legends behave consistently.

Tooltips display correctly.

Responsive layout works.

Accessibility complies with WCAG 2.1 AA.

Export functions correctly.

Performance remains acceptable.

---

# Related Documents

Dashboard.md

Dashboard_Widgets.md

KPIs.md

KPI_Cards.md

OEE_Dashboard.md

Reports.md

Design_Tokens.md

Color_Tokens.md

Typography.md

Accessibility.md
