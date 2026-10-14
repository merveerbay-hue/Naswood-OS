# TASK-009 — Header

**Module:** Platform

**Sprint:** Sprint 00 – Platform

**Category:** User Interface

**Priority:** High

**Estimated Effort:** 5 Days

**Status:** Completed

---

# Purpose

Develop the centralized Header component used throughout Naswood OS.

The Header provides access to global application features including company and plant selection, global search, notifications, AI assistant, user profile, language selection and session management.

The Header remains consistent across every module and dynamically adapts according to user permissions and preferences.

---

# Objectives

- Unified User Experience
- Fast Global Navigation
- Company & Plant Switching
- Global Search
- Real-Time Notifications
- AI Assistant Access
- Responsive Design

---

# Scope

The Header includes

- Company Selector
- Plant Selector
- Global Search
- Notifications
- AI Assistant
- User Profile
- Language Selector
- Theme Selector
- Quick Actions
- Logout

Out of Scope

- Business Dashboards
- Module Navigation
- Authentication Logic
- Business Transactions

---

# Header Architecture

```
Application

↓

Header Component

↓

Navigation Service

↓

Notification Service

↓

Search Service

↓

User Profile Service

↓

UI
```

---

# Header Layout

```
---------------------------------------------------------------------

Logo

Company

Plant

Global Search

Quick Actions

Notifications

AI Assistant

Language

Theme

User Profile

---------------------------------------------------------------------
```

---

# Company Selector

Supports

- Single Company
- Multiple Companies
- Favorite Company
- Recent Companies

Workflow

```
Company

↓

Select Company

↓

Reload Permissions

↓

Reload Dashboard
```

Reference

Permission_Model.md

---

# Plant Selector

Supports

- Single Plant
- Multiple Plants

Changing plant updates

- Dashboard
- KPIs
- Reports
- Menus
- Default Filters

---

# Global Search

Supports searching

- Materials
- Warehouses
- Suppliers
- Customers
- Employees
- Purchase Orders
- Sales Orders
- Production Orders
- Inventory
- Reports

Search behavior

- Instant Search
- Recent Searches
- Search Suggestions
- Keyboard Shortcut

Shortcut

```
Ctrl + K
```

Reference

Search_Filtering.md

---

# Quick Actions

Displays commonly used actions.

Examples

- New Purchase Request
- New Purchase Order
- Goods Receipt
- New Sales Order
- Inventory Count
- Production Order

Visible according to permissions.

---

# Notifications

Displays

- Pending Approvals
- System Alerts
- Stock Alerts
- Delivery Delays
- Maintenance Alerts
- Quality Alerts
- AI Recommendations

Supports

- Read / Unread
- Mark All Read
- Open Related Screen

Reference

Notification_System.md

---

# AI Assistant

Provides quick access to

- AI Copilot
- Business Insights
- Forecasts
- Recommendations
- Chat Interface

Example

```
Ask AI

↓

Show Supplier Risk

↓

Open Dashboard
```

Reference

AI_Copilot.md

---

# Language Selector

Supports

- English
- Turkish

Future languages

- German
- Arabic
- French

Changing language updates the interface immediately.

Reference

Localization.md

---

# Theme Selector

Supports

- Light
- Dark
- System Default

Future

- Corporate Themes

---

# User Profile

Displays

- Profile Photo
- Name
- Position
- Company
- Online Status

Menu

- My Profile
- Preferences
- Security
- Help
- Logout

Reference

TASK-003_User_Management.md

---

# User Preferences

Supports

- Default Company
- Default Plant
- Theme
- Language
- Time Zone
- Date Format

Reference

Localization.md

TimeZone.md

---

# Session Information

Displays

- Session Status
- Last Login
- Current Device

Supports

- Logout
- Lock Screen
- Switch Company

Reference

TASK-001_Authentication.md

---

# Responsive Design

Desktop

```
Full Header
```

Tablet

```
Compact Header
```

Mobile

```
Logo

Search

Notifications

Profile
```

Reference

Mobile_Architecture.md

---

# Accessibility

Supports

- Keyboard Navigation
- Screen Readers
- High Contrast Mode
- Focus Indicators
- WCAG 2.1 AA

---

# Personalization

Each user may customize

- Favorite Actions
- Notification Preferences
- Header Density
- Search History
- Default Language
- Default Theme

Settings are stored per user.

---

# API Endpoints

```
GET /api/v1/header

GET /api/v1/profile

GET /api/v1/notifications

GET /api/v1/search

GET /api/v1/preferences

PUT /api/v1/preferences

POST /api/v1/logout
```

Reference

API_Standards.md

---

# Events

Publishes

- CompanyChanged
- PlantChanged
- NotificationOpened
- SearchExecuted
- ThemeChanged
- LanguageChanged
- UserLoggedOut

Reference

Event_Model.md

Integration_Events.md

---

# Performance

Targets

- Header Load < 300 ms
- Search Suggestions < 150 ms
- Notifications < 300 ms
- Company Switch < 1 second
- Cached User Preferences

Reference

Performance.md

Caching.md

---

# Security

Supports

- Secure Session Validation
- Company Isolation
- Plant Isolation
- Permission Validation
- Secure Logout

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Company Changed
- Plant Changed
- Search Executed
- User Logged Out
- Preferences Updated
- Notification Opened

Reference

Audit_Log.md

Logging.md

---

# Naswood Default Header

```
---------------------------------------------------------------

NASWOOD

Company ▼

Plant ▼

🔍 Search...

➕ Quick Actions

🔔 Notifications

🤖 AI Assistant

🌐 EN / TR

🌙 Theme

👤 User

---------------------------------------------------------------
```

---

# Acceptance Criteria

The Header module shall

- Provide a consistent global application header.
- Support company and plant switching.
- Support global search with keyboard shortcuts.
- Display notifications in real time.
- Provide AI Assistant access.
- Support responsive layouts.
- Persist user preferences.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-001_Authentication.md
- TASK-003_User_Management.md
- TASK-006_Dashboard_Layout.md
- TASK-007_Navigation.md
- TASK-008_Sidebar.md
- Security.md
- API_Standards.md

---

# Related Documents

TASK-001_Authentication.md

TASK-003_User_Management.md

TASK-006_Dashboard_Layout.md

TASK-007_Navigation.md

TASK-008_Sidebar.md

Permission_Model.md

Security.md

Localization.md

TimeZone.md

Search_Filtering.md

Notification_System.md

Performance.md

Caching.md

Logging.md

Audit_Log.md

Mobile_Architecture.md

AI_Copilot.md

API_Standards.md

Event_Model.md

Integration_Events.md
