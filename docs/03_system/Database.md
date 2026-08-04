# Database Design

**Project:** Naswood OS  
**Document:** Database Design  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines the database architecture of Naswood OS.

It establishes:

- Data layers
- Entity organization
- Database standards
- Naming conventions
- Relationships
- Storage principles
- Performance strategy
- Scalability strategy

The physical database implementation shall follow the principles defined in this document.

---

# 2. Database Philosophy

Naswood OS uses a relational database as the primary source of truth.

The database is designed around:

- Materials
- Events
- Operations
- Traceability

rather than traditional ERP documents.

Every physical movement, transformation and business action must be represented in the database.

---

# 3. Database Engine

Primary Database

PostgreSQL

Supported Features

- UUID
- JSONB
- Full Text Search
- Partitioning
- Materialized Views
- Triggers
- Transactions

---

# 4. Data Layers

Naswood OS separates data into five logical layers.

```
Master Data
Transaction Data
Event Store
Analytics Data
Audit Data
```

---

# 5. Master Data

Master Data contains relatively stable business information.

Entities include:

- Material Types
- Wood Species
- Quality Grades
- Defect Types
- Machines
- Tools
- Cutter Heads
- Recipes
- Warehouses
- Warehouse Locations
- Customers
- Suppliers
- Employees
- Products
- Units
- Currencies
- Companies
- Factories

Master Data is shared across all business modules.

---

# 6. Transaction Data

Transaction Data stores operational records.

Entities include:

- Receiving Lots
- Materials
- Material Transformations
- Inventory Movements
- Work Orders
- Operations
- Packages
- Shipments
- Purchase Orders
- Sales Orders
- Maintenance Orders
- Tool Installations
- Quality Events

Transaction records are immutable whenever possible.

---

# 7. Event Store

Every completed business action generates an event.

Examples:

- MaterialReceived
- MaterialCreated
- MaterialSplit
- MaterialMerged
- MaterialConsumed
- MaterialProduced
- MaterialRecovered
- QualityApproved
- MachineStarted
- MachineStopped
- PackageCreated
- ShipmentCompleted

Events are never modified.

Events are never deleted.

---

# 8. Analytics Data

Analytics tables contain summarized information.

Examples:

- KPI Snapshots
- Daily Production
- Daily Inventory
- Machine Statistics
- Waste Statistics
- AI Predictions
- Dashboard Snapshots

Analytics data may be regenerated from source data.

---

# 9. Audit Data

Audit data records user activity.

Examples:

- Login History
- Permission Changes
- Record Changes
- Approval History
- Security Events

Audit records are immutable.

---

# 10. Primary Key Strategy

Every entity shall use:

UUID

as its primary key.

Human-readable business codes are stored separately.

Example:

UUID

↓

550e8400-e29b-41d4-a716-446655440000

Business Code

↓

THM-PN-000254

---

# 11. Business Codes

Business codes are defined by Naming_Standards.md.

Business codes may be displayed to users.

Internal UUIDs are used for relationships.

---

# 12. Material Identity

Every physical material has:

UUID

Business Code

Receiving Lot

Material Type

Species

Quality

Dimensions

Current Status

Current Location

Parent Relationship

Creation Timestamp

The UUID never changes.

Business attributes may change.

---

# 13. Transformation Model

Material transformations are stored separately.

Transformation Types:

- Split
- Merge
- Conversion
- Recovery
- Scrap

Transformation records connect parent and child materials.

This preserves complete genealogy.

---

# 14. Inventory Model

Inventory is movement-based.

Current stock is calculated from inventory movements.

Inventory entities:

- Warehouse
- Location
- Inventory Movement
- Reservation

No stock value is manually edited.

---

# 15. Quality Model

Quality data is stored independently.

Entities:

- Quality Event
- Defect
- Inspection
- Measurements
- Attachments

Quality history remains permanently accessible.

---

# 16. Machine Model

Machine data includes:

- Machine Master
- Machine Events
- Operating Hours
- Downtime
- OEE History
- Maintenance History

---

# 17. Tooling Model

Tooling data includes:

- Tool
- Cutter Head
- Tool Assembly
- Recipe
- Sharpening
- Tool Life

---

# 18. Package Model

Packages are independent entities.

A package references many materials.

One package belongs to one shipment.

---

# 19. Shipment Model

Shipment includes:

Customer

Packages

Vehicle

Driver

Delivery Date

Documents

---

# 20. Relationship Principles

Relationships use UUID.

Foreign keys enforce integrity.

Cascade delete is prohibited.

Soft delete is preferred.

---

# 21. Soft Delete Strategy

Business records shall never be permanently removed.

Entities include:

Created At

Updated At

Deleted At

Deleted By

Historical information remains available.

---

# 22. Timestamp Strategy

Every entity contains:

Created At

Updated At

Created By

Updated By

Critical entities additionally include:

Approved At

Approved By

---

# 23. File Storage

Large files are not stored inside the database.

Files include:

Photos

Technical Drawings

Certificates

PDF Documents

DXF Files

STEP Files

Images

Only file references are stored.

---

# 24. Performance Strategy

Indexes shall be created for:

UUID

Business Code

Material Code

Work Order

Machine

Package

Shipment

Timestamp

Frequently queried fields.

---

# 25. Partitioning Strategy

Large tables may be partitioned by:

Factory

Year

Month

Event Type

This supports long-term scalability.

---

# 26. Backup Strategy

Daily Backup

Weekly Backup

Monthly Archive

Point-in-Time Recovery

Backup verification is mandatory.

---

# 27. Security

Database access is role-based.

Production database is never accessed directly by users.

Sensitive information is encrypted.

Passwords are never stored in plain text.

---

# 28. Scalability

The database supports:

- Multiple Factories
- Multiple Warehouses
- Multiple Companies
- Multi-language
- Multi-currency
- Cloud Deployment
- On-Premise Deployment

---

# 29. Design Principles

- UUID-based architecture
- Fully normalized business data
- Immutable event history
- Traceability-first design
- Material-centric data model
- API-first compatibility
- AI-ready structure
- Future-proof schema

---

# 30. Future Extensions

The architecture is prepared for:

- CLT Production
- Glulam Production
- CNC Manufacturing
- Digital Twin
- BIM Integration
- Carbon Tracking
- Digital Product Passport
- Machine Vision
- IoT Integration
- Advanced AI Analytics
