# Tables

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Table component provides a simple, structured and lightweight way to display tabular information throughout Naswood OS.

Unlike the Data Grid component, Tables are intended for static or read-only datasets with limited interaction.

Use Tables for summaries, reports and reference information.

---

# Objectives

- Simple Data Presentation
- Readability
- Consistent Layout
- Responsive Display
- Accessibility Compliance

---

# Design Principles

Tables should be

- Simple
- Readable
- Lightweight
- Predictable
- Responsive

Tables display information.

They do not manage information.

---

# When To Use

Use Tables for

Dashboard Summaries

Reports

Preview Dialogs

Audit History

Document Details

Print Views

PDF Reports

Comparison Tables

Statistics

Read-only Information

---

# When NOT To Use

Do not use Tables for

CRUD Operations

Filtering

Sorting

Grouping

Bulk Actions

Large Datasets

Inline Editing

Virtual Scrolling

For these scenarios use

Data_Grid.md

---

# Standard Structure

```
Table

├── Caption (Optional)

├── Header

├── Body

├── Footer (Optional)

└── Summary (Optional)
```

---

# Columns

Supported

Text

Number

Currency

Percentage

Date

Status

Badge

Icon

Link

---

# Rows

Standard Row

Summary Row

Header Row

Footer Row

---

# Alignment

Text

Left

Numbers

Right

Dates

Center

Status

Center

Actions

Center

---

# Row Height

Default

48 px

Compact

40 px

Large

56 px

---

# Cell Padding

Horizontal

16 px

Vertical

12 px

Reference

Spacing.md

---

# Typography

Header

600 Weight

Body

400 Weight

Footer

500 Weight

Reference

Typography.md

---

# Borders

Horizontal borders only.

Avoid heavy grid lines.

Reference

Color_Tokens.md

---

# Zebra Rows

Optional.

Use subtle background color.

Do not reduce readability.

---

# Hover

Optional.

Highlight row background.

No elevation.

---

# Selection

Not supported.

Interactive selection belongs to Data Grid.

---

# Sorting

Not supported.

Use Data Grid if sorting is required.

---

# Filtering

Not supported.

---

# Editing

Not supported.

---

# Pagination

Not supported.

Tables should contain limited data.

Recommended

Maximum

100 Rows

---

# Empty State

Display

Illustration

Title

Description

---

# Loading State

Skeleton Rows

Optional

Spinner

---

# Error State

Error Message

Retry Button

---

# Responsive Behaviour

Desktop

Standard Table

Tablet

Horizontal Scroll

Mobile

Card Layout

---

# Accessibility

Supports

Screen Readers

Keyboard Navigation

High Contrast

Header Association

ARIA Roles

---

# Performance

Render efficiently.

Avoid unnecessary nesting.

Keep datasets small.

---

# React API

```tsx
<Table>

    <TableHeader />

    <TableBody />

    <TableFooter />

</Table>
```

---

# Recommended Libraries

Shadcn UI Table

TanStack Table (Simple Mode)

---

# Best Practices

✓ Keep tables simple.

✓ Limit row count.

✓ Align numeric values.

✓ Use consistent spacing.

✓ Keep headers concise.

---

# Do

✓ KPI Summary

✓ Monthly Report

✓ Audit Preview

✓ Financial Summary

✓ Material Specification

---

# Don't

✗ CRUD Operations

✗ Bulk Selection

✗ Virtual Scrolling

✗ Inline Editing

✗ Server Pagination

---

# Acceptance Criteria

Table uses official component.

Typography follows standards.

Spacing follows design tokens.

Responsive layout works correctly.

Accessibility passes WCAG 2.1 AA.

Dataset remains readable.

---

# Related Documents

Data_Grid.md

Typography.md

Spacing.md

Accessibility.md

Cards.md

Design_Tokens.md
