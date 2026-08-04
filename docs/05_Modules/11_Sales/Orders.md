# Orders Module

**Project:** Naswood OS

**Document:** Orders

**Module Code:** MOD-SLS-ORD-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Orders module manages the complete lifecycle of customer sales orders from confirmation through production, logistics, delivery and invoicing.

It orchestrates production planning, inventory allocation, shipment execution and financial processes while providing complete traceability and AI-assisted order optimization.

The module serves as the Sales Order Execution System (SOES) of Naswood OS.

---

# 2. Objectives

- Centralize sales orders
- Automate production initiation
- Improve delivery performance
- Reduce order processing time
- Ensure full traceability
- Support AI-assisted order management
- Synchronize Digital Twin

---

# 3. Order Lifecycle

Quotation

↓

Customer Approval

↓

Sales Order

↓

Credit Check

↓

Capacity Check

↓

Inventory Allocation

↓

Production Planning

↓

Production

↓

Quality Approval

↓

Packaging

↓

Shipment

↓

Delivery

↓

Invoice

↓

Archive

---

# 4. Order Types

Standard Order

Dealer Order

Export Order

Project Order

Sample Order

Replacement Order

Warranty Order

Consignment Order

Framework Agreement

Call-Off Order

---

# 5. Order Header

Order Number

Customer

Dealer

Project

Currency

Sales Representative

Order Date

Requested Delivery

Confirmed Delivery

Payment Terms

Incoterms

Priority

Status

---

# 6. Order Lines

Product

Species

Grade

Profile

Dimensions

Quantity

Unit

Price

Discount

Tax

Warehouse

Production Route

Packaging Type

---

# 7. Production Integration

Production Order

Production Status

Routing

Machine Assignment

Production Batch

Capacity Reservation

Estimated Completion

Quality Hold

---

# 8. Inventory Integration

Available Stock

Reserved Stock

ATP (Available to Promise)

CTP (Capable to Promise)

Warehouse

Lot Number

Batch Number

Material Reservation

---

# 9. Logistics Integration

Shipment Plan

Packaging

Pallet Configuration

Container Planning

Vehicle Assignment

Tracking Number

Delivery Confirmation

Proof of Delivery

---

# 10. Financial Integration

Invoice

Proforma Invoice

Advance Payment

Credit Limit

Payment Status

Exchange Rate

Profitability

Margin

Cost Analysis

---

# 11. Customer Integration

Customer Profile

Customer Preferences

Previous Orders

Project History

Special Requirements

Technical Documents

Warranty Terms

---

# 12. Quality Integration

Inspection Plan

Quality Hold

Certificate Requirements

Moisture Report

Dimensional Report

Thermowood Report

CE/FSC/EPD Documents

---

# 13. Digital Product Passport

DPP ID

Batch Traceability

Material Genealogy

Production History

Certificates

Environmental Data

QR Code

---

# 14. AI Capabilities

Delivery Prediction

Capacity Optimization

Inventory Recommendation

Production Prioritization

Shipment Optimization

Profitability Prediction

Customer Risk Prediction

Order Copilot

---

# 15. Digital Twin Integration

Order Timeline

Production Progress

Shipment Tracking

Customer Delivery Journey

Factory Capacity View

Scenario Simulation
---

# Project-Based Order Management

## Purpose

Naswood primarily delivers products for architectural, construction and industrial projects rather than standard stock sales.

Therefore, every Sales Order shall support complete project-based planning, production, logistics and installation management.

This functionality enables the entire factory to execute production according to the project's construction schedule rather than only the requested delivery date.

---

## Project Information

Every Sales Order may be linked to a Project.

Project Information includes:

- Project ID
- Project Name
- Customer
- Dealer
- Architect
- Contractor
- Investor
- Country
- City
- Construction Site
- Building Type
- Project Status
- Project Manager
- Site Manager
- Installation Company

---

## Construction Structure

Large projects shall support hierarchical structures.

Example:

Project

↓

Phase

↓

Block

↓

Building

↓

Floor

↓

Zone

↓

Facade

↓

Room

↓

Installation Area

↓

Order Line

Each Sales Order Line may reference one installation location.

---

## Delivery Planning

Orders shall support phased deliveries.

Delivery Types

- Phase Delivery
- Block Delivery
- Floor Delivery
- Facade Delivery
- Zone Delivery
- Product Group Delivery
- Batch Delivery
- Container Delivery
- Partial Delivery
- Emergency Delivery

Each delivery shall maintain independent logistics and traceability.

---

## Installation Planning

Each order line may contain installation information.

Fields:

Installation Sequence

Installation Date

Installation Team

Installation Priority

Installation Area

Required Equipment

Required Accessories

Installation Notes

Estimated Installation Duration

Completion Status

---

## Production Planning Integration

Production Planning shall automatically consider:

- Construction schedule
- Installation sequence
- Delivery milestones
- Site readiness
- Material availability
- Factory capacity
- Machine availability
- Production lead time

Production shall prioritize installation sequence instead of order entry date whenever required.

---

## Logistics Integration

Shipment planning shall support:

- Multi-site delivery
- Multi-phase delivery
- Mixed containers
- Mixed trucks
- Cross-docking
- Warehouse staging
- Delivery appointments
- GPS tracking

---

## Material Traceability

Every installation area shall maintain complete traceability.

Traceability includes:

- Production Order
- Batch Number
- Material Genealogy
- Package Number
- Pallet Number
- Container Number
- Shipment Number
- Delivery Confirmation
- Installation Confirmation

---

## Quality Integration

Quality records shall be linked to project locations.

Each installation area may include:

- Moisture Report
- Dimensional Inspection
- Color Classification
- Surface Inspection
- Final Inspection
- CE Documents
- FSC / PEFC Certificates
- EPD
- Digital Product Passport

---

## Project Timeline

Every project shall maintain a unified timeline.

Example:

Quotation

↓

Order

↓

Engineering Approval

↓

Production Started

↓

Production Completed

↓

Packaging

↓

Shipment

↓

Delivered

↓

Installed

↓

Warranty Started

↓

Project Closed

---

## AI Capabilities

AI shall support project-based optimization.

Capabilities include:

- Delivery sequencing
- Installation sequencing
- Capacity optimization
- Production prioritization
- Shipment optimization
- Delay prediction
- Material availability prediction
- Site readiness prediction
- Resource optimization

---

## Dashboard Widgets

Project Progress

Project Deliveries

Installation Progress

Delayed Milestones

Production Readiness

Shipment Status

Installation Status

AI Risk Analysis

---

## Business Rules

Every project order shall support phased execution.

Every delivery shall be traceable to a project location.

Production priorities may follow construction milestones.

Installation sequence shall override shipment sequence when required.

All project revisions shall remain fully auditable.

Project completion shall automatically archive all associated production, logistics and quality records.
---

# 16. Dashboard Widgets

Open Orders

Production Status

Delayed Orders

Orders by Customer

Orders by Dealer

Revenue

Order Fulfillment Rate

AI Recommendations

---

# 17. Reports

Sales Order Report

Order Backlog Report

Production Allocation Report

Delivery Performance Report

Customer Order History

Profitability Report

Export Order Report

AI Order Report

---

# 18. API Resources

GET /orders

GET /orders/{id}

GET /orders/open

GET /orders/status

GET /orders/production

GET /orders/shipment

POST /orders

POST /orders/confirm

POST /orders/cancel

POST /orders/release

POST /orders/ship

POST /orders/complete

---

# 19. Events

OrderCreated

OrderConfirmed

CapacityReserved

InventoryReserved

ProductionStarted

ProductionCompleted

ShipmentCreated

Delivered

InvoiceCreated

AIRecommendationGenerated

---

# 20. Mobile

Order Lookup

Order Approval

Shipment Tracking

QR Scan

Proof of Delivery

Photo Capture

Digital Signature

Offline Mode

---

# 21. Business Rules

Every order shall have a unique identifier.

Confirmed orders shall reserve production capacity.

Inventory shall be allocated before production release.

Production shall start only after engineering approval when required.

Every shipment shall be linked to its originating order.

Orders shall maintain complete traceability throughout their lifecycle.

All order revisions shall be version-controlled.

---

# 22. Future Extensions

Customer Portal

Dealer Portal

Real-Time Factory Tracking

Digital Contracts

Blockchain Order Verification

Industry 5.0

Digital Thread

MCP Order Agents

---

# 23. Architecture Review

## Database Changes

sales_orders

sales_order_lines

sales_order_revisions

sales_order_allocations

sales_order_status

sales_order_shipments

sales_order_finance

sales_order_documents

sales_order_ai

sales_order_events

sales_order_history

sales_order_dpp

## Related Modules

CRM

Customers

Dealers

Quotations

Production_Orders

Production_Planning

Scheduling

Inventory

Warehouse

Reservations

Finished_Goods

Packaging

Logistics

Finance

Quality_Control

Digital_Product_Passport

Analytics

AI

Digital_Twin

## Application Updates

API_Contracts.md

Dashboard_Definitions.md

Report_Catalog.md

Screen_Catalog.md

UI_Flows.md

Events.md

Notification_System.md

Customer_Portal.md

Dealer_Portal.md

Mobile_App.md

## Naswood-Specific Enhancements

### Order Intelligence

- Project-based sales orders
- Multi-stage delivery scheduling
- Project milestone deliveries
- Species-specific production validation
- Thermowood order management
- Mass timber order management

### Production Intelligence

- Automatic production order generation
- Dynamic capacity reservation
- ATP & CTP calculations
- Machine availability validation
- Material reservation
- Production priority engine

### Logistics Intelligence

- Pallet optimization
- Container optimization
- Multi-shipment management
- Export documentation
- Delivery appointment scheduling
- GPS shipment tracking

### Quality Intelligence

- FSC / PEFC certificate assignment
- CE / EPD documentation
- Moisture reports
- Quality certificates
- Batch genealogy
- Digital Product Passport integration

### Commercial Intelligence

- Framework agreement releases
- Dealer order management
- Customer-specific pricing
- Margin analysis
- Currency management
- Export order workflows

### AI Optimization

- Intelligent order prioritization
- Delivery date prediction
- Capacity optimization
- Margin optimization
- Late delivery risk prediction
- Customer demand forecasting

### Digital Twin

- Live order progress visualization
- Factory order timeline
- Production bottleneck visualization
- Shipment journey tracking
- What-if order scheduling simulation
