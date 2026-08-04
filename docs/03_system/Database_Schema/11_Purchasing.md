# Database Schema — Purchasing

**Project:** Naswood OS
**Document:** Purchasing Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Purchasing module manages suppliers, purchase requests, purchase orders, receipts and procurement activities.

Purchasing supplies the manufacturing process with materials, services, tooling and spare parts.

Every purchased item becomes traceable from supplier to production.

---

# Philosophy

Purchasing satisfies manufacturing demand.

Materials enter the factory through controlled procurement.

Purchasing is integrated with Receiving, Quality, Inventory and Finance.

---

# Entity List

Supplier

SupplierAddress

SupplierContact

PurchaseRequest

PurchaseRequestLine

PurchaseOrder

PurchaseOrderLine

SupplierPrice

PurchaseReceipt

PurchaseReceiptLine

---

# supplier

Represents a supplier.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| company_name | VARCHAR(200) |
| supplier_type | VARCHAR(50) |
| tax_number | VARCHAR(50) |
| country | VARCHAR(100) |
| currency | VARCHAR(10) |
| payment_term | VARCHAR(50) |
| active | BOOLEAN |

Supplier Types

- Log Supplier
- Lumber Supplier
- Thermowood Supplier
- Panel Supplier
- Tool Supplier
- Spare Parts Supplier
- Service Provider
- Packaging Supplier
- Chemical Supplier
- Transport Company

---

# supplier_address

Supplier addresses.

| Field | Type |
|--------|------|
| id | UUID |
| supplier_id | UUID FK |
| address_type | VARCHAR(30) |
| address | TEXT |
| city | VARCHAR(100) |
| country | VARCHAR(100) |

Address Types

- Billing
- Shipping
- Office
- Factory

---

# supplier_contact

Supplier contacts.

| Field | Type |
|--------|------|
| id | UUID |
| supplier_id | UUID FK |
| full_name | VARCHAR(150) |
| title | VARCHAR(100) |
| phone | VARCHAR(50) |
| email | VARCHAR(150) |
| active | BOOLEAN |

---

# purchase_request

Internal purchasing demand.

| Field | Type |
|--------|------|
| id | UUID |
| request_number | VARCHAR(30) |
| requested_by | UUID FK |
| department_id | UUID FK |
| priority | VARCHAR(20) |
| request_date | DATE |
| status | VARCHAR(30) |

Status

- Draft
- Submitted
- Approved
- Rejected
- Ordered
- Closed

Priority

- Low
- Normal
- High
- Critical

---

# purchase_request_line

Requested items.

| Field | Type |
|--------|------|
| id | UUID |
| purchase_request_id | UUID FK |
| item_type | VARCHAR(30) |
| material_id | UUID FK |
| product_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_id | UUID FK |
| required_date | DATE |
| notes | TEXT |

Item Types

- Material
- Tool
- Spare Part
- Service
- Consumable

---

# purchase_order

Official purchase order.

| Field | Type |
|--------|------|
| id | UUID |
| purchase_order_number | VARCHAR(30) |
| supplier_id | UUID FK |
| purchase_request_id | UUID FK |
| order_date | DATE |
| expected_delivery_date | DATE |
| currency | VARCHAR(10) |
| status | VARCHAR(30) |

Status

- Draft
- Sent
- Confirmed
- Partially Received
- Completed
- Cancelled

---

# purchase_order_line

Purchase order items.

| Field | Type |
|--------|------|
| id | UUID |
| purchase_order_id | UUID FK |
| item_type | VARCHAR(30) |
| material_type_id | UUID FK |
| product_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_price | NUMERIC(18,2) |
| discount_percentage | NUMERIC(5,2) |
| tax_rate | NUMERIC(5,2) |

---

# supplier_price

Supplier pricing history.

| Field | Type |
|--------|------|
| id | UUID |
| supplier_id | UUID FK |
| material_type_id | UUID FK |
| unit_price | NUMERIC(18,2) |
| currency | VARCHAR(10) |
| valid_from | DATE |
| valid_to | DATE |

---

# purchase_receipt

Receipt confirmation.

| Field | Type |
|--------|------|
| id | UUID |
| receipt_number | VARCHAR(30) |
| purchase_order_id | UUID FK |
| receiving_lot_id | UUID FK |
| received_by | UUID FK |
| receipt_date | TIMESTAMP |

---

# purchase_receipt_line

Received items.

| Field | Type |
|--------|------|
| id | UUID |
| purchase_receipt_id | UUID FK |
| purchase_order_line_id | UUID FK |
| received_quantity | NUMERIC(18,3) |
| accepted_quantity | NUMERIC(18,3) |
| rejected_quantity | NUMERIC(18,3) |

---

# Relationships

Supplier

1 → N Addresses

Supplier

1 → N Contacts

Supplier

1 → N Purchase Orders

Supplier

1 → N Supplier Prices

Purchase Request

1 → N Purchase Request Lines

Purchase Request

1 → N Purchase Orders

Purchase Order

1 → N Purchase Order Lines

Purchase Order

1 → N Purchase Receipts

Purchase Receipt

1 → N Purchase Receipt Lines

Purchase Receipt

1 → 1 Receiving Lot

---

# Business Rules

### BR-1101

Every Purchase Order shall reference exactly one Supplier.

---

### BR-1102

Received Materials shall generate a Receiving Lot.

---

### BR-1103

Incoming Materials requiring inspection shall remain under Quality Hold until released.

---

### BR-1104

Purchase Receipts shall generate Inventory Movements.

---

### BR-1105

Supplier prices are version-controlled.

Historical prices remain unchanged.

---

### BR-1106

Partial deliveries are supported.

---

### BR-1107

Rejected quantities shall create Quality records and supplier performance statistics.

---

### BR-1108

Purchase Orders may include Materials, Services, Tools and Spare Parts.

---

### BR-1109

Every Purchase Receipt shall generate Business Events and Audit Logs.

---

### BR-1110

Only approved Purchase Requests may be converted into Purchase Orders.

---

# Integration

Purchasing integrates with:

- Materials
- Receiving
- Inventory
- Quality
- Tooling
- Maintenance
- Production
- Finance
- Analytics
- Supplier Performance

---

# Future Extensions

The architecture supports:

- Supplier Portal
- RFQ (Request for Quotation)
- Multi-Supplier Comparison
- Supplier Scorecards
- AI Purchasing Assistant
- Automatic Reorder Suggestions
- Contract Management
- Import / Export Documentation
- EDI Integration

---

# Purchasing Philosophy

Purchasing is the starting point of the manufacturing supply chain.

Every purchased item enters the factory through a controlled receiving process, receives a digital identity where applicable and becomes fully traceable throughout its lifecycle.
