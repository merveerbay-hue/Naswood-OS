# Production Strategies

**Project:** Naswood OS
**Document:** Production Strategies
**Version:** 1.0
**Status:** Approved

---

# Purpose

This document defines the manufacturing planning strategies supported by Naswood OS.

Production Strategy determines how customer demand is converted into manufacturing activities.

Different products may use different production strategies.

The Planning Engine selects the appropriate strategy according to business rules.

---

# Strategy Overview

Naswood OS supports multiple manufacturing strategies.

A company may use one or several strategies simultaneously.

Supported Strategies

- Make to Stock (MTS)
- Make to Order (MTO)
- Assemble to Order (ATO)
- Engineer to Order (ETO)
- Batch Manufacturing
- Hybrid Manufacturing

---

# Make to Stock (MTS)

Products are manufactured before customer orders are received.

Production is based on demand forecasts and minimum stock levels.

Typical Products

- Standard Deck Profiles
- Standard Cladding
- Standard Battens
- Standard KD Lumber

Advantages

- Fast delivery
- Stable production
- High machine utilization

Considerations

- Higher inventory
- Forecast accuracy is important

---

# Make to Order (MTO)

Production begins only after a confirmed customer order.

Typical Products

- Customer dimensions
- Special species
- Custom packaging

Advantages

- Low inventory
- Customer-specific production

Considerations

- Longer delivery times
- More detailed planning

---

# Assemble to Order (ATO)

Semi-finished materials are kept in stock.

Final products are completed after receiving the order.

Typical Products

- Standard Thermowood Lumber
- Standard Lamellas
- Standard Panels requiring final sizing or machining

Advantages

- Faster delivery than MTO
- Lower inventory than MTS

---

# Engineer to Order (ETO)

Products require engineering before production.

Typical Products

- Architectural projects
- Custom timber systems
- Large construction projects
- Prototype products

ETO may require

- Engineering approval
- CAD drawings
- Special routing
- Special recipes

---

# Batch Manufacturing

Multiple materials are processed together.

Typical Processes

- Kiln Drying
- Thermowood
- Panel Press
- Surface Treatment

Batch composition is determined by:

- Species
- Thickness
- Moisture
- Recipe
- Machine Capacity
- Delivery Priority

A Production Batch may contain materials belonging to different Production Orders when business rules allow.

Complete traceability is mandatory.

---

# Hybrid Manufacturing

Different strategies may coexist.

Example

Standard Deck

↓

MTS

Customer Finish

↓

ATO

Special Packaging

↓

MTO

The Planning Engine manages all strategies simultaneously.

---

# Strategy Selection

The Planning Engine evaluates:

Customer Order

↓

Product

↓

Stock Availability

↓

Production Capacity

↓

Material Availability

↓

Delivery Date

↓

Business Rules

↓

Production Strategy

---

# Strategy Decision Factors

Planning considers:

- Product Family
- Product Variant
- Customer Priority
- Material Availability
- Machine Availability
- Tool Availability
- Batch Optimization
- Delivery Commitment
- Production Cost
- Inventory Levels
- Energy Efficiency

---

# Manufacturing Optimization

The Planning Engine aims to maximize:

- Material Yield
- Machine Utilization
- Batch Efficiency
- Energy Efficiency
- Delivery Performance
- Recovery Rate

while minimizing:

- Waste
- Setup Time
- Inventory
- Production Cost
- Lead Time

---

# Thermowood Strategy

Thermowood production follows batch optimization.

Planning groups materials according to:

- Species
- Thickness
- Moisture
- Recipe
- Target Color
- Customer Commitments

One Thermowood Batch may satisfy multiple customer orders.

Traceability remains complete.

---

# Panel Production Strategy

Solid Panel

Only defect-free lamellas are selected.

Finger Joint Panel

Recovered and defect-free segments are joined into full-length lamellas.

Planning prioritizes material recovery before waste.

---

# Inventory Strategy

Inventory targets differ by material type.

Examples

Raw Materials

Safety Stock

↓

KD Lumber

Minimum Stock

↓

Thermowood Lumber

Production Buffer

↓

Finished Products

Strategy dependent

---

# AI Planning Support

AI may recommend:

- Better production sequence
- Better batch composition
- Machine allocation
- Material substitution
- Capacity balancing
- Yield optimization

AI recommendations require user approval.

---

# Business Rules

- Every Production Order shall have one Production Strategy.
- Different Production Order Lines may use different strategies.
- Strategy selection shall be configurable.
- Production strategies may change before execution with appropriate approval.
- Batch optimization shall never compromise traceability.
- Material recovery has priority over waste generation.
- Planning decisions shall be recorded for audit purposes.
- AI recommendations shall be stored with their acceptance or rejection status.

---

# Strategy Comparison

| Strategy | Stock | Customer Specific | Lead Time | Inventory |
|----------|-------|-------------------|-----------|-----------|
| MTS | High | Low | Short | High |
| MTO | Low | High | Medium | Low |
| ATO | Medium | Medium | Short | Medium |
| ETO | Very Low | Very High | Long | Low |
| Batch | Variable | Variable | Optimized | Optimized |
| Hybrid | Configurable | Configurable | Optimized | Optimized |

---

# Future Extensions

The architecture supports additional planning strategies, including:

- Demand Driven MRP (DDMRP)
- Constraint-Based Planning
- Finite Capacity Scheduling
- AI Autonomous Scheduling
- Carbon-Aware Production Planning
- Multi-Factory Optimization

---

# Production Strategy Philosophy

Production strategy is not fixed.

It adapts to customer demand, material availability, machine capacity and business objectives.

Naswood OS enables flexible manufacturing while preserving complete traceability and operational control.
