# ==============================================================================
# TASK-051 — SHIFT
# Naswood Operating System (NOS)
# Module: Production Master
# Document: Design Specification
# Version: 1.0
# Status: Approved
# ==============================================================================

# 1. PURPOSE

The Shift module defines the working periods during which manufacturing
operations are executed.

Shifts provide the operational time structure for Production, Planning,
Maintenance, Labor Tracking and Analytics.

A Shift defines **when** manufacturing resources are available.

It does not define calendars or employee attendance.

---

# 2. OWNERSHIP

Module Owner

```
Production Master
```

Shift definitions are owned by the Production Master module.

HR manages employee assignments.

Planning schedules production using Shifts.

Production records execution within Shifts.

---

# 3. RESPONSIBILITIES

The Shift module is responsible for:

- Shift Definitions
- Working Hours
- Break Schedules
- Shift Rotation
- Production Availability
- Shift Capacity
- Overtime Configuration
- Shift Status

The Shift module is NOT responsible for:

- Employee Attendance
- Payroll
- Leave Management
- Calendar Holidays
- Production Orders

---

# 4. DEPENDENCIES

Depends on

- Calendar
- Plant

Referenced by

- Planning
- Production
- Machine
- Work Center
- Labor Tracking
- Maintenance
- Analytics

---

# 5. AGGREGATE ROOT

```
Shift
```

Children

- Shift Break
- Shift Rotation
- Shift Capacity
- Attachments

---

# 6. ENTITY MODEL

```
Shift
│
├── Break Schedule
├── Rotation
├── Capacity
├── Attachments
└── Audit
```

---

# 7. SHIFT MASTER

Every Shift contains

- Shift Code
- Shift Name
- Description
- Plant
- Start Time
- End Time
- Status

Shift Code is unique.

---

# 8. SHIFT TYPES

Supported shift types

- Morning Shift
- Afternoon Shift
- Night Shift
- Weekend Shift
- Holiday Shift
- Overtime Shift

Organizations may define additional shift types.

---

# 9. WORKING HOURS

Every Shift defines

- Start Time
- End Time
- Total Hours
- Planned Working Time
- Planned Break Time

Shift duration is calculated automatically.

---

# 10. BREAK MANAGEMENT

Each Shift supports multiple breaks.

Example

```
Morning Shift

08:00 - 12:00

↓

Lunch

12:00 - 12:30

↓

12:30 - 17:00
```

Breaks are excluded from productive time calculations.

---

# 11. SHIFT ROTATION

Shift Rotations define recurring work patterns.

Examples

- Weekly Rotation
- Two-Shift Rotation
- Three-Shift Rotation
- Continuous Production

Rotation definitions are reusable.

Employee assignments belong to HR.

---

# 12. SHIFT CAPACITY

Shift Capacity includes

- Available Hours
- Planned Operators
- Maximum Operators
- Planned Production Time
- Overtime Allowance

Planning uses Shift Capacity during scheduling.

---

# 13. CALENDAR RELATIONSHIP

Each Shift references an operational Calendar.

The Calendar defines

- Working Days
- Holidays
- Plant Shutdowns

The Shift defines

- Daily working periods

Calendars and Shifts are separate concepts.

---

# 14. WORK CENTER RELATIONSHIP

One Shift may be assigned to multiple Work Centers.

One Work Center may support multiple Shifts.

Example

```
Work Center

↓

Morning Shift

↓

Night Shift
```

Assignments are configured independently.

---

# 15. MACHINE RELATIONSHIP

Machines inherit operational availability from:

- Calendar
- Assigned Shift
- Maintenance Status

A Machine cannot execute production outside its assigned Shift.

---

# 16. VALIDATION RULES

System validates

- Unique Shift Code
- Valid Time Range
- No overlapping breaks
- Positive working duration
- Valid Calendar
- Valid Plant

Invalid Shifts cannot become Active.

---

# 17. APPROVAL WORKFLOW

```
Draft

↓

Review

↓

Approved

↓

Active

↓

Inactive

↓

Archived
```

Only Active Shifts participate in production planning.

---

# 18. BUSINESS RULES

Mandatory rules

- A Shift belongs to one Plant.
- A Shift references one Calendar.
- Shift Capacity is derived from working hours.
- Breaks reduce productive time.
- Employee assignment belongs to HR.
- Attendance belongs to HR.
- Shift assignment belongs to Production Planning.

---

# 19. API ENDPOINTS

```
GET    /api/v1/shifts

GET    /api/v1/shifts/{id}

POST   /api/v1/shifts

PUT    /api/v1/shifts/{id}

POST   /api/v1/shifts/{id}/approve

POST   /api/v1/shifts/{id}/activate

POST   /api/v1/shifts/{id}/deactivate

GET    /api/v1/shifts/{id}/capacity
```

---

# 20. EVENTS

Publishes

```
ShiftCreated

ShiftApproved

ShiftActivated

ShiftDeactivated

ShiftUpdated

ShiftCapacityChanged
```

---

# 21. PERMISSIONS

```
production.shift.read

production.shift.create

production.shift.update

production.shift.approve

production.shift.activate

production.shift.deactivate
```

---

# 22. USER INTERFACE

The Shift screen contains

Header

↓

General Information

↓

Working Hours

↓

Break Schedule

↓

Rotation

↓

Capacity

↓

Assigned Work Centers

↓

Attachments

↓

Audit Timeline

Timeline visualization displays the full Shift schedule.

---

# 23. SEARCH & FILTERS

Support filtering by

- Shift Code
- Shift Name
- Plant
- Status
- Shift Type
- Calendar
- Work Center

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

Uses Shift Capacity for finite scheduling.

Production

Records execution by Shift.

HR

Assigns employees to active Shifts.

Maintenance

Schedules maintenance outside production Shifts whenever possible.

Analytics

Calculates

- Shift Utilization
- Production by Shift
- OEE by Shift
- Labor Efficiency
- Downtime by Shift

---

# 26. REPORTING

Shift reporting supports

- Production by Shift
- Capacity Utilization
- Overtime Analysis
- Labor Productivity
- Downtime Analysis
- Machine Utilization
- OEE Trends

Historical reports are based on transactional production data.

---

# 27. SUCCESS CRITERIA

The Shift module is successful when

- Working periods are consistently defined.
- Planning schedules production accurately.
- Production execution is recorded by Shift.
- Capacity calculations are reliable.
- Labor and Machine utilization are measurable.
- Historical reporting is fully traceable.

---

# 28. FINAL DESIGN STATEMENT

The Shift module is the canonical definition of operational working periods
within the Naswood Operating System.

It provides the temporal structure required for production planning,
manufacturing execution and performance analysis while remaining independent
from employee attendance and payroll.

By separating Shifts from Calendars, HR and Production Execution, NOS achieves
greater flexibility, scalability and accurate manufacturing scheduling.
