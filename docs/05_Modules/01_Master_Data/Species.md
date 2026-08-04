
# Species Module

**Project:** Naswood OS

**Document:** Species Module

**Version:** 1.0

**Status:** Approved

---

# 1. Module Overview

## Module Name

Species

## Module Code

MOD-SPC

## Module Category

Master Data

---

## Description

The Species module defines all wood species used throughout Naswood OS.

Each species contains technical, physical and commercial properties required by manufacturing, quality, costing and sales processes.

Species are referenced by Materials, Products, Recipes and Production.

---

## Objectives

- Maintain a standardized wood species library
- Support production and quality decisions
- Standardize species properties
- Enable recipe selection
- Support costing calculations
- Improve AI recommendations

---

# 2. Business Scope

## Included Functions

Species Registration

Species Classification

Physical Properties

Mechanical Properties

Drying Properties

Thermowood Compatibility

Commercial Information

Species Status

Species Documents

Species Images

---

## Excluded Functions

Material Registration

Inventory

Sales Orders

Production Orders

Accounting

---

## Dependencies

Materials

Products

Recipes

Production

Quality

Analytics

AI

---

# 3. User Roles

Production Engineer

Quality Engineer

Sales Engineer

Product Manager

Administrator

AI Agent

---

# 4. Business Processes

Create Species

↓

Define Technical Properties

↓

Validation

↓

Approval

↓

Release

↓

Operational Usage

↓

Archive

---

# 5. Screens

Species List

Species Detail

Create Species

Edit Species

Physical Properties

Mechanical Properties

Drying Parameters

Thermowood Compatibility

Species Dashboard

---

# 6. User Actions

Create

Update

Archive

Activate

Deactivate

Export

Import

Attach Documents

Upload Images

---

# 7. Data Model

Primary Entity

Species

Business Code

SPC-PINE

Related Entities

Materials

Products

Recipes

Quality Specifications

Production Orders

Documents

---

# 8. Standard Fields

Species Code

Species Name

Botanical Name

Commercial Name

Category

Origin

Color

Grain Pattern

Texture

Density

Moisture Class

Durability Class

Strength Class

Movement Class

Workability

Machinability

Thermowood Compatible

FSC Available

PEFC Available

Status

Revision

---

# 9. Physical Properties

Average Density

Green Density

Dry Density

Shrinkage

Swelling

Moisture Range

Thermal Conductivity

Hardness

Weight

---

# 10. Mechanical Properties

Bending Strength

Compression Strength

Tensile Strength

Elasticity

Impact Resistance

Surface Hardness

Fastener Holding

---

# 11. Manufacturing Properties

Sawing Difficulty

Planing Quality

Profiling Quality

Finger Joint Compatibility

Glue Compatibility

Press Compatibility

Surface Finish

Recommended Feed Rate

Recommended Cutting Speed

Recommended Knife Angle

---

# 12. Drying Properties

Recommended Kiln Recipe

Initial Moisture

Target Moisture

Average Drying Time

Drying Sensitivity

Warping Risk

Checking Risk

---

# 13. Thermowood Properties

Thermowood Compatible

Recommended Recipe

Maximum Temperature

Holding Time

Cooling Method

Expected Color

Dimensional Stability

---

# 14. Business Rules

Species Codes are unique.

Species cannot be deleted once referenced.

Inactive species cannot be assigned to new materials.

Every species shall define minimum manufacturing properties.

---

# 15. Workflow

Draft

↓

Technical Review

↓

Approval

↓

Released

↓

Archived

---

# 16. Events

SpeciesCreated

SpeciesUpdated

SpeciesReleased

SpeciesArchived

SpeciesActivated

SpeciesDeactivated

---

# 17. Notifications

Species Awaiting Approval

Species Updated

Property Changed

Recipe Review Required

---

# 18. Permissions

View

Create

Update

Approve

Archive

Export

Manage Properties

---

# 19. Audit Log

Species Created

Properties Updated

Recipe Recommendation Changed

Status Changed

---

# 20. Reports

Species Library

Species Comparison

Species Usage

Species Performance

Species Manufacturing Guide

Species Cost Analysis

---

# 21. Dashboard Widgets

Species Count

Most Used Species

Species Distribution

Production by Species

Revenue by Species

Species Performance

---

# 22. KPIs

Species Utilization

Production by Species

Waste by Species

Yield by Species

Energy per Species

Quality Pass Rate

---

# 23. Mobile Support

Species Search

Species Detail

Technical Properties

Species QR Lookup

Read-Only Access

---

# 24. AI Capabilities

Species Recommendation

Recipe Recommendation

Machining Recommendation

Quality Prediction

Yield Prediction

Cost Prediction

Alternative Species Suggestion

---

# 25. API Resources

GET /species

GET /species/{id}

POST /species

PATCH /species/{id}

GET /species/search

---

# 26. Integrations

Materials

Products

Recipes

Production

Quality

Analytics

AI

---

# 27. Printing

Species Datasheet

Technical Properties

Species Labels

Manufacturing Guide

---

# 28. Security

Role-Based Access

Technical Data Protection

Audit Logging

---

# 29. Error Handling

Duplicate Species Code

Duplicate Botanical Name

Missing Technical Properties

Invalid Density

---

# 30. Performance Requirements

Species Search < 1 second

Species Detail < 1 second

Support unlimited species records

Bulk Import Supported

---

# 31. Future Enhancements

Automatic Species Identification

Computer Vision Integration

AI Property Prediction

Carbon Footprint by Species

Digital Product Passport

---

# 32. Acceptance Criteria

✓ Species created

✓ Technical properties defined

✓ Manufacturing properties completed

✓ Recipes linked

✓ Audit Logs generated

✓ Events generated

✓ Mobile supported

✓ AI supported

---

# 33. Related Documents

Materials Module

Products Module

Recipes Module

Production Module

Quality Module

Material Attributes

Database Schema

Analytics

---

# 34. Operational Metrics

Success Metrics

- Species data completeness
- Recipe assignment accuracy
- Manufacturing consistency

Failure Metrics

- Missing technical properties
- Invalid recipe assignments
- Species without manufacturing data

Operational Risks

- Incorrect species selection
- Wrong recipe recommendation
- Production quality loss

Monitoring Alerts

- Species without recipe
- Species without density
- Species awaiting approval

SLA

Species registration < 30 minutes

Recovery Procedure

Recover previous species definitions using Audit Logs and version history.

---

# Module Philosophy

Species represent the technical knowledge base of wood materials within Naswood OS.

Each species provides standardized physical, mechanical and manufacturing properties that support production, quality, costing and AI-assisted decision making.

By centralizing species data, Naswood OS ensures consistent manufacturing processes, accurate traceability and repeatable product quality across all production lines.
