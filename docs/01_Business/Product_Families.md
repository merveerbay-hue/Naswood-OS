# Product Families

**Project:** Naswood OS  
**Document:** Product Families  
**Version:** 1.0  
**Status:** Approved

---

# Purpose

This document defines the commercial product hierarchy used throughout Naswood OS.

Products represent sellable items.

Materials represent physical manufacturing objects.

A Product may be manufactured from different Materials while maintaining the same commercial identity.

Product Families provide a standardized classification for:

- Sales
- Pricing
- Quotations
- Production Planning
- Product Configuration
- Reporting
- Dealer Management
- AI Recommendations

---

# Product Hierarchy

```
Product Family
        │
        ▼
Product Series
        │
        ▼
Product
        │
        ▼
Variant
```

---

# Product Family Structure

## Thermowood

Products manufactured using the Thermowood process.

Typical products

- Deck
- Cladding
- Battens
- Facade Profiles
- Pergola Components
- Special Profiles

---

## Solid Wood Panels

Single-piece glued wooden panels.

Typical products

- Pine Panel
- Ash Panel
- Oak Panel
- Beech Panel

---

## Finger Joint Panels

Panels manufactured from finger-jointed lamellas.

Typical products

- Pine FJ Panel
- Ash FJ Panel
- Oak FJ Panel

---

## Structural Lumber

Construction-grade timber.

Examples

- KVH
- Structural Lumber
- Framing Lumber

---

## Semi-Finished Products

Products sold for further processing.

Examples

- KD Lumber
- Thermowood Lumber
- Lamellas
- Finger Joint Lamellas

---

## Raw Materials

Commercially traded raw materials.

Examples

- Logs
- Green Lumber
- Dry Lumber
- Lamellas

---

## Biomass Products

Products generated from manufacturing by-products.

Examples

- Pellets
- Wood Chips
- Sawdust
- Bark

---

# Product Structure

Each Product contains:

Product Code

↓

Commercial Name

↓

Product Family

↓

Series

↓

Variant

↓

Dimensions

↓

Wood Species

↓

Surface Finish

↓

Quality Grade

↓

Packaging Type

---

# Product Variant Attributes

Variants may differ by:

## Dimensions

Thickness

Width

Length

---

## Wood Species

Pine

Spruce

Ash

Beech

Oak

Ayous

Iroko

Teak

Accoya

---

## Surface Finish

Smooth

Brushed

Wire Brushed

Sawn

Planed

---

## Edge Profile

Square Edge

Bevel

Rounded

Custom

---

## Color

Natural

Thermowood

Custom Finish

---

## Quality Grade

AA

AB

AC

BB

BC

Industrial

---

## Moisture Class

Green

KD

Thermowood

---

## Packaging

Bundle

Pallet

Crate

Container

---

# Product Coding

Example

```
NW-TWD-2692-BR-AB
```

Meaning

```
NW

↓

Naswood

↓

TWD

↓

Thermowood Deck

↓

26x92

↓

Brushed

↓

AB Grade
```

Business codes are configurable.

Internal UUIDs remain immutable.

---

# Product Lifecycle

```
Draft

↓

Under Development

↓

Approved

↓

Active

↓

Discontinued

↓

Archived
```

---

# Product Configuration

Each Product may reference:

- Routing
- Recipe
- Material Requirements
- Quality Requirements
- Packaging Rules
- Inspection Plans
- Certificates

Configuration changes are version-controlled.

---

# Product Relationships

A Product may have:

Alternative Products

Accessory Products

Replacement Products

Compatible Products

Recommended Products

---

# Product Documents

Products may include:

- Technical Datasheet
- Installation Guide
- CAD Drawings
- BIM Objects
- Certifications
- Product Images
- Marketing Documents

---

# Sales Integration

Every Product supports:

- Price Lists
- Dealer Pricing
- Customer Pricing
- Currency
- Discounts
- Campaigns

---

# Manufacturing Integration

Each Product references:

- Routing
- Production Strategy
- Quality Plan
- Packaging Rules
- Standard Recipes

One Product may be manufactured using different Material combinations while maintaining the same commercial specification.

---

# AI Support

AI may recommend:

- Alternative Products
- Material Substitutions
- Compatible Variants
- Production Optimization
- Packaging Optimization

AI recommendations require user approval.

---

# Business Rules

- Every sellable item shall be represented as a Product.
- Every Product belongs to exactly one Product Family.
- Products may contain multiple Variants.
- Product identity is independent of Material identity.
- Product configuration is version-controlled.
- Commercial information shall never be stored in Material entities.
- Manufacturing information shall reference Products without duplicating Product definitions.
- Product codes shall be unique within the company.

---

# Product Family Summary

| Product Family | Typical Outputs |
|----------------|-----------------|
| Thermowood | Deck, Cladding, Profiles |
| Solid Panels | Glued Panels |
| Finger Joint Panels | FJ Panels |
| Structural Lumber | Construction Timber |
| Semi-Finished | KD Lumber, Thermowood Lumber, Lamellas |
| Raw Materials | Logs, Green Lumber |
| Biomass | Pellets, Chips, Bark |
