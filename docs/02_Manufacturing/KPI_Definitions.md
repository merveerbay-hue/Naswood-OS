# KPI Definitions

**Project:** Naswood OS  
**Document:** KPI Definitions  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines all Key Performance Indicators (KPIs) used within Naswood OS.

KPIs are used by:

- Production
- Planning
- Warehouse
- Quality
- Maintenance
- Management
- Business Intelligence
- AI Analytics

Every KPI has a standardized calculation method to ensure consistency across reports and dashboards.

---

# 2. KPI Categories

Naswood OS groups KPIs into the following categories:

- Production
- Material
- Inventory
- Quality
- Waste & Recovery
- Machine
- Maintenance
- Planning
- Logistics
- Financial
- Sustainability

---

# 3. Production KPIs

## Daily Production

**Description**

Total finished production within a day.

**Unit**

m³

pieces

m²

kg

**Calculation**

Sum of all completed production quantities.

---

## Production Efficiency

**Description**

Actual production compared to planned production.

**Formula**

Actual Production / Planned Production × 100

---

## Capacity Utilization

**Formula**

Used Capacity / Available Capacity × 100

---

## Production Lead Time

Time between production order release and completion.

Unit:

Hours

---

## Order Completion Rate

Completed Orders / Total Orders × 100

---

# 4. Material KPIs

## Material Yield

Useful Output / Input Material × 100

---

## Material Recovery Rate

Recovered Material / Total Waste × 100

---

## Material Consumption

Input Material used for production.

Unit:

m³

kg

pieces

---

## Material Traceability

Tracked Materials / Total Materials × 100

Target:

100%

---

# 5. Inventory KPIs

## Inventory Accuracy

System Stock / Physical Stock × 100

---

## Stock Turnover

Material Consumption / Average Inventory

---

## Average Stock Days

Average number of storage days.

---

## Warehouse Occupancy

Occupied Locations / Total Locations × 100

---

# 6. Quality KPIs

## First Pass Yield

Products accepted without rework.

Formula

Accepted Products / Total Products × 100

---

## Reject Rate

Rejected Products / Total Products × 100

---

## Rework Rate

Reworked Products / Total Products × 100

---

## Customer Complaint Rate

Complaints / Total Shipments × 100

---

## Supplier Quality Score

Accepted Material / Received Material × 100

---

# 7. Waste & Recovery KPIs

## Waste Rate

Waste / Input Material × 100

---

## Recovery Rate

Recovered Material / Total Waste × 100

---

## Pellet Yield

Pellet Production / Recoverable Sawdust × 100

---

## Thermowood Fuel Recovery

Thermowood Sawdust Used as Fuel / Total Thermowood Sawdust × 100

Target:

100%

---

# 8. Machine KPIs

## Machine Availability

Operating Time / Planned Time × 100

---

## Machine Performance

Actual Speed / Design Speed × 100

---

## Machine Quality

Accepted Output / Total Output × 100

---

## OEE

Availability × Performance × Quality

---

## Breakdown Frequency

Number of machine failures.

---

## Mean Time Between Failures (MTBF)

Operating Time / Number of Failures

---

## Mean Time To Repair (MTTR)

Repair Time / Number of Repairs

---

# 9. Maintenance KPIs

## Preventive Maintenance Compliance

Completed PM / Planned PM × 100

---

## Maintenance Cost

Total maintenance cost.

---

## Spare Part Availability

Available Spare Parts / Required Spare Parts × 100

---

# 10. Planning KPIs

## Schedule Adherence

Completed According to Plan / Total Orders × 100

---

## Production Plan Accuracy

Planned Quantity vs Actual Quantity

---

## Routing Accuracy

Correct Routing / Total Routing Decisions × 100

---

# 11. Logistics KPIs

## On-Time Shipment

On-Time Deliveries / Total Deliveries × 100

---

## Packaging Accuracy

Correct Packages / Total Packages × 100

---

## Picking Accuracy

Correct Picks / Total Picks × 100

---

# 12. Financial KPIs

## Production Cost

Material

+

Labor

+

Energy

+

Maintenance

+

Overhead

---

## Cost per m³

Production Cost / Produced Volume

---

## Cost per Panel

Production Cost / Produced Panels

---

## Waste Cost

Total Waste Cost

---

## Recovery Savings

Recovered Material Value

---

# 13. Sustainability KPIs

## Energy Consumption

kWh

per

m³

---

## Water Consumption

Liters

per

m³

---

## Carbon Emission

CO₂

per

m³

---

## Waste Recycling Rate

Recycled Waste / Total Waste × 100

---

# 14. AI KPIs

AI continuously evaluates:

- Fire Prediction
- Quality Prediction
- Machine Failure Prediction
- Maintenance Prediction
- Production Forecast
- Delivery Forecast
- Inventory Forecast
- Operator Performance Trend
- Material Optimization

---

# 15. Dashboard Levels

## Executive Dashboard

- Production
- Profitability
- Waste
- OEE
- Orders
- Revenue

---

## Factory Dashboard

- Production
- Machines
- Routing
- Traceability
- Inventory

---

## Production Dashboard

- Orders
- Operations
- Delays
- Recovery

---

## Quality Dashboard

- Defects
- Rejects
- Recovery
- Complaints

---

## Maintenance Dashboard

- OEE
- Breakdowns
- PM Status
- MTBF
- MTTR

---

# 16. Business Rules

- Every KPI has a single calculation method.
- KPI formulas cannot be modified without revision approval.
- Dashboards always use standardized KPIs.
- Historical KPI values are never overwritten.
- AI models use the same KPI definitions as business reports.
