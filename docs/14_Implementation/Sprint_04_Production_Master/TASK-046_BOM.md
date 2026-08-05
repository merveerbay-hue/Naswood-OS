# TASK-046 — Bill of Materials (BOM)

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Engineering Master Data

**Priority:** Critical

**Estimated Effort:** 10 Days

**Status:** Planned

---

# Purpose

Develop the Bill of Materials (BOM) module for Naswood OS.

The BOM module defines the complete product structure required for manufacturing finished and semi-finished products. It manages all raw materials, components, consumables and operations required for production while providing full version control and engineering traceability.

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
- Product Code
- Product Name
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

- Material Code
- Material Name
- Quantity
- Unit
- Component Type
- Warehouse
- Issue Method
- Scrap %
- Alternative Material
- Notes

---

# Component Types

Supports

- Raw Material
- Semi-Finished Product
- Purchased Component
- Packaging
- Consumable
- Adhesive
- Fastener
- Chemical

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

- Alternative Material
- Alternative Process
- Alternative Supplier

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

Automatically calculates

- Material Cost
- Component Cost
- Packaging Cost
- Waste Cost
- Standard Cost

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
- Warehouse Validation
- Batch Allocation

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
- Product exists.
- Components exist.
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
- BOMExploded

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

- Support multi-level BOM structures.
- Manage engineering revisions and versions.
- Support alternative BOMs.
- Integrate with MRP, Production and Inventory.
- Calculate standard material costs.
- Maintain complete engineering traceability.
- Publish BOM lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-016_Material.md
- TASK-017_Warehouse.md
- TASK-020_Batch.md
- TASK-012_File_Upload.md
- Production_API.md
- Validation_Rules.md

---

# Related Documents

Production_Architecture.md

Production_API.md

Production_Workflow.md

TASK-047_Routing.md

TASK-048_Work_Center.md

TASK-049_Machine.md

TASK-050_Tool.md

TASK-051_Recipe.md

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
