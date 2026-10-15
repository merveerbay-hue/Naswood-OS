# ADR-002 — Tesis Bazlı Stok Sahipliği, Ortak Malzeme Kodu ve Karşılıklı Salt-Okunur Görünüm

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

İki fabrikanın stok işleyişi birbirinden bağımsızdır. Buna karşın aynı malzeme kodlarını kullanırlar ve birbirlerinin stoğunu görmeleri gerekir.

## Karar

1. **Her uygulama kendi stoğunun tek sahibidir.**
   - Bucak stoğu NOS'ta, Antalya stoğu Operations'ta tutulur.
   - Mal kabul, paket, lot, hareket, sayım, karantina ve etiket işlemleri her uygulamanın kendi içinde çalışır.
   - Bir uygulamanın kullanıcısı diğer tesisin stoğunu **değiştiremez**. Bu kural yetkiyle değil, mimariyle sağlanır: diğer tarafın yazma uç noktası hiç açılmaz.

2. **NOS stok modülü, Operations stok modülünün birebir karşılığıdır.**
   - Ekranlar, iş akışları, çıktılar (Excel/PDF) ve etiketler (iz etiketi, ZPL, etiket profilleri, yazdırma kuyruğu) Operations ile aynı davranır ve aynı görünür.
   - Referans uygulama Operations'tır. NOS'ta stok davranışı değişecekse önce Operations'taki karşılığı incelenir; fark bilinçli ise bu belgede kayıt altına alınır.

3. **Malzeme kodu iki uygulamada aynıdır.**
   - Kod şeması: HM / YM / MP / TW aileleri, Operations'taki `material_master_server_prefix` kuralı.
   - Kod sunucuda üretilir; istemci kod göndermez.
   - NOS'taki 882 seed kodu bu kurala %100 uyumludur (2026-10-14 kontrolü).

4. **Karşılıklı salt-okunur stok görünümü.**
   - Her uygulama, diğerinin okuyabileceği salt-okunur bir stok bakiyesi uç noktası açar: malzeme kodu, ad, tesis, depo, lokasyon, miktar, birim, paket sayısı, kalite durumu.
   - Okuma, uygulamalar arası servis kimliği ile yapılır; son kullanıcı oturumu karşı tarafa taşınmaz.
   - Karşı tesisin stoğu ekranda "salt okunur" olarak işaretlenir; işlem butonları görünmez.

5. **Fabrikalar arası sevkiyat iki ayrı kayıttır.** Bucak'tan Antalya'ya giden mal, NOS'ta çıkış, Operations'ta mal kabul olarak işlenir. İki kayıt sevkiyat belge numarasıyla eşleşir. Hiçbir uygulama karşı tarafta stok oluşturmaz.

6. **Ortak kimlik sözleşmesi (etiket, QR, lot).** Bir tesiste basılan etiket diğer tesiste okutulduğunda tanınır ve izlenebilirlik kesilmez.
   - Kimlik formatı: `<TİP>-<TESİS>-<SIRA>`. Tesis kodları: `ANT` (Antalya), `BUC` (Bucak). Sıra en az 6 hanedir ve tesis içinde artar.
     - Paket: `PKG-BUC-000001` · Lot: `LOT-BUC-000001` · Lamine birimi: `LAM-BUC-000001`
     - Aynı kural Operations'taki diğer kimliklere de uygulanır: transfer `TRF-BUC-…`, kesim artığı `REST-BUC-…`, otomatik lamine etiketi `AUTO-BUC-…`.
   - Operations'ın bugünkü kodları (2026-10-14 kontrolü): `PKG-000245`, `LOT-000123`, `LAM-000001`, `AUTO-000001`, `TRF-000001`, `REST-000001` — tesis ön eksiz, 6 haneli veritabanı sırası. `NW-PKG-…`, `NWPKG-…`, `LOT-OPEN-…`, `LOT-PR-…` yalnız NOS'ta vardır; Operations'ta kullanılmaz.
   - Operations'ta bugüne kadar basılmış ön eksiz kodlar (`PKG-000245`, `LOT-000123`) geçerli kalır ve Antalya'ya ait sayılır; yeniden basılmaz.
   - QR ve Code128 yalnızca bu kodu taşır. URL, GUID veya tesis sunucusu adresi taşımaz. Kod değişmez; etiket yeniden basılsa da aynı kalır.
   - Okutucu kuralı (iki uygulamada aynı): `^(PKG|LOT|LAM|AUTO|TRF|REST)-(?:(ANT|BUC)-)?\d{6,}$`. Ön ek yoksa `ANT` kabul edilir.
   - Çözümleme: okutulan kodun tesisi kendi tesisiyse yerel kayıt açılır; değilse karşı uygulamanın salt-okunur kimlik uç noktasından pasaport (malzeme, lot, ölçü, miktar, durum, tesis, hareket özeti) getirilir ve "salt okunur" gösterilir.
   - Fabrikalar arası sevkiyatta kimlik korunur: Bucak'tan gelen `PKG-BUC-…` Antalya'da aynı kodla kabul edilir, kaynak lot (`LOT-BUC-…`) soy kökü olarak saklanır. Kabul eden taraf yeni kod basmaz.

## Sonuçlar

- NOS'un .NET stok kodu korunur ve Operations davranışına hizalanır; ikinci bir kopya olarak değil, Bucak'ın asıl stok motoru olarak.
- İki stok kodu tabanı vardır. Ayrışmayı önlemek için her stok değişikliği PR'ında "Operations karşılığı" bölümü bulunur.
- Malzeme ana verisinin hangi uygulamada oluşturulacağı (tek kaynak mı, iki yönlü eşitleme mi) ayrı bir kararla netleşir (açık konu 1).

## Yol haritası

1. **Fark analizi:** tamamlandı — `ADR-002_Stock_Parity_Plan.md` (P0–P7 aşamaları).
2. **Birebir hizalama:** Fark listesi ekran ekran kapatılır (mal kabul → paket/lot → hareket → sayım → etiket/yazdırma → çıktılar).
3. **Salt-okunur görünüm:** NOS stok bakiyesi uç noktası + Operations'ta Bucak görünümü; Operations bakiye görünümü + NOS'ta Antalya görünümü.
4. **Ortak kimlik sözleşmesi:** NOS numaralandırması `PKG/LOT/LAM-BUC-…` formatına geçer; Operations okutucusu ön ekli kodu tanır ve `BUC` kodlarını NOS'tan çözer (Operations tarafı Cuma canlıya çıkışından sonra).
5. **Fabrikalar arası sevkiyat eşleştirmesi.**

## Açık konular

1. Malzeme ana verisi: kartlar tek yerde mi oluşturulur, yoksa iki uygulama aynı kural ile kendi kartını oluşturup eşitlenir mi?
2. Thermowood'lu YM/MP kodları (`YM-LM-TPIN-S`, `MP-TCP-…`) Operations kuralında şu an reddedilir; Bucak bunları üretecekse kural iki tarafta birlikte genişletilir.
