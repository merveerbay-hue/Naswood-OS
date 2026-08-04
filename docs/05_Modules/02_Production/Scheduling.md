# Scheduling Module

**Project:** Naswood OS

**Document:** Scheduling

**Module Code:** MOD-SCH-001

**Version:** 2.0

**Status:** Enterprise

---

# 1. Purpose

The Scheduling module transforms Production Plans into executable manufacturing schedules.

It allocates machines, work centers, operators, tools, materials, kilns, Thermowood batches, packaging stations and logistics resources while respecting capacity, priorities and operational constraints.

Scheduling provides real-time optimization and continuous rescheduling based on shop-floor events.

---

# 2. Objectives

- Optimize production execution
- Balance workloads
- Reduce lead times
- Minimize idle resources
- Improve OEE
- Prevent bottlenecks
- Enable AI-assisted scheduling
- Synchronize Digital Twin

---

# 3. Scheduling Types

Master Schedule

Finite Capacity Scheduling

Infinite Capacity Scheduling

Forward Scheduling

Backward Scheduling

Dynamic Scheduling

Shift Scheduling

Machine Scheduling

Operator Scheduling

Kiln Scheduling

Thermowood Scheduling

Packaging Scheduling

Loading Scheduling

Container Scheduling

AI Optimized Scheduling

Emergency Scheduling

---

# 4. Scheduling Sources

Production Orders

Production Planning

Sales Orders

Forecasts

Maintenance Calendar

Machine Availability

Material Availability

Warehouse Capacity

Quality Holds

Customer Priorities

AI Recommendations

---

# 5. Scheduling Dimensions

Factory

Production Area

Production Line

Work Center

Machine

Operator

Shift

Tool Assembly

Warehouse

Package Type

Container

Carrier

---

# 6. Resource Scheduling

Machines

Operators

Tools

Work Centers

Warehouses

Forklifts

Kilns

Thermowood Furnaces

Packaging Stations

Loading Docks

Containers

Energy Capacity

---

# 7. Scheduling Constraints

Machine Capacity

Operator Skills

Operator Certification

Material Availability

Warehouse Capacity

Tool Availability

Maintenance Window

Energy Limits

Customer Due Date

Shift Availability

Packaging Capacity

Transport Availability

---

# 8. Priority Rules

Customer Priority

Production Order Priority

Export Orders

Rush Orders

Project Orders

FIFO

LIFO

EDD (Earliest Due Date)

SPT (Shortest Processing Time)

Critical Ratio

AI Dynamic Priority

---

# 9. Timber Yard Scheduling

Truck Arrival Schedule

Log Receiving Queue

Log Measurement Queue

Sorting Queue

Storage Assignment

Primary Saw Queue

Supplier Priority

Species Priority

AI Yard Optimization

---

# 10. Kiln Scheduling

Kiln Occupancy

Drying Batch Planning

Recipe Compatibility

Moisture Target

Loading Sequence

Cooling Sequence

Energy Optimization

Expected Finish Time

AI Kiln Scheduler

---

# 11. Thermowood Scheduling

Batch Planning

Recipe Planning

Kiln Assignment

Cooling Planning

Color Consistency

Energy Planning

Quality Constraints

AI Batch Optimization

---

# 12. Machine Scheduling

Preferred Machine

Alternative Machine

Backup Machine

Capacity Check

Maintenance Check

OEE Analysis

Tool Availability

Energy Consumption

AI Machine Allocation

---

# 13. Operator Scheduling

Skill Matrix

Certification Check

Shift Planning

Overtime

Leave Calendar

Training Schedule

Workload Balancing

AI Workforce Optimization

---

# 14. Packaging Scheduling

Packaging Queue

Bundle Capacity

Pallet Capacity

Container Capacity

Label Printing

Inspection Queue

Shipment Deadline

---

# 15. Logistics Scheduling

Truck Booking

Carrier Assignment

Loading Dock Planning

Container Allocation

Route Planning

ETA

Export Documentation

---

# 16. Rescheduling

Machine Failure

Material Delay

Operator Absence

Maintenance Event

Quality Hold

Urgent Customer Order

Energy Restriction

AI Continuous Rescheduling

---

# 17. AI Capabilities

AI Schedule Optimization

Autonomous Rescheduling

Material Shortage Prediction

Machine Load Balancing

Operator Recommendation

Shift Optimization

Tool Availability Prediction

Kiln Optimization

Thermowood Optimization

Packaging Optimization

Loading Optimization

Energy Optimization

Carbon Optimization

Delay Prediction

What-if Simulation

Scenario Comparison

Factory Bottleneck Detection

Root Cause Analysis

Production Copilot

---

# 18. Digital Twin Integration

Live Schedule

Machine Timeline

Operator Timeline

Factory Timeline

Material Flow

WIP Flow

Energy Flow

Production Simulation

What-if Analysis

---

# 19. Dashboard Widgets

Production Calendar

Machine Calendar

Operator Calendar

Kiln Calendar

Thermowood Calendar

Packaging Calendar

Loading Calendar

Capacity Heat Map

Critical Orders

Late Orders

Current WIP

Factory Bottlenecks

Energy Consumption

Carbon Emissions

AI Schedule Suggestions

---

# 20. Reports

Daily Schedule

Weekly Schedule

Monthly Capacity

Machine Utilization

Operator Utilization

Kiln Schedule

Thermowood Schedule

Packaging Schedule

Loading Schedule

Schedule Adherence

Planning Accuracy

Late Order Analysis

Energy Schedule

Carbon Report

AI Scheduling Report

---

# 21. API Resources

GET /schedules

GET /schedules/calendar

GET /schedules/resources

GET /schedules/conflicts

GET /schedules/machines

GET /schedules/operators

GET /schedules/kilns

GET /schedules/thermowood

GET /schedules/packages

POST /schedules/generate

POST /schedules/optimize

POST /schedules/reschedule

POST /schedules/simulate

---

# 22. Events

ScheduleCreated

ScheduleUpdated

ScheduleReleased

ScheduleOptimized

ScheduleRescheduled

MachineAllocated

OperatorAllocated

KilnAllocated

ThermowoodBatchScheduled

PackagingScheduled

ShipmentScheduled

ConflictDetected

DelayDetected

AIScheduleGenerated

---

# 23. Mobile

Today's Schedule

Operator Tasks

Machine Queue

QR Scan

Schedule Approval

Delay Reporting

Photo Capture

Offline Mode

---

# 24. Business Rules

Only Released Production Orders may be scheduled.

Finite Capacity Scheduling shall not overload machines.

Operator certifications shall be validated before assignment.

Kiln schedules shall respect recipe compatibility.

Thermowood batches shall not mix incompatible recipes.

Packaging capacity shall be reserved before shipment planning.

Every scheduling change shall generate Events and Audit Logs.

AI recommendations require planner approval unless autonomous scheduling is enabled.

---

# 25. Future Extensions

Advanced APS

Constraint Solver

Multi Factory Scheduling

Cross Factory Optimization

AGV Scheduling

AMR Scheduling

Robot Scheduling

Autonomous Factory

Industry 5.0

MCP Scheduling Agents

---

# 26. Architecture Review

## Database Changes

schedules

schedule_operations

schedule_resources

schedule_constraints

schedule_conflicts

schedule_versions

schedule_ai_recommendations

schedule_simulations

resource_calendars

resource_allocations

## Related Modules

Production_Planning

Production_Orders

Routing

Operations

Work_Centers

Machines

Tooling

Materials

Inventory

Warehouses

Packaging

Finished_Goods

Logistics

Maintenance

Quality

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

## Naswood-Specific Enhancements

### Timber Yard Scheduling

- Log receiving appointments
- Supplier unloading windows
- Yard occupancy optimization
- Log rotation planning
- Species segregation

### Kiln Scheduling

- Batch grouping by moisture target
- Recipe compatibility matrix
- Cooling buffer management
- Energy tariff-aware scheduling

### Thermowood Scheduling

- Recipe family sequencing
- Furnace occupancy optimization
- Color consistency grouping
- Batch traceability

### Production Campaign Scheduling

- Group similar profiles
- Group same species
- Group identical dimensions
- Reduce setup times
- Optimize knife changes

### Packaging & Export Scheduling

- Customer-specific packaging windows
- Container booking synchronization
- Export documentation deadlines
- Loading sequence optimization

### Sustainability Scheduling

- Peak energy avoidance
- Carbon-aware production sequencing
- Waste minimization scheduling
- Pellet production synchronization

### AI Scheduling

- Autonomous schedule generation
- Live disruption management
- Predictive bottleneck avoidance
- Scenario ranking
- AI planner assistant

### Digital Twin

- Live Gantt visualization
- Factory timeline
- Resource occupancy map
- Real-time WIP animation
- What-if schedule simulation
