# Sorting

**Module:** Shared

**Category:** Data Sorting

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Sorting standard defines how business data is ordered, prioritized and presented throughout Naswood OS.

It provides a consistent sorting experience across web, mobile, APIs, reports and dashboards while ensuring predictable behavior and optimal performance.

All sortable collections must follow this standard.

---

# Objectives

- Consistent Sorting
- Predictable User Experience
- Efficient Database Queries
- Reusable Sorting Logic
- Scalable Data Presentation
- Cross-Module Consistency

---

# Design Principles

Sorting should be

Consistent

Stable

Predictable

Performant

Accessible

Sorting must always be performed before pagination unless a specialized retrieval strategy defines otherwise.

---

# Sorting Architecture

```
User

↓

Sort Request

↓

Validation

↓

Query Builder

↓

Database

↓

Result Set
```

---

# Sorting Types

Single Column

Multi Column

Natural Sorting

Alphabetical

Numerical

Date-Based

Priority-Based

Custom Business Sorting

Relevance Sorting

---

# Standard Parameters

```
sort
```

Examples

```
?sort=name

?sort=-createdAt

?sort=status,-createdAt,name
```

---

# Default Direction

Ascending

Unless explicitly configured otherwise.

A leading `-` before a field indicates descending order. The separate
`direction` parameter is superseded and shall not be used in new contracts.

---

# Supported Directions

Ascending

Descending

---

# Stable Sorting

When multiple records have identical values, a secondary sort must be applied.

Default fallback

```
createdAt

↓

id
```

This guarantees deterministic ordering.

---

# Multi-Column Sorting

Example

```
status ASC

createdAt DESC

name ASC
```

---

# Natural Sorting

Supports

Item 1

Item 2

Item 10

Instead of

Item 1

Item 10

Item 2

---

# Alphabetical Sorting

Supports

Localized comparison

Case insensitive

Accent insensitive (where supported)

Reference

Localization.md

---

# Numeric Sorting

Supports

Integer

Decimal

Currency

Measurement Values

Reference

Currency.md

Measurement_System.md

---

# Date Sorting

Supports

Created Date

Modified Date

Transaction Date

Approval Date

Production Date

Always sort using UTC internally.

---

# Status Sorting

Business-defined ordering.

Example

Draft

Pending

Approved

Released

Completed

Archived

Cancelled

Status order is configurable per workflow.

---

# Priority Sorting

Example

Critical

High

Normal

Low

---

# Relevance Sorting

Used for

Global Search

AI Search

Document Search

Knowledge Base

Reference

Search_Filtering.md

AI_Copilot.md

---

# Dashboard Sorting

Supports

Top N

Bottom N

Highest KPI

Lowest KPI

Most Recent

Most Active

---

# Mobile

Supports

Default Sort

Remember Last Sort

Quick Sort

Reference

Navigation.md

---

# API

Example

```
GET /materials?sort=name

GET /production-orders?sort=-createdAt

GET /inventory?sort=status,name
```

---

# Reports

Supports

Multiple Sort Levels

Grouped Sorting

Summary Sorting

Export Consistency

Reference

Reports.md

---

# User Preferences

Users may save

Default Sort

Sort Direction

Multi-Column Sort

Reference

Search_Filtering.md

---

# Database Optimization

Supports

Indexed Columns

Composite Indexes

Seek Queries

Server-side Sorting

Sorting should never occur in application memory unless unavoidable.

---

# Performance

Supports

Server-side Sorting

Pagination Integration

Caching

Indexed Queries

Reference

Performance.md

Caching.md

Pagination.md

---

# Security

Sorting must never expose unauthorized records.

Sorting is applied after authorization filters.

Reference

Permission_Model.md

---

# Monitoring

Track

Most Used Sorts

Slow Sort Queries

Average Sort Time

Large Result Sets

Reference

Monitoring.md

---

# Accessibility

Supports

Keyboard Sorting

Screen Reader Announcements

Visible Sort Indicators

Consistent Icons

---

# Example

Entity

Material

Sort

Category ASC

Material Code ASC

Name ASC

---

# Best Practices

✓ Sort on the server.

✓ Use indexed fields.

✓ Keep sorting deterministic.

✓ Remember user preferences.

✓ Apply sorting before pagination.

✓ Provide visible sort indicators.

---

# Do

✓ Use stable sorting

✓ Support multiple sort fields

✓ Preserve user sort settings

✓ Optimize database indexes

✓ Validate sort parameters

---

# Don't

✗ Sort large datasets in memory

✗ Use inconsistent default ordering

✗ Allow sorting on unauthorized fields

✗ Ignore locale when sorting text

✗ Break pagination consistency

---

# Acceptance Criteria

Sorting behaves consistently across all modules.

Multi-column sorting is supported.

Sorting integrates with filtering and pagination.

Performance targets are achieved.

Accessibility requirements are satisfied.

User preferences are respected.

---

# Related Documents

Search_Filtering.md

Pagination.md

Performance.md

Caching.md

Localization.md

Measurement_System.md

Currency.md

Permission_Model.md

API_Standards.md

Reports.md
