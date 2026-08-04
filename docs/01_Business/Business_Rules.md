# Business Rules

**Project:** Naswood OS  
**Document:** Business Rules  
**Version:** 1.0  
**Status:** Approved

---

# Purpose

This document defines the official business rules governing all operations within Naswood OS.

Business rules are independent of software implementation.

Every module, API, AI agent and user interface shall comply with these rules.

If software behavior conflicts with these rules, the business rules take precedence.

---

# Material Rules

## BR-001

Every physical object inside the factory shall have a unique Material identity.

No anonymous material is permitted.

---

## BR-002

Material identity never changes.

Dimensions, quality, moisture, status and location may change.

The Material UUID remains permanent.

---

## BR-003

Every material originates from exactly one Receiving Lot.

Receiving Lot is the beginning of material genealogy.

---

## BR-004

A material may produce multiple child materials.

Every parent-child relationship must remain traceable.

---

## BR-005

A material may participate in multiple Transformations during its lifecycle.

Each Transformation shall be recorded independently.

---

# Receiving Rules

## BR-101

Every incoming shipment shall create one Receiving Lot.

---

## BR-102

A Receiving Lot may contain multiple materials.

Examples

- Logs
- Green Lumber
- KD Lumber
- Thermowood Lumber
- Dry Lamellas

---

## BR-103

Supplier information shall be recorded before materials become available for production.

---

# Production Rules

## BR-201

Production is represented by Transformations.

Inventory movements alone do not represent production.

---

## BR-202

Every Transformation must have:

- Input Materials
- Output Materials
- Machine
- Operation
- Operator
- Timestamp

---

## BR-203

One Transformation may consume multiple input materials.

---

## BR-204

One Transformation may produce multiple output materials.

---

## BR-205

Every output material must preserve genealogy.

---

## BR-206

Routing is configurable.

Production Planning may select different routes according to:

- Species
- Moisture
- Quality
- Dimensions
- Customer requirements
- Machine availability

---

## BR-207

Length optimization may occur before or after drying.

The production department determines the optimum sequence.

---

## BR-208

Multiple Production Orders may be processed within the same Production Batch when approved by planning rules.

Complete traceability shall be preserved.

---

# Thermowood Rules

## BR-301

Thermowood production accepts multiple input sources.

Possible inputs

- Purchased KD Lumber
- Internally dried lumber
- Purchased Dry Lamellas

---

## BR-302

Every Thermowood batch shall reference the executed recipe.

---

## BR-303

Thermowood products shall pass through the profiling process before becoming finished products.

---

## BR-304

Thermowood Lumber may be:

- Sold
- Profiled
- Used in Panel Production

---

# Panel Production Rules

## BR-401

Solid Panel production accepts multiple input material types.

Examples

- Purchased KD Lumber
- Internal KD Lumber
- Thermowood Lumber
- Purchased Dry Lamellas
- Profiled Lumber (where applicable)

---

## BR-402

Defect-free lamellas are used for Solid Panels.

---

## BR-403

Defective lamellas shall be evaluated for recovery before being classified as waste.

---

## BR-404

Finger Joint Panels are produced using:

- Defect-free recovered segments
- Short recoverable pieces
- Planned Finger Joint production

---

## BR-405

Recovered lamellas remain fully traceable.

---

# Inventory Rules

## BR-501

Inventory is calculated from material status and movements.

No independent stock quantity shall exist.

---

## BR-502

A material may exist in only one physical location at a time.

---

## BR-503

Every location change generates an Inventory Movement.

---

## BR-504

Reserved materials cannot be consumed by another Production Order unless the reservation is released.

---

# Quality Rules

## BR-601

Materials requiring inspection cannot continue production until released.

---

## BR-602

Quality decisions shall be recorded permanently.

---

## BR-603

Quality history is immutable.

Corrections create new records.

---

# Waste and Recovery Rules

## BR-701

Recovery has priority over waste.

---

## BR-702

Recovered material becomes a new production input while maintaining genealogy.

---

## BR-703

Waste shall always be classified.

Examples

- Bark
- Wood Chips
- Wet Sawdust
- Dry Sawdust
- Thermowood Sawdust
- Packaging Waste

---

## BR-704

Thermowood sawdust shall be reused as Thermowood kiln fuel.

It shall not be transferred to pellet production.

---

## BR-705

Wet sawdust shall be dried before pellet production.

---

## BR-706

Wood waste may be processed through crushing and grinding before pellet production.

---

# Packaging Rules

## BR-801

Every package receives a unique Package identity.

---

## BR-802

Package contents remain fully traceable.

---

## BR-803

One package may contain materials originating from multiple Transformations.

Genealogy must remain complete.

---

# Shipment Rules

## BR-901

Only approved packages may be shipped.

---

## BR-902

Shipment history is permanent.

---

## BR-903

Delivered products remain traceable to their original Receiving Lot.

---

# Security Rules

## BR-1001

Critical business actions require authorization.

Examples

- Recipe approval
- Production approval
- Inventory adjustment
- Shipment confirmation

---

## BR-1002

Every critical action generates an Audit Log.

---

# AI Rules

## BR-1101

AI may recommend decisions.

AI shall not execute critical business actions without human approval.

---

## BR-1102

AI recommendations must be explainable and traceable.

---

## BR-1103

AI models shall use approved business data only.

---

# Data Rules

## BR-1201

Every business object has one authoritative owner.

---

## BR-1202

Duplicate master data is prohibited.

---

## BR-1203

Business Codes are unique within their scope.

---

## BR-1204

UUID is the permanent system identifier.

---

# Global Rules

- Every material is traceable.
- Every Transformation is recorded.
- Every production decision is auditable.
- Recovery is part of manufacturing.
- Waste is classified.
- Material genealogy is never broken.
- Software follows manufacturing.
- Business rules override implementation details.
- Documentation shall be updated before changing business behavior.

---

# Rule Governance

Every new business rule shall:

- Receive a unique Rule ID.
- Be documented before implementation.
- Be approved by business stakeholders.
- Be referenced by related modules and APIs.

Business Rules are version-controlled and form the contractual behavior of Naswood OS.
