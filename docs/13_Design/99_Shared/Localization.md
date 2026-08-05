# Localization

**Module:** Shared

**Category:** Localization & Internationalization

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Localization standard defines how Naswood OS supports multiple languages, regions, cultural conventions and country-specific business requirements.

Localization ensures a consistent user experience across all supported countries while maintaining a single application architecture.

Internationalization (i18n) enables the platform to adapt to different locales without requiring code changes.

Localization (l10n) provides region-specific translations and formatting.

---

# Objectives

- Multi-Language Support
- Regional Formatting
- International Business Readiness
- Consistent User Experience
- Configurable Localization
- Regulatory Compliance

---

# Design Principles

Localization should be

Configurable

Consistent

Culture Aware

Extensible

Accessible

Business Safe

Business logic must never depend on translated text.

---

# Supported Languages

Primary

Turkish (tr-TR)

English (en-US)

Future

German (de-DE)

French (fr-FR)

Arabic (ar-SA)

Spanish (es-ES)

Italian (it-IT)

Russian (ru-RU)

Additional languages may be added without code changes.

---

# Locale Model

```
Language

↓

Region

↓

Timezone

↓

Currency

↓

Formatting
```

---

# Localization Scope

User Interface

Validation Messages

Notifications

Reports

PDF Documents

Email Templates

Labels

Dashboard

AI Responses

Help Content

---

# Translation Keys

Use key-based translations.

Example

```
inventory.material.name

inventory.stock.available

purchase.order.create

quality.inspection.status
```

Do not use UI text as translation keys.

---

# Translation Files

Example

```
tr-TR.json

en-US.json

de-DE.json
```

---

# Date Formatting

Use ISO 8601 internally.

Display according to user locale.

Examples

Turkey

05.08.2026

United States

08/05/2026

Germany

05.08.2026

---

# Time Formatting

Supports

24-Hour

12-Hour

Time Zone Conversion

UTC Storage

User Local Display

---

# Time Zones

All timestamps are stored in UTC.

Displayed according to the user's configured timezone.

Example

UTC

2026-08-05T10:00:00Z

User Display

05.08.2026 13:00

---

# Number Formatting

Supports

Decimal Separator

Thousands Separator

Locale-Specific Display

Examples

Turkey

1.250,50

United States

1,250.50

---

# Currency Formatting

Reference

Currency.md

Supports

ISO 4217

Currency Symbol

Localized Display

Multi-Currency

---

# Units of Measure

Supports

Metric

Imperial

Examples

mm

cm

m

kg

ton

m²

m³

ft

in

lb

Users may configure preferred display units where applicable.

---

# Address Formatting

Supports

Country-specific address formats.

Example fields

Country

Province / State

City

District

Postal Code

Street

Building

---

# Phone Numbers

Use

E.164 format internally.

Display according to locale.

Example

+90 532 123 45 67

---

# Calendar

Supports

Gregorian Calendar

Future support for additional calendars where required.

---

# Right-to-Left (RTL)

Future Support

Arabic

Hebrew

Layouts must be RTL compatible.

---

# Regional Settings

Supports

Language

Timezone

Date Format

Time Format

Number Format

Currency

Measurement Units

Paper Size

Week Start Day

---

# User Preferences

Each user may configure

Language

Timezone

Theme

Date Format

Time Format

Measurement Units

Currency Display

---

# Validation Messages

Localized

Field Names

Business Messages

Errors

Warnings

Hints

Reference

Error_Handling.md

---

# Notifications

Supports

Localized Push Notifications

Emails

SMS

System Messages

Reference

Notifications.md

---

# Reports

Supports

Localized Headers

Localized Numbers

Localized Dates

Localized Currency

Localized Templates

Reference

PDF.md

Print.md

---

# AI Localization

AI responses should

Respect user language

Use localized terminology

Display localized dates and currencies

Preserve business terminology

Reference

AI_Copilot.md

---

# API

APIs remain language-neutral.

Localization is requested using

```
Accept-Language
```

Example

```
Accept-Language: tr-TR
```

---

# Accessibility

Supports

Localized Screen Reader Labels

Localized ARIA Descriptions

Localized Keyboard Shortcuts (where applicable)

---

# Security

Localization must never affect

Permissions

Business Rules

Audit Records

Identifiers

Workflow Logic

---

# Performance

Supports

Translation Caching

Lazy Loading

Language Packs

Incremental Updates

---

# Testing

Verify

Translations

Layout Expansion

RTL Compatibility

Date Formatting

Number Formatting

Currency Formatting

Pluralization

---

# Best Practices

✓ Store all translations externally.

✓ Keep translation keys stable.

✓ Use UTC internally.

✓ Format data according to locale.

✓ Test with long translated strings.

✓ Separate localization from business logic.

---

# Do

✓ Use translation keys

✓ Store timestamps in UTC

✓ Support multiple locales

✓ Cache translation resources

✓ Localize user-facing messages

---

# Don't

✗ Hardcode UI text

✗ Store localized values in the database

✗ Translate business identifiers

✗ Depend on locale for business rules

✗ Mix formatting conventions

---

# Acceptance Criteria

Localization supports multiple languages.

Formatting follows user locale.

Translation keys are used consistently.

Business logic remains language-independent.

Reports and notifications are localized.

Accessibility requirements are satisfied.

---

# Related Documents

Currency.md

API_Standards.md

Error_Handling.md

Notifications.md

PDF.md

Print.md

AI_Copilot.md

Architecture.md

Authentication.md

User_Preferences.md
