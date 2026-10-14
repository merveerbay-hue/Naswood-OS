# ADR-001 — Teknoloji Yığını

**Durum:** Kabul edildi  
**Tarih:** 2026-10-14  
**Kapsam:** Naswood OS, tek kişilik ekip, fabrika içi kurulum, yaklaşık 17 modül

## Bağlam

`docs/03_system/System_Architecture.md` sistemi malzeme merkezli, modüler, olay güdümlü ve API-first tanımlar. Her motor kendi verisinin sahibidir. Olaylar değişmezdir ve kalıcı saklanır. Kimlik JWT ve rol tabanlı yetkidir. Aynı belge mesaj kuyruğunu gelecek sürüme bırakır.

`docs/14_Implementation/Sprint_01_Inventory/TASK-017_Warehouse.md` depo ana verisini bu yığının ilk iş alanı yapar. Oradaki “.NET 9, MediatR, React” listesi bir görev istemidir. `docs/14_Implementation/README.md` bu klasörü FROZEN, tarihsel TASK arşivi olarak işaretler; teknoloji kararı bu ADR’dedir.

`docs/20_Frontend_Architecture/Overview.md` kabuğu modül, çalışma alanı ve ekran diye kurar. `docs/20_Frontend_Architecture/State_Data.md` sunucu durumunu sorgu kütüphanesine, form durumunu ekrana bırakır. Görünüm `docs/16_Design_System` ve `docs/18_Component_Library` ile hizalanır.

## 1. Mimari stil

**Karar:** Modüler monolit. Tek süreç, tek konuşlandırma, modül sınırları kod ve veritabanı şemasıyla zorunlu.

On yedi modülü ayrı servis yapmak, tek kişilik ekipte dağıtık işlem, sürüm ve işletim yükü üretir. Fabrika içi ilk kurulumda bu yükün karşılığı yoktur. Motorların veri sahipliği, süreç içinde şema, uygulama sınırı ve olayla korunur. Sınırı kanıtlanmış bir modül daha sonra ayrı sürece çıkarılabilir.

**İleride eklenebilir:** Seçilmiş bir modülün ayrı host’a alınması. İlk günden mikroservis yok.

## 2. Backend

**Karar:** .NET 10 LTS ve ASP.NET Core. ORM olarak EF Core. CQRS bir kütüphane değil, komut ve sorgu ayrımıdır. MediatR alınmaz; `BuildingBlocks` içinde ince bir dağıtıcı yeter. Doğrulama FluentValidation ile yapılır. HTTP API `/api/v1` altındadır.

.NET 8 ve .NET 9 Kasım 2026’da destek dışı kalır. Yeni kod, aynı ASP.NET Core modelinde, desteklenen LTS olan .NET 10 ile başlar. EF Core, modül şeması, göç ve bütünlük kuralları için yeterlidir. MediatR bu ayrımı kısaltır ama ek bir bağımlılık yüzeyi açar. İnce dağıtıcı, komut, sorgu ve doğrulama hattını bu çözümün içinde tutar.

**İleride eklenebilir:** Üçüncü parti bir boru hattı, ancak davranış gerçekten büyürse.

## 3. Veritabanı

**Karar:** Tek PostgreSQL veritabanı, modül başına şema. Örnek: `platform`, `masterdata`, `inventory`, `production`, `quality`, `finance`. Çapraz şema yabancı anahtar yasaktır. Tek istisna, `masterdata` şemasına referanslardır: malzeme, birim ve ana depo kimliği. Başka modülün kimliği düz değer olarak tutulur; ilişki olay veya API ile kurulur.

`System_Architecture.md` her motora kendi tablolarını verir. Ayrı veritabanı, tek kişilik ekipte yedek, göç ve işlem sınırı çoğaltır. Tek şema ise sahipliği isimlendirmeye bırakır. Malzeme, birim ve ana depo kimliği birçok modülün paylaştığı ana veridir; bunların tek sahibi `masterdata` şemasıdır ve yalnızca bu kimliklere yabancı anahtar serbesttir. Deponun iş tabloları `inventory` şemasındadır. Platform kimliği, denetim ve ayar `platform` şemasındadır.

**İleride eklenebilir:** Okuma yoğun bir modül için ayrı okuma modeli. Ayrı sunucu değil.

## 4. Frontend

**Karar:** `apps/web` içinde React, TypeScript ve Vite. Görünüm, `docs/16_Design_System` ve `docs/18_Component_Library` ile hizalı ince bir iç bileşen kütüphanesidir. Tailwind yalnızca token ve yerleşim içindir. Sunucu durumu TanStack Query’dedir. Form durumu ekrandadır. Genel durum deposu yoktur.

`docs/20_Frontend_Architecture` klasörü ekranı sunucunun otoritesine bağlar. React bu kabuğu karşılar. Sunucu verisinin ikinci bir istemci deposunda kopyalanması, yetki ve iş akışı kurallarını arayüze taşır. Sorgu kütüphanesi önbelleği ekran anahtarıyla tutar. Form gönderimi modül API komutuna gider.

**İleride eklenebilir:** Ayrı mobil istemci. İlk sürümde tek web kabuğu.

## 5. Kimlik doğrulama

**Karar:** Platform modülünde kendi JWT servisimiz. Kısa ömürlü erişim jetonu, PostgreSQL’de yenileme oturumu, rol ve tesis bazlı yetki. Parolalar ASP.NET Core’un parola hasher’ı ile saklanır. Keycloak ve OpenIddict ilk sürümde yoktur.

Mimari belge JWT ve rol tabanlı yetki ister; çok faktörlü doğrulamayı sonraya bırakır. Fabrika içi kullanıcılar için kendi kullanıcı deposu yeterlidir ve tek kurulumda ayrı bir kimlik sunucusu işletmeyi gerektirmez. OAuth, müşteri veya tedarikçi portalı dışarıdan giriş istediğinde anlam kazanır. O gün OpenIddict mevcut kullanıcı tablosunun önüne konabilir.

**İleride eklenebilir:** OpenIddict, çok faktörlü doğrulama, dış kimlik sağlayıcı.

## 6. Mesajlaşma ve önbellek

**Karar:** İlk sürümde RabbitMQ ve Redis yok. Modül, iş kaydıyla aynı veritabanı işleminde değişmez bir olay satırı yazar. Süreç içi bir dağıtıcı bu satırı işler ve `DispatchedAt` yazar. Satır silinmez.

Mimari, olayın kalıcı ve değişmez olmasını bugün ister; mesaj kuyruğunu gelecek sürüme erteler. Kuyruk, ikinci bir süreç veya dış sistem güvenilir tüketim istediğinde gerekir. Redis, jeton zaten taşınırken ve okuma yükü ölçülmemişken ek bir arıza noktasıdır. Kalıcı olay tablosu hem izi hem yeniden denemeyi PostgreSQL içinde tutar.

**İleride eklenebilir:** RabbitMQ, dış entegrasyon tüketicisi olarak. Redis, ölçülmüş bir okuma darboğazı için.

## 7. Solution yapısı

```text
naswood-os/
  Naswood.sln
  src/
    Hosts/
      Naswood.Api/                 # tek ASP.NET Core host, modülleri bağlar
    BuildingBlocks/
      Naswood.BuildingBlocks.Domain/
      Naswood.BuildingBlocks.Application/      # komut, sorgu, dağıtıcı
      Naswood.BuildingBlocks.Infrastructure/   # EF tabanı, outbox dağıtıcı, query filter
    Modules/
      Platform/
        Naswood.Modules.Platform.Domain/
        Naswood.Modules.Platform.Application/
        Naswood.Modules.Platform.Infrastructure/   # şema: platform
        Naswood.Modules.Platform.Presentation/
      MasterData/
        Naswood.Modules.MasterData.Domain/
        Naswood.Modules.MasterData.Application/
        Naswood.Modules.MasterData.Infrastructure/ # şema: masterdata
        Naswood.Modules.MasterData.Presentation/
      Inventory/
        Naswood.Modules.Inventory.Domain/
        Naswood.Modules.Inventory.Application/
        Naswood.Modules.Inventory.Infrastructure/  # şema: inventory
        Naswood.Modules.Inventory.Presentation/
      Finance/
        Naswood.Modules.Finance.Domain/
        Naswood.Modules.Finance.Application/
        Naswood.Modules.Finance.Infrastructure/    # şema: finance
        Naswood.Modules.Finance.Presentation/
      Production/        # aynı dört katman
      Quality/
      Maintenance/
      ...                # kalan modüller aynı kalıp
  apps/
    web/
      src/
        shell/
        navigation/
        modules/
          inventory/
          production/
        shared/
        locales/
          tr-TR.json
          en-US.json
  tests/
    Naswood.Modules.Inventory.Domain.Tests/
    Naswood.Modules.Inventory.IntegrationTests/
  deploy/
    docker-compose.yml
    Dockerfile.api
    Dockerfile.web
  docs/
    00_Decisions/
      ADR-001_Technology_Stack.md
```

Her modülde bağımlılık içe doğrudur. Presentation ve Infrastructure, Application’a bakar. Domain hiçbir altyapıya bakmaz. Host yalnızca modül kurulumunu çağırır.

## 8. Test, CI ve Docker

**Karar:** Alan kuralları birim testtir ve veritabanı açmaz. Her modülün bir ince entegrasyon testi, Testcontainers ile gerçek PostgreSQL’e karşı API’yi yürür. Sorgu filtresi testi, başka şirketin satırının listeye düşmediğini doğrular. CI tek iş akışıdır: derle, birim test, entegrasyon test. Docker Compose ilk günde üç servistir: `postgres`, `api`, `web`.

Tek kişide uygulanabilir test, depo kodunun tekilliği, “stok varken depo silinmez” gibi kurallar ve şirket filtresinin otomatik sınanmasıdır. Her ekran için uçtan uca tarayıcı testi ilk sürüme girmez. İmaj, fabrika içi kurulumu tekrarlanabilir kılar. Kuyruk ve önbellek imajı bu dosyada yer almaz.

**İleride eklenebilir:** Seçilmiş iş akışları için tarayıcı testi. Ayrı bir önizleme ortamı.

## 9. Veri temelleri

**Karar:** Her iş tablosunda `company_id` ve `site_id` vardır. EF Core global sorgu filtresi bu iki alanı oturumdaki şirket ve tesise kilitler. Birincil anahtar UUID v7’dir ve uygulama üretir; kolon tipi `uuid`dir. Miktar `numeric(18,6)`, para `numeric(18,4)`, oran `numeric(9,6)` tutulur. Birim dönüşümü MasterData’nın işidir. Tüm zaman damgaları `timestamptz` ve UTC’dir. Her tutarın yanında para birimi kodu durur. Kur tablosu Finance modülündedir. Denetim alanları `created_at`, `created_by`, `updated_at`, `updated_by` ve `row_version`dır.

Mimari tek fabrikadan çok şirket ve çok tesise büyümeyi baştan ister. Filtre olmadan bir liste sorgusu başka şirketin stokunu döndürür. Filtre varsayılandır; şirketler arası rapor ancak açık ve yetkili bir sorguda filtreyi kaldırır. UUID v7 zaman sıralıdır, indeks dostudur ve modüller arası kimlik için merkezi bir sıra sayacı gerektirmez. Kayan nokta, miktar ve parada kuruş ve birim sapması biriktirir. Dönüşüm formülü tek yerde, MasterData’da durur; stok satırı kendi birimindeki miktarı saklar. UTC saklama, vardiya ve tesis saatlerini birbirine karıştırmaz; arayüz kullanıcının yerel saatini gösterir. Tutar, para birimi olmadan anlamsızdır. Kur, Finance’ın sahibi olduğu veridir ve başka modül kur uydurmaz. `row_version` eşzamanlı güncellemede kayıp yazmayı engeller.

Platformun gerçekten şirket üstü katalogları, bu filtrenin belgelenmiş istisnasıdır. İstisna tesadüfi bir eksik kolonla oluşmaz.

## 10. Yerelleştirme

**Karar:** İlk diller `tr-TR` ve `en-US`dir. Metinler anahtarlıdır (`inventory.material.name`). Çeviri dosyaları `apps/web/src/locales/tr-TR.json` ve `en-US.json` içindedir. İş kuralı çevrilmiş metne bakmaz. Kodlar kalıcı ve kültürden bağımsızdır. Görünen ad, locale’e göre ayrı durur. Tarih, saat ve sayı biçimi kullanıcının locale’ine göredir. API ve veritabanı ISO 8601, UTC ve `numeric` taşır.

`docs/13_Design/99_Shared/Localization.md` iş mantığının çevrilmiş metne bağlanmasını yasaklar ve yeni dilin kod değişmeden eklenebilmesini ister. Anahtar, ekrandaki cümlenin kendisi olursa çeviri kopyası kural haline gelir. Kod sabit kalır; `docs/13_Design/99_Shared/Reference_Data.md` görünen adı locale satırında tutar. Biçim kullanıcıya aittir, saklanan değere değil. Aynı kural `docs/13_Design/99_Shared/TimeZone.md` ile uyumludur: depoda UTC, ekranda yerel saat.

**İleride eklenebilir:** `de-DE` ve Localization belgesindeki diğer diller, yeni çeviri dosyasıyla.

## Sonuç

İlk sürüm tek host, tek PostgreSQL, modül şemaları, `masterdata` istisnası, kendi JWT’si ve kalıcı süreç içi olaydır. Her iş satırı şirket, tesis, UTC zaman ve denetim taşır. .NET 10, EF Core, React ve Vite bu sınırı taşır. RabbitMQ, Redis, Keycloak, OpenIddict ve mikroservis ancak ölçülen bir ihtiyaçla eklenir.
