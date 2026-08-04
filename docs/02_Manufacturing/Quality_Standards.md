# Quality Standards

**Project:** Naswood OS  
**Document:** Quality Standards  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines the quality management model used throughout the manufacturing process.

Quality control is not limited to finished products.

Every material entering, moving through, or leaving production shall be evaluated according to standardized quality rules.

The Quality Engine is responsible for:

- Material Classification
- Process Inspection
- Defect Recording
- Product Classification
- Recovery Decisions
- Reject Decisions
- Quality Traceability

---

# 2. Quality Philosophy

Quality begins at raw material acceptance and continues until shipment.

Every material has a measurable quality status.

Quality decisions are based on objective inspection criteria.

The system shall never allow undefined quality classifications.

---

# 3. Quality Levels

Incoming materials are classified into four primary quality levels.

| Code | Description |
|------|-------------|
| H1 | Premium / Selected |
| H2 | Standard |
| H3 | Rustic |
| RET | Reject |

Finished panels are classified as:

AA

AB

AC

BB

BC

CC

RET

---

# 4. Input Material Classification

Incoming lamellas shall be inspected according to standardized defect criteria.

Classification considers:

- Knot Size
- Dead Knots
- Resin Pocket
- Cracks
- Pith
- Sapwood / Heartwood Transition
- Blue Stain
- Compression Wood
- Bark Inclusion
- Moisture
- Straightness

The worst defect found on the lamella determines the final quality class. :contentReference[oaicite:0]{index=0}

---

# 5. Output Panel Classification

Finished panels are evaluated independently on each face.

Possible combinations include:

AA

AB

AC

BB

BC

CC

RET

Panel quality is determined by the lowest quality lamella on each face (Weakest Link Rule). :contentReference[oaicite:1]{index=1}

---

# 6. Quality Rules

## Worst Defect Rule

The highest severity defect found on a material determines its quality class.

Average quality shall never be calculated.

---

## Weakest Link Rule

Panel face quality equals the lowest quality lamella used on that face. :contentReference[oaicite:2]{index=2}

---

## H1 Protection Rule

H1 materials are reserved exclusively for A-face production.

They shall never be consumed in B-face or C-face products. :contentReference[oaicite:3]{index=3}

---

## Species Separation Rule

Different wood species shall never be mixed within the same panel. :contentReference[oaicite:4]{index=4}

---

# 7. Defect Catalog

Every defect has a predefined code.

Example:

DF001 Sound Knot

DF002 Dead Knot

DF003 Loose Knot

DF004 Resin Pocket

DF005 Surface Crack

DF006 End Split

DF007 Blue Stain

DF008 Bark Inclusion

DF009 Compression Wood

DF010 Moisture Out of Range

DF011 Glue Joint Opening

DF012 Delamination

DF013 Sanding Burn

...

The catalog is maintained centrally and used across all inspection points. It is based on the common defect terminology defined for operators. :contentReference[oaicite:5]{index=5}

---

# 8. Quality Events

Every inspection creates a Quality Event.

Each event contains:

Quality Event ID

Material ID

Operation

Machine

Operator

Inspector

Inspection Type

Quality Grade

Detected Defects

Measurements

Photos

Decision

Timestamp

---

# 9. Inspection Points

Quality inspections are performed at the following production stages.

Receiving

↓

Sawing

↓

Kiln Drying

↓

Thermowood

↓

Profiling

↓

Finger Joint

↓

Planing

↓

Press

↓

Calibration

↓

Packaging

Each inspection generates a Quality Event.

---

# 10. Automatic Reject Rules

Materials shall automatically receive RET status when critical defects are detected.

Examples include:

- Loose Knot
- Delamination
- Through Crack
- Active Rot
- Insect Damage
- Moisture Outside Allowed Range
- Metal Contamination

These rules are derived from the panel quality guide and require no operator interpretation. :contentReference[oaicite:6]{index=6} :contentReference[oaicite:7]{index=7}

---

# 11. Recovery Decision

Rejected materials are evaluated for recovery.

Possible outcomes:

Reuse

Finger Joint

Pellet

Fuel

Waste

Recovery decisions are recorded within the Quality Event.

---

# 12. Technical Product Specification

Finished products include a standardized technical specification.

Typical attributes:

Wood Species

Thickness

Quality Class

Lamination Type

Glue Type

Moisture

Surface Finish

Length Category

Production Date

Production Line

Operator

Package ID

The coding structure follows the product coding principles defined for technical identification. :contentReference[oaicite:8]{index=8}

---

# 13. Gold Sample Policy

Each quality class shall have an approved physical reference sample.

Reference samples are used:

- Operator Training
- Inspection Calibration
- Customer Claims
- Quality Audits

The physical sample is the final reference in disputed classifications. :contentReference[oaicite:9]{index=9}

---

# 14. Quality KPIs

The system calculates:

First Pass Yield

Reject Rate

Recovery Rate

Supplier Quality

Machine Quality

Operator Quality

Inspection Accuracy

Claim Rate

Rework Rate

Average Quality Score

---

# 15. AI Vision Integration

Future AI systems will support:

Automatic Defect Detection

Surface Classification

Panel Face Classification

Knot Detection

Crack Measurement

Color Analysis

Grain Direction Detection

Surface Finish Inspection

Predictive Quality Analysis

Operator Decision Validation

---

# 16. Business Rules

- Every material must have a quality grade.
- Every inspection creates a Quality Event.
- Every defect must use a standard defect code.
- Materials without quality approval cannot proceed to the next operation.
- Quality history cannot be modified after approval.
- H1 materials are reserved for A-face production only.
- Different wood species shall not be mixed within the same panel.
- Quality classifications must remain fully traceable throughout the product lifecycle.
