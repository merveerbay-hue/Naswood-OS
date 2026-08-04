
# Database Schema — Finance

**Project:** Naswood OS
**Document:** Finance Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Finance module manages operational costing, inventory valuation, budgeting and ERP financial integration.

Naswood OS is not an Accounting System.

Financial accounting remains the responsibility of the connected ERP.

Naswood OS provides manufacturing cost intelligence.

---

# Philosophy

Manufacturing generates costs.

Finance analyzes costs.

ERP records accounting transactions.

Operational costing and financial accounting remain separated.

---

# Entity List

CostCenter

CostElement

CostRate

CostTransaction

ProductCost

ProductionCost

InventoryValuation

Budget

BudgetLine

FinancialIntegration

---

# cost_center

Represents organizational cost centers.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(150) |
| department_id | UUID FK |
| active | BOOLEAN |

Examples

- Sawmill
- Kiln Drying
- Thermowood
- Profiling
- Panel Production
- Pellet Production
- Warehouse
- Maintenance
- Quality

---

# cost_element

Defines cost categories.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |
| category | VARCHAR(50) |

Categories

- Material
- Labor
- Energy
- Tooling
- Maintenance
- Packaging
- Overhead
- Logistics
- Depreciation

---

# cost_rate

Stores configurable cost rates.

| Field | Type |
|--------|------|
| id | UUID |
| cost_element_id | UUID FK |
| effective_from | DATE |
| effective_to | DATE |
| unit_cost | NUMERIC(18,4) |
| currency | VARCHAR(10) |
| unit | VARCHAR(20) |

Examples

- Electricity (₺/kWh)
- Labor (₺/hour)
- Machine Hour (₺/hour)
- Biomass (₺/kg)
- Glue (₺/kg)

---

# cost_transaction

Represents operational cost generation.

| Field | Type |
|--------|------|
| id | UUID |
| transformation_id | UUID FK |
| production_order_id | UUID FK |
| material_id | UUID FK |
| cost_center_id | UUID FK |
| cost_element_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_cost | NUMERIC(18,4) |
| total_cost | NUMERIC(18,2) |
| transaction_date | TIMESTAMP |

---

# production_cost

Aggregated production costs.

| Field | Type |
|--------|------|
| id | UUID |
| production_order_id | UUID FK |
| material_cost | NUMERIC(18,2) |
| labor_cost | NUMERIC(18,2) |
| energy_cost | NUMERIC(18,2) |
| tooling_cost | NUMERIC(18,2) |
| maintenance_cost | NUMERIC(18,2) |
| overhead_cost | NUMERIC(18,2) |
| total_cost | NUMERIC(18,2) |

---

# product_cost

Calculated finished product costs.

| Field | Type |
|--------|------|
| id | UUID |
| product_id | UUID FK |
| production_order_id | UUID FK |
| cost_version | INTEGER |
| unit_cost | NUMERIC(18,4) |
| currency | VARCHAR(10) |
| calculated_at | TIMESTAMP |

---

# inventory_valuation

Inventory value by Material.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| valuation_method | VARCHAR(30) |
| quantity | NUMERIC(18,3) |
| unit_cost | NUMERIC(18,4) |
| inventory_value | NUMERIC(18,2) |
| valuation_date | DATE |

Valuation Methods

- Moving Average
- FIFO
- Standard Cost

---

# budget

Annual or project budgets.

| Field | Type |
|--------|------|
| id | UUID |
| budget_name | VARCHAR(150) |
| fiscal_year | INTEGER |
| budget_type | VARCHAR(50) |
| status | VARCHAR(30) |

Budget Types

- Annual
- Department
- Project
- Investment

---

# budget_line

Budget allocations.

| Field | Type |
|--------|------|
| id | UUID |
| budget_id | UUID FK |
| cost_center_id | UUID FK |
| cost_element_id | UUID FK |
| planned_amount | NUMERIC(18,2) |
| actual_amount | NUMERIC(18,2) |

---

# financial_integration

Tracks ERP integration.

| Field | Type |
|--------|------|
| id | UUID |
| source_document | VARCHAR(50) |
| source_id | UUID |
| target_system | VARCHAR(100) |
| integration_status | VARCHAR(30) |
| exported_at | TIMESTAMP |
| external_reference | VARCHAR(100) |

Target Systems

- SAP
- Logo
- Mikro
- Netsis
- Microsoft Dynamics
- Oracle ERP
- Custom ERP

---

# Relationships

Cost Center

1 → N Cost Transactions

Cost Element

1 → N Cost Rates

Cost Element

1 → N Cost Transactions

Production Order

1 → N Cost Transactions

Production Order

1 → 1 Production Cost

Material

1 → N Inventory Valuations

Product

1 → N Product Costs

Budget

1 → N Budget Lines

---

# Business Rules

### BR-1201

Every Production Order shall generate operational cost records.

---

### BR-1202

Product Costs shall be calculated automatically from Production data.

---

### BR-1203

Inventory valuation shall be based on configurable valuation methods.

---

### BR-1204

Cost rates are version-controlled.

Historical calculations remain unchanged.

---

### BR-1205

Financial accounting transactions are generated only through ERP integration.

---

### BR-1206

Manual modification of calculated production costs requires authorization.

---

### BR-1207

Every financial integration shall be logged.

---

### BR-1208

Budget variances shall be measurable by Cost Center.

---

### BR-1209

Operational costs shall remain traceable to the originating Transformation.

---

### BR-1210

Historical cost records shall never be deleted.

---

# Integration

Finance integrates with:

- Materials
- Production
- Inventory
- Purchasing
- Sales
- Maintenance
- Tooling
- ERP
- Analytics
- AI Cost Optimization

---

# Future Extensions

The architecture supports:

- Multi-Currency Costing
- Standard Cost Simulation
- Activity-Based Costing (ABC)
- Carbon Cost Accounting
- AI Cost Prediction
- Investment Analysis
- Profitability Analysis
- Cost-to-Serve
- Financial Dashboards

---

# Finance Philosophy

Finance in Naswood OS measures the economic performance of manufacturing.

Accounting belongs to ERP.

Operational costing belongs to Naswood OS.

Every cost originates from a real manufacturing activity and remains traceable from Material to Finished Product.
