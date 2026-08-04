# User Management

**Module:** Platform

**Domain:** Identity & Access Management (IAM)

**Version:** 1.0

**Status:** Draft

---

# Purpose

The User Management module provides centralized administration of all users within Naswood OS.

It manages user identities, organizational information, authentication settings, role assignments and account lifecycle while integrating with Authentication, Authorization and Audit Log.

Every person accessing Naswood OS must have a User account.

---

# Business Goals

- Centralized User Administration
- Enterprise Identity Management
- Secure Account Lifecycle
- Organizational Visibility
- Multi-Plant Support
- Regulatory Compliance
- High Security
- Scalability

---

# Scope

Included

- User Registration
- User Profile
- User Search
- User Update
- User Activation
- User Deactivation
- User Lock
- User Unlock
- Password Reset
- Role Assignment
- Plant Assignment
- Department Assignment
- User Preferences

Excluded

- Authentication
- Authorization
- Permission Evaluation

Handled by dedicated modules.

---

# Actors

System Administrator

Application Administrator

Human Resources

Department Manager

Factory Manager

Auditor

User

---

# Business Rules

Every user has a unique username.

Email address must be unique.

A user may belong to multiple roles.

A user may belong to multiple plants.

A user belongs to one department.

Inactive users cannot login.

Locked users cannot login.

Deleted users are soft deleted.

Every change creates an Audit Log.

---

# User Lifecycle

Created

↓

Pending Activation

↓

Active

↓

Locked

↓

Inactive

↓

Archived

---

# Functional Requirements

The system shall:

Create User

Update User

Search User

Filter User

Activate User

Deactivate User

Lock User

Unlock User

Reset Password

Assign Roles

Assign Plants

Assign Department

Upload Avatar

Export Users

Import Users

---

# User Profile

Username

Employee Number

First Name

Last Name

Display Name

Email

Phone

Mobile

Avatar

Department

Position

Role

Plant

Status

Language

Theme

Timezone

Last Login

Created Date

Updated Date

---

# Organizational Information

Company

Plant

Department

Team

Position

Manager

Cost Center

Employee Type

Shift

---

# Account Information

Username

Email

Password

Status

Locked

Lock Reason

Failed Login Count

Password Expiration

Two Factor Enabled

Authentication Provider

---

# User Preferences

Theme

Language

Dashboard Layout

Sidebar State

Default Plant

Notification Settings

Date Format

Time Format

Number Format

Measurement System

---

# User Types

Administrator

Manager

Supervisor

Office User

Warehouse Operator

Production Operator

Quality Engineer

Maintenance Engineer

Purchasing User

Sales User

Finance User

AI Service Account

System Account

Guest

---

# Workflow

Create User

↓

Validate

↓

Assign Department

↓

Assign Plant

↓

Assign Role

↓

Create Authentication Account

↓

Create Preferences

↓

Audit Log

↓

Activate

---

# State Machine

Draft

↓

Pending Activation

↓

Active

↓

Locked

↓

Inactive

↓

Archived

---

# Validation

Unique Username

Unique Email

Valid Department

Valid Plant

At Least One Role

Valid Status

Password Policy

---

# Relationships

User

↓

Roles

↓

Permissions

↓

Authentication

↓

Authorization

↓

Audit Log

↓

Notifications

↓

Dashboard

---

# Permissions

User.View

User.Create

User.Update

User.Delete

User.Export

User.Import

User.AssignRole

User.ResetPassword

User.Lock

User.Unlock

---

# API

GET /api/users

GET /api/users/{id}

POST /api/users

PUT /api/users/{id}

DELETE /api/users/{id}

POST /api/users/{id}/activate

POST /api/users/{id}/deactivate

POST /api/users/{id}/lock

POST /api/users/{id}/unlock

POST /api/users/{id}/reset-password

POST /api/users/{id}/assign-role

POST /api/users/{id}/assign-plant

GET /api/users/search

---

# UI

User List

User Detail

User Editor

Role Assignment

Plant Assignment

Department Assignment

Preferences

Security Settings

Login History

---

# UI Components

User Grid

Search Box

Avatar Upload

Department Selector

Plant Selector

Role Selector

Status Badge

Reset Password Button

Lock Button

Unlock Button

Export Button

Import Button

---

# Database

Tables

Users

UserProfiles

UserRoles

UserPlants

Departments

Positions

UserPreferences

UserSessions

---

# Database Fields

Id

EmployeeNumber

Username

FirstName

LastName

DisplayName

Email

Phone

DepartmentId

PositionId

Status

Language

Theme

DefaultPlantId

Avatar

CreatedAt

UpdatedAt

CreatedBy

UpdatedBy

---

# Events

UserCreated

UserUpdated

UserActivated

UserDeactivated

UserLocked

UserUnlocked

PasswordReset

RoleAssigned

PlantAssigned

ProfileUpdated

---

# Audit

Every user action records:

User

Timestamp

Action

Previous Values

Current Values

IPAddress

Browser

SessionId

CorrelationId

---

# Reports

User List

Active Users

Inactive Users

Locked Users

Last Login Report

Department Users

Plant Users

Role Assignments

Security Report

---

# KPIs

Total Users

Active Users

Inactive Users

Locked Users

Average Login Frequency

Role Distribution

Department Distribution

Plant Distribution

---

# Security

Role-Based Access

Permission Validation

Password Policy

JWT Authentication

HTTPS Only

Audit Logging

Soft Delete

Account Lockout

Session Management

---

# Non Functional Requirements

Support 100,000+ users.

User lookup < 100 ms.

Distributed cache support.

Horizontal scalability.

Bulk import support.

Bulk export support.

Responsive UI.

---

# Acceptance Criteria

User creation works.

User update works.

Role assignment works.

Plant assignment works.

Department assignment works.

Password reset works.

Account lock works.

User search works.

Audit Log generated.

Performance requirements achieved.

---

# Dependencies

Login

Authentication

Authorization

Role Management

Permission Management

Settings

Audit Log

Notification Center

Dashboard Layout

---

# Integration Points

Authentication

- User credentials.

Authorization

- User permissions.

Role Management

- User roles.

Permission Management

- Permission inheritance.

Dashboard

- Personalized dashboard.

Navigation

- Dynamic menu generation.

Header

- User profile.

Notification Center

- User notifications.

Audit Log

- User activity tracking.

---

# Best Practices

Never assign permissions directly to users.

Always use Roles.

Use Soft Delete.

Separate identity from employee information.

Support multiple plants.

Keep usernames immutable.

Use globally unique identifiers (GUID).

Log every administrative action.

---

# Future Enhancements

Employee Directory

HR System Integration

Active Directory Synchronization

LDAP Integration

Azure Entra ID

Biometric Identity

Temporary Users

Delegated Administration

User Impersonation

AI User Recommendations
