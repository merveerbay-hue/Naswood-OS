# Screen Catalog

**Project:** Naswood OS

**Document:** Screen Catalog

**Version:** 2.0

**Status:** Approved

---

# Purpose

This document defines every application screen available in Naswood OS.

It serves as the central registry for all user interfaces across Web, Mobile and future client applications.

Each screen belongs to a business module and follows common navigation, security and workflow principles.

---

# Screen Philosophy

Every screen has a single responsibility.

Screens display and collect business information.

Business logic is executed through Application Services.

Every screen follows Role-Based Access Control (RBAC).

Every business action generates Events and Audit Logs when applicable.

---

# Application Structure

The application is divided into the following areas.

• Authentication

• Home

• Dashboards

• Master Data

• Operations

• Manufacturing

• Warehouse & Inventory

• Quality

• Maintenance

• Commercial

• Finance

• Analytics

• Workflow

• Administration

• AI Copilot

---

# Authentication

Login

Forgot Password

Reset Password

Multi-Factor Authentication

Session Timeout

Change Password

---

# Home

Home

My Tasks

Notifications

Recent Activities

Favorites

Global Search

Help Center

---

# Dashboards

Executive Dashboard

Production Dashboard

Warehouse Dashboard

Quality Dashboard

Maintenance Dashboard

Sales Dashboard

Purchasing Dashboard

Finance Dashboard

Logistics Dashboard

Engineering Dashboard

AI Dashboard

Administration Dashboard

---

# Master Data

Organizations

Users

Roles

Permissions

Employees

Customers

Suppliers

Material Types

Materials

Product Families

Products

Species

Grades

Dimensions

Units

Warehouses

Warehouse Locations

Machine Groups

Machines

Tool Categories

Tools

Recipes

Routing Library

---

# Timber Yard

Truck Reception

Receiving Lots

Log Yard Map

Log Inventory

Log Measurement

Log Classification

Receiving Inspection

Log Stock Movements

---

# Manufacturing

Production Planning

Production Calendar

Capacity Planning

Production Orders

Operation Execution

Transformation History

Material Consumption

Packaging

Finished Goods

Kiln Dashboard

Kiln Schedule

Kiln Batch Management

Kiln Recipes

Kiln Monitoring

Thermowood Dashboard

Thermowood Batch Management

Thermowood Recipes

Temperature Curves

Humidity Curves

Energy Monitoring

---

# Warehouse & Inventory

Warehouse Overview

Warehouse Map

Inventory

Material Lookup

Warehouse Transfers

Location Transfers

Cycle Counting

Inventory Adjustments

Package Management

Shipment Preparation

Loading Operations

---

# Quality

Inspection Queue

Inspection Details

Incoming Inspection

In-Process Inspection

Final Inspection

Moisture Measurement

Dimensional Inspection

Quality Approval

Non-Conformance

Corrective Actions

Quality History

Laboratory Results

---

# Maintenance

Maintenance Dashboard

Preventive Maintenance

Corrective Maintenance

Work Orders

Machine History

Failure Analysis

Spare Parts

Maintenance Calendar

Asset Registry

---

# Machines & Tooling

Machine Dashboard

Machine Parameters

Machine Runtime

Machine Alarms

Energy Monitoring

Tool Inventory

Tool Assemblies

Knife Library

Knife Profiles

Tool Configuration

Sharpening History

Tool Life

Tool Installation

---

# Commercial

CRM

Leads

Customers

Dealer Management

Quotations

Quotation Revisions

Sales Orders

Purchase Requests

Purchase Orders

Supplier Evaluation

Shipment Tracking

Customer History

---

# Finance

Cost Centers

Manufacturing Cost

Product Cost

Inventory Valuation

Budget

Cost Analysis

ERP Export Status

Financial Reports

---

# Logistics

Transfer Orders

Loading Plan

Vehicle Management

Shipment Management

Container Loading

Delivery Tracking

Export Documents

Carrier Management

---

# Analytics

Reports

KPI Explorer

Forecasts

Trend Analysis

Executive Reports

Material Genealogy

Factory Digital Twin

---

# Material Genealogy

Material Lifecycle

Material Genealogy

Parent–Child Tree

Transformation History

Batch Traceability

Package Traceability

Shipment Traceability

---

# Workflow

Workflow Monitor

Approvals

Workflow History

Task Queue

Escalations

Workflow Designer (Future)

---

# Administration

Users

Roles

Permissions

Security

Audit Logs

API Clients

Printer Management

Label Templates

Integration Monitor

System Settings

Master Data Import

---

# AI Copilot

Factory Copilot

Production Advisor

Quality Advisor

Maintenance Advisor

Sales Assistant

Purchasing Advisor

Knowledge Search (RAG)

AI Recommendations

Prompt History

AI Memory

AI Agent Monitor

Document Assistant

---

# Global Components

Global Search

Notification Center

Task Center

QR Scanner

Barcode Scanner

User Profile

Language Selector

Theme Selector

Favorites

Recent Items

Help Center

---

# Screen Types

Dashboard

List

Detail

Create

Edit

Wizard

Approval

Configuration

Monitoring

Analytics

Report

Map

Timeline

Tree View

---

# Navigation Principles

Maximum navigation depth: 3 levels.

Every screen shall include:

- Breadcrumb
- Page Title
- Search
- Filters
- Primary Actions
- Secondary Actions
- Context Menu
- Help

---

# Standard Screen Layout

Header

↓

Navigation Menu

↓

Toolbar

↓

Search & Filters

↓

Main Content

↓

Details Panel

↓

Status Bar

---

# Mobile Availability

The following modules support Mobile Application.

Receiving

Warehouse

Inventory

Production

Quality

Maintenance

Logistics

Notifications

My Tasks

QR Scanner

Barcode Scanner

AI Copilot

---

# Security

Every screen follows Role-Based Access Control.

Permissions include:

View

Create

Update

Approve

Delete

Export

Print

---

# Business Rules

### SCR-001

Every screen belongs to exactly one functional module.

---

### SCR-002

Every screen shall support localization.

---

### SCR-003

Every screen shall support responsive layouts where applicable.

---

### SCR-004

All business actions shall execute through Workflow and Application Services.

---

### SCR-005

Screens shall display Business Codes instead of internal database identifiers whenever possible.

---

### SCR-006

Critical operational screens shall support QR Code and Barcode scanning.

---

### SCR-007

Every business action shall generate Business Events and Audit Logs when applicable.

---

### SCR-008

Dashboard visibility shall follow Role-Based Access Control.

---

### SCR-009

Navigation shall remain consistent across all modules.

---

### SCR-010

AI-assisted screens shall clearly distinguish AI recommendations from user actions.

---

# Integration

This document integrates with:

- Module Specifications
- UI Flows
- Dashboard Definitions
- API Contracts
- Workflow
- Permission Model
- Barcode & QR Model
- Mobile Application
- Notifications
- Analytics
- AI

---

# Future Extensions

The architecture supports:

- Low-Code Screen Builder
- Drag-and-Drop Dashboards
- Digital Twin Visualization
- AI Generated Forms
- Voice Navigation
- Augmented Reality Interfaces
- Multi-Monitor Factory Displays
- Smart Glass Integration

---

# Screen Philosophy

The Screen Catalog defines every user interface within Naswood OS.

Each screen represents a single business capability while maintaining consistency in navigation, security, workflow and traceability.

A standardized screen architecture ensures usability, maintainability and long-term scalability across the entire Manufacturing Operating System.
