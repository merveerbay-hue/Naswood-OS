# Color Tokens

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Color Tokens define the official color variables used throughout Naswood OS.

All applications, components and themes must reference these tokens.

Hardcoded colors are prohibited.

---

# Objectives

- Single Source of Truth
- Theme Support
- Consistent UI
- Easy Maintenance
- Dark Mode Ready
- Tailwind Compatible

---

# Token Naming Convention

color-{category}-{level}

Examples

color-primary

color-success

color-danger

color-gray-200

color-background

---

# Brand Tokens

| Token | HEX |
|---------|---------|
| color-primary | #E67E22 |
| color-primary-hover | #D46F17 |
| color-primary-active | #B85F10 |
| color-secondary | #2F3A45 |
| color-secondary-hover | #25303A |

---

# Semantic Tokens

| Token | HEX |
|---------|---------|
| color-success | #16A34A |
| color-warning | #F59E0B |
| color-danger | #DC2626 |
| color-info | #2563EB |

---

# Neutral Tokens

| Token | HEX |
|---------|---------|
| gray-50 | #F9FAFB |
| gray-100 | #F3F4F6 |
| gray-200 | #E5E7EB |
| gray-300 | #D1D5DB |
| gray-400 | #9CA3AF |
| gray-500 | #6B7280 |
| gray-600 | #4B5563 |
| gray-700 | #374151 |
| gray-800 | #1F2937 |
| gray-900 | #111827 |

---

# Background Tokens

| Token | Light | Dark |
|---------|---------|---------|
| background | #FFFFFF | #0F172A |
| surface | #F8FAFC | #1E293B |
| surface-hover | #F1F5F9 | #334155 |

---

# Text Tokens

| Token | Light | Dark |
|---------|---------|---------|
| text-primary | #111827 | #F8FAFC |
| text-secondary | #4B5563 | #CBD5E1 |
| text-muted | #9CA3AF | #64748B |
| text-inverse | #FFFFFF | #111827 |

---

# Border Tokens

| Token | Light | Dark |
|---------|---------|---------|
| border-default | #E5E7EB | #334155 |
| border-hover | #CBD5E1 | #475569 |
| border-focus | #2563EB | #60A5FA |

---

# Status Tokens

Inventory

Production

Quality

Maintenance

Purchasing

Sales

Finance

AI

Each module may extend the semantic tokens without changing the core palette.

---

# CSS Variables

```css
:root{

--color-primary:#E67E22;
--color-secondary:#2F3A45;

--color-success:#16A34A;
--color-warning:#F59E0B;
--color-danger:#DC2626;
--color-info:#2563EB;

--color-background:#FFFFFF;
--color-surface:#F8FAFC;

--text-primary:#111827;
--text-secondary:#4B5563;

--border-default:#E5E7EB;

}
```

---

# Dark Theme

```css
[data-theme="dark"]{

--color-background:#0F172A;
--color-surface:#1E293B;

--text-primary:#F8FAFC;
--text-secondary:#CBD5E1;

--border-default:#334155;

}
```

---

# Tailwind Mapping

```js
theme:{

colors:{

primary:"var(--color-primary)",

secondary:"var(--color-secondary)",

success:"var(--color-success)",

warning:"var(--color-warning)",

danger:"var(--color-danger)",

info:"var(--color-info)"

}

}
```

---

# Usage Rules

Always use tokens.

Never use HEX values inside components.

Never duplicate tokens.

Support Light and Dark themes.

Keep semantic meaning unchanged.

---

# Best Practices

✓ Use semantic colors.

✓ Keep brand colors centralized.

✓ Use CSS variables.

✓ Use Tailwind mapping.

✓ Support Dark Mode.

---

# Don't

✗ Hardcode colors

✗ Duplicate color values

✗ Mix brand and semantic colors

✗ Change token names

---

# Acceptance Criteria

All components use color tokens.

No hardcoded HEX values exist.

Light and Dark themes work correctly.

Tailwind configuration matches tokens.

React components reference tokens only.

---

# Related Documents

Colors.md

Design_Tokens.md

Theme.md

Corporate_Colors.md

Accessibility.md
