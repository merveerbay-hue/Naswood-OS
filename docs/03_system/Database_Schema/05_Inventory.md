# Database Schema — Inventory

**Project:** Naswood OS
**Document:** Inventory Schema
**Database:** PostgreSQL
**Version:** 2.0
**Status:** Approved

---

# Purpose

The Inventory module manages the physical location, availability, reservation, packaging and shipment of Materials throughout the factory.

Naswood OS does not manage anonymous stock quantities.

Inventory is always derived from uniquely identifiable Materials.

Every inventory operation is fully traceable.

---

# Philosophy

Inventory never owns Materials.

Materials own Inventory.

Inventory represents:

- Physical Location
- Availability
- Reservation
- Packaging
- Shipment
- Storage History

Every Material always exists in exactly one physical location.

Material genealogy is never affected by inventory operations.

---

# Entity List

Warehouse

WarehouseLocation

InventoryMovement

InventoryReservation

InventoryHold

MaterialTransfer

InventoryAdjustment

CycleCount

InventorySnapshot

OpeningInventory

Package

PackageItem

Shipment

ShipmentItem

---

# warehouse

Represents a physical storage area.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(20) |
| name | VARCHAR(150) |
| warehouse_type | VARCHAR(50) |
| factory_id | UUID FK |
| active | BOOLEAN |

Warehouse Types

- Raw Material Yard
- Log Yard
- Kiln Buffer
- Thermowood Buffer
- Production Buffer
- Panel Warehouse
- Finished Goods Warehouse
- Pellet Warehouse
- Waste Area
- Shipping Area

---

# warehouse_location

Represents an exact storage position.

| Field | Type |
|--------|------|
| id | UUID |
| warehouse_id | UUID FK |
| code | VARCHAR(30) |
| location_type | VARCHAR(50) |
| zone | VARCHAR(50) |
| capacity | NUMERIC |
| capacity_unit | VARCHAR(20) |
| active | BOOLEAN |

Location Types

- Rack
- Floor
- Outdoor
- Machine Buffer
- Kiln Chamber
- Thermowood Chamber
- Press Buffer
- Loading Area

---

# inventory_movement

Represents every physical movement or inventory status change.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| movement_type | VARCHAR(50) |
| from_location_id | UUID FK |
| to_location_id | UUID FK |
| transformation_id | UUID FK |
| work_order_id | UUID FK |
| quantity | NUMERIC(18,3) |
| movement_time | TIMESTAMP |
| performed_by | UUID FK |

Movement Types

- Receiving
- Opening Inventory
- Warehouse Transfer
- Location Transfer
- Production Consumption
- Production Output
- Packaging
- Unpackaging
- Shipment
- Customer Return
- Supplier Return
- Recovery
- Scrap
- Inventory Adjustment
- Cycle Count Correction

---

# inventory_reservation

Reserves Materials for production or shipment.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| production_order_id | UUID FK |
| shipment_id | UUID FK |
| reserved_quantity | NUMERIC(18,3) |
| reserved_by | UUID FK |
| reserved_at | TIMESTAMP |
| expires_at | TIMESTAMP |

---

# inventory_hold

Temporarily blocks a Material from use.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| hold_type | VARCHAR(50) |
| reason | TEXT |
| start_time | TIMESTAMP |
| end_time | TIMESTAMP |
| released_by | UUID FK |

Hold Types

- Quality Hold
- Engineering Hold
- Customer Hold
- Moisture Stabilization
- Thermowood Cooling
- Glue Curing
- Quarantine

---

# material_transfer

Represents warehouse-to-warehouse transfers.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| from_warehouse_id | UUID FK |
| to_warehouse_id | UUID FK |
| transfer_reason | VARCHAR(100) |
| transferred_by | UUID FK |
| transferred_at | TIMESTAMP |

---

# inventory_adjustment

Manual inventory correction.

Requires authorization.

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| adjustment_reason | VARCHAR(200) |
| previous_quantity | NUMERIC |
| adjusted_quantity | NUMERIC |
| approved_by | UUID FK |
| adjusted_at | TIMESTAMP |

---

# cycle_count

Represents physical inventory counting.

| Field | Type |
|--------|------|
| id | UUID |
| warehouse_location_id | UUID FK |
| counted_by | UUID FK |
| count_date | DATE |
| status | VARCHAR(20) |

---

# inventory_snapshot

Historical inventory snapshot.

Generated automatically.

| Field | Type |
|--------|------|
| id | UUID |
| snapshot_date | TIMESTAMP |
| warehouse_id | UUID FK |
| warehouse_location_id | UUID FK |
| material_id | UUID FK |
| quantity | NUMERIC |

---

# opening_inventory

Registers materials already existing before Naswood OS implementation.

Historical genealogy before registration is not required.

Genealogy origin is recorded as:

Opening Inventory

| Field | Type |
|--------|------|
| id | UUID |
| material_id | UUID FK |
| warehouse_id | UUID FK |
| warehouse_location_id | UUID FK |
| registration_source | VARCHAR(50) |
| registration_date | TIMESTAMP |
| quantity | NUMERIC(18,3) |
| unit_id | UUID FK |
| estimated_cost | NUMERIC(18,2) |
| notes | TEXT |
| registered_by | UUID FK |

Registration Sources

- Opening Inventory
- Legacy ERP
- Excel Import
- Manual Registration
- Physical Count

---

# package

Represents a physical package.

| Field | Type |
|--------|------|
| id | UUID |
| package_code | VARCHAR(40) |
| package_type | VARCHAR(50) |
| status | VARCHAR(20) |
| created_at | TIMESTAMP |

Package Types

- Bundle
- Pallet
- Crate
- Container

---

# package_item

Links Materials to Packages.

| Field | Type |
|--------|------|
| id | UUID |
| package_id | UUID FK |
| material_id | UUID FK |
| sequence | INTEGER |

---

# shipment

Represents customer shipments.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_number | VARCHAR(40) |
| customer_id | UUID FK |
| shipment_date | TIMESTAMP |
| carrier | VARCHAR(150) |
| vehicle_plate | VARCHAR(30) |
| status | VARCHAR(20) |

---

# shipment_item

Links Packages to Shipments.

| Field | Type |
|--------|------|
| id | UUID |
| shipment_id | UUID FK |
| package_id | UUID FK |

---

# Inventory Status

Each Material always has one current Inventory Status.

Available Statuses

- AVAILABLE
- RESERVED
- IN_PRODUCTION
- ON_HOLD
- QUALITY_HOLD
- BLOCKED
- PACKAGED
- SHIPPED
- RETURNED
- SCRAPPED
- RECOVERED

Status history is preserved through Inventory Movements.

---

# Relationships

Warehouse

1 → N Warehouse Locations

Warehouse

1 → N Opening Inventory

Warehouse Location

1 → N Inventory Movements

Warehouse Location

1 → N Cycle Counts

Material

1 → N Inventory Movements

Material

1 → N Reservations

Material

1 → N Holds

Material

1 → N Package Items

Material

1 → 0..1 Opening Inventory

Package

1 → N Package Items

Shipment

1 → N Shipment Items

---

# Inventory Calculation

Current Inventory is calculated dynamically.

Inventory is derived from:

- Material
- Current Warehouse
- Current Warehouse Location
- Current Inventory Status
- Active Reservations
- Active Holds

No standalone Stock table exists.

Inventory balances are never duplicated.

---

# Business Rules

### BR-501

Inventory does not own quantity.

Inventory represents the current location, status and availability of uniquely identified Materials.

---

### BR-502

Every Material shall always have exactly one current Inventory Status.

Status changes generate Inventory Movements.

---

### BR-503

Every Material shall always exist in exactly one physical location.

Location history shall be preserved.

---

### BR-504

Inventory operations shall never break Material genealogy.

---

### BR-505

Packages contain Materials, not anonymous quantities.

---

### BR-506

Manual inventory adjustments require authorization and Audit Logs.

---

### BR-507

Reservations prevent Materials from being consumed by other Production Orders until released.

---

### BR-508

Materials may be temporarily blocked using Inventory Holds.

Held Materials cannot participate in production or shipment until released.

---

### BR-509

Inventory Snapshots are read-only historical records.

---

### BR-510

Materials existing before the implementation of Naswood OS shall be registered using the Opening Inventory process.

These Materials receive a Material identity without requiring historical genealogy before the registration date.

Their genealogy origin shall be recorded as:

Opening Inventory

---

# Integration

Inventory integrates with:

- Receiving
- Materials
- Production
- Transformation
- Packaging
- Shipment
- Quality
- Maintenance
- Sales
- Purchasing
- Analytics

---

# Inventory Philosophy

Inventory is the digital representation of physical reality.

The system never manages anonymous stock.

It manages uniquely identifiable Materials.

Every movement, reservation, package, shipment and inventory hold contributes to the current inventory state.

Reliable inventory begins with reliable material traceability.
