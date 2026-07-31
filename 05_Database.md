
# Naswood OS - Veritabanı Tasarımı

**Sürüm:** 1.0
**Durum:** Taslak

---

# 1. Amaç

Bu doküman, Naswood OS'un veritabanı mimarisini tanımlar.

Temel hedefler:

- Performanslı
- Ölçeklenebilir
- Denetlenebilir
- Gelecekte yeni modüller eklenebilecek yapı

Veritabanı PostgreSQL üzerinde geliştirilecektir.

---

# 2. Temel Kurallar

## Primary Key

Bütün tablolarda

```text
id UUID
```

kullanılır.

Auto Increment kullanılmaz.

---

## Soft Delete

Hiçbir veri fiziksel olarak silinmez.

Her tabloda;

```text
deleted_at
deleted_by
```

alanları bulunur.

---

## Audit

Her tabloda aşağıdaki alanlar bulunacaktır.

```text
created_at

created_by

updated_at

updated_by
```

---

## Timestamp

UTC kullanılacaktır.

---

## Para Birimi

Decimal(18,2)

---

## Ölçüler

m³

m²

adet

paket

kg

mt

hepsi ayrı alanlarda tutulacaktır.

---

# 3. Ana Modüller

MVP için tablolar

```text
users

roles

permissions

warehouses

locations

product_categories

products

product_images

barcodes

stock

stock_movements

stock_count

stock_count_items

suppliers

customers

attachments
```

---

# 4. Users

Amaç

Sisteme giriş yapan kullanıcılar.

Alanlar

```text
id

name

surname

email

phone

password_hash

role_id

is_active

last_login

created_at

updated_at
```

---

# 5. Roles

```text
id

name

description
```

Örnek

Admin

Depo

Satış

Üretim

Muhasebe

Yönetici

---

# 6. Warehouses

```text
id

code

name

description

type

is_active
```

Örnek

```text
RAW

THERMO

PANEL

PELLET

FINISHED
```

---

# 7. Locations

Lokasyon yapısı

```text
Warehouse

↓

Koridor

↓

Raf

↓

Kat

↓

Göz
```

Örnek

```text
THM-A-03-B-12
```

Alanlar

```text
id

warehouse_id

code

aisle

rack

level

bin

capacity

is_locked
```

---

# 8. Product Categories

```text
id

parent_id

name

code
```

Örnek

Masif

Thermowood

Tomruk

Pelet

---

# 9. Products

En önemli tablo.

```text
id

product_code

barcode

qr_code

name

category_id

species

quality

thickness

width

length

moisture

density

volume_m3

area_m2

unit

description

status
```

---

# 10. Product Images

```text
id

product_id

file_name

url

order_no
```

---

# 11. Barcodes

```text
id

product_id

barcode

barcode_type

is_default
```

---

# 12. Stock

Bu tabloda sadece güncel stok bulunur.

```text
id

product_id

warehouse_id

location_id

batch_no

quantity

reserved_quantity

available_quantity

last_movement
```

---

# 13. Stock Movements

En kritik tablo.

Asla silinmez.

```text
id

movement_type

product_id

warehouse_id

location_id

batch_no

quantity

reference_type

reference_id

description

created_by

created_at
```

---

## Hareket Türleri

```text
RECEIPT

ISSUE

TRANSFER

COUNT

ADJUSTMENT

RETURN

SCRAP
```

---

# 14. Stock Count

```text
id

warehouse_id

count_date

status

created_by
```

---

# 15. Stock Count Items

```text
id

stock_count_id

product_id

expected_quantity

counted_quantity

difference
```

---

# 16. Suppliers

```text
id

company_name

tax_number

phone

email

address
```

---

# 17. Customers

```text
id

company_name

tax_number

phone

email

address
```

---

# 18. Attachments

```text
id

module

record_id

file_name

url

mime_type
```

---

# 19. İlişkiler

Users

↓

Roles

---

Warehouses

↓

Locations

---

Categories

↓

Products

↓

Barcodes

↓

Stock

↓

Stock Movements

↓

Stock Count

---

# 20. İndeksler

İndeks oluşturulacak alanlar

```text
product_code

barcode

warehouse_id

location_id

batch_no

created_at

movement_type
```

---

# 21. Gelecek Tablolar

İlk sürümde oluşturulmayacak.

```text
production_orders

work_orders

machines

machine_logs

quality_controls

purchase_orders

sales_orders

shipments

crm

maintenance

employees

tasks

notifications

ai_logs
```

---

# 22. Veritabanı Kuralları

- UUID kullanılacak.
- Foreign Key zorunlu olacak.
- Cascade sadece gerekli yerlerde kullanılacak.
- Soft Delete uygulanacak.
- Trigger ile Audit Log tutulacak.
- Hiçbir stok manuel değiştirilmeyecek.
- Stok yalnızca Stock Movements üzerinden hesaplanacak.
- İş kuralları servis katmanında uygulanacak.
- SQL sorguları ORM dışında yazılmayacak (özel raporlar hariç).

---

# 23. Tasarım İlkeleri

- PostgreSQL
- Prisma ORM
- Repository Pattern
- Clean Architecture
- Domain Driven Design
- API First
- Event Ready
- Multi Warehouse
- Multi User
- Multi Company (gelecekte)
