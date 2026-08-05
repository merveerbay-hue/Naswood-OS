# Print

**Module:** Design System

**Category:** Documents

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Print standard defines how business documents, reports and application pages are optimized for printing within Naswood OS.

Printed output must remain readable, professional and consistent regardless of printer type or paper size.

All printable content must follow the official Print standard.

---

# Objectives

- Standardize Printed Output
- Professional Print Layout
- Paper Optimization
- Corporate Branding
- Readability
- Accessibility

---

# Design Principles

Printed documents should be

- Clean

- Minimal

- Readable

- Consistent

- Printer Friendly

Printing should preserve business information while eliminating unnecessary interface elements.

---

# Printable Content

Supports

Reports

Purchase Orders

Sales Orders

Production Orders

Work Orders

Inventory Reports

Packing Lists

Labels

Invoices

Certificates

Quality Reports

Maintenance Reports

Executive Reports

Audit Reports

---

# Print Layout

```
Header

↓

Document Information

↓

Content

↓

Summary

↓

Footer
```

---

# Print Mode

Hide

Sidebar

Header Navigation

Buttons

Filters

Search Bars

Dialogs

Notifications

Tooltips

Floating Actions

Only business content should remain.

---

# Page Sizes

Supported

A4 Portrait

A4 Landscape

A3 Landscape

Letter

Legal

Custom

---

# Margins

Top

20 mm

Bottom

20 mm

Left

15 mm

Right

15 mm

---

# Orientation

Portrait

Landscape

Automatic

---

# Page Break Rules

Avoid breaking

Tables

Charts

Signatures

Images

QR Codes

Barcodes

Group related information together.

---

# Table Printing

Repeat table headers.

Avoid splitting rows.

Keep totals together.

Display page totals if necessary.

Reference

Tables.md

---

# Charts

Print as

Vector Graphics

High Resolution

Minimum

300 DPI

Reference

Standard_Charts.md

---

# Images

PNG

JPEG

SVG

Recommended

300 DPI

Maintain aspect ratio.

---

# Typography

Reference

Typography.md

Minimum Font Size

9 pt

Recommended

10–12 pt

---

# Colors

Black & White Compatible

Required

Corporate Colors

Optional

Avoid low contrast.

---

# Header

Displays

Company Logo

Document Title

Document Number

Revision

Date

---

# Footer

Displays

Page Number

Print Date

Printed By

Document Version

Company Information

---

# Watermarks

Supported

Draft

Approved

Cancelled

Confidential

Internal Use

---

# Barcode

Supported

Code128

EAN

GS1

Reference

Labels.md

---

# QR Code

Supported

Traceability

Document Verification

Production Tracking

Customer Portal

---

# Printing Options

Printer Selection

Copies

Duplex

Color

Black & White

Scaling

Paper Size

Orientation

---

# Browser Printing

Supports

Chrome

Edge

Firefox

Safari

Print CSS required.

---

# CSS Guidelines

Use

@media print

Hide

Navigation

Sidebar

Dialogs

Buttons

Forms

Tooltips

Notifications

Use page-break rules.

---

# React Structure

```tsx
<PrintableDocument>

    <PrintHeader />

    <PrintContent />

    <PrintFooter />

</PrintableDocument>
```

---

# Print Preview

Supports

Zoom

Orientation

Margins

Paper Size

Page Count

---

# Performance

Generate print layout lazily.

Optimize images.

Avoid unnecessary rendering.

Use vector graphics when possible.

---

# Accessibility

Supports

Readable Fonts

High Contrast

Tagged PDFs

Large QR Codes

Accessible Tables

---

# Security

Supports

Watermarks

Read Only

Print Permissions

Audit Logging

Print History

---

# Best Practices

✓ Remove navigation before printing.

✓ Repeat table headers.

✓ Preserve page margins.

✓ Use vector graphics.

✓ Display revision information.

✓ Print page numbers.

---

# Do

✓ Print only business content

✓ Optimize for A4

✓ Include document metadata

✓ Show page numbers

✓ Maintain branding

---

# Don't

✗ Print navigation menus

✗ Split signatures

✗ Break charts

✗ Stretch images

✗ Hide revision numbers

---

# Acceptance Criteria

Print layout follows official standards.

Navigation elements are hidden.

Page breaks are optimized.

Charts remain readable.

Tables repeat headers.

Corporate branding is preserved.

Accessibility requirements are met.

---

# Related Documents

PDF.md

Labels.md

Email_Templates.md

Tables.md

Standard_Charts.md

Typography.md

Design_Tokens.md

Accessibility.md
