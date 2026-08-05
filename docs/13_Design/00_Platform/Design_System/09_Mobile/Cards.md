# Mobile Cards

**Module:** Design System

**Category:** Mobile

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Mobile Card standard defines how card components are presented and interacted with on mobile devices within Naswood OS.

Cards are the primary container for presenting business information on smartphones and tablets.

All mobile interfaces must use the official Mobile Card component.

---

# Objectives

- Mobile First Design
- Touch Friendly Interface
- Consistent Information Hierarchy
- Fast Data Consumption
- Responsive Layout
- Accessibility Compliance

---

# Design Principles

Mobile cards should be

- Compact
- Readable
- Touch Optimized
- Action Focused
- Lightweight

Cards should minimize scrolling while keeping critical information visible.

---

# Standard Structure

```
Mobile Card

├── Header

├── Main Content

├── Metadata

├── Status

├── Quick Actions

└── Footer
```

---

# Header

Displays

Icon

Title

Subtitle

Status Badge

Overflow Menu

---

# Content

Displays

Primary Value

Description

Important Metrics

Progress

Thumbnail (Optional)

---

# Metadata

Displays

Date

Owner

Department

Priority

Reference Number

---

# Status

Supports

Success

Warning

Critical

Pending

Completed

Draft

Reference

Color_Tokens.md

---

# Footer

Displays

Primary Action

Secondary Action

Last Updated

---

# Card Sizes

Compact

Regular

Expanded

Fullscreen (Optional)

---

# Card Variants

Summary Card

Detail Card

KPI Card

Task Card

Notification Card

Approval Card

Document Card

Order Card

AI Card

Production Card

Inventory Card

---

# Mobile Interactions

Tap

Long Press

Swipe Left

Swipe Right

Pull to Refresh

Expand

Collapse

Drag (Optional)

---

# Swipe Actions

Left

Archive

Delete

Dismiss

Right

Approve

Favorite

Pin

Custom Action

---

# Quick Actions

Supported

View

Edit

Approve

Reject

Share

Download

Call

Navigate

---

# Expandable Cards

Expandable cards may display

Additional Details

History

Attachments

Comments

Timeline

Related Records

---

# Card Layout Rules

One primary action only.

Maximum two secondary actions.

Avoid nested cards.

Keep touch targets at least 44 × 44 px.

---

# Card States

Loading

Ready

Refreshing

Empty

Offline

Error

Disabled

---

# Empty State

Illustration

Title

Description

Primary Action

Reference

Illustrations.md

---

# Loading State

Skeleton Card

Placeholder Content

Loading Indicator

---

# Error State

Error Message

Retry Action

Help Link

---

# Responsive Behaviour

Phone

Single Column

Tablet

Two Columns

Foldable

Adaptive Layout

Landscape

Grid Layout

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

High Contrast

Large Text

Focus Indicators

Minimum Touch Target

44 × 44 px

---

# Performance

Lazy Loading

Virtual Lists

Image Optimization

Caching

Smooth Scrolling

---

# Security

Cards respect

Role Permissions

Module Permissions

Record Permissions

Sensitive Data Masking

---

# React Structure

```tsx
<MobileCard>

    <CardHeader />

    <CardContent />

    <CardMetadata />

    <CardActions />

</MobileCard>
```

---

# Example Cards

Inventory Summary

Production Order

Purchase Request

Maintenance Task

Quality Inspection

Approval Request

AI Recommendation

Notification

Document Preview

Employee Card

---

# Best Practices

✓ Show only essential information.

✓ Keep actions simple.

✓ Prioritize touch interactions.

✓ Support offline viewing.

✓ Use semantic status colors.

✓ Maintain consistent spacing.

---

# Do

✓ Use one-column layouts

✓ Optimize for thumb reach

✓ Show clear status

✓ Keep actions visible

✓ Support swipe gestures

---

# Don't

✗ Overload cards

✗ Use tiny buttons

✗ Nest multiple cards

✗ Hide critical actions

✗ Depend on hover interactions

---

# Acceptance Criteria

Cards follow the official mobile layout.

Touch interactions work consistently.

Responsive behavior adapts correctly.

Accessibility complies with WCAG 2.1 AA.

Performance remains smooth on supported devices.

Permissions are enforced correctly.

---

# Related Documents

Cards.md

Responsive.md

Dashboard.md

KPIs.md

KPI_Cards.md

Notifications.md

Color_Tokens.md

Spacing.md

Accessibility.md
