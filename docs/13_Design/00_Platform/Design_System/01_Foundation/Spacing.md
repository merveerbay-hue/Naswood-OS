# Spacing

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Spacing defines the standard distance between interface elements throughout Naswood OS.

Consistent spacing improves readability, visual hierarchy, usability and maintainability.

Every component, page and layout must use predefined spacing tokens.

Hardcoded spacing values are prohibited.

---

# Objectives

- Consistent Layout
- Better Readability
- Predictable UI
- Faster Development
- Responsive Design
- Design Token Based

---

# Design Principles

Spacing should be:

- Consistent
- Predictable
- Scalable
- Minimal
- Responsive

Whitespace is an important design element and should not be minimized unnecessarily.

---

# Spacing Scale

The platform uses an **8-point grid system**.

Small adjustments may use **4 px** increments.

---

# Spacing Tokens

| Token | Value | Usage |
|--------|------:|---------------------------|
| spacing-0 | 0 px | No spacing |
| spacing-1 | 4 px | Icon spacing |
| spacing-2 | 8 px | Small gaps |
| spacing-3 | 12 px | Compact layouts |
| spacing-4 | 16 px | Default spacing |
| spacing-5 | 20 px | Medium spacing |
| spacing-6 | 24 px | Cards |
| spacing-8 | 32 px | Sections |
| spacing-10 | 40 px | Large sections |
| spacing-12 | 48 px | Dashboard blocks |
| spacing-16 | 64 px | Page separation |

---

# Page Layout

Page Padding

24 px

Section Gap

32 px

Content Gap

24 px

Bottom Margin

32 px

---

# Card Layout

Card Padding

24 px

Header Gap

16 px

Content Gap

16 px

Footer Gap

24 px

---

# Dashboard Layout

Widget Gap

24 px

Row Gap

24 px

KPI Gap

16 px

Chart Gap

24 px

---

# Forms

Label → Input

8 px

Input → Input

16 px

Field Group

24 px

Form Section

32 px

Button Group

16 px

---

# Buttons

Icon → Text

8 px

Button → Button

12 px

Toolbar Button Gap

8 px

---

# Tables

Header Padding

16 px

Cell Padding

12 px

Row Height

48 px

Toolbar Gap

16 px

Filter Gap

12 px

---

# Dialogs

Outer Padding

32 px

Header Gap

24 px

Content Gap

24 px

Footer Gap

24 px

Action Buttons

16 px

---

# Sidebar

Menu Item Padding

16 px

Menu Group Gap

24 px

Icon Gap

12 px

Section Gap

32 px

---

# Header

Horizontal Padding

24 px

Search Gap

16 px

Notification Gap

12 px

Profile Gap

16 px

---

# Navigation

Breadcrumb Gap

8 px

Navigation Gap

16 px

Tab Gap

8 px

---

# Empty States

Illustration → Title

24 px

Title → Description

12 px

Description → Action

24 px

---

# Notifications

Icon → Text

12 px

Notification Padding

16 px

Toast Gap

12 px

---

# Mobile

Page Padding

16 px

Card Padding

16 px

Widget Gap

16 px

Form Gap

16 px

---

# Tablet

Page Padding

24 px

Card Padding

24 px

Section Gap

24 px

---

# Desktop

Page Padding

24 px

Section Gap

32 px

Dashboard Gap

24 px

---

# Ultra Wide

Maximum Content Width

1600 px

Maintain centered layout.

Do not increase spacing indefinitely.

---

# CSS Variables

```css
:root{

--spacing-0:0px;
--spacing-1:4px;
--spacing-2:8px;
--spacing-3:12px;
--spacing-4:16px;
--spacing-5:20px;
--spacing-6:24px;
--spacing-8:32px;
--spacing-10:40px;
--spacing-12:48px;
--spacing-16:64px;

}
```

---

# Tailwind Mapping

| Token | Tailwind |
|--------|----------|
| spacing-1 | p-1 |
| spacing-2 | p-2 |
| spacing-3 | p-3 |
| spacing-4 | p-4 |
| spacing-5 | p-5 |
| spacing-6 | p-6 |
| spacing-8 | p-8 |
| spacing-10 | p-10 |
| spacing-12 | p-12 |
| spacing-16 | p-16 |

---

# Usage Rules

Use spacing tokens only.

Never hardcode margin or padding.

Prefer layout spacing over empty elements.

Maintain equal spacing inside similar components.

Use larger spacing to separate sections.

---

# Responsive Behaviour

Spacing decreases on smaller screens.

Component proportions remain consistent.

Touch targets must remain accessible.

---

# Accessibility

Spacing must support:

- Readability
- Touch interaction
- Keyboard navigation
- Screen magnification

Minimum touch target

44 × 44 px

---

# Best Practices

✓ Use the 8-point grid.

✓ Keep spacing consistent.

✓ Align elements to the grid.

✓ Use whitespace intentionally.

✓ Follow design tokens.

---

# Do

✓ Use spacing tokens

✓ Keep equal spacing

✓ Use layout padding

✓ Test responsive layouts

---

# Don't

✗ Hardcode spacing values

✗ Mix random spacing sizes

✗ Use empty divs for spacing

✗ Reduce spacing to fit more content

✗ Break alignment between components

---

# Acceptance Criteria

All layouts use spacing tokens.

No hardcoded margin or padding values exist.

Responsive layouts preserve spacing.

Forms remain aligned.

Cards maintain consistent padding.

Dashboard widgets use standard gaps.

Accessibility requirements are satisfied.

---

# Related Documents

Design_Tokens.md

Grid_System.md

Breakpoints.md

Border_Radius.md

Elevation.md

Buttons.md

Forms.md

Cards.md

Tables.md

Dashboard.md
