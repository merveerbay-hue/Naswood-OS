# Sidebar

**Module:** Design System

**Category:** Layout

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Sidebar provides the primary navigation for Naswood OS.

It enables users to access business modules, navigate between workspaces and quickly switch contexts while maintaining a consistent experience across the platform.

The Sidebar remains the primary navigation component on desktop and industrial devices.

---

# Objectives

- Consistent Navigation
- Fast Module Access
- Enterprise Scalability
- Responsive Behaviour
- Accessibility Compliance
- User Productivity

---

# Design Principles

The Sidebar should be

- Persistent
- Predictable
- Minimal
- Hierarchical
- Accessible

Navigation should reduce clicks while keeping the interface uncluttered.

---

# Standard Structure

```
Sidebar

├── Logo

├── Module Navigation

├── Favorites

├── Recent Items

├── Divider

├── Administration

└── Collapse Button
```

---

# Sidebar Layout

Top

Logo

↓

Primary Modules

↓

Favorites

↓

Recent Items

↓

Administration

↓

Collapse Button

---

# Width

Expanded

280 px

Collapsed

72 px

Reference

Design_Tokens.md

---

# Position

Fixed

Left Side

Full Height

Independent Scroll

---

# Logo

Displays

Naswood Logo

Environment Badge (Optional)

Click

Navigate to Dashboard

---

# Primary Modules

Dashboard

Master Data

Inventory

Purchasing

Sales

Production

Quality

Maintenance

Finance

Analytics

AI

Digital Twin

Administration

---

# Module Structure

Example

```
Inventory

▼

Materials

Warehouses

Locations

Inventory

Batches

Movements
```

Only one module should be expanded by default.

---

# Favorites

Users may pin

Pages

Reports

Dashboards

Searches

Frequently used items appear above Recent Items.

---

# Recent Items

Displays

Recently Opened Records

Recently Visited Pages

Maximum

10 Items

---

# Administration

Contains

Users

Roles

Permissions

Settings

Audit Logs

System Health

Visible only with appropriate permissions.

---

# Collapse Behaviour

Expanded

280 px

↓

Collapsed

72 px

Icons remain visible.

Tooltips display labels.

---

# Expand Behaviour

Click Module

Expand

↓

Display Children

↓

Navigate

---

# Active State

Current module

Highlighted

Current page

Highlighted

Parent expanded automatically.

---

# Icons

Reference

Icons.md

Every navigation item includes

Icon

Label

Optional Badge

---

# Badges

Supported

Unread Notifications

Pending Approvals

Alerts

Warnings

Task Count

---

# Search

Sidebar Search

Optional

Searches

Modules

Pages

Favorites

---

# Responsive Behaviour

## Desktop

Persistent Sidebar

Expanded by default

---

## Tablet

Collapsible Sidebar

Overlay optional

---

## Mobile

Drawer Navigation

Hidden by default

Swipe supported

---

# Scrolling

Sidebar scrolls independently.

Header remains fixed.

Content remains independent.

---

# Keyboard Navigation

Supports

Tab

Shift + Tab

Arrow Keys

Enter

Escape

Home

End

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

Focus Indicators

High Contrast

Touch Targets

44 × 44 px minimum

---

# Permissions

Visibility depends on

User

Role

Permission

Organization

Module License

Unauthorized items are never displayed.

---

# Performance

Lazy load submenu items.

Cache navigation state.

Avoid rendering hidden modules.

Optimize icon loading.

---

# User Preferences

Remember

Collapsed State

Expanded Modules

Favorites

Recent Items

Default Module

---

# React Structure

```tsx
<Sidebar>

    <SidebarLogo />

    <SidebarModules />

    <SidebarFavorites />

    <SidebarRecent />

    <SidebarAdministration />

    <SidebarCollapse />

</Sidebar>
```

---

# Example Navigation

```
Dashboard

Inventory
    Materials
    Warehouses
    Locations
    Inventory
    Batches

Purchasing
    Suppliers
    Purchase Requests
    Purchase Orders

Production
    Production Orders
    Work Orders
    Routing

Quality
    Inspections
    NCR
    CAPA
```

---

# Best Practices

✓ Keep module names short.

✓ Show only authorized modules.

✓ Keep icons consistent.

✓ Remember user preferences.

✓ Support keyboard navigation.

✓ Allow collapsing.

---

# Do

✓ Use official icons

✓ Highlight active page

✓ Show tooltips when collapsed

✓ Keep hierarchy shallow

✓ Support favorites

---

# Don't

✗ Deep nested menus

✗ Duplicate navigation

✗ Show unauthorized modules

✗ Mix different icon styles

✗ Use scrolling inside menu groups

---

# Acceptance Criteria

Sidebar follows the official layout.

Collapse and expand behave consistently.

Active navigation is always visible.

Permissions control visibility.

Responsive behaviour functions correctly.

Accessibility complies with WCAG 2.1 AA.

User preferences persist across sessions.

---

# Related Documents

Application_Shell.md

Navigation.md

Header.md

Dashboard.md

Icons.md

Search.md

Accessibility.md

Design_Tokens.md
