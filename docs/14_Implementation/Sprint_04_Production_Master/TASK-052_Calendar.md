# TASK-052 — Production Calendar

**Module:** Production Master

**Sprint:** Sprint 04 – Production Master

**Category:** Production Planning

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Completed

---

# Purpose

Develop the Production Calendar module for Naswood OS.

The Production Calendar module manages all production working calendars, shifts, holidays, maintenance windows and factory availability across companies, plants, production lines and work centers.

The Production Calendar serves as the official scheduling calendar for Production Planning, MRP, Capacity Planning, MES and Maintenance.

---

# Objectives

- Centralized Production Calendar
- Factory Availability Planning
- Shift Calendar Management
- Capacity Calculation
- Holiday Management
- Maintenance Window Planning
- Scheduling Standardization

---

# Scope

The Production Calendar module includes

- Calendar Creation
- Working Day Management
- Shift Calendar
- Holiday Calendar
- Maintenance Calendar
- Production Shutdown Planning
- Capacity Calendar
- Calendar Exceptions
- Calendar Versioning
- Calendar Synchronization

Out of Scope

- HR Leave Management
- Payroll
- Attendance
- Machine Maintenance Execution

---

# Production Calendar Architecture

```
Company

↓

Plant

↓

Production Calendar

↓

Production Line

↓

Work Center

↓

Shift

↓

Production Planning
```

---

# Calendar Lifecycle

```
Draft

↓

Configured

↓

Approved

↓

Published

↓

Active

↓

Revised

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Calendar Types

Supports

- Factory Calendar
- Plant Calendar
- Production Line Calendar
- Work Center Calendar
- Shift Calendar
- Maintenance Calendar
- Holiday Calendar

---

# Calendar Header

Each calendar contains

## General Information

- Calendar Code
- Calendar Name
- Company
- Plant
- Calendar Type
- Version
- Status

---

## Validity

- Effective From
- Effective To
- Time Zone
- Calendar Owner
- Last Revision Date

---

# Working Days

Supports

- Monday
- Tuesday
- Wednesday
- Thursday
- Friday
- Saturday
- Sunday

Each day may be configured as

- Working
- Non-Working
- Half Day
- Overtime Day

---

# Shift Calendar

Supports

- Shift A
- Shift B
- Shift C
- Weekend Shift
- Overtime Shift

Each shift defines

- Start Time
- End Time
- Break Time
- Available Capacity

Reference

TASK-051_Shift.md

---

# Holiday Management

Supports

- National Holidays
- Religious Holidays
- Company Holidays
- Regional Holidays
- Emergency Shutdown Days

Holiday rules automatically affect scheduling.

---

# Maintenance Calendar

Supports

- Planned Maintenance
- Preventive Maintenance
- Annual Shutdown
- Machine Calibration
- Utility Maintenance

Maintenance periods reduce available capacity.

Reference

Maintenance Module

---

# Calendar Exceptions

Supports

- Overtime Production
- Weekend Production
- Emergency Production
- Special Customer Projects
- Extra Shift

Exception history is retained.

---

# Capacity Calculation

Calendar automatically calculates

- Working Hours
- Shift Hours
- Available Capacity
- Lost Capacity
- Planned Capacity

Formula

```
Working Hours

×

Available Resources

=

Available Capacity
```

Reference

Capacity Planning Module

---

# Production Planning Integration

Workflow

```
Production Calendar

↓

Capacity Planning

↓

MRP

↓

Production Schedule

↓

Production Order
```

Reference

Production Planning Module

---

# Work Center Integration

Each Work Center references one Production Calendar.

Supports

- Shared Calendar
- Dedicated Calendar
- Seasonal Calendar

Reference

TASK-049_Work_Center.md

---

# Production Line Integration

Each Production Line references one Production Calendar.

Supports

- Independent Calendars
- Shared Calendars
- Seasonal Production

Reference

TASK-050_Production_Line.md

---

# Shift Integration

The calendar automatically generates

- Shift Schedule
- Operator Availability
- Production Capacity

Reference

TASK-051_Shift.md

---

# MRP Integration

Production Calendar determines

- Available Production Dates
- Material Planning Dates
- Purchase Requirement Dates
- Delivery Planning

Reference

MRP Module

---

# MES Integration

Supports

- Live Shift Calendar
- Current Working Status
- Real-Time Availability
- Planned Downtime

Reference

MES Module

---

# Calendar Synchronization

Supports synchronization with

- Shift Module
- Maintenance Module
- HR Calendar
- Production Planning
- Capacity Planning

---

# Attachments

Supports

- Annual Production Calendar
- Shift Plans
- Holiday Lists
- Maintenance Plans
- Company Announcements

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Calendar Code
- Calendar Name
- Plant
- Production Line
- Calendar Type
- Status
- Date

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Today's Production Status
- Working Plants
- Active Shifts
- Upcoming Holidays
- Planned Maintenance
- Capacity Availability

Reference

Production Dashboard

---

# Reports

Supports

- Production Calendar
- Working Days Report
- Holiday Report
- Maintenance Calendar
- Capacity Calendar
- Shift Calendar
- Calendar Exceptions

Reference

Production Reports

---

# API Endpoints

```
GET /api/v1/calendars

GET /api/v1/calendars/{id}

POST /api/v1/calendars

PUT /api/v1/calendars/{id}

DELETE /api/v1/calendars/{id}

GET /api/v1/calendars/capacity

GET /api/v1/calendars/working-days

GET /api/v1/calendars/holidays

POST /api/v1/calendars/{id}/publish

POST /api/v1/calendars/{id}/revise

GET /api/v1/calendars/search
```

Reference

Production_API.md

---

# Validation Rules

The system validates

- Calendar Code is unique.
- Company exists.
- Plant exists.
- Effective Dates are valid.
- Shift assignments do not overlap.
- Holidays cannot duplicate.
- Published calendars are read-only.
- Archived calendars cannot be assigned.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Production Authorization
- Planning Authorization
- Company Isolation
- Plant Isolation

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Calendar Created
- Calendar Updated
- Calendar Published
- Calendar Revised
- Holiday Added
- Maintenance Added
- Calendar Archived

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Holiday Reminder
- Planned Shutdown
- Calendar Published
- Calendar Revision
- Capacity Reduction
- Extra Shift Scheduled

Reference

Notification_System.md

---

# Events

Publishes

- CalendarCreated
- CalendarPublished
- CalendarRevised
- HolidayAdded
- MaintenanceScheduled
- CapacityChanged

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Calendar View
- Shift Calendar
- Holiday Calendar
- Maintenance Schedule
- Capacity Overview

Calendar editing remains desktop-first.

Reference

Production_Mobile.md

---

# Performance

Targets

- Calendar Load < 500 ms
- Capacity Calculation < 1 second
- Calendar Search < 300 ms
- Calendar Publish < 2 seconds
- Support 10,000+ calendars
- Support 20-year calendar history

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1 — CLT Factory Calendar

```
Monday–Friday

↓

3 Shifts

↓

Saturday

2 Shifts

↓

Sunday

Maintenance
```

---

### Example 2 — Thermowood Line

```
24/7 Operation

↓

Annual Kiln Maintenance

↓

National Holidays

↓

Automatic Capacity Update
```

---

### Example 3 — Pellet Plant

```
Normal Production

↓

Weekend Overtime

↓

Annual Shutdown

↓

Restart Planning
```

---

# Acceptance Criteria

The Production Calendar module shall

- Manage production calendars across all plants.
- Support shifts, holidays and maintenance schedules.
- Calculate available production capacity.
- Integrate with Production Planning, MRP and MES.
- Synchronize with Shift and Work Center modules.
- Publish calendar lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-049_Work_Center.md
- TASK-050_Production_Line.md
- TASK-051_Shift.md
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

TASK-051_Shift.md

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
