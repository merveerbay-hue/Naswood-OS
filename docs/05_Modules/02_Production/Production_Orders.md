# Production Orders Module

**Project:** Naswood OS

**Document:** Production Orders

**Module Code:** MOD-PROD-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

Production Orders are the central execution engine of Naswood OS.

Every manufacturing activity begins with a Production Order and ends with Finished Goods, Packaging and Shipment.

The Production Order orchestrates:

- Materials
- Inventory
- Warehouses
- Routing
- Operations
- Recipes
- Machines
- Tooling
- Operators
- Quality
- Maintenance
- Packaging
- Finished Goods
- Logistics
- AI
- Digital Twin

---

# 2. Production Order Types

## Make To Stock

Standard production.

---

## Make To Order

Customer specific production.

---

## Project Production

Large construction projects.

---

## Export Production

Export-only production.

---

## Thermowood Batch

Thermowood furnace batches.

---

## Kiln Drying Batch

Drying operations.

---

## Massive Panel Production

---

## Finger Joint Production

---

## CLT Production

Future

---

## Glulam Production

Future

---

## Pellet Production

---

## Rework Order

---

## Prototype Order

---

# 3. Production Lifecycle

Draft

↓

Planned

↓

Material Reserved

↓

Material Allocated

↓

Released

↓

Scheduled

↓

Running

↓

Paused

↓

Quality Inspection

↓

Packaging

↓

Finished Goods

↓

Shipment Ready

↓

Closed

↓

Archived

---

# 4. Production Sources

Sales Order

Forecast

Minimum Stock

Customer Project

Internal Request

Engineering Change

AI Generated Production Suggestion

---

# 5. Material Planning

Required Materials

Reserved Materials

Allocated Materials

Alternative Materials

Substitute Materials

Rejected Materials

Returned Materials

Waste Materials

Recovered Materials

By-products

---

## Material Availability Check

Automatic

Manual

AI Prediction

Warehouse Recommendation

Supplier Recommendation

---

# 6. Routing Integration

Primary Routing

Alternative Routing

Emergency Routing

Rework Routing

Parallel Routing

Dynamic Routing

AI Optimized Routing

---

# 7. Operation Generation

Every Production Order automatically generates:

Operations

Setup Operations

Inspection Operations

Cleaning Operations

Packaging Operations

Transfer Operations

---

# 8. Work Centers

Timber Yard

Primary Saw Line

Prism Line

Kiln

Thermowood

Sorting Line

Scanner

Finger Joint

Planer

Profiling

Massive Panel

CLT

Packaging

Finished Goods Warehouse

Shipping

---

# 9. Machine Assignment

Machine Group

Preferred Machine

Alternative Machine

AI Machine Selection

Capacity Check

Maintenance Check

Energy Check

---

# 10. Tool Assignment

Knife Set

Tool Group

Tool Assembly

Tool Life

Calibration Status

Replacement Prediction

---

# 11. Recipe Assignment

Drying Recipe

Thermowood Recipe

Glue Recipe

Press Recipe

Profil Recipe

Customer Recipe

AI Recipe Recommendation

---

# 12. Quality Gates

Incoming Inspection

First Piece Inspection

In Process Inspection

Final Inspection

Packaging Inspection

Shipment Approval

Customer Approval

---

# 13. Material Genealogy

Every Production Order maintains complete genealogy.

Log

↓

Prism

↓

Kiln

↓

Thermowood

↓

Profil

↓

Finished Goods

↓

Package

↓

Pallet

↓

Container

↓

Shipment

↓

Customer

---

Parent Material

Child Material

Transformation History

Batch Relationships

Material Tree

---

# 14. Package Integration

Package Assignment

Bundle Assignment

Pallet Assignment

Container Assignment

Shipment Assignment

Customer Packaging Rules

Private Label

---

# 15. Finished Goods Integration

Automatic FG Creation

Warehouse Allocation

Reservation

Shipment Allocation

Digital Product Passport

Certificates

---

# 16. Logistics Integration

Shipment

Carrier

Vehicle

Loading Plan

Container

Route

ETA

Tracking

---

# 17. Production Scheduling

Forward

Backward

Finite Capacity

Infinite Capacity

Shift Based

Priority Based

AI Optimized

---

# 18. Energy Management

Energy Target

Energy Consumption

Energy per m³

Energy per Batch

Machine Energy

Kiln Energy

Thermowood Energy

Carbon Impact

---

# 19. Sustainability

Carbon Footprint

FSC

PEFC

EPD

Waste Generation

Recycling

Pellet Recovery

CO₂ Tracking

---

# 20. Digital Product Passport

Automatic Generation

Material Origin

Production History

Certificates

Carbon Data

Installation Documents

Maintenance Information

QR Link

GS1 Digital Link

---

# 21. Barcode & QR

Production QR

Operation QR

Material QR

Package QR

Pallet QR

Container QR

Operator QR

Machine QR

---

# 22. Digital Twin Integration

Live Production Status

Live Material Flow

Machine Status

Operator Status

WIP Map

Warehouse Occupancy

Energy Flow

Alarm Layer

Production Timeline

---

# 23. AI Capabilities

AI Production Copilot

Natural Language Production Search

Production Scheduling

Production Rescheduling

Dynamic Routing

Machine Recommendation

Operator Recommendation

Material Shortage Prediction

Supplier Recommendation

Recipe Optimization

Thermowood Recipe Optimization

Kiln Drying Prediction

Cycle Time Prediction

Downtime Prediction

Tool Wear Prediction

Machine Failure Prediction

Production Delay Prediction

Quality Prediction

Scrap Prediction

Yield Prediction

Energy Optimization

Carbon Optimization

Production Cost Prediction

Bottleneck Detection

Root Cause Analysis

Production Simulation

Digital Twin Simulation

Autonomous Scheduling

Autonomous Material Allocation

Autonomous Routing Recommendation

---

# 24. Reports

Production Timeline

Production Status

Production Cost

Material Consumption

Material Yield

Scrap Report

Energy Report

Carbon Report

Genealogy Report

Operation History

Machine Utilization

Operator Performance

Quality Report

Thermowood Batch Report

Kiln Batch Report

Production KPI

AI Recommendation Report

---

# 25. Dashboard Widgets

Open Orders

Running Orders

Late Orders

Critical Orders

Current WIP

Material Availability

Machine Capacity

Operator Availability

Quality Holds

Packaging Queue

Shipment Queue

Energy Consumption

Carbon Footprint

Factory Bottlenecks

Thermowood Batches

Kiln Occupancy

Material Genealogy

Production Timeline

AI Recommendations

---

# 26. API

GET /production-orders

GET /production-orders/{id}

GET /production-orders/{id}/timeline

GET /production-orders/{id}/genealogy

GET /production-orders/{id}/energy

GET /production-orders/{id}/carbon

GET /production-orders/{id}/events

GET /production-orders/{id}/dpp

POST /production-orders

POST /production-orders/{id}/release

POST /production-orders/{id}/schedule

POST /production-orders/{id}/reschedule

POST /production-orders/{id}/pause

POST /production-orders/{id}/resume

POST /production-orders/{id}/complete

POST /production-orders/{id}/simulate

POST /production-orders/{id}/optimize

---

# 27. Events

ProductionOrderCreated

ProductionOrderReleased

ProductionOrderScheduled

ProductionStarted

ProductionPaused

ProductionResumed

ProductionCompleted

MaterialsReserved

OperationsGenerated

GenealogyUpdated

QualityGatePassed

PackagingStarted

FinishedGoodsCreated

ShipmentAssigned

DPPGenerated

CarbonCalculated

AIRecommendationGenerated

---

# 28. Mobile Functions

Production Queue

Operation Execution

Material Scan

Machine Scan

QR Verification

Photo Capture

Voice Notes

Offline Mode

Digital Signature

---

# 29. Business Rules

Every Production Order shall generate complete genealogy.

Every material movement shall be event driven.

Every finished product shall receive a QR Code.

Every export order shall generate a Digital Product Passport.

Thermowood production shall always be batch controlled.

Kiln drying shall always reference a drying recipe.

Packaging shall follow customer packaging rules.

Production Orders cannot be modified after closure.

---

# 30. Future Extensions

APS Integration

Vision AI Inspection

Collaborative Robots

AGV Integration

AMR Integration

IoT Edge Devices

RFID

NFC

Blockchain Manufacturing Records

Autonomous Factory

Industry 5.0

Digital Thread

MCP AI Agents
