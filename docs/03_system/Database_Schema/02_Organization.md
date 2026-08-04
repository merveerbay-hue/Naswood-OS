# Database Schema — Organization

**Project:** Naswood OS
**Document:** Organization Schema
**Database:** PostgreSQL
**Version:** 1.0

---

# Purpose

This schema defines the organizational structure of Naswood OS.

The Organization module is responsible for:

- Employees
- Departments
- Positions
- Reporting Structure
- User Accounts
- Roles
- Permissions
- Approval Hierarchy
- Shift Assignments

The organization model is independent from production and supports multi-company and multi-factory operations.

---

# Entity List

Employee

Department

Position

User

Role

Permission

UserRole

RolePermission

EmployeeAssignment

ReportingLine

Shift

ShiftAssignment

Delegation

ApprovalLevel

ApprovalHistory

---

# employee

Represents every employee within the organization.

| Field | Type | Constraint |
|--------|------|------------|
| id | UUID | PK |
| employee_no | VARCHAR(30) | UNIQUE |
| first_name | VARCHAR(100) | NOT NULL |
| last_name | VARCHAR(100) | NOT NULL |
| national_id | VARCHAR(30) | UNIQUE |
| email | VARCHAR(150) | |
| phone | VARCHAR(50) | |
| hire_date | DATE | |
| termination_date | DATE | NULL |
| company_id | UUID | FK |
| factory_id | UUID | FK |
| department_id | UUID | FK |
| position_id | UUID | FK |
| manager_employee_id | UUID | FK (Self Reference) |
| status | VARCHAR(20) | Active / Passive |
| created_at | TIMESTAMP | |
| updated_at | TIMESTAMP | |

Indexes

employee.employee_no

employee.department_id

employee.factory_id

employee.manager_employee_id

---

# user

Application login account.

| Field | Type |
|--------|------|
| id | UUID |
| employee_id | UUID FK |
| username | VARCHAR(80) UNIQUE |
| password_hash | TEXT |
| email | VARCHAR(150) |
| language | VARCHAR(10) |
| timezone | VARCHAR(50) |
| last_login | TIMESTAMP |
| failed_login_count | INTEGER |
| is_locked | BOOLEAN |
| status | VARCHAR(20) |

---

# role

Examples

Administrator

Factory Manager

Production Manager

Planner

Operator

Warehouse Operator

Quality Inspector

Maintenance Technician

Sales Manager

Purchasing Manager

Executive

---

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) UNIQUE |
| name | VARCHAR(100) |
| description | TEXT |

---

# permission

Represents atomic system permissions.

Examples

material.read

material.create

material.update

material.delete

production.execute

quality.approve

inventory.adjust

recipe.change

shipment.close

| Field | Type |
|--------|------|
| id | UUID |
| module | VARCHAR(50) |
| action | VARCHAR(50) |
| description | TEXT |

---

# user_role

Many-to-Many

Users ↔ Roles

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| role_id | UUID FK |
| assigned_at | TIMESTAMP |
| assigned_by | UUID FK |

---

# role_permission

Many-to-Many

Roles ↔ Permissions

| Field | Type |
|--------|------|
| id | UUID |
| role_id | UUID FK |
| permission_id | UUID FK |

---

# reporting_line

Defines reporting hierarchy.

| Field | Type |
|--------|------|
| id | UUID |
| employee_id | UUID FK |
| manager_id | UUID FK |
| effective_from | DATE |
| effective_to | DATE |

Historical reporting relationships are preserved.

---

# shift

Represents production shifts.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(20) |
| name | VARCHAR(100) |
| start_time | TIME |
| end_time | TIME |
| factory_id | UUID FK |

Examples

Morning

Evening

Night

Weekend

---

# shift_assignment

Assigns employees to shifts.

| Field | Type |
|--------|------|
| id | UUID |
| employee_id | UUID FK |
| shift_id | UUID FK |
| start_date | DATE |
| end_date | DATE |

---

# employee_assignment

Defines operational assignments.

Employee may be assigned to

Machine

Production Line

Warehouse

Department

Project

| Field | Type |
|--------|------|
| id | UUID |
| employee_id | UUID FK |
| assignment_type | VARCHAR(30) |
| assignment_id | UUID |
| start_date | DATE |
| end_date | DATE |

Examples

Machine

Warehouse

Project

Production Line

---

# delegation

Temporary authority transfer.

| Field | Type |
|--------|------|
| id | UUID |
| owner_employee_id | UUID FK |
| delegate_employee_id | UUID FK |
| start_date | DATE |
| end_date | DATE |
| reason | TEXT |
| approved_by | UUID FK |

---

# approval_level

Approval authority matrix.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(20) |
| level | INTEGER |
| description | TEXT |

Examples

Operator

Supervisor

Manager

Director

Executive

---

# approval_history

Stores every approval decision.

| Field | Type |
|--------|------|
| id | UUID |
| entity_type | VARCHAR(50) |
| entity_id | UUID |
| approval_level_id | UUID FK |
| approved_by | UUID FK |
| approval_date | TIMESTAMP |
| decision | VARCHAR(30) |
| notes | TEXT |

---

# Relationship Summary

Company

↓

Factory

↓

Department

↓

Position

↓

Employee

↓

User

↓

Roles

↓

Permissions

Employee additionally relates to

- Shift
- Manager
- Assignments
- Approvals
- Delegations

---

# General Rules

- Every employee belongs to one company.
- Every employee belongs to one primary factory.
- Every employee belongs to one primary department.
- Every employee has one primary position.
- A user account is optional for employees who do not access the system.
- Every user may have multiple roles.
- Roles may contain multiple permissions.
- Organizational history must never be deleted.
- Delegation records remain permanently traceable.
- Approval history is immutable.
- Soft Delete is preferred.
- UUID is used for all primary keys.
