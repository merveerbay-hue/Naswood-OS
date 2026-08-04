# Permission Model

**Project:** Naswood OS  
**Document:** Permission Model  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines the authorization and access control model used throughout Naswood OS.

The permission model protects:

- Business Data
- Production Operations
- Financial Information
- Quality Decisions
- Administrative Functions

The system combines Role-Based Access Control (RBAC) and Attribute-Based Access Control (ABAC).

---

# 2. Authorization Principles

Permissions are granted based on:

- Role
- Department
- Factory
- Production Line
- Warehouse
- Business Function
- Approval Level

Every action is evaluated before execution.

---

# 3. Access Model

Naswood OS uses four authorization levels.

Authentication

↓

Role

↓

Permission

↓

Business Rules

Authentication verifies identity.

Role defines responsibilities.

Permissions define allowed actions.

Business Rules determine whether the action is currently valid.

---

# 4. Roles

System Administrator

Factory Manager

Production Manager

Planning Engineer

Production Engineer

Shift Supervisor

Machine Operator

Warehouse Manager

Warehouse Operator

Purchasing Manager

Purchasing Specialist

Sales Manager

Sales Specialist

Quality Manager

Quality Inspector

Maintenance Manager

Maintenance Technician

Finance Manager

Finance Specialist

HR Manager

Executive

Customer

Supplier

API Client

Each user may have multiple roles.

---

# 5. Permission Types

Read

Create

Update

Delete

Approve

Reject

Execute

Print

Export

Import

Manage

Admin

---

# 6. Business Modules

Permissions are assigned separately for each module.

Material

Production

Planning

Inventory

Warehouse

Routing

Quality

Maintenance

Tooling

Machine

Sales

Purchasing

Finance

Reports

Dashboard

AI

System Settings

---

# 7. Factory Scope

Permissions may be limited to specific factories.

Example

Factory A

↓

Production Manager

↓

Access only Factory A

---

# 8. Warehouse Scope

Warehouse Operators only access assigned warehouses.

Example

Warehouse

↓

RAW

↓

Read

Update

No Delete

---

# 9. Machine Scope

Machine Operators may only interact with assigned machines.

Example

Operator

↓

PRF-01

PRF-02

No access to

THM-01

---

# 10. Approval Levels

Level 1

Operator

↓

Creates data

---

Level 2

Supervisor

↓

Approves production

---

Level 3

Manager

↓

Approves quality

↓

Approves inventory corrections

↓

Approves maintenance

---

Level 4

Executive

↓

Approves strategic changes

---

# 11. Sensitive Operations

The following actions always require authorization.

Recipe Change

Quality Override

Inventory Adjustment

Material Deletion

User Management

Role Assignment

Machine Configuration

System Configuration

Cost Modification

Production Cancellation

Shipment Cancellation

---

# 12. Permission Matrix

Every permission consists of:

Role

↓

Module

↓

Action

↓

Scope

↓

Approval Level

↓

Conditions

---

# 13. Attribute-Based Rules

Additional attributes may limit permissions.

Examples

Factory

Production Line

Machine

Warehouse

Department

Shift

Business Unit

Supplier

Customer

Only matching attributes grant access.

---

# 14. Audit Requirements

Every permission-controlled action generates an Audit Log.

Audit records include:

User

Role

Action

Timestamp

IPAddress

Device

Result

Reason

Audit records are immutable.

---

# 15. Temporary Permissions

Temporary permissions are supported.

Examples

Vacation Replacement

Maintenance Support

External Consultant

Temporary permissions automatically expire.

---

# 16. Emergency Access

Emergency Access allows authorized users to bypass restrictions.

Requirements

Approval

Reason

Automatic Logging

Manager Notification

---

# 17. API Permissions

Every API Client receives its own permission profile.

API permissions follow the same authorization rules as users.

---

# 18. AI Permissions

Artificial Intelligence has read-only access by default.

AI may:

Read Production Data

Read Quality Data

Read Inventory

Generate Recommendations

AI may never:

Approve

Delete

Modify Business Data

Execute Production

Without explicit user approval.

---

# 19. Business Rules

- Every user must have at least one role.
- Every action requires permission validation.
- Permissions are evaluated before execution.
- Business Rules override role permissions.
- Sensitive operations require approval.
- Audit logging is mandatory.
- Deleted users retain historical ownership.
- Temporary permissions expire automatically.

---

# 20. Future Extensions

The permission model is designed to support:

- Multi-Company Authorization
- Multi-Factory Authorization
- External Auditors
- Customer Portal
- Supplier Portal
- SSO Integration
- Azure AD
- LDAP
- MFA
- Biometric Authentication
