# Breakpoints

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Breakpoints standard defines the responsive layout behavior of Naswood OS across desktop, tablet, mobile and industrial touch devices.

Every page, component and module must adapt consistently to different screen sizes while maintaining usability, readability and operational efficiency.

The platform is desktop-first because most users operate the system from office workstations or industrial terminals.

---

# Objectives

- Responsive User Interface
- Desktop First Design
- Consistent Layout
- Touch Screen Support
- Mobile Compatibility
- Large Monitor Support
- Future Scalability

---

# Design Principles

Responsive behavior must be:

- Predictable
- Consistent
- Performant
- Accessible
- Touch Friendly

Layouts should never hide critical business information.

---

# Responsive Strategy

Desktop First

↓

Tablet Adaptation

↓

Mobile Optimization

---

# Standard Breakpoints

| Device | Width |
|---------|------:|
| Mobile | < 640 px |
| Small Tablet | 640–767 px |
| Tablet | 768–1023 px |
| Laptop | 1024–1279 px |
| Desktop | 1280–1535 px |
| Large Desktop | 1536–1919 px |
| Ultra Wide | ≥ 1920 px |

---

# Tailwind Mapping

| Tailwind | Width |
|-----------|------:|
| sm | 640 px |
| md | 768 px |
| lg | 1024 px |
| xl | 1280 px |
| 2xl | 1536 px |

---

# Target Devices

Office PC

Industrial Touch Panel

Laptop

Tablet

Mobile Phone

Ultra Wide Monitor

TV Dashboard

---

# Layout Behaviour

## Mobile

Single column

Overlay sidebar

Stacked cards

Compact tables

Bottom actions

Touch optimized

---

## Tablet

Two-column layout

Collapsible sidebar

Adaptive dashboard

Medium spacing

Touch friendly

---

## Laptop

Full navigation

Sidebar expanded

Standard tables

Dashboard widgets

---

## Desktop

Maximum workspace

Multi-column layout

Persistent sidebar

Full dashboard

Multiple panels

---

## Ultra Wide

Three-column workspace

Extended dashboards

Split views

Multi-panel monitoring

Digital Twin support

---

# Sidebar Behaviour

| Device | Behaviour |
|----------|-----------|
| Mobile | Hidden |
| Tablet | Collapsible |
| Desktop | Expanded |
| Ultra Wide | Fixed |

---

# Header Behaviour

Always visible.

Height remains constant.

Search collapses on mobile.

Notifications become icon-only on small screens.

---

# Dashboard Behaviour

## Mobile

1 widget per row

---

## Tablet

2 widgets per row

---

## Desktop

3–4 widgets per row

---

## Ultra Wide

4–6 widgets per row

---

# Tables

## Mobile

Card View preferred

Horizontal scroll allowed

Hide low-priority columns

---

## Tablet

Compact Data Grid

---

## Desktop

Full Data Grid

---

## Ultra Wide

Pinned columns

Advanced filters

Multiple panels

---

# Forms

Mobile

Single column

---

Tablet

Single column

---

Desktop

Two columns

---

Large Desktop

Two or three columns

---

# Dialogs

Mobile

Fullscreen

---

Tablet

Large Modal

---

Desktop

Centered Dialog

---

Ultra Wide

Centered Dialog

Maximum width

900 px

---

# Cards

Cards expand automatically.

Minimum width

320 px

Preferred width

400–500 px

---

# Charts

Mobile

Single chart

---

Tablet

Two charts

---

Desktop

Multiple charts

---

Ultra Wide

Dashboard wall

---

# Touch Support

Minimum touch target

44 × 44 px

Large buttons

Large spacing

Gesture support

---

# Responsive Images

Use responsive images.

Avoid fixed sizes.

Maintain aspect ratio.

---

# Navigation

Desktop

Persistent sidebar

---

Tablet

Collapsible sidebar

---

Mobile

Drawer navigation

---

# Performance

Load only required components.

Lazy loading.

Virtual scrolling.

Image optimization.

---

# CSS Guidelines

Use Flexbox.

Use CSS Grid.

Avoid fixed widths.

Use percentage layouts.

Prefer min/max width.

---

# Responsive Tokens

Container Width

Content Width

Sidebar Width

Header Height

Card Width

Grid Columns

Spacing Scale

---

# Layout Widths

| Layout | Width |
|---------|-------|
| Content Max Width | 1600 px |
| Sidebar Width | 280 px |
| Collapsed Sidebar | 72 px |
| Header Height | 64 px |
| Footer Height | 40 px |

---

# Testing Matrix

Desktop

Windows

macOS

Linux

---

Tablet

iPad

Android Tablet

---

Mobile

Android

iPhone

---

Industrial Panel

Windows Touch

---

# Browser Support

Chrome

Edge

Firefox

Safari

---

# Accessibility

Zoom up to 200%.

Keyboard navigation maintained.

No horizontal scrolling on standard pages.

Touch controls remain usable.

---

# Best Practices

Design desktop first.

Avoid fixed widths.

Use responsive containers.

Keep layouts consistent.

Test every breakpoint.

Prioritize business information.

---

# Do

✓ Use responsive grid

✓ Use adaptive layouts

✓ Collapse navigation

✓ Optimize touch controls

✓ Test on multiple devices

---

# Don't

✗ Don't use fixed pixel layouts

✗ Don't hide important data

✗ Don't rely on hover

✗ Don't create different workflows per device

✗ Don't break keyboard navigation

---

# Acceptance Criteria

Layouts adapt correctly to every breakpoint.

Sidebar behaves according to device.

Dashboard remains usable.

Forms are responsive.

Tables remain readable.

Touch interactions work.

Performance remains acceptable.

Accessibility requirements are maintained.

---

# Related Documents

Grid_System.md

Spacing.md

Accessibility.md

Application_Shell.md

Sidebar.md

Header.md

Dashboard.md

Responsive.md

Design_Tokens.md
