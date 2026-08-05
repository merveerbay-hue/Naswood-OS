# Detail View

**Module:** Design System

**Category:** Data Display

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Detail View provides a standardized layout for displaying complete information about a single business entity.

It enables users to review records, monitor status, access related information and perform contextual actions without unnecessary navigation.

All modules must use the official Detail View layout.

---

# Objectives

- Consistent Record Presentation
- Improved Readability
- Faster Decision Making
- Contextual Actions
- Responsive Design
- Accessibility Compliance

---

# Design Principles

A Detail View should be

- Informative
- Structured
- Readable
- Contextual
- Consistent

Information should be grouped logically.

---

# Supported Entities

Material

Warehouse

Location

Inventory

Batch

Supplier

Customer

Purchase Order

Sales Order

Production Order

Machine

Work Center

Quality Inspection

Maintenance Work Order

Invoice

Employee

---

# Standard Layout

```
Breadcrumb

↓

Header

↓

Summary Cards

↓

Tabs

↓

Content

↓

Related Records

↓

History

↓

Audit Information
```

---

# Header

Contains

Title

Record Number

Status

Reference Code

Created Date

Modified Date

Owner

Primary Actions

---

# Summary Section

Displays

Key Information

Current Status

KPIs

Statistics

Badges

Quick Actions

---

# Primary Actions

Edit

Delete

Print

Export

Duplicate

Archive

Share

Custom Actions

---

# Tabs

Examples

General

Inventory

Transactions

Production

Quality

Maintenance

Attachments

Notes

History

Audit

Tabs should remain consistent across modules.

---

# General Section

Displays

Basic Information

Description

Classification

Status

References

Metadata

---

# Related Records

May include

Purchase Orders

Sales Orders

Inventory

Production Orders

Quality Records

Maintenance Records

Invoices

---

# Attachments

Displays

Documents

Images

CAD Drawings

Certificates

Reports

Reference

File_Upload.md

---

# Notes

Supports

Plain Text

Rich Text

Mentions

Internal Notes

---

# History

Displays

Changes

Workflow Events

User Actions

Status Changes

---

# Audit Information

Displays

Created By

Created Date

Modified By

Modified Date

Version

Reference

Audit_Log.md

---

# Status Display

Supported

Draft

Active

Pending

Approved

Completed

Cancelled

Archived

Status uses semantic colors.

---

# Related Actions

View Related Records

Create Related Record

Open Workflow

Generate Report

Export PDF

---

# Layout Variants

Standard Detail

Split View

Tabbed Detail

Fullscreen Detail

Drawer Detail

Read Only Detail

---

# Split View

```
List

│

├──────────────┐
│              │
│              │
│ Detail View  │
│              │
└──────────────┘
```

Used for

Inventory

Purchasing

Sales

Production

---

# Empty State

Display

Illustration

Message

Suggested Actions

---

# Loading State

Skeleton Header

Skeleton Cards

Skeleton Tabs

Loading Indicator

---

# Error State

Error Banner

Retry

Diagnostic Message

---

# Responsive Behaviour

Desktop

Full Layout

Tablet

Adaptive Layout

Mobile

Single Column

Collapsible Sections

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

Lazy Load Tabs

Lazy Load Related Records

Cache Record Data

Virtualize Large Lists

---

# Security

Respect

Role Permissions

Module Permissions

Field-Level Security

Sensitive Data Masking

Audit Logging

---

# React Structure

```tsx
<DetailView>

    <DetailHeader />

    <SummaryCards />

    <DetailTabs>

        <GeneralTab />

        <AttachmentsTab />

        <HistoryTab />

    </DetailTabs>

</DetailView>
```

---

# User Preferences

Remember

Selected Tab

Expanded Sections

Visible Panels

Density

---

# Best Practices

✓ Show important information first.

✓ Group related fields.

✓ Use summary cards.

✓ Keep actions contextual.

✓ Load heavy sections lazily.

---

# Do

✓ Display record status

✓ Provide quick actions

✓ Show related records

✓ Display audit history

✓ Keep layout consistent

---

# Don't

✗ Mix unrelated information

✗ Hide important actions

✗ Load every tab immediately

✗ Create different layouts for each module

✗ Duplicate information

---

# Acceptance Criteria

Detail View follows the official layout.

Summary information is displayed consistently.

Tabs load correctly.

Related records are accessible.

Audit information is available.

Responsive behaviour functions correctly.

Accessibility complies with WCAG 2.1 AA.

Performance remains acceptable.

---

# Related Documents

Workspace.md

Forms.md

Cards.md

Data_Grid.md

Tables.md

File_Upload.md

Notifications.md

Audit_Log.md

Accessibility.md

Design_Tokens.md
