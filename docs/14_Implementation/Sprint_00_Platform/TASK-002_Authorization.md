# TASK-002 — Authorization

**Module:** Platform

**Sprint:** Sprint 00 – Platform

**Category:** Security

**Priority:** Critical

**Estimated Effort:** 6 Days

**Status:** Planned

---

# Purpose

Develop the centralized Authorization service responsible for determining what authenticated users are allowed to access and perform within Naswood OS.

Authorization enforces Role-Based Access Control (RBAC), Company and Plant isolation, document-level security and field-level permissions across every module of the platform.

Authentication identifies **who the user is**.

Authorization determines **what the user can access**.

---

# Objectives

- Centralized Authorization
- Role-Based Access Control (RBAC)
- Company Isolation
- Plant Isolation
- Document-Level Security
- Field-Level Security
- API Authorization
- Complete Auditability

---

# Scope

Authorization includes

- Role Management
- Permission Evaluation
- Company Authorization
- Plant Authorization
- Module Authorization
- Screen Authorization
- API Authorization
- Action Authorization
- Field-Level Authorization
- Dynamic Permission Evaluation

Out of Scope

- User Authentication
- User Registration
- Password Policies
- Session Management

---

# Authorization Architecture

```
Authenticated User

↓

JWT Claims

↓

Authorization Middleware

↓

Permission Engine

↓

Role Evaluation

↓

Company Validation

↓

Plant Validation

↓

Action Permission

↓

Application
```

---

# Authorization Model

Naswood OS uses

```
Role

↓

Permission

↓

Resource

↓

Action
```

Example

```
Role

Warehouse Manager

↓

Inventory Module

↓

Goods Receipt

↓

Create
```

---

# Permission Hierarchy

```
Platform

↓

Module

↓

Feature

↓

Document

↓

Action

↓

Field
```

---

# Supported Actions

Standard permissions

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

Additional actions may be defined per module.

Reference

Permission_Model.md

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

Roles are configurable.

---

# Company Authorization

Users may access

```
Single Company

or

Multiple Companies
```

Every request validates

- Company Assignment
- Company Status
- Company Permission

---

# Plant Authorization

Users may access

- One Plant
- Multiple Plants

Plant validation occurs on every request.

---

# Module Authorization

Example

```
Inventory

View

Create

Edit

Delete
```

Another example

```
Purchasing

RFQ

Approve

Purchase Order

Release
```

---

# Document Authorization

Supports

- Owner Access
- Department Access
- Company Access
- Plant Access
- Approval Access

Example

```
Purchase Request

↓

Requester

↓

Department Manager

↓

Purchasing Manager
```

---

# Field-Level Security

Supports

Example

```
Purchase Order

Price

Visible

Editable

Hidden

Read Only
```

Sensitive fields

- Cost
- Margin
- Salary
- Financial Data

may be hidden.

---

# API Authorization

Every endpoint validates

- Authentication
- Permission
- Company
- Plant
- Resource Access

Example

```
POST

/api/v1/purchase-orders

↓

Permission

PurchaseOrder.Create
```

Reference

API_Standards.md

---

# UI Authorization

Navigation automatically adapts.

Unauthorized menus are hidden.

Example

```
Dashboard

Inventory

Purchasing

Finance

Administration
```

Only authorized modules appear.

---

# Dynamic Permissions

Supports

- Temporary Permission
- Project Permission
- Delegated Approval
- Time-Limited Access
- Emergency Access

---

# Approval Permissions

Approval permissions are evaluated separately.

Example

```
Purchase Order

↓

Buyer

↓

Purchasing Manager

↓

Finance

↓

CEO
```

Approval amount limits are configurable.

---

# Permission Evaluation Flow

```
User

↓

Authentication

↓

Role

↓

Permission

↓

Company

↓

Plant

↓

Document

↓

Action Allowed
```

---

# JWT Claims

Authorization reads

- User ID
- Roles
- Company IDs
- Plant IDs
- Session ID

---

# Caching

Permission cache

Supports

- User Cache
- Role Cache
- Permission Cache

Automatic invalidation after

- Role Change
- Permission Change
- User Deactivation

Reference

Caching.md

---

# API Endpoints

```
GET /api/v1/permissions

GET /api/v1/roles

GET /api/v1/me/permissions

POST /api/v1/authorization/check

POST /api/v1/roles

PUT /api/v1/roles/{id}

DELETE /api/v1/roles/{id}
```

---

# Authorization Response

```json
{
  "allowed": true,
  "permission": "PurchaseOrder.Create"
}
```

---

# Error Codes

```
ACCESS_DENIED

PERMISSION_REQUIRED

COMPANY_ACCESS_DENIED

PLANT_ACCESS_DENIED

ROLE_REQUIRED

SESSION_INVALID
```

Reference

Error_Handling.md

---

# Security

Supports

- RBAC
- Least Privilege Principle
- Company Isolation
- Plant Isolation
- API Security
- JWT Validation
- Audit Logging

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Permission Granted
- Permission Denied
- Role Assigned
- Role Removed
- Company Changed
- Plant Changed
- Delegated Access
- Temporary Permission

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- New Role Assigned
- Permission Changed
- Unauthorized Access Attempt
- Temporary Access Expired

Reference

Notification_System.md

---

# Events

Publishes

- RoleAssigned
- RoleRemoved
- PermissionChanged
- AuthorizationDenied
- CompanyAccessGranted
- CompanyAccessRevoked

Reference

Event_Model.md

Integration_Events.md

---

# Performance

Targets

- Permission Check < 20 ms
- Authorization Middleware < 10 ms
- Cached Permission Lookup < 5 ms
- Concurrent Authorization Requests > 5,000

Reference

Performance.md

Caching.md

Concurrency.md

---

# Mobile Authorization

Supports

- Role-Based Mobile Menus
- Offline Permission Cache
- Mobile API Authorization
- Device-Based Access Validation

Reference

Mobile_Architecture.md

---

# Acceptance Criteria

The Authorization module shall

- Enforce Role-Based Access Control (RBAC).
- Validate Company and Plant permissions.
- Support document and field-level security.
- Secure every API endpoint.
- Cache permissions for high performance.
- Record all authorization events.
- Integrate with Authentication and all business modules.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-000_Login.md
- TASK-001_Authentication.md
- Security.md
- Permission_Model.md
- API_Standards.md
- Validation_Rules.md
- Error_Handling.md

---

# Related Documents

TASK-000_Login.md

TASK-001_Authentication.md

Permission_Model.md

Security.md

API_Standards.md

Validation_Rules.md

Performance.md

Caching.md

Concurrency.md

Logging.md

Audit_Log.md

Notification_System.md

Event_Model.md

Integration_Events.md
