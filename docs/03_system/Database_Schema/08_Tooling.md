# Database Schema — Tooling

**Project:** Naswood OS
**Document:** Tooling Schema
**Database:** PostgreSQL
**Version:** 1.0
**Status:** Approved

---

# Purpose

The Tooling module manages all production tooling used by manufacturing machines.

Tooling includes cutter heads, knives, saw blades, drill bits, router tools and all replaceable production components.

The module provides complete traceability of tooling configuration, tool life, sharpening history and production usage.

---

# Philosophy

Machines perform Operations.

Tools perform Cutting.

A Machine may execute thousands of Operations.

The quality of the produced Material depends on the Tool configuration.

Tooling is therefore considered a production asset.

---

# Entity List

Tool

ToolAssembly

ToolPosition

Knife

KnifeProfile

ToolConfiguration

ToolInstallation

ToolLife

Sharpening

ToolInspection

ToolInventory

---

# tool

Represents an individual production tool.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| tool_type | VARCHAR(50) |
| manufacturer | VARCHAR(100) |
| model | VARCHAR(100) |
| serial_number | VARCHAR(100) |
| purchase_date | DATE |
| status | VARCHAR(30) |

Tool Types

- Cutter Head
- Saw Blade
- Circular Saw
- Router Bit
- Drill
- Planer Knife
- Finger Joint Cutter

---

# tool_assembly

Represents a complete cutter head or tooling assembly.

Examples

- Weinig Head #12
- Leadermac Head A
- Finger Joint Head

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| machine_id | UUID FK |
| name | VARCHAR(150) |
| diameter | NUMERIC |
| width | NUMERIC |
| maximum_rpm | INTEGER |

---

# tool_position

Defines knife positions within an assembly.

| Field | Type |
|--------|------|
| id | UUID |
| tool_assembly_id | UUID FK |
| position_number | INTEGER |
| knife_id | UUID FK |
| rotation_angle | NUMERIC |

---

# knife

Represents an individual knife.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| knife_profile_id | UUID FK |
| material | VARCHAR(50) |
| hardness | NUMERIC |
| current_height | NUMERIC |
| minimum_height | NUMERIC |
| sharpening_count | INTEGER |
| status | VARCHAR(30) |

Knife Status

- New
- Installed
- In Stock
- Sent for Sharpening
- Ready
- Worn
- Scrap

---

# knife_profile

Defines cutting geometry.

| Field | Type |
|--------|------|
| id | UUID |
| code | VARCHAR(30) |
| profile_name | VARCHAR(150) |
| profile_family | VARCHAR(50) |
| drawing_number | VARCHAR(50) |
| version | INTEGER |

Examples

- Deck 26x92
- Cladding
- Lambri
- Radius
- Chamfer
- Custom Profile

---

# tool_configuration

Defines which tooling configuration is required for production.

| Field | Type |
|--------|------|
| id | UUID |
| routing_id | UUID FK |
| recipe_id | UUID FK |
| machine_id | UUID FK |
| tool_assembly_id | UUID FK |
| version | INTEGER |
| active | BOOLEAN |

---

# tool_installation

Tracks installation history.

| Field | Type |
|--------|------|
| id | UUID |
| machine_id | UUID FK |
| tool_assembly_id | UUID FK |
| installed_by | UUID FK |
| installed_at | TIMESTAMP |
| removed_at | TIMESTAMP |

---

# tool_life

Stores tool usage.

| Field | Type |
|--------|------|
| id | UUID |
| knife_id | UUID FK |
| runtime_minutes | INTEGER |
| processed_meters | NUMERIC |
| processed_cubic_meters | NUMERIC |
| processed_pieces | INTEGER |
| remaining_life_percentage | NUMERIC |

---

# sharpening

Stores sharpening history.

| Field | Type |
|--------|------|
| id | UUID |
| knife_id | UUID FK |
| sharpening_date | DATE |
| previous_height | NUMERIC |
| new_height | NUMERIC |
| removed_material | NUMERIC |
| sharpening_company | VARCHAR(150) |
| notes | TEXT |

---

# tool_inspection

Inspection after sharpening or before installation.

| Field | Type |
|--------|------|
| id | UUID |
| knife_id | UUID FK |
| inspection_date | DATE |
| inspector_id | UUID FK |
| result | VARCHAR(30) |
| remarks | TEXT |

Results

- Pass
- Fail
- Conditional

---

# tool_inventory

Current tooling inventory.

| Field | Type |
|--------|------|
| id | UUID |
| tool_id | UUID FK |
| warehouse_location_id | UUID FK |
| quantity | INTEGER |

---

# Relationships

Machine

1 → N Tool Assemblies

Tool Assembly

1 → N Tool Positions

Tool Position

1 → 1 Knife

Knife

1 → N Sharpening Records

Knife

1 → N Tool Life Records

Knife

1 → N Inspections

Knife Profile

1 → N Knives

Tool Assembly

1 → N Installations

Tool Configuration

1 → N Production Operations

---

# Business Rules

### BR-801

Every installed Tool Assembly shall be traceable.

---

### BR-802

Every Knife shall belong to exactly one Knife Profile.

---

### BR-803

Knife sharpening shall never overwrite historical data.

Every sharpening creates a new record.

---

### BR-804

Tool life shall accumulate automatically from Production.

---

### BR-805

Expired tools cannot be assigned to production.

---

### BR-806

Every profile shall reference a Tool Configuration.

---

### BR-807

Changing Tool Configuration requires authorization and generates an Audit Log.

---

### BR-808

Tool Configurations are version-controlled.

Historical configurations remain available.

---

### BR-809

Each Production Operation shall reference the Tool Configuration used during execution.

---

### BR-810

Sharpening history shall remain permanently traceable.

---

# Integration

Tooling integrates with:

- Machines
- Production
- Routing
- Recipes
- Maintenance
- Quality
- Inventory
- Audit Log
- AI Planning

---

# Future Extensions

The architecture supports:

- Automatic tool measurement
- RFID tool identification
- Presetter integration
- CNC tool management
- Tool balancing
- Predictive tool replacement
- AI tool optimization
- Digital profile library

---

# Tooling Philosophy

Tooling is not a consumable.

It is a managed production asset.

Every profile, every cutter head, every knife and every sharpening operation contributes directly to manufacturing quality.

Reliable tooling management enables repeatable production, lower waste, higher quality and complete manufacturing traceability.
