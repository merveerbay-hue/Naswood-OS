# Header

**Module:** Design System

**Category:** Layout

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Header is the global navigation and command bar of Naswood OS.

It provides consistent access to navigation, search, notifications, user settings and global actions while remaining visible throughout the application.

Every page within Naswood OS must use the official Header component.

---

# Objectives

- Consistent Navigation
- Fast Access
- Global Search
- User Productivity
- Responsive Design
- Accessibility Compliance

---

# Design Principles

The Header should be

- Minimal
- Predictable
- Persistent
- Responsive
- Non-intrusive

Business content always has priority over navigation.

---

# Standard Layout

```
┌────────────────────────────────────────────────────────────────────────────┐
│ Logo │ Sidebar │ Search │ Quick Actions │ Notifications │ User │ Theme │
└────────────────────────────────────────────────────────────────────────────┘
```

---

# Component Structure

```
Header

├── Logo

├── Sidebar Toggle

├── Breadcrumb (Optional)

├── Global Search

├── Quick Actions

├── AI Assistant

├── Notification Center

├── Help

├── Theme Switch

├── User Menu

└── Environment Badge
```

---

# Height

64 px

Reference

Design_Tokens.md

---

# Position

Fixed

Top

Full Width

Always Visible

---

# Logo

Contains

Naswood Logo

Environment Badge (Optional)

Click

Returns to Dashboard

---

# Sidebar Toggle

Desktop

Collapse Sidebar

Tablet

Collapse Sidebar

Mobile

Open Drawer

---

# Global Search

Reference

Search.md

Supports

Ctrl + K

Recent Searches

AI Search

Global Search

---

# Quick Actions

Examples

Create Material

New Purchase Order

Receive Goods

Start Production

Create Customer

Upload File

Quick actions are configurable by role.

---

# AI Assistant

Always available.

Provides

AI Chat

Suggestions

Knowledge Search

Workflow Assistance

---

# Notification Center

Displays

Unread Count

Recent Notifications

Workflow Alerts

Machine Alarms

AI Suggestions

Reference

Notifications.md

---

# Help

Contains

Documentation

Keyboard Shortcuts

Support

Release Notes

---

# Theme Switch

Supports

Light

Dark

System

Corporate

Reference

Theme.md

---

# User Menu

Displays

Avatar

Name

Role

Department

Contains

My Profile

Preferences

Settings

Activity Log

Logout

---

# Environment Badge

Optional

Values

Development

Test

Staging

Production

Development environments should be clearly distinguishable.

---

# Responsive Behaviour

## Desktop

Full Header

Persistent Search

Expanded Navigation

---

## Tablet

Compact Search

Collapsed Sidebar

Reduced Actions

---

## Mobile

Drawer Navigation

Search Overlay

Bottom Actions (Optional)

---

# Search Shortcut

Ctrl + K

Opens Global Search.

---

# Notification Shortcut

Ctrl + Shift + N

---

# Accessibility

Supports

Keyboard Navigation

ARIA Labels

Screen Readers

Focus Indicators

High Contrast

Minimum Touch Target

44 × 44 px

---

# Performance

Lazy load notifications.

Debounce search.

Cache user profile.

Avoid unnecessary re-rendering.

---

# Security

Respect user permissions.

Display only authorized actions.

Mask sensitive information.

Support secure logout.

---

# React Structure

```tsx
<AppHeader>

    <Logo />

    <SidebarToggle />

    <GlobalSearch />

    <QuickActions />

    <AIAssistant />

    <NotificationBell />

    <HelpMenu />

    <ThemeSwitcher />

    <UserMenu />

</AppHeader>
```

---

# User Preferences

Remember

Header Density

Theme

Pinned Actions

Recent Searches

Notification Preferences

---

# Best Practices

✓ Keep header compact.

✓ Prioritize search.

✓ Keep actions role-based.

✓ Show notification count.

✓ Provide quick access to AI.

---

# Do

✓ Fixed header

✓ Global search

✓ Notification bell

✓ User menu

✓ Theme switch

✓ Quick actions

---

# Don't

✗ Overload the header

✗ Duplicate sidebar navigation

✗ Display unauthorized actions

✗ Use multiple header layouts

✗ Hide global search

---

# Acceptance Criteria

Header follows the official layout.

Global Search functions correctly.

Notifications display real-time updates.

User menu reflects permissions.

Responsive layout works.

Accessibility complies with WCAG 2.1 AA.

Performance remains smooth.

---

# Related Documents

Application_Shell.md

Sidebar.md

Navigation.md

Search.md

Notifications.md

Theme.md

User_Management.md

Accessibility.md

Design_Tokens.md
