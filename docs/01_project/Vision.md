# Naswood OS Vision
**Version:** 1.0  
**Status:** Draft  
**Last Updated:** 2026-08-01

---

# 1. Giriş

## 1.1 Projenin Adı

**Naswood OS**

## 1.2 Projenin Tanımı

Naswood OS; Naswood firmasının tüm operasyonlarını tek bir dijital platform üzerinden yönetebilmesini sağlayacak modüler, ölçeklenebilir ve yapay zekâ destekli bir işletim sistemidir.

Bu sistem yalnızca bir stok programı veya ERP değildir.

Amaç;

- fabrikanın dijitalleşmesi,
- süreçlerin standartlaşması,
- verilerin tek merkezde toplanması,
- manuel iş yükünün azaltılması,
- gerçek zamanlı karar alınabilmesi,
- gelecekte AI destekli yönetim altyapısının oluşturulmasıdır.

---

# 2. Vizyon

Naswood OS'un vizyonu;

Türkiye'nin en modern ahşap üretim yönetim sistemlerinden biri olmak ve ilerleyen yıllarda farklı fabrikalarda da kullanılabilecek ölçeklenebilir bir platform haline gelmektir.

Uzun vadede sistem;

- ERP
- MES
- WMS
- CRM
- BI
- AI Assistant
- IoT

modüllerini tek platform altında birleştirecektir.

---

# 3. Misyon

Naswood'un tüm operasyonlarını;

- daha hızlı
- daha güvenilir
- ölçülebilir
- sürdürülebilir

hale getirmek.

---

# 4. Temel Amaçlar

İlk fazın amacı;

- Depoları dijitalleştirmek
- Ürün kartlarını oluşturmak
- Barkod sistemi kurmak
- QR kod sistemi kurmak
- Lokasyon yönetimi oluşturmak
- Gerçek zamanlı stok takibi yapmak
- Telefon üzerinden stok sayımı yapmak

Bu ilk sürüm gerçek fabrikada aktif kullanılabilir seviyede olacaktır.

---

# 5. Uzun Vadeli Hedef

Naswood OS zaman içerisinde aşağıdaki modüllere sahip olacaktır.

## Yönetim

- Dashboard
- KPI
- Raporlama
- Yönetici Paneli

## Satış

- CRM
- Müşteri Yönetimi
- Teklif
- Sipariş
- Bayi Yönetimi

## Satın Alma

- Tedarikçiler
- Teklif Toplama
- Sipariş
- Mal Kabul

## Depo

- Lokasyon
- Barkod
- QR
- Transfer
- Sayım

## Üretim

- İş Emirleri
- Üretim Planlama
- Operasyon Yönetimi
- Makine Takibi
- OEE

## Kalite

- Giriş Kontrol
- Süreç Kontrol
- Nihai Kontrol
- Fire Yönetimi

## Bakım

- Periyodik Bakım
- Arıza Takibi
- Bakım Planı

## İnsan Kaynakları

- Personel
- Yetki
- Eğitim
- Performans

## Finans

- Cari
- Tahsilat
- Ödeme
- Muhasebe Entegrasyonu

## AI

- AI Asistan
- Tahmini Stok
- Tahmini Satış
- Tahmini Üretim
- AI Raporlama

---

# 6. Temel Tasarım İlkeleri

Naswood OS aşağıdaki prensiplere göre geliştirilecektir.

## Modüler

Her modül bağımsız geliştirilebilir olmalıdır.

Hiçbir modül diğerine gereksiz bağımlı olmayacaktır.

---

## Ölçeklenebilir

100 ürün ile de,

1 milyon ürün ile de

aynı performansı gösterebilmelidir.

---

## API First

Her işlem API üzerinden gerçekleştirilecektir.

Mobil uygulamalar aynı API'yi kullanacaktır.

---

## Cloud Ready

Sistem;

Docker

Kubernetes

Cloud Sunucular

üzerinde çalışabilecek şekilde geliştirilecektir.

---

## Mobile First

Telefon üzerinden;

- Stok Sayımı
- Barkod
- QR
- Ürün Sorgulama
- Transfer

işlemleri yapılabilecektir.

---

## Offline Ready

İnternet kesildiğinde;

stok sayımı yapılabilecek,

bağlantı geldiğinde senkronizasyon sağlanacaktır.

---

# 7. İlk Faz Kapsamı (MVP)

İlk sürüm yalnızca aşağıdaki modülleri içerecektir.

## Kullanıcı Yönetimi

- Giriş
- Yetki
- Roller

---

## Depolar

- Depo Tanımları
- Lokasyonlar

---

## Ürünler

- Ürün Kartları
- Barkod
- QR

---

## Stok

- Giriş
- Çıkış
- Transfer
- Sayım
- Hareketler

---

## Dashboard

- Toplam Ürün
- Toplam Stok
- Kritik Stoklar
- Son Hareketler

---

# 8. Proje Başarı Kriterleri

İlk sürüm sonunda;

✓ Tüm ürünler kayıtlı olacak.

✓ Tüm depolar oluşturulmuş olacak.

✓ Barkod sistemi çalışacak.

✓ Telefon ile sayım yapılabilecek.

✓ Stok farkları görülebilecek.

✓ Kritik stoklar izlenebilecek.

✓ Excel içe/dışa aktarma yapılabilecek.

---

# 9. Gelecek Fazlar

## Faz 2

Üretim Hareketleri

İş Emirleri

Fire Takibi

Makine Takibi

---

## Faz 3

Satın Alma

CRM

Teklif

Sipariş

---

## Faz 4

AI

n8n

IoT

PLC

Makine Veri Toplama

Tahmini Bakım

AI Destekli Planlama

---

# 10. Teknoloji Vizyonu

Frontend

- Next.js
- TypeScript
- Tailwind CSS
- shadcn/ui

Backend

- NestJS
- Prisma ORM

Veritabanı

- PostgreSQL

Kimlik Doğrulama

- JWT

Cache

- Redis

Dosya Depolama

- MinIO

Container

- Docker

Reverse Proxy

- Nginx

---

# 11. Sonuç

Naswood OS yalnızca mevcut ihtiyaçları karşılamak için değil, önümüzdeki 10 yıl boyunca büyüyebilecek bir dijital altyapı oluşturmak amacıyla geliştirilmektedir.

Bu proje; süreçleri dijitalleştiren, veriyi merkezileştiren, yapay zekâ entegrasyonuna hazır, modüler ve sürdürülebilir bir platform olacaktır.

Her yeni modül mevcut mimariyi bozmadan sisteme eklenebilecek şekilde tasarlanacaktır.
