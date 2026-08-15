# ==============================================================================
# PRODUCTION WORKSPACES
# Naswood Operating System (NOS)
# Module: Production
# Version: 1.0
# ==============================================================================

# PURPOSE

This document defines the functional workspaces of the Production module.

A Workspace represents a business area that groups related screens,
workflows, permissions and components.

Workspaces are the primary navigation units of NOS.

Implementation must never create standalone CRUD screens outside a Workspace.

---

# WORKSPACE HIERARCHY

Production

├── Dashboard

├── Planning

├── Execution

├── Shop Floor

├── Monitoring

├── Master Data

├── Quality Integration

├── Maintenance Integration

├── Analytics

└── Reports

---

# 1. DASHBOARD

Purpose

Real-time production overview.

Primary Users

- Production Manager
- Plant Manager
- Operations Director

Contains

- Production KPIs
- OEE
- Capacity
- Active Orders
- Machine Status
- Alerts
- Bottlenecks
- Shift Summary

---

# 2. PLANNING

Purpose

Production planning and scheduling.

Contains

- Production Orders
- Work Orders
- Scheduling
- Capacity Planning
- Material Availability
- Dispatch Board

Primary Users

- Planner
- Production Manager

---

# 3. EXECUTION

Purpose

Production execution management.

Contains

- Material Consumption
- Production Confirmation
- WIP
- Packaging
- Finished Goods
- Scrap
- Rework

Primary Users

- Supervisor
- Production Engineer

---

# 4. SHOP FLOOR

Purpose

Operator interface.

Contains

- Operator Terminal
- Machine Terminal
- Barcode
- QR Scan
- Production Start
- Production Stop
- Downtime
- Quality Check

Primary Users

- Operator

---

# 5. MONITORING

Purpose

Live production monitoring.

Contains

- Machine Status
- Work Center Status
- Production Timeline
- Live Counters
- Active Alarms

Primary Users

- Supervisor
- Production Manager

---

# 6. MASTER DATA

Purpose

Production master data administration.

Contains

- BOM
- Routing
- Machines
- Work Centers
- Production Lines
- Operations
- Calendars
- Shifts
- Tooling

Primary Users

- Manufacturing Engineer

---

# 7. QUALITY INTEGRATION

Purpose

Production-quality interaction.

Contains

- In Process Inspection
- Final Inspection
- NCR
- CAPA
- Certificates

Primary Users

- Quality Engineer
- Supervisor

---

# 8. MAINTENANCE INTEGRATION

Purpose

Maintenance interaction.

Contains

- Downtime
- Work Request
- Maintenance Status
- Asset Health

Primary Users

- Maintenance
- Production

---

# 9. ANALYTICS

Purpose

Operational analysis.

Contains

- OEE
- Productivity
- Yield
- Efficiency
- Capacity Utilization
- Loss Analysis

Primary Users

- Plant Manager

---

# 10. REPORTS

Purpose

Reporting.

Contains

- Production Reports
- KPI Reports
- Shift Reports
- Machine Reports
- WIP Reports
- Cost Reports

---

# DESIGN RULES

Every Workspace

- has its own Dashboard
- has its own navigation
- has its own permissions
- has its own filters
- has its own actions
- may contain multiple screens
- may contain Wizards
- may contain Dialogs

Workspace is the first navigation level.

Screens are the second level.

Components are the third level.

CRUD pages must never exist outside a Workspace.

---

# IMPLEMENTATION RULE

Frontend generation must begin with Workspace creation.

Screens are generated only after the corresponding Workspace has been implemented.
