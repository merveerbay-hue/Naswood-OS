# Database Schema — Logistics

**Project:** Naswood OS
**Document:** Logistics Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Logistics module manages all internal and external material movements after production.

It covers warehouse transfers, loading operations, transportation, shipment planning and delivery confirmation.

Logistics ensures that every Material and Package reaches the correct destination while maintaining complete traceability.

---

# Philosophy

Production creates products.

Logistics moves products.

Every movement is traceable.

Every shipment is planned.

Every delivery is verifiable.

---

# Entity List

TransferOrder

TransferOrderLine

LoadingPlan

LoadingOperation

Vehicle

Carrier

ShipmentRoute

Delivery

DeliveryConfirmation

ExportDocument

---

# transfer_order

Represents an internal logistics request.

| Field | Type |
|--------|------|
| id | UUID |
| transfer_number | VARCHAR(30) |
| from_warehouse_id | UUID FK |
| to_warehouse_id | UUID FK |
| requested_by | UUID FK |
| status | VARCHAR(30) |
| planned_date | DATE |

Status

- Draft
- Planned
- Released
- In Progress
- Completed
- Cancelled

---

# transfer_order_line

Materials or Packages to be transferred.

| Field | Type |
|--------|------|
| id | UUID |
| transfer_order_id | UUID FK |
| material_id | UUID FK |
| package_id | UUID FK |
| quantity | NUMERIC(18,3) |

---

# loading_plan

Represents planned loading for one vehicle.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_id | UUID FK |
| vehicle_id | UUID FK |
| loading_date | TIMESTAMP |
| loading_status | VARCHAR(30) |

Loading Status

- Planned
- Loading
- Completed
- Cancelled

---

# loading_operation

Individual loading records.

| Field | Type |
|--------|------|
| id | UUID |
| loading_plan_id | UUID FK |
| package_id | UUID FK |
| loaded_by | UUID FK |
| loaded_at | TIMESTAMP |

---

# vehicle

Transportation vehicle.

| Field | Type |
|--------|------|
| id | UUID |
| plate_number | VARCHAR(30) |
| vehicle_type | VARCHAR(50) |
| capacity_weight | NUMERIC |
| capacity_volume | NUMERIC |
| active | BOOLEAN |

Vehicle Types

- Truck
- Trailer
- Container
- Forklift
- Internal Transport

---

# carrier

Transportation company.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| company_name | VARCHAR(200) |
| contact_person | VARCHAR(150) |
| phone | VARCHAR(50) |
| active | BOOLEAN |

---

# shipment_route

Shipment route information.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_id | UUID FK |
| departure_location | VARCHAR(150) |
| destination_location | VARCHAR(150) |
| estimated_distance_km | NUMERIC |
| estimated_duration_hours | NUMERIC |

---

# delivery

Represents product delivery.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_id | UUID FK |
| customer_id | UUID FK |
| delivery_date | TIMESTAMP |
| delivery_status | VARCHAR(30) |

Delivery Status

- Planned
- In Transit
- Delivered
- Delayed
- Returned

---

# delivery_confirmation

Proof of delivery.

| Field | Type |
|--------|------|
| id | UUID |
| delivery_id | UUID FK |
| confirmed_by | VARCHAR(150) |
| confirmation_date | TIMESTAMP |
| document_reference | VARCHAR(100) |
| remarks | TEXT |

---

# export_document

Export documentation.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_id | UUID FK |
| document_type | VARCHAR(50) |
| document_number | VARCHAR(100) |
| issue_date | DATE |

Examples

- Invoice
- Packing List
- Certificate of Origin
- EUR.1
- Bill of Lading
- CMR
- Customs Declaration

---

# Relationships

Transfer Order

1 → N Transfer Order Lines

Loading Plan

1 → N Loading Operations

Vehicle

1 → N Loading Plans

Carrier

1 → N Shipments

Shipment

1 → 1 Shipment Route

Shipment

1 → 1 Delivery

Delivery

1 → 1 Delivery Confirmation

Shipment

1 → N Export Documents

---

# Business Rules

### BR-1301

Every internal warehouse movement shall be initiated by a Transfer Order.

---

### BR-1302

Every Shipment shall have one Loading Plan.

---

### BR-1303

Only approved Packages may be loaded.

---

### BR-1304

Loading Operations shall generate Inventory Movements.

---

### BR-1305

Delivery confirmation shall complete the Shipment lifecycle.

---

### BR-1306

Returned deliveries shall generate reverse Inventory Movements.

---

### BR-1307

Export Shipments shall include all mandatory export documents.

---

### BR-1308

Vehicles shall not exceed configured weight or volume capacities.

---

### BR-1309

Every logistics transaction shall generate Business Events and Audit Logs.

---

### BR-1310

Package traceability shall remain intact throughout transportation.

---

# Integration

Logistics integrates with:

- Inventory
- Packaging
- Sales
- Production
- Warehouse
- Finance
- Analytics
- GPS (Future)
- Carrier Systems
- AI Route Optimization

---

# Future Extensions

The architecture supports:

- GPS Vehicle Tracking
- Live Shipment Tracking
- RFID Gate Control
- QR Code Loading Verification
- Dock Scheduling
- Route Optimization
- AI Load Optimization
- Freight Cost Optimization
- Customer Delivery Portal

---

# Logistics KPIs

The Logistics module shall support calculation of:

- On-Time Delivery
- Vehicle Utilization
- Warehouse Transfer Time
- Loading Time
- Delivery Accuracy
- Freight Cost per m³
- Freight Cost per Shipment
- Average Delivery Duration
- Loading Efficiency
- Container Utilization

---

# Logistics Philosophy

Logistics is responsible for the controlled movement of Materials and Products throughout the supply chain.

Every transfer, loading operation, shipment and delivery is digitally recorded, fully traceable and linked to the originating Materials, Packages and Sales Orders.

Reliable logistics completes the manufacturing lifecycle by ensuring the correct product reaches the correct customer at the correct time.
