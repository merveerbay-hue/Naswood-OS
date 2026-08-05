# TASK-037 — Lead Management

**Module:** Sales

**Sprint:** Sprint 03 – Sales

**Category:** CRM

**Priority:** Critical

**Estimated Effort:** 8 Days

**Status:** Planned

---

# Purpose

Develop the Lead Management module for Naswood OS.

The Lead Management module manages the complete lifecycle of potential customers from the first inquiry through qualification and conversion into active customers.

It enables Sales teams to capture, qualify, prioritize and convert opportunities while maintaining complete visibility over the sales pipeline.

---

# Objectives

- Centralized Lead Management
- Digital Sales Pipeline
- Lead Qualification
- Opportunity Tracking
- Sales Performance Monitoring
- Customer Conversion
- Complete CRM Traceability

---

# Scope

The Lead Management module includes

- Lead Registration
- Lead Qualification
- Lead Assignment
- Opportunity Management
- Activity Tracking
- Lead Scoring
- Pipeline Management
- Lead Conversion
- Follow-Up Scheduling
- Lead Analytics

Out of Scope

- Quotations
- Sales Orders
- Customer Invoices
- Customer Payments

---

# Lead Architecture

```
Marketing

↓

Lead

↓

Qualification

↓

Opportunity

↓

Quotation

↓

Customer

↓

Sales Order
```

---

# Lead Lifecycle

```
New

↓

Assigned

↓

Contacted

↓

Qualified

↓

Opportunity

↓

Quotation

↓

Won

↓

Converted

or

Lost

or

Archived
```

Reference

Status_Lifecycle.md

---

# Lead Sources

Supports

- Website
- Email
- Phone
- Trade Fair
- Social Media
- Dealer
- Referral
- Advertisement
- AI Recommendation
- Manual Entry

---

# Lead Header

Each Lead contains

## General Information

- Lead Number
- Lead Name
- Company
- Contact Person
- Source
- Assigned Salesperson
- Status
- Priority
- Creation Date

---

## Contact Information

- Phone
- Mobile
- Email
- Website
- Country
- City
- Address

---

## Business Information

- Industry
- Company Size
- Annual Revenue
- Interested Products
- Expected Budget
- Estimated Closing Date

---

# Product Interest

Supports

- CLT
- Glulam
- Thermowood
- Solid Wood Panels
- Timber
- Pellet
- Custom Manufacturing
- Engineering Services

Multiple product interests are supported.

---

# Lead Qualification

Evaluation criteria

- Budget
- Authority
- Need
- Timeline
- Technical Compatibility

Qualification result

- Qualified
- Requires Follow-up
- Unqualified

---

# Lead Scoring

Automatic scoring

Criteria

- Company Size
- Budget
- Project Size
- Engagement
- Product Match
- Sales Activities

Overall Score

```
0 – 100
```

Higher scores receive higher priority.

---

# Opportunity Management

Each qualified lead may create

- One Opportunity
- Multiple Opportunities
- Multiple Quotations

Tracks

- Estimated Value
- Closing Probability
- Expected Closing Date
- Competitors
- Project Stage

---

# Sales Activities

Supports

- Phone Call
- Meeting
- Site Visit
- Email
- Video Meeting
- Product Demo
- Sample Delivery
- Follow-Up

Each activity records

- Date
- Responsible Employee
- Notes
- Outcome
- Next Action

---

# Follow-Up Management

Supports

- Scheduled Calls
- Meetings
- Reminders
- Tasks
- Email Follow-Up

Automatic reminders are generated.

---

# Lead Assignment

Supports

- Manual Assignment
- Territory Assignment
- Product-Based Assignment
- Workload Balancing
- AI Recommendation

---

# Lead Conversion

Workflow

```
Qualified Lead

↓

Customer Created

↓

Opportunity Created

↓

Quotation

↓

Sales Order
```

Reference

TASK-036_Customer.md

---

# Lost Leads

Lost reasons

- Price
- Competitor
- Budget
- Project Cancelled
- No Response
- Technical Mismatch
- Duplicate

Lost reason is mandatory.

---

# Attachments

Supports

- Drawings
- Specifications
- Customer Documents
- Meeting Notes
- Photos
- Emails

Reference

TASK-012_File_Upload.md

---

# Search

Supports

- Lead Number
- Company
- Contact Person
- Salesperson
- Product
- Status
- Lead Source
- Date Range

Reference

Search_Filtering.md

---

# Dashboard Information

Displays

- New Leads
- Qualified Leads
- Conversion Rate
- Pipeline Value
- Open Opportunities
- Sales Activities
- Lead Sources

Reference

TASK-042_Sales_Dashboard.md

---

# Reports

Supports

- Lead Register
- Lead Conversion
- Sales Pipeline
- Lead Sources
- Sales Activities
- Lost Lead Analysis

Reference

TASK-043_Sales_Reports.md

---

# API Endpoints

```
GET /api/v1/leads

GET /api/v1/leads/{id}

POST /api/v1/leads

PUT /api/v1/leads/{id}

DELETE /api/v1/leads/{id}

POST /api/v1/leads/{id}/qualify

POST /api/v1/leads/{id}/convert

POST /api/v1/leads/{id}/assign

POST /api/v1/leads/{id}/close
```

Reference

Sales_API.md

---

# Validation Rules

The system validates

- Lead Number is unique.
- Company Name is mandatory.
- Contact Person is mandatory.
- Email format is valid.
- Assigned Salesperson exists.
- Qualified Lead required before conversion.
- Lost reason required when status is Lost.

Reference

Validation_Rules.md

---

# Security

Supports

- Role-Based Access
- Territory Authorization
- Company Isolation
- Plant Isolation
- Sales Team Authorization

Reference

Security.md

Permission_Model.md

---

# Audit

Records

- Lead Created
- Lead Updated
- Lead Assigned
- Lead Qualified
- Opportunity Created
- Lead Converted
- Lead Closed

Reference

Audit_Log.md

Logging.md

---

# Notifications

Supports

- New Lead Assigned
- Follow-Up Reminder
- Opportunity Created
- Lead Qualified
- Lead Converted
- High Value Lead
- Inactive Lead Warning

Reference

Notification_System.md

---

# Events

Publishes

- LeadCreated
- LeadAssigned
- LeadQualified
- OpportunityCreated
- LeadConverted
- LeadClosed

Reference

Event_Model.md

Integration_Events.md

---

# Mobile Support

Supports

- Lead Creation
- Customer Visit
- GPS Location
- Business Card Scan
- Photo Upload
- Voice Notes
- Offline CRM

Reference

Sales_Mobile.md

---

# Performance

Targets

- Lead Creation < 1 second
- Lead Search < 300 ms
- Lead Assignment < 500 ms
- Pipeline Calculation < 2 seconds
- Support 2,000,000+ leads

Reference

Performance.md

Caching.md

---

# Naswood Examples

### Example 1

```
Trade Fair

↓

Architect Visits Booth

↓

Lead Created

↓

Sales Follow-Up

↓

Qualified

↓

Quotation
```

---

### Example 2

```
Website Inquiry

↓

CLT Project

↓

Lead Score 94

↓

Opportunity

↓

Customer

↓

Sales Order
```

---

### Example 3

```
Dealer Referral

↓

Thermowood Project

↓

Meeting Scheduled

↓

Sample Sent

↓

Won
```

---

# Acceptance Criteria

The Lead Management module shall

- Manage the complete lead lifecycle.
- Support qualification and scoring.
- Support opportunity creation.
- Support follow-up activities and reminders.
- Convert qualified leads into customers.
- Provide complete CRM visibility.
- Publish lead lifecycle events.
- Follow all shared platform standards.

---

# Dependencies

Depends On

- TASK-012_File_Upload.md
- TASK-013_Audit_Log.md
- TASK-014_Settings.md
- TASK-036_Customer.md
- Sales_API.md
- Validation_Rules.md

---

# Related Documents

Sales_Architecture.md

Sales_API.md

Sales_Workflow.md

TASK-036_Customer.md

TASK-038_Sales_Order.md

TASK-039_Delivery.md

TASK-040_Customer_Invoice.md

TASK-041_Customer_Payment.md

TASK-042_Sales_Dashboard.md

TASK-043_Sales_Reports.md

Security.md

Permission_Model.md

Validation_Rules.md

Performance.md

Caching.md

Search_Filtering.md

Audit_Log.md

Logging.md

Notification_System.md

Event_Model.md

Integration_Events.md
