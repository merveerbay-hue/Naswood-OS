# Currency

**Module:** Shared

**Category:** Currency Management

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Currency standard defines how monetary values, exchange rates and multi-currency transactions are managed throughout Naswood OS.

The objective is to ensure financial consistency, international compatibility and accurate reporting across all business modules.

Currency management is a shared platform service.

---

# Objectives

- Multi-Currency Support
- Accurate Financial Reporting
- Consistent Exchange Rates
- International Business Support
- Financial Traceability
- Regulatory Compliance

---

# Design Principles

Currency handling should be

Consistent

Accurate

Transparent

Auditable

Configurable

Every monetary value must always include its currency.

---

# Supported Currency Model

Base Currency

↓

Transaction Currency

↓

Reporting Currency

↓

Presentation Currency

---

# Currency Types

Base Currency

Transaction Currency

Accounting Currency

Reporting Currency

Display Currency

Future Currency

Historical Currency

---

# Supported Modules

Purchasing

Sales

Inventory Valuation

Finance

Accounting

CRM

Projects

Costing

Manufacturing

Reporting

Dashboard

AI

---

# Currency Record

Currency Code

Currency Name

Symbol

ISO Code

Decimal Precision

Minor Unit

Status

Effective Date

---

# ISO Standard

Use

ISO 4217

Examples

TRY

USD

EUR

GBP

CHF

JPY

SAR

AED

---

# Monetary Fields

Every monetary value includes

Amount

Currency

Exchange Rate

Rate Date

Converted Amount

Base Currency Amount

---

# Exchange Rate Types

Buying

Selling

Average

Central Bank

Custom

Corporate

Historical

---

# Exchange Rate Sources

Central Bank

Corporate Rate

Manual Entry

External API

Custom Provider

Reference

API_Standards.md

---

# Rate Validity

Supports

Daily

Weekly

Monthly

Manual

Historical

Future Effective Date

---

# Currency Conversion

Supports

Real-Time Conversion

Historical Conversion

Batch Conversion

Manual Override

Audit Tracking

---

# Rounding Rules

Configurable

Currency Precision

Tax Precision

Accounting Precision

Display Precision

Rounding Method

Half Up

Half Even

Floor

Ceiling

---

# Decimal Precision

Examples

TRY

2

USD

2

EUR

2

JPY

0

Configurable for custom currencies.

---

# Currency Formatting

Display

Symbol

Amount

ISO Code (Optional)

Examples

₺ 1,250.00

$ 250.50

€ 980.75

---

# Historical Rates

Historical exchange rates are immutable.

Transactions always retain the rate used at posting time.

---

# Exchange Rate Update

Supports

Automatic Import

Manual Update

Approval Workflow

Scheduled Refresh

Validation

---

# Financial Transactions

Every transaction stores

Transaction Currency

Exchange Rate

Base Currency

Converted Amount

Rate Source

Rate Timestamp

---

# Reporting

Supports

Base Currency

Original Currency

Multi-Currency Reports

Currency Comparison

Exchange Gain/Loss

---

# Costing

Supports

Material Cost

Purchase Cost

Production Cost

Transportation Cost

Currency Conversion

---

# Tax

Tax calculations use

Transaction Currency

Configured Tax Rules

Jurisdiction Rules

---

# Dashboards

Supports

Currency Selector

Multi-Currency KPIs

Financial Summary

Exchange Rate Widget

Reference

Dashboard.md

---

# AI Support

AI may

Explain exchange rate impacts

Recommend purchasing timing

Forecast currency risks

Compare supplier prices

Analyze historical trends

Reference

AI_Copilot.md

---

# Validation Rules

Currency Code is mandatory.

Exchange Rate must be positive.

Historical rates cannot be modified after posting.

Transactions require a valid exchange rate.

---

# Security

Supports

Role-Based Permissions

Exchange Rate Approval

Audit Trail

Change History

Rate Locking

---

# Audit

Track

Rate Changes

Source

Approval

Manual Overrides

Conversion History

Reference

Audit_Log.md

---

# API

Example Endpoints

```
GET /currencies

GET /currencies/{code}

GET /exchange-rates

POST /exchange-rates

GET /exchange-rates/history

POST /currency/convert
```

---

# User Interface

Displays

Currency Selector

Exchange Rate

Rate Date

Converted Amount

Original Amount

Rate Source

---

# Example Transaction

Currency

USD

Amount

15,000.00

Exchange Rate

41.27

Base Currency

TRY

Converted Amount

619,050.00

Rate Date

2026-08-05

---

# Performance

Supports

Cached Exchange Rates

Background Updates

Historical Lookup Optimization

Read-Only Rate Cache

---

# Best Practices

✓ Always store original currency.

✓ Preserve historical exchange rates.

✓ Use ISO currency codes.

✓ Audit exchange rate changes.

✓ Separate display and accounting currencies.

✓ Validate conversion before posting.

---

# Do

✓ Store exchange rate with every transaction

✓ Keep historical rates immutable

✓ Use configurable precision

✓ Support multiple rate providers

✓ Audit manual overrides

---

# Don't

✗ Overwrite historical rates

✗ Store amounts without currency

✗ Round inconsistently

✗ Mix display and accounting currencies

✗ Allow unauthorized rate changes

---

# Acceptance Criteria

Multi-currency transactions are supported.

Exchange rates are auditable.

Historical values remain immutable.

Financial reports support multiple currencies.

Currency formatting is consistent.

Security and permissions are enforced.

---

# Related Documents

API_Standards.md

Audit_Log.md

Authentication.md

Authorization.md

Dashboard.md

Material.md

Approval_Workflow.md

Architecture.md
