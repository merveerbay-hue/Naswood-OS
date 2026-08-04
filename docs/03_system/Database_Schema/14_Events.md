# Events Database Schema

**Project:** Naswood OS

**Document:** Events Database Schema

**Version:** 2.0

**Status:** Approved

---

# Purpose

The Events module records every significant business event occurring within Naswood OS.

Events provide the foundation for:

- Auditability
- Material Traceability
- Manufacturing Genealogy
- Workflow Automation
- Notification Services
- AI Learning
- Analytics
- Digital Twin Synchronization
- Event-Driven Architecture

Events are immutable.

Events are append-only.

Events are never deleted.

---

# Event Architecture

Business Action

↓

Business Event

↓

Event Store

↓

Event Bus

↓

Subscribers

↓

Notifications

↓

Analytics

↓

AI

↓

Digital Twin

---

# Main Tables

events

event_types

event_categories

event_subscriptions

event_handlers

event_queue

event_failures

event_replay

event_snapshots

---

# Table: events

Event_ID (UUID)

Event_Type

Category

Source_Module

Entity_Type

Entity_ID

Business_Code

Parent_Event_ID

Correlation_ID

Session_ID

User_ID

Organization_ID

Factory_ID

Production_Order_ID

Operation_ID

Material_ID

Package_ID

Shipment_ID

Machine_ID

Severity

Status

Event_Data (JSON)

Metadata (JSON)

Created_At

Processed_At

Version

---

# Table: event_types

Event_Type_ID

Event_Name

Category

Description

Source_Module

Is_System

Version

Active

---

# Table: event_categories

Category_ID

Category_Name

Description

---

# Categories

Master Data

Production

Inventory

Warehouse

Packaging

Quality

Maintenance

Purchasing

Sales

Finance

Logistics

Security

Workflow

Notification

AI

Digital Twin

IoT

Analytics

System

---

# Table: event_subscriptions

Subscription_ID

Subscriber_Name

Module

Event_Type

Handler

Retry_Count

Active

---

# Table: event_handlers

Handler_ID

Handler_Name

Module

Priority

Timeout

Retry

---

# Table: event_queue

Queue_ID

Event_ID

Status

Attempts

Created_At

Processed_At

---

# Table: event_failures

Failure_ID

Event_ID

Reason

Stacktrace

Retry_Count

Resolved

---

# Table: event_replay

Replay_ID

Replay_Name

Start_Date

End_Date

Status

Started_By

---

# Table: event_snapshots

Snapshot_ID

Aggregate_Type

Aggregate_ID

Version

Snapshot_Data

Created_At

---

# Event Naming Standard

<EventDomain><Action>

Examples

MaterialCreated

ProductionOrderReleased

OperationStarted

PackageCreated

ShipmentClosed

MachineStopped

QualityApproved

MaintenanceCompleted

---

# Material Lifecycle Events

MaterialCreated

MaterialReceived

MaterialMeasured

MaterialClassified

MaterialStored

MaterialReserved

MaterialIssued

MaterialTransferred

MaterialConsumed

MaterialTransformed

MaterialSplit

MaterialMerged

MaterialReturned

MaterialReworked

MaterialRejected

MaterialReleased

MaterialPacked

MaterialLoaded

MaterialShipped

MaterialDelivered

MaterialArchived

---

# Production Events

ProductionOrderCreated

ProductionOrderReleased

ProductionOrderScheduled

ProductionOrderStarted

ProductionOrderPaused

ProductionOrderResumed

ProductionOrderCompleted

ProductionOrderCancelled

---

# Operation Events

OperationCreated

OperationReleased

OperationAssigned

OperationStarted

OperationPaused

OperationResumed

OperationCompleted

OperationFailed

OperationCancelled

CycleCompleted

SetupStarted

SetupCompleted

---

# Routing Events

RoutingAssigned

RoutingChanged

RoutingCompleted

AlternativeRoutingSelected

---

# Recipe Events

RecipeAssigned

RecipeChanged

RecipeValidated

RecipeApproved

---

# Machine Events

MachineStarted

MachineStopped

MachineIdle

MachineAlarm

MachineFailure

MachineRecovered

MachineMaintenanceDue

MachineConnected

MachineDisconnected

PLCConnected

PLCDisconnected

---

# Tool Events

ToolInstalled

ToolRemoved

ToolChanged

ToolCalibrated

KnifeChanged

---

# Inventory Events

InventoryReceived

InventoryReserved

InventoryAllocated

InventoryMoved

InventoryAdjusted

InventoryCounted

InventoryBlocked

InventoryReleased

InventoryExpired

---

# Warehouse Events

WarehouseCreated

WarehouseActivated

WarehouseCapacityExceeded

WarehouseLocationAssigned

WarehouseTransferCompleted

WarehouseOptimized

---

# Quality Events

InspectionStarted

InspectionCompleted

InspectionApproved

InspectionRejected

QualityHoldCreated

QualityReleased

CAPAOpened

CAPAClosed

NCRCreated

SPCAlert

---

# Packaging Events

PackageCreated

PackageUpdated

PackageVerified

PackageClosed

PackageStored

PackageReserved

PackageLoaded

PackageShipped

PackageDelivered

PackageReopened

PalletCreated

ContainerLoaded

ContainerSealed

ContainerClosed

LabelPrinted

QRCodeGenerated

BarcodeGenerated

DigitalPassportGenerated

---

# Finished Goods Events

FinishedGoodsCreated

FinishedGoodsReleased

FinishedGoodsPackaged

FinishedGoodsStored

FinishedGoodsReserved

FinishedGoodsAllocated

FinishedGoodsLoaded

FinishedGoodsShipped

FinishedGoodsDelivered

FinishedGoodsArchived

---

# Logistics Events

ShipmentCreated

ShipmentApproved

ShipmentLoaded

ShipmentDispatched

ShipmentDelayed

ShipmentArrived

ShipmentDelivered

TransferOrderCreated

VehicleAssigned

DockAssigned

RouteOptimized

CarrierAssigned

---

# Maintenance Events

MaintenancePlanned

MaintenanceStarted

MaintenanceCompleted

MaintenanceCancelled

BreakdownOccurred

CalibrationCompleted

---

# Sales Events

CustomerCreated

QuotationCreated

QuotationApproved

SalesOrderCreated

SalesOrderConfirmed

SalesOrderCancelled

---

# Purchasing Events

SupplierCreated

PurchaseRequestCreated

PurchaseOrderCreated

GoodsReceived

SupplierEvaluated

---

# Finance Events

InvoiceCreated

InvoicePaid

PaymentReceived

CostCalculated

BudgetExceeded

---

# Security Events

UserLoggedIn

UserLoggedOut

LoginFailed

PermissionChanged

RoleAssigned

PasswordChanged

MFAValidated

---

# AI Events

PredictionGenerated

RecommendationCreated

AnomalyDetected

QualityPredictionCompleted

DemandForecastCompleted

RouteOptimizedByAI

ProductionOptimizedByAI

WarehouseOptimizedByAI

---

# Digital Twin Events

TwinUpdated

MachinePositionUpdated

MaterialPositionUpdated

EnergyUpdated

SimulationStarted

SimulationCompleted

---

# IoT Events

SensorReadingReceived

TemperatureExceeded

HumidityExceeded

VibrationDetected

EnergyConsumptionRecorded

---

# Notification Events

EmailSent

SMSDelivered

PushNotificationSent

TeamsMessageSent

WebhookTriggered

---

# Event Severity

Info

Warning

Critical

Emergency

---

# Event Status

Pending

Queued

Processing

Completed

Failed

DeadLetter

Replayed

---

# Business Rules

Every business transaction shall generate at least one event.

Events are immutable.

Events are append-only.

Events shall never be physically deleted.

Every event shall have a Correlation ID.

Every event shall support replay.

Every event shall support versioning.

Events shall be processed asynchronously unless marked Critical.

---

# AI Support

Anomaly Detection

Event Correlation

Predictive Alerts

Root Cause Analysis

Failure Prediction

Automatic Workflow Triggering

Digital Twin Synchronization

Operational Intelligence

---

# Monitoring

Events Per Minute

Failed Events

Queue Length

Average Processing Time

Retry Count

Dead Letter Queue

Subscriber Performance

AI Processing Queue

---

# Integrations

Workflow

Audit Logs

Notifications

Analytics

AI

Digital Twin

IoT

Mobile

API Gateway

MES

ERP

---

# Future Extensions

Apache Kafka

RabbitMQ

Azure Service Bus

AWS EventBridge

Event Store DB

CQRS

Event Sourcing

Real-Time Streaming

Digital Thread

Industry 4.0 Event Hub
