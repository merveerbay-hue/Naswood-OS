# Pagination

**Module:** Shared

**Category:** Data Retrieval

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Pagination standard defines how large collections of data are retrieved, filtered, sorted and presented throughout Naswood OS.

The objective is to provide consistent, scalable and performant access to business data across web, mobile, APIs and reporting services.

All list-based endpoints and user interfaces must comply with this standard.

---

# Objectives

- Consistent Data Retrieval
- High Performance
- Predictable API Design
- Scalable User Experience
- Efficient Database Queries
- Support Large Datasets

---

# Design Principles

Pagination should be

Consistent

Predictable

Fast

Configurable

Accessible

Pagination must never require loading the full dataset into memory.

---

# Pagination Strategies

Supports

Offset Pagination

Cursor Pagination

Keyset Pagination

Infinite Scroll (where appropriate)

Virtual Scrolling

Each strategy should be selected according to the data characteristics.

---

# Default Strategy

Offset Pagination

Used for

Business Lists

Master Data

Reports

Reference Data

---

# Cursor Pagination

Recommended for

Audit Logs

Notifications

Machine Events

Telemetry

AI Conversations

Very Large Datasets

---

# Keyset Pagination

Recommended for

Time-Series Data

Event Streams

Production History

Sensor Data

---

# Standard Request

```
GET /materials?page=1&pageSize=25
```

---

# Standard Response

```json
{
  "success": true,
  "data": [],
  "message": null,
  "metadata": {
    "pagination": {
      "page": 1,
      "pageSize": 25,
      "totalItems": 1240,
      "totalPages": 50,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

---

# Cursor Response

```json
{
  "success": true,
  "data": [],
  "message": null,
  "metadata": {
    "pagination": {
      "nextCursor": "eyJpZCI6MTI0NTZ9",
      "hasNext": true
    }
  }
}
```

---

# Default Page Size

25

---

# Allowed Page Sizes

10

25

50

100

250

Maximum values are configurable by endpoint.

---

# Sorting

Supports

Single Column

Multi Column

Ascending

Descending

Stable Ordering

Examples

```
?sort=name

?sort=-createdAt

?sort=status,name
```

---

# Filtering

Supports

Equality

Range

Date Range

Boolean

Multiple Values

Examples

```
?status=Active

?warehouse=FG01

?category=Thermowood

?createdAfter=2026-01-01
```

---

# Search

Supports

Full Text

Business Code

Barcode

QR

Tags

AI Search

---

# Large Dataset Handling

Supports

Virtualization

Incremental Loading

Lazy Loading

Server-side Filtering

Server-side Sorting

---

# Infinite Scroll

Recommended for

Notifications

AI Chat

Audit Timeline

Machine Logs

Not recommended for

Master Data

Financial Reports

Approval Lists

---

# Mobile

Supports

Infinite Scroll

Pull to Refresh

Load More

Offline Cache

Reference

Navigation.md

Offline_UI.md

---

# API Standards

Pagination parameters

```
page

pageSize

sort

filter

search
```

Reference

API_Standards.md

---

# Database

Supports

Indexed Queries

Efficient COUNT Queries

Seek Pagination

Read Replicas

Query Optimization

---

# Caching

Supports

Page Cache

Metadata Cache

Search Cache

Reference

Caching.md

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

Page Announcements

Focus Management

---

# Performance Targets

Average Response

<300 ms

Maximum Page Size

250

Database Query

<150 ms

---

# UI Behaviour

Displays

Current Page

Total Pages

Item Count

Page Size Selector

Next

Previous

First

Last

---

# State Preservation

The application should preserve

Current Page

Filters

Sorting

Search

Selected Rows

Scroll Position (where appropriate)

---

# Export

Export operations should not depend on the current page.

Exports must support the full filtered dataset through asynchronous processing when necessary.

Reference

Reports.md

---

# Security

Pagination must respect

Role Permissions

Department Permissions

Record Visibility

Reference

Authorization.md

---

# Monitoring

Track

Average Query Time

Average Page Size

Response Time

Cache Hit Rate

Timeouts

Slow Queries

Reference

Monitoring.md

---

# Example

```
GET /production-orders?page=3&pageSize=50&sort=-createdAt&status=Released
```

---

# Best Practices

✓ Apply pagination on the server.

✓ Filter before paginating.

✓ Sort consistently.

✓ Use indexes.

✓ Preserve user state.

✓ Use cursor pagination for event streams.

---

# Do

✓ Support configurable page sizes

✓ Return pagination metadata

✓ Optimize database queries

✓ Preserve filters

✓ Limit maximum page size

---

# Don't

✗ Load entire tables

✗ Paginate on the client

✗ Return inconsistent ordering

✗ Ignore indexing

✗ Reset user filters unexpectedly

---

# Acceptance Criteria

Pagination is implemented consistently.

Large datasets remain responsive.

Sorting and filtering are stable.

Performance targets are achieved.

Accessibility requirements are satisfied.

State is preserved across navigation.

---

# Related Documents

API_Standards.md

Caching.md

Search.md

Data_Grid.md

Tables.md

Lists.md

Monitoring.md

Authorization.md

Architecture.md
