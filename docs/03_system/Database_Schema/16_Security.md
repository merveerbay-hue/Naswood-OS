# Database Schema — Security

**Project:** Naswood OS
**Document:** Security Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Security module manages authentication, authorization, user sessions, API security and system access policies.

Its objectives are:

- Protect business data
- Secure manufacturing operations
- Enforce access control
- Support regulatory compliance
- Maintain complete auditability

---

# Philosophy

Every action requires authentication.

Every resource requires authorization.

Every critical action is audited.

Security protects manufacturing.

---

# Entity List

User

Role

Permission

RolePermission

UserRole

UserSession

LoginHistory

ApiClient

ApiToken

RefreshToken

PasswordHistory

SecurityPolicy

TrustedDevice

MultiFactorAuthentication

---

# user

Represents a system user.

| Field | Type |
|--------|------|
| id | UUID |
| employee_id | UUID FK |
| username | VARCHAR(100) |
| email | VARCHAR(150) |
| password_hash | TEXT |
| status | VARCHAR(30) |
| active | BOOLEAN |
| last_login | TIMESTAMP |

Status

- Active
- Locked
- Disabled
- Pending

---

# role

Represents a security role.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(100) |
| description | TEXT |

Examples

- Administrator
- Factory Manager
- Production Planner
- Warehouse Operator
- Quality Engineer
- Maintenance Technician
- Sales Manager

---

# permission

Represents a system permission.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(100) |
| module | VARCHAR(50) |
| action | VARCHAR(30) |

Actions

- Create
- Read
- Update
- Delete
- Approve
- Export
- Execute

---

# role_permission

Role to Permission mapping.

| Field | Type |
|--------|------|
| id | UUID |
| role_id | UUID FK |
| permission_id | UUID FK |

---

# user_role

Assigns Roles to Users.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| role_id | UUID FK |
| assigned_at | TIMESTAMP |
| assigned_by | UUID FK |

---

# user_session

Active login sessions.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| login_time | TIMESTAMP |
| logout_time | TIMESTAMP |
| ip_address | VARCHAR(50) |
| device_name | VARCHAR(100) |
| browser | VARCHAR(100) |
| session_token | TEXT |

---

# login_history

Login attempts.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| login_time | TIMESTAMP |
| ip_address | VARCHAR(50) |
| result | VARCHAR(20) |

Result

- Success
- Failed
- Locked

---

# api_client

External systems.

| Field | Type |
|--------|------|
| id | UUID |
| client_name | VARCHAR(100) |
| client_type | VARCHAR(50) |
| active | BOOLEAN |

Client Types

- ERP
- CRM
- AI
- Mobile
- Dealer Portal
- Customer Portal
- PLC Gateway

---

# api_token

Authentication tokens.

| Field | Type |
|--------|------|
| id | UUID |
| api_client_id | UUID FK |
| token_hash | TEXT |
| expires_at | TIMESTAMP |
| active | BOOLEAN |

---

# refresh_token

JWT refresh tokens.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| token_hash | TEXT |
| expires_at | TIMESTAMP |
| revoked | BOOLEAN |

---

# password_history

Password reuse prevention.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| password_hash | TEXT |
| changed_at | TIMESTAMP |

---

# security_policy

System-wide security configuration.

| Field | Type |
|--------|------|
| id | UUID |
| policy_name | VARCHAR(100) |
| policy_value | TEXT |

Examples

- Password Length
- Password Expiry
- Login Attempts
- Session Timeout
- MFA Required

---

# trusted_device

Trusted devices.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| device_fingerprint | TEXT |
| trusted_since | TIMESTAMP |

---

# multi_factor_authentication

MFA configuration.

| Field | Type |
|--------|------|
| id | UUID |
| user_id | UUID FK |
| method | VARCHAR(30) |
| enabled | BOOLEAN |
| secret_reference | TEXT |

Methods

- TOTP
- SMS
- Email
- Authenticator App

---

# Relationships

User

1 → N User Sessions

User

1 → N Login History

User

1 → N User Roles

Role

1 → N Role Permissions

Permission

1 → N Role Permissions

User

1 → N Trusted Devices

User

1 → N Refresh Tokens

User

1 → N Password History

API Client

1 → N API Tokens

---

# Business Rules

### BR-1601

Every user shall authenticate before accessing the system.

---

### BR-1602

Authorization is Role-Based (RBAC).

---

### BR-1603

Critical actions require explicit permissions.

---

### BR-1604

Passwords shall be stored only as secure hashes.

---

### BR-1605

Password reuse is prohibited according to Security Policy.

---

### BR-1606

Sessions shall expire automatically after inactivity.

---

### BR-1607

Failed login attempts shall trigger account lockout according to Security Policy.

---

### BR-1608

API access requires authenticated API Clients.

---

### BR-1609

Every security-sensitive action shall generate an Audit Log.

---

### BR-1610

Multi-Factor Authentication shall be configurable by Role or User.

---

# Integration

Security integrates with:

- Organization
- Permissions
- Audit Log
- API
- Events
- AI Services
- ERP
- Mobile Applications
- Dealer Portal
- Customer Portal

---

# Future Extensions

The architecture supports:

- Single Sign-On (SSO)
- OAuth 2.0
- OpenID Connect
- LDAP / Active Directory
- Azure AD
- Google Workspace
- Passkeys (FIDO2)
- Biometric Authentication
- Hardware Security Keys
- Zero Trust Security

---

# Security Philosophy

Security is an integral part of the Manufacturing Operating System.

Every identity is authenticated.

Every action is authorized.

Every critical operation is auditable.

Manufacturing data is protected without compromising operational efficiency.
