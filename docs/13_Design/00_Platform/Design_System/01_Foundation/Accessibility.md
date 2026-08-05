# Accessibility

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

Accessibility defines the standards that ensure Naswood OS can be used efficiently by all users, including people with visual, hearing, motor and cognitive impairments.

Accessibility is a mandatory requirement across the entire platform and must be considered during design, development and testing.

---

# Objectives

- Inclusive User Experience
- Keyboard Accessibility
- Screen Reader Compatibility
- High Contrast Support
- Color Independent Design
- WCAG 2.1 AA Compliance
- Consistent Navigation

---

# Standards

The platform follows:

- WCAG 2.1 AA
- ARIA Guidelines
- HTML5 Accessibility
- Keyboard Navigation Standards

---

# Accessibility Principles

Perceivable

Information must be visible and understandable.

---

Operable

Every function must be usable without a mouse.

---

Understandable

The interface must behave consistently.

---

Robust

The interface must work with assistive technologies.

---

# Keyboard Navigation

Every interactive component must support keyboard navigation.

Required Keys

Tab

Shift + Tab

Enter

Space

Arrow Keys

Esc

Home

End

Page Up

Page Down

---

# Focus Management

Visible focus indicator is mandatory.

Focus order follows page layout.

Focus cannot become trapped.

After closing dialogs, focus returns to previous control.

---

# Color Usage

Never communicate information using color alone.

Incorrect

Red = Error

Correct

Red + Icon + Text

---

# Contrast Ratios

Normal Text

Minimum 4.5 : 1

Large Text

Minimum 3 : 1

Icons

Minimum 3 : 1

Interactive Elements

Minimum 3 : 1

---

# Typography

Minimum font size

14px

Preferred

16px

Line Height

1.5

Paragraph Width

Maximum 80 characters

---

# Forms

Every input requires

Label

Placeholder (optional)

Validation Message

Error Message

Help Text (when necessary)

---

# Buttons

Minimum height

40px

Preferred

44px

Minimum width

44px

Disabled buttons must remain readable.

---

# Icons

Decorative icons

aria-hidden="true"

Functional icons

Accessible label required.

Example

Delete

Download

Search

Filter

Settings

---

# Images

Every informative image requires

Alternative Text

Decorative images

Empty alt attribute

Example

alt=""

---

# Tables

Every table requires

Header

Column Names

Row Names (when applicable)

Sortable columns must announce state.

---

# Dialogs

Focus moves inside dialog.

ESC closes dialog.

Background interaction disabled.

Screen reader announces dialog title.

---

# Notifications

Success

Information

Warning

Error

Must include

Icon

Title

Description

ARIA live region

---

# Animations

Respect reduced motion preferences.

Animations

< 300 ms

Allow user to disable animations.

---

# Responsive Accessibility

Desktop

Tablet

Mobile

Touch devices

Keyboard support remains available.

---

# Screen Reader Support

Support

NVDA

JAWS

VoiceOver

TalkBack

Narrator

---

# ARIA Usage

Use only where native HTML is insufficient.

Examples

aria-label

aria-describedby

aria-expanded

aria-selected

aria-current

aria-live

---

# Error Messages

Every error must

Explain problem

Suggest solution

Identify affected field

Example

Email address is required.

---

# Validation

Real-time validation

Accessible error summary

Field highlighting

Keyboard focus on first invalid field

---

# Loading States

Loading indicators require

Visible spinner

Loading text

ARIA status

Prevent duplicate actions

---

# Empty States

Every empty page includes

Illustration (optional)

Title

Description

Primary Action

---

# Accessibility Testing

Manual Testing

Keyboard Testing

Screen Reader Testing

Contrast Testing

Responsive Testing

Automated Testing

---

# Browser Support

Chrome

Edge

Firefox

Safari

---

# Mobile Accessibility

Minimum touch target

44 x 44 px

Support

VoiceOver

TalkBack

Large Text

Dark Mode

---

# Best Practices

Use semantic HTML.

Provide labels for all controls.

Avoid placeholder-only forms.

Support keyboard users.

Maintain logical focus order.

Use descriptive link text.

Avoid flashing content.

Provide sufficient spacing.

---

# Do

✓ Use semantic elements

✓ Use proper labels

✓ Use visible focus

✓ Test keyboard navigation

✓ Test dark mode

✓ Test screen readers

---

# Don't

✗ Don't use color alone

✗ Don't remove focus outline

✗ Don't use tiny touch targets

✗ Don't autoplay audio

✗ Don't trap keyboard focus

✗ Don't rely on placeholders as labels

---

# Acceptance Criteria

All pages support keyboard navigation.

All interactive controls have accessible names.

Color contrast meets WCAG AA.

Forms are fully accessible.

Dialogs manage focus correctly.

Notifications are announced.

Responsive layouts remain accessible.

Screen readers can navigate the application.

Accessibility tests pass before release.

---

# Related Documents

Colors.md

Typography.md

Icons.md

Buttons.md

Inputs.md

Forms.md

Tables.md

Dialogs.md

Responsive.md

Design_Tokens.md
