# Navigation

**Module:** Design System

**Category:** Layout

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Navigation system defines how users move throughout Naswood OS.

It establishes a consistent navigation hierarchy across all modules while minimizing cognitive load and maximizing productivity.

Navigation should allow users to reach any business function with the fewest possible interactions.

---

# Objectives

- Consistent Navigation
- Enterprise Information Architecture
- Faster User Workflows
- Predictable User Experience
- Responsive Navigation
- Accessibility Compliance

---

# Design Principles

Navigation should be

- Consistent
- Predictable
- Hierarchical
- Minimal
- Accessible

Business workflows should determine navigation structure.

---

# Navigation Hierarchy

```
Platform

↓

Module

↓

Workspace

↓

Entity

↓

Detail

↓

Action
```

---

# Navigation Levels

Level 1

Platform

Examples

Dashboard

Inventory

Production

Purchasing

Sales

Finance

Quality

Maintenance

Analytics

AI

Settings

---

Level 2

Workspace

Examples

Materials

Warehouses

Customers

Suppliers

Production Orders

Purchase Orders

Invoices

---

Level 3

Detail

Examples

Material Detail

Purchase Order

Production Order

Customer

Supplier

---

Level 4

Action

Examples

Create

Edit

Delete

Approve

Print

Export

---

# Primary Navigation

Primary navigation is provided by the Sidebar.

Reference

Sidebar.md

---

# Secondary Navigation

Provided by

Tabs

Toolbar

Page Menu

Breadcrumb

---

# Breadcrumb

Example

```
Inventory

>

Materials

>

Wood Panels

>

Material Details
```

Breadcrumbs should always reflect the current location.

---

# Navigation Components

Sidebar

Header

Breadcrumb

Tabs

Toolbar

Context Menu

Quick Actions

Global Search

Command Palette

---

# Module Navigation

Every module follows

Dashboard

↓

List

↓

Details

↓

Edit

↓

History

---

# Page Navigation

Every page follows

List

↓

Details

↓

Actions

Users should always know where they are.

---

# Tabs

Used inside pages.

Examples

General

Inventory

Quality

Attachments

History

Notes

Settings

---

# Quick Navigation

Supports

Favorites

Pinned Items

Recent Records

Frequently Used

---

# Global Navigation

Available from

Header

Supports

Global Search

AI Assistant

Notifications

User Menu

Quick Create

---

# Context Navigation

Right-click menus.

Available in

Data Grid

Tree View

Documents

Dashboard Widgets

---

# Command Palette

Shortcut

Ctrl + K

Supports

Navigate

Search

Commands

Create

Open Recent

AI Search

---

# Keyboard Navigation

Supports

Tab

Shift + Tab

Arrow Keys

Enter

Escape

Ctrl + K

Alt + Left

Alt + Right

---

# Responsive Behaviour

## Desktop

Persistent Sidebar

Breadcrumb

Toolbar

---

## Tablet

Collapsible Sidebar

Compact Navigation

---

## Mobile

Drawer Navigation

Bottom Actions

Fullscreen Search

---

# Navigation Patterns

List

↓

Detail

↓

Edit

↓

Save

↓

Return to List

---

Dashboard

↓

Widget

↓

Details

↓

Action

---

Notification

↓

Open Record

↓

Complete Workflow

---

# Navigation States

Default

Active

Hover

Focused

Disabled

Collapsed

Expanded

---

# Deep Linking

Every page should have a unique URL.

Examples

```
/inventory/materials

/inventory/materials/145

/production/orders

/production/orders/PO-1023

/purchasing/orders
```

---

# Route Structure

```
Module

↓

Feature

↓

Entity

↓

Action
```

Example

```
/inventory/materials/create

/inventory/materials/145/edit
```

---

# Navigation Permissions

Navigation visibility depends on

User

Role

Permission

Organization

Module License

Unauthorized pages should never appear.

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

Visible Focus

Skip Navigation Link

High Contrast

---

# Performance

Lazy load routes.

Code split by module.

Prefetch frequently used pages.

Cache navigation configuration.

---

# Security

Navigation respects

Role Permissions

Module Permissions

Tenant Isolation

Environment

Audit Logging

---

# React Structure

```tsx
<AppShell>

    <SidebarNavigation />

    <HeaderNavigation />

    <Breadcrumb />

    <PageRoutes />

</AppShell>
```

---

# User Preferences

Users may configure

Favorites

Pinned Modules

Recent Items

Navigation Density

Collapsed Sidebar

Default Landing Page

---

# Best Practices

✓ Keep navigation shallow.

✓ Use descriptive labels.

✓ Keep module names consistent.

✓ Provide breadcrumbs.

✓ Remember user preferences.

✓ Keep navigation responsive.

---

# Do

✓ Dashboard first

✓ Logical hierarchy

✓ Persistent navigation

✓ Breadcrumbs

✓ Global Search

✓ Command Palette

---

# Don't

✗ Deep nested menus

✗ Duplicate navigation

✗ Hidden navigation paths

✗ Inconsistent page names

✗ More than three navigation levels

---

# Acceptance Criteria

Navigation hierarchy is consistent.

Users reach any feature within three navigation levels.

Breadcrumbs accurately reflect location.

Deep links function correctly.

Permissions control visibility.

Responsive navigation works.

Accessibility complies with WCAG 2.1 AA.

---

# Related Documents

Application_Shell.md

Sidebar.md

Header.md

Dashboard.md

Search.md

Breadcrumb.md

Accessibility.md

Design_Tokens.md
