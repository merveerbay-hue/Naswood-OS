# Kiln Batches Module

**Project:** Naswood OS

**Document:** Kiln Batches

**Module Code:** MOD-KILN-BATCH-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Kiln Batches module manages the execution, monitoring and traceability of every kiln drying batch.

A Kiln Batch represents a controlled manufacturing process where multiple materials are dried under a defined recipe and monitored through real-time telemetry.

Every Kiln Batch becomes part of the permanent Material Genealogy and Digital Product Passport.

---

# 2. Objectives

- Manage kiln batches
- Track batch lifecycle
- Monitor drying performance
- Preserve traceability
- Improve energy efficiency
- Enable AI optimization
- Synchronize Digital Twin

---

# 3. Batch Lifecycle

Batch Planned

↓

Material Reserved

↓

Kiln Loaded

↓

Recipe Assigned

↓

Ready

↓

Running

↓

Paused

↓

Conditioning

↓

Cooling

↓

Moisture Verification

↓

Quality Approved

↓

Released

↓

Completed

↓

Archived

---

# 4. Batch Types

Standard Batch

Thermowood Pre-Drying

Export Batch

Mixed Species Batch

Mixed Thickness Batch

Trial Batch

Research Batch

Rework Batch

AI Optimized Batch

---

# 5. Batch Information

Batch ID

Business Code

Batch Number

Factory

Kiln

Recipe

Operator

Supervisor

Shift

Production Order

Planning Reference

Priority

Status

---

# 6. Material Allocation

Material List

Species

Thickness

Width

Length

Moisture

Volume

Weight

Quality Grade

Package

QR Code

RFID

Genealogy Reference

---

# 7. Recipe Assignment

Drying Recipe

Recipe Version

Target Moisture

Temperature Curve

Humidity Curve

Air Velocity

Pressure

Fan Speed

Conditioning Parameters

Cooling Parameters

---

# 8. Sensor Integration

Dry Bulb Temperature

Wet Bulb Temperature

Relative Humidity

Wood Moisture

Air Velocity

Fan Speed

Steam Pressure

Valve Positions

Energy Meter

CO₂ Meter

Vibration

Door Status

---

# 9. Batch Monitoring

Running Time

Remaining Time

Current Phase

Progress %

Temperature Trend

Humidity Trend

Moisture Trend

Energy Trend

Alarm Status

Operator Notes

---

# 10. Batch Quality

Average Moisture

Moisture Distribution

Warp

Twist

Bow

Cup

Cracks

Honeycomb

Case Hardening

Color Uniformity

Final Approval

---

# 11. Energy Monitoring

Electricity

Natural Gas

Steam

Biomass

Hot Water

Energy per Batch

Energy per m³

Peak Load

Energy Cost

Carbon Emissions

---

# 12. Material Genealogy

Input Materials

Kiln Batch

Recipe

Sensor History

Operator

Machine

Output Materials

Production Order

Transformation History

Certificates

---

# 13. Sustainability

Carbon Footprint

Energy Efficiency

Waste

Recovered Material

Renewable Energy

Biomass Usage

ESG Metrics

---

# 14. Digital Product Passport

Batch History

Drying Curve

Quality Results

Certificates

Carbon Data

Energy Data

Material Origin

Traceability

---

# 15. AI Capabilities

Batch Optimization

Recipe Recommendation

Remaining Time Prediction

Energy Optimization

Quality Prediction

Defect Prediction

Moisture Prediction

Batch Grouping

Kiln Load Optimization

Alarm Prediction

Autonomous Drying

AI Kiln Copilot

---

# 16. Vision AI

Kiln Loading Verification

Stack Position Detection

Load Distribution Analysis

Surface Crack Detection

Color Uniformity Analysis

Thermal Camera Inspection

Automatic Batch Verification

---

# 17. Digital Twin Integration

Live Kiln

Live Batch

Live Sensors

Temperature Heat Map

Humidity Heat Map

Moisture Heat Map

Energy Flow

Alarm Layer

Replay

Simulation

---

# 18. Dashboard Widgets

Running Batches

Kiln Occupancy

Batch Progress

Recipe Compliance

Moisture Trend

Temperature Trend

Humidity Trend

Energy Consumption

Carbon Emissions

Quality Alerts

AI Recommendations

---

# 19. Reports

Batch Summary

Batch Timeline

Recipe Performance

Drying Curve

Moisture Report

Energy Report

Carbon Report

Quality Report

Batch Comparison

Kiln Utilization

AI Batch Analysis

---

# 20. API Resources

GET /kiln-batches

GET /kiln-batches/{id}

GET /kiln-batches/{id}/telemetry

GET /kiln-batches/{id}/genealogy

GET /kiln-batches/{id}/energy

GET /kiln-batches/{id}/quality

GET /kiln-batches/{id}/timeline

POST /kiln-batches

POST /kiln-batches/{id}/start

POST /kiln-batches/{id}/pause

POST /kiln-batches/{id}/resume

POST /kiln-batches/{id}/complete

POST /kiln-batches/{id}/simulate

POST /kiln-batches/{id}/optimize

---

# 21. Events

BatchCreated

BatchLoaded

RecipeAssigned

BatchStarted

BatchPaused

BatchResumed

PhaseChanged

MoistureMeasured

QualityVerified

BatchCompleted

BatchReleased

EnergyCalculated

CarbonCalculated

AIRecommendationGenerated

---

# 22. Mobile

Batch List

Batch Status

Recipe Viewer

QR Scan

Sensor Overview

Alarm Management

Photo Capture

Digital Signature

Offline Mode

---

# 23. Business Rules

Every Kiln Batch shall have one approved recipe.

Every batch shall contain compatible materials.

Sensor data shall be recorded continuously.

Drying curves shall be immutable after completion.

Quality approval is mandatory before release.

Every completed batch shall update Material Genealogy.

Every batch shall generate Digital Product Passport data.

---

# 24. Future Extensions

Autonomous Kiln

Adaptive Drying

Edge AI Controllers

RFID Batch Tracking

Digital Thread

Thermal Camera AI

Industry 5.0

MCP Kiln Agents

---

# 25. Architecture Review

## Database Changes

kiln_batches

kiln_batch_materials

kiln_batch_sensors

kiln_batch_phases

kiln_batch_events

kiln_batch_quality

kiln_batch_energy

kiln_batch_ai

kiln_batch_documents

kiln_batch_history

## Related Modules

Drying_Process

Production_Planning

Scheduling

Production_Orders

Transformations

Material_Genealogy

Recipes

Quality

Energy

Maintenance

Inventory

Warehouse

Analytics

AI

Digital_Twin

## Application Updates

API_Contracts.md

Dashboard_Definitions.md

Report_Catalog.md

Screen_Catalog.md

UI_Flows.md

Barcode_QR_Model.md

Events.md

## Naswood-Specific Enhancements

### Kiln Intelligence

- Automatic batch formation by species, thickness and moisture
- Multi-kiln load balancing
- Dynamic batch merging and splitting
- Batch readiness scoring
- Kiln occupancy optimization

### Thermowood Integration

- Automatic transfer to Thermowood after approval
- Pre-drying validation
- Recipe compatibility control
- Moisture verification before treatment

### Energy Intelligence

- Real-time energy dashboard
- Energy per species
- Energy per recipe
- Energy cost per batch
- Peak tariff optimization

### Production Intelligence

- Production order linkage
- Campaign production synchronization
- Automatic warehouse allocation
- Ready-for-production scoring

### Sustainability

- Carbon footprint per batch
- Biomass consumption
- Renewable energy ratio
- ESG reporting
- Drying efficiency index

### AI Optimization

- Batch composition recommendation
- Remaining drying time prediction
- Dynamic recipe tuning
- Predictive defect analysis
- Autonomous alarm prioritization

### Digital Twin

- Live kiln visualization
- 3D stack model
- Sensor overlay
- Historical replay
- What-if batch simulation
