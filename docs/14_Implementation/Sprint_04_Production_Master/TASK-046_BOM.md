# TASK-046 — Bill of Materials (BOM)

**Module:** Manufacturing — Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Engineering Master Data

**Priority:** Critical

**Status:** Planned

---

# Purpose

Develop the Bill of Materials (BOM) module for Naswood OS.

The BOM capability defines the released Product revisions, quantities, units
and operation context required to manufacture a Product. It provides full
version control and engineering traceability without owning Product or physical
Material.

The BOM is the foundation of Production Planning, MRP, Costing, Inventory and Quality.

---

# Objectives

- Centralized BOM Management
- Engineering Version Control
- Multi-Level Product Structures
- Manufacturing Traceability
- Cost Calculation Support
- Production Planning Integration
- MRP Integration

---

# Scope

The BOM module includes

- BOM Creation
- Multi-Level BOM
- BOM Revision
- BOM Version Control
- Alternative BOM
- Effective Dates
- Component Management
- Cost Rollup
- BOM Approval
- BOM Comparison

Out of Scope

- Product Master
- Material Master and Physical Material
- Production Orders
- Routing Execution
- Inventory Transactions
- Purchasing

---

# BOM Architecture

```
Finished Product

↓

Bill of Materials

↓

Sub Assemblies

↓

Components

↓

Raw Materials

↓

Production
```

---

# BOM Lifecycle

```
Draft

↓

Engineering Review

↓

Approved

↓

Released

↓

Active

↓

Revised

↓

Obsolete

↓

Archived
```

Reference

Status_Lifecycle.md

---

# BOM Types

Supports

- Manufacturing BOM
- Engineering BOM
- Sales BOM
- Service BOM
- Phantom BOM
- Planning BOM

---

# BOM Header

Each BOM contains

## General Information

- BOM Number
- Output Product ID
- Output Product Revision ID
- Company
- Plant
- BOM Type
- Version
- Revision
- Status

---

## Validity

- Effective From
- Effective To
- Revision Date
- Approved By
- Approval Date

---

## Production Information

- Standard Batch Size
- Unit
- Yield
- Scrap Percentage
- Production Notes

Reference

Unit_Conversion.md

---

# BOM Components

Each component contains

- Component Product ID
- Component Product Revision ID
- Quantity
- Unit
- Component Type
- Issue Method
- Scrap %
- Alternative Product
- Operation ID (Optional)
- Notes

---

# Product Type and Component Role

Product Type is read from the referenced Product revision.

BOM does not define a duplicate Product Type catalog.

Component Role is a Manufacturing-owned BOM classification. Its catalog
requires separate business approval.

---

# Multi-Level BOM

Supports unlimited levels.

Example

```
CLT Panel

↓

Lamination

↓

Finger Joint Lamella

↓

Spruce Timber

↓

Adhesive
```

---

# Alternative BOM

Supports

- Alternative Product
- Alternative Process

Selection based on

- Plant
- Product
- Customer
- Production Route

---

# Version Management

Supports

```
Version 1.0

↓

Version 1.1

↓

Version 2.0
```

Each version maintains

- Revision Notes
- Effective Dates
- Engineering Approval

Previous versions remain available.

---

# Engineering Change Control

Supports

- Engineering Change Request (ECR)
- Engineering Change Order (ECO)
- Revision Approval
- Impact Analysis

Reference

Engineering Module

---

# Cost Rollup

The BOM exposes released component quantities and approved factors to the
Finance Costing capability.

Finance owns prices, valuation layers, cost methods and calculated cost
results. Manufacturing does not write financial costs into the BOM aggregate.

Reference

Finance Module

---

# MRP Integration

Workflow

```
Sales Order

↓

MRP

↓

BOM Explosion

↓

Material Requirements

↓

Purchasing

↓

Production
```

Reference

MRP Module

---

# Production Integration

Supports

- Production Orders
- Material Consumption
- Work Orders
- Shop Floor Execution

Reference

Production Module

---

# Inventory Integration

Supports

- Material Reservation
- Component Availability
- Batch Allocation

Inventory resolves physical Material using the Component Product revision.
BOM never references warehouse, batch, serial or physical Material identifiers.

Reference

Inventory Module

---

# Quality Integration

Supports

- Approved Materials
- Material Specifications
- Quality Inspection Points
- Traceability

Reference

Quality Module

---

# BOM Comparison

Supports

- Version Comparison
- Revision Comparison
- Cost Comparison
- Component Difference Analysis

---

# Attachments

Supports

- CAD Drawings
- Technical Specifications
- Assembly Drawings
- 3D Models
- Work Instructions
- Engineering Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- BOM Number
- Product
- Material
- Revision
- Version
- Status
- Plant

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active BOMs
- BOM Revisions
- Pending Approvals
- Engineering Changes
- Cost Changes
- Obsolete BOMs

Reference

Production Dashboard

---

# Reports

Supports

- BOM Register
- Multi-Level BOM
- Cost Rollup
- Revision History
- Material Usage
- Engineering Changes

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/boms

GET /api/v1/boms/{id}

POST /api/v1/boms

PUT /api/v1/boms/{id}

DELETE /api/v1/boms/{id}

POST /api/v1/boms/{id}/approve

POST /api/v1/boms/{id}/release

POST /api/v1/boms/{id}/revise

GET /api/v1/boms/{id}/explode

GET /api/v1/boms/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- BOM Number is unique.
- Output Product revision exists and is released.
- Component Product revisions exist and are released.
- Output Product has Production Capability `OUTPUT_ONLY` or `BOTH`.
- Component Products have Production Capability `CONSUMPTION_ONLY` or `BOTH`.
- Component Quantity > 0.
- Unit is valid.
- Effective Dates are valid.
- Circular references are not allowed.
- Released BOMs cannot be edited.
- Obsolete BOMs cannot be assigned to Production Orders.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Engineering Authorization
- Company Isolation
- Plant Isolation
- Revision Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- BOM Created
- BOM Updated
- BOM Approved
- BOM Released
- BOM Revised
- Component Added
- Component Removed
- Version Changed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- BOM Approval Required
- BOM Released
- Engineering Change
- Revision Published
- Obsolete BOM Warning

Reference

Notification_System.md

---

# Events

Publishes

- BOMCreated
- BOMApproved
- BOMReleased
- BOMRevised
- BOMObsolete

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- BOM Lookup
- Component View
- Revision History
- CAD Attachment Viewing

BOM editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- BOM Save < 1 second
- BOM Explosion < 2 seconds
- Multi-Level Expansion < 3 seconds
- Search < 300 ms
- Support 500,000+ BOMs
- Support 20+ BOM Levels

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Finished Product

↓

3-Layer CLT Panel

↓

Spruce Lamella

↓

PUR Adhesive

↓

Packaging
```

---

### Example 2

```
Thermowood Decking

↓

Spruce Timber

↓

Heat Treatment

↓

Packaging

↓

Label
```

---

### Example 3

```
Finger Joint Beam

↓

Lamella

↓

Finger Joint Adhesive

↓

Packaging

↓

Finished Product
```

---

# Acceptance Criteria

The BOM module shall

- Be owned exclusively by Manufacturing Production Master.
- Support multi-level BOM structures.
- Manage engineering revisions and versions.
- Support alternative BOMs.
- Integrate with MRP, Production and Inventory.
- Provide versioned component quantities to Finance Costing.
- Reference Product revisions rather than physical Material.
- Validate Product capabilities.
- Maintain complete engineering traceability.
- Publish BOM lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- Product_Management_Architecture.md
- Product_Type_and_Capabilities.md
- TASK-012_File_Upload.md
- Production_API.md
- Validation_Rules.md

---

# Related Documents

Production_Architecture.md

Production_API.md

BOM_Architecture.md

Production_Workflow.md

TASK-047_Routing.md

TASK-049_Work_Center.md

TASK-048_Machine.md

TASK-053_Tooling.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Unit_Conversion.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md
