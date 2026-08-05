# Buttons

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Button component is the primary interactive control used throughout Naswood OS.

Buttons allow users to initiate actions, submit forms, trigger workflows and navigate between application states.

Every button must follow a consistent appearance, behavior and accessibility standard.

---

# Objectives

- Consistent User Experience
- Clear Visual Hierarchy
- Accessibility Compliance
- Responsive Interaction
- Reusable Component
- Enterprise Standard

---

# Design Principles

Buttons should be

- Clear
- Predictable
- Consistent
- Accessible
- Responsive

A button must clearly communicate its action.

---

# Button Hierarchy

Primary

Main action.

Example

Save

Submit

Create

Approve

---

Secondary

Alternative action.

Example

Cancel

Back

Close

Preview

---

Outline

Low emphasis action.

Example

Details

View

Open

---

Ghost

Minimal action.

Example

Toolbar

Navigation

Quick Actions

---

Danger

Destructive action.

Example

Delete

Reject

Archive

Reset

---

Success

Positive confirmation.

Example

Complete

Confirm

Finish

---

Link

Navigation only.

---

# Button Sizes

| Size | Height | Padding |
|--------|--------:|---------:|
| Small | 32 px | 12 px |
| Medium | 40 px | 16 px |
| Large | 48 px | 20 px |

Default

Medium

---

# Border Radius

Small

6 px

Medium

8 px

Large

8 px

Reference

Border_Radius.md

---

# Typography

Font

Inter

Weight

600

Text Transform

Sentence Case

Never use ALL CAPS.

---

# Icon Support

Supported

Leading Icon

Trailing Icon

Icon Only

Loading Icon

Icons use Lucide React.

---

# Icon Sizes

Small

16 px

Medium

18 px

Large

20 px

---

# Width

Default

Auto

Optional

Full Width

Buttons should not stretch unless required.

---

# Button States

Default

Hover

Pressed

Focused

Disabled

Loading

Success

Error

---

# Hover

Background slightly darkens.

Cursor becomes pointer.

Animation

100 ms

---

# Focus

Visible focus ring.

Keyboard accessible.

No shadow increase.

---

# Pressed

Scale

98%

Duration

100 ms

---

# Disabled

Reduced opacity.

No hover.

No click.

Cursor

Not Allowed

---

# Loading

Spinner replaces icon.

Text remains visible.

Button becomes disabled.

---

# Success

Optional check icon.

Short confirmation.

---

# Error

No shake animation.

Display message externally.

---

# Color Usage

Primary

Brand Color

Secondary

Neutral

Danger

Semantic Danger

Success

Semantic Success

Reference

Color_Tokens.md

---

# Spacing

Icon → Text

8 px

Button → Button

12 px

Toolbar Gap

8 px

Reference

Spacing.md

---

# Accessibility

Minimum Height

40 px

Touch Target

44 × 44 px

Keyboard Navigation

Required

Focus Ring

Required

ARIA Label

Required for Icon Buttons

---

# Keyboard Support

Tab

Shift + Tab

Enter

Space

---

# Responsive Behaviour

Desktop

Standard

Tablet

Standard

Mobile

Full Width when appropriate

---

# Usage Rules

Use only one Primary button per section.

Use Danger only for destructive actions.

Avoid more than three actions in a button group.

Prefer icons only when universally understood.

---

# Button Groups

Primary + Secondary

Preferred

Primary + Danger

Avoid

Three Buttons

Maximum

More than three

Use dropdown.

---

# Confirmation Actions

Delete

Archive

Reset

Require confirmation dialog.

---

# Forms

Submit

Primary

Cancel

Secondary

Reset

Outline

---

# Tables

Toolbar actions

Ghost

Bulk actions

Primary

Delete

Danger

---

# Dashboard

Quick Actions

Ghost

Main Actions

Primary

---

# Loading Behaviour

Disable repeated clicks.

Show spinner.

Maintain button width.

Do not hide text.

---

# Performance

Render quickly.

Avoid unnecessary animations.

Support lazy-loaded icons.

---

# React API

```tsx
<Button
    variant="primary"
    size="md"
    loading={false}
    disabled={false}
    icon={<Save />}
>
    Save
</Button>
```

---

# Supported Variants

Primary

Secondary

Outline

Ghost

Danger

Success

Link

Icon

---

# Best Practices

✓ One primary action.

✓ Clear labels.

✓ Consistent spacing.

✓ Keyboard accessible.

✓ Use semantic colors.

✓ Use icons carefully.

---

# Do

✓ Save

✓ Create Order

✓ Receive Goods

✓ Start Production

✓ Approve

✓ Print

---

# Don't

✗ Click Here

✗ OK

✗ Yes

✗ Button1

✗ Submit Form Now Immediately

---

# Acceptance Criteria

Buttons follow official variants.

Sizes comply with standards.

Icons use Lucide React.

Accessibility requirements pass.

Focus state is visible.

Loading state works correctly.

Disabled state prevents interaction.

Responsive behaviour is verified.

---

# Related Documents

Colors.md

Color_Tokens.md

Typography.md

Spacing.md

Border_Radius.md

Accessibility.md

Animation.md

Design_Tokens.md

Forms.md

Dialogs.md
