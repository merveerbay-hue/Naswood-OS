# TASK-051 — Shift

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Production Resources

**Priority:** High

**Estimated Effort:** 7 Days

**Status:** Completed

---

# Purpose

Develop the Shift Management module for Naswood OS.

The Shift module manages production calendars, shift schedules, workforce assignments and operational working hours across all plants, production lines and work centers.

It provides the time foundation for Production Planning, Capacity Planning, OEE, Labor Tracking and Manufacturing Execution.

---

# Objectives

- Centralized Shift Management
- Production Calendar Management
- Workforce Planning
- Capacity Calculation
- Attendance Integration
- OEE Time Calculation
- Manufacturing Schedule Standardization

---

# Scope

The Shift module includes

- Shift Definition
- Shift Calendar
- Shift Assignment
- Operator Assignment
- Holiday Management
- Overtime Planning
- Shift Rotation
- Shift Capacity
- Attendance Integration
- Shift Performance

Out of Scope

- Payroll
- HR Leave Management
- Recruitment
- Performance Evaluation

---

# Shift Architecture

```
Company

↓

Plant

↓

Production Line

↓

Work Center

↓

Shift

↓

Operators

↓

Production
```

---

# Shift Lifecycle

```
Draft

↓

Planned

↓

Approved

↓

Scheduled

↓

Active

↓

Completed

↓

Closed

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Shift Types

Supports

- Single Shift
- Double Shift
- Triple Shift
- Weekend Shift
- Night Shift
- Overtime Shift
- Maintenance Shift

---

# Shift Header

Each Shift contains

## General Information

- Shift Code
- Shift Name
- Company
- Plant
- Production Line
- Work Center
- Shift Type
- Status

---

## Time Information

- Start Time
- End Time
- Break Duration
- Net Working Time
- Overtime
- Calendar

---

## Workforce Information

- Supervisor
- Team Leader
- Number of Operators
- Skill Group
- Target Capacity

---

# Shift Schedule

Supports

Example

```
Shift A

06:00 - 14:00

↓

Shift B

14:00 - 22:00

↓

Shift C

22:00 - 06:00
```

Supports configurable schedules per plant.

---

# Shift Calendar

Supports

- Working Days
- Holidays
- Company Shutdowns
- Planned Maintenance Days
- Special Production Days

Calendar affects capacity planning automatically.

---

# Workforce Assignment

Supports

- Operators
- Team Leaders
- Maintenance Staff
- Quality Inspectors
- Logistics Operators

Each employee may belong to one active shift.

---

# Shift Rotation

Supports

- Weekly Rotation
- Monthly Rotation
- Custom Rotation
- Automatic Rotation

Example

```
Week 1

Morning

↓

Week 2

Evening

↓

Week 3

Night
```

---

# Capacity Planning

Shift capacity is calculated using

```
Working Hours

×

Operator Count

×

Machine Efficiency

=

Available Capacity
```

Supports

- Hourly Capacity
- Daily Capacity
- Weekly Capacity

Reference

Capacity Planning Module

---

# Work Center Integration

Each Shift may operate

- One Work Center
- Multiple Work Centers
- Entire Production Line

Reference

TASK-049_Work_Center.md

---

# Production Line Integration

Each Production Line may contain

- Multiple Shifts
- Parallel Shifts
- Seasonal Shifts

Reference

TASK-050_Production_Line.md

---

# Production Planning Integration

Production Orders are scheduled according to

- Shift Calendar
- Available Capacity
- Operator Availability
- Machine Availability

Reference

Production Planning Module

---

# Attendance Integration

Supports

- Check In
- Check Out
- Late Arrival
- Early Leave
- Overtime
- Absence

Future integration

- HR System
- Biometric Devices

---

# OEE Integration

Shift OEE calculates

- Availability
- Performance
- Quality
- Shift Utilization

Reference

OEE Module

---

# Productivity Monitoring

Displays

- Planned Production
- Actual Production
- Productivity %
- Operator Efficiency
- Downtime
- Idle Time

---

# Quality Integration

Supports

- Shift Quality Rate
- Defect Count
- Scrap Quantity
- Rework Quantity

Reference

Quality Module

---

# Safety Management

Supports

- Safety Briefing
- Incident Recording
- PPE Verification
- Emergency Contacts

---

# Attachments

Supports

- Shift Instructions
- Daily Production Plan
- Safety Procedures
- Attendance Reports
- Work Instructions

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Shift Code
- Shift Name
- Production Line
- Work Center
- Supervisor
- Status
- Date

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Active Shifts
- Shift Capacity
- Shift OEE
- Attendance Rate
- Production Performance
- Shift Downtime
- Operator Utilization

Reference

Production Dashboard

---

# Reports

Supports

- Shift Register
- Attendance Report
- Overtime Report
- Shift Productivity
- Shift Capacity
- Shift OEE
- Operator Performance

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/shifts

GET /api/v1/shifts/{id}

POST /api/v1/shifts

PUT /api/v1/shifts/{id}

DELETE /api/v1/shifts/{id}

GET /api/v1/shifts/calendar

GET /api/v1/shifts/capacity

POST /api/v1/shifts/{id}/activate

POST /api/v1/shifts/{id}/complete

GET /api/v1/shifts/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Shift Code is unique.
- Company exists.
- Plant exists.
- Production Line exists.
- Work Center exists.
- Start Time < End Time.
- Shift overlaps are not allowed.
- Active Shift requires assigned operators.
- Closed Shifts are read-only.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Production Authorization
- HR Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Shift Created
- Shift Updated
- Shift Activated
- Shift Completed
- Operator Assigned
- Schedule Changed
- Calendar Updated

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Shift Started
- Shift Ending Soon
- Operator Missing
- Capacity Below Target
- Overtime Required
- Shift Completed

Reference

Notification_System.md

---

# Events

Publishes

- ShiftCreated
- ShiftActivated
- ShiftCompleted
- OperatorAssigned
- ShiftCapacityChanged
- ShiftCalendarUpdated

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Shift Schedule
- Check-In / Check-Out
- Attendance
- Production Progress
- Shift Notifications
- QR Code Login

Reference

Production_Mobile.md

---

# Performance

Targets

- Shift Creation < 1 second
- Calendar Load < 500 ms
- Capacity Calculation < 1 second
- Search < 300 ms
- Support 100,000+ Shifts
- Real-Time Attendance Synchronization

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1 — CLT Factory

```
Morning Shift

06:00–14:00

↓

CLT Production Line

↓

35 Operators

↓

Target

200 m³
```

---

### Example 2 — Thermowood

```
24-Hour Operation

↓

3 Shifts

↓

Continuous Kiln Process

↓

Automatic Rotation
```

---

### Example 3 — Pellet Line

```
Weekend Shift

↓

Reduced Workforce

↓

Maintenance Window

↓

Restart Monday
```

---

# Acceptance Criteria

The Shift module shall

- Manage production shifts and calendars.
- Assign operators to shifts.
- Calculate shift-based production capacity.
- Integrate with Production Lines and Work Centers.
- Support attendance and overtime tracking.
- Calculate shift productivity and OEE.
- Publish shift lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-049_Work_Center.md
- TASK-050_Production_Line.md
- TASK-012_File_Upload.md
- Production_API.md
- Validation_Rules.md

---

# Related Documents

Production_Architecture.md

Production_API.md

Production_Workflow.md

TASK-046_BOM.md

TASK-047_Routing.md

TASK-048_Machine.md

TASK-049_Work_Center.md

TASK-050_Production_Line.md

TASK-052_Production_Calendar.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Unit_Conversion.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md
