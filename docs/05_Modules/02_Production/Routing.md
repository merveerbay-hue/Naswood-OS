# Routing Module

**Project:** Naswood OS

**Document:** Routing

**Module Code:** MOD-ROUT-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Routing module defines the complete manufacturing path that materials and products follow from raw material to finished goods.

A Routing consists of ordered Operations, Work Centers, Machines, Quality Gates, Material Transformations and Resource Requirements.

Routing serves as the production blueprint for every manufactured product.

---

# 2. Objectives

- Standardize production flow
- Optimize manufacturing sequence
- Support multiple production methods
- Reduce production time
- Improve traceability
- Enable AI optimization
- Synchronize Digital Twin

---

# 3. Routing Types

Standard Routing

Customer Routing

Thermowood Routing

Kiln Routing

Massive Panel Routing

Finger Joint Routing

Profil Routing

CLT Routing

Glulam Routing

Pellet Routing

Export Routing

Rework Routing

Prototype Routing

Emergency Routing

AI Optimized Routing

---

# 4. Routing Sources

Product Family

Customer

Recipe

Production Order

Engineering

Quality Requirement

Sales Order

AI Recommendation

---

# 5. Routing Structure

Routing

↓

Operation

↓

Work Center

↓

Machine Group

↓

Machine

↓

Tool Assembly

↓

Recipe

↓

Quality Gate

↓

Expected Output

↓

Next Operation

---

# 6. Standard Routing Steps

Material Receiving

Log Measurement

Log Classification

Log Yard Storage

Primary Sawing

Prism Cutting

Kiln Drying

Thermowood Treatment

Scanning

Optimization

Finger Joint

Planing

Profiling

Massive Panel Pressing

CLT Assembly

Glue Curing

Calibration

Sanding

Final Inspection

Packaging

Finished Goods

Shipment

---

# 7. Work Center Assignment

Timber Yard

Primary Saw Line

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

# 8. Machine Assignment

Preferred Machine

Alternative Machine

Backup Machine

Machine Group

Capacity Validation

Maintenance Validation

Energy Validation

AI Machine Recommendation

---

# 9. Tool Assignment

Knife Set

Tool Group

Tool Assembly

Calibration Status

Tool Life

Replacement Threshold

AI Tool Recommendation

---

# 10. Recipe Assignment

Kiln Recipe

Thermowood Recipe

Glue Recipe

Profil Recipe

Customer Recipe

Quality Recipe

AI Recipe Recommendation

---

# 11. Material Transformation

Input Materials

Intermediate Materials

Output Materials

Yield

Expected Scrap

Recovered Materials

By-products

Waste

Material Genealogy

---

# 12. Quality Gates

Incoming Inspection

Machine Setup Approval

First Piece Inspection

In Process Inspection

Dimensional Inspection

Moisture Inspection

Surface Inspection

Final Inspection

Packaging Inspection

Shipment Approval

---

# 13. Routing Constraints

Machine Capacity

Operator Skills

Shift Availability

Material Availability

Tool Availability

Energy Limits

Maintenance Windows

Warehouse Capacity

Packaging Capacity

Customer Due Date

---

# 14. Routing Versions

Draft

Under Review

Approved

Released

Obsolete

Archived

Only one version may be active for a product revision.

---

# 15. Routing Lifecycle

Draft

↓

Validation

↓

Simulation

↓

Approval

↓

Release

↓

Execution

↓

Monitoring

↓

Revision

↓

Archive

---

# 16. AI Capabilities

Dynamic Routing Recommendation

Alternative Routing Selection

Machine Load Balancing

Operator Skill Matching

Material Optimization

Yield Prediction

Cycle Time Prediction

Bottleneck Detection

Energy Optimization

Carbon Optimization

Recipe Optimization

Thermowood Optimization

Kiln Optimization

Predictive Scheduling

Root Cause Analysis

Digital Twin Simulation

Autonomous Routing Optimization

AI Routing Copilot

---

# 17. Digital Twin Integration

Live Routing Visualization

Material Flow

Operation Status

Machine Status

Work Center Status

WIP Tracking

Energy Flow

Factory Bottlenecks

Simulation

---

# 18. Dashboard Widgets

Routing Performance

Routing Versions

Operation Sequence

Machine Utilization

Routing Bottlenecks

Current WIP

Alternative Routing Usage

Quality Gate Status

Energy Consumption

Carbon Footprint

Thermowood Routing

Kiln Queue

AI Recommendations

---

# 19. Reports

Routing List

Routing Comparison

Routing History

Operation Sequence

Routing Performance

Routing Cost

Routing Time

Routing Yield

Routing Genealogy

Thermowood Routing

Kiln Routing

Quality Gates

Machine Usage

AI Routing Analysis

---

# 20. API Resources

GET /routing

GET /routing/{id}

GET /routing/{id}/operations

GET /routing/{id}/versions

GET /routing/{id}/simulation

GET /routing/{id}/genealogy

POST /routing

POST /routing/{id}/approve

POST /routing/{id}/release

POST /routing/{id}/simulate

POST /routing/{id}/optimize

PATCH /routing/{id}

---

# 21. Events

RoutingCreated

RoutingUpdated

RoutingApproved

RoutingReleased

RoutingArchived

RoutingVersionCreated

RoutingSimulationStarted

RoutingSimulationCompleted

RoutingOptimized

AlternativeRoutingSelected

QualityGateAdded

MachineAssigned

RecipeAssigned

---

# 22. Mobile

Routing Viewer

Operation Flow

QR Scan

Machine Assignment

Quality Checklist

Photo Capture

Offline Support

---

# 23. Business Rules

Every Product shall have at least one approved Routing.

Only released Routings may be used in Production Orders.

Routing changes create a new version.

All routing revisions shall preserve history.

Thermowood products require a Thermowood Routing.

Kiln operations require an approved Drying Recipe.

Customer-specific Routings override default Routings.

---

# 24. Future Extensions

Constraint-Based Routing

Vision AI Verification

Collaborative Robots

AGV Routing

AMR Routing

IoT Driven Routing

RFID Tracking

Blockchain Genealogy

Digital Thread

Industry 5.0

MCP AI Routing Agents

---

# 25. Architecture Review

## Database Changes

routings

routing_versions

routing_operations

routing_resources

routing_quality_gates

routing_constraints

routing_simulations

routing_ai_recommendations

---

## Related Modules

Production_Planning

Production_Orders

Operations

Work_Centers

Machines

Tooling

Recipes

Materials

Quality

Packaging

Finished_Goods

Warehouse

Inventory

Logistics

Analytics

AI

Digital_Twin

---

## Application Updates

API_Contracts.md

Dashboard_Definitions.md

Report_Catalog.md

Screen_Catalog.md

UI_Flows.md

Events.md

---

## Naswood-Specific Enhancements

### Timber Routing

- Log receiving optimization
- Species-based routing
- Diameter and length decision rules
- Supplier-specific routing

### Kiln & Thermowood Routing

- Automatic kiln loading sequence
- Recipe compatibility validation
- Moisture target verification
- Batch balancing
- Cooling sequence management
- Color consistency checkpoints

### Massif Panel Routing

- Lamella selection rules
- Glue spread validation
- Press sequence
- Calibration workflow

### Finger Joint Routing

- Defect scanning
- Automatic defect cutting
- Finger profile verification
- Press timing control

### Profil Routing

- Tool profile verification
- Cutter life monitoring
- Surface quality checkpoints

### Packaging & Logistics Routing

- Customer-specific packaging path
- Automatic palletization
- Container loading sequence
- Export documentation trigger

### Sustainability

- Carbon footprint per routing
- Energy consumption per operation
- Waste generation tracking
- Pellet recovery flow

### AI Planning

- AI route comparison
- Best routing recommendation
- Automatic rerouting after machine failure
- Bottleneck avoidance
- Predictive routing based on live factory conditions

### Digital Twin

- 2D/3D routing visualization
- Live WIP animation
- Material genealogy tree
- Factory flow simulation
- What-if scenario analysis
