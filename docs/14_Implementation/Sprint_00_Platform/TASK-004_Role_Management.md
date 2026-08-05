# TASK-004 — Role Management

**Module:** Platform

**Sprint:** Sprint 00 – Platform

**Category:** Administration

**Priority:** Critical

**Estimated Effort:** 6 Days

**Status:** Planned

---

# Purpose

Develop the centralized Role Management module responsible for defining, organizing and maintaining user roles throughout Naswood OS.

Role Management provides Role-Based Access Control (RBAC) by grouping permissions into reusable roles that can be assigned to users. It simplifies permission administration while ensuring secure and consistent access across all modules.

---

# Objectives

- Centralized Role Administration
- Role-Based Access Control (RBAC)
- Permission Grouping
- Organizational Security
- Multi-Company Support
- Role Templates
- Complete Auditability

---

# Scope

The Role Management module includes

- Role Creation
- Role Editing
- Role Deactivation
- Permission Assignment
- Module Authorization
- Role Hierarchy
- Role Templates
- Company-Based Roles
- Plant-Based Roles
- User Role Assignment

Out of Scope

- Authentication
- User Registration
- Permission Engine Logic
- Login Sessions

---

# Role Architecture

```
Administrator

↓

Role Management API

↓

Role Service

↓

Permission Assignment

↓

Database

↓

Event Bus
```

---

# Role Lifecycle

```
Created

↓

Configured

↓

Active

↓

Modified

↓

Inactive

↓

Archived
```

Reference

Status_Lifecycle.md

---

# Role Structure

Each role contains

## General Information

- Role Code
- Role Name
- Description
- Status

---

## Organization

- Company
- Plant
- Department
- Business Unit

---

## Permissions

- Modules
- Screens
- Actions
- Field Permissions

---

## Audit Information

- Created By
- Created Date
- Last Modified
- Version

---

# Role Types

Supports

- Administrator
- Executive
- Purchasing Manager
- Buyer
- Warehouse Manager
- Warehouse Operator
- Production Planner
- Production Manager
- Quality Engineer
- Maintenance Manager
- Finance Manager
- Sales Manager
- HR Manager
- Read Only
- Custom Role

---

# Permission Assignment

Each role may contain

```
Module

↓

Feature

↓

Document

↓

Action
```

Example

```
Inventory

↓

Goods Receipt

↓

Create

Edit

Approve

Print
```

Reference

Permission_Model.md

---

# Supported Actions

Standard actions

- View
- Create
- Edit
- Delete
- Approve
- Reject
- Release
- Cancel
- Print
- Export
- Import

Module-specific actions may be added.

---

# Module Assignment

Roles may include permissions for

- Platform
- Inventory
- Purchasing
- Sales
- Production
- Quality
- Maintenance
- Finance
- Analytics
- AI

---

# Company Assignment

Supports

- Global Role
- Company Role
- Plant Role

Examples

```
Administrator

↓

All Companies
```

```
Warehouse Manager

↓

BUCAK Plant
```

---

# Role Templates

Supports predefined templates

- Administrator
- Purchasing
- Inventory
- Production
- Quality
- Finance
- Sales
- Executive

Templates can be copied and customized.

---

# Role Cloning

Supports

```
Existing Role

↓

Clone

↓

Modify

↓

Save New Role
```

Useful for rapid implementation.

---

# User Assignment

One user may have

- One Role
- Multiple Roles

Effective permissions are calculated automatically.

Reference

TASK-003_User_Management.md

---

# Role Hierarchy

Supports

```
Administrator

↓

Manager

↓

Supervisor

↓

Operator

↓

Viewer
```

Hierarchy is informational and does not override explicit permissions.

---

# API Endpoints

```
GET /api/v1/roles

GET /api/v1/roles/{id}

POST /api/v1/roles

PUT /api/v1/roles/{id}

DELETE /api/v1/roles/{id}

POST /api/v1/roles/{id}/clone

POST /api/v1/roles/{id}/activate

POST /api/v1/roles/{id}/deactivate
```

Reference

API_Standards.md

---

# Example Request

```json
{
  "code":"PUR_MANAGER",
  "name":"Purchasing Manager",
  "company":"NASWOOD",
  "permissions":[
    "PurchaseOrder.Approve",
    "RFQ.Create",
    "Supplier.View"
  ]
}
```

---

# Validation Rules

The system validates

- Role Code is unique.
- Role Name is unique within Company.
- Company exists.
- Assigned permissions exist.
- Reserved system roles cannot be deleted.
- Active roles cannot contain invalid permissions.

Reference

Validation_Rules.md

---

# User Interface

Desktop

```
--------------------------------------

Roles

--------------------------------------

Search

+ New Role

--------------------------------------

Role Code

Role Name

Company

Status

Actions

--------------------------------------
```

---

# Role Detail Screen

Tabs

- General
- Permissions
- Companies
- Plants
- Assigned Users
- Audit Log

---

# Search

Supports

- Role Code
- Role Name
- Company
- Plant
- Status

Reference

Search_Filtering.md

---

# Security

Supports

- RBAC
- Company Isolation
- Plant Isolation
- Permission Validation
- Secure APIs

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Role Created
- Role Updated
- Permission Added
- Permission Removed
- Role Activated
- Role Deactivated
- Role Assigned
- Role Removed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- Role Created
- Role Modified
- Permission Changed
- Role Assigned
- Role Deactivated

Reference

Notification_System.md

---

# Events

Publishes

- RoleCreated
- RoleUpdated
- RoleActivated
- RoleDeactivated
- RoleAssigned
- PermissionChanged

Reference

Event_Model.md

Integration_Events.md

---

# Performance

Targets

- Role Search < 300 ms
- Permission Assignment < 500 ms
- Role Save < 1 second
- Support 10,000+ roles
- Permission Evaluation via Cache

Reference

Performance.md

Caching.md

Concurrency.md

---

# Acceptance Criteria

The Role Management module shall

- Support centralized RBAC.
- Allow creation and maintenance of custom roles.
- Support module, document and action-level permissions.
- Support multi-company and multi-plant assignments.
- Provide role templates and cloning.
- Integrate with User Management and Authorization.
- Publish role lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-001_Authentication.md
- TASK-002_Authorization.md
- TASK-003_User_Management.md
- Permission_Model.md
- Security.md
- Validation_Rules.md

---

# Related Documents

TASK-001_Authentication.md

TASK-002_Authorization.md

TASK-003_User_Management.md

Permission_Model.md

Security.md

Validation_Rules.md

Performance.md

Caching.md

Concurrency.md

Search_Filtering.md

Logging.md

Audit_Log.md

Notification_System.md

Event_Model.md

Integration_Events.md

API_Standards.md
