# Typography

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Typography defines the visual language for all written content within Naswood OS.

A consistent typography system improves readability, hierarchy, usability and overall user experience while supporting enterprise workflows across office, warehouse and production environments.

Typography must remain clear, functional and accessible.

---

# Objectives

- Improve Readability
- Establish Visual Hierarchy
- Ensure Consistency
- Support Accessibility
- Enhance Enterprise User Experience
- Optimize Multi-Device Usage

---

# Design Principles

Typography should be:

- Simple
- Readable
- Consistent
- Accessible
- Scalable

Text exists to communicate information, not decoration.

---

# Primary Typeface

Primary Font

Inter

Usage

Entire application

Reasons

- Excellent screen readability
- Modern appearance
- Open Source
- Excellent multilingual support
- Optimized for UI

---

# Secondary Typeface

Fallback Stack

Inter

↓

Segoe UI

↓

Roboto

↓

Helvetica Neue

↓

Arial

↓

sans-serif

---

# Font Weights

| Weight | Usage |
|---------|------|
| 400 | Body Text |
| 500 | Labels |
| 600 | Section Titles |
| 700 | Page Titles |

Avoid using weights above 700.

---

# Typography Scale

| Style | Size | Weight | Line Height |
|--------|------:|-------:|------------:|
| Display | 36 px | 700 | 44 px |
| H1 | 32 px | 700 | 40 px |
| H2 | 28 px | 700 | 36 px |
| H3 | 24 px | 600 | 32 px |
| H4 | 20 px | 600 | 28 px |
| H5 | 18 px | 600 | 24 px |
| H6 | 16 px | 600 | 24 px |
| Body Large | 16 px | 400 | 24 px |
| Body | 14 px | 400 | 22 px |
| Small | 13 px | 400 | 20 px |
| Caption | 12 px | 400 | 18 px |
| Overline | 11 px | 500 | 16 px |

---

# Usage Hierarchy

Display

Landing pages

---

H1

Page titles

---

H2

Module titles

---

H3

Section titles

---

H4

Card titles

---

Body

General content

---

Caption

Metadata

---

# Text Alignment

Default

Left

Numbers

Right

Titles

Left

Centered text should be avoided except in empty states and landing pages.

---

# Line Length

Recommended

60–80 characters

Maximum

100 characters

---

# Paragraph Spacing

Paragraph Gap

16 px

Section Gap

32 px

List Gap

8 px

---

# Text Colors

Primary

Main content

Secondary

Descriptions

Muted

Hints

Disabled

Unavailable content

All colors are defined in **Color_Tokens.md**.

---

# Links

Links use the primary brand color.

Hover

Underline

Visited

Optional

Links should always be distinguishable from body text.

---

# Numbers

Financial values

Right aligned

Use tabular numbers where supported.

Maintain consistent decimal formatting.

---

# Tables

Header

600 Weight

Body

400 Weight

Numeric columns

Right aligned

Text columns

Left aligned

---

# Forms

Labels

Medium (500)

Input Text

Regular (400)

Help Text

Small

Error Message

Small

Required indicator

Visible

---

# Buttons

Weight

600

Text

Sentence Case

Avoid ALL CAPS.

---

# Dashboard

KPI Value

Large

KPI Label

Small

Chart Labels

Body

Axis Labels

Small

---

# Accessibility

Minimum body text

14 px

Minimum contrast

WCAG 2.1 AA

Avoid light gray text.

Do not rely on font weight alone to communicate importance.

---

# Responsive Behaviour

Desktop

Full typography scale

Tablet

Same hierarchy

Mobile

Reduce large headings only

Body text remains at least 14 px.

---

# Internationalization

Support

Turkish

English

German

French

Future languages

Typography must support Unicode.

---

# Performance

Use variable fonts when possible.

Load only required font weights.

Enable font-display: swap.

---

# CSS Variables

```css
:root{

--font-family:Inter,sans-serif;

--font-weight-regular:400;
--font-weight-medium:500;
--font-weight-semibold:600;
--font-weight-bold:700;

}
```

---

# Best Practices

✓ Keep typography simple.

✓ Use consistent hierarchy.

✓ Use sentence case.

✓ Limit font weights.

✓ Maintain readability.

---

# Do

✓ Use Inter

✓ Follow typography scale

✓ Keep body text readable

✓ Use semantic headings

✓ Align numbers correctly

---

# Don't

✗ Use multiple font families

✗ Use decorative fonts

✗ Use ALL CAPS excessively

✗ Use centered paragraphs

✗ Reduce body text below 14 px

---

# Acceptance Criteria

Typography follows the official scale.

Only approved font families are used.

Body text is readable.

Tables use correct alignment.

Buttons use consistent typography.

Accessibility requirements are met.

Responsive layouts preserve readability.

---

# Related Documents

Design_Tokens.md

Color_Tokens.md

Spacing.md

Accessibility.md

Buttons.md

Forms.md

Tables.md

Dashboard.md
