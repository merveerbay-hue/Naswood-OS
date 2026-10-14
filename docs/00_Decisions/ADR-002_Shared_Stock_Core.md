# ADR-002 — Ortak Stok Çekirdeği (Operations) ve NOS Kapsamı

**Durum:** Kabul edildi
**Tarih:** 2026-10-14
**İlgili:** ADR-001 (teknoloji yığını)

## Bağlam

Naswood'da iki uygulama vardır:

| | Naswood Operations | Naswood OS (NOS) |
|---|---|---|
| Fabrika | Antalya | Bucak |
| Mantık | Proje bazlı (şantiye, iş, maliyet) | Sürekli üretim (Thermowood, lamine, FJ, masif panel) |
| Teknoloji | React + Supabase (PostgreSQL RPC) | React + ASP.NET Core + PostgreSQL |
| Repo | `merveerbay-hue/managment` | `merveerbay-hue/naswood-os` |

İki uygulama aynı malzeme kodlarını ve aynı stok çekirdeğini kullanmalıdır. Stok ekranları, çıktılar ve etiket yazdırma iki fabrikada birebir aynı olmalıdır.

Operations'ın stok modülü bu ihtiyacı bugün karşılar:

- Çok tesisli stok: `inventory_plants` (Antalya, Bucak), tesis başına depo ve lokasyon.
- Tesis bazlı yetki: kullanıcının ev tesisi, tesis başına mal kabul / yönetim izni.
- Stok iş kuralları sunucuda (Supabase RPC): mal kabul, paket, lot, hareket, sayım, bakiye, hareket geçmişi.
- Etiket ve yazdırma: iz etiketleri, ZPL, etiket profilleri, yazdırma kuyruğu.
- Fabrikalar arası akış: Bucak lamine üretimi → Antalya kabul → CNC.
- Malzeme ana verisi ve kod üretimi sunucuda (`master_materials`, `material_master_create_canonical_v2`); istemcinin kod göndermesi yasaktır.

## Karar

1. **Stok çekirdeğinin tek sahibi Operations'tır.** Bucak stoğu Operations'ın stok motorunda, `Bucak` tesisi altında tutulur.
2. **Malzeme ana verisinin ve kod üretiminin tek sahibi Operations'tır.** NOS malzeme kodu üretmez; Operations'ın kanonik kod servisini çağırır.
3. **NOS ikinci bir stok motoru geliştirmez.** NOS'un kapsamı Bucak üretimidir (MES): iş emri, parti, Thermowood fırın süreci, FJ, lamine, masif panel, kalite, atölye ekranları. Stok etkisi olan her üretim olayı (tüketim, üretim çıkışı, paket oluşturma, karantina/serbest bırakma) Operations stok çekirdeğine yazılır.
4. **İnternet kesintisi kabul edilebilir.** Bucak'ta stok işlemleri internete bağlıdır; çevrimdışı kuyruk ilk sürümde yoktur.

## Sonuçlar

- NOS'taki mevcut .NET stok kodu (`Business` modülü, Inventory klasörü, `apps/web/src/modules/inventory`) **dondurulur.** Yeni özellik eklenmez; Aşama 2'de Operations stok çekirdeğine bağlanan ince bir katmana dönüştürülür veya kaldırılır.
- NOS'taki istemci tarafı kod üretimi (`materialCoding.ts`) kullanılmaz; kod üretimi Operations RPC'sine taşınır.
- ADR-001'deki "modül başına şema" ve "kendi JWT" kararları NOS'un üretim kapsamı için geçerlidir; stok ve malzeme ana verisi için geçerli değildir.
- Operations'ta yapılan her stok değişikliği iki fabrikayı birden etkiler. Bu nedenle stok çekirdeğindeki değişiklikler Operations reposunda yapılır ve Bucak etkisi PR'da belirtilir.

## Yol haritası

### Aşama 1 — Bucak, Operations stok modülünü kullanır (kod yok)
Operations'ın canlıya çıkışından (16.10.2026) sonra:
- Bucak depo ve lokasyonları Operations'ta tanımlanır.
- Bucak kullanıcıları için ev tesisi = Bucak, tesis bazlı yetkiler verilir.
- Bucak etiket yazıcısı ve etiket profilleri Operations'ta ayarlanır.
- NOS menüsündeki "Stok" bağlantısı Operations stok modülüne yönlenir.

### Aşama 2 — NOS üretimi Operations stok çekirdeğine bağlanır
- NOS, Operations Supabase'ine sunucu tarafında (ASP.NET Core) bağlanır; tarayıcıdan doğrudan değil.
- Üretim olayları Operations stok RPC'leri ile yazılır (`inventory_create_package`, `inventory_record_movement`, `record_production_laminate_output` vb.).
- Malzeme seçimi ve yeni malzeme kartı Operations'ın kanonik servisinden gelir.
- NOS'un kendi stok tabloları emekliye ayrılır; varsa Bucak verisi Operations'a taşınır.

### Aşama 3 — Ortak stok ekranları (gerekirse)
Stok ekranlarının NOS kabuğu içinde görünmesi gerekirse, Operations'ın stok ekranları ortak bir pakete çıkarılır ve iki uygulama aynı paketi kullanır. Ekran kodu kopyalanmaz.

## Açık konular

- Thermowood'lu YM/MP kodları (`YM-LM-TPIN-S`, `MP-TCP-…`) Operations kod kuralında şu an reddedilir. Bucak bunları üretecekse kural Operations'ta genişletilir.
- NOS'ta canlı Bucak verisi varsa (FJ pilotu), Aşama 2'de taşınma planı yapılır.
