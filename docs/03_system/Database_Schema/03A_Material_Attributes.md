# Database Schema — Material Attributes

**Project:** Naswood OS
**Document:** Material Attributes Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Material Attributes module provides a flexible engineering data model for all material-specific technical properties.

Instead of adding new columns to the Material table, every engineering characteristic is managed through configurable attribute definitions.

This enables unlimited product expansion without changing the database schema.

---

# Philosophy

A Material represents the identity of a physical object.

Attributes describe that object.

Measurements observe that object.

Snapshots preserve historical values.

This separation keeps the core Material entity simple while allowing unlimited engineering flexibility.

---

# Entity List

AttributeCategory

AttributeDefinition

MaterialTypeAttribute

MaterialAttribute

MaterialAttributeHistory

AttributeValidationRule

AttributeOption

MeasurementDevice

MeasurementMethod

---

# attribute_category

Groups similar attributes.

Examples

Geometry

Moisture

Physical

Mechanical

Surface

Visual

Thermowood

Drying

Press

Glue

Machine

Quality

Commercial

Environmental

---

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |
| display_order | INTEGER |

---

# attribute_definition

Defines every available engineering attribute.

Examples

Length

Width

Thickness

Bottom Diameter

Top Diameter

Average Diameter

Moisture

Density

Glue Spread

Thermo Class

Feed Speed

Surface Roughness

Color

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(50) UNIQUE |
| name | VARCHAR(150) |
| category_id | UUID FK |
| data_type | VARCHAR(20) |
| default_unit_id | UUID FK |
| description | TEXT |
| active | BOOLEAN |

Supported Data Types

Number

Text

Boolean

Date

Enum

JSON

---

# material_type_attribute

Defines which attributes belong to each Material Type.

Example

LOG

↓

Bottom Diameter

Required

---

THERMOWOOD_LUMBER

↓

Thermo Class

Required

↓

Final Moisture

Required

↓

Density

Optional

| Field | Type |
|--------|------|
| id | UUID |
| material_type_id | UUID FK |
| attribute_definition_id | UUID FK |
| required | BOOLEAN |
| editable | BOOLEAN |
| calculated | BOOLEAN |
| display_order | INTEGER |

---

# material_attribute

Stores the current engineering properties of a material.

Only one active value exists for each attribute.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| attribute_definition_id | UUID FK |
| value_number | NUMERIC(18,6) |
| value_text | TEXT |
| value_boolean | BOOLEAN |
| value_date | DATE |
| value_enum | VARCHAR(50) |
| unit_id | UUID FK |
| source | VARCHAR(30) |
| updated_at | TIMESTAMP |

Attribute Sources

Manual

PLC

IoT

Scanner

Recipe

Calculation

Import

AI

---

# material_attribute_history

Stores previous values.

Example

Moisture

28%

↓

18%

↓

10%

↓

6%

History is never deleted.

| Field | Type |
|--------|------|
| id | UUID |
| material_attribute_id | UUID FK |
| previous_number | NUMERIC(18,6) |
| previous_text | TEXT |
| previous_boolean | BOOLEAN |
| previous_date | DATE |
| changed_at | TIMESTAMP |
| changed_by | UUID FK |
| transformation_id | UUID FK |

---

# attribute_validation_rule

Validation rules.

Examples

Moisture

0–100

---

Length

>0

---

Density

100–1200

| Field | Type |
|--------|------|
| id | UUID |
| attribute_definition_id | UUID FK |
| minimum_value | NUMERIC |
| maximum_value | NUMERIC |
| regular_expression | TEXT |
| required | BOOLEAN |

---

# attribute_option

Enumeration values.

Example

Thermo Class

↓

D

↓

S

↓

T

↓

Thermo-S

↓

Thermo-D

Another Example

Surface Finish

↓

Smooth

↓

Brushed

↓

Wire Brush

↓

Rough

| Field | Type |
|--------|------|
| id | UUID |
| attribute_definition_id | UUID FK |
| option_code | VARCHAR(30) |
| option_name | VARCHAR(100) |
| display_order | INTEGER |

---

# measurement_device

Registered measuring equipment.

Examples

Moisture Meter

Laser Scanner

Caliper

Scale

Vision Camera

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(150) |
| serial_number | VARCHAR(100) |
| calibration_date | DATE |
| next_calibration | DATE |
| status | VARCHAR(20) |

---

# measurement_method

Measurement procedure.

Examples

Manual

Automatic

Laser

Vision

Inline

Offline

PLC

Laboratory

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |

---

# Example Attribute Sets

## LOG

- Bottom Diameter
- Top Diameter
- Average Diameter
- Length
- Bark Percentage
- Forest Region
- FSC Certificate

---

## GREEN_LUMBER

- Length
- Width
- Thickness
- Moisture
- Volume

---

## KD_LUMBER

- Moisture
- Density
- Width
- Thickness
- Length

---

## THERMOWOOD

- Thermo Class
- Process Batch
- Final Moisture
- Density
- Color
- Surface Roughness

---

## SOLID PANEL

- Length
- Width
- Thickness
- Glue Batch
- Press Time
- Press Pressure
- Surface Grade

---

# Relationships

Attribute Category

1 → N Attribute Definitions

Attribute Definition

1 → N Material Type Attributes

Material Type

1 → N Material Type Attributes

Material

1 → N Material Attributes

Material Attribute

1 → N History Records

Measurement Device

1 → N Material Measurements

Measurement Method

1 → N Material Measurements

---

# General Rules

- Material never stores engineering properties directly.
- Engineering properties are stored as Material Attributes.
- Historical values are preserved.
- Validation rules are configurable.
- Measurement devices are traceable.
- New product types never require schema changes.
- Attribute Definitions are reusable across all material types.
- UUID is mandatory.
- Soft Delete is preferred.
