# Iconography

**Module:** Design System

**Category:** Foundation

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Iconography standard defines the visual language, usage rules and consistency of icons used throughout Naswood OS.

Icons improve recognition, navigation and usability by providing clear visual cues for actions, objects and system states.

Icons supplement text and should never replace meaningful labels.

---

# Objectives

- Consistent Visual Language
- Faster Recognition
- Improved Navigation
- Enterprise User Experience
- Accessibility Compliance
- Cross Platform Consistency

---

# Design Principles

Icons must be:

- Simple
- Recognizable
- Consistent
- Minimal
- Functional

Icons should communicate meaning immediately.

---

# Icon Library

Official icon library

Lucide Icons

Secondary Support

Heroicons

Custom Icons

Naswood Manufacturing Icons

No other icon sets are permitted.

---

# Icon Style

Outlined Icons

Preferred

Filled Icons

Status indicators only

Two-tone Icons

Not recommended

3D Icons

Not allowed

Gradient Icons

Not allowed

---

# Icon Size

| Size | Usage |
|------|------|
| 16 px | Dense Tables |
| 18 px | Secondary Controls |
| 20 px | Navigation |
| 24 px | Standard Actions |
| 32 px | Dashboard |
| 48 px | Empty States |
| 64 px | Landing Pages |

---

# Stroke Width

Default

2 px

Small Icons

1.5 px

Large Icons

2 px

Maintain consistent stroke across the application.

---

# Color Usage

Icons inherit text color.

Do not assign arbitrary colors.

Semantic colors are allowed only for:

Success

Warning

Danger

Information

---

# Standard Icon Categories

Navigation

Actions

Status

Documents

Inventory

Production

Quality

Maintenance

Finance

Analytics

AI

Settings

Users

Notifications

Reports

---

# Navigation Icons

Dashboard

Home

Inventory

Warehouse

Production

Quality

Maintenance

Purchasing

Sales

Finance

Analytics

AI

Digital Twin

Settings

Help

Logout

---

# Action Icons

Add

Edit

Delete

Save

Cancel

Refresh

Download

Upload

Print

Export

Import

Search

Filter

Sort

Copy

Share

Approve

Reject

---

# Status Icons

Success

Warning

Error

Information

Pending

Completed

Running

Paused

Stopped

Offline

Online

Locked

Unlocked

---

# File Icons

PDF

Excel

Word

Image

Video

Archive

CSV

Text

Barcode

QR Code

---

# Manufacturing Icons

Material

Warehouse

Pallet

Batch

Machine

Production Line

Tool

Maintenance

Inspection

Quality

Packaging

Shipment

Truck

Factory

Forklift

---

# AI Icons

AI Assistant

Chat

Prompt

Suggestion

Knowledge

Automation

Prediction

Digital Twin

---

# Dashboard Icons

KPI

Chart

Trend

Performance

Efficiency

OEE

Energy

Inventory

Orders

Revenue

---

# Interactive States

Default

Hover

Pressed

Focused

Disabled

Selected

Active

Icons should respond consistently.

---

# Accessibility

Icons must not be the only source of information.

Every action icon requires:

Visible Label

or

Accessible Label

Decorative icons

aria-hidden="true"

Interactive icons

aria-label required

---

# Responsive Behaviour

Desktop

Standard sizes

Tablet

Standard sizes

Mobile

Minimum 24 px

Touch targets

Minimum

44 × 44 px

---

# Empty States

Large icons

48–64 px

Combined with

Title

Description

Primary Action

---

# Error States

Error icon

Title

Explanation

Recovery Action

---

# Loading States

Spinner

Skeleton

Progress

Avoid decorative animations.

---

# Disabled Icons

Opacity

40%

No hover effect

No pointer cursor

---

# Performance

Use SVG icons.

Avoid PNG icons.

Avoid Icon Fonts.

Lazy load large icon collections.

---

# Naming Convention

Use PascalCase.

Examples

Add

Edit

Delete

Warehouse

Production

Settings

Notification

---

# Best Practices

✓ Use Lucide icons.

✓ Keep icon size consistent.

✓ Use semantic colors only.

✓ Pair icons with text.

✓ Optimize SVG assets.

---

# Do

✓ Use SVG

✓ Use accessible labels

✓ Use consistent sizing

✓ Keep icons simple

✓ Follow semantic meaning

---

# Don't

✗ Mix icon libraries

✗ Use decorative icons excessively

✗ Replace text with icons

✗ Stretch icons

✗ Apply random colors

---

# Acceptance Criteria

All icons use the approved icon library.

Icons follow size standards.

Interactive icons include accessible labels.

No bitmap icons are used.

Icons remain sharp on all screen sizes.

Color usage follows semantic rules.

Accessibility requirements are satisfied.

---

# Related Documents

Colors.md

Color_Tokens.md

Typography.md

Buttons.md

Navigation.md

Sidebar.md

Accessibility.md

Design_Tokens.md
