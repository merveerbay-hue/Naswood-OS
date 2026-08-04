# Log Classification Module

**Project:** Naswood OS

**Document:** Log Classification

**Module Code:** MOD-TY-CLS-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Log Classification module evaluates every incoming log according to physical, commercial and production characteristics.

Classification results determine the optimal production route, expected yield, drying strategy, Thermowood suitability and customer allocation.

The module serves as the decision engine for all downstream manufacturing processes.

---

# 2. Objectives

- Standardize log grading
- Maximize material yield
- Improve production planning
- Support AI-assisted classification
- Optimize routing
- Enable full traceability
- Reduce waste
- Synchronize Digital Twin

---

# 3. Classification Workflow

Truck Arrival

↓

Log Registration

↓

QR / RFID Assignment

↓

Measurement

↓

Species Identification

↓

Visual Inspection

↓

AI Vision Inspection

↓

Defect Detection

↓

Quality Classification

↓

Production Recommendation

↓

Warehouse Assignment

↓

Production Planning

---

# 4. Classification Methods

Manual Inspection

Scanner-Based Classification

Laser Measurement

3D Scanning

Vision AI Classification

Moisture Measurement

Density Estimation

Acoustic Analysis (Future)

X-Ray Internal Defect Detection (Future)

Hybrid AI Classification

---

# 5. Species Classification

Scots Pine

Black Pine

Spruce

Fir

Cedar

Oak

Beech

Ash

Walnut

Chestnut

Poplar

Custom Species

---

# 6. Diameter Classification

100–150 mm

151–200 mm

201–250 mm

251–300 mm

301–350 mm

351–400 mm

400+ mm

Custom Classes

---

# 7. Length Classification

2.0 m

2.5 m

3.0 m

3.5 m

4.0 m

4.5 m

5.0 m

6.0 m

Custom Lengths

---

# 8. Quality Classification

Premium

A

B

C

Industrial

Reject

Customer Specific Grade

---

# 9. Defect Detection

Knots

Dead Knots

Loose Knots

Cracks

End Splits

Heart Cracks

Blue Stain

Rot

Insect Damage

Resin Pockets

Reaction Wood

Warp

Curvature

Ovality

Fork

Fire Damage

Metal Detection

Stone Inclusion

---

# 10. Moisture Classification

Fresh

Partially Dry

Dry

Kiln Ready

Thermowood Ready

Custom Moisture Classes

---

# 11. Production Recommendation

Structural Timber

Construction Timber

Thermowood

Decking

Cladding

Finger Joint

Massive Panel

Glulam Lamella

CLT Lamella

Pellet

Biomass

Reject

---

# 12. Yield Prediction

Expected Lumber Yield

Expected Recovery

Expected Waste

Expected Sawdust

Expected Chips

Expected Pellet Material

Expected Carbon Storage

Expected Energy Consumption

AI Yield Prediction

---

# 13. Warehouse Assignment

Receiving Area

Species Zone

Diameter Zone

Quality Zone

Kiln Queue

Thermowood Queue

Production Queue

Export Buffer

---

# 14. Material Genealogy

Original Supplier

Forest Region

Harvest Lot

Harvest Permit

Truck

Log Number

QR Code

RFID Tag

Future Parent Materials

Transformation Prediction

---

# 15. AI Capabilities

Species Recognition

Defect Detection

Automatic Grading

Yield Prediction

Production Recommendation

Kiln Recommendation

Thermowood Suitability

Routing Recommendation

Customer Allocation

Inventory Optimization

Carbon Estimation

Quality Prediction

Anomaly Detection

Continuous Learning

AI Timber Copilot

---

# 16. Vision AI

Log Detection

Diameter Measurement

Length Measurement

End Surface Analysis

Bark Damage Detection

Crack Detection

Knot Detection

Curvature Detection

Surface Quality

Automatic Classification

---

# 17. Digital Twin Integration

Live Timber Yard Map

Log Position

Storage Zone

Classification Heat Map

Supplier Distribution

Species Distribution

Quality Distribution

Future Production Allocation

---

# 18. Dashboard Widgets

Today's Arrivals

Species Distribution

Diameter Distribution

Length Distribution

Quality Distribution

Supplier Performance

Harvest Region Map

Yield Prediction

Thermowood Candidates

Kiln Candidates

AI Classification Accuracy

Warehouse Occupancy

---

# 19. Reports

Daily Receiving Report

Species Analysis

Diameter Distribution

Length Distribution

Quality Report

Supplier Quality

Harvest Region Report

Yield Forecast

Thermowood Candidate Report

Kiln Loading Report

Rejected Logs

Carbon Storage Report

AI Classification Report

---

# 20. API Resources

GET /logs

GET /logs/{id}

GET /logs/{id}/classification

GET /logs/{id}/genealogy

GET /logs/{id}/vision

GET /logs/{id}/yield

POST /logs/classify

POST /logs/reclassify

POST /logs/simulate

POST /logs/approve

---

# 21. Events

LogRegistered

LogMeasured

SpeciesDetected

DiameterMeasured

LengthMeasured

QualityAssigned

DefectDetected

YieldCalculated

RoutingRecommended

WarehouseAssigned

GenealogyCreated

AIClassificationCompleted

---

# 22. Mobile

QR Scan

RFID Scan

Photo Capture

Voice Notes

Manual Classification

Approval Workflow

Offline Mode

---

# 23. Business Rules

Every log shall receive a unique identity.

Every log shall be measured before classification.

Classification requires species determination.

Rejected logs shall remain traceable.

Classification changes shall create a new revision.

Every accepted log shall receive genealogy records.

Thermowood recommendations require species compatibility.

All export logs shall preserve origin information.

---

# 24. Future Extensions

CT Log Scanner

Industrial X-Ray

LiDAR Measurement

Drone Yard Inventory

RFID Yard Tracking

Autonomous Yard Vehicles

Blockchain Timber Origin

EU Deforestation Regulation (EUDR) Compliance

Digital Forest Passport

MCP AI Timber Agents

---

# 25. Architecture Review

## Database Changes

log_classifications

log_measurements

log_defects

log_quality_grades

log_ai_predictions

log_species

log_yield_predictions

log_revision_history

log_origin

log_scanner_data

## Related Modules

Timber_Yard

Log_Receiving

Log_Measurement

Materials

Material_Genealogy

Transformations

Production_Planning

Production_Orders

Routing

Inventory

Warehouses

Kiln

Thermowood

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

### Timber Intelligence

- Species-specific production optimization
- Diameter-based routing
- Length optimization
- Grade-to-product matrix
- Supplier quality scoring
- Forest region performance analysis

### Thermowood Intelligence

- Thermowood suitability score
- Recommended drying recipe
- Expected color class
- Estimated treatment duration
- Energy consumption forecast

### Production Intelligence

- Automatic product family recommendation
- AI routing suggestion
- Expected machine utilization
- Predicted setup requirements
- Campaign production grouping

### Sustainability

- Carbon storage estimation
- FSC / PEFC verification
- EUDR origin validation
- Waste minimization analysis
- Pellet recovery estimation

### AI Optimization

- Best product recommendation
- Highest profit recommendation
- Highest yield recommendation
- Alternative classification simulation
- Continuous AI model training

### Digital Twin

- Live log yard visualization
- Interactive timber map
- Classification heat maps
- Material flow simulation
- Yard occupancy forecasting
