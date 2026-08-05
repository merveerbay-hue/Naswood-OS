# TASK-038 — Opportunity

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** Sales Pipeline

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Planned

---

# Purpose

Develop the Opportunity Management module for Naswood OS.

The Opportunity module manages qualified sales opportunities from initial customer interest until quotation, negotiation, contract award and Sales Order creation.

It enables the Sales team to monitor the entire sales pipeline, estimate future revenue and improve sales forecasting.

---

# Objectives

- Opportunity Pipeline Management
- Revenue Forecasting
- Sales Probability Analysis
- Activity Tracking
- Quotation Integration
- CRM Visibility
- Complete Sales Traceability

---

# Scope

The Opportunity module includes

- Opportunity Creation
- Sales Pipeline
- Opportunity Qualification
- Activity Management
- Revenue Forecasting
- Competitor Tracking
- Win/Loss Analysis
- Quotation Generation
- Opportunity Closure
- Pipeline Reporting

Out of Scope

- Customer Master
- Sales Order
- Customer Invoice
- Customer Payment

---

# Opportunity Architecture

```
Lead

↓

Qualified Lead

↓

Opportunity

↓

Activities

↓

Quotation

↓

Negotiation

↓

Won

↓

Sales Order

or

Lost
```

---

# Opportunity Lifecycle

```
Created

↓

Qualified

↓

Proposal

↓

Negotiation

↓

Contract Review

↓

Won

↓

Sales Order

or

Lost

or

Cancelled
```

Reference

Status_Lifecycle.md

---

# Opportunity Sources

Supports

- Qualified Lead
- Existing Customer
- Manual Entry
- Dealer
- Website Inquiry
- Marketing Campaign
- Trade Fair
- AI Recommendation

---

# Opportunity Header

Each Opportunity contains

## General Information

- Opportunity Number
- Opportunity Name
- Customer
- Contact Person
- Assigned Salesperson
- Company
- Plant
- Status
- Priority
- Source

---

## Commercial Information

- Estimated Revenue
- Currency
- Probability
- Expected Closing Date
- Project Value
- Discount Expectation
- Competitors

Reference

Currency.md

---

## Products

Supports

- CLT
- Glulam
- Thermowood
- Solid Wood Panels
- Timber
- Pellet
- Custom Manufacturing
- Engineering Services

Multiple products are supported.

---

# Opportunity Stages

Supports

- Qualification
- Needs Analysis
- Technical Review
- Proposal
- Negotiation
- Contract Review
- Won
- Lost

Each stage records

- Start Date
- End Date
- Responsible Employee
- Completion Percentage

---

# Sales Probability

Automatic probability calculation based on

- Opportunity Stage
- Customer Score
- Sales Activity
- Historical Win Rate
- Competitor Presence
- AI Prediction

Probability Range

```
0% – 100%
```

---

# Revenue Forecast

Calculates

- Expected Revenue
- Weighted Revenue
- Monthly Forecast
- Quarterly Forecast
- Annual Forecast

Formula

```
Estimated Revenue

×

Probability

=

Weighted Revenue
```

---

# Competitor Analysis

Stores

- Competitor Name
- Estimated Price
- Competitive Advantage
- Risk Level
- Win Probability

Supports multiple competitors.

---

# Sales Activities

Supports

- Call
- Meeting
- Site Visit
- Product Presentation
- Technical Workshop
- Sample Delivery
- Email
- Video Conference

Each activity stores

- Date
- Duration
- Responsible Employee
- Result
- Next Action

---

# Follow-Up

Supports

- Tasks
- Calendar Events
- Email Reminders
- Call Reminders
- Visit Scheduling

Automatic reminders are generated.

---

# Opportunity Team

Supports

- Sales Representative
- Sales Manager
- Technical Sales Engineer
- Project Manager
- Architect
- Finance Representative

Multiple team members may participate.

---

# Quotation Integration

Workflow

```
Opportunity

↓

Quotation

↓

Revision

↓

Negotiation

↓

Accepted

↓

Sales Order
```

Reference

Sales Quotation Module

---

# Opportunity Closure

Won

```
Opportunity

↓

Sales Order
```

Lost

Reasons

- Price
- Competitor
- Budget
- Project Cancelled
- Technical Issue
- No Decision
- Other

Lost reason is mandatory.

---

# Attachments

Supports

- Drawings
- Specifications
- BOQ
- Meeting Notes
- Emails
- Photos
- Contracts
- Customer Documents

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Opportunity Number
- Customer
- Salesperson
- Product
- Status
- Stage
- Probability
- Date Range

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- Open Opportunities
- Pipeline Value
- Weighted Revenue
- Closing This Month
- Won Opportunities
- Lost Opportunities
- Sales Forecast

Reference

TASK-042_Sales_Dashboard.md

---

# Reports

Supports

- Opportunity Register
- Pipeline Analysis
- Sales Forecast
- Win/Loss Analysis
- Opportunity by Salesperson
- Opportunity Aging

Reference

TASK-043_Sales_Reports.md

---

# API Endpoints

```
GET /api/v1/opportunities

GET /api/v1/opportunities/{id}

POST /api/v1/opportunities

PUT /api/v1/opportunities/{id}

DELETE /api/v1/opportunities/{id}

POST /api/v1/opportunities/{id}/advance-stage

POST /api/v1/opportunities/{id}/close-won

POST /api/v1/opportunities/{id}/close-lost

GET /api/v1/opportunities/search
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Opportunity Number is unique.
- Customer exists.
- Assigned Salesperson exists.
- Probability is between 0–100%.
- Expected Closing Date is valid.
- Estimated Revenue ≥ 0.
- Lost Reason required when Closed Lost.
- Won Opportunities require Sales Order creation.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Sales Territory Authorization
- Company Isolation
- Plant Isolation
- Revenue Visibility Rules

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Opportunity Created
- Opportunity Updated
- Stage Changed
- Revenue Changed
- Probability Updated
- Opportunity Won
- Opportunity Lost

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- New Opportunity Assigned
- Stage Changed
- Follow-Up Reminder
- Closing Date Reminder
- Opportunity Won
- Opportunity Lost
- High Value Opportunity

Reference

Notification_System.md

---

# Events

Publishes

- OpportunityCreated
- OpportunityUpdated
- OpportunityStageChanged
- OpportunityWon
- OpportunityLost
- OpportunityConvertedToSalesOrder

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Opportunity View
- Stage Update
- Customer Visit
- GPS Check-In
- Photo Upload
- Voice Notes
- Offline CRM

Reference

Sales_Mobile.md

---

# Performance

Targets

- Opportunity Creation < 1 second
- Opportunity Search < 300 ms
- Pipeline Calculation < 2 seconds
- Forecast Calculation < 2 seconds
- Support 1,000,000+ opportunities

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Architect

↓

CLT School Project

↓

Estimated Value

12,000,000 TRY

↓

Negotiation

↓

Won

↓

Sales Order
```

---

### Example 2

```
Construction Company

↓

Thermowood Facade

↓

Probability

80%

↓

Quotation

↓

Contract
```

---

### Example 3

```
Export Customer

↓

Glulam Project

↓

Technical Workshop

↓

Proposal

↓

Sales Pipeline
```

---

# Acceptance Criteria

The Opportunity module shall

- Manage qualified sales opportunities.
- Support configurable sales pipeline stages.
- Calculate weighted revenue forecasts.
- Track competitors and sales activities.
- Integrate with Quotations and Sales Orders.
- Maintain complete opportunity history.
- Publish sales pipeline events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-012_File_Upload.md
- TASK-013_Audit_Log.md
- TASK-014_Settings.md
- TASK-036_Customer.md
- TASK-037_Lead.md
- Sales_API.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

TASK-036_Customer.md

TASK-037_Lead.md

TASK-039_Quotation.md

TASK-040_Sales_Order.md

TASK-042_Sales_Dashboard.md

TASK-043_Sales_Reports.md

Security.md

Permission_Model.md

Validation_Rules.md

Currency.md

Performance.md

Caching.md

Search_Filtering.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md
