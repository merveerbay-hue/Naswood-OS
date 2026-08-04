# Costing Module

**Project:** Naswood OS

**Document:** Enterprise Costing

**Module Code:** MOD-FIN-CST-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Costing module manages the complete product costing lifecycle by collecting, allocating and analyzing all direct and indirect costs across the enterprise.

It supports standard costing, actual costing, project costing, machine costing, activity-based costing and AI-assisted cost optimization.

The module serves as the Enterprise Cost Intelligence System (ECIS) of Naswood OS.

---

# 2. Objectives

- Calculate accurate product costs
- Improve profitability
- Reduce waste
- Optimize production costs
- Support management decisions
- Enable AI-assisted cost analysis
- Synchronize Digital Twin

---

# 3. Cost Flow

Purchase

↓

Inventory

↓

Production

↓

Machine Processing

↓

Quality

↓

Packaging

↓

Shipment

↓

Financial Posting

↓

Profitability Analysis

---

# 4. Cost Types

Material Cost

Labor Cost

Machine Cost

Tool Cost

Energy Cost

Maintenance Cost

Packaging Cost

Transportation Cost

Warehouse Cost

Quality Cost

Rework Cost

Scrap Cost

Warranty Cost

Overhead Cost

Project Cost

---

# 5. Material Costing

Logs

Lumber

Glue

Chemicals

Packaging

Consumables

Imported Materials

Exchange Rate Effects

Material Yield

Material Loss

---

# 6. Production Costing

Production Order

Operation

Machine Time

Setup Time

Cycle Time

Downtime

Operator Time

Production Yield

Scrap

Rework

---

# 7. Machine Costing

Machine Hour Rate

Electricity

Compressed Air

Maintenance

Depreciation

Tool Wear

Operator Allocation

Idle Time

Utilization

---

# 8. Energy Costing

Electricity

Natural Gas

Pellet Consumption

Thermal Oil

Compressed Air

Water

Energy per m³

Energy per Batch

CO₂ Cost

---

# 9. Tool Costing

Knife Cost

Sharpening Cost

Tool Life

Tool Change Time

Tool Inventory

Tool Depreciation

Assembly Cost

---

# 10. Logistics Costing

Internal Transport

Forklift

Warehouse

Loading

Shipment

Container

Export

Freight

Insurance

---

# 11. Project Costing

Material

Production

Machine

Labor

Logistics

Installation

Warranty

Project Margin

---

# 12. Financial Costing

Standard Cost

Actual Cost

Variance

Overhead Allocation

Cost Centers

Profit Centers

Contribution Margin

EBITDA Allocation

---

# 13. AI Capabilities

Cost Prediction

Margin Optimization

Yield Optimization

Scrap Analysis

Energy Optimization

Profitability Forecast

Alternative Cost Scenarios

Cost Copilot

---

# 14. Digital Twin Integration

Cost Timeline

Production Cost Flow

Energy Flow

Machine Cost Map

Project Cost Visualization

Profitability Simulation

---

# 15. Dashboard Widgets

Cost per m³

Cost per Product

Energy Cost

Machine Cost

Labor Cost

Gross Margin

Scrap Cost

Rework Cost

AI Recommendations

---

# 16. Reports

Product Cost Report

Project Cost Report

Machine Cost Report

Energy Cost Report

Material Yield Report

Variance Report

Profitability Report

AI Cost Report

---

# 17. API Resources

GET /costing

GET /costing/products

GET /costing/projects

GET /costing/machines

GET /costing/energy

POST /costing/calculate

POST /costing/recalculate

POST /costing/simulate

---

# 18. Events

CostCalculated

VarianceDetected

MachineCostUpdated

EnergyCostUpdated

ProjectCostUpdated

ProfitabilityUpdated

AIRecommendationGenerated

---

# 19. Mobile

Cost Dashboard

Margin Analysis

Alerts

Approvals

Offline Mode

---

# 20. Business Rules

Every production order shall generate actual costs.

Every material movement shall affect inventory valuation.

Every machine shall have an hourly cost.

Every tool shall have lifecycle costing.

Every project shall have profitability analysis.

All costing revisions shall remain auditable.

---

# 21. Future Extensions

Carbon Cost Accounting

ESG Cost Allocation

Real-Time Costing

Digital Cost Twin

Autonomous Cost Optimization

Industry 5.0

Digital Thread

MCP Finance Agents

---

# 22. Architecture Review

## Database Changes

cost_models

cost_transactions

cost_centers

profit_centers

machine_costs

energy_costs

tool_costs

project_costs

material_costs

scrap_costs

variance_analysis

cost_ai

cost_history

## Related Modules

ERP

Inventory

Production

Production_Orders

Machines

Energy

Tooling

Maintenance

Quality

Shipment

Export

Projects

Finance

Analytics

AI

Digital_Twin

## Application Updates

API_Contracts.md

Dashboard_Definitions.md

Report_Catalog.md

Screen_Catalog.md

Events.md

Executive_Dashboard.md

Mobile_App.md

## Naswood-Specific Enhancements

### Timber Cost Intelligence

- Log yield costing
- Sawmill recovery analysis
- Species-based costing
- Moisture impact costing
- Drying cost allocation
- Thermowood process costing

### Manufacturing Intelligence

- Machine-hour costing
- Setup cost allocation
- Tool wear costing
- Production loss costing
- Capacity cost analysis

### Logistics Intelligence

- Internal logistics costing
- Container costing
- Export costing
- Packaging cost analysis
- Freight allocation

### AI Optimization

- Cost anomaly detection
- Margin optimization
- Yield optimization
- Alternative production scenarios
- Predictive profitability

### Digital Twin

- Real-time cost visualization
- Cost heat maps
- Production cost replay
- Scenario simulations
- Enterprise profitability analysis
