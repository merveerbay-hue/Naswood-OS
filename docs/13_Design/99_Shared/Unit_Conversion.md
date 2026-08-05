# Unit Conversion

**Module:** Shared

**Category:** Unit Conversion

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Unit Conversion standard defines how values are converted between compatible measurement units throughout Naswood OS.

It ensures accurate, deterministic and traceable conversions across ERP, MES, WMS, Manufacturing, AI and Reporting.

All unit conversions must comply with this standard.

---

# Objectives

- Accurate Conversions
- Deterministic Results
- Consistent Precision
- Manufacturing Compatibility
- International Standards
- Business Rule Support

---

# Design Principles

Conversions should be

Deterministic

Traceable

Configurable

Precise

Auditable

Conversions must never modify the original stored value.

---

# Conversion Architecture

Value

↓

Source Unit

↓

Conversion Engine

↓

Target Unit

↓

Precision

↓

Display

---

# Supported Conversion Types

Length

Area

Volume

Mass

Density

Temperature

Pressure

Energy

Power

Time

Humidity

Wood Industry Units

Packaging Units

Business Units

---

# Standard Length Conversions

mm ↔ cm

mm ↔ m

cm ↔ m

m ↔ km

mm ↔ inch

inch ↔ ft

---

# Area

mm²

cm²

m²

ha

ft²

---

# Volume

cm³

dm³

m³

Liter

ml

ft³

Board Foot

---

# Mass

g

kg

ton

lb

oz

---

# Temperature

°C

°F

Kelvin

---

# Pressure

Pa

kPa

MPa

bar

psi

---

# Time

Milliseconds

Seconds

Minutes

Hours

Days

Weeks

---

# Conversion Engine

Supports

Direct Conversion

Derived Conversion

Lookup Tables

Business Rules

Formula-Based Conversion

---

# Conversion Factors

Example

```
1000 mm

↓

1 m
```

Factors must be centrally maintained.

---

# Precision

Default precision

Length

0.1 mm

Area

0.01 m²

Volume

0.001 m³

Weight

0.01 kg

Temperature

0.1 °C

Precision may be overridden by entity-specific rules.

---

# Rounding

Supports

Round Half Up

Round Half Even

Floor

Ceiling

Business Rule Rounding

The rounding strategy must be configurable by domain where necessary.

---

# Business Conversions

Supports

Purchase Unit

Inventory Unit

Production Unit

Sales Unit

Packaging Unit

Reference

Measurement_System.md

---

# Packaging Conversions

Examples

1 Pallet

=

24 Packages

1 Package

=

120 Pieces

Conversion factors are configurable.

---

# Wood Industry

Supports

m³ ↔ Board Foot

Running Meter ↔ Piece

Package ↔ Pallet

Lamella ↔ Panel

Log ↔ m³

Conversions that depend on product dimensions must use material-specific rules.

---

# Density-Based Conversion

Supports

Volume ↔ Weight

Requires

Material Density

Example

m³

↓

kg

using

Density

Reference

Material.md

---

# API

Example

```json
{
  "value": 2500,
  "unit": "mm",
  "convertTo": "m"
}
```

---

# AI

AI may

Convert engineering values

Validate conversions

Recommend preferred units

Reference

AI_Copilot.md

---

# Localization

Display units follow user preferences.

Reference

Localization.md

---

# Performance

Supports

Cached Conversion Tables

Immutable Factors

Fast Lookup

Reference

Performance.md

---

# Validation

Reject

Unknown Units

Invalid Quantities

Negative Values (where prohibited)

Incompatible Conversion Requests

Example

kg

↓

°C

❌

---

# Security

Conversion rules require administrative permission to modify.

Reference

Permission_Model.md

---

# Audit

Track

Conversion Rule Changes

Factor Updates

Business Rule Changes

Reference

Audit_Log.md

---

# Monitoring

Track

Conversion Errors

Most Used Conversions

Failed Requests

Reference

Monitoring.md

---

# Best Practices

✓ Store canonical units.

✓ Convert only when required.

✓ Keep conversion factors centralized.

✓ Validate compatible quantities.

✓ Preserve precision.

✓ Audit rule changes.

---

# Do

✓ Use SI internally

✓ Cache conversion tables

✓ Validate dimensions

✓ Use material-specific rules where necessary

✓ Document custom conversions

---

# Don't

✗ Convert incompatible units

✗ Hardcode conversion factors

✗ Round too early

✗ Store converted values as canonical

✗ Duplicate conversion logic

---

# Acceptance Criteria

Conversions are deterministic.

Precision is configurable.

Business unit conversions are supported.

Material-specific conversions are available.

Conversion rules are centrally managed.

Performance targets are achieved.

---

# Related Documents

Measurement_System.md

Material.md

Localization.md

Performance.md

Audit_Log.md

Permission_Model.md

AI_Copilot.md

API_Standards.md

Monitoring.md
