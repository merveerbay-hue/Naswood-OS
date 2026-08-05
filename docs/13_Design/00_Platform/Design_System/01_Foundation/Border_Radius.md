# Border Radius

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Border Radius defines the corner rounding standard for every UI component in Naswood OS.

A consistent border radius improves visual harmony, usability and recognition across the platform while maintaining a modern enterprise appearance.

All components must use predefined radius tokens. Hardcoded values are not allowed.

---

# Objectives

- Consistent Appearance
- Modern Enterprise UI
- Reusable Components
- Predictable Design
- Design Token Based
- Theme Compatible

---

# Design Principles

Border radius should be:

- Consistent
- Minimal
- Functional
- Predictable
- Token Based

Avoid excessive rounding.

Naswood OS follows a professional industrial software design language rather than a consumer application style.

---

# Radius Scale

| Token | Value | Usage |
|--------|------:|------|
| radius-none | 0px | Tables, grids |
| radius-xs | 2px | Badges |
| radius-sm | 4px | Inputs |
| radius-md | 6px | Cards |
| radius-lg | 8px | Buttons |
| radius-xl | 12px | Dialogs |
| radius-2xl | 16px | Dashboard widgets |
| radius-full | 9999px | Avatar, Pills |

---

# Default Radius

Buttons

8px

---

Inputs

6px

---

Cards

8px

---

Dialogs

12px

---

Tables

0px

---

Dropdown

8px

---

Tooltip

6px

---

Notification

8px

---

Sidebar

0px

---

Header

0px

---

Dashboard Widgets

12px

---

Avatar

Full

---

Tags

9999px

---

Progress Bar

9999px

---

Search Box

8px

---

Modal

12px

---

Drawer

0px

---

# Component Standards

## Buttons

Primary

8px

Secondary

8px

Danger

8px

Ghost

8px

---

## Inputs

Text

6px

Number

6px

Password

6px

Search

8px

Textarea

6px

---

## Cards

Standard Card

8px

Dashboard Card

12px

Information Card

8px

Statistic Card

12px

---

## Tables

Data Grid

0px

Header

0px

Rows

0px

Cells

0px

---

## Dialogs

Confirmation

12px

Alert

12px

Wizard

12px

Settings

12px

---

## Notifications

Toast

8px

Banner

0px

Alert

8px

Snackbar

8px

---

## Navigation

Sidebar

0px

Top Navigation

0px

Tabs

6px

Breadcrumb

0px

---

## AI Components

AI Chat

12px

Prompt Box

8px

Suggestion Card

12px

Knowledge Panel

8px

---

# Design Tokens

```css
--radius-none: 0px;
--radius-xs: 2px;
--radius-sm: 4px;
--radius-md: 6px;
--radius-lg: 8px;
--radius-xl: 12px;
--radius-2xl: 16px;
--radius-full: 9999px;
```

---

# Tailwind Mapping

| Token | Tailwind |
|--------|----------|
| radius-none | rounded-none |
| radius-xs | rounded-sm |
| radius-sm | rounded |
| radius-md | rounded-md |
| radius-lg | rounded-lg |
| radius-xl | rounded-xl |
| radius-2xl | rounded-2xl |
| radius-full | rounded-full |

---

# Usage Rules

Use predefined tokens only.

Never use random values.

Do not mix different radii within the same component.

Use larger radii only for containers.

Maintain consistency across modules.

---

# Responsive Behaviour

Desktop

Same radius values.

Tablet

Same radius values.

Mobile

Same radius values.

Border radius should not change between devices.

---

# Theme Support

Light Theme

Supported

Dark Theme

Supported

High Contrast

Supported

Corporate Theme

Supported

---

# Accessibility

Rounded corners must not reduce clickable area.

Touch targets remain at least 44 × 44 px.

Focus outlines must remain visible.

---

# Examples

Primary Button

radius-lg

---

Input Field

radius-md

---

Dashboard Widget

radius-2xl

---

Table

radius-none

---

Avatar

radius-full

---

# Best Practices

Use one radius scale across the application.

Prefer subtle rounding.

Use larger radius only for high-level containers.

Keep forms visually consistent.

Follow design tokens.

---

# Do

✓ Use design tokens

✓ Keep radius consistent

✓ Use 8px for buttons

✓ Use 6px for inputs

✓ Use 12px for dialogs

✓ Keep tables square

---

# Don't

✗ Don't hardcode radius values

✗ Don't mix different radius values

✗ Don't over-round business components

✗ Don't use circular buttons unless required

✗ Don't create inconsistent card styles

---

# Acceptance Criteria

All components use predefined radius tokens.

No hardcoded radius values exist.

Buttons use 8px radius.

Inputs use 6px radius.

Dialogs use 12px radius.

Tables remain square.

Responsive layouts preserve border radius.

Design token validation passes.

---

# Related Documents

Colors.md

Spacing.md

Design_Tokens.md

Buttons.md

Inputs.md

Cards.md

Dialogs.md

Dashboard.md

Accessibility.md
