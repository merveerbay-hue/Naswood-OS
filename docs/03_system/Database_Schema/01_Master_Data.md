# Database Schema — Master Data

**Project:** Naswood OS
**Document:** Master Data Schema
**Database:** PostgreSQL
**Version:** 2.0
**Status:** Approved

---

# Purpose

Master Data contains relatively static business information shared across all modules.

Master Data is the foundation of every business process.

Changes are infrequent and controlled.

Every transactional entity references Master Data through UUID foreign keys.

---

# Entity List

## Organization

Company

Factory

Department

Position

Role

Permission

ApprovalLevel

Shift

ProductionCalendar

---

## Material

MaterialType

MaterialStatus

WoodSpecies

QualityGrade

DefectType

WasteType

AttributeDefinition

MeasurementType

TransformationType

ProductionStrategy

---

## Production

RecipeType

OperationType

RoutingType

MachineParameterType

---

## Machines

MachineType

MachineGroup

EnergyType

---

## Tooling

ToolType

ToolCategory

KnifeType

CutterHeadType

---

## Warehouse

Warehouse

WarehouseLocation

StorageZone

PackageType

ShipmentType

---

## Commercial

CustomerType

SupplierType

Currency

Country

Language

Unit

GlueType

GlueSupplier

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
| action | VARCHAR(50) |
| description | TEXT |

---

# Material Types

Examples

LOG

PRISM

GREEN_LUMBER

KD_LUMBER

THERMOWOOD_LUMBER

LAMELLA

FJ_LAMELLA

SOLID_PANEL

FJ_PANEL

PROFILE

DECK

CLADDING

PELLET

WOOD_CHIP

SAWDUST

BARK

PACKAGING

---

# Material Status

Available

Reserved

In Production

Waiting Quality

Approved

Rejected

Recovered

Packaged

Shipped

Consumed

Scrapped

Archived

---

# Wood Species

Examples

Pine

Spruce

Fir

Ash

Beech

Oak

Ayous

Iroko

Teak

Accoya

---

# Quality Grades

AA

AB

AC

BB

BC

CC

Industrial

Reject

---

# Transformation Types

Split

Merge

Conversion

Recovery

Scrap

Packaging

Shipment

---

# Production Strategy

MTS

Make To Stock

---

MTO

Make To Order

---

ATO

Assemble To Order

---

ETO

Engineer To Order

---

# Waste Types

Sawdust

Wet Sawdust

Thermowood Sawdust

Wood Chip

Trim

Bark

Rejected Material

Glue Waste

Packaging Waste

Other

---

# Attribute Definition

Defines every dynamic material property.

Examples

Bottom Diameter

Top Diameter

Average Diameter

Length

Width

Thickness

Moisture

Density

Weight

Volume

Color

Surface Roughness

Growth Ring

Heartwood Ratio

Sapwood Ratio

Thermo Class

Glue Batch

Press Time

Press Pressure

---

# Measurement Types

## Geometry

Length

Width

Thickness

Bottom Diameter

Top Diameter

Average Diameter

Cross Section

---

## Moisture

Moisture

---

## Physical

Weight

Volume

Density

---

## Surface

Surface Roughness

Brush Depth

Color

Gloss

---

## Process

Temperature

Pressure

Feed Speed

RPM

Glue Spread

Press Time

Humidity

---

## Machine

Motor Current

Voltage

Power

Air Pressure

Hydraulic Pressure

---

## Quality

Warp

Twist

Bow

Cup

Crack Width

Straightness

---

## Energy

Power Consumption

Fuel Consumption

Compressed Air Consumption

Steam Consumption

---

# Machine Parameter Types

Feed Speed

RPM

Pressure

Temperature

Humidity

Motor Current

Voltage

Power

Hydraulic Pressure

Air Pressure

Tool Offset

Spindle Speed

---

# Recipe Types

Drying

Thermowood

Profiling

Finger Joint

Panel Press

Calibration

Packaging

---

# Tool Types

Circular Saw

Band Saw

Planer Knife

Moulder Knife

Finger Joint Cutter

Router Bit

Drill

CNC Tool

---

# Energy Types

Electricity

Natural Gas

Biomass

Diesel

LPG

Compressed Air

Steam

---

# Units

mm

cm

m

m²

m³

kg

g

piece

liter

minute

hour

kWh

°C

bar

rpm

%

---

# General Rules

- UUID Primary Keys
- Soft Delete
- Audit Fields
- Human Readable Business Codes
- Company Scoped
- Factory Scoped where applicable
- Unique Code Constraints
- No Physical Deletes
- Version Controlled Master Data
- All Transactional Tables Reference Master Data via UUID
