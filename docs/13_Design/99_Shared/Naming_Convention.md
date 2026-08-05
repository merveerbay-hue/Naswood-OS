# Naming Convention

**Module:** Shared

**Category:** Naming Standards

**Version:** 1.0

**Status:** Approved

---

# Purpose

The Naming Convention standard defines the official naming rules used throughout Naswood OS.

It ensures consistency across source code, APIs, databases, documentation, UI components, configuration files and business identifiers.

All platform assets must comply with this standard.

---

# Objectives

- Consistent Naming
- Improved Readability
- Easier Maintenance
- Predictable Development
- Better AI Code Generation
- Cross-Team Collaboration

---

# Design Principles

Naming should be

Consistent

Descriptive

Predictable

Stable

Technology Independent

Avoid abbreviations unless officially defined.

---

# General Rules

Use English for all technical identifiers.

Business terms should match the official business glossary.

Names should describe intent rather than implementation.

Avoid unnecessary prefixes and suffixes.

Avoid ambiguous abbreviations.

---

# Language Standard

Source Code

English

Database

English

API

English

Configuration

English

Documentation

English

User Interface

Localized

---

# Case Standards

## PascalCase

Use for

Classes

Interfaces

Enums

React Components

Example

```
Material

PurchaseOrder

InventoryTransaction

ProductionDashboard
```

---

## camelCase

Use for

Variables

Properties

Parameters

Methods

Functions

Example

```
materialCode

purchaseOrderId

createdAt

calculateVolume()
```

---

## snake_case

Use only where required.

Examples

```
docker-compose.yml

postgres_extensions.sql
```

---

## kebab-case

Use for

URLs

CSS Classes

File Slugs

Examples

```
production-dashboard

inventory-report

main-navigation
```

---

## UPPER_CASE

Use for

Constants

Environment Variables

Examples

```
MAX_UPLOAD_SIZE

DEFAULT_LANGUAGE

API_TIMEOUT
```

---

# File Naming

Markdown

```
Material.md

Purchase_Order.md

Inventory_Report.md
```

React Components

```
MaterialCard.tsx

InventoryTable.tsx

DashboardWidget.tsx
```

Services

```
InventoryService.cs

MaterialService.cs
```

---

# Folder Naming

Use

PascalCase

Examples

```
Inventory

Production

Quality

Reports
```

Avoid

```
inventory2

testFolder

misc
```

---

# API Naming

Use plural nouns.

Examples

```
/materials

/customers

/purchase-orders

/inventory-transactions
```

Avoid verbs.

---

# Database Naming

Tables

PascalCase

Examples

```
Material

PurchaseOrder

Warehouse
```

Columns

camelCase

Examples

```
materialCode

createdAt

updatedBy
```

Primary Key

```
id
```

Foreign Key

```
materialId

supplierId

warehouseId
```

---

# Entity Naming

Examples

Material

Customer

Supplier

Warehouse

ProductionOrder

InventoryTransaction

Machine

---

# Enum Naming

PascalCase

Example

```
OrderStatus

MachineState

ApprovalType
```

Enum Values

PascalCase

```
Draft

Approved

Completed

Cancelled
```

---

# Interface Naming

Prefix

```
I
```

Example

```
IMaterialRepository

IInventoryService

ILogger
```

---

# Event Naming

Past tense.

Examples

```
MaterialCreated

InventoryAdjusted

ProductionCompleted

MachineStopped
```

Reference

Event_Model.md

---

# Command Naming

Imperative.

Examples

```
CreateMaterial

ApprovePurchaseOrder

StartProduction

GenerateReport
```

---

# Query Naming

Examples

```
GetMaterial

SearchMaterials

ListWarehouses

FindProductionOrders
```

---

# DTO Naming

Suffix

```
Dto
```

Examples

```
MaterialDto

InventoryItemDto

PurchaseOrderDto
```

---

# ViewModel Naming

Suffix

```
ViewModel
```

Examples

```
DashboardViewModel

LoginViewModel
```

---

# Repository Naming

Suffix

```
Repository
```

Example

```
MaterialRepository

SupplierRepository
```

---

# Service Naming

Suffix

```
Service
```

Example

```
InventoryService

NotificationService
```

---

# Controller Naming

Suffix

```
Controller
```

Examples

```
MaterialController

ProductionController
```

---

# Database Constraints

Primary Key

```
PK_Table
```

Foreign Key

```
FK_Order_Customer
```

Index

```
IX_Table_Column
```

Unique

```
UK_Table_Field
```

---

# Environment Variables

UPPER_CASE

Examples

```
DATABASE_CONNECTION

JWT_SECRET

REDIS_CONNECTION

OPENAI_API_KEY
```

---

# Configuration Keys

Use

```
:

```

Examples

```
Authentication:Jwt

Storage:Azure

AI:Provider
```

---

# CSS Variables

kebab-case

Example

```
--primary-color

--spacing-lg

--shadow-md
```

---

# Test Naming

Pattern

```
Method_State_Result
```

Example

```
CreateMaterial_InvalidCode_ShouldThrowValidationError
```

---

# Logging

Use structured names.

Example

```
MaterialCreated

InventoryTransferred

LoginSucceeded
```

Reference

Logging.md

---

# Documentation

Markdown

Pascal_Case.md

Examples

```
Approval_Workflow.md

Material.md

Audit_Log.md
```

---

# Business Codes

Reference

Document_Numbering.md

Examples

```
MAT-000245

SUP-000021

PO-2026-000245
```

---

# Abbreviations

Allowed

API

AI

CRM

ERP

MES

WMS

PDF

CAD

UUID

JWT

OEE

Avoid project-specific abbreviations unless documented.

---

# Reserved Words

Do not use language or framework reserved keywords as identifiers.

Examples

```
class

namespace

public

return
```

---

# AI Compatibility

Consistent naming improves

AI code generation

AI documentation

Semantic search

Knowledge indexing

Code completion

---

# Best Practices

✓ Prefer descriptive names.

✓ Use one concept per identifier.

✓ Keep names stable.

✓ Follow the official glossary.

✓ Maintain consistency.

✓ Review naming during code review.

---

# Do

✓ Use English identifiers

✓ Use official business terms

✓ Follow casing rules

✓ Keep names predictable

✓ Standardize abbreviations

---

# Don't

✗ Mix languages

✗ Use unclear abbreviations

✗ Encode implementation details

✗ Use inconsistent casing

✗ Rename public contracts without versioning

---

# Acceptance Criteria

Naming conventions are applied consistently across the platform.

Identifiers are readable and predictable.

Public APIs remain stable.

Documentation follows the same standard.

Code reviews enforce compliance.

AI-generated code adheres to the convention.

---

# Related Documents

Architecture.md

API_Standards.md

Entity_Rules.md

Document_Numbering.md

Event_Model.md

Integration_Events.md

Logging.md

Localization.md
