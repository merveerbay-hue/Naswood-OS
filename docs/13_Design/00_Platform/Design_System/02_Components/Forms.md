# Forms

**Module:** Design System

**Category:** Components

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Form component provides a standardized structure for collecting, validating, editing and displaying business data across Naswood OS.

Forms must provide a consistent, efficient and accessible experience while supporting complex enterprise workflows.

All modules must use the official Form component.

---

# Objectives

- Consistent Data Entry
- Enterprise User Experience
- Accessibility Compliance
- Validation Standards
- Responsive Layout
- Reusable Components
- High Productivity

---

# Design Principles

Forms should be

- Simple
- Predictable
- Structured
- Accessible
- Responsive

Users should complete forms with minimal effort.

---

# Form Types

Create Form

Edit Form

View Form

Wizard Form

Search Form

Filter Form

Approval Form

Settings Form

Import Form

AI Assisted Form

---

# Standard Layout

```
Page Header

↓

Toolbar

↓

General Information

↓

Business Sections

↓

Attachments

↓

Notes

↓

Audit Information

↓

Actions
```

---

# Form Structure

```
Form

├── Header

├── Sections

│     ├── Fields

│     ├── Groups

│     └── Validation

├── Attachments

├── Notes

└── Footer Actions
```

---

# Form Header

Contains

Title

Subtitle

Status

Reference Number

Created Date

Modified Date

Owner

---

# Form Sections

Sections divide large forms into logical groups.

Examples

General Information

Address

Contact

Financial Information

Production Information

Quality Information

Attachments

Notes

---

# Field Types

Text

Textarea

Number

Currency

Percentage

Date

Time

DateTime

Checkbox

Radio

Switch

Select

Multi Select

Autocomplete

Lookup

File Upload

Image Upload

Barcode

QR Code

Signature

Rich Text

---

# Field Layout

Desktop

Two Columns

Tablet

Two Columns

Mobile

Single Column

---

# Labels

Always visible.

Required fields display

*

Help text appears below the field.

---

# Required Fields

Must be clearly identified.

Validation occurs before submission.

---

# Placeholder

Used only as examples.

Never replace labels.

---

# Help Text

Used for

Descriptions

Examples

Business Rules

Hints

---

# Validation

Supports

Required

Minimum

Maximum

Length

Pattern

Unique

Business Rules

Cross Field Validation

Server Validation

---

# Validation Behaviour

Real-time

On Blur

On Submit

Server Response

---

# Error Messages

Displayed below field.

Must explain

Problem

Solution

Examples

Material Code already exists.

Warehouse is required.

Quantity must be greater than zero.

---

# Success Messages

Displayed after save.

Examples

Material saved successfully.

Purchase Order created.

Inventory updated.

---

# Read Only Mode

Fields remain visible.

Editing disabled.

Actions hidden according to permissions.

---

# Disabled Fields

Used when

Permission denied

Workflow restriction

Calculated value

---

# Section Collapse

Supported

Expand

Collapse

Remember last state.

---

# Attachments

Supported

Documents

Images

CAD Files

Certificates

Reference

File_Upload.md

---

# Notes

Supports

Plain Text

Rich Text

Mentions

History

---

# Audit Information

Displays

Created By

Created Date

Modified By

Modified Date

Version

---

# Footer Actions

Primary

Save

Secondary

Cancel

Optional

Save & New

Save & Close

Delete

Print

Export

---

# Auto Save

Optional

Configurable

Draft Mode supported.

---

# Keyboard Navigation

Tab

Shift + Tab

Enter

Escape

Arrow Keys

---

# Accessibility

Supports

Keyboard Navigation

ARIA Labels

Screen Readers

Focus Indicators

High Contrast

---

# Responsive Behaviour

Desktop

Two Columns

Tablet

Adaptive Layout

Mobile

Single Column

Sticky Footer Actions

---

# Performance

Lazy load lookup data.

Debounce searches.

Virtualize large dropdowns.

Avoid unnecessary re-renders.

---

# Security

Permission Based Fields

Read Only Mode

Sensitive Data Masking

Audit Logging

---

# React API

```tsx
<Form
    mode="create"
    onSubmit={handleSubmit}
    validationSchema={schema}
>

    <FormSection title="General Information">

        <TextField name="code" />

        <TextField name="description" />

    </FormSection>

</Form>
```

---

# Events

onSubmit

onCancel

onChange

onValidate

onSave

onDelete

onReset

---

# Best Practices

✓ Group related fields.

✓ Keep forms short.

✓ Validate early.

✓ Use meaningful labels.

✓ Display clear errors.

✓ Save drafts when appropriate.

---

# Do

✓ Organize fields into sections

✓ Use visible labels

✓ Show validation messages

✓ Support keyboard navigation

✓ Keep primary action visible

---

# Don't

✗ Use placeholder instead of labels

✗ Display all fields on one screen

✗ Hide validation errors

✗ Require unnecessary information

✗ Use more than one primary action

---

# Acceptance Criteria

Forms use the official layout.

Validation works correctly.

Accessibility passes WCAG 2.1 AA.

Responsive layout functions properly.

Required fields are clearly indicated.

Audit information is displayed.

Attachments integrate with File Upload.

---

# Related Documents

Buttons.md

Inputs.md

Dialogs.md

File_Upload.md

Typography.md

Spacing.md

Accessibility.md

Design_Tokens.md

Data_Grid.md
