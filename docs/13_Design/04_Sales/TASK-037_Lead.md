> **UX authority note:** `+ New` / Create wireframes in this TASK are historical. Live CTAs: [`Sales_Screens.md`](./Sales_Screens.md) · [`Screen_Types.md`](../Common/Screen_Types.md) § Create matrix · Process_Screens.

# TASK-037 — Lead

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Lead module manages potential customers before they become official customers within Naswood ERP.

A Lead represents an organization or individual that has shown interest in Naswood products or services but has not yet been qualified.

The Lead module is the starting point of the CRM and Sales pipeline.

---

# Design Goals

The module is designed to

- Capture all sales opportunities
- Standardize lead qualification
- Improve conversion rates
- Prevent duplicate leads
- Track lead lifecycle
- Integrate with CRM activities
- Enable AI-based lead scoring

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Lead List

────────────────────────────────────────────────────────────

Search

Filters

Lead Pipeline

Lead Grid

────────────────────────────────────────────────────────────

**Capture lead** / Lead kaydet

Import

Export

────────────────────────────────────────────────────────────
```

Selecting a lead opens the Lead Detail screen.

---

# Lead Detail Layout

```
────────────────────────────────────────────────────────────

Lead Header

────────────────────────────────────────────────────────────

General

Contacts

Company

Activities

Opportunities

Documents

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Lead Header

Displays

- Lead Number
- Lead Name
- Company Name
- Status
- Lead Score
- Assigned Salesperson
- Source
- Expected Value
- Company
- Plant

Actions

- Edit
- Assign
- Qualify
- Convert
- Archive

---

# Tab — General

Stores

## Basic Information

- Lead Number
- Lead Name
- Company Name
- Industry
- Website
- Source
- Status
- Priority

## Qualification

- Budget
- Authority
- Need
- Timeline

(BANT Model)

## Sales Information

- Estimated Revenue
- Probability
- Expected Closing Date
- Competitor

---

# Lead Sources

Supports

- Website
- Contact Form
- Phone Call
- Email
- Fair
- Dealer
- Social Media
- Advertisement
- Referral
- Existing Customer
- Manual Entry

---

# Lead Status

```
New

↓

Contacted

↓

Qualified

↓

Proposal Requested

↓

Converted

or

Lost

or

Archived
```

---

# Lead Priority

Supports

- Low
- Medium
- High
- Critical

Priority affects dashboard visibility.

---

# Lead Score

Calculated automatically.

Score range

```
0 — 100
```

Example

| Score | Meaning |
|--------|----------|
| 0–30 | Cold |
| 31–60 | Warm |
| 61–80 | Qualified |
| 81–100 | Hot |

AI may update scores continuously.

---

# Tab — Contacts

Stores

- Name
- Position
- Department
- Email
- Mobile
- Office Phone
- Preferred Contact Method

Supports multiple contacts.

---

# Tab — Company

Displays

- Company Name
- Industry
- Employee Count
- Annual Revenue
- Country
- City
- Website
- ERP System
- Existing Supplier

---

# Tab — Activities

Timeline includes

- Phone Calls
- Emails
- Meetings
- Site Visits
- Tasks
- Notes
- Attachments

Supports chronological view.

---

# Tab — Opportunities

Displays

Converted opportunities

Information

- Opportunity Number
- Value
- Probability
- Stage
- Closing Date

Reference

TASK-038_Opportunity.md

---

# Tab — Documents

Supports

- Business Card
- Company Profile
- Drawings
- Specifications
- Photos
- Emails
- Attachments

Reference

TASK-012_File_Upload.md

---

# Tab — Timeline

Displays complete activity history.

```
Lead Created

↓

Assigned

↓

Phone Call

↓

Meeting

↓

Site Visit

↓

Quotation Request

↓

Converted
```

---

# Tab — Notes

Supports

- Internal Notes
- Commercial Notes
- Technical Notes

Supports mentions and attachments.

---

# Lead Assignment

Supports assignment by

- Salesperson
- Sales Manager
- Territory
- Product Group
- Region

Supports reassignment.

---

# Lead Qualification

Qualification checklist

- Contact Established
- Budget Confirmed
- Decision Maker Identified
- Product Interest
- Project Timeline
- Potential Revenue

System calculates qualification level.

---

# Lead Conversion

Workflow

```
Qualified Lead

↓

Customer

↓

Opportunity

↓

Activities Preserved
```

Supports automatic creation of

- Customer
- Opportunity
- Contacts
- Addresses
- Activity History

---

# Search

Supports

- Lead Number
- Company
- Contact
- Email
- Phone
- Salesperson
- Source
- Status

Supports fuzzy search.

---

# Filters

Supports

- Status
- Source
- Priority
- Salesperson
- Industry
- Country
- Date
- Company
- Plant

---

# Lead KPIs

Displays

- Total Leads
- New Leads
- Qualified Leads
- Conversion Rate
- Average Response Time
- Average Qualification Time
- Lost Leads
- Estimated Revenue

---

# User Actions

Users may

- Create Lead
- Edit Lead
- Assign Lead
- Merge Duplicate Leads
- Qualify Lead
- Convert Lead
- Archive Lead
- Schedule Activity
- Create Opportunity

---

# Validation Rules

The system validates

- Lead Number is unique.
- Lead Name is required.
- Company Name is required.
- Assigned Salesperson is required.
- Source is required.
- Email format is valid.
- Duplicate Lead warning by Company + Email.

---

# Permissions

Supports

- View Lead
- Create Lead
- Edit Lead
- Delete Lead
- Assign Lead
- Convert Lead
- Export Leads

Reference

Permission_Model.md

---

# Notifications

Triggers

- New Lead Assigned
- Lead Qualified
- Lead Converted
- High Score Lead
- Lead Inactive
- Follow-up Reminder

Reference

Notification_System.md

---

# Audit

Records

- Lead Created
- Lead Updated
- Lead Assigned
- Lead Qualified
- Lead Converted
- Status Changed

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Lead Search
- Lead Creation
- Business Card Scan
- GPS Check-In
- Activity Logging
- Voice Notes
- Photo Upload
- Offline Mode

Reference

Sales_Mobile.md

---

# API References

```
GET    /leads

GET    /leads/{id}

POST   /leads

PUT    /leads/{id}

DELETE /leads/{id}

POST   /leads/{id}/assign

POST   /leads/{id}/convert

GET    /leads/search
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Opportunity
- Activities
- CRM
- Quotation
- Dashboard
- Reports
- AI Copilot

---

# UI Components

Uses standard platform components

- Kanban Pipeline
- Data Grid
- Search Box
- Filter Panel
- Timeline
- Activity Feed
- KPI Cards
- Status Badge
- Progress Indicator
- Avatar

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — Trade Fair

```
Lead

ABC Construction

↓

Source

Wood Fair

↓

Interest

CLT Panels

↓

Status

Qualified
```

---

### Example 2 — Website Form

```
Lead

Nord Timber

↓

Country

Germany

↓

Interest

Thermowood

↓

Priority

High
```

---

### Example 3 — Architect

```
Lead

XYZ Architecture

↓

Project

Wood Hotel

↓

Expected Value

€750,000

↓

Converted

Opportunity
```
