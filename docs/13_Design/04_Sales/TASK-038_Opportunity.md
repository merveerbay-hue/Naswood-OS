# TASK-038 — Opportunity

**Module:** Sales

**Document Type:** Design Specification

**Version:** 1.0

**Status:** Approved

**Owner:** Sales Product Team

---

# Purpose

The Opportunity module manages qualified sales opportunities after a Lead has been converted.

An Opportunity represents a real commercial project with identified customer requirements, expected revenue, estimated closing date and probability of success.

It serves as the commercial planning center before quotation preparation.

---

# Design Goals

The module is designed to

- Manage commercial opportunities
- Improve sales forecasting
- Standardize opportunity lifecycle
- Measure pipeline value
- Support project-based sales
- Track competitors
- Integrate AI opportunity scoring

---

# Screen Layout

```
────────────────────────────────────────────────────────────

Opportunity List

────────────────────────────────────────────────────────────

Search

Filters

Pipeline

Opportunity Grid

────────────────────────────────────────────────────────────

+ New Opportunity

Forecast

Export

────────────────────────────────────────────────────────────
```

Selecting an opportunity opens the Opportunity Detail screen.

---

# Opportunity Detail Layout

```
────────────────────────────────────────────────────────────

Opportunity Header

────────────────────────────────────────────────────────────

General

Customer

Products

Activities

Quotations

Forecast

Competitors

Documents

Timeline

Notes

────────────────────────────────────────────────────────────
```

---

# Opportunity Header

Displays

- Opportunity Number
- Opportunity Name
- Customer
- Status
- Stage
- Probability
- Expected Revenue
- Expected Closing Date
- Assigned Salesperson
- Company
- Plant

Actions

- Edit
- Advance Stage
- Create Quotation
- Mark Won
- Mark Lost
- Archive

---

# Tab — General

Stores

## Basic Information

- Opportunity Number
- Opportunity Name
- Opportunity Type
- Sales Channel
- Source
- Priority

## Commercial Information

- Expected Revenue
- Expected Margin
- Probability
- Closing Date
- Currency

## Project Information

- Project Name
- Project Location
- Estimated Quantity
- Expected Start Date

---

# Opportunity Types

Supports

- New Project
- Existing Customer
- Export Project
- Dealer Project
- Government Tender
- Public Procurement
- Framework Agreement
- Repeat Order

---

# Opportunity Pipeline

```
Qualified

↓

Needs Analysis

↓

Solution Design

↓

Quotation

↓

Negotiation

↓

Contract Review

↓

Won

or

Lost
```

---

# Opportunity Status

Supports

- Open
- In Progress
- On Hold
- Won
- Lost
- Cancelled
- Archived

---

# Probability

Supports

| Stage | Default Probability |
|---------|--------------------:|
| Qualified | 20% |
| Needs Analysis | 35% |
| Solution Design | 50% |
| Quotation | 70% |
| Negotiation | 85% |
| Contract Review | 95% |
| Won | 100% |

Users with permission may override probability.

---

# Tab — Customer

Displays

- Customer
- Contacts
- Industry
- Previous Orders
- Revenue History
- Credit Status
- Customer Rating

Reference

TASK-036_Customer.md

---

# Tab — Products

Stores

Potential products

- Product
- Quantity
- Unit
- Estimated Price
- Estimated Revenue
- Expected Margin

Examples

- CLT Panels
- Thermowood
- Glulam
- Solid Panels
- Pellet

---

# Tab — Activities

Displays

- Meetings
- Phone Calls
- Emails
- Site Visits
- Technical Reviews
- Samples Sent
- Internal Discussions

Supports activity scheduling.

---

# Tab — Quotations

Displays

Linked quotations

Information

- Quotation Number
- Revision
- Status
- Value
- Date

Actions

- Create Quotation
- Open Quotation
- Compare Revisions

Reference

TASK-039_Quotation.md

---

# Tab — Forecast

Displays

- Expected Revenue
- Weighted Revenue
- Forecast Month
- Closing Probability
- Forecast Confidence

Formula

```
Expected Revenue

×

Probability

=

Weighted Forecast
```

---

# Tab — Competitors

Supports

- Competitor Name
- Competitor Products
- Strengths
- Weaknesses
- Estimated Price
- Win Probability

Example

```
Competitor

ABC Timber

↓

Estimated Price

€480,000

↓

Probability

70%
```

---

# Tab — Documents

Supports

- RFQ Documents
- Drawings
- Specifications
- BOQ
- Customer Files
- Technical Documents

Reference

TASK-012_File_Upload.md

---

# Tab — Timeline

Displays

```
Lead Converted

↓

Customer Meeting

↓

Requirements

↓

Quotation

↓

Negotiation

↓

Won
```

Every event is timestamped.

---

# Tab — Notes

Supports

- Commercial Notes
- Technical Notes
- Internal Notes
- Risk Assessment
- Next Actions

Supports mentions and attachments.

---

# Opportunity Scoring

AI calculates

- Win Probability
- Customer Engagement
- Activity Level
- Competitor Risk
- Forecast Accuracy

Displayed as

```
Opportunity Score

0 — 100
```

---

# Forecasting

Supports

- Monthly Forecast
- Quarterly Forecast
- Annual Forecast
- Salesperson Forecast
- Product Forecast
- Regional Forecast

---

# Search

Supports

- Opportunity Number
- Opportunity Name
- Customer
- Product
- Salesperson
- Project
- Competitor

Supports fuzzy search.

---

# Filters

Supports

- Stage
- Status
- Salesperson
- Customer
- Region
- Product
- Expected Closing
- Probability
- Company
- Plant

---

# Opportunity KPIs

Displays

- Open Opportunities
- Pipeline Value
- Weighted Forecast
- Win Rate
- Lost Rate
- Average Deal Size
- Average Sales Cycle
- Forecast Accuracy

---

# User Actions

Users may

- Create Opportunity
- Edit Opportunity
- Change Stage
- Schedule Activities
- Create Quotation
- Duplicate Opportunity
- Mark Won
- Mark Lost
- Archive Opportunity

---

# Validation Rules

The system validates

- Opportunity Number is unique.
- Customer is required.
- Opportunity Name is required.
- Probability is between 0 and 100.
- Expected Revenue ≥ 0.
- Closing Date is required.
- Won opportunities require at least one quotation.

---

# Permissions

Supports

- View Opportunity
- Create Opportunity
- Edit Opportunity
- Delete Opportunity
- Mark Won
- Mark Lost
- Create Quotation
- Export Opportunities

Reference

Permission_Model.md

---

# Notifications

Triggers

- Opportunity Assigned
- Stage Changed
- High Value Opportunity
- Closing Date Approaching
- Opportunity Won
- Opportunity Lost

Reference

Notification_System.md

---

# Audit

Records

- Opportunity Created
- Opportunity Updated
- Stage Changed
- Probability Updated
- Forecast Updated
- Opportunity Won
- Opportunity Lost

Reference

Audit_Log.md

---

# Mobile Behavior

Supports

- Opportunity Search
- Customer Meeting Notes
- Activity Logging
- GPS Check-In
- Voice Notes
- Photo Upload
- Forecast View
- Offline Mode

Reference

Sales_Mobile.md

---

# API References

```
GET    /opportunities

GET    /opportunities/{id}

POST   /opportunities

PUT    /opportunities/{id}

DELETE /opportunities/{id}

POST   /opportunities/{id}/win

POST   /opportunities/{id}/lose

GET    /opportunities/search
```

Reference

Sales_API.md

---

# Related Modules

- Customer
- Lead
- Quotation
- Sales Order
- CRM Activities
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
- KPI Cards
- Forecast Chart
- Progress Indicator
- Activity Feed
- Attachment Viewer

Reference

Dashboard_Layout.md

Navigation.md

Theme.md

---

# Naswood Examples

### Example 1 — CLT Hotel Project

```
Opportunity

Hotel CLT Project

↓

Customer

ABC Construction

↓

Estimated Revenue

€1,250,000

↓

Probability

75%
```

---

### Example 2 — Export Project

```
Opportunity

Germany Timber Project

↓

Product

Thermowood

↓

Forecast

Q3 2027

↓

Stage

Negotiation
```

---

### Example 3 — Government Tender

```
Opportunity

Municipality Sports Hall

↓

Product

Glulam Structure

↓

Tender Closing

15 August

↓

Status

Quotation
```
