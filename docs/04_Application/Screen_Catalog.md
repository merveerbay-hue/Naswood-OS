# Screen Catalog

**Project:** Naswood OS

**Document:** Screen Catalog

**Version:** 1.0

**Status:** Approved

---

# Purpose

This document defines all application screens available within Naswood OS.

The Screen Catalog serves as the central registry of user interfaces across Web, Mobile and future applications.

Each screen belongs to a business module and follows common UI and navigation principles.

---

# Philosophy

Every screen has one primary responsibility.

Screens display business information.

Business logic belongs to the Application Layer.

Every screen supports Role-Based Access Control.

---

# Screen Categories

Authentication

Home

Dashboard

Master Data

Production

Inventory

Warehouse

Quality

Maintenance

Machines

Tooling

Sales

Purchasing

Finance

Logistics

Analytics

Workflow

Administration

AI

---

# Authentication

Login

Logout

Forgot Password

Reset Password

Change Password

Multi-Factor Authentication

Session Expired

---

# Home

Home

Notifications

My Tasks

Recent Activities

Favorites

Search

Help

---

# Dashboard

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

Material Types

Materials

Products

Species

Grades

Dimensions

Units

Warehouses

Warehouse Locations

Customers

Suppliers

Employees

Organizations

Cost Centers

Machine Groups

Tool Categories

---

# Receiving

Truck Reception

Receiving Lots

Material Registration

Incoming Inspection

Receiving History

---

# Production Planning

Production Planning Board

Production Orders

Production Calendar

Production Queue

Capacity Planning

Scheduling

Production Simulation

---

# Production

Production Order Details

Operation Execution

Transformation History

Material Consumption

Production History

Production Timeline

Packaging

Finished Goods

---

# Machines

Machine List

Machine Details

Machine Dashboard

Machine Parameters

Machine Runtime

Machine Alarms

Machine Maintenance

Energy Monitoring

---

# Tooling

Tool Inventory

Tool Assemblies

Knife Library

Knife Profiles

Tool Configuration

Sharpening History

Tool Life

Tool Installation

---

# Warehouse

Warehouse Overview

Warehouse Map

Inventory

Material Lookup

Warehouse Transfers

Location Transfers

Cycle Counting

Inventory Adjustments

Package Management

---

# Quality

Inspection Queue

Inspection Details

Measurements

Moisture Measurement

Dimensional Inspection

Quality Approvals

Non-Conformance

Corrective Actions

Quality History

---

# Maintenance

Maintenance Dashboard

Work Orders

Maintenance Calendar

Preventive Maintenance

Corrective Maintenance

Asset Details

Spare Parts

Failure Analysis

---

# Sales

CRM

Leads

Customers

Opportunities

Quotations

Quotation Revisions

Sales Orders

Dealer Portal

Shipment Tracking

Customer History

---

# Purchasing

Purchase Requests

Purchase Orders

Suppliers

Supplier Evaluation

Receiving Status

Supplier Price Lists

Material Procurement

---

# Finance

Cost Centers

Manufacturing Cost

Product Cost

Inventory Valuation

Budget

Cost Analysis

ERP Export Status

---

# Logistics

Transfer Orders

Loading Plan

Vehicle Management

Shipment Management

Container Loading

Delivery Tracking

Export Documents

---

# Analytics

Reports

KPIs

Dashboards

Forecasts

Trend Analysis

Digital Twin

Material Genealogy

Executive Reports

---

# Workflow

Workflow Monitor

Approvals

Workflow History

Workflow Designer (Future)

Tasks

Escalations

---

# Administration

Users

Roles

Permissions

Security

Audit Logs

API Clients

Integration Monitor

Printer Management

Label Templates

System Settings

---

# AI

Factory Copilot

Production Advisor

Maintenance Advisor

Quality Advisor

Sales Assistant

Purchasing Assistant

AI Recommendations

AI History

Knowledge Base

Prompt History

---

# Global Components

Global Search

Notifications

Task Center

Help Center

User Profile

Language Selector

Theme Selector

Recent Items

Favorites

QR Scanner

Barcode Scanner

---

# Screen Types

List Screen

Detail Screen

Create Screen

Edit Screen

Dashboard

Wizard

Approval Screen

Report Screen

Configuration Screen

Monitoring Screen

Analytics Screen

---

# Navigation Principles

Maximum navigation depth: 3 levels.

Every screen shall include:

Breadcrumb

Page Title

Primary Actions

Secondary Actions

Context Menu

Search

Filters

Help

---

# Standard Screen Layout

Header

↓

Navigation Menu

↓

Toolbar

↓

Filters

↓

Content

↓

Details Panel

↓

Status Bar

---

# Search

Global Search supports:

Material Code

Product Code

Package Code

Production Order

Sales Order

Purchase Order

Shipment

Machine

Tool

QR Code

Barcode

Customer

Supplier

---

# Mobile Availability

The following screens shall support Mobile:

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

---

# Permissions

Every screen shall define:

View

Create

Update

Delete

Approve

Export

Print

---

# Business Rules

### SCR-001

Every screen belongs to exactly one functional module.

---

### SCR-002

Every screen follows Role-Based Access Control.

---

### SCR-003

Every screen shall support localization.

---

### SCR-004

Every screen shall support responsive layouts where applicable.

---

### SCR-005

All business actions shall execute through Workflows and APIs.

---

### SCR-006

Every screen shall display immutable Business Codes rather than database identifiers whenever possible.

---

### SCR-007

Every screen shall support Audit Log and Business Event generation through application services.

---

### SCR-008

Navigation shall remain consistent across all modules.

---

### SCR-009

Critical operational screens shall support QR and Barcode scanning.

---

### SCR-010

Dashboard screens shall be configurable according to user roles.

---

# Integration

Screen Catalog integrates with:

UI Flows

Module Specifications

Dashboard Definitions

API Contracts

Workflow

Permissions

Barcode & QR Model

Mobile Application

Notifications

Analytics

AI

---

# Future Extensions

The architecture supports:

Low-Code Screen Builder

AI Generated Forms

Custom User Dashboards

Drag-and-Drop Layouts

Digital Twin Visualization

Augmented Reality Interfaces

Voice Navigation

Multi-Monitor Factory Displays

---

# Screen Philosophy

Each screen represents a single business capability within Naswood OS.

Screens provide intuitive access to manufacturing information while enforcing workflow, security and traceability.

A consistent screen architecture ensures usability, maintainability and scalability across web, mobile and future application platforms.
