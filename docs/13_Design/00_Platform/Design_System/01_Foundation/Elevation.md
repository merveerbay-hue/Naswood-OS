# Elevation

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Elevation defines the visual hierarchy of interface elements through shadows and layering.

It helps users understand which elements are interactive, floating or currently active without relying solely on color.

Elevation must remain subtle and support usability rather than decoration.

---

# Objectives

- Visual Hierarchy
- Depth Perception
- Consistent UI
- Predictable Layering
- Better User Experience
- Theme Compatibility
- Enterprise Design Language

---

# Design Principles

Elevation should be:

- Minimal
- Consistent
- Functional
- Predictable
- Accessible

Every increase in elevation must indicate increased importance or interaction.

---

# Elevation Levels

| Level | Token | Usage |
|--------|--------|-----------------------------|
| 0 | elevation-0 | Flat surfaces |
| 1 | elevation-1 | Inputs |
| 2 | elevation-2 | Cards |
| 3 | elevation-3 | Dropdowns |
| 4 | elevation-4 | Dialogs |
| 5 | elevation-5 | Notifications |
| 6 | elevation-6 | Global overlays |

---

# Usage Guide

## Elevation 0

No shadow.

Examples

- Application background
- Header
- Sidebar
- Tables

---

## Elevation 1

Very subtle shadow.

Examples

- Input fields
- Search boxes
- Form controls

---

## Elevation 2

Small floating effect.

Examples

- Cards
- Dashboard widgets
- Information panels

---

## Elevation 3

Medium elevation.

Examples

- Dropdown menus
- Context menus
- Tooltips
- Popovers

---

## Elevation 4

High emphasis.

Examples

- Dialogs
- Modals
- Confirmation windows

---

## Elevation 5

Temporary overlays.

Examples

- Toast notifications
- Floating action panels
- Command palette

---

## Elevation 6

Highest elevation.

Examples

- Emergency alerts
- Fullscreen overlays
- Loading overlays

---

# Shadow Tokens

| Token | Shadow |
|--------|--------|
| elevation-0 | none |
| elevation-1 | 0 1px 3px rgba(0,0,0,.08) |
| elevation-2 | 0 4px 10px rgba(0,0,0,.10) |
| elevation-3 | 0 8px 18px rgba(0,0,0,.12) |
| elevation-4 | 0 12px 28px rgba(0,0,0,.16) |
| elevation-5 | 0 20px 40px rgba(0,0,0,.18) |
| elevation-6 | 0 24px 60px rgba(0,0,0,.24) |

---

# Light Theme

Use soft shadows.

Maintain subtle depth.

Avoid dark heavy shadows.

---

# Dark Theme

Reduce shadow opacity.

Use surface contrast instead of stronger shadows.

Prefer borders combined with shadows.

---

# Elevation Hierarchy

Application

↓

Header / Sidebar

↓

Cards

↓

Dropdowns

↓

Dialogs

↓

Notifications

↓

Global Overlay

---

# Hover Behaviour

Hover may increase elevation by one level.

Example

Card

Level 2

↓

Hover

Level 3

---

Button

No elevation change.

Use background transition instead.

---

# Focus Behaviour

Focus should use outline.

Do not increase elevation.

---

# Disabled Components

Disabled components never receive elevation changes.

---

# Tables

Tables remain flat.

Rows use background color for hover.

Avoid shadows inside data grids.

---

# Cards

Default

Elevation 2

Hover

Elevation 3

Selected

Elevation 3 + Border

---

# Dialogs

Always centered.

Elevation 4.

Background overlay required.

---

# Dropdowns

Elevation 3.

Maximum width determined by content.

Shadow must not interfere with adjacent components.

---

# Notifications

Toast notifications

Elevation 5.

Temporary.

Auto-dismiss.

---

# AI Components

AI Chat Panel

Elevation 4

Suggestion Cards

Elevation 2

Prompt Box

Elevation 1

---

# Mobile

Reduce shadow intensity.

Avoid excessive blur.

Use spacing instead of deep shadows.

---

# Performance

Use CSS box-shadow.

Avoid multiple layered shadows.

Avoid animated shadow blur.

Prefer opacity and transform animations.

---

# CSS Variables

```css
:root{

--elevation-0:none;

--elevation-1:0 1px 3px rgba(0,0,0,.08);

--elevation-2:0 4px 10px rgba(0,0,0,.10);

--elevation-3:0 8px 18px rgba(0,0,0,.12);

--elevation-4:0 12px 28px rgba(0,0,0,.16);

--elevation-5:0 20px 40px rgba(0,0,0,.18);

--elevation-6:0 24px 60px rgba(0,0,0,.24);

}
```

---

# Tailwind Mapping

| Token | Utility |
|--------|---------|
| elevation-0 | shadow-none |
| elevation-1 | shadow-sm |
| elevation-2 | shadow |
| elevation-3 | shadow-md |
| elevation-4 | shadow-lg |
| elevation-5 | shadow-xl |
| elevation-6 | shadow-2xl |

---

# Accessibility

Elevation must never be the only indicator.

Combine elevation with:

- Border
- Color
- Focus Ring
- Icons
- Labels

---

# Best Practices

✓ Keep shadows subtle.

✓ Use consistent elevation levels.

✓ Combine elevation with spacing.

✓ Prefer borders for dark mode.

✓ Test on low-brightness displays.

---

# Do

✓ Use predefined elevation tokens.

✓ Keep hierarchy consistent.

✓ Use shadows sparingly.

✓ Increase elevation only when interaction requires it.

---

# Don't

✗ Don't create custom shadows.

✗ Don't stack multiple shadows.

✗ Don't animate blur radius.

✗ Don't rely only on shadows to indicate focus.

✗ Don't use shadows inside tables.

---

# Acceptance Criteria

All components use predefined elevation tokens.

No custom shadows exist.

Cards, dialogs and dropdowns follow the elevation hierarchy.

Dark theme remains visually balanced.

Performance remains smooth.

Accessibility requirements are satisfied.

---

# Related Documents

Design_Tokens.md

Border_Radius.md

Colors.md

Animation.md

Cards.md

Dialogs.md

Dashboard.md

Theme.md
