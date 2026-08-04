# Factory Flow

**Project:** Naswood OS  
**Document:** Factory Flow  
**Version:** 1.0  
**Status:** Approved

---

# Purpose

This document defines the complete manufacturing flow of Naswood factories.

It describes how materials enter the factory, how they are transformed, and how finished products are produced while maintaining complete traceability.

The Factory Flow serves as the primary reference for:

- Production Planning
- Routing
- Inventory
- Material Traceability
- Costing
- AI Optimization
- Manufacturing Execution

---

# Manufacturing Philosophy

Every physical material entering the factory receives a unique digital identity.

Throughout production:

- Material identity is preserved.
- Every transformation is recorded.
- Every movement is traceable.
- Every loss is classified.
- Every recovery remains part of genealogy.

Manufacturing is modeled as a sequence of Transformations.

---

# High-Level Factory Flow

```
Supplier
    │
    ▼
Receiving
    │
    ▼
Receiving Lot
    │
    ▼
Material Registration
    │
    ▼
Production Decision
```

---

# Raw Material Entry

The factory accepts multiple raw material types.

## 1. Log

Supplier delivers complete logs.

Possible destinations

- Sawmill
- Direct Sale

---

## 2. Green Lumber

Purchased rough sawn lumber.

Possible destinations

- Kiln Drying
- Resale

---

## 3. Kiln Dried Lumber

Purchased kiln dried lumber.

Possible destinations

- Profiling
- Thermowood
- Panel Production
- Resale

---

## 4. Dry Lamella

Purchased dry lamellas.

Possible destinations

- Profiling
- Thermowood
- Panel Production

---

## 5. Thermowood Lumber

Purchased finished Thermowood lumber.

Possible destinations

- Profiling
- Panel Production
- Direct Sale

---

# Log Processing

```
Receiving Log
        │
        ▼
Log Yard
        │
        ▼
Canter / Primary Saw
        │
        ├────────► Log Sale
        │
        ▼
Prism
        │
        ▼
Sawing
        │
        ▼
Green Lumber
```

During sawing, production determines the optimum cutting strategy to maximize material recovery.

Length optimization may occur before or after drying depending on production conditions.

---

# Green Lumber

Possible destinations

```
Green Lumber
      │
      ├────────► Direct Sale
      │
      ▼
Kiln Drying
```

---

# Kiln Drying

```
Green Lumber
      │
      ▼
Kiln Drying
      │
      ▼
Kiln Dried Lumber
```

Quality measurements include:

- Moisture
- Warp
- Twist
- Bow
- Cracks

Materials are classified after drying.

---

# Thermowood Flow

Thermowood production accepts three different inputs.

```
Purchased KD Lumber
            │
            │
Kiln Dried Lumber
            │
            │
Purchased Dry Lamella
            │
            ▼
      Pre-Processing
            │
            ▼
      First Planer
            │
            ▼
 Thermowood Kiln
            │
            ▼
Thermowood Lumber
```

After Thermowood treatment the material continues to profiling.

---

# Profiling

```
Thermowood Lumber
          │
          ▼
First Planer
          │
          ▼
Profiling
          │
          ▼
Finished Profile
```

Examples

- Deck
- Cladding
- Siding
- Battens
- Custom Profiles

Finished profiles may be:

- Packaged
- Sent to panel production (if applicable)
- Sold directly

---

# Solid Panel Production

Solid panel production may begin from multiple raw material sources.

Accepted inputs

- Purchased Kiln Dried Lumber
- Kiln Dried Lumber produced internally
- Thermowood Lumber
- Purchased Dry Lamellas
- Profiled Lumber (where applicable)

```
Material
      │
      ▼
First Planer
      │
      ▼
Quality Sorting
      │
      ▼
Four Side Planer
      │
      ▼
Glue Application
      │
      ▼
Panel Press
      │
      ▼
Calibration
      │
      ▼
Final Inspection
      │
      ▼
Packaging
```

Only defect-free lamellas are used in Solid Panels.

---

# Finger Joint Panel Production

Materials containing defects follow a different route.

```
Material
      │
      ▼
First Planer
      │
      ▼
Defect Detection
      │
      ▼
Cross Cut
      │
      ▼
Finger Joint
      │
      ▼
Long Lamella
      │
      ▼
Four Side Planer
      │
      ▼
Glue
      │
      ▼
Panel Press
```

Recovered short pieces from other manufacturing processes may also enter the Finger Joint process.

Recovery materials remain fully traceable.

---

# Packaging

Products are grouped into Packages.

Package contents remain linked to every originating material.

Package types

- Bundle
- Pallet
- Crate
- Container

---

# Shipment

```
Finished Product
        │
        ▼
Package
        │
        ▼
Warehouse
        │
        ▼
Shipment
        │
        ▼
Customer
```

Complete genealogy remains available after shipment.

---

# Recovery Flow

Recovery is considered part of manufacturing.

Examples

- Short pieces
- Recoverable offcuts
- Reusable lamellas

Recovered materials receive new production routing while preserving genealogy.

---

# Waste Flow

Waste is classified by type.

Examples

- Bark
- Wood Chips
- Wet Sawdust
- Dry Sawdust
- Thermowood Sawdust
- Reject Material
- Packaging Waste

Thermowood sawdust is not used for pellet production.

It is reused as fuel for Thermowood kilns.

Wet sawdust is dried before entering pellet production.

Wood waste may pass through crushing and grinding before pelletizing.

---

# Pellet Production

Possible inputs

- Wood Chips
- Dry Sawdust
- Dried Wet Sawdust
- Crushed Wood Waste

```
Wood Waste
      │
      ▼
Crusher
      │
      ▼
Grinder
      │
      ▼
Dryer
      │
      ▼
Pellet Line
      │
      ▼
Pellet Packaging
```

---

# Business Rules

- Every material receives a unique digital identity.
- Every transformation generates genealogy records.
- Materials may have multiple possible production routes.
- Routing decisions are determined by production planning.
- Material optimization has priority over fixed process order.
- Recovery remains part of production.
- Waste must always be classified.
- Thermowood sawdust shall not enter pellet production.
- Production history shall never be deleted.

---

# Factory Flow Summary

```
Receiving
    │
    ▼
Receiving Lot
    │
    ▼
Raw Material
    │
    ├────────► Direct Sale
    │
    ▼
Sawing
    │
    ▼
Green Lumber
    │
    ▼
Kiln Drying
    │
    ▼
Kiln Dried Lumber
    │
    ├────────► Direct Sale
    │
    ├────────► Thermowood
    │
    ├────────► Panel Production
    │
    └────────► Profiling
                  │
                  ▼
            Finished Products
                  │
                  ▼
             Packaging
                  │
                  ▼
              Shipment
```

---

# Design Principles

- Material-centric manufacturing
- Complete traceability
- Flexible routing
- Recovery before waste
- Transformation-based production
- AI-ready manufacturing data
- One digital identity for every physical object
- Full genealogy from Receiving Lot to Customer
