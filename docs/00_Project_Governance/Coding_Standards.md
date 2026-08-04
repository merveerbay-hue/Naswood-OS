# Coding Standards

**Project:** Naswood OS
**Document:** Coding Standards
**Version:** 1.0
**Status:** Approved

---

# Purpose

This document defines the software development standards for Naswood OS.

These standards ensure that all contributors, AI coding assistants and developers produce consistent, maintainable and scalable software.

Every implementation must follow these standards.

---

# Guiding Principles

Development must always follow the principles defined in:

- Project_Principles.md
- Architecture_Decisions.md
- System_Architecture.md

Business rules always take precedence over technical convenience.

---

# General Development Principles

- Clean Architecture
- Domain-Driven Design (DDD)
- SOLID Principles
- Event-Driven Architecture
- API First
- Modular Design
- Testable Code
- Maintainable Code
- Readable Code
- AI Friendly Code

---

# Naming Conventions

## Classes

PascalCase

Example

MaterialService

ProductionOrder

TransformationEngine

---

## Interfaces

Prefix with "I"

Example

IMaterialRepository

IProductionService

---

## Methods

camelCase

Example

createMaterial()

calculateYield()

generateWorkOrder()

---

## Variables

camelCase

Example

materialId

productionOrder

currentMoisture

---

## Constants

UPPER_SNAKE_CASE

Example

MAX_MOISTURE

DEFAULT_PRESSURE

---

## Database Tables

snake_case

Examples

material

production_order

inventory_movement

material_attribute

---

## Database Columns

snake_case

Examples

material_id

created_at

updated_at

---

# File Organization

One responsibility per file.

Large files should be split into smaller modules.

Maximum recommended file size:

500 lines

If exceeded, refactor.

---

# Folder Organization

Group code by business domain.

Correct

Production/

Inventory/

Quality/

Maintenance/

Incorrect

Controllers/

Models/

Helpers/

Repositories/

Business functionality comes before technical layer.

---

# Code Style

Prefer readable code over clever code.

Avoid deeply nested conditions.

Prefer early return.

Keep methods short.

Maximum recommended method size:

50 lines

Maximum nesting:

3 levels

---

# Comments

Code should explain "how".

Comments should explain "why".

Avoid obvious comments.

Bad

// increase i

Good

// Moisture is limited according to Thermowood process requirements.

---

# Error Handling

Never ignore exceptions.

Errors must contain:

- Error Code
- Human Readable Message
- Technical Details
- Timestamp
- Correlation ID

Business errors and system errors shall be separated.

---

# Logging

Log important business events.

Examples

Production Started

Transformation Completed

Inventory Reserved

Quality Approved

Recipe Changed

Machine Alarm

Avoid excessive logging.

Sensitive information must never be logged.

---

# Database Rules

Use UUID as primary key.

Never expose internal IDs.

Business Codes are displayed to users.

No hard delete.

Use transactions for critical operations.

Indexes required for:

- UUID
- Business Code
- Foreign Keys
- Frequently filtered columns

---

# API Standards

REST First

JSON format

Versioned APIs

Example

/api/v1/materials

Consistent response structure.

Example

{
  "success": true,
  "data": {},
  "errors": []
}

---

# Validation

Validate at every layer.

Client

↓

API

↓

Domain

↓

Database

Never trust client input.

---

# Security

Never trust external input.

Validate permissions.

Use parameterized queries.

Hash passwords.

Encrypt sensitive information.

Audit important actions.

---

# Performance

Avoid unnecessary database queries.

Prefer pagination.

Lazy load large collections.

Optimize indexes.

Avoid N+1 queries.

Cache static data.

---

# Testing

Every business rule should be testable.

Testing levels

Unit Tests

Integration Tests

API Tests

End-to-End Tests

Factory Acceptance Tests

---

# Event Standards

Business events are immutable.

Examples

MaterialReceived

TransformationCompleted

PackageCreated

ShipmentCompleted

Events must never be edited.

---

# Git Standards

Branch Naming

feature/material-engine

feature/production

bugfix/inventory

hotfix/login

Commit Messages

feat:

fix:

docs:

refactor:

test:

chore:

Example

feat(material): add material genealogy support

---

# Documentation

Every public class requires documentation.

Every API requires documentation.

Every business rule must reference the relevant documentation.

---

# AI Coding Rules

AI-generated code must:

Follow this document.

Follow Architecture Decisions.

Never invent business rules.

Never duplicate logic.

Always preserve traceability.

Always preserve auditability.

When uncertain, request clarification rather than making assumptions.

---

# Manufacturing Rules

Software must reflect real factory operations.

Factory workflow always has priority over software simplicity.

Every physical material must remain traceable.

Recovery is part of production.

Waste must always be classified.

Transformation creates value.

---

# Definition of Done

A feature is complete only if:

- Business rules implemented
- Tests completed
- Documentation updated
- API documented
- Permissions validated
- Events generated
- Audit logging implemented
- Traceability preserved
- Code reviewed

---

# Future Extensions

This standard is designed to support:

- Multi-Factory
- Multi-Company
- AI Assisted Development
- Digital Twin
- PLC Integration
- Machine Vision
- Predictive Analytics

---

# Final Rule

Write software that a factory can trust for the next 20 years.

Every line of code should improve traceability, maintainability and operational excellence.
