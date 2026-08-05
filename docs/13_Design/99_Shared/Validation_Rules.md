# Validation Rules

**Module:** Shared

**Category:** Validation Framework

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Validation Rules standard defines how data integrity, business rules and domain constraints are validated throughout Naswood OS.

Validation ensures that only complete, accurate and authorized data enters the platform.

Validation must be consistent across Web, Mobile, API, AI, Imports and Integrations.

---

# Objectives

- Consistent Validation
- Data Integrity
- Business Rule Enforcement
- User Guidance
- API Consistency
- Domain Protection

---

# Validation Principles

Validation should be

Predictable

Reusable

Centralized

Auditable

Fast

User Friendly

Validation rules must be defined once and reused across all clients.

---

# Validation Architecture

```
Client

↓

UI Validation

↓

API Validation

↓

Application Validation

↓

Domain Validation

↓

Database Constraints
```

---

# Validation Categories

Input Validation

Business Validation

Domain Validation

Security Validation

Permission Validation

Workflow Validation

Integration Validation

AI Validation

File Validation

Import Validation

---

# Validation Layers

## UI

Required Fields

Formatting

Instant Feedback

Accessibility

---

## API

Schema Validation

Data Types

Length

Required Fields

Enums

Reference

API_Standards.md

---

## Domain

Business Invariants

State Validation

Lifecycle Rules

Reference

Status_Lifecycle.md

---

## Database

Primary Keys

Foreign Keys

Unique Constraints

Check Constraints

Reference

Entity_Rules.md

---

# Input Validation

Supports

Required

Maximum Length

Minimum Length

Regular Expressions

Allowed Characters

Data Types

---

# Data Types

String

Integer

Decimal

Boolean

Date

Time

DateTime

UUID

Currency

Measurement

Email

Phone

URL

---

# Business Validation

Examples

Purchase Order cannot be approved without lines.

Inventory cannot become negative.

Production cannot start without released status.

Invoice total must equal line totals.

---

# Workflow Validation

Supports

Approval Required

Status Validation

Transition Validation

Assignment Validation

Reference

Approval_Workflow.md

Status_Lifecycle.md

---

# Security Validation

Supports

Authorization

Permission Checks

Ownership Validation

Input Sanitization

Reference

Permission_Model.md

Security.md

---

# AI Validation

Supports

Prompt Validation

Output Validation

Hallucination Detection (where applicable)

Sensitive Data Detection

Reference

AI_Copilot.md

---

# File Validation

Supports

File Type

Maximum Size

Virus Scan

Duplicate Detection

Reference

File_Storage.md

---

# Import Validation

Supports

Required Columns

Duplicate Rows

Business Rules

Reference Integrity

Error Reporting

Rollback

---

# Measurement Validation

Supports

Compatible Units

Precision

Range

Reference

Measurement_System.md

Unit_Conversion.md

---

# Currency Validation

Supports

Currency Exists

Exchange Rate Availability

Precision

Reference

Currency.md

---

# Localization Validation

Supports

Date Format

Number Format

Regional Rules

Reference

Localization.md

---

# Error Messages

Validation messages should be

Clear

Localized

Actionable

Consistent

Reference

Error_Handling.md

---

# Validation Severity

Info

Warning

Error

Critical

Warnings may allow continuation.

Errors must block the operation.

---

# Cross-Field Validation

Examples

End Date ≥ Start Date

Quantity > 0

Width × Length = Area

Total = Sum(Lines)

---

# Cross-Entity Validation

Examples

Customer must exist.

Warehouse must belong to Company.

Material must be Active.

Machine must be Available.

---

# API Response Example

```json
{
  "errors": [
    {
      "field": "materialCode",
      "code": "REQUIRED",
      "message": "Material Code is required."
    }
  ]
}
```

---

# Monitoring

Track

Validation Failures

Most Common Errors

Import Errors

API Validation Failures

Reference

Monitoring.md

---

# Performance

Validation should

Complete before persistence

Avoid duplicate execution

Use cached reference data

---

# Accessibility

Validation messages support

Screen Readers

Keyboard Navigation

Color-independent feedback

---

# Audit

Track

Validation Rule Changes

Administrative Overrides

Reference

Audit_Log.md

---

# Best Practices

✓ Validate early.

✓ Validate again at the server.

✓ Centralize business rules.

✓ Reuse validation logic.

✓ Keep error messages understandable.

✓ Never trust client input.

---

# Do

✓ Validate every API request

✓ Reuse validators

✓ Keep rules deterministic

✓ Separate UI from business validation

✓ Localize messages

---

# Don't

✗ Trust client validation

✗ Duplicate rules

✗ Hardcode validation in UI

✗ Skip domain validation

✗ Expose internal exception messages

---

# Acceptance Criteria

Validation rules are centralized.

All clients reuse the same business rules.

Validation messages are localized.

Business invariants are enforced.

Performance targets are met.

Audit logging is available.

---

# Related Documents

API_Standards.md

Security.md

Permission_Model.md

Entity_Rules.md

Status_Lifecycle.md

Approval_Workflow.md

Measurement_System.md

Unit_Conversion.md

Error_Handling.md

Localization.md

Monitoring.md
