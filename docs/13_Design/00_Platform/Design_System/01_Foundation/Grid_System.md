# Grid System

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Grid System defines the layout structure used throughout Naswood OS.

It ensures consistent alignment, spacing and responsiveness across all pages, modules and devices.

Every screen must be built using the official grid system.

---

# Objectives

- Consistent Layout
- Responsive Design
- Predictable Alignment
- Enterprise Scalability
- Better Readability
- Faster UI Development

---

# Design Principles

The grid system should be

- Consistent
- Flexible
- Responsive
- Predictable
- Reusable

Grid should support both business forms and analytical dashboards.

---

# Layout Strategy

Desktop First

↓

Responsive

↓

Touch Optimized

---

# Container Width

| Device | Max Width |
|----------|----------:|
| Mobile | 100% |
| Tablet | 100% |
| Desktop | 1600 px |
| Ultra Wide | 1920 px |

Content should remain centered on large displays.

---

# Grid Structure

Desktop

12 Columns

Tablet

8 Columns

Mobile

4 Columns

---

# Column Gap

Default

24 px

Compact

16 px

Dashboard

24 px

Large

32 px

---

# Margin

Desktop

32 px

Tablet

24 px

Mobile

16 px

---

# Content Padding

Page

24 px

Card

24 px

Dialog

32 px

Drawer

24 px

Form

24 px

---

# Column Layout Examples

## Full Width

12 / 12

---

## Two Columns

6 / 6

---

## Sidebar Layout

3 / 9

---

## Dashboard

3 / 3 / 3 / 3

---

## Analytics

4 / 8

---

## Master Detail

4 / 8

---

# Dashboard Grid

Desktop

4 widgets per row

---

Tablet

2 widgets

---

Mobile

1 widget

---

# Forms

Simple Forms

1 Column

---

Business Forms

2 Columns

---

Complex Forms

3 Columns

Maximum

---

# Cards

Minimum Width

320 px

Preferred

400 px

Maximum

600 px

Cards automatically wrap.

---

# Tables

Always use full width.

Horizontal scrolling only when necessary.

Pinned columns supported.

---

# Dialogs

Small

480 px

Medium

720 px

Large

900 px

Fullscreen

100%

---

# Sidebar

Expanded

280 px

Collapsed

72 px

Overlay on mobile.

---

# Header

Height

64 px

Full width

Sticky

---

# Footer

Height

40 px

Optional

---

# Page Structure

Header

↓

Breadcrumb

↓

Toolbar

↓

Filters

↓

Content

↓

Footer

---

# Dashboard Structure

Header

↓

KPIs

↓

Charts

↓

Tables

↓

Recent Activity

---

# Analytics Layout

Filters

↓

KPIs

↓

Charts

↓

Data Grid

---

# Responsive Behaviour

## Mobile

4 Columns

Single Column Forms

Overlay Sidebar

Compact Tables

---

## Tablet

8 Columns

Two Column Forms

Collapsible Sidebar

---

## Desktop

12 Columns

Full Layout

Persistent Sidebar

---

## Ultra Wide

12 Columns

Multiple Panels

Digital Twin Ready

---

# CSS Grid

Preferred Layout

CSS Grid

Fallback

Flexbox

---

# Tailwind Guidelines

Use

grid

grid-cols-12

gap-6

container

max-w-screen-2xl

Avoid custom grid implementations.

---

# Layout Tokens

Container Width

1600 px

Grid Gap

24 px

Content Padding

24 px

Sidebar Width

280 px

Header Height

64 px

Footer Height

40 px

---

# Performance

Avoid deeply nested grids.

Maximum nesting

3 Levels

Lazy load heavy dashboard widgets.

---

# Accessibility

Logical reading order.

No visual-only positioning.

Support keyboard navigation.

Maintain layout at 200% zoom.

---

# Best Practices

✓ Use CSS Grid.

✓ Follow 12-column layout.

✓ Maintain consistent spacing.

✓ Keep forms aligned.

✓ Use responsive containers.

---

# Do

✓ Align components to the grid.

✓ Keep equal spacing.

✓ Test every breakpoint.

✓ Use predefined gaps.

---

# Don't

✗ Don't position elements manually.

✗ Don't use fixed widths.

✗ Don't create custom grid systems.

✗ Don't break alignment.

---

# Acceptance Criteria

Every page follows the official grid.

Responsive layouts work correctly.

Forms align consistently.

Dashboard widgets remain aligned.

Spacing follows design tokens.

Grid remains accessible.

---

# Related Documents

Breakpoints.md

Spacing.md

Design_Tokens.md

Application_Shell.md

Dashboard.md

Responsive.md

Cards.md

Forms.md

Tables.md
