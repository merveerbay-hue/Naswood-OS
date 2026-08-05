# Colors

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Colors specification defines the visual color language used throughout Naswood OS.

Colors communicate hierarchy, status, feedback and brand identity. Every module must follow the same color rules to provide a consistent enterprise user experience.

Color implementation is defined in **Color_Tokens.md**.

---

# Design Principles

The color system must be:

- Consistent
- Accessible
- Semantic
- Minimal
- Brand Oriented
- Theme Compatible

Color should support the interface rather than dominate it.

---

# Color Categories

The Naswood color system consists of:

- Brand Colors
- Neutral Colors
- Semantic Colors
- Background Colors
- Text Colors
- Border Colors
- Module Colors

---

# Brand Colors

## Primary

Naswood Orange

Purpose

- Primary Buttons
- Active Navigation
- Links
- Progress
- Primary Actions

---

## Secondary

Anthracite Gray

Purpose

- Header
- Sidebar
- Navigation
- Secondary Buttons

---

# Neutral Colors

Neutral colors build the interface.

Used for:

- Backgrounds
- Cards
- Tables
- Forms
- Borders
- Disabled States

Never use neutral colors to communicate business meaning.

---

# Semantic Colors

## Success

Green

Used for

- Successful Operations
- Approved Records
- Completed Production
- Available Inventory

---

## Warning

Amber

Used for

- Low Inventory
- Delayed Production
- Pending Approval
- Maintenance Due

---

## Danger

Red

Used for

- Errors
- Validation Failures
- Critical Alarms
- Machine Breakdown

---

## Information

Blue

Used for

- Notifications
- Messages
- Reports
- Links

---

# Background Colors

Backgrounds are divided into:

Application Background

Workspace

Card Surface

Modal Surface

Hover Surface

Selected Surface

Different surfaces improve hierarchy.

---

# Text Colors

Text uses three levels.

Primary Text

Main content.

Secondary Text

Descriptions.

Muted Text

Hints.

Never use brand colors for body text.

---

# Border Colors

Borders separate information.

Used for

Inputs

Cards

Tables

Dialogs

Panels

---

# Module Identity Colors

Each module has a visual identity.

| Module | Primary Color |
|----------|---------------|
| Platform | Gray |
| Master Data | Slate |
| Inventory | Blue |
| Purchasing | Purple |
| Sales | Emerald |
| Production | Orange |
| Quality | Green |
| Maintenance | Amber |
| Finance | Indigo |
| Analytics | Cyan |
| AI | Violet |
| Digital Twin | Teal |

Module colors should appear in icons, badges and dashboard widgets only.

---

# Color Hierarchy

Primary Action

↓

Secondary Action

↓

Information

↓

Warning

↓

Danger

Users should immediately recognize priority by color.

---

# Interactive States

Every interactive component supports:

Default

Hover

Focused

Pressed

Disabled

Selected

Error

Success

Colors must change consistently between states.

---

# Dark Theme

Dark mode is supported.

Only token values change.

No component should define separate colors.

All themes use the same design tokens.

---

# Accessibility

Do not use color as the only indicator.

Always combine with:

- Icons
- Labels
- Status Text
- Tooltips

Minimum contrast follows WCAG 2.1 AA.

---

# Color Usage Rules

Use semantic colors for status.

Use brand colors for actions.

Use neutral colors for layout.

Avoid decorative colors.

Maintain consistency across modules.

---

# Best Practices

✓ Keep the interface clean.

✓ Use semantic colors consistently.

✓ Minimize accent colors.

✓ Follow design tokens.

✓ Test light and dark themes.

---

# Do

✓ Use Primary for primary actions.

✓ Use Success for completed operations.

✓ Use Warning for attention.

✓ Use Danger for critical issues.

✓ Use Neutral colors for layouts.

---

# Don't

✗ Don't hardcode HEX values.

✗ Don't invent new colors.

✗ Don't use red for warnings.

✗ Don't overload the interface with colors.

✗ Don't rely only on color for communication.

---

# Acceptance Criteria

Brand colors are used consistently.

Semantic colors are correctly applied.

Dark theme functions correctly.

Accessibility requirements are satisfied.

No hardcoded colors exist.

Components reference Color Tokens.

---

# Related Documents

Color_Tokens.md

Design_Tokens.md

Accessibility.md

Theme.md

Buttons.md

Tables.md

Cards.md

Dashboard.md
