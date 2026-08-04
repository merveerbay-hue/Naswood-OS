# Data Dictionary

**Project:** Naswood OS  
**Document:** Data Dictionary  
**Version:** 1.0  
**Status:** Active Development

---

# 1. Purpose

This document defines every business entity and every business field used throughout Naswood OS.

It is the single source of truth for:

- Database Schema
- API Models
- Validation Rules
- User Interface
- AI Models
- Reporting

Every field shall be defined only once.

---

# 2. Field Definition Standard

Every field shall contain the following attributes.

| Property | Description |
|----------|-------------|
| Field Name | Technical field name |
| Data Type | PostgreSQL data type |
| Required | Yes / No |
| Nullable | Yes / No |
| Indexed | Yes / No |
| Default Value | Default value |
| Description | Business meaning |
| Example | Example value |
| Source Engine | Responsible Engine |

---

# 3. Common Fields

These fields exist on almost every entity.

| Field | Type | Required |
|------|------|----------|
| id | UUID | Yes |
| created_at | Timestamp | Yes |
| updated_at | Timestamp | Yes |
| created_by | UUID | Yes |
| updated_by | UUID | Yes |
| deleted_at | Timestamp | No |
| deleted_by | UUID | No |
| version | Integer | Yes |
| company_id | UUID | Yes |
| factory_id | UUID | Yes |

---

# 4. Material Entity

Represents every physical material inside the factory.

## Fields

| Field | Type | Required | Description |
|------|------|----------|-------------|
| id | UUID | Yes | Internal identifier |
| code | Varchar | Yes | Business Code |
| material_type_id | UUID | Yes | Material Type |
| species_id | UUID | Yes | Wood Species |
| quality_grade_id | UUID | Yes | Current Quality |
| receiving_lot_id | UUID | No | Source Receiving Lot |
| parent_material_id | UUID | No | Parent Material |
| warehouse_location_id | UUID | Yes | Current Location |
| status | Varchar | Yes | Material Status |
| moisture | Decimal | No | Moisture (%) |
| thickness | Decimal | No | mm |
| width | Decimal | No | mm |
| length | Decimal | No | mm |
| volume | Decimal | No | m³ |
| weight | Decimal | No | kg |

---

# 5. Receiving Lot

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| supplier_id | UUID |
| arrival_date | Timestamp |
| delivery_note | Varchar |
| truck_plate | Varchar |
| driver_name | Varchar |
| species_id | UUID |
| quantity | Decimal |
| volume | Decimal |

---

# 6. Material Transformation

Represents every material conversion.

| Field | Type |
|------|------|
| id | UUID |
| transformation_type | Enum |
| operation_id | UUID |
| parent_material_id | UUID |
| child_material_id | UUID |
| quantity | Decimal |
| waste_quantity | Decimal |
| recovery_quantity | Decimal |

Transformation Types

- Split
- Merge
- Conversion
- Recovery
- Scrap

---

# 7. Work Order

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| status | Enum |
| routing_id | UUID |
| recipe_id | UUID |
| planned_start | Timestamp |
| planned_finish | Timestamp |
| actual_start | Timestamp |
| actual_finish | Timestamp |

---

# 8. Operation

| Field | Type |
|------|------|
| id | UUID |
| work_order_id | UUID |
| operation_code | Varchar |
| machine_id | UUID |
| recipe_id | UUID |
| status | Enum |
| sequence | Integer |

---

# 9. Machine

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| machine_type | Varchar |
| manufacturer | Varchar |
| model | Varchar |
| serial_number | Varchar |
| installation_date | Date |
| status | Enum |

---

# 10. Tool

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| tool_type | Varchar |
| cutter_head_id | UUID |
| sharpening_count | Integer |
| remaining_life | Decimal |
| status | Enum |

---

# 11. Recipe

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| recipe_type | Enum |
| version | Integer |
| machine_id | UUID |
| description | Text |

---

# 12. Warehouse

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| warehouse_type | Enum |
| description | Text |

---

# 13. Warehouse Location

| Field | Type |
|------|------|
| id | UUID |
| warehouse_id | UUID |
| code | Varchar |
| aisle | Varchar |
| rack | Varchar |
| level | Integer |

---

# 14. Inventory Movement

| Field | Type |
|------|------|
| id | UUID |
| material_id | UUID |
| movement_type | Enum |
| source_location | UUID |
| destination_location | UUID |
| quantity | Decimal |
| timestamp | Timestamp |

---

# 15. Package

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| package_type | Enum |
| weight | Decimal |
| volume | Decimal |
| shipment_id | UUID |

---

# 16. Shipment

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| customer_id | UUID |
| vehicle | Varchar |
| shipment_date | Timestamp |
| delivery_status | Enum |

---

# 17. Quality Event

| Field | Type |
|------|------|
| id | UUID |
| material_id | UUID |
| inspector_id | UUID |
| quality_grade | UUID |
| inspection_type | Enum |
| result | Enum |
| notes | Text |

---

# 18. Defect

| Field | Type |
|------|------|
| id | UUID |
| code | Varchar |
| name | Varchar |
| severity | Integer |
| description | Text |

---

# 19. Event

| Field | Type |
|------|------|
| id | UUID |
| event_name | Varchar |
| entity_type | Varchar |
| entity_id | UUID |
| payload | JSONB |
| timestamp | Timestamp |

---

# 20. User

| Field | Type |
|------|------|
| id | UUID |
| username | Varchar |
| full_name | Varchar |
| email | Varchar |
| role_id | UUID |
| department | Varchar |
| status | Enum |

---

# 21. Future Entities

The following entities are reserved for future development.

- CLT Panel
- Glulam Beam
- CNC Program
- Digital Product Passport
- Carbon Footprint
- Energy Consumption
- AI Prediction
- Vision Inspection
- IoT Sensor
- Digital Twin
