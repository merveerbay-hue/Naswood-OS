# Material

> **Status: Superseded**
>
> This document previously combined Product, commercial, purchasing, inventory
> and physical Material concerns. It is retained for historical traceability
> and shall not be used for implementation.
>
> Canonical ownership:
>
> - Product definition, Product Type and capabilities:
>   `../01_Product_Management/Product_Management_Architecture.md`
> - Physical Material and Material Master:
>   `../02_Inventory/Material_Master.md`
> - BOM: `../05_Production/BOM_Architecture.md`

## Purpose

Material Master stores every raw material, semi-finished product and finished product used within Naswood OS.

---

## Business Rules

- Every material has a unique code.
- Material code cannot be changed after creation.
- Every material belongs to one category.
- Every material has one base unit.
- Materials may be batch tracked.
- Materials may be serial tracked.
- Materials may be active or inactive.

# Material Master

**Module:** Master Data

**Category:** Material

**Version:** 1.0

**Status:** Superseded

---

# Purpose

The Material Master defines the official structure, lifecycle and business rules for all materials managed within Naswood OS.

Every physical or virtual item used throughout purchasing, inventory, production, sales, quality, maintenance and finance shall exist as a Material Master record.

Material Master is the single source of truth for all material-related information.

---

# Objectives

- Centralized Material Management
- Eliminate Duplicate Materials
- Standardize Material Information
- Improve Traceability
- Support Manufacturing
- Enable AI Analysis

---

# Material Lifecycle

Draft

↓

Review

↓

Approved

↓

Active

↓

Blocked

↓

Obsolete

↓

Archived

---

# Material Categories

Raw Material

Semi Finished Product

Finished Product

Trading Goods

Consumables

Packaging

Chemical

Spare Part

Tool

Service

Asset

Virtual Material

Waste

Scrap

---

# Wood Material Categories

Log

Prism

Lumber

Lamella

Finger Joint

CLT Panel

Glulam

Massive Panel

Thermowood

Pellet

Wood Chips

Sawdust

---

# Material Identification

Each material receives

Material Code

UUID

Barcode

QR Code

Optional RFID

---

# Material Code Structure

Example

```
TMP-001245

RAW-000452

PEL-000124

LOG-000521
```

---

# General Information

Material Code

Material Name

Short Description

Long Description

Category

Sub Category

Material Group

Brand

Manufacturer

Country of Origin

---

# Classification

Product Family

Species

Quality Class

Strength Class

Moisture Class

Surface Finish

Usage

Industry

---

# Inventory Information

Warehouse

Storage Location

Bin

Lot Tracking

Serial Tracking

Batch Tracking

Safety Stock

Minimum Stock

Maximum Stock

Reorder Point

ABC Class

XYZ Class

---

# Purchasing Information

Preferred Supplier

Alternative Suppliers

Purchase Unit

Lead Time

MOQ

Currency

Price

Contract

Incoterms

---

# Sales Information

Sales Unit

Price List

Customer Group

Export Category

Tax

Discount Group

---

# Production Information

BOM

Routing

Production Version

Machine

Production Line

Cycle Time

Yield

Scrap %

---

# Wood Properties

Species

Density

Moisture

Thickness

Width

Length

Volume

Weight

Strength Grade

Thermal Treatment

Fire Rating

FSC

PEFC

CE

---

# Thermowood Properties

Treatment Class

Maximum Temperature

Holding Time

Cooling Process

Kiln

Batch

---

# Massive Panel Properties

Panel Thickness

Layer Count

Glue Type

Press Type

Panel Grade

---

# Pellet Properties

Diameter

Length

Density

Ash %

Calorific Value

Moisture %

---

# Quality Information

Inspection Plan

Quality Standard

Sampling

Certificates

Test Reports

CAPA

NCR

---

# Logistics

Packaging

Pallet Type

Gross Weight

Net Weight

Dimensions

Stackability

Storage Conditions

Transport Conditions

---

# Costing

Standard Cost

Average Cost

Last Purchase Price

Currency

Cost Center

GL Account

Valuation Class

---

# Sustainability

FSC

PEFC

Carbon Footprint

Recycled Content

Environmental Declaration

---

# Attachments

Photos

Technical Drawing

Specification

Certificate

SDS

Installation Guide

Manual

Videos

---

# Relationships

BOM

Alternative Material

Successor

Predecessor

Compatible Materials

Accessories

---

# AI Attributes

Demand Prediction

Suggested Supplier

Quality Prediction

Price Trend

Risk Score

Alternative Material

AI Summary

---

# Digital Twin

Machine Usage

Production History

Sensor Data

Live Status

Traceability

---

# Security

Role Permissions

Department Permissions

Approval Workflow

Audit Trail

Version History

---

# Validation Rules

Material Code must be unique.

Material Name is required.

Category is required.

Unit is required.

Warehouse assignment is mandatory for stock materials.

Species is mandatory for wood materials.

---

# Search

Supports

Code

Barcode

QR

Name

Supplier

Category

Species

Certificate

Dimensions

AI Search

---

# User Actions

Create

Edit

Duplicate

Archive

Block

Print Label

Generate QR

Export

Import

View History

---

# Integrations

Inventory

Purchasing

Production

Sales

Quality

Maintenance

Finance

CRM

AI

Digital Twin

---

# Example Material

Material Code

TMP-000245

Material Name

Thermowood Deck 26x140

Category

Finished Product

Species

Scots Pine

Thickness

26 mm

Width

140 mm

Length

3900 mm

Moisture

6%

Treatment

Thermo-D

Warehouse

FG-01

---

# Best Practices

✓ Keep one material per record.

✓ Never duplicate materials.

✓ Maintain complete specifications.

✓ Store technical documents.

✓ Enable full traceability.

✓ Maintain revision history.

---

# Do

✓ Use standardized material codes

✓ Define material lifecycle

✓ Track batches

✓ Store certifications

✓ Maintain supplier information

---

# Don't

✗ Duplicate material cards

✗ Leave mandatory fields empty

✗ Mix different units

✗ Delete historical records

✗ Ignore traceability

---

# Acceptance Criteria

Material codes are unique.

Classification is complete.

Traceability is maintained.

Material lifecycle is enforced.

Permissions are respected.

AI attributes are available.

Digital Twin integration is supported.

---

# Related Documents

Supplier.md

Customer.md

Warehouse.md

BOM.md

Routing.md

Inventory.md

Quality.md

AI_Copilot.md

Labels.md
