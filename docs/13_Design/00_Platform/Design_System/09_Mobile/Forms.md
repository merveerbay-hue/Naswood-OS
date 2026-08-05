# Mobile Forms

**Module:** Design System

**Category:** Mobile

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Mobile Forms standard defines how forms are designed, validated and completed on smartphones and tablets within Naswood OS.

Unlike desktop forms, mobile forms prioritize speed, touch interaction and field usability for warehouse, production and field operations.

All mobile forms must follow the official Mobile Forms standard.

---

# Objectives

- Mobile First Data Entry
- Touch Optimized Interaction
- Minimize User Input
- Improve Data Accuracy
- Support Offline Operations
- Accessibility Compliance

---

# Design Principles

Mobile forms should be

- Simple

- Fast

- Touch Friendly

- Error Resistant

- Context Aware

Users should complete tasks with the minimum number of interactions.

---

# Standard Layout

```
Header

↓

Progress (Optional)

↓

Form Sections

↓

Input Fields

↓

Attachments

↓

Validation

↓

Primary Action
```

---

# Header

Displays

Page Title

Back Button

Save Draft

Progress

Help

---

# Form Sections

Examples

General Information

Product Details

Inventory

Production

Quality

Attachments

Notes

Approval

---

# Input Components

Supported

Text

Number

Currency

Date

Time

Dropdown

Autocomplete

Search

Checkbox

Radio

Switch

Slider

Textarea

Barcode

QR Code

Signature

Photo

Location

---

# Mobile Input Rules

Maximum

One primary input focus.

Avoid multiple columns.

Display one logical section at a time.

---

# Keyboard Optimization

Text Keyboard

Numeric Keyboard

Email Keyboard

Phone Keyboard

Decimal Keyboard

Auto-capitalization configurable.

---

# Barcode Integration

Supports

Barcode Scanner

QR Scanner

Camera Scanner

Bluetooth Scanner

Automatic field population.

---

# Camera Integration

Supports

Take Photo

Upload Image

Document Capture

Damage Report

Inspection Photo

---

# Voice Input

Future Support

Speech to Text

Voice Commands

AI Dictation

---

# Attachments

Supports

Images

PDF

Documents

Video (Future)

Audio (Future)

---

# Validation

Real-Time Validation

Required Fields

Business Rules

Duplicate Detection

Offline Validation

---

# Error Messages

Should be

Clear

Short

Actionable

Displayed near the related field.

---

# Progress Indicator

Supported

Step Wizard

Completion Percentage

Required Fields Remaining

---

# Form States

Loading

Editing

Saving

Offline

Submitting

Completed

Error

Read Only

---

# Offline Mode

Supports

Local Draft

Auto Save

Synchronization Queue

Conflict Resolution

Offline Indicator

---

# Auto Save

Save

Every change

Configurable interval

Manual Save

---

# AI Assistance

Supports

Auto Fill

Suggested Values

Validation Assistance

Translation

Summarization

Reference

AI_Copilot.md

---

# Search

Supports

Global Search

Material Search

Supplier Search

Customer Search

Barcode Search

QR Search

---

# Navigation

Next Field

Previous Field

Section Navigation

Bottom Navigation

Reference

Navigation.md

---

# Responsive Behaviour

Phone

Single Column

Tablet

Adaptive Layout

Landscape

Optimized Width

Foldable

Adaptive Layout

---

# Accessibility

Supports

Screen Readers

Large Touch Targets

High Contrast

Keyboard Navigation

Voice Control

Minimum touch target

44 × 44 px

---

# Performance

Lazy Loading

Local Caching

Background Synchronization

Image Compression

Fast Validation

---

# Security

Forms respect

Role Permissions

Field Permissions

Record Permissions

Offline Security Policies

Sensitive Data Masking

---

# React Structure

```tsx
<MobileForm>

    <FormHeader />

    <FormSection />

    <InputField />

    <AttachmentSection />

    <ValidationSummary />

    <PrimaryButton />

</MobileForm>
```

---

# Example Forms

Inventory Adjustment

Goods Receipt

Goods Issue

Purchase Request

Production Order

Quality Inspection

Maintenance Work Order

Expense Entry

Approval Form

Visitor Registration

---

# Best Practices

✓ Use one-column layouts.

✓ Minimize typing.

✓ Prefer scanning over manual entry.

✓ Enable auto-save.

✓ Support offline work.

✓ Use clear validation.

---

# Do

✓ Use barcode scanning

✓ Use large touch targets

✓ Keep forms short

✓ Group related fields

✓ Auto-save drafts

---

# Don't

✗ Use desktop layouts

✗ Require excessive typing

✗ Depend on hover interactions

✗ Hide validation errors

✗ Use multi-column forms on phones

---

# Acceptance Criteria

Forms follow the official mobile layout.

Touch interactions work consistently.

Offline mode functions correctly.

Barcode integration is supported.

Validation behaves correctly.

Accessibility complies with WCAG 2.1 AA.

Performance remains smooth on supported devices.

---

# Related Documents

Forms.md

Cards.md

Navigation.md

Responsive.md

AI_Copilot.md

File_Upload.md

Accessibility.md

Design_Tokens.md
