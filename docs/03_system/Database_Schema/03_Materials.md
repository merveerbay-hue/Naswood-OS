# Database Schema — Materials

**Project:** Naswood OS
**Document:** Materials Schema
**Database:** PostgreSQL
**Version:** 2.0
**Status:** Approved

---

# Purpose

The Materials domain manages every physical object throughout its lifecycle.

A Material represents a uniquely identifiable physical object.

A Material does NOT represent a production operation.

Production operations are represented by the Transformation entity in the Production module.

The Materials module is responsible for:

- Receiving
- Material Identity
- Material Attributes
- Material Status
- Material Location
- Material Genealogy
- Certifications
- Measurements
- Reservations
- Documents

---

# Entity List

ReceivingLot

Material

MaterialAttribute

MaterialTypeAttribute

MaterialPropertySnapshot

MaterialLocation

MaterialReservation

MaterialRelationship

MaterialMeasurement

MaterialCertification

MaterialDocument

MaterialTag

---

# receiving_lot

Represents the first accepted batch entering the factory.

Examples

- Log Truck
- Purchased Green Lumber
- Purchased KD Lumber
- Purchased Thermowood Lumber
- Purchased Dry Lamellas

| Field | Type | Constraint |
|--------|------|------------|
| id | UUID | PK |
| code | VARCHAR(30) | UNIQUE |
| supplier_id | UUID FK |
| factory_id | UUID FK |
| arrival_date | TIMESTAMP |
| vehicle_plate | VARCHAR(30) |
| driver_name | VARCHAR(100) |
| delivery_note | VARCHAR(100) |
| invoice_number | VARCHAR(100) |
| purchase_order_id | UUID FK |
| notes | TEXT |

---

# material

Represents one unique physical material.

The Material table contains only identity information.

Technical properties are stored separately.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(40) UNIQUE |
| material_type_id | UUID FK |
| receiving_lot_id | UUID FK |
| current_status_id | UUID FK |
| quality_grade_id | UUID FK |
| species_id | UUID FK |
| warehouse_location_id | UUID FK |
| current_package_id | UUID FK |
| production_order_id | UUID FK |
| work_order_id | UUID FK |
| current_transformation_id | UUID FK |
| barcode | VARCHAR(100) |
| qr_code | VARCHAR(255) |
| created_at | TIMESTAMP |
| updated_at | TIMESTAMP |

---

# material_attribute

Stores the current values of all material attributes.

Examples

Length

Width

Thickness

Moisture

Density

Bottom Diameter

Thermo Class

Glue Batch

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| attribute_definition_id | UUID FK |
| value_string | TEXT |
| value_number | NUMERIC(18,6) |
| value_boolean | BOOLEAN |
| value_date | DATE |
| unit_id | UUID FK |
| measured_at | TIMESTAMP |
| measured_by | UUID FK |

Only one active value exists for each attribute.

---

# material_type_attribute

Defines which attributes belong to each Material Type.

Example

LOG

↓

Bottom Diameter

Required

↓

Average Diameter

Required

↓

Length

Required

---

THERMOWOOD_LUMBER

↓

Thermo Class

Required

↓

Moisture

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
| default_value | TEXT |
| display_order | INTEGER |

---

# material_property_snapshot

Stores historical values of material properties.

Examples

Moisture

22%

↓

Kiln Drying

↓

11%

↓

Thermowood

↓

6%

Previous values are never overwritten.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| attribute_definition_id | UUID FK |
| value_string | TEXT |
| value_number | NUMERIC(18,6) |
| unit_id | UUID FK |
| transformation_id | UUID FK |
| recorded_at | TIMESTAMP |

---

# material_location

Tracks current material location.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| warehouse_id | UUID FK |
| warehouse_location_id | UUID FK |
| updated_at | TIMESTAMP |

---

# material_measurement

Stores actual measurements.

Measurements differ from Attributes.

Measurements are observations.

Attributes describe the material.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| transformation_id | UUID FK |
| measurement_type_id | UUID FK |
| value | NUMERIC(18,6) |
| tolerance | NUMERIC(18,6) |
| device | VARCHAR(100) |
| operator_id | UUID FK |
| measured_at | TIMESTAMP |

---

# material_relationship

Maintains genealogy.

Relationship Types

Parent

Child

Split

Merge

Recovery

| Field | Type |
|--------|------|
| id | UUID |
| parent_material_id | UUID FK |
| child_material_id | UUID FK |
| transformation_id | UUID FK |
| relationship_type | VARCHAR(30) |

---

# material_reservation

Material allocation.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| production_order_id | UUID FK |
| reserved_quantity | NUMERIC(18,3) |
| reserved_at | TIMESTAMP |
| reserved_by | UUID FK |

---

# material_certification

Stores certifications.

Examples

FSC

PEFC

CE

EPD

DPP

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| certificate_type | VARCHAR(50) |
| certificate_number | VARCHAR(100) |
| issue_date | DATE |
| expiry_date | DATE |
| issuing_authority | VARCHAR(150) |

---

# material_document

Stores references to external documents.

Examples

Photo

PDF

DXF

STEP

Inspection Report

Delivery Note

Certificate

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| document_type | VARCHAR(50) |
| file_name | VARCHAR(255) |
| storage_url | TEXT |
| uploaded_by | UUID FK |
| uploaded_at | TIMESTAMP |

---

# material_tag

Flexible labels.

Examples

FSC

Export

Customer Project

Urgent

Premium

Custom

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| tag | VARCHAR(100) |

---

# Relationships

ReceivingLot

1 → N Materials

Material

1 → N Attributes

Material

1 → N Measurements

Material

1 → N Property Snapshots

Material

1 → N Documents

Material

1 → N Certifications

Material

1 → N Reservations

Material

1 → N Tags

Material

1 → N Relationships

---

# General Rules

- Every physical object has exactly one Material UUID.
- Material identity never changes.
- Material Attributes represent the current state.
- Material Property Snapshots preserve historical values.
- Measurements are immutable observations.
- Technical properties are not stored in the Material table.
- Every material movement generates an Event.
- Material genealogy is maintained through Material Relationships.
- Soft Delete is preferred.
- UUID is mandatory for all primary keys.
