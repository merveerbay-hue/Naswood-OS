# Design Tokens

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Design Tokens are the single source of truth for all visual properties used throughout Naswood OS.

Every UI component, page, report and application must use design tokens instead of hardcoded values.

Tokens ensure visual consistency, simplify maintenance and enable theme support.

---

# Objectives

- Consistent UI
- Centralized Styling
- Theme Support
- Maintainability
- Cross Platform Compatibility
- Figma Synchronization
- Tailwind Integration

---

# Design Token Categories

The Design System consists of the following token groups:

- Color Tokens
- Typography Tokens
- Spacing Tokens
- Radius Tokens
- Shadow Tokens
- Opacity Tokens
- Motion Tokens
- Size Tokens
- Z-Index Tokens
- Layout Tokens

---

# Token Naming Convention

Tokens follow a structured naming convention.

Format

category-name-level

Examples

color-primary

spacing-md

radius-lg

shadow-sm

font-size-base

z-modal

---

# Spacing Tokens

| Token | Value |
|--------|------:|
| spacing-0 | 0 px |
| spacing-1 | 4 px |
| spacing-2 | 8 px |
| spacing-3 | 12 px |
| spacing-4 | 16 px |
| spacing-5 | 20 px |
| spacing-6 | 24 px |
| spacing-8 | 32 px |
| spacing-10 | 40 px |
| spacing-12 | 48 px |
| spacing-16 | 64 px |

---

# Radius Tokens

| Token | Value |
|--------|------:|
| radius-none | 0 px |
| radius-sm | 4 px |
| radius-md | 6 px |
| radius-lg | 8 px |
| radius-xl | 12 px |
| radius-2xl | 16 px |
| radius-full | 9999 px |

---

# Shadow Tokens

| Token | Usage |
|--------|-------|
| shadow-none | Flat UI |
| shadow-sm | Inputs |
| shadow-md | Cards |
| shadow-lg | Dialogs |
| shadow-xl | Floating Panels |

---

# Opacity Tokens

| Token | Value |
|--------|------:|
| opacity-0 | 0% |
| opacity-25 | 25% |
| opacity-50 | 50% |
| opacity-75 | 75% |
| opacity-100 | 100% |

---

# Motion Tokens

| Token | Duration |
|--------|---------:|
| motion-fast | 100 ms |
| motion-normal | 200 ms |
| motion-slow | 300 ms |

---

# Size Tokens

## Buttons

Height

40 px

---

## Inputs

Height

40 px

---

## Header

Height

64 px

---

## Sidebar

Expanded

280 px

Collapsed

72 px

---

## Cards

Minimum Width

320 px

---

## Dialog

Maximum Width

900 px

---

# Layout Tokens

Container Max Width

1600 px

Content Padding

24 px

Grid Gap

24 px

Section Gap

32 px

Card Padding

24 px

Form Gap

16 px

---

# Z-Index Tokens

| Token | Value |
|--------|------:|
| z-base | 1 |
| z-dropdown | 100 |
| z-sticky | 200 |
| z-header | 300 |
| z-sidebar | 400 |
| z-modal | 1000 |
| z-toast | 1100 |
| z-tooltip | 1200 |

---

# Token Hierarchy

Foundation

↓

Component

↓

Page

↓

Application

Tokens may only be overridden through themes.

---

# Theme Support

Design Tokens support

- Light Theme
- Dark Theme
- Corporate Theme

Only token values change.

Component code remains unchanged.

---

# CSS Variables

Example

```css
:root{

--spacing-4:16px;
--spacing-6:24px;

--radius-lg:8px;

--shadow-md:0 4px 12px rgba(0,0,0,.08);

--motion-normal:200ms;

}
```

---

# Tailwind Mapping

Example

```js
theme:{

spacing:{
4:"16px",
6:"24px"
},

borderRadius:{
lg:"8px"
}

}
```

---

# Usage Rules

Use design tokens only.

Never hardcode values.

Do not duplicate tokens.

Keep naming consistent.

Use semantic naming.

---

# Best Practices

✓ Centralize every visual value.

✓ Use CSS Variables.

✓ Keep tokens reusable.

✓ Support theming.

✓ Document every token.

---

# Do

✓ Use spacing tokens

✓ Use radius tokens

✓ Use shadow tokens

✓ Use z-index tokens

✓ Use motion tokens

---

# Don't

✗ Hardcode spacing

✗ Hardcode radius

✗ Hardcode shadows

✗ Create duplicate tokens

✗ Override tokens inside components

---

# Acceptance Criteria

All visual values are token-based.

No hardcoded UI values exist.

Tailwind configuration uses tokens.

React components consume tokens.

Theme switching updates token values.

Figma design references the same token names.

---

# Related Documents

Colors.md

Color_Tokens.md

Typography.md

Spacing.md

Border_Radius.md

Animation.md

Breakpoints.md

Theme.md
