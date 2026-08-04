# Entity Relationship Model

**Project:** Naswood OS  
**Document:** Entity Relationship Model  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines the business entities of Naswood OS and the relationships between them.

It serves as the foundation for:

- Database Design
- API Design
- Event Model
- Business Logic
- Reporting
- AI Analytics

This document represents the logical business model, not the physical database schema.

---

# 2. Domain Structure

Naswood OS consists of the following business domains.

- Material
- Production
- Routing
- Inventory
- Warehouse
- Quality
- Machine
- Tooling
- Maintenance
- Purchasing
- Sales
- Planning
- Traceability
- Packaging
- Shipping
- User & Security

---

# 3. Core Business Entities

## Material

Represents every physical material inside the factory.

Relationships

Material

↓

Material Type

↓

Material Species

↓

Material Status

↓

Warehouse Location

↓

Receiving Lot

↓

Package

↓

Production History

↓

Quality History

↓

Transformation History

---

## Material Type

Examples

- Log
- Prism
- Lumber
- Kiln Dried Lumber
- Thermowood
- Lamella
- Panel
- Profile
- Pellet

One Material Type

↓

Many Materials

---

## Material Species

Examples

- Pine
- Spruce
- Ash
- Beech
- Oak
- Ayous
- Iroko

One Species

↓

Many Materials

---

## Receiving Lot

Represents the first accepted material group entering the factory.

One Receiving Lot

↓

Many Materials

---

## Warehouse

One Warehouse

↓

Many Locations

---

## Warehouse Location

One Location

↓

Many Materials

---

# 4. Production Domain

## Work Order

One Work Order

↓

Many Operations

↓

Many Produced Materials

↓

Many Events

---

## Operation

One Operation

↓

Many Material Consumptions

↓

Many Produced Materials

↓

One Machine

↓

One Recipe

↓

Many Events

---

## Routing

One Routing Definition

↓

Many Operations

---

# 5. Traceability Domain

Every Material may have

One Parent Material

or

Many Parent Materials

Examples

Finger Joint

Panel

Merge Operations

Material

↓

Transformation

↓

Child Material

Transformation stores

- Split
- Merge
- Recovery
- Scrap

---

# 6. Inventory Domain

Warehouse

↓

Locations

↓

Inventory Balance

↓

Material

Material exists in only one active location at a time.

---

# 7. Package Domain

Package

↓

Many Materials

↓

One Shipment

---

# 8. Shipment Domain

Shipment

↓

Many Packages

↓

One Customer

---

# 9. Customer Domain

One Customer

↓

Many Sales Orders

↓

Many Shipments

↓

Many Complaints

---

# 10. Supplier Domain

One Supplier

↓

Many Receiving Lots

↓

Many Purchase Orders

---

# 11. Machine Domain

Machine

↓

Many Operations

↓

Many Maintenance Records

↓

Many Events

↓

Many Tool Assemblies

---

# 12. Tooling Domain

Tool

↓

Many Tool Installations

↓

Many Sharpening Records

↓

Many Recipes

---

## Cutter Head

One Cutter Head

↓

Many Tools

---

## Recipe

One Recipe

↓

Many Operations

↓

Many Tools

↓

Many Machines

---

# 13. Maintenance Domain

Machine

↓

Many Maintenance Work Orders

↓

Many Spare Parts

↓

Many Maintenance Events

---

# 14. Quality Domain

Material

↓

Many Quality Events

↓

Many Defects

↓

One Current Quality Grade

---

## Defect

One Defect Type

↓

Many Quality Events

---

# 15. User Domain

User

↓

Many Operations

↓

Many Quality Events

↓

Many Maintenance Records

↓

Many Approvals

---

# 16. Event Domain

Every Business Event references:

User

Factory

Machine

Material

Work Order

Operation

Timestamp

Correlation ID

Events never directly own business data.

---

# 17. Relationship Summary

Supplier

↓

Receiving Lot

↓

Material

↓

Operation

↓

Material

↓

Package

↓

Shipment

↓

Customer

Material additionally connects to:

- Warehouse
- Quality
- Machine
- Tool
- Event
- Inspection
- Recovery
- Waste

---

# 18. Cardinality Rules

Supplier

1 → N Receiving Lots

Receiving Lot

1 → N Materials

Material

1 → N Quality Events

Material

1 → N Inventory Movements

Material

1 → N Events

Work Order

1 → N Operations

Machine

1 → N Operations

Machine

1 → N Maintenance Records

Operation

1 → N Produced Materials

Operation

1 → N Consumed Materials

Package

1 → N Materials

Shipment

1 → N Packages

Customer

1 → N Shipments

Recipe

1 → N Operations

Tool

1 → N Recipes

Warehouse

1 → N Locations

Location

1 → N Materials

---

# 19. Traceability Rules

Every produced material must reference its origin.

Transformation records support:

- Split
- Merge
- Recovery
- Scrap

A material genealogy must always be reconstructable.

No orphan material records are allowed.

---

# 20. Design Principles

- Every entity has a globally unique identifier (UUID).
- Business codes remain human-readable.
- Relationships are normalized.
- Event history is immutable.
- Every material movement is traceable.
- Soft delete is preferred over physical delete.
- Audit information is stored for every critical entity.

---

# 21. Future Extensions

The relationship model is designed to support:

- CLT Production
- Glulam Production
- CNC Operations
- BIM Integration
- Digital Product Passport
- Carbon Tracking
- Digital Twin
- Multi-Factory Operations
- Multi-Company Architecture
