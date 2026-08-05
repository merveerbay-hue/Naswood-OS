# Cards

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Card component is a reusable container used to group related information, actions and visual elements throughout Naswood OS.

Cards provide structure, improve readability and create consistent layouts across dashboards, forms and business modules.

---

# Objectives

- Organize Information
- Improve Readability
- Support Responsive Layouts
- Reusable Component
- Consistent User Experience
- Enterprise Design Standard

---

# Design Principles

Cards should be

- Simple
- Structured
- Consistent
- Responsive
- Accessible

Cards are containers, not pages.

---

# Card Types

Standard Card

Dashboard Card

KPI Card

Information Card

Summary Card

Action Card

Status Card

AI Recommendation Card

Statistics Card

Document Card

Empty State Card

---

# Standard Structure

```
Card
│
├── Header
│     ├── Title
│     ├── Subtitle (Optional)
│     └── Actions
│
├── Body
│
└── Footer (Optional)
```

---

# Header

Contains

Title

Subtitle

Actions

Status Badge

Menu

---

# Body

Contains

Text

Tables

Charts

Forms

Lists

Images

KPIs

Widgets

---

# Footer

Contains

Primary Action

Secondary Action

Metadata

Links

---

# Card Sizes

| Size | Width |
|--------|-------|
| Small | 320 px |
| Medium | 480 px |
| Large | 640 px |
| Full Width | 100% |

---

# Height

Dynamic

Cards should grow with content.

Avoid fixed heights except dashboard widgets.

---

# Padding

Header

24 px

Body

24 px

Footer

24 px

---

# Border Radius

Default

8 px

Dashboard

12 px

Reference

Border_Radius.md

---

# Elevation

Default

Elevation 2

Hover

Elevation 3

Reference

Elevation.md

---

# Typography

Title

H5

Subtitle

Small

Body

Body

Metadata

Caption

Reference

Typography.md

---

# Color Usage

Background

Surface

Border

Neutral

Status

Semantic Colors

Reference

Color_Tokens.md

---

# Dashboard Cards

Used for

KPIs

Charts

Production

Inventory

Sales

Finance

Quality

Maintenance

AI

---

# KPI Cards

Contains

Title

Value

Trend

Comparison

Status

Icon

---

# Status Cards

Examples

Machine Running

Machine Stopped

Low Inventory

Maintenance Due

Quality Alert

---

# Action Cards

Contains

Description

Primary Button

Secondary Button

---

# AI Cards

Contains

Suggestion

Confidence

Reason

Actions

Generated Time

---

# Information Cards

Contains

General Information

Customer

Supplier

Machine

Material

Warehouse

---

# Document Cards

Contains

Document Name

Type

Owner

Date

Status

Actions

---

# Empty State Card

Contains

Illustration

Title

Description

Primary Action

---

# Card Actions

Menu

Refresh

Expand

Collapse

Delete

Edit

Export

Pin

---

# Loading State

Skeleton Loading

Preferred

Spinner

Optional

---

# Empty State

Illustration

Title

Description

Action

---

# Error State

Error Icon

Title

Description

Retry Button

---

# Responsive Behaviour

Desktop

Grid Layout

Tablet

Two Columns

Mobile

Single Column

Cards expand automatically.

---

# Accessibility

Keyboard Navigation

Required

Focus State

Visible

Minimum Contrast

WCAG AA

Interactive Cards

Role="button"

---

# Performance

Lazy load charts.

Virtualize long lists.

Avoid excessive nested cards.

---

# React API

```tsx
<Card>

    <CardHeader>

        <CardTitle>

        </CardTitle>

    </CardHeader>

    <CardContent>

    </CardContent>

    <CardFooter>

    </CardFooter>

</Card>
```

---

# Variants

Standard

Dashboard

Outlined

Flat

Interactive

Clickable

KPI

Status

AI

---

# Usage Rules

Cards should contain one logical subject.

Avoid placing cards inside cards.

Keep titles concise.

Do not overload a single card.

Use dashboard widgets for metrics.

---

# Best Practices

✓ One topic per card.

✓ Consistent spacing.

✓ Use semantic colors.

✓ Keep actions minimal.

✓ Maintain visual hierarchy.

---

# Do

✓ Inventory Summary

✓ Production Status

✓ Machine Information

✓ KPI Overview

✓ AI Recommendation

---

# Don't

✗ Full application pages

✗ Very long forms

✗ Nested cards

✗ Multiple unrelated datasets

✗ Decorative shadows

---

# Acceptance Criteria

Cards follow official layout.

Padding follows spacing tokens.

Elevation follows standards.

Responsive layout works correctly.

Accessibility requirements are satisfied.

Loading and empty states are implemented.

Interactive cards provide keyboard support.

---

# Related Documents

Buttons.md

Typography.md

Spacing.md

Border_Radius.md

Elevation.md

Color_Tokens.md

Dashboard.md

Accessibility.md

Design_Tokens.md
