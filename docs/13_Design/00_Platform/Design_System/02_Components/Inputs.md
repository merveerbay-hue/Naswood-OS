# Inputs

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Input component provides a standardized way to collect user input across all Naswood OS modules.

Inputs must be consistent, accessible, validated and reusable.

Every form should use the official Input components.

---

# Objectives

- Consistent Data Entry
- Better User Experience
- Accessibility Compliance
- Validation Support
- Responsive Design
- Enterprise Standard

---

# Design Principles

Inputs should be

- Simple
- Predictable
- Accessible
- Responsive
- Consistent

Data entry should be fast and error-free.

---

# Supported Input Types

Text

Textarea

Password

Email

Phone

Number

Decimal

Currency

Percentage

Date

Time

DateTime

Search

URL

Barcode

QR Code

Color

File

Hidden

Read Only

Disabled

---

# Enterprise Input Components

Text Field

Text Area

Number Field

Currency Field

Percentage Field

Search Box

Date Picker

Time Picker

Date Time Picker

Select

Multi Select

Autocomplete

Lookup

Checkbox

Radio Button

Switch

Tag Input

Barcode Scanner

QR Scanner

File Upload

Rich Text Editor

Signature

---

# Standard Structure

```
Label

↓

Input

↓

Helper Text

↓

Validation Message
```

---

# Input Sizes

| Size | Height |
|--------|--------:|
| Small | 32 px |
| Medium | 40 px |
| Large | 48 px |

Default

Medium

---

# Width

Default

100%

Maximum

Container Width

Minimum

120 px

---

# Border Radius

6 px

Reference

Border_Radius.md

---

# Typography

Font

Inter

Weight

400

Reference

Typography.md

---

# Label

Always visible.

Position

Above Input

Required fields display

*

---

# Placeholder

Optional.

Examples only.

Never replace labels.

---

# Helper Text

Used for

Examples

Business Rules

Descriptions

Formatting

---

# States

Default

Hover

Focused

Filled

Disabled

Read Only

Loading

Error

Success

---

# Focus

Visible border.

Visible focus ring.

Keyboard accessible.

---

# Disabled

No interaction.

Readable text.

Reduced opacity.

---

# Read Only

Value visible.

Cannot be edited.

Can be copied.

---

# Validation

Supports

Required

Minimum

Maximum

Length

Regex

Unique

Business Rules

Server Validation

Cross Field Validation

---

# Validation Timing

Real Time

On Blur

On Submit

Server Response

---

# Error Messages

Displayed below input.

Examples

Required field.

Invalid email.

Material code already exists.

Quantity must be positive.

---

# Success State

Optional.

Green border.

Success icon.

---

# Prefix / Suffix

Supported

Currency

Percentage

Units

Icons

Buttons

---

# Clear Button

Optional.

Supported

Search

Text

Number

---

# Password

Show / Hide

Strength Indicator

Caps Lock Warning

Generate Password (Optional)

---

# Search

Instant Search

Debounced

Clear Button

Search Icon

---

# Number

Supports

Minimum

Maximum

Decimals

Step

Negative Values

Thousands Separator

---

# Currency

Localized Format

Currency Symbol

Decimal Precision

Auto Formatting

---

# Date

Calendar Picker

Keyboard Entry

Localization

Min / Max Date

---

# Select

Single Select

Searchable

Grouped Options

Virtual Scroll

---

# Multi Select

Search

Tags

Select All

Clear All

Maximum Selection

---

# Lookup

Server Search

Lazy Loading

Infinite Scroll

Keyboard Navigation

---

# Barcode

Keyboard Scanner

USB Scanner

Camera Scanner (Mobile)

Automatic Validation

---

# QR Code

Camera Support

USB Scanner

Mobile Support

---

# File Input

Integrated with

File_Upload.md

---

# Accessibility

Keyboard Navigation

Screen Readers

ARIA Labels

Focus Indicators

Minimum Height

40 px

Touch Target

44 × 44 px

---

# Responsive Behaviour

Desktop

Standard Width

Tablet

Responsive

Mobile

Full Width

---

# Performance

Debounce search.

Virtualize large option lists.

Lazy load lookup data.

Avoid unnecessary renders.

---

# React API

```tsx
<TextField
    name="materialCode"
    label="Material Code"
    placeholder="Enter material code"
    required
/>

<NumberField
    name="quantity"
    min={0}
    step={1}
/>

<SelectField
    name="warehouse"
    options={warehouseOptions}
/>
```

---

# Events

onChange

onFocus

onBlur

onClear

onSearch

onSelect

onValidate

---

# Best Practices

✓ Always show labels.

✓ Use helper text.

✓ Validate early.

✓ Use correct input type.

✓ Keep fields aligned.

✓ Support keyboard users.

---

# Do

✓ Material Code

✓ Warehouse

✓ Quantity

✓ Unit Price

✓ Supplier

✓ Batch Number

---

# Don't

✗ Placeholder as label

✗ Tiny input fields

✗ Hidden validation

✗ Inconsistent widths

✗ Multiple input styles

---

# Acceptance Criteria

Inputs follow official styles.

Validation works correctly.

Accessibility passes WCAG 2.1 AA.

Keyboard navigation is supported.

Responsive layout functions correctly.

Error messages are clear.

Business validation is supported.

---

# Related Documents

Forms.md

Buttons.md

File_Upload.md

Dialogs.md

Typography.md

Spacing.md

Accessibility.md

Design_Tokens.md

Color_Tokens.md
