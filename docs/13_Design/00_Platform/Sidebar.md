# Sidebar

**Module:** Platform

**Domain:** User Interface

**Version:** 1.0

**Status:** Draft

---

# Purpose

The Sidebar module provides the primary navigation structure of Naswood OS.

It offers fast, role-based access to all application modules while maintaining a consistent user experience across the platform.

The Sidebar dynamically displays only the modules and functions the authenticated user is authorized to access.

It is the primary navigation component of the application and works together with Header, Navigation and Dashboard.

---

# Business Goals

- Fast Navigation
- Role-Based Menu
- Dynamic Module Loading
- Personalized Experience
- Multi-Plant Support
- Responsive Design
- AI Integration
- Enterprise Scalability

---

# Scope

Included

- Dynamic Menu
- Module Groups
- Expand / Collapse
- Favorites
- Recently Used
- Module Icons
- Permission Filtering
- Plant Context
- Sidebar Search
- Responsive Sidebar

Excluded

- Header Navigation
- Dashboard Widgets

Implemented by dedicated modules.

---

# Actors

Administrator

Factory Manager

Production Manager

Warehouse Manager

Quality Manager

Maintenance Manager

Purchasing Manager

Sales Manager

Finance Manager

Operator

Guest

---

# Business Rules

Sidebar is displayed only after successful authentication.

Menu items are generated dynamically.

Unauthorized modules are hidden.

Menu visibility depends on assigned permissions.

Collapsed state is remembered.

Expanded state is remembered.

Favorites are user-specific.

Current plant affects visible modules where applicable.

---

# Functional Requirements

The system shall:

Display Modules

Expand Module Groups

Collapse Module Groups

Display Favorites

Display Recent Modules

Filter Menu by Permission

Highlight Active Module

Support Keyboard Navigation

Support Search

Remember User Preferences

---

# Sidebar Layout

Company Logo

↓

Sidebar Search

↓

Favorites

↓

Main Modules

↓

Administration

↓

Settings

↓

Collapse Button

---

# Main Menu Structure

Dashboard

Master Data

Inventory

Purchasing

Sales

Production

Quality

Maintenance

Finance

Analytics

AI

Digital Twin

Administration

Settings

---

# Inventory Menu

Warehouse

Location

Inventory

Material

Batch

Goods Receipt

Goods Issue

Stock Transfer

Inventory Count

Inventory Adjustment

---

# Purchasing Menu

Suppliers

Purchase Requests

RFQ

Supplier Quotations

Purchase Orders

Purchase Returns

Supplier Invoices

Reports

Dashboard

---

# Sales Menu

Customers

Leads

Quotations

Sales Orders

Shipments

Deliveries

Invoices

Dashboard

Reports

---

# Production Menu

Production Orders

Work Orders

Material Consumption

Machine Monitoring

Production Confirmation

Packaging

Finished Goods

Scrap

Production Dashboard

---

# Quality Menu

Inspection Plans

Incoming Inspection

In Process Inspection

Final Inspection

CAPA

Non Conformance

Certificates

Reports

---

# Maintenance Menu

Assets

Machines

Preventive Maintenance

Corrective Maintenance

Work Orders

Downtime

Spare Parts

OEE

Reports

---

# Finance Menu

Accounts

Payments

Budget

Inventory Valuation

Costing

Reports

Dashboard

---

# AI Menu

Factory Copilot

Knowledge Base

Forecasting

Optimization

AI Reports

Prompt Library

---

# Digital Twin Menu

Factory Model

Warehouse Model

Machine Model

Simulation

Visualization

Live Status

Analytics

---

# Administration Menu

Users

Roles

Permissions

Settings

Audit Log

Health Check

System Logs

Integrations

---

# Navigation Behaviour

Single Click

Open Module

Double Click

Expand Group

Collapse Group

Keyboard Navigation Supported

---

# Favorites

Users may pin:

Modules

Pages

Reports

Records

Favorites appear at the top.

---

# Recently Used

Last 20 visited modules.

Configurable.

Automatically updated.

---

# Search

Sidebar Search supports:

Module Name

Page Name

Report Name

Screen Name

Recent Results

Favorite Results

---

# Multi Plant

Plant Selector updates:

Warehouse

Inventory

Production

Reports

Dashboard

Current plant context is shared across the application.

---

# Responsive Behaviour

Desktop

Expanded Sidebar

Tablet

Collapsible Sidebar

Mobile

Overlay Sidebar

---

# Workflow

Login

↓

Authentication

↓

Authorization

↓

Load Permissions

↓

Generate Sidebar

↓

Load Favorites

↓

Ready

---

# State Machine

Loading

↓

Building Menu

↓

Ready

↓

Collapsed

↓

Expanded

---

# Validation

Authenticated User

Permission Exists

Module Exists

Plant Selected

Navigation Available

---

# Permissions

Sidebar.View

Sidebar.Configure

Sidebar.Favorites

Sidebar.Search

---

# API

GET /api/sidebar

GET /api/sidebar/menu

GET /api/sidebar/favorites

GET /api/sidebar/recent

POST /api/sidebar/favorites

DELETE /api/sidebar/favorites/{id}

---

# UI

Sidebar

Menu Groups

Module Items

Favorites

Recent Modules

Search

Collapse Button

---

# UI Components

Navigation Tree

Search Box

Favorite Button

Expand Icon

Collapse Icon

Module Icon

Badge

Tooltip

Scrollbar

---

# Database

Tables

NavigationMenus

NavigationItems

UserFavorites

RecentModules

SidebarPreferences

---

# Database Fields

Id

UserId

Module

Page

Favorite

SortOrder

Expanded

Collapsed

CreatedAt

UpdatedAt

---

# Relationships

Authentication

↓

Authorization

↓

Navigation

↓

Sidebar

↓

Header

↓

Dashboard

↓

Modules

---

# Events

SidebarLoaded

ModuleOpened

ModuleCollapsed

ModuleExpanded

FavoriteAdded

FavoriteRemoved

SidebarPreferenceUpdated

---

# Audit

Every significant action records:

User

Timestamp

Module

Action

Plant

SessionId

IPAddress

---

# Reports

Most Used Modules

Sidebar Usage

Favorite Modules

Recent Modules

Navigation Frequency

---

# KPIs

Sidebar Load Time

Most Used Module

Favorite Count

Search Usage

Average Navigation Time

Collapsed Usage

---

# Security

Permission Validation

JWT Validation

Secure Navigation

Role-Based Menu

Audit Logging

Hidden Routes Protected

---

# Non Functional Requirements

Sidebar Load < 300 ms.

Lazy Loading.

Virtual Scrolling.

Responsive Design.

Accessibility (WCAG 2.1 AA).

Caching Enabled.

Horizontal Scalability.

---

# Acceptance Criteria

Sidebar loads after login.

Authorized modules displayed.

Unauthorized modules hidden.

Favorites work.

Recent modules work.

Search works.

Responsive layout works.

User preferences saved.

Audit Log created.

Performance requirements achieved.

---

# Dependencies

Authentication

Authorization

Navigation

Header

Dashboard Layout

Settings

Audit Log

Notification Center

---

# Integration Points

Authentication

- Sidebar loads after successful login.

Authorization

- Filters menu items.

Navigation

- Controls routing.

Header

- Displays current module and breadcrumb.

Dashboard

- Default landing page.

Settings

- Stores sidebar preferences.

Audit Log

- Records navigation actions.

AI Assistant

- Opens modules through AI commands.

---

# Best Practices

Keep menu depth maximum three levels.

Do not display unauthorized modules.

Use icons consistently.

Remember user preferences.

Load menu asynchronously.

Keep navigation responsive.

Use lazy loading for large menu structures.

Separate business modules from administration.

---

# Future Enhancements

AI Generated Navigation

Voice Navigation

Drag & Drop Favorites

Pinned Workspaces

Module Usage Analytics

Plugin-Based Menu

Context-Aware Navigation

Custom User Menus

Workflow Shortcuts

Multi-Window Navigation
