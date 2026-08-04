# Logistics Database Schema

**Project:** Naswood OS

**Document:** Logistics Database Schema

**Version:** 2.0

**Status:** Approved

---

# Purpose

Defines all logistics-related database entities used throughout Naswood OS.

The Logistics module manages internal material movements, warehouse transfers, packaging, shipment planning, transportation, export operations and customer delivery.

---

# Logistics Architecture

Receiving

↓

Warehouse

↓

Internal Transfer

↓

Production

↓

Packaging

↓

Finished Goods

↓

Shipment

↓

Loading

↓

Transportation

↓

Customer

---

# Main Entities

Shipment

Shipment Item

Package

Package Item

Pallet

Pallet Item

Container

Container Item

Loading Unit

Loading Event

Vehicle

Carrier

Route

Delivery

Transfer Order

Transfer Item

Dock

Dock Appointment

Export Document

Delivery Confirmation

GPS Tracking

Logistics Event

---

# Table: shipments

Shipment_ID

Shipment_No

Customer_ID

Carrier_ID

Route_ID

Shipment_Type

Status

Planned_Date

Actual_Date

Warehouse_ID

Destination

Incoterms

Priority

Created_By

Created_Date

---

# Table: shipment_items

Shipment_Item_ID

Shipment_ID

Package_ID

Material_ID

Finished_Goods_ID

Quantity

Weight

Volume

Status

---

# Table: packages

Package_ID

Package_Code

Package_Type

Customer_ID

Product_ID

Package_Status

Gross_Weight

Net_Weight

Volume

Length

Width

Height

Package_Date

Warehouse_ID

QR_Code

Barcode

DPP_Link

---

# Table: package_items

Package_Item_ID

Package_ID

Finished_Goods_ID

Material_ID

Quantity

Weight

Volume

Sequence_No

---

# Table: pallets

Pallet_ID

Pallet_Code

Pallet_Type

Warehouse_ID

Gross_Weight

Net_Weight

Height

QR_Code

Status

---

# Table: pallet_items

Pallet_Item_ID

Pallet_ID

Package_ID

Sequence_No

---

# Table: containers

Container_ID

Container_Number

Container_Type

Seal_Number

Carrier_ID

Route_ID

Shipment_ID

Gross_Weight

Net_Weight

Loading_Date

Departure_Date

Arrival_Date

Destination

Status

QR_Code

---

# Table: container_items

Container_Item_ID

Container_ID

Pallet_ID

Package_ID

Sequence_No

---

# Table: loading_units

Loading_Unit_ID

Shipment_ID

Loading_Type

Vehicle_ID

Loading_Start

Loading_End

Operator_ID

Dock_ID

Status

---

# Table: loading_events

Loading_Event_ID

Loading_Unit_ID

Package_ID

Timestamp

Operator_ID

Action

Result

---

# Table: transfer_orders

Transfer_Order_ID

Transfer_No

Source_Warehouse

Destination_Warehouse

Status

Priority

Created_By

Created_Date

---

# Table: transfer_items

Transfer_Item_ID

Transfer_Order_ID

Material_ID

Package_ID

Quantity

Status

---

# Table: carriers

Carrier_ID

Carrier_Name

Carrier_Type

Contact

Country

Vehicle_Count

Insurance

Performance_Score

Status

---

# Table: vehicles

Vehicle_ID

Plate_Number

Vehicle_Type

Carrier_ID

Capacity

Driver

GPS_Device

Status

---

# Table: routes

Route_ID

Route_Name

Country

Distance

Estimated_Time

Risk_Level

Carbon_Factor

---

# Table: docks

Dock_ID

Dock_Name

Warehouse_ID

Dock_Type

Status

---

# Table: dock_appointments

Appointment_ID

Dock_ID

Shipment_ID

Vehicle_ID

Arrival_Time

Departure_Time

Status

---

# Table: export_documents

Export_Document_ID

Shipment_ID

Document_Type

Country

Document_No

Issue_Date

Status

File_Link

---

# Table: delivery_confirmations

Confirmation_ID

Shipment_ID

Delivery_Date

Receiver

Signature

GPS_Location

Photo

Status

---

# Table: gps_tracking

Tracking_ID

Shipment_ID

Latitude

Longitude

Speed

Timestamp

---

# Table: logistics_events

Event_ID

Entity_Type

Entity_ID

Event_Type

Timestamp

User_ID

Description

---

# Relationships

Shipment

↓

Packages

↓

Pallets

↓

Containers

↓

Delivery

---

Package

↓

Finished Goods

↓

Material Genealogy

↓

Production Order

---

Transfer Order

↓

Transfer Items

↓

Warehouse

↓

Inventory

---

# Shipment Lifecycle

Draft

↓

Planned

↓

Picking

↓

Packing

↓

Loaded

↓

In Transit

↓

Delivered

↓

Closed

---

# Package Lifecycle

Created

↓

Verified

↓

Stored

↓

Reserved

↓

Loaded

↓

Shipped

↓

Delivered

↓

Archived

---

# Container Lifecycle

Created

↓

Loading

↓

Sealed

↓

Dispatched

↓

In Transit

↓

Arrived

↓

Unloaded

↓

Closed

---

# Business Rules

Every Shipment shall have one Customer.

Every Package shall have one QR Code.

Every Package belongs to one Shipment.

Every Container belongs to one Shipment.

Every Transfer shall update Inventory automatically.

Every Loading Event shall be logged.

Export Shipments require Export Documents.

Digital Product Passport shall be linked to every export package.

---

# Indexes

Shipment_No

Package_Code

Pallet_Code

Container_Number

Transfer_No

QR_Code

Barcode

GPS Timestamp

---

# AI Support

Shipment Optimization

Container Optimization

Loading Optimization

Vehicle Recommendation

Carrier Recommendation

Route Optimization

Carbon Footprint Calculation

Delay Prediction

ETA Prediction

Logistics Risk Detection

Warehouse Traffic Optimization

Dock Scheduling Optimization

---

# Digital Twin Integration

Live Shipment Tracking

Warehouse Loading Visualization

Container Visualization

Dock Utilization

Forklift Movement

Package Heat Map

Vehicle Tracking

Real-Time Logistics Dashboard

---

# Related Modules

Packaging

Finished Goods

Warehouse

Inventory

Production

Customers

Transportation

Analytics

Printing

Barcode & QR

Digital Product Passport

AI

---

# Future Extensions

Autonomous Vehicles

AGV Integration

RFID Tracking

IoT Sensors

Smart Containers

Blockchain Logistics

GS1 EPCIS

EDI Integration

Drone Inventory

Carbon Accounting
