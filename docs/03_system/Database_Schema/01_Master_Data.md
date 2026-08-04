# Database Schema — Master Data

**Project:** Naswood OS
**Document:** Master Data Schema
**Database:** PostgreSQL
**Version:** 1.0

---

# Purpose

Master Data contains relatively static business entities shared across all modules.

Master Data should rarely change.

Every transactional table references Master Data through UUID foreign keys.

---

# Entity List

Company

Factory

Department

Position

Role

Permission

MaterialType

WoodSpecies

QualityGrade

DefectType

TransformationType

WasteType

MeasurementType

MachineType

ToolType

RecipeType

Warehouse

WarehouseLocation

Unit

Currency

Country

Language

Shift

ProductionCalendar

PackageType

ShipmentType

---

# company

| Field | Type | Constraint |
|--------|------|------------|
| id | UUID | PK |
| code | VARCHAR(20) | UNIQUE |
| name | VARCHAR(200) | NOT NULL |
| tax_number | VARCHAR(30) | |
| address | TEXT | |
| phone | VARCHAR(50) | |
| email | VARCHAR(200) | |
| website | VARCHAR(200) | |
| status | VARCHAR(20) | |
| created_at | TIMESTAMP | |
| updated_at | TIMESTAMP | |

Indexes

company.code

---

# factory

| Field | Type |
|--------|------|
| id | UUID |
| company_id | UUID FK |
| code | VARCHAR(20) |
| name | VARCHAR(150) |
| city | VARCHAR(100) |
| address | TEXT |
| timezone | VARCHAR(50) |
| status | VARCHAR(20) |

Relationships

Company

1

↓

N

Factories

---

# department

| Field | Type |
|--------|------|
| id | UUID |
| factory_id | UUID FK |
| code | VARCHAR(20) |
| name | VARCHAR(100) |
| manager_position_id | UUID FK |

---

# position

| Field | Type |
|--------|------|
| id | UUID |
| department_id | UUID FK |
| code | VARCHAR(20) |
| title | VARCHAR(100) |
| approval_level | INTEGER |

---

# role

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |

---

# permission

| Field | Type |
|--------|------|
| id | UUID |
| module | VARCHAR(50) |
| action | VARCHAR(30) |
| scope | VARCHAR(50) |

---

# material_type

Examples

LOG

PRISM

LUMBER

KD

THERMO

LAMELLA

SOLID_PANEL

FJ_PANEL

PELLET

---

# wood_species

Examples

Pine

Spruce

Ash

Beech

Oak

Ayous

Iroko

Teak

---

# quality_grade

Examples

H1

H2

H3

AA

AB

AC

BB

BC

CC

RET

---

# defect_type

Stores standardized defect codes.

DF001

DF002

DF003

...

---

# transformation_type

Split

Merge

Conversion

Recovery

Scrap

Packaging

Shipment

---

# measurement_type

Moisture

Thickness

Width

Length

Weight

Density

Temperature

Pressure

RPM

Feed Speed

Glue Spread

---

# unit

mm

cm

m

m²

m³

kg

piece

hour

minute

---

# currency

TRY

USD

EUR

GBP

---

# General Rules

- UUID Primary Keys
- Soft Delete
- Audit Fields
- Company Scoped
- Factory Scoped where applicable
- Human-readable Codes
- Unique Code Constraints
- No Physical Deletes
