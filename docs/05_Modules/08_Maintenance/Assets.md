# Assets Module

**Project:** Naswood OS

**Document:** Assets

**Module Code:** MOD-MNT-AST-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Assets module manages the complete lifecycle of physical production assets including machinery, equipment, infrastructure, utilities, mobile equipment and tooling.

It provides centralized asset management, Digital Twin synchronization, lifecycle analytics, AI-driven asset intelligence and complete maintenance traceability.

The module serves as the Enterprise Asset Management System (EAMS) of Naswood OS.

---

# 2. Objectives

- Centralize all asset information
- Improve equipment availability
- Extend asset lifecycle
- Optimize maintenance planning
- Reduce total ownership cost
- Support AI-assisted asset management
- Synchronize Digital Twin

---

# 3. Asset Lifecycle

Acquisition

↓

Installation

↓

Commissioning

↓

Operation

↓

Maintenance

↓

Upgrade

↓

Relocation

↓

Decommission

↓

Disposal

↓

Archive

---

# 4. Asset Categories

Production Equipment

Production Lines

Thermowood Furnaces

Kiln Systems

Panel Presses

Finger Joint Lines

Planers

Profilers

Saws

Conveyors

Packaging Equipment

Dust Collection Systems

Compressors

Boilers

Forklifts

Overhead Cranes

Utility Systems

Buildings

Electrical Infrastructure

IT Equipment

Measuring Devices

Tools

Molds

Jigs

Fixtures

---

# 5. Asset Information

Asset ID

Business Code

Asset Name

Category

Manufacturer

Brand

Model

Serial Number

Production Year

Country of Origin

Asset Status

Asset Criticality

Responsible Department

Responsible Person

---

# 6. Technical Specifications

Power

Voltage

Current

Frequency

Pressure

Temperature Limits

Capacity

Maximum Speed

Operating Hours

Cycle Count

Dimensions

Weight

Energy Class

IP Rating

Software Version

Firmware Version

PLC Version

---

# 7. Asset Hierarchy

Company

↓

Factory

↓

Building

↓

Production Area

↓

Production Line

↓

Machine

↓

Subassembly

↓

Component

↓

Sensor

---

# 8. Asset Status

Planned

Installed

Commissioning

Operational

Idle

Maintenance

Breakdown

Standby

Retired

Disposed

---

# 9. Asset Criticality

Critical

High

Medium

Low

Utility

Support Equipment

Laboratory

Infrastructure

---

# 10. Asset Performance

Availability

Reliability

Maintainability

OEE

MTBF

MTTR

Operating Hours

Energy Consumption

Production Capacity

Efficiency Score

Health Score

---

# 11. Asset Documentation

Technical Manuals

Electrical Drawings

Mechanical Drawings

Hydraulic Diagrams

Pneumatic Diagrams

PLC Programs

Software Backups

Certificates

Calibration Records

Warranty

Service Contracts

Risk Assessments

---

# 12. Asset Integration

Work Orders

Preventive Maintenance

Corrective Maintenance

Predictive Maintenance

Spare Parts

Inventory

Purchasing

Production Orders

Production Planning

Energy Management

Quality

SCADA

IoT

---

# 13. Material Genealogy

Associated Production Orders

Equipment History

Maintenance History

Failure History

Calibration History

Installed Components

Asset BOM

---

# 14. Sustainability

Energy Consumption

Carbon Emissions

Lifecycle Carbon

Water Consumption

Lubricants

Waste Generation

ESG Indicators

---

# 15. AI Capabilities

Asset Health Prediction

Remaining Useful Life (RUL)

Failure Prediction

Maintenance Recommendation

Energy Optimization

Performance Benchmarking

Lifecycle Cost Prediction

Asset Copilot

---

# 16. Digital Twin Integration

Live Asset Status

3D Equipment Model

Sensor Overlay

PLC Status

SCADA Integration

Equipment Timeline

Historical Replay

Simulation

---

# 17. Dashboard Widgets

Asset Health

Equipment Availability

Critical Assets

Asset Utilization

Energy Consumption

Maintenance Status

MTBF

MTTR

AI Recommendations

---

# 18. Reports

Asset Register Report

Equipment Health Report

Lifecycle Report

Asset Performance Report

Reliability Report

Calibration Report

Warranty Report

AI Asset Report

---

# 19. API Resources

GET /assets

GET /assets/{id}

GET /assets/hierarchy

GET /assets/status

GET /assets/performance

GET /assets/health

POST /assets

POST /assets/update

POST /assets/commission

POST /assets/retire

---

# 20. Events

AssetCreated

AssetCommissioned

AssetActivated

AssetUpdated

AssetHealthChanged

AssetRetired

WarrantyExpired

CalibrationDue

AIRecommendationGenerated

---

# 21. Mobile

QR Scan

RFID Scan

Asset Lookup

Equipment History

Photo Capture

Video Capture

Voice Notes

Offline Mode

Digital Signature

---

# 22. Business Rules

Every production asset shall have a unique identifier.

Every maintenance activity shall reference an asset.

Critical assets require preventive maintenance plans.

Asset documentation shall be version-controlled.

Equipment hierarchy shall remain consistent.

Installed components shall update Asset BOM.

Asset history shall remain immutable.

---

# 23. Future Extensions

3D CAD Integration

BIM Integration

AR Asset Navigation

Remote Monitoring

Autonomous Inspection Robots

Digital Thread

Industry 5.0

MCP Asset Agents

---

# 24. Architecture Review

## Database Changes

assets

asset_categories

asset_hierarchy

asset_status

asset_health

asset_documents

asset_bom

asset_components

asset_calibrations

asset_warranty

asset_contracts

asset_costs

asset_energy

asset_history

asset_ai

asset_events

asset_locations

asset_sensor_links

asset_versions

## Related Modules

Work_Orders

Preventive_Maintenance

Corrective_Maintenance

Predictive_Maintenance

Spare_Parts

Inventory

Warehouse

Purchasing

Suppliers

Production_Orders

Production_Planning

Scheduling

Energy_Management

Quality

SCADA

IoT

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

Mobile_App.md

## Naswood-Specific Enhancements

### Factory Intelligence

- Factory → Line → Machine hierarchy
- Digital Asset Register
- Asset QR & RFID identification
- GPS tracking for mobile assets
- Asset criticality classification

### Equipment Intelligence

- Ledinek production line management
- Hundegger CNC asset management
- Weinig planer and moulder assets
- Thermowood furnace assets
- Kiln systems
- Dust collection assets
- Boiler and compressor management
- Forklift fleet management

### Technical Intelligence

- Digital equipment manuals
- PLC backup management
- Firmware and software version tracking
- Electrical & hydraulic documentation
- CAD drawing linkage
- Maintenance video library

### Lifecycle Intelligence

- Acquisition cost tracking
- Total Cost of Ownership (TCO)
- Lifecycle cost analysis
- Warranty monitoring
- Service contract management
- End-of-life planning

### AI Optimization

- Remaining Useful Life prediction
- Health score calculation
- Failure probability analysis
- Benchmark against similar assets
- Lifecycle optimization
- Asset investment recommendations

### Digital Twin

- Live 3D asset visualization
- Equipment sensor overlay
- Maintenance timeline replay
- Health heat maps
- What-if lifecycle simulations
