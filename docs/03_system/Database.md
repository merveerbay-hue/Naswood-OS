# Database Design

**Project:** Naswood OS
**Document:** Database Design
**Version:** 2.0
**Status:** Architecture Approved

---

# 1. Purpose

This document defines the database architecture, storage strategy, and data organization principles for Naswood OS.

The database is designed to support:

- Material-centric manufacturing
- Full traceability
- Event-driven architecture
- High-volume production
- AI analytics
- Multi-factory operations
- Long-term scalability

The database is the single source of truth for all operational data.

---

# 2. Database Philosophy

Naswood OS does not store documents.

It stores business objects and business events.

Examples:

- Materials
- Transformations
- Operations
- Measurements
- Events
- Inventory Movements

Every business action creates a permanent digital history.

---

# 3. Storage Architecture

The database consists of five logical layers.

```

Master Data

↓

Transaction Data

↓

Event Store

↓

Analytics

↓

Audit

```

Each layer has its own responsibility.

---

# 4. Master Data

Master Data contains relatively static business information.

Examples

Company

Factory

Department

Position

Employee

Role

Permission

Material Type

Species

Product

Product Variant

Quality Grade

Defect

Machine

Machine Group

Tool

Cutter Head

Recipe

Warehouse

Warehouse Location

Customer

Supplier

Currency

Unit

Measurement Type

Transformation Type

Waste Type

Package Type

Shipment Type

Shift

Production Calendar

Master Data is version-controlled.

---

# 5. Transaction Data

Stores daily business operations.

Receiving Lots

Production Orders

Work Orders

Transformations

Materials

Inventory Movements

Measurements

Quality Events

Maintenance Orders

Tool Installations

Packages

Shipments

Purchase Orders

Sales Orders

Customer Complaints

Documents

Transactions represent the operational state of the factory.

---

# 6. Event Store

Every completed business action generates an immutable event.

Examples

MaterialReceived

TransformationStarted

TransformationCompleted

MaterialSplit

MaterialMerged

MaterialRecovered

QualityApproved

MachineStarted

ToolChanged

PackageCreated

ShipmentCompleted

Events are append-only.

No event is updated.

No event is deleted.

---

# 7. Analytics Layer

Analytics data is generated from transactions and events.

Examples

Daily KPIs

Production KPIs

Inventory KPIs

Machine KPIs

Waste KPIs

Recovery KPIs

Financial KPIs

AI Predictions

Forecasts

Dashboard Snapshots

Analytics tables may be regenerated.

---

# 8. Audit Layer

Stores security and compliance records.

Examples

User Login

Permission Change

Configuration Change

Approval History

Record Change

Data Export

Security Events

Audit records are immutable.

---

# 9. Identity Strategy

Every business object has two identities.

Internal UUID

Example

550e8400-e29b-41d4-a716-446655440000

Business Code

Example

THM-PN-000145

UUID is used internally.

Business Code is displayed to users.

Business codes follow Naming_Standards.md.

---

# 10. Core Business Objects

The database revolves around these core entities.

Organization

↓

Factory

↓

Receiving Lot

↓

Material

↓

Transformation

↓

Operation

↓

Package

↓

Shipment

↓

Customer

Supporting entities include:

Recipe

Machine

Tool

Measurement

Quality Event

Inventory Movement

Document

Event

---

# 11. Transformation-Centric Design

Transformation is the central operational entity.

Each Transformation records:

Inputs

Outputs

Waste

Recovery

Machine

Operator

Recipe

Measurements

Energy

Duration

Quality

Cost

Traceability

Every produced material references its source Transformation.

---

# 12. Material Model

Material is a physical object.

Every Material contains:

UUID

Business Code

Material Type

Species

Dimensions

Moisture

Quality

Current Location

Current Status

Receiving Lot

Transformation Reference

Package Reference

Shipment Reference

A Material never changes identity.

---

# 13. Measurement Model

Measurements are stored independently.

Examples

Moisture

Thickness

Width

Length

Weight

Density

Temperature

Pressure

Humidity

Glue Spread

Machine Parameters

Every measurement references:

Material

Transformation

Machine

Measurement Device

Operator

Timestamp

---

# 14. Inventory Model

Inventory is movement-based.

There is no editable stock balance.

Current stock is calculated from movements.

Movement Types

Receiving

Production

Transfer

Reservation

Consumption

Recovery

Shipment

Adjustment

Inventory movements remain immutable.

---

# 15. Quality Model

Quality is event-based.

Each inspection creates a Quality Event.

Quality history is never overwritten.

Inspection photos and documents are stored externally.

---

# 16. File Storage

Binary files are stored outside PostgreSQL.

Examples

Images

Certificates

DXF

STEP

PDF

Videos

The database stores metadata and file references only.

---

# 17. Data Integrity

Foreign Keys are mandatory.

Cascade Delete is prohibited.

Soft Delete is preferred.

Transactions guarantee consistency.

Every critical operation uses database transactions.

---

# 18. Performance Strategy

Indexes shall exist for:

UUID

Business Code

Material Code

Transformation

Receiving Lot

Package

Shipment

Machine

Timestamp

Frequently filtered fields.

Large tables shall be partitioned.

---

# 19. Partitioning Strategy

Partition by:

Factory

Year

Month

Event Type

Large production tables:

Events

Measurements

Inventory Movements

Quality Events

Audit Logs

---

# 20. Scalability

Supports

Multi Factory

Multi Warehouse

Multi Company

Cloud

On-Premise

Hybrid

Microservice Migration

Future Distributed Databases

---

# 21. Backup Strategy

Daily Incremental Backup

Weekly Full Backup

Monthly Archive

Point-in-Time Recovery

Backup Validation

Disaster Recovery Tests

---

# 22. Security

Encrypted Connections

Role-Based Access

Attribute-Based Access

Row-Level Security (Future)

Encrypted Sensitive Data

API Authentication

Audit Logging

---

# 23. Database Standards

UUID Primary Keys

UTC Timestamps

ISO Date Formats

Soft Delete

Immutable Events

Normalized Business Data

JSONB only for flexible payloads

No duplicated business logic

---

# 24. Future Extensions

Prepared for:

Digital Twin

Machine Vision

IoT Sensors

OPC-UA Integration

PLC Integration

Carbon Tracking

Digital Product Passport

AI Copilot

Predictive Maintenance

Autonomous Scheduling

Multi-Tenant SaaS Architecture

---

# 25. Design Principles

- Material-centric
- Transformation-centric
- Event-driven
- API-first
- AI-ready
- Traceability-first
- Cloud-ready
- Modular
- Extensible
- Long-term maintainable
