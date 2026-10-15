# Stok Modülü — Operations Birebir Hizalama Planı

**Bağlı karar:** ADR-002 (Tesis bazlı stok sahipliği)
**Tarih:** 2026-10-14
**Referans:** Naswood Operations stok modülü (StockModule + 77 stok/malzeme/etiket migration'ı). NOS tarafı: `origin/main` 41a0ce8.

Bu belge iki uygulamanın stok modülünü alan alan karşılaştırır ve NOS'un Operations ile birebir davranması için yapılacak işleri sıralar. Her aşama ayrı bir dal ve PR'dır. Her PR'da "Operations karşılığı" bölümü bulunur.

## Ayna ilkesi

NOS stok modülü Operations'ın aynasıdır. Sayım formları (XLSX, PDF, elle yazılı form), mal kabul akışı, etiketler ve ekranlar Operations'taki ile aynı düzen, alan ve metinlerle kurulur. Tek fark tesis bilgisidir:

- Operations'ta Antalya'ya sabit olan yerler NOS'ta Bucak'a sabittir: form başlığı "NASWOOD — BUCAK STOK SAYIM FORMU", PDF dosya adı `NASWOOD_BUCAK_STOK_SAYIM_FORMU.pdf`, varsayılan tesis Bucak, depo kodları `BUC-…`.
- Operations'taki Antalya sabitleri doğrudur ve değiştirilmez.
- Operations'ta bir form veya akış değişirse NOS'taki karşılığı aynı değişikliği alır.

## Fark özeti

| # | Alan | Operations | NOS bugün | Fark |
|---|---|---|---|---|
| 1 | Ekran yapısı | 7 ana sekme + 3 Ana Veri sekmesi (Malzeme Kartları, Depolar/Lokasyonlar, Etiket/QR) | 31 ayrı rota, farklı menü | Menü ve ekran düzeni Operations sekmelerine göre yeniden kurulmalı |
| 2 | Kimlik / etiket kodu | `PKG-000245`, `LOT-000123`; QR = yalnızca kod | `NW-PKG-F01-26-000001`, barkod `NWPKG-…`, QR = URL+GUID; 3 farklı lot formatı | **Kritik.** İki taraf birbirinin etiketini tanımıyor. ADR-002 madde 6'ya geçilmeli |
| 3 | Etiket ve yazdırma | 3 ZPL şablonu (paket GC420T, lamine, saha parçası), 5 etiket profili, merkezi yazdırma kuyruğu, Windows Print Bridge, baskı geçmişi | Tek HTML etiket (100×70), QR dış servisten (`api.qrserver.com`), kuyruk/yazıcı yok | En büyük fark. Tamamı taşınmalı; QR yerelde üretilmeli |
| 4 | Mal kabul | 6 adımlı sihirbaz, 4 karar (Tam/Kısmi/Karantina/Red), 9 uyuşmazlık tipi, 3 paketleme modu, belge/fotoğraf yükleme, irsaliye tekilliği | Stoğa yazma çalışıyor; sihirbaz demo satırlarla, kanıtlar yalnızca anahtar, karantinadan çıkış yolu yok | Sihirbaz Operations akışına göre yeniden kurulmalı |
| 5 | Paket / lot | 2 paket türü, 11 durum, lot değişmez soy kökü, mal kabul başına tek lot | Böl/birleştir/yeniden paketle/taşı çalışıyor; barkod veritabanında tekil değil; numara "en büyük + 1" | Durumlar ve lot kuralları hizalanmalı, tekillik veritabanında zorlanmalı |
| 6 | Hareket / rezervasyon / transfer | 17 hareket tipi, 3 rezervasyon tipi, 5 adımlı transfer (Talep → Onay → Hazırlık → Yolda → Kabul), kısmi tüketimde 300 mm altı hurda | Rezervasyon yok (alan hiç dolmuyor); transfer tesis içi ve paketleri taşımıyor; çıkış ekranı sabit değerli | Rezervasyon ve transfer akışı kurulmalı |
| 7 | Sayım | 5 mod (Manuel, Excel, Barkod/QR, Fotoğraf, Açılış), elle yazılı form, sesli sayım, Excel 4 yerleşim, PDF form | Açılış/Periyodik/Kör sayım ve Excel çalışıyor; PDF yok; onaylayan ayrımı yok | Modlar ve PDF eklenmeli; **hata:** `NW-PKG` paketleri sayım satırında düşüyor |
| 8 | Çıktılar | Sayım formu XLSX + PDF | Ana stok Excel'i (3 sayfa), sayım şablonu | NOS'taki ana stok Excel'i korunur (Operations'ta yok) |
| 9 | Yetkiler | 7 stok yetkisi + tesis başına 6 yetki (MANAGE hepsini kapsar) | Rol bazlı; "Depo Sorumlusu" transfer yapamıyor; sayım onaylayıcısı ayrı değil | Yetki seti Operations'a eşlenmeli |
| 10 | Ana veri | Kod sunucuda üretilir; yoğunluk tablosu; malzeme silme korumalı | Kod tarayıcıda üretilir; yoğunluk backend'de yok; stoklu malzeme silinebilir | Kod üretimi sunucuya, yoğunluk tablosu eklenmeli |
| 11 | Karşı tesis görünümü | Yok | Yok | İki tarafta salt-okunur uç nokta + ekran |

Ortak temel kurallar (iki tarafta da geçerli olacak): bakiye yalnızca kesinleşmiş hareketlerden hesaplanır; her yazma tekrar güvenlidir (idempotent); kullanılabilir = fiziksel − rezerve, karantina/red/yolda paket kullanılamaz; m³ tek formülle (adet: K×G×U×adet/1e9); kaplama m², lamine/kereste m³.

## Aşamalar (her biri ayrı PR)

| Aşama | İş | Neden bu sırada |
|---|---|---|
| **P0** | Ortak kimlik sözleşmesi: `PKG/LOT/LAM-BUC-000001`, QR = yalnızca kod, okutucu kuralı, barkod tekilliği veritabanında, sayımdaki `NW-PKG` hatası | Yanlış formatla basılan her etiket sonradan sorun olur; her şeyden önce |
| **P1** | Etiket ve yazdırma: ZPL şablonları (GC420T), etiket profilleri, yazdırma kuyruğu, baskı geçmişi, yerel QR | Bucak'ta etiket basmadan stok işleyişi başlayamaz |
| **P2** | Mal kabul sihirbazı (6 adım, 4 karar, uyuşmazlık, paketleme modları, belge yükleme, karantinadan çıkış) | Stoğa giriş noktası |
| **P3** | Paket durumları, lot kuralları, rezervasyon, 5 adımlı transfer, çıkış ekranı | Üretim tüketimi rezervasyona dayanır |
| **P4** | Sayım modları, elle yazılı form, sesli sayım, PDF, ayrı onaylayıcı | Açılış sayımı canlıya geçişin ilk günü gerekir |
| **P5** | Ana veri: sunucu tarafı kod üretimi (Operations kuralı), yoğunluk, silme koruması, yetki seti | Kodların iki tarafta aynı kalması |
| **P6** | Ekran yapısı: menü ve sekmeler Operations düzenine | Görünüm birebirliği; P1–P5 ekranları bu kabuğa oturur |
| **P7** | Karşılıklı salt-okunur stok görünümü ve kimlik çözümleme uç noktası (NOS tarafı); Operations tarafı Cuma sonrası | Etiketin karşı tesiste tanınması |

## Açık konular

- Operations Windows Print Bridge'i Supabase kuyruğuna bağlıdır. NOS'un aynı köprüyü mü (iki kuyruğu dinleyen sürüm) yoksa kendi köprüsünü mü kullanacağı P1'de karar verilir.
- Operations'ta yerel kopyada bulunmayan migration'lar (248, 249, 251, `cross_plant_laminate_transfer`) ve bazı ekran dosyaları (Malzeme Kartları) ilgili aşamada ayrıca okunacak.
- Operations analizinde görülen olası hatalar Operations sohbetine iletilmeli: RED kararında miktar 0 iken taslak RPC'lerinin qty>0 istemesi; `inventory_split_package` rezerve miktarı kontrol etmemesi; tarayıcı baskısında geçmişin baskıdan önce yazılması.
