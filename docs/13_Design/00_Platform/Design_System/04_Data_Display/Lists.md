# Lists

**Module:** Design System

**Category:** Data Display

**Version:** 1.0

**Status:** Approved

---

# Purpose

The List component provides a standardized way to display collections of related items in a simple, readable and lightweight format.

Lists are optimized for navigation, summaries and activity feeds rather than structured data management.

Use Lists when users need to browse or select information quickly.

---

# Objectives

- Simple Information Display
- Fast Navigation
- Consistent Presentation
- Responsive Design
- Accessibility Compliance
- Reusable Component

---

# Design Principles

Lists should be

- Simple
- Readable
- Lightweight
- Consistent
- Action Oriented

Each list item should represent a single business object.

---

# Typical Usage

Lists are used for

Recent Activities

Notifications

Tasks

Approvals

Favorites

Recent Records

History

Timeline

Search Results

AI Suggestions

Documents

Attachments

Comments

---

# Standard Structure

```
List

├── Header (Optional)

├── Filters (Optional)

├── Items

└── Footer (Optional)
```

---

# List Item Structure

```
List Item

├── Leading Icon / Avatar

├── Title

├── Subtitle

├── Metadata

├── Status

├── Actions

└── Divider
```

---

# Item Elements

Supported

Icon

Avatar

Image

Title

Subtitle

Description

Badge

Status

Timestamp

Tags

Quick Actions

Progress

---

# List Types

Simple List

Navigation List

Action List

Selection List

Activity Feed

Notification List

Timeline List

Document List

Search Result List

AI Recommendation List

---

# Navigation List

Used in

Sidebar

Favorites

Quick Access

Settings

Administration

---

# Activity List

Displays

Inventory Movements

Production Events

Audit Events

Workflow Changes

User Actions

---

# Notification List

Displays

Unread Notifications

Workflow Alerts

Machine Alarms

AI Suggestions

Reference

Notifications.md

---

# Search Result List

Displays

Matching Records

Recent Searches

Suggested Results

Grouped Results

---

# Document List

Displays

File Name

Type

Size

Owner

Date

Actions

---

# Timeline List

Displays

Chronological Events

Workflow Steps

Production Events

Approval History

---

# Selection List

Supports

Single Selection

Multiple Selection

Checkbox Selection

Radio Selection

---

# Item States

Default

Hover

Focused

Selected

Disabled

Loading

Error

---

# Item Actions

Open

Edit

Delete

Pin

Favorite

Download

Preview

Share

Archive

---

# Empty State

Illustration

Title

Description

Primary Action

---

# Loading State

Skeleton Items

Spinner

Progress Indicator

---

# Error State

Error Message

Retry Button

Help Link

---

# Grouping

Lists may be grouped by

Date

Status

Category

Priority

Owner

Department

---

# Sorting

Supported

Alphabetical

Date

Priority

Status

Manual

---

# Filtering

Optional

Status

Category

Date

Owner

Tags

Priority

---

# Pagination

Supported

Page Size

25

50

100

Or Infinite Scroll

---

# Responsive Behaviour

Desktop

Full List

Tablet

Compact List

Mobile

Touch Optimized List

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

Infinite Scroll

Virtual Rendering

Memoization

Cached Queries

---

# Security

Lists respect

Role Permissions

Module Permissions

Record Permissions

Sensitive Data Masking

---

# React Structure

```tsx
<List>

    <ListHeader />

    <ListItem />

    <ListItem />

    <ListFooter />

</List>
```

---

# Example Components

ActivityList

NotificationList

TaskList

DocumentList

TimelineList

ApprovalList

SearchResultList

AIRecommendationList

FavoriteList

RecentItemList

---

# Best Practices

✓ Keep titles concise.

✓ Show relevant metadata.

✓ Use icons consistently.

✓ Group related items.

✓ Provide contextual actions.

✓ Keep scrolling smooth.

---

# Do

✓ Use for navigation

✓ Use for activity feeds

✓ Show timestamps

✓ Display status badges

✓ Support keyboard navigation

---

# Don't

✗ Use for large datasets

✗ Replace Data Grid

✗ Display excessive details

✗ Mix unrelated item types

✗ Overload list items

---

# Acceptance Criteria

Lists use the official component.

Items follow the standard layout.

Responsive behaviour functions correctly.

Accessibility complies with WCAG 2.1 AA.

Loading and empty states are implemented.

Permissions control item visibility.

Performance remains acceptable.

---

# Related Documents

Cards.md

Data_Grid.md

Tables.md

Detail_View.md

Notifications.md

Search.md

Typography.md

Spacing.md

Design_Tokens.md

Accessibility.md
