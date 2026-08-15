# ==============================================================================
# TASK-052 — CALENDAR
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Calendar module defines the operational availability of manufacturing
resources within the Naswood Operating System.

A Calendar determines **when** production resources are available for planning
and execution.

Calendars are shared across Production, Planning, Maintenance and Logistics.

Calendars define availability.

Shifts define working periods.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Calendars are owned exclusively by the Production Master module.

Planning consumes Calendars for finite scheduling.

Production uses Calendars to validate execution.

Maintenance uses Calendars for shutdown planning.

---

# 3. RESPONSIBILITIES

The Calendar module is responsible for:

- Working Days
- Holidays
- Planned Shutdowns
- Maintenance Windows
- Exceptional Working Days
- Calendar Versions
- Effective Periods

The Calendar module is NOT responsible for:

- Employee Attendance
- Shift Definitions
- Payroll
- Production Orders
- Machine Assignments

---

# 4. DEPENDENCIES

Depends on

- Plant
- Organization

Referenced by

- Shift
- Work Center
- Machine
- Production Line
- Planning
- Production
- Maintenance

---

# 5. AGGREGATE ROOT

```
Calendar
```

Children

- Calendar Day
- Holiday
- Shutdown
- Exception Day
- Calendar Revision

---

# 6. ENTITY MODEL

```
Calendar
│
├── Working Days
├── Holidays
├── Shutdowns
├── Exception Days
├── Revisions
└── Audit
```

---

# 7. CALENDAR MASTER

Every Calendar contains

- Calendar Code
- Calendar Name
- Description
- Plant
- Time Zone
- Status

Calendar Code is unique.

---

# 8. CALENDAR TYPES

Supported Calendar Types

- Factory Calendar
- Production Calendar
- Maintenance Calendar
- Warehouse Calendar
- Logistics Calendar

Current implementation

```
Factory Calendar
Production Calendar
```

---

# 9. WORKING DAYS

Working Days define

- Monday
- Tuesday
- Wednesday
- Thursday
- Friday
- Saturday
- Sunday

Each day may be

- Working
- Non-Working

---

# 10. HOLIDAYS

Holiday records include

- Date
- Name
- Holiday Type
- Description
- Recurring Flag

Examples

- National Holiday
- Religious Holiday
- Company Holiday

Planning excludes Holidays automatically.

---

# 11. SHUTDOWNS

Shutdown periods include

- Planned Maintenance
- Annual Maintenance
- Factory Shutdown
- Energy Shutdown
- Emergency Shutdown

Shutdowns block production planning.

---

# 12. EXCEPTION DAYS

Exception Days override the standard Calendar.

Examples

- Additional Working Day
- Overtime Production
- Weekend Production
- Emergency Production

Exceptions always have higher priority than normal Calendar rules.

---

# 13. CALENDAR REVISIONS

Calendars are versioned.

Example

```
Calendar

↓

Revision A

↓

Revision B

↓

Revision C
```

Historical Production Orders remain linked to the Calendar Revision used during planning.

---

# 14. EFFECTIVITY

Each Calendar Revision defines

- Effective From
- Effective To

Only one Calendar Revision may be Active.

Planning always uses the active revision.

---

# 15. RESOURCE ASSIGNMENT

Calendars may be assigned to

- Production Lines
- Work Centers
- Machines
- Shifts

Assignments are reusable.

Multiple resources may reference the same Calendar.

---

# 16. VALIDATION RULES

System validates

- Unique Calendar Code
- Valid Effective Dates
- Non-overlapping Shutdowns
- Valid Holiday Dates
- Valid Exception Dates
- One Active Revision

Invalid Calendars cannot become Active.

---

# 17. APPROVAL WORKFLOW

```
Draft

↓

Review

↓

Approved

↓

Released

↓

Active

↓

Superseded

↓

Archived
```

Only Released Calendars may be used for planning.

---

# 18. BUSINESS RULES

Mandatory rules

- Every Plant requires at least one Calendar.
- Every Work Center references one Calendar.
- Every Machine inherits Calendar availability.
- Exception Days override normal Calendar rules.
- Shutdown periods prohibit production scheduling.
- Calendar changes create new revisions.
- Historical schedules always reference their original Calendar Revision.

---

# 19. API ENDPOINTS

```
GET    /api/v1/calendars

GET    /api/v1/calendars/{id}

POST   /api/v1/calendars

PUT    /api/v1/calendars/{id}

POST   /api/v1/calendars/{id}/approve

POST   /api/v1/calendars/{id}/release

POST   /api/v1/calendars/{id}/activate

GET    /api/v1/calendars/{id}/revisions
```

---

# 20. EVENTS

Publishes

```
CalendarCreated

CalendarApproved

CalendarReleased

CalendarActivated

CalendarSuperseded

CalendarUpdated

ShutdownScheduled

HolidayAdded
```

---

# 21. PERMISSIONS

```
production.calendar.read

production.calendar.create

production.calendar.update

production.calendar.approve

production.calendar.release

production.calendar.activate
```

---

# 22. USER INTERFACE

The Calendar screen contains

Header

↓

Revision Selector

↓

Monthly Calendar View

↓

Working Day Configuration

↓

Holiday Management

↓

Shutdown Planning

↓

Exception Days

↓

Assignments

↓

Audit Timeline

Calendar supports both monthly and yearly views.

---

# 23. SEARCH & FILTERS

Support filtering by

- Calendar Code
- Calendar Name
- Plant
- Calendar Type
- Status
- Revision
- Effective Date

---

# 24. AUDIT

Every modification records

- User
- Timestamp
- Previous Value
- New Value
- Changed Fields
- Approval Action

Audit records are immutable.

---

# 25. CROSS MODULE INTEGRATION

Planning

Uses Calendar availability for finite scheduling.

Production

Validates execution dates against the assigned Calendar.

Work Center

Determines operational availability.

Machine

Inherits working availability from its assigned Calendar.

Maintenance

Schedules planned shutdowns and maintenance windows.

Analytics

Calculates:

- Available Production Days
- Utilization
- Planned Downtime
- Calendar Efficiency

---

# 26. REPORTING

Calendar reporting supports

- Working Day Summary
- Holiday Schedule
- Shutdown Analysis
- Available Capacity Calendar
- Production Availability Forecast

Reports support yearly comparison.

---

# 27. SUCCESS CRITERIA

The Calendar module is successful when

- Production resources have clearly defined availability.
- Planning schedules only during valid operational periods.
- Shutdowns automatically prevent production allocation.
- Holiday management is centralized.
- Calendar revisions preserve historical planning accuracy.
- All operational resources share a consistent availability model.

---

# 28. FINAL DESIGN STATEMENT

The Calendar module is the canonical definition of operational availability
within the Naswood Operating System.

It provides a centralized, versioned and auditable scheduling foundation for
Production, Planning, Maintenance and Manufacturing resources while remaining
independent from Shift definitions and employee attendance.

By separating Calendars from Shifts, NOS achieves flexible planning,
predictable scheduling and complete historical consistency.
