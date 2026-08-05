# AI Widgets

**Module:** Design System

**Category:** AI

**Version:** 1.0

**Status:** Approved

---

# Purpose

AI Widgets are reusable intelligent UI components embedded throughout Naswood OS to provide contextual insights, recommendations, predictions and business assistance.

Unlike AI Chat, widgets operate passively within business screens and dashboards, delivering relevant information without requiring user interaction.

AI Widgets should be available wherever business decisions are made.

---

# Objectives

- Context-Aware Intelligence
- Real-Time Recommendations
- Decision Support
- Predictive Analytics
- Workflow Assistance
- Reusable Components

---

# Design Principles

AI Widgets should be

- Context Aware
- Explainable
- Lightweight
- Actionable
- Non Intrusive

AI should assist users without interrupting workflows.

---

# Widget Categories

Recommendation Widget

Insight Widget

Prediction Widget

Risk Widget

Optimization Widget

Summary Widget

Forecast Widget

Alert Widget

Knowledge Widget

Assistant Widget

Digital Twin Widget

---

# Standard Structure

```
AI Widget

├── Header

├── AI Result

├── Confidence

├── Explanation

├── Recommended Actions

└── Footer
```

---

# Header

Displays

AI Icon

Title

Model

Refresh

Settings

---

# AI Result

Displays

Summary

Recommendation

Prediction

Warning

Opportunity

Business Insight

---

# Confidence Score

Supported

Very High

High

Medium

Low

Very Low

Displayed as

Percentage

Example

94%

---

# Explanation

Displays

Reasoning

Related Records

Affected Processes

Expected Outcome

Potential Risks

---

# Recommended Actions

Supports

Open Record

Generate Report

Create Purchase Request

Create Work Order

Optimize Production

Accept Recommendation

Dismiss Recommendation

Ask AI

Write operations always require confirmation.

---

# Footer

Displays

Generated Time

Last Updated

Data Source

Model Version

---

# Widget Types

## Recommendation Widget

Displays

Suggested Actions

Business Improvements

Optimization Opportunities

---

## Prediction Widget

Displays

Demand Forecast

Machine Failure Prediction

Delivery Forecast

Production Forecast

Inventory Forecast

---

## Insight Widget

Displays

Business Analysis

Root Cause

Anomaly Detection

Trend Summary

---

## Risk Widget

Displays

Supply Risks

Production Risks

Quality Risks

Financial Risks

Safety Risks

---

## Optimization Widget

Displays

Inventory Optimization

Production Optimization

Warehouse Optimization

Energy Optimization

Scheduling Optimization

---

## Summary Widget

Displays

Daily Summary

Shift Summary

Production Summary

Financial Summary

Executive Summary

---

## Knowledge Widget

Displays

Related Procedures

SOP

Manuals

Work Instructions

Policies

---

## Alert Widget

Displays

Critical Events

AI Alerts

Machine Alerts

Quality Alerts

Business Risks

---

# Widget States

Loading

Ready

Refreshing

Offline

Error

No Data

---

# AI Context

Widgets automatically detect

Current Module

Current Record

Current User

Current Workspace

Applied Filters

Permissions

Language

---

# Modules

Supported

Dashboard

Inventory

Purchasing

Sales

Production

Quality

Maintenance

Finance

Analytics

Digital Twin

---

# Refresh

Manual

Automatic

Real-Time

Background Refresh

---

# Interaction

Expand

Collapse

Refresh

Ask AI

Pin

Move

Fullscreen

Dismiss

---

# Personalization

Users may configure

Visible Widgets

Refresh Interval

Preferred Widget Size

Widget Position

Pinned Widgets

---

# Responsive Behaviour

Desktop

Dashboard Layout

Tablet

Compact Layout

Mobile

Single Column

---

# Accessibility

Supports

Keyboard Navigation

Screen Readers

ARIA Labels

Focus Indicators

High Contrast

---

# Performance

Lazy Loading

Streaming Updates

Caching

Background Processing

Incremental Rendering

---

# Security

Widgets respect

Role Permissions

Department Permissions

Module Permissions

Record Permissions

Tenant Isolation

Sensitive data is masked automatically.

---

# React Structure

```tsx
<AiWidget>

    <WidgetHeader />

    <AiContent />

    <ConfidenceScore />

    <RecommendationPanel />

</AiWidget>
```

---

# Widget Registry

Every widget registers

Widget ID

Category

Supported Modules

Permissions

Refresh Strategy

Data Source

Priority

---

# Example Widgets

Inventory Recommendation

Low Stock Prediction

Supplier Risk

Machine Health

OEE Improvement

Energy Optimization

Production Bottleneck

Quality Risk

Cash Flow Forecast

Executive Summary

---

# Best Practices

✓ Explain every recommendation.

✓ Show confidence score.

✓ Keep widgets contextual.

✓ Display only actionable insights.

✓ Allow drill-down.

✓ Keep refresh efficient.

---

# Do

✓ Explain recommendations

✓ Show expected impact

✓ Display related records

✓ Support quick actions

✓ Use semantic colors

---

# Don't

✗ Interrupt workflows

✗ Execute actions automatically

✗ Hide uncertainty

✗ Display irrelevant insights

✗ Ignore permissions

---

# Acceptance Criteria

Widgets follow the official layout.

Recommendations include explanations.

Confidence scores are displayed.

Permissions are enforced.

Write actions require confirmation.

Responsive layout functions correctly.

Accessibility complies with WCAG 2.1 AA.

---

# Related Documents

AI_Chat.md

AI_Copilot.md

AI_Search.md

AI_Actions.md

Dashboard_Widgets.md

KPIs.md

Notifications.md

Workspace.md

Accessibility.md

Design_Tokens.md
