# Drying Process Module

**Project:** Naswood OS

**Document:** Drying Process

**Module Code:** MOD-KILN-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Drying Process module manages the complete kiln drying lifecycle of lumber before production.

It controls drying schedules, kiln recipes, moisture targets, energy consumption, quality monitoring and material genealogy while providing real-time visibility through Digital Twin.

---

# 2. Objectives

- Standardize drying processes
- Reduce drying defects
- Optimize kiln utilization
- Minimize energy consumption
- Improve moisture consistency
- Enable AI-assisted drying
- Maintain complete traceability
- Synchronize Digital Twin

---

# 3. Drying Workflow

Material Allocation

↓

Kiln Loading

↓

Batch Creation

↓

Recipe Assignment

↓

Drying Start

↓

Heating Phase

↓

Main Drying

↓

Conditioning

↓

Equalization

↓

Cooling

↓

Moisture Verification

↓

Quality Approval

↓

Warehouse Transfer

↓

Production Ready

---

# 4. Drying Batch

Batch ID

Batch Number

Batch Type

Production Order

Material List

Species

Thickness

Dimensions

Volume

Weight

Target Moisture

Kiln

Recipe

Operator

Shift

Status

---

# 5. Drying Recipes

Standard Recipe

Species Recipe

Thickness Recipe

Customer Recipe

Export Recipe

Thermowood Pre-Drying

Fast Drying

Low Stress Drying

AI Optimized Recipe

---

# 6. Kiln Types

Conventional Kiln

High Temperature Kiln

Dehumidification Kiln

Vacuum Kiln

Steam Kiln

Solar Kiln

Future Hybrid Kiln

---

# 7. Process Parameters

Dry Bulb Temperature

Wet Bulb Temperature

Relative Humidity

Wood Moisture

Air Velocity

Air Direction

Fan Speed

Steam Valve Position

Heating Valve

Vent Position

Pressure

Kiln Load

---

# 8. Drying Phases

Heating

Equalization

Conditioning

Main Drying

Stress Relief

Cooling

Finished

---

# 9. Moisture Management

Initial Moisture

Target Moisture

Current Moisture

Average Moisture

Minimum Moisture

Maximum Moisture

Moisture Distribution

Moisture Trend

Moisture Sensors

Manual Verification

---

# 10. Energy Management

Electricity

Natural Gas

Biomass

Steam

Hot Water

Energy per m³

Energy per Batch

Peak Demand

Energy Cost

Carbon Emissions

---

# 11. Quality Control

Surface Checks

Crack Detection

Warp Detection

Twist Detection

Bow Detection

Cup Detection

Honeycomb Detection

Case Hardening

End Checks

Moisture Uniformity

Final Inspection

---

# 12. Material Genealogy

Material Allocation

Kiln Batch

Recipe

Drying Curve

Operator

Sensors

Quality Results

Energy Consumption

Carbon Data

Output Materials

---

# 13. Digital Twin Integration

Live Kiln Status

Batch Progress

Temperature Heat Map

Humidity Heat Map

Moisture Trend

Energy Flow

Kiln Occupancy

Alarm Layer

Simulation

---

# 14. AI Capabilities

Recipe Optimization

Drying Curve Prediction

Drying Time Prediction

Moisture Prediction

Energy Optimization

Quality Prediction

Defect Prediction

Stress Prediction

Kiln Load Optimization

Batch Sequencing

Carbon Optimization

Predictive Maintenance

Autonomous Drying

AI Kiln Copilot

---

# 15. Vision AI

Surface Crack Detection

Warp Detection

End Check Detection

Color Analysis

Stack Position Verification

Load Verification

Thermal Camera Analysis

Automatic Quality Inspection

---

# 16. Dashboard Widgets

Active Kilns

Kiln Occupancy

Drying Progress

Batch Timeline

Temperature Curves

Humidity Curves

Moisture Trends

Energy Consumption

Carbon Emissions

Quality Alerts

AI Recommendations

---

# 17. Reports

Drying Batch Report

Kiln Performance

Recipe Performance

Drying Curve

Moisture Analysis

Energy Report

Carbon Report

Quality Report

Defect Analysis

Kiln Utilization

AI Optimization Report

---

# 18. API Resources

GET /kilns

GET /kilns/{id}

GET /kilns/{id}/batches

GET /kilns/{id}/telemetry

GET /kilns/{id}/energy

GET /kilns/{id}/alarms

GET /drying-batches/{id}

POST /drying-batches

POST /drying-batches/{id}/start

POST /drying-batches/{id}/pause

POST /drying-batches/{id}/resume

POST /drying-batches/{id}/complete

POST /drying-batches/{id}/simulate

POST /drying-recipes/{id}/optimize

---

# 19. Events

KilnLoaded

BatchCreated

DryingStarted

RecipeAssigned

TemperatureChanged

HumidityChanged

MoistureMeasured

QualityInspectionCompleted

BatchCompleted

BatchTransferred

EnergyCalculated

CarbonCalculated

AIRecommendationGenerated

---

# 20. Mobile

Kiln Status

Batch Progress

Recipe Viewer

QR Scan

Moisture Entry

Photo Capture

Alarm Acknowledgement

Offline Mode

---

# 21. Business Rules

Every kiln batch shall reference an approved recipe.

Materials within a batch shall be compatible by species, thickness and target moisture.

Moisture shall be verified before release.

Drying curves shall be stored permanently.

Energy consumption shall be calculated automatically.

Quality approval is mandatory before production release.

Every drying batch shall update Material Genealogy.

---

# 22. Future Extensions

IoT Edge Controllers

Digital Psychrometric Model

Adaptive Drying Algorithms

Autonomous Kiln Control

Thermal Camera Analytics

RFID Kiln Tracking

Industry 5.0

Digital Thread

MCP AI Kiln Agents

---

# 23. Architecture Review

## Database Changes

drying_batches

drying_batch_materials

drying_recipes

drying_recipe_versions

drying_curves

drying_sensor_data

drying_moisture_readings

drying_quality

drying_energy

drying_ai

drying_alarms

## Related Modules

Log_Inventory

Log_Classification

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

Events.md

Barcode_QR_Model.md

## Naswood-Specific Enhancements

### Kiln Intelligence

- Multi-kiln scheduling
- Kiln occupancy optimization
- Species compatibility matrix
- Thickness compatibility matrix
- Automatic batch balancing

### Energy Intelligence

- Energy per m³
- Energy per species
- Energy per recipe
- Peak demand optimization
- Biomass boiler integration
- Waste heat recovery monitoring

### Drying Intelligence

- Moisture gradient analysis
- Internal stress prediction
- Case hardening detection
- Drying defect probability
- Drying recipe benchmarking

### Production Intelligence

- Automatic production release after approval
- Thermowood-ready material identification
- Priority allocation to production orders
- Integration with campaign planning

### Sustainability

- Carbon emissions per batch
- Biomass fuel consumption
- Renewable energy ratio
- Drying efficiency KPI
- ESG reporting integration

### AI Optimization

- Dynamic recipe adjustment
- Real-time drying optimization
- Predictive quality control
- Remaining drying time prediction
- Autonomous alarm analysis

### Digital Twin

- 2D/3D kiln visualization
- Live sensor overlays
- Batch replay
- Historical curve comparison
- What-if drying simulation
