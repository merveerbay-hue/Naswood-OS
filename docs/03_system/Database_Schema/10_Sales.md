# Database Schema — Sales

**Project:** Naswood OS
**Document:** Sales Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Sales module manages customer relationships, quotations, sales orders, pricing, deliveries and commercial transactions.

Sales is the primary source of production demand.

Every confirmed Sales Order may generate one or more Production Orders.

---

# Philosophy

Sales does not manufacture products.

Sales defines customer demand.

Production fulfills that demand.

Commercial information remains separated from Manufacturing data.

---

# Entity List

Customer

CustomerAddress

CustomerContact

SalesQuotation

SalesQuotationLine

SalesOrder

SalesOrderLine

PriceList

PriceRule

DeliverySchedule

---

# customer

Represents a commercial customer.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| company_name | VARCHAR(200) |
| tax_number | VARCHAR(50) |
| customer_type | VARCHAR(50) |
| country | VARCHAR(100) |
| currency | VARCHAR(10) |
| payment_term | VARCHAR(50) |
| active | BOOLEAN |

Customer Types

- Dealer
- Distributor
- Contractor
- Export
- Retail
- Internal

---

# customer_address

Customer addresses.

| Field | Type |
|--------|------|
| id | UUID |
| customer_id | UUID FK |
| address_type | VARCHAR(30) |
| address | TEXT |
| city | VARCHAR(100) |
| country | VARCHAR(100) |
| postal_code | VARCHAR(20) |

Address Types

- Billing
- Shipping
- Factory
- Office

---

# customer_contact

Customer contact persons.

| Field | Type |
|--------|------|
| id | UUID |
| customer_id | UUID FK |
| full_name | VARCHAR(150) |
| title | VARCHAR(100) |
| phone | VARCHAR(50) |
| email | VARCHAR(150) |
| active | BOOLEAN |

---

# sales_quotation

Commercial quotation.

| Field | Type |
|--------|------|
| id | UUID |
| quotation_number | VARCHAR(30) |
| customer_id | UUID FK |
| quotation_date | DATE |
| expiry_date | DATE |
| status | VARCHAR(30) |
| currency | VARCHAR(10) |
| total_amount | NUMERIC(18,2) |

Status

- Draft
- Sent
- Revised
- Approved
- Rejected
- Expired

---

# sales_quotation_line

Quotation items.

| Field | Type |
|--------|------|
| id | UUID |
| quotation_id | UUID FK |
| product_id | UUID FK |
| product_variant_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_price | NUMERIC(18,2) |
| discount | NUMERIC(18,2) |
| total_price | NUMERIC(18,2) |

---

# sales_order

Confirmed customer order.

| Field | Type |
|--------|------|
| id | UUID |
| sales_order_number | VARCHAR(30) |
| customer_id | UUID FK |
| quotation_id | UUID FK |
| order_date | DATE |
| requested_delivery_date | DATE |
| status | VARCHAR(30) |
| currency | VARCHAR(10) |

Status

- Draft
- Confirmed
- Planning
- In Production
- Ready
- Partially Shipped
- Completed
- Cancelled

---

# sales_order_line

Sales Order items.

| Field | Type |
|--------|------|
| id | UUID |
| sales_order_id | UUID FK |
| product_id | UUID FK |
| product_variant_id | UUID FK |
| quantity | NUMERIC(18,3) |
| unit_price | NUMERIC(18,2) |
| requested_delivery_date | DATE |
| production_strategy_id | UUID FK |

---

# price_list

Commercial price lists.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| name | VARCHAR(150) |
| currency | VARCHAR(10) |
| valid_from | DATE |
| valid_to | DATE |

Examples

- Domestic Retail
- Dealer
- Export USD
- Export EUR
- Campaign

---

# price_rule

Pricing rules.

| Field | Type |
|--------|------|
| id | UUID |
| price_list_id | UUID FK |
| product_id | UUID FK |
| customer_type | VARCHAR(50) |
| minimum_quantity | NUMERIC |
| unit_price | NUMERIC(18,2) |
| discount_percentage | NUMERIC(5,2) |

---

# delivery_schedule

Delivery planning.

| Field | Type |
|--------|------|
| id | UUID |
| sales_order_line_id | UUID FK |
| planned_delivery_date | DATE |
| planned_quantity | NUMERIC(18,3) |
| shipment_id | UUID FK |

---

# Relationships

Customer

1 → N Addresses

Customer

1 → N Contacts

Customer

1 → N Quotations

Customer

1 → N Sales Orders

Quotation

1 → N Quotation Lines

Quotation

1 → 0..1 Sales Order

Sales Order

1 → N Sales Order Lines

Sales Order Line

1 → N Delivery Schedules

Price List

1 → N Price Rules

---

# Business Rules

### BR-1001

Every Sales Order shall reference exactly one Customer.

---

### BR-1002

A Sales Order may originate from a Quotation.

---

### BR-1003

Every Sales Order Line references a Product, not a Material.

---

### BR-1004

Sales Orders generate Production Demand.

---

### BR-1005

Production Planning determines whether demand is fulfilled from inventory or manufacturing according to the assigned Production Strategy.

---

### BR-1006

One Sales Order may generate multiple Production Orders.

---

### BR-1007

Partial deliveries are supported.

---

### BR-1008

Commercial prices shall never be stored in Material records.

---

### BR-1009

Price Lists are version-controlled.

Historical prices remain unchanged.

---

### BR-1010

Order status changes shall generate Business Events and Audit Logs.

---

# Integration

Sales integrates with:

- Product
- Production
- Inventory
- Shipment
- Pricing
- CRM
- Finance
- Analytics
- AI Sales Assistant

---

# Future Extensions

The architecture supports:

- Dealer Portal
- Customer Portal
- CRM
- Opportunity Management
- Project Sales
- BIM Integration
- Dynamic Pricing
- AI Quotation Assistant
- E-Commerce
- Multi-Company Sales

---

# Sales Philosophy

Sales defines customer demand.

Products are sold.

Materials are manufactured.

Production fulfills Sales Orders while preserving complete traceability from customer order to delivered package.

