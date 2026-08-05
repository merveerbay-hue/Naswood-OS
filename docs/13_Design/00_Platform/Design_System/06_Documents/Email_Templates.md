# Email Templates

**Module:** Design System

**Category:** Documents

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Email Template standard defines the structure, branding and presentation of all system-generated emails in Naswood OS.

Emails must be consistent, responsive, secure and recognizable while maintaining the Naswood corporate identity.

---

# Objectives

- Consistent Corporate Branding
- Responsive Email Design
- Readability
- Accessibility
- Multi-language Support
- Secure Communication

---

# Design Principles

Email templates should be

- Clean
- Professional
- Minimal
- Responsive
- Accessible

Emails should communicate essential information without unnecessary visual complexity.

---

# Email Categories

Authentication

Password Reset

Welcome

User Invitation

Approval Request

Approval Result

Purchase Notifications

Sales Notifications

Production Notifications

Quality Notifications

Maintenance Notifications

Workflow Notifications

AI Notifications

System Alerts

Reports

Scheduled Reports

---

# Standard Layout

```
Header

↓

Logo

↓

Title

↓

Body

↓

Primary Action

↓

Secondary Information

↓

Footer
```

---

# Header

Displays

Naswood Logo

Environment Badge (Optional)

Company Name

---

# Email Body

Contains

Greeting

Main Message

Business Context

Reference Information

Action Instructions

---

# Primary Action

Supported

Open Record

Approve

Reject

Reset Password

Verify Email

Download Report

View Dashboard

Open AI Recommendation

---

# Secondary Information

Displays

Reference Number

Status

Date

Time

Responsible User

Module

Priority

---

# Footer

Displays

Company Information

Support Contact

Privacy Notice

Copyright

System Version (Optional)

---

# Branding

Logo

Required

Corporate Colors

Required

Typography

Reference

Typography.md

Icons

Reference

Icons.md

---

# Email Types

Plain Text

HTML

Multipart

Responsive HTML

---

# Responsive Behaviour

Desktop

Standard Width

Tablet

Responsive Width

Mobile

Single Column

Minimum Width

320 px

Maximum Width

640 px

---

# Theme

Corporate Theme

Light Theme

Dark Mode Compatible

---

# Images

Hosted Securely

Lazy Loading Not Required

Alternative Text Required

---

# Buttons

Primary Button

Secondary Button

Text Link

Reference

Buttons.md

---

# Tables

Supported

Summary Tables

Invoice Tables

Order Tables

Report Tables

Reference

Tables.md

---

# Attachments

Supported

PDF

Excel

CSV

Images

Reports

Certificates

---

# Dynamic Content

Supports

User Name

Order Number

Supplier

Customer

Status

Due Date

Links

Localized Text

---

# Localization

Supports

Turkish

English

Future Languages

Date

Currency

Number Formatting

Localized automatically.

---

# Accessibility

Supports

Semantic HTML

Screen Readers

Alternative Text

Sufficient Contrast

Keyboard Navigation

Readable Font Size

---

# Security

Secure Links

Expiring Tokens

Signed URLs

Sensitive Data Masking

No confidential data in subject lines

---

# Performance

Optimized HTML

Compressed Images

Minimal CSS

Fast Rendering

---

# Subject Line Standards

Examples

Purchase Order Approved

Production Order Completed

Inventory Alert

Maintenance Reminder

Password Reset Request

AI Recommendation Available

---

# Naming Convention

EMAIL_<MODULE>_<EVENT>

Examples

EMAIL_PURCHASE_APPROVED

EMAIL_PRODUCTION_COMPLETED

EMAIL_PASSWORD_RESET

EMAIL_AI_RECOMMENDATION

---

# React Email Structure

```tsx
<EmailLayout>

    <EmailHeader />

    <EmailBody />

    <PrimaryButton />

    <EmailFooter />

</EmailLayout>
```

---

# Best Practices

✓ Keep subject lines concise.

✓ Display one primary action.

✓ Use corporate branding.

✓ Optimize for mobile.

✓ Include reference numbers.

✓ Support localization.

---

# Do

✓ Display company logo

✓ Use responsive layout

✓ Show important information first

✓ Include clear action button

✓ Display contact information

---

# Don't

✗ Embed large images

✗ Use multiple primary buttons

✗ Expose confidential data

✗ Use inconsistent branding

✗ Overload the email

---

# Acceptance Criteria

Emails follow the official template.

Responsive layout works correctly.

Corporate branding is consistent.

Accessibility complies with WCAG 2.1 AA.

Localization functions correctly.

Security standards are applied.

---

# Related Documents

PDF.md

Print.md

Typography.md

Color_Tokens.md

Buttons.md

Icons.md

Tables.md

Accessibility.md

Design_Tokens.md
