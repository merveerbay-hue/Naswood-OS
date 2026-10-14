# Measurement System

**Module:** Shared

**Category:** Measurement & Units

**Version:** 1.0

**Status:** Approved

**Material multi-UoM business engine (enter-once / all modules):** [`Measurement_Conversion_Engine.md`](./Measurement_Conversion_Engine.md) — compose; this document remains the SI / precision / measurable-value authority.

---

# Purpose

The Measurement System standard defines how physical quantities, engineering units, conversions and precision rules are represented throughout Naswood OS.

It ensures consistent calculations, reporting and interoperability across ERP, MES, WMS, AI, CAD and Digital Twin services.

All measurable values must comply with this standard.

---

# Objectives

- Standardized Units
- Accurate Calculations
- International Compatibility
- Engineering Consistency
- Manufacturing Precision
- AI Readiness

---

# Design Principles

Measurements should be

Consistent

Precise

Convertible

Culture Independent

Machine Readable

Every measured value consists of

Value

+

Unit

Units are never implied.

---

# Measurement Model

Quantity

↓

Value

↓

Unit

↓

Precision

↓

Display Format

---

# Standard System

Primary

SI (International System of Units)

Imperial display may be supported where required.

---

# Supported Quantities

Length

Area

Volume

Mass

Weight

Density

Temperature

Pressure

Force

Energy

Power

Speed

Time

Moisture

Humidity

Angle

Frequency

Electrical

---

# Length

Base Unit

millimeter (mm)

Supported

mm

cm

m

km

in

ft

---

# Area

Base Unit

square meter (m²)

Supported

mm²

cm²

m²

ha

ft²

---

# Volume

Base Unit

cubic meter (m³)

Supported

cm³

dm³

m³

L

ml

ft³

board foot

---

# Mass

Base Unit

kilogram (kg)

Supported

g

kg

ton

lb

oz

---

# Density

Supported

kg/m³

g/cm³

---

# Temperature

Base Unit

°C

Supported

°C

°F

K

---

# Pressure

Supported

Pa

kPa

MPa

bar

psi

---

# Force

Supported

N

kN

---

# Energy

Supported

J

kJ

MJ

kWh

---

# Power

Supported

W

kW

MW

---

# Time

Supported

ms

s

min

h

day

---

# Humidity

Supported

%

---

# Moisture Content

Supported

%

Used for

Logs

Lumber

Lamella

Thermowood

Finished Products

---

# Electrical

Supports

V

A

Hz

kWh

---

# Wood Industry Units

Supports

Piece

Bundle

Package

Pallet

Log

Board

Lamella

Panel

m³

Running Meter

Linear Meter

Board Foot

---

# Measurement Precision

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

Moisture

0.1 %

Precision should be configurable.

---

# Conversion Rules

Conversions must be deterministic and reversible where applicable.

Example

1000 mm

↓

1 m

---

# Storage Rules

Store

Value

Canonical SI Unit

Display Unit

Example

```
Value

2500

Stored Unit

mm

Display

2.5 m
```

---

# Display Rules

Users may configure preferred display units.

Internal calculations always use canonical units.

---

# Validation

Every measurable value requires

Quantity

Unit

Valid Range

Precision

---

# Manufacturing

Supports

Machine Dimensions

Tool Sizes

Production Parameters

Material Thickness

Board Width

Board Length

---

# Inventory

Supports

Stock Units

Purchase Units

Sales Units

Production Units

Conversion Factors

---

# Packaging

Supports

Piece

Bundle

Pallet

Container

Weight

Volume

---

# CAD Integration

Supports

Millimeter

Meter

Scale Conversion

Reference Units

---

# CNC Integration

Supports

Millimeter Precision

Machine Coordinates

Tool Offsets

Tolerance

---

# Digital Twin

Supports

Sensor Measurements

Machine Dimensions

Live Telemetry

Reference

Digital_Twin.md

---

# AI Support

AI may

Convert units

Detect inconsistent measurements

Recommend standard units

Validate engineering values

---

# Localization

Display formats follow user locale.

Reference

Localization.md

---

# API

Example

```json
{
  "value": 26,
  "unit": "mm"
}
```

Never transmit unit-less values.

---

# Performance

Supports

Cached Conversion Tables

Immutable Unit Definitions

Fast Conversion Engine

---

# Security

Measurement rules are centrally managed.

Unauthorized changes require approval.

Reference

Approval_Workflow.md

---

# Example Material

Thickness

26 mm

Width

140 mm

Length

3900 mm

Volume

0.014196 m³

Weight

7.9 kg

Moisture

6.5 %

---

# Best Practices

✓ Always store units.

✓ Use SI internally.

✓ Convert only for presentation.

✓ Define precision explicitly.

✓ Validate engineering ranges.

✓ Avoid implicit conversions.

---

# Do

✓ Store canonical units

✓ Validate conversions

✓ Keep precision configurable

✓ Support manufacturing tolerances

✓ Document custom units

---

# Don't

✗ Mix units in calculations

✗ Store values without units

✗ Hardcode conversion factors

✗ Round prematurely

✗ Assume user locale equals storage format

---

# Acceptance Criteria

All measurable values include units.

SI units are used internally.

Conversions are deterministic.

Precision is configurable.

Manufacturing and reporting use the same standard.

Localization affects only presentation.

---

# Related Documents

Localization.md

Currency.md

Material.md

API_Standards.md

Digital_Twin.md

CAD_Standards.md

AI_Copilot.md

Architecture.md
