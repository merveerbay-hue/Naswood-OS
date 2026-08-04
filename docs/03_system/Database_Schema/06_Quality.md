# Database Schema — Quality

**Project:** Naswood OS
**Document:** Quality Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Quality module manages inspections, measurements, approvals, defects, laboratory tests and quality decisions throughout the manufacturing lifecycle.

Quality is integrated into every stage of production.

Every inspection is fully traceable.

---

# Philosophy

Quality does not inspect products only.

Quality validates:

- Materials
- Production
- Machines
- Recipes
- Packages
- Shipments

Every quality decision becomes part of Material genealogy.

---

# Entity List

InspectionPlan

Inspection

InspectionResult

Measurement

Defect

NonConformance

QualityDecision

QualityCertificate

LaboratoryTest

---

# inspection_plan

Defines standard inspection requirements.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(150) |
| inspection_type | VARCHAR(50) |
| material_type_id | UUID FK |
| product_family_id | UUID FK |
| operation_type_id | UUID FK |
| active | BOOLEAN |

Inspection Types

- Incoming
- In Process
- Final
- Shipment

---

# inspection

Represents one inspection.

| Field | Type |
|--------|------|
| id | UUID |
| inspection_plan_id | UUID FK |
| material_id | UUID FK |
| transformation_id | UUID FK |
| operation_id | UUID FK |
| machine_id | UUID FK |
| inspector_id | UUID FK |
| inspection_status | VARCHAR(30) |
| inspection_date | TIMESTAMP |

Inspection Status

- Planned
- In Progress
- Completed
- Cancelled

---

# inspection_result

Stores inspection outcomes.

| Field | Type |
|--------|------|
| id | UUID |
| inspection_id | UUID FK |
| characteristic | VARCHAR(100) |
| target_value | VARCHAR(100) |
| measured_value | VARCHAR(100) |
| tolerance | VARCHAR(100) |
| result | VARCHAR(20) |

Result

- Pass
- Fail
- Conditional

---

# measurement

Stores measured values.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| inspection_id | UUID FK |
| measurement_type | VARCHAR(50) |
| value | NUMERIC |
| unit | VARCHAR(20) |
| measured_at | TIMESTAMP |

Measurement Types

- Moisture
- Thickness
- Width
- Length
- Density
- Weight
- Temperature
- Humidity
- Pressure
- Glue Spread
- Color
- Surface Roughness

---

# defect

Represents detected defects.

| Field | Type |
|--------|------|
| id | UUID |
| inspection_id | UUID FK |
| material_id | UUID FK |
| defect_type | VARCHAR(100) |
| severity | VARCHAR(20) |
| description | TEXT |

Severity

- Minor
- Major
- Critical

Examples

- Knot
- Crack
- Warp
- Twist
- Bow
- Resin Pocket
- Burn Mark
- Glue Failure
- Surface Damage
- Dimension Error

---

# non_conformance

Represents quality non-conformities.

| Field | Type |
|--------|------|
| id | UUID |
| inspection_id | UUID FK |
| material_id | UUID FK |
| defect_id | UUID FK |
| disposition | VARCHAR(30) |
| approved_by | UUID FK |
| approved_at | TIMESTAMP |

Disposition

- Accept
- Rework
- Recover
- Downgrade
- Scrap
- Customer Approval Required

---

# quality_decision

Final quality disposition.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| inspection_id | UUID FK |
| decision | VARCHAR(30) |
| decided_by | UUID FK |
| decision_date | TIMESTAMP |
| remarks | TEXT |

Decision Types

- Approved
- Rejected
- Hold
- Released
- Rework
- Recovery

---

# quality_certificate

Stores quality certificates.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| certificate_type | VARCHAR(50) |
| certificate_number | VARCHAR(100) |
| issue_date | DATE |
| expiry_date | DATE |

Examples

- FSC
- PEFC
- CE
- Customer Certificate
- Internal Certificate

---

# laboratory_test

Stores laboratory analyses.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| inspection_id | UUID FK |
| test_type | VARCHAR(100) |
| result | TEXT |
| tested_by | UUID FK |
| tested_at | TIMESTAMP |

Examples

- Moisture Analysis
- Density Test
- Adhesion Test
- Strength Test
- Color Stability
- Thermal Modification Verification

---

# Relationships

Inspection Plan

1 → N Inspections

Inspection

1 → N Inspection Results

Inspection

1 → N Measurements

Inspection

1 → N Defects

Inspection

1 → N Laboratory Tests

Inspection

1 → 1 Quality Decision

Defect

1 → N Non-Conformances

Material

1 → N Inspections

Material

1 → N Measurements

Material

1 → N Quality Decisions

Material

1 → N Certificates

---

# Business Rules

### BR-601

Every inspection shall follow an approved Inspection Plan.

---

### BR-602

Every quality decision shall be permanently recorded.

---

### BR-603

Measurements are immutable.

Corrections create new measurements.

---

### BR-604

Rejected Materials shall receive an Inventory Hold until disposition is completed.

---

### BR-605

Recovery has priority over Scrap whenever technically possible.

---

### BR-606

Quality Decisions shall update Material Status.

---

### BR-607

Inspection history shall never be deleted.

---

### BR-608

Laboratory results become part of Material genealogy.

---

### BR-609

Every critical defect shall create a Non-Conformance record.

---

### BR-610

Quality approvals requiring authorization shall generate an Audit Log.

---

# Integration

Quality integrates with:

- Materials
- Inventory
- Production
- Transformation
- Packaging
- Shipment
- Maintenance
- AI
- Audit Log

---

# Quality Philosophy

Quality is built into the manufacturing process rather than inspected only at the end.

Every Material carries its complete quality history throughout its lifecycle.

Reliable quality depends on reliable measurements, consistent inspections and complete traceability.
