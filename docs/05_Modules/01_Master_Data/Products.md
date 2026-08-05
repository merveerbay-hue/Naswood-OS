# Product Management Module

**Project:** Naswood OS

**Document:** Product Management Module

**Version:** 1.0

**Status:** Approved

---

# 1. Module Overview

## Module Name

Product Management

## Module Code

MOD-PRD

## Module Category

Product Management

---

## Description

Product Management owns the single Product Master used throughout Naswood.

Products may represent Raw Material, Semi Finished, Finished Good, Consumable,
Packaging, Service, Tool or Spare Part.

Each Product revision has an approved Product Type and separate versioned
Capability Profile controlling its eligibility for Inventory, Production,
Purchasing, Sales, Quality, Maintenance and Planning.

---

## Objectives

- Maintain a centralized product catalog
- Standardize enterprise Product definitions
- Support all capability-enabled modules
- Maintain Product Type and capability governance
- Support multiple product families
- Enable product versioning
- Support multilingual product information

---

# 2. Business Scope

## Included Functions

Product Registration

Product Classification

Product Families

Product Type

Technical Specifications

Capability Management

Classification

Product Images

Product Documents

Product Lifecycle

Product Versioning

Customer Specific Products

---

## Excluded Functions

Customer-specific Pricing and Discounts

BOM Ownership

Routing Ownership

Material Master and Physical Material

Material Transformations

Inventory Transactions

Machine Operations

Production Execution

Accounting

---

## Dependencies

Inventory Material Master

Sales

Manufacturing

Production

Inventory

Workflow

Analytics

AI

---

# 3. User Roles

Sales Manager

Sales Engineer

Product Manager

Production Planner

Export Manager

Administrator

AI Agent

---

# 4. Business Processes

Create Product

↓

Technical Review

↓

Commercial Approval

↓

Release

↓

Sales Usage

↓

Revision

↓

Archive

---

# 5. Screens

Product List

Product Detail

Create Product

Edit Product

Product Families

BOM

Routing

Price Lists

Documents

Product Dashboard

---

# 6. User Actions

Create

Update

Archive

Release

Duplicate

Import

Export

Attach Documents

Upload Images

Assign BOM

Assign Routing

---

# 7. Data Model

Primary Entity

Product

Business Code

PRD-000001

Related Entities

Product Family

BOM

Routing

Materials

Price Lists

Sales Orders

Customer Products

Documents

Images

---

# 8. Product Families

Thermowood

Decking

Cladding

Facade Systems

Pergola Systems

Massive Panel

CLT

Glulam

Finger Joint

Profiles

Doors

Windows

Outdoor Products

Custom Products

---

# 9. Product Attributes

Product Code

Product Name

Short Description

Product Family

Product Type

Current Capability Profile ID

Species

Dimensions

Surface Finish

Strength Class

Fire Class

Durability Class

Thermowood Class

Certification

Unit

Standard Length

Weight

Volume

Packaging Type

Status

Revision

Language

---

# 10. Product Lifecycle

Draft

↓

Technical Review

↓

Approved

↓

Released

↓

Revision

↓

Archived

---

# 11. Product States

Draft

Under Review

Approved

Released

Obsolete

Archived

---

# 12. Business Rules

Every Product shall have a unique Business Code.

Every Product belongs to one Product Family.

Every Product revision shall have one Product Type and one Active versioned
Capability Profile for an effective instant.

Capabilities are enum-based domain values. Boolean capability fields are
prohibited.

Product Type defaults follow
`docs/13_Design/01_Product_Management/Product_Type_and_Capabilities.md`.

Released Products cannot be deleted.

Archived Products remain searchable.

Product creation or release shall never create Material or Inventory
automatically.

Inventory creates physical Material only from authorized posted physical
transactions.

Manufacturing owns BOM and Routing. Product Management does not modify those
aggregates.

---

# 13. Workflow

Draft

↓

Review

↓

Approval

↓

Release

↓

Revision

↓

Archive

---

# 14. Events

ProductCreated

ProductUpdated

ProductReleased

ProductArchived

ProductRevisionCreated

ProductCapabilityProfileCreated

ProductCapabilityProfileApproved

ProductCapabilityProfileActivated

ProductCapabilityProfileSuperseded

ProductTypeChanged

---

# 15. Notifications

Product Approved

Revision Released

Capabilities Updated

Document Missing

Certification Expiring

---

# 16. Permissions

View

Create

Update

Approve

Release

Archive

Export

Manage Prices

Manage BOM

Manage Routing

---

# 17. Audit Log

Product Created

Product Updated

Revision Changed

Price Updated

BOM Changed

Routing Changed

Status Changed

---

# 18. Reports

Product Catalog

Product Family Summary

Product Revisions

Product Sales

Product Profitability

BOM Usage

Routing Summary

---

# 19. Dashboard Widgets

Total Products

Products by Family

Released Products

Pending Reviews

Top Selling Products

Revenue by Product

New Products

---

# 20. KPIs

Released Products

Revision Frequency

Time to Release

Sales per Product

Product Profitability

Product Portfolio Growth

---

# 21. Mobile Support

Product Search

Product Detail

Product Documents

Product Images

QR Lookup

---

# 22. AI Capabilities

Product Recommendation

Cross Selling Suggestions

Demand Forecast

Product Classification

Specification Assistant

Technical Document Assistant

---

# 23. API Resources

GET /products

GET /products/{id}

POST /products

PATCH /products/{id}

GET /products/search

GET /products/{id}/bom

GET /products/{id}/routing

---

# 24. Integrations

Materials

BOM

Routing

Sales

CRM

Production Planning

Inventory

Analytics

AI

---

# 25. Printing

Product Catalog

Technical Datasheet

Product Label

QR Product Card

Marketing Sheet

---

# 26. Security

Role-Based Access

Revision Control

Commercial Data Protection

Audit Logging

---

# 27. Error Handling

Duplicate Product Code

Duplicate Product Name (within same family)

Missing BOM

Missing Routing

Invalid Revision

---

# 28. Performance Requirements

Product Search < 2 seconds

Product Detail < 1 second

Support 100,000+ products

Bulk Import

Bulk Export

---

# 29. Future Enhancements

Product Configurator

Variant Generator

Customer Product Catalog

Digital Product Passport

BIM Object Library

Carbon Footprint per Product

---

# 30. Acceptance Criteria

✓ Product created

✓ Product released

✓ Product family assigned

✓ BOM assigned

✓ Routing assigned

✓ Events generated

✓ Audit Logs generated

✓ Mobile supported

✓ AI integrated

---

# 31. Related Documents

Materials Module

Sales Module

Production Module

Routing Rules

Database Schema

Workflow

API Contracts

Analytics

Dashboard Definitions

---

# 32. Operational Metrics

Success Metrics

- Product release time
- Product data completeness
- Active product ratio

Failure Metrics

- Missing BOM
- Missing Routing
- Obsolete products still in use

Operational Risks

- Incorrect product definition
- Wrong product version
- Incomplete technical documentation

Monitoring Alerts

- Production-enabled Product without a released BOM
- Production-enabled Product without a released Routing
- Expired certification
- Pending technical review

Recovery Procedure

Restore previous product revision using version history and Audit Logs.

---

# Module Philosophy

Product Management maintains one canonical Product Master for every approved
Product Type.

Product capabilities determine which modules may use a released Product.
Product is a definition; Material is physical inventory owned by Inventory.

Manufacturing owns how a Product is made through BOM, Routing and Operation
definitions. Sales owns customers and sales transactions. No module duplicates
the Product Master.
