# Data Grid

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Data Grid is the standard component for displaying and managing structured business data throughout Naswood OS.

It supports enterprise-level features such as sorting, filtering, grouping, inline editing, virtualization, export and role-based actions.

All business modules must use the official Data Grid component.

---

# Objectives

- Enterprise Data Management
- High Performance
- Consistent User Experience
- Responsive Layout
- Accessibility Compliance
- Reusable Architecture
- Server-Side Integration

---

# Design Principles

The Data Grid must be

- Fast
- Predictable
- Accessible
- Scalable
- Configurable
- Keyboard Friendly

Large datasets must remain responsive.

---

# Typical Usage

The Data Grid is used in

- Materials
- Warehouses
- Inventory
- Purchase Orders
- Sales Orders
- Customers
- Suppliers
- Production Orders
- Work Orders
- Quality Inspections
- Maintenance
- Finance
- AI Results
- Audit Logs

---

# Grid Structure

```
Toolbar
│
├── Search
├── Filters
├── Bulk Actions
├── Export
├── Import
├── Refresh
└── Column Settings

↓

Header

↓

Rows

↓

Summary Row (Optional)

↓

Pagination
```

---

# Toolbar

The toolbar may include

- Global Search
- Advanced Filter
- Saved Views
- Refresh
- Density Selector
- Export
- Import
- Column Selector
- Full Screen
- Bulk Actions

Toolbar is configurable.

---

# Columns

Supported column types

- Text
- Number
- Currency
- Percentage
- Boolean
- Status
- Badge
- Date
- DateTime
- Time
- Image
- Avatar
- Barcode
- QR Code
- Progress
- Link
- Action Buttons

---

# Column Features

Supported features

- Resize
- Reorder
- Hide
- Pin Left
- Pin Right
- Sort
- Filter

User preferences are persisted.

---

# Rows

Supported row types

- Standard
- Expandable
- Tree
- Group
- Summary
- Editable
- Read Only

---

# Cell Types

Cells may contain

- Text
- Badge
- Icon
- Avatar
- Button
- Checkbox
- Switch
- Progress Bar
- Link
- Image
- Currency
- Status Indicator

---

# Selection

Supported

Single Row

Multiple Rows

Select All

Range Selection

Keyboard Selection

---

# Sorting

Supported

Ascending

Descending

Multi Column

Server Side

Natural Sorting

---

# Filtering

Supported

Quick Filter

Column Filter

Advanced Filter

Saved Filter

Multi Condition

Date Range

Numeric Range

---

# Search

Global Search

Column Search

Instant Search

Highlight Matches

Debounced Input

---

# Pagination

Default

25 Rows

Supported Sizes

25

50

100

250

500

Server-side pagination is recommended.

---

# Grouping

Supported

Single Group

Nested Groups

Expand

Collapse

Summary Rows

Aggregate Values

---

# Summary Row

May display

Record Count

Total Quantity

Total Amount

Average

Minimum

Maximum

Custom Calculations

---

# Bulk Actions

Delete

Export

Approve

Reject

Move

Assign

Archive

Print

Label Printing

---

# Row Actions

View

Edit

Duplicate

Delete

History

Audit Log

Download

Print

Custom Action

---

# Inline Editing

Supported

Single Cell

Entire Row

Validation

Undo

Cancel

Save

---

# Frozen Areas

Pinned Header

Pinned Columns

Pinned Summary

Sticky Toolbar

---

# Density Modes

Comfortable

Compact

Dense

User selectable.

---

# Empty State

Illustration

Title

Description

Primary Action

---

# Loading State

Skeleton Rows

Loading Indicator

Progress Bar

---

# Error State

Banner

Message

Retry Button

Diagnostic Details

---

# Export

Supported

Excel

CSV

PDF

Print

---

# Import

Supported

CSV

Excel

Preview Validation

Import Report

---

# Keyboard Shortcuts

| Shortcut | Action |
|----------|---------|
| Tab | Next Cell |
| Shift + Tab | Previous Cell |
| ↑ ↓ ← → | Navigation |
| Enter | Open |
| Space | Select |
| Ctrl + A | Select All |
| Ctrl + C | Copy |
| Ctrl + V | Paste (Editable) |
| Ctrl + F | Search |
| Delete | Delete |
| Esc | Cancel |

---

# Permissions

The Data Grid respects

Column Permissions

Row Permissions

Action Permissions

Module Permissions

Sensitive Data Masking

Read Only Mode

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

Focus Indicators

High Contrast

Minimum Row Height

48 px

---

# Performance

Required

Virtual Scrolling

Server Pagination

Server Sorting

Server Filtering

Lazy Rendering

Memoization

Target Dataset

1,000,000+ Rows

---

# User Preferences

Remember

Visible Columns

Column Width

Column Order

Sorting

Filters

Density

Page Size

Saved Views

---

# Responsive Behaviour

Desktop

Full Grid

Tablet

Reduced Columns

Mobile

Card View

Horizontal scroll only when unavoidable.

---

# React Component

```tsx
<DataGrid
    rows={rows}
    columns={columns}
    loading={loading}
    selectable
    searchable
    sortable
    filterable
    pageable
    exportable
    density="comfortable"
/>
```

---

# Events

onRowClick

onRowDoubleClick

onSelectionChange

onFilterChange

onSortChange

onPageChange

onColumnResize

onColumnReorder

onExport

onImport

---

# Recommended Libraries

TanStack Table

TanStack Virtual

React Hook Form

React Query

Lucide React

---

# Best Practices

✓ Use server-side pagination

✓ Enable virtualization

✓ Save user preferences

✓ Keep row height consistent

✓ Use semantic status badges

✓ Support keyboard navigation

---

# Do

✓ Search

✓ Filter

✓ Export

✓ Bulk Actions

✓ Saved Views

✓ Virtual Scroll

---

# Don't

✗ Load all records into memory

✗ Nest tables

✗ Use inconsistent row heights

✗ Disable keyboard navigation

✗ Hardcode columns

---

# Acceptance Criteria

Data Grid uses the official component.

Sorting works correctly.

Filtering works correctly.

Virtual scrolling performs efficiently.

Accessibility passes WCAG 2.1 AA.

User preferences persist.

Server-side operations are supported.

Large datasets remain responsive.

---

# Related Documents

Tables.md

Search.md

Buttons.md

Forms.md

Cards.md

Typography.md

Spacing.md

Accessibility.md

Design_Tokens.md
