# Dialogs

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Dialog component provides a standardized way to display temporary content, confirmations, forms and critical information without navigating away from the current page.

Dialogs must interrupt the user's workflow only when necessary.

---

# Objectives

- Consistent User Experience
- Safe User Confirmation
- Accessible Modal Windows
- Reusable Component
- Enterprise Standard
- Keyboard Friendly

---

# Design Principles

Dialogs should be

- Focused
- Minimal
- Predictable
- Accessible
- Responsive

A dialog should solve one task only.

---

# Dialog Types

Confirmation Dialog

Alert Dialog

Information Dialog

Form Dialog

Wizard Dialog

Fullscreen Dialog

Drawer Dialog

AI Dialog

Preview Dialog

Help Dialog

---

# Dialog Structure

```
Dialog
│
├── Header
│     ├── Title
│     ├── Subtitle (Optional)
│     └── Close Button
│
├── Body
│
└── Footer
      ├── Primary Action
      ├── Secondary Action
      └── Optional Actions
```

---

# Header

Contains

Title

Subtitle

Close Button

Status Badge (Optional)

---

# Body

Contains

Text

Forms

Tables

Lists

Images

Charts

Documents

---

# Footer

Contains

Primary Button

Secondary Button

Danger Button (Optional)

---

# Dialog Sizes

| Size | Width |
|--------|-------|
| Small | 480 px |
| Medium | 720 px |
| Large | 900 px |
| Extra Large | 1200 px |
| Fullscreen | 100% |

Default

Medium

---

# Height

Automatic

Maximum

90vh

Body should scroll independently.

---

# Border Radius

12 px

Reference

Border_Radius.md

---

# Elevation

Elevation Level

4

Reference

Elevation.md

---

# Padding

Header

24 px

Body

24 px

Footer

24 px

---

# Dialog Variants

## Confirmation

Purpose

Confirm user actions.

Examples

Delete

Archive

Approve

Reject

Reset

---

## Alert

Critical system messages.

Single action.

---

## Information

Read-only information.

---

## Form

Create

Edit

Settings

Configuration

---

## Wizard

Multi-step process.

Examples

Production Order Wizard

Supplier Onboarding

Material Creation

---

## Drawer

Slides from right.

Used for quick editing.

Does not block workflow.

---

## Fullscreen

Complex workflows.

Large forms.

Document preview.

---

## AI Dialog

AI Chat

Prompt

Recommendations

Knowledge Search

---

# Buttons

Primary

Maximum one.

Secondary

Optional.

Danger

Only for destructive actions.

---

# Close Behaviour

Allowed

Close Button

ESC

Cancel Button

Backdrop Click (Optional)

---

Not Allowed

During critical processing.

During data save.

---

# Keyboard Support

Tab

Shift + Tab

Enter

Escape

Arrow Keys

---

# Focus Management

Focus moves to first interactive element.

Focus is trapped inside dialog.

Closing dialog restores previous focus.

---

# Backdrop

Background becomes inactive.

Scrolling disabled.

Opacity

40%

---

# Responsive Behaviour

Desktop

Centered Dialog

---

Tablet

Large Dialog

---

Mobile

Fullscreen

Bottom actions fixed.

---

# Accessibility

Supports

Keyboard Navigation

Focus Trap

Screen Readers

ARIA Labels

Accessible Titles

Required

role="dialog"

aria-modal="true"

aria-labelledby

aria-describedby

---

# Validation

Validation messages remain inside dialog.

First invalid field receives focus.

Buttons remain visible.

---

# Loading State

Disable actions.

Show progress.

Keep dialog open.

Prevent duplicate submission.

---

# Error State

Error Banner

Retry

Cancel

Help Link (Optional)

---

# Success State

Success Message

Close Automatically (Optional)

Navigate Back (Optional)

---

# Performance

Lazy load large content.

Virtualize large tables.

Avoid opening multiple dialogs simultaneously.

---

# React API

```tsx
<Dialog
    open={open}
    size="md"
    title="Create Material"
    onClose={handleClose}
>

    <DialogHeader />

    <DialogContent />

    <DialogFooter />

</Dialog>
```

---

# Supported Sizes

sm

md

lg

xl

fullscreen

drawer

---

# Supported Variants

confirmation

alert

info

form

wizard

drawer

fullscreen

ai

preview

---

# Best Practices

✓ One task per dialog

✓ Clear title

✓ Clear primary action

✓ Limit content

✓ Restore focus after close

✓ Validate before submit

---

# Do

✓ Confirm Delete

✓ Create Customer

✓ Edit Material

✓ Approve Purchase Order

✓ Show Error Details

---

# Don't

✗ Display large dashboards

✗ Nest dialogs

✗ Open multiple dialogs

✗ Hide primary action

✗ Use dialogs for navigation

---

# Acceptance Criteria

Dialogs use official component.

Focus is trapped correctly.

ESC closes dialog when allowed.

Accessibility requirements pass.

Responsive layout works.

Loading state prevents duplicate actions.

Primary and secondary actions are clearly defined.

---

# Related Documents

Buttons.md

Forms.md

Inputs.md

Cards.md

Typography.md

Spacing.md

Border_Radius.md

Elevation.md

Accessibility.md

Design_Tokens.md
