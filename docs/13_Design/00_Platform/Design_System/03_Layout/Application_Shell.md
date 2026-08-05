# Application Shell

**Module:** Design System

**Category:** Layout

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Application Shell defines the overall layout framework used throughout Naswood OS.

It provides a consistent structure for navigation, content presentation, user interaction and responsive behavior across every module.

Every page within Naswood OS must use the official Application Shell.

---

# Objectives

- Consistent Layout
- Predictable Navigation
- Responsive Architecture
- Enterprise User Experience
- Reusable Structure
- Accessibility Compliance

---

# Design Principles

The Application Shell should be

- Stable
- Consistent
- Responsive
- Accessible
- Minimal

Navigation remains fixed while business content changes.

---

# Standard Layout

```
┌─────────────────────────────────────────────────────────────┐
│ Header                                                      │
├───────────────┬─────────────────────────────────────────────┤
│ Sidebar       │ Breadcrumb                                 │
│               ├─────────────────────────────────────────────┤
│               │ Toolbar                                    │
│               ├─────────────────────────────────────────────┤
│               │ Filters (Optional)                         │
│               ├─────────────────────────────────────────────┤
│               │                                             │
│               │ Main Content                               │
│               │                                             │
│               ├─────────────────────────────────────────────┤
│               │ Footer (Optional)                          │
└───────────────┴─────────────────────────────────────────────┘
```

---

# Layout Components

Application Shell consists of

Header

Sidebar

Breadcrumb

Toolbar

Filter Area

Main Workspace

Notification Layer

Dialog Layer

Footer

---

# Header

Position

Fixed

Height

64 px

Contains

Logo

Search

Notifications

Quick Actions

User Menu

Theme Switch

---

# Sidebar

Position

Fixed

Width

280 px

Collapsed Width

72 px

Contains

Modules

Favorites

Recent

Administration

---

# Breadcrumb

Displays

Current Location

Navigation Hierarchy

Quick Navigation

---

# Toolbar

Contains

Page Title

Primary Actions

Secondary Actions

Export

Import

Refresh

Search

---

# Filter Area

Optional

Contains

Quick Filters

Advanced Filters

Saved Filters

Date Range

Search

---

# Main Workspace

Contains

Forms

Data Grids

Cards

Dashboards

Reports

Charts

AI Components

---

# Footer

Optional

Displays

Version

Environment

Copyright

System Status

---

# Overlay Layers

Dialogs

Notifications

Context Menus

Tooltips

Loading Screen

Command Palette

---

# Layout Width

Maximum Content Width

1600 px

Centered Layout

Enabled

---

# Responsive Behaviour

## Desktop

Fixed Sidebar

Full Toolbar

Persistent Navigation

---

## Tablet

Collapsible Sidebar

Adaptive Toolbar

Responsive Content

---

## Mobile

Drawer Navigation

Compact Header

Fullscreen Dialogs

Bottom Actions

---

# Navigation Flow

Login

↓

Dashboard

↓

Module

↓

Workspace

↓

Detail

↓

Dialog

---

# Page Template

Every page follows

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

# Scroll Behaviour

Header

Fixed

Sidebar

Independent Scroll

Content

Scrollable

Dialogs

Independent Scroll

---

# Keyboard Navigation

Supports

Tab

Shift + Tab

Escape

Enter

Arrow Keys

Shortcut Keys

---

# Accessibility

Supports

Screen Readers

Keyboard Navigation

High Contrast

Reduced Motion

Focus Indicators

WCAG 2.1 AA

---

# Theme Support

Light

Dark

System

Corporate

Only design tokens change.

---

# Loading States

Page Loading

Section Loading

Skeleton

Progress Indicator

---

# Error States

Page Error

Module Error

Network Error

Permission Error

Empty State

---

# Performance

Lazy Loading

Route Based Code Splitting

Virtual Rendering

Memoization

Asset Optimization

---

# Security

Permission Based Navigation

Protected Routes

Session Validation

Sensitive Data Masking

Audit Logging

---

# React Structure

```tsx
<AppShell>

    <Header />

    <Sidebar />

    <MainLayout>

        <Breadcrumb />

        <Toolbar />

        <PageContent />

    </MainLayout>

    <NotificationCenter />

    <DialogProvider />

</AppShell>
```

---

# Layout Tokens

Header Height

64 px

Sidebar Width

280 px

Collapsed Sidebar

72 px

Content Padding

24 px

Grid Gap

24 px

Reference

Design_Tokens.md

---

# Best Practices

✓ Keep navigation persistent.

✓ Keep layouts consistent.

✓ Separate navigation from content.

✓ Support responsive behavior.

✓ Minimize page transitions.

✓ Use reusable layout components.

---

# Do

✓ One application shell

✓ Shared navigation

✓ Fixed header

✓ Responsive sidebar

✓ Consistent workspace

---

# Don't

✗ Create different layouts for modules

✗ Duplicate navigation

✗ Hardcode spacing

✗ Change shell structure

✗ Mix page layouts

---

# Acceptance Criteria

All modules use the official Application Shell.

Header and Sidebar remain consistent.

Responsive layout functions correctly.

Accessibility complies with WCAG 2.1 AA.

Navigation behaves consistently.

Layout tokens are respected.

Performance remains acceptable.

---

# Related Documents

Header.md

Sidebar.md

Navigation.md

Dashboard.md

Responsive.md

Grid_System.md

Breakpoints.md

Design_Tokens.md

Accessibility.md
