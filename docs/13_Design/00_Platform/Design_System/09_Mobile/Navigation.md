# Mobile Navigation

**Module:** Design System

**Category:** Mobile

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Mobile Navigation standard defines how users navigate throughout Naswood OS on smartphones and tablets.

Navigation must prioritize speed, simplicity and operational efficiency for manufacturing, warehouse and field personnel.

The goal is to minimize taps while maximizing productivity.

---

# Objectives

- Mobile First Navigation
- Fast Task Completion
- Touch Optimized UX
- Context Awareness
- Consistent Navigation Patterns
- Accessibility Compliance

---

# Design Principles

Navigation should be

- Simple

- Predictable

- Fast

- Consistent

- Context Aware

Users should always know

Where they are

Where they can go

How to return

---

# Navigation Hierarchy

```
Application

↓

Workspace

↓

Module

↓

List

↓

Detail

↓

Action
```

---

# Navigation Components

Bottom Navigation

Top App Bar

Drawer Navigation

Context Menu

Tabs

Bottom Sheet

FAB (Floating Action Button)

Breadcrumb (Tablet Only)

Quick Actions

Search

---

# Bottom Navigation

Primary navigation method.

Maximum

5 Items

Recommended

Dashboard

Tasks

Search

AI

Profile

---

# Top App Bar

Displays

Back Button

Title

Search

Notifications

Overflow Menu

---

# Navigation Drawer

Used for

Module Selection

Settings

Administration

Profile

Help

Logout

Drawer should not replace Bottom Navigation.

---

# Tabs

Used for

Detail Pages

Settings

Reports

Analytics

AI

Maximum

5 visible tabs

---

# Floating Action Button

Supports

Create Record

Scan Barcode

Open AI

Start Inspection

Quick Approval

One primary action per screen.

---

# Bottom Sheet

Used for

Quick Actions

Filters

Sorting

Context Menu

Attachments

AI Suggestions

---

# Search

Supports

Global Search

Barcode Search

QR Search

Material Search

Customer Search

Supplier Search

AI Search

---

# Context Menu

Supports

View

Edit

Delete

Share

Export

Duplicate

History

Permissions

---

# Navigation Patterns

List

↓

Detail

↓

Edit

↓

Save

↓

Return

---

Dashboard

↓

Module

↓

Record

↓

Action

↓

Confirmation

---

# Quick Actions

Examples

Scan Barcode

Create Order

Approve

Reject

Take Photo

Upload File

Open AI

Navigate to Machine

---

# Deep Linking

Supports

Push Notifications

QR Codes

Barcodes

Email Links

Reports

AI Recommendations

---

# Notifications

Navigate directly to

Record

Task

Approval

Alert

Machine

Document

---

# Gesture Navigation

Supports

Swipe Back

Pull to Refresh

Swipe Actions

Long Press

Pinch to Zoom

Tap

Double Tap (Optional)

---

# Navigation States

Default

Active

Selected

Disabled

Loading

Offline

---

# Offline Navigation

Supports

Cached Pages

Recently Opened Records

Offline Tasks

Pending Synchronization

Limited Navigation

---

# Role Based Navigation

Examples

Production Operator

Warehouse Operator

Maintenance Technician

Quality Inspector

Sales

Executive

Administrator

Each role receives a simplified navigation.

---

# AI Navigation

AI can navigate users directly to

Recommended Records

Reports

Production Orders

Purchase Orders

Inventory

Knowledge Articles

Reference

AI_Copilot.md

---

# Camera Integration

Navigation via

Barcode

QR Code

Image Recognition (Future)

---

# Responsive Behaviour

Phone

Bottom Navigation

Tablet

Navigation Rail

Drawer

Landscape

Adaptive Navigation

Foldable

Adaptive Layout

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

Large Touch Targets

High Contrast

Voice Control

Minimum touch target

44 × 44 px

---

# Performance

Cached Navigation

Preloaded Modules

Lazy Navigation

Smooth Transitions

Background Loading

---

# Security

Navigation respects

Role Permissions

Module Permissions

Record Permissions

Offline Policies

Sensitive Module Restrictions

---

# React Structure

```tsx
<MobileNavigation>

    <TopBar />

    <BottomNavigation />

    <NavigationDrawer />

    <QuickActions />

</MobileNavigation>
```

---

# Example Navigation Flows

Inventory

Dashboard

↓

Inventory

↓

Material

↓

Stock Detail

↓

Transfer

---

Production

Dashboard

↓

Production

↓

Order

↓

Machine

↓

Complete

---

Maintenance

Dashboard

↓

Work Order

↓

Machine

↓

Inspection

↓

Close

---

Quality

Dashboard

↓

Inspection

↓

Defect

↓

Photo

↓

Submit

---

# Best Practices

✓ Keep navigation shallow.

✓ Prioritize one-handed use.

✓ Support barcode navigation.

✓ Display only relevant modules.

✓ Minimize taps.

✓ Maintain consistency.

---

# Do

✓ Use Bottom Navigation

✓ Support swipe gestures

✓ Keep labels short

✓ Provide quick actions

✓ Remember last location

---

# Don't

✗ Copy desktop navigation

✗ Use deep menu hierarchies

✗ Hide important actions

✗ Depend on hover interactions

✗ Require excessive scrolling

---

# Acceptance Criteria

Navigation follows the official mobile standard.

Touch interactions work consistently.

Role-based navigation functions correctly.

Offline navigation is supported.

Accessibility complies with WCAG 2.1 AA.

Performance remains responsive.

---

# Related Documents

Application_Shell.md

Sidebar.md

Navigation.md

Responsive.md

Dashboard.md

Cards.md

Forms.md

AI_Copilot.md

Accessibility.md

Design_Tokens.md
