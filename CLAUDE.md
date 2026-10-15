# Naswood Operating System (NOS) — Claude Code

Ahşap üretimi için modüler monolit: ASP.NET Core + EF Core + PostgreSQL backend, React + Vite web. Kurallar tek yerde tutulur; bu dosya onları içe aktarır, kopyalamaz.

## Kurallar (otorite sırası)

1. Teknoloji kararları: @docs/00_Decisions/ADR-001_Technology_Stack.md — çelişkide ADR geçerlidir.
   Stok ve malzeme: @docs/00_Decisions/ADR-002_Plant_Stock_Ownership.md — Bucak stoğu NOS'ta; stok davranışı Operations ile birebir; karşı tesis stoğu salt okunur.
2. Çekirdek çalışma kuralları: @.cursor/rules/00-core.mdc
3. Backend (`src/**/*.cs` üzerinde çalışırken): @.cursor/rules/10-backend.mdc
4. Veritabanı (migration ve `*Configuration.cs` üzerinde çalışırken): @.cursor/rules/20-database.mdc

`AI/NOS_CONSTITUTION/` binlerce satırlık arka plan belgesidir; yalnızca bir karar orada aranması gerektiğinde ilgili bölümü oku, tamamını bağlama yükleme.

## Repo haritası

- `src/Naswood.OS.sln` — çözüm. `src/Hosts/Naswood.Api` tek host.
- `src/Modules/Platform` (şema `platform`), `src/Modules/Business` (şema `business`; Inventory, Production, Purchasing, Sales klasörleri).
- `src/BuildingBlocks` — ortak Domain/Application/Infrastructure/AspNetCore.
- `apps/web` — React kabuk (`@naswood/web`), `packages/ui` — bileşen kütüphanesi. pnpm workspace.
- `tests/` — birim ve entegrasyon testleri.
- `docs/` — ürün ve tasarım belgeleri. `docs/14_Implementation` dondurulmuştur (README).

## Komutlar

```bash
dotnet build src/Naswood.OS.sln
dotnet test src/Naswood.OS.sln
pnpm install
pnpm --filter @naswood/web dev          # geliştirme sunucusu
pnpm --filter @naswood/web exec vite build
pnpm --filter @naswood/web typecheck    # bilinen: mevcut tip hataları var, yenisini ekleme

dotnet tool restore                     # dotnet-ef (dotnet-tools.json)
dotnet ef migrations add <Ad> --project <Infrastructure projesi> --context <DbContext> --output-dir Persistence/Migrations
dotnet ef migrations has-pending-model-changes --project <Infrastructure projesi> --context <DbContext>
# Örnek: --project src/Modules/Business/Naswood.Modules.Business.Infrastructure --context BusinessDbContext
#        --project src/Modules/Platform/Naswood.Modules.Platform.Infrastructure --context PlatformDbContext
```

API yerelde `http://localhost:5080` (sağlık: `/health`), web `http://localhost:5173`.

## Şema değişikliği

- Şema değişikliği yalnız EF Core migration ile yapılır. `Program.cs`'e veya başka bir yere ALTER/CREATE/`EnsureCreated` ekleme.
- Her şema değişikliği PR'ında `has-pending-model-changes` iki DbContext için de temiz olmalı.
- `BusinessStartupDataFixes.cs` yalnız iki geçici veri düzeltmesi içerir (MP M2→M3, paket `WarehouseId` doldurma). Oraya yeni düzeltme eklenmez; ikisi de kaynak hata düzeltilince silinir.
- Ayrıntı: @.cursor/rules/20-database.mdc

## Git akışı

- Her görev `main`'den yeni bir dalda başlar; bitince `main`'e PR açılır. Eski `cursor/*` dallarından devam etme.
- Commit tek işe odaklanır. Görev dışı dosyaya dokunma.
- `fj-pilot-1` etiketli sürümün davranışını bozan değişiklik, testlerle gerekçelendirilmeden yapılmaz.
